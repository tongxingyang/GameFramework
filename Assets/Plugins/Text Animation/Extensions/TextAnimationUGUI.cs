using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace I2.TextAnimation
{
    #if UNITY_5_3_OR_NEWER || UNITY_5_3 || UNITY_5_2
    public partial class TextAnimation : IMeshModifier
	{
        public void ModifyMesh(Mesh mesh)
        {
            if (!enabled) return;

            //if (!this.IsActive()) return;

            using (VertexHelper vertexHelper = new VertexHelper(mesh))
            {
                this.ModifyMesh(vertexHelper);
                vertexHelper.FillMesh(mesh);
            }
        }

        public void ModifyMesh (VertexHelper vh)
		{
            if (!isActiveAndEnabled) return;

			//--[ Cache ]------------
			if (mGraphic==null)	mGraphic=GetComponent<Graphic>();
			if (mGraphic==null)	return;

			if (mRectTransform==null) mRectTransform = transform as RectTransform;
			if (mRectTransform==null) return;

            if (vh.currentVertCount == 0)
                return;

			mRect = mRectTransform.rect;
			mRectPivot = mRectTransform.pivot;
			mWidgetColor = mGraphic.color;

            if (vh.currentVertCount<=0 || (GetPlayingAnimation()==null))
                return;

            ImportVerticesFromUGUI(vh);

            ModifyVertices();

            ExportVerticesToUGUI(vh);

        }

        private void ImportVerticesFromUGUI(VertexHelper vh)
        {
            Text label = mGraphic as Text;
			mCharacterSize = (label == null ? 1 : (float)label.fontSize);
			mLineHeight = mCharacterSize;

			var fontScale = (label == null ? 1 : (mCharacterSize / (float)label.font.fontSize));
			var ascender = (label == null ? mLineHeight : (label.font.ascent*fontScale));

            string strText = (label==null ? null : label.text);
            float Pixel2Units = (label == null ? 1 : 1/label.pixelsPerUnit);

            //--[ Populate Original Mesh ]-----------------

			mOriginalVertices.Clear();


            mOriginalVertices.Reset(vh.currentVertCount);
            mCharacters.Reset(mOriginalVertices.Size / 4);

            mAllCharactersMin = MathUtils.v2max;
            mAllCharactersMax = MathUtils.v2min;
            var cIndex = 0;

            int iLine = 0, nCharactersInLine=0;
            int iWord = 0, nWordsInLine = 0;
            int iParagraph = 0;
            bool inWord = false;
            IList<UILineInfo> lineInfos = null;

            if (label != null)
            {
                if (label.cachedTextGeneratorForLayout.characterCount != strText.Length)
                    label.cachedTextGeneratorForLayout.Populate(strText, label.GetGenerationSettings(mRect.size));
                lineInfos = label.cachedTextGeneratorForLayout.lines;
            }
            //else
              //  mLines.Clear();

            var iFirstCharOfNextLine = (lineInfos == null || lineInfos.Count <= 1) ? int.MaxValue : lineInfos[1].startCharIdx;
            float lineHeight = (lineInfos == null || lineInfos.Count <= 0) ? 1 : lineInfos[0].height*Pixel2Units;
            float linePosY = (lineInfos == null || lineInfos.Count <= 0) ? 0 : lineInfos[0].topY * Pixel2Units;

            // The array is cleared to initialize all RichText values
            System.Array.Clear(mCharacters.Buffer, 0, mCharacters.Size);

            for (int c = 0; c < mCharacters.Size; ++c)
            {
                mCharacters.Buffer[cIndex].Min = MathUtils.v2max;
                mCharacters.Buffer[cIndex].Max = MathUtils.v2min;

                for (int i = 0; i < 4; i++)
                {
                    vh.PopulateUIVertex(ref uiVertex, c * 4 + i);

                    seVertex.position    = uiVertex.position;
                    seVertex.color       = uiVertex.color;
                    seVertex.uv          = uiVertex.uv0;
                    seVertex.normal      = uiVertex.normal;
                    seVertex.tangent     = uiVertex.tangent;
                    seVertex.characterID = cIndex;

                    if (mCharacters.Buffer[cIndex].Min.x > uiVertex.position.x) mCharacters.Buffer[cIndex].Min.x = uiVertex.position.x;
                    if (mCharacters.Buffer[cIndex].Min.y > uiVertex.position.y) mCharacters.Buffer[cIndex].Min.y = uiVertex.position.y;
                    if (mCharacters.Buffer[cIndex].Max.x < uiVertex.position.x) mCharacters.Buffer[cIndex].Max.x = uiVertex.position.x;
                    if (mCharacters.Buffer[cIndex].Max.y < uiVertex.position.y) mCharacters.Buffer[cIndex].Max.y = uiVertex.position.y;

                    mOriginalVertices.Buffer[cIndex*4 + i] = seVertex;
                }

                char chr = strText == null ? (char)0 : strText[c];
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
                        lineHeight = lineInfos[iLine].height * Pixel2Units;
                        linePosY = lineInfos[iLine].topY * Pixel2Units;
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
                    mCharacters.Buffer[cIndex].TopY = linePosY;
                    mCharacters.Buffer[cIndex].BaselineY = linePosY - ascender;
                    mCharacters.Buffer[cIndex].Height = lineHeight;

                    cIndex++;
                }
            }
            mCharacters.Size = cIndex;
            mOriginalVertices.Size = cIndex*4;
        }

        private void ExportVerticesToUGUI(VertexHelper vh)
        {
            vh.Clear();
            var ioffset = 0;

            for (int i = 0; i < mOriginalVertices.Size; ++i)
                vh.AddVert(mOriginalVertices.Buffer[i].position, mOriginalVertices.Buffer[i].color, mOriginalVertices.Buffer[i].uv, mOriginalVertices.Buffer[i].uv1, mOriginalVertices.Buffer[i].normal, mOriginalVertices.Buffer[i].tangent);

            for (int i = 0; i < mOriginalVertices.Size; i += 4)
            {
                vh.AddTriangle(ioffset + i, ioffset + i + 1, ioffset + i + 2);
                vh.AddTriangle(ioffset + i + 2, ioffset + i + 3, ioffset + i);
            }
        }



#else
    public partial class TextAnimation
    {
    #endif
        [NonSerialized]public Graphic mGraphic;
		RectTransform mRectTransform;

		static UIVertex uiVertex = default(UIVertex);

        void OnEnableUGUI()
        {
            if (mGraphic == null) mGraphic = GetComponent<Graphic>();
        }

        void OnDisableUGUI()
        {
        }

        // When useVertices==true, it updates the Character min and max from its vertices and then updates the mAllCharacterMinMax
        // otherwise, it just updates the mAllCharacterMinMax from the min/max of each character
        public void UpdateCharactersMinMax( bool useVertices=true )
        {
            if (useVertices)
            {
                for (var i = 0; i < mOriginalVertices.Size; i++)
                {
                    var cIndex = i / 4;
                    if (i % 4 == 0)
                    {
                        mCharacters.Buffer[cIndex].Min = MathUtils.v2max;
                        mCharacters.Buffer[cIndex].Max = MathUtils.v2min;
                    }
                    var pos = mOriginalVertices.Buffer[i].position;

                    if (mCharacters.Buffer[cIndex].Min.x > pos.x) mCharacters.Buffer[cIndex].Min.x = pos.x;
                    if (mCharacters.Buffer[cIndex].Min.y > pos.y) mCharacters.Buffer[cIndex].Min.y = pos.y;
                    if (mCharacters.Buffer[cIndex].Max.x < pos.x) mCharacters.Buffer[cIndex].Max.x = pos.x;
                    if (mCharacters.Buffer[cIndex].Max.y < pos.y) mCharacters.Buffer[cIndex].Max.y = pos.y;
                }
            }

            mAllCharactersMin = MathUtils.v2max;
            mAllCharactersMax = MathUtils.v2min;
            for (var c = 0; c < mCharacters.Size; c++)
            {
                if (mCharacters.Buffer[c].Min.x < mAllCharactersMin.x) mAllCharactersMin.x = mCharacters.Buffer[c].Min.x;
                if (mCharacters.Buffer[c].Min.y < mAllCharactersMin.y) mAllCharactersMin.y = mCharacters.Buffer[c].Min.y;
                if (mCharacters.Buffer[c].Max.x > mAllCharactersMax.x) mAllCharactersMax.x = mCharacters.Buffer[c].Max.x;
                if (mCharacters.Buffer[c].Max.y > mAllCharactersMax.y) mAllCharactersMax.y = mCharacters.Buffer[c].Max.y;
            }

        }

		void SetWidgetColor_UGUI ( Color32 color )
		{
			if (mGraphic!=null)
				mGraphic.color = color;
		}
    }
}
