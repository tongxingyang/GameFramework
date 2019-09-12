using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace I2.TextAnimation
{
#if UNITY_5_3_OR_NEWER || UNITY_5_3 || UNITY_5_2
#else
    public partial class TextAnimation : IVertexModifier
	{
        public void ModifyVertices(List<UIVertex> verts)
		{ 
			if (!enabled)
				return;

            //using (VertexHelper vertexHelper = new VertexHelper(mesh))

            //--[ Cache ]------------
            if (mGraphic == null) mGraphic = GetComponent<Graphic>();
            if (mGraphic == null) return;

            if (mRectTransform == null) mRectTransform = transform as RectTransform;
            if (mRectTransform == null) return;

            mRect = mRectTransform.rect;
            mRectPivot = mRectTransform.pivot;
            mWidgetColor = mGraphic.color;

            if (verts.Count == 0 || (mWidgetColor.a <= 1/255f && GetPlayingAnimation()==null))
                return;

            ImportVerticesFromUGUI(verts);

            ModifyVertices();

            ExportVerticesToUGUI(ref verts);
		}

        private void ImportVerticesFromUGUI( List<UIVertex> verts)
        {
            Text label = mGraphic as Text;
            mCharacterSize = (label == null ? 1 : (float)label.fontSize);
            mLineHeight = mCharacterSize;
            var fontScale = (label == null ? 1 : (mCharacterSize / (float)label.font.fontSize));
            var ascender = (label == null ? mLineHeight : (label.font.ascent*fontScale));

            string strText = (label == null ? null : label.text);
            int textLength = strText == null ? 0 : strText.Length;
            float Pixel2Units = (label == null ? 1 : 1 / label.pixelsPerUnit);

            //float yOffset = (label != null && label.font != null) ? -label.font.ascent * (mCharacterSize / (float)label.font.fontSize) : 0;

            //--[ Populate Original Mesh ]-----------------

			mOriginalVertices.Clear();

            int nVerts = verts.Count;
            mOriginalVertices.Reset(nVerts);
            mCharacters.Reset(nVerts / 4);

            mAllCharactersMin = MathUtils.v2max;
            mAllCharactersMax = MathUtils.v2min;
            var cIndex = 0;

            int iLine = 0, nCharactersInLine = 0;
            int iWord = 0, nWordsInLine = 0;
            int iParagraph = 0;
            bool inWord = false;
            IList<UILineInfo> lineInfos = null;


            if (label != null)
            {
                if (label.cachedTextGeneratorForLayout.characterCount != strText.Length)
                    label.cachedTextGeneratorForLayout.Populate(strText, label.GetGenerationSettings(mRect.size));
                lineInfos = label.cachedTextGeneratorForLayout.lines;

                if (lineInfos.Count > 0)
                    mLineHeight = lineInfos[0].height * Pixel2Units;
            }
            //else
            //  mLines.Clear();

            var iFirstCharOfNextLine = (lineInfos == null || lineInfos.Count <= 1) ? int.MaxValue : lineInfos[1].startCharIdx;
            float lineHeight = (lineInfos == null || lineInfos.Count <= 0) ? mLineHeight : lineInfos[0].height * Pixel2Units;
            //float linePosY = //(lineInfos == null || lineInfos.Count <= 0) ? 0 : lineInfos[0].height/* * lineInfos.Count*/ * Pixel2Units;
            float linePosY = mRect.yMax; 

            for (int c = 0; c < mCharacters.Size; ++c)
            {
                mCharacters.Buffer[cIndex].Min = MathUtils.v2max;
                mCharacters.Buffer[cIndex].Max = MathUtils.v2min;

                for (int i = 0; i < 4; i++)
                {
                    uiVertex = verts[c * 4 + i];
                    //uiVertex.position.y += yOffset;

                    seVertex.position = uiVertex.position;
                    seVertex.color = uiVertex.color;
                    seVertex.uv = uiVertex.uv0;
                    seVertex.normal = uiVertex.normal;
                    seVertex.tangent = uiVertex.tangent;
                    seVertex.characterID = cIndex;

                    if (mCharacters.Buffer[cIndex].Min.x > uiVertex.position.x) mCharacters.Buffer[cIndex].Min.x = uiVertex.position.x;
                    if (mCharacters.Buffer[cIndex].Min.y > uiVertex.position.y) mCharacters.Buffer[cIndex].Min.y = uiVertex.position.y;
                    if (mCharacters.Buffer[cIndex].Max.x < uiVertex.position.x) mCharacters.Buffer[cIndex].Max.x = uiVertex.position.x;
                    if (mCharacters.Buffer[cIndex].Max.y < uiVertex.position.y) mCharacters.Buffer[cIndex].Max.y = uiVertex.position.y;

                    mOriginalVertices.Buffer[cIndex * 4 + i] = seVertex;
                }

                char chr = c >= textLength ? (char)0 : strText[c];

                bool isWhiteSpace = char.IsWhiteSpace(chr);
                if (inWord && (isWhiteSpace || c >= iFirstCharOfNextLine))  // is a new word if we reach a whitespace or started another line
                {
                    iWord++;
                    nWordsInLine++;
                }
                inWord = !isWhiteSpace;
                if (chr == '\n')
                    iParagraph++;

                while (c >= iFirstCharOfNextLine)
                {
                    iLine++;
                    nCharactersInLine = 0;
                    nWordsInLine = 0;
                    iFirstCharOfNextLine = (lineInfos.Count <= (iLine + 1)) ? int.MaxValue : lineInfos[iLine + 1].startCharIdx;

                    if (lineInfos != null && lineInfos.Count > iLine)
                    {
                        linePosY -= lineHeight;
                        lineHeight = lineInfos[iLine].height * Pixel2Units;
                    }
                }

                if (mCharacters.Buffer[cIndex].Min.x < mCharacters.Buffer[cIndex].Max.x && mCharacters.Buffer[cIndex].Min.y < mCharacters.Buffer[cIndex].Max.y)
                {
                    if (mCharacters.Buffer[cIndex].Min.x < mAllCharactersMin.x) mAllCharactersMin.x = mCharacters.Buffer[cIndex].Min.x;
                    if (mCharacters.Buffer[cIndex].Min.y < mAllCharactersMin.y) mAllCharactersMin.y = mCharacters.Buffer[cIndex].Min.y;
                    if (mCharacters.Buffer[cIndex].Max.x > mAllCharactersMax.x) mAllCharactersMax.x = mCharacters.Buffer[cIndex].Max.x;
                    if (mCharacters.Buffer[cIndex].Max.y > mAllCharactersMax.y) mAllCharactersMax.y = mCharacters.Buffer[cIndex].Max.y;

                    mCharacters.Buffer[cIndex].Character = chr;
                    mCharacters.Buffer[cIndex].iLine = iLine;
                    mCharacters.Buffer[cIndex].iCharacterInText = cIndex;
                    mCharacters.Buffer[cIndex].iCharacterInLine = nCharactersInLine++;
                    mCharacters.Buffer[cIndex].iWord = iWord;
                    mCharacters.Buffer[cIndex].iWordInLine = nWordsInLine;
                    mCharacters.Buffer[cIndex].iParagraph = iParagraph;
                    mCharacters.Buffer[cIndex].BaselineY = linePosY;
                    mCharacters.Buffer[cIndex].TopY = linePosY-ascender;
                    mCharacters.Buffer[cIndex].Height = lineHeight;

                    cIndex++;
                }
            }
            mCharacters.Size = cIndex;
            mOriginalVertices.Size = cIndex * 4;
        }

        private void ExportVerticesToUGUI(ref List<UIVertex> verts)
        {
            int nVerts = mOriginalVertices.Size;

            //--[ Import RenderMesh into verts ]-------------------------
            verts.Clear();
            verts.Capacity = nVerts;

            for (int i = 0; i < mOriginalVertices.Size; ++i)
            {
                uiVertex.position = mOriginalVertices.Buffer[i].position;
                uiVertex.color    = mOriginalVertices.Buffer[i].color;
                uiVertex.uv0      = mOriginalVertices.Buffer[i].uv;
                uiVertex.uv1      = mOriginalVertices.Buffer[i].uv1;
                uiVertex.normal   = mOriginalVertices.Buffer[i].normal;
                uiVertex.tangent  = mOriginalVertices.Buffer[i].tangent;
                verts.Add(uiVertex);
            }
        }
    }
#endif
}
