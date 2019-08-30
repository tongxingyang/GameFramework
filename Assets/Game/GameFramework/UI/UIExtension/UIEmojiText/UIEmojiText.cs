using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GameFramework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension.UIEmojiText
{
    public class UIEmojiText: Text, IPointerClickHandler
    {
        public struct EmojiInfo
        {
            public float x;
            public float y;
            public float size;
        }
        
        public delegate void OnLinkTextClick(int textId, int linkId);
        private static UIVertex vert;
        private static Dictionary<string, EmojiInfo> emojiInfos;

        public static Dictionary<string, EmojiInfo> EmojiInfos
        {
            get
            {
                if (emojiInfos == null)
                {
                    emojiInfos = new Dictionary<string, EmojiInfo>();
                    TextAsset emojiContent = Resources.Load<TextAsset> ("emoji");
                    string[] lines = emojiContent.text.Split ('\n');
                    for(int i = 1 ; i < lines.Length; i ++)
                    {
                        if (! string.IsNullOrEmpty (lines [i])) {
                            string[] strs = lines [i].Split ('\t');
                            EmojiInfo info;
                            info.x = float.Parse (strs [1]);
                            info.y = float.Parse (strs [2]);
                            info.size = float.Parse (strs [3]);
                            emojiInfos.Add (strs [0], info);
                        }
                    }
                }
                return emojiInfos;
            }
        }
        
        private const string EMOTIONTAG = "e";
        private const string LINKTAG = "l";
        private const string TEXTQUAD = "<quad name={0} size={1} width={2} />";
        private static readonly Regex TextRegex = new Regex(@"\[([el])\](.+?)\[\-\]", RegexOptions.Singleline);
        private static readonly Regex LinkRegex = new Regex(@"^(#[abcdefABCDEF0-9]{6}) (.+?)$", RegexOptions.Singleline);
        private List<UIEmotionTextModel> _GraphicItemModels = new List<UIEmotionTextModel>();
        private List<UILinkTextModel> _TextLinkModels = new List<UILinkTextModel>();
        private UIEmojiTextGraphicBoard _GraphicBoard;
        private string originText = string.Empty;
        private string buildText = string.Empty;
        private int textID = 0;
        private Vector3 graphicOffset = Vector3.zero;
        private float graphicScale = 1f;
        private StringBuilder stringBuilder;
    
        public string OriginText
        {
            get { return originText; }
        }
        public OnLinkTextClick OnClick = null;
        public int TextID { set { textID = value; } }
    
        public Vector3 GraphicOffset
        {
            get { return graphicOffset; }
            set { graphicOffset = value; }
        }
    
        public float GraphicScale
        {
            get { return graphicScale; }
            set { graphicScale = value; }
        }
        
        protected override void Awake()
        {
            this.alignByGeometry = false;
            if (rectTransform.childCount <= 0)
            {
                _GraphicBoard = new GameObject("_emojiGraphicBoard").AddComponent<UIEmojiTextGraphicBoard>();
                _GraphicBoard.rectTransform.SetParent(transform, false);
            }
            else
            {
                _GraphicBoard = GetComponentInChildren<UIEmojiTextGraphicBoard>();
                if (_GraphicBoard == null)
                {
                    _GraphicBoard = rectTransform.GetChild(0).gameObject.AddComponent<UIEmojiTextGraphicBoard>();
                }
            }
            FormatBoard();
        }
    
        public void FormatBoard()
        {
            if (_GraphicBoard == null) return;
            _GraphicBoard.rectTransform.anchorMin = Vector2.zero;
            _GraphicBoard.rectTransform.anchorMax = Vector2.one;
            _GraphicBoard.rectTransform.offsetMin = Vector2.zero;
            _GraphicBoard.rectTransform.offsetMax = Vector2.zero;
            _GraphicBoard.rectTransform.pivot = rectTransform.pivot;
        }
    
        public override void SetVerticesDirty()
        {
            originText = m_Text;
            MatchCollection collections = TextRegex.Matches(originText);
            _GraphicItemModels.Clear();
            _TextLinkModels.Clear();
            int last_index = 0;
            if (stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
            }
            stringBuilder.Length = 0;
    
            for (var i = 0; i < collections.Count; i++)
            {
                Match match = collections[i];
                int match_index = match.Index;
                string type = match.Groups[1].Value;
                if (type == EMOTIONTAG)
                {
                    stringBuilder.Append(originText.Substring(last_index, match_index - last_index));
                    var start_index = stringBuilder.Length;
                    string quad = StringUtility.Format(TEXTQUAD, match.Groups[2].Value, (int)(fontSize), (GraphicScale));
                    stringBuilder.Append(quad);
                    UIEmotionTextModel model = new UIEmotionTextModel
                    {
                        TextIndex = start_index,
                        SpriteName = match.Groups[2].Value
                    };
                    EmojiInfo emojiInfo = EmojiInfos[model.SpriteName];
                    model.X = emojiInfo.x;
                    model.Y = emojiInfo.y;
                    model.Size = emojiInfo.size;
                    _GraphicItemModels.Add(model);
                }
                else if (type == LINKTAG)
                {
                    string seg = originText.Substring(last_index, match_index - last_index);
                    stringBuilder.Append(seg);
                    UILinkTextModel linkModel = new UILinkTextModel();
                    string inner_txt;
                    string color_text;
                    MatchCollection collections_con = LinkRegex.Matches(match.Groups[2].Value);
                    if (collections_con.Count == 1 && collections_con[0].Groups.Count == 3)
                    {
                        color_text = collections_con[0].Groups[1].Value;
                        inner_txt = collections_con[0].Groups[2].Value;
                    }
                    else
                    {
                        color_text = "#0066cc";
                        inner_txt = match.Groups[2].Value;
                    }
    
                    stringBuilder.AppendFormat("<color={0}>", color_text);
                    linkModel.StartIndex = stringBuilder.Length;
                    stringBuilder.AppendFormat(inner_txt);
                    linkModel.EndIndex = stringBuilder.Length - 1;
                    stringBuilder.Append("</color>");
                    _TextLinkModels.Add(linkModel);
                }
                last_index = match_index + match.Value.Length;
            }
            stringBuilder.Append(originText.Substring(last_index, originText.Length - last_index));
            buildText = stringBuilder.ToString();
            base.SetVerticesDirty();
        }
        
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            m_Text = buildText;
            base.OnPopulateMesh(toFill);
            int i, j, startIndex;
            for (i = 0; i < _GraphicItemModels.Count; i++)
            {
                UIEmotionTextModel model = _GraphicItemModels[i];
                startIndex = model.TextIndex * 4;
                for (j = 0; j < 4; j++)
                {
                    var indics = startIndex + j;
                    if (indics >= toFill.currentVertCount) break;
    
                    toFill.PopulateUIVertex(ref vert, indics);
                    vert.uv0 = vert.uv1 = Vector2.zero;
                    vert.position += new Vector3(GraphicOffset.x, GraphicOffset.y * 0.5f, GraphicOffset.z);
    
                    toFill.SetUIVertex(vert, indics);
    
                    if (j == 0)
                        model.PosMin = vert.position + new Vector3(0, (GraphicScale - 1) / 2 * fontSize, 0);
                    if (j == 2)
                        model.PosMax = vert.position + new Vector3(0, -(GraphicScale - 1) / 2 * fontSize, 0);
                }
            }
            
            if (_GraphicBoard != null)
            {
                _GraphicBoard.SetVertexData(_GraphicItemModels);
            }
    
            for (i = 0; i < _TextLinkModels.Count; i++)
            {
                UILinkTextModel linkModel = _TextLinkModels[i];
                startIndex = linkModel.StartIndex * 4;
                int endIndex = linkModel.EndIndex * 4 + 3;
                if (startIndex >= toFill.currentVertCount)
                {
                    continue;
                }
                toFill.PopulateUIVertex(ref vert, startIndex);
                Vector3 pos = vert.position;
                Vector3 last_char = pos;
                Bounds bounds = new Bounds(pos, Vector3.zero);
                for (j = startIndex + 2; j < endIndex; j += 2)
                {
                    if (j >= toFill.currentVertCount)
                    {
                        break;
                    }
                    toFill.PopulateUIVertex(ref vert, j);
                    pos = vert.position;
                    if (j % 4 == 0)
                    {
                        if (pos.x < last_char.x)
                        {
                            if (bounds.size.x > 0)
                            {
                                linkModel.AddRect(new Rect(bounds.min, bounds.size));
                            }
                            bounds = new Bounds(pos, Vector3.zero);
                        }
                        else
                        {
                            bounds.Encapsulate(pos);
                        }
                        last_char = pos;
                    }
                    else
                    {
                        bounds.Encapsulate(pos);
                    }
                }
    
                if (bounds.size.x > 0)
                {
                    linkModel.AddRect(new Rect(bounds.min, bounds.size));
                }
            }
            m_Text = originText;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick == null) return;
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);
            var clickLinkId = -1;
            for (int i = 0; i < _TextLinkModels.Count; i++)
            {
                UILinkTextModel linkModel = _TextLinkModels[i];
                for (int j = 0; j < linkModel.Rects.Count; ++j)
                {
                    if (linkModel.Rects[j].Contains(lp))
                    {
                        clickLinkId = i;
                        break;
                    }
                }
            }
            if (clickLinkId != -1)
            {
                OnClick.Invoke(textID, clickLinkId + 1);
            }
        }
    
        public override float preferredWidth
        {
            get
            {
                var settings = GetGenerationSettings(Vector2.zero);
                return cachedTextGeneratorForLayout.GetPreferredWidth(buildText, settings) / pixelsPerUnit;
            }
        }
    
        public override float preferredHeight
        {
            get
            {
                var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
                return cachedTextGeneratorForLayout.GetPreferredHeight(buildText, settings) / pixelsPerUnit;
            }
        }
    }

}