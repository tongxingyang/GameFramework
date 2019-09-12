#if I2_TMPro
/**************************************************************
 * If you are getting errors in this file:
 * 
 * - Check that you have TextMeshPro installed or you are using Unity 2017+
 *
 * - If you don't have TMP installed, then remove I2_TMPro from 
 *      your "Scripting Define Symbols"  (Unity Editor Menu => Edit \ Project Settings \ Player)
 *      
 * - If TMP is installed, then add I2_TMPro to the Scripting Define Symbols
 * 
 *      
 *  HELP: http://inter-illusion.com/assets/I2TextAnimationManual/TMProIntegration.html    
 *      
 ***************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

namespace I2.TextAnimation
{
	public partial class TextAnimation
	{
		[NonSerialized]
		TMP_Text mTMP_Label;

		protected void OnEnableTMPro()
		{
			mTMP_Label = GetComponent<TMP_Text>();
			EnableTextModification_TMPro();
		}

		protected void OnDisableTMPro()
		{
			DisableTextModification_TMPro();
		}

		public void MarkWidgetAsChanged_TMPro(bool MarkVertices = true, bool MarkMaterial = false)
		{
			if (mTMP_Label != null)
			{
				mTMP_Label.SetAllDirty();
			}
		}

		void SetWidgetColor_TMPro( Color32 color )
		{
			if (mTMP_Label!=null)
				mTMP_Label.color = color;
		}




		protected void EnableTextModification_TMPro ()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add( OnTextChanged_TMPro );
		}

		protected void DisableTextModification_TMPro ()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove( OnTextChanged_TMPro );
		}

		void OnTextChanged_TMPro ( UnityEngine.Object obj )
		{
			if (obj!=mTMP_Label)
				return;

			if (!isActiveAndEnabled) return;
			var textInfo = mTMP_Label.textInfo;

            if (textInfo.characterCount == 0)
                return;

			if (mRectTransform == null) mRectTransform = transform as RectTransform;
			if (mRectTransform == null) return;

			mRectTransform = mTMP_Label.rectTransform;
			mRect = mRectTransform.rect;
			mRectPivot = mRectTransform.pivot;
			mWidgetColor = mTMP_Label.color;

			mCharacterSize = mTMP_Label.fontSize;
			mLineHeight = (textInfo.lineInfo.Length>0) ? textInfo.lineInfo[0].lineHeight : mCharacterSize;


			//--[ Characters ]--------------------------------------

			mAllCharactersMin.x = mAllCharactersMin.y = float.MaxValue;
			mAllCharactersMax.x = mAllCharactersMax.y = float.MinValue;
			mCharacters.Reset(textInfo.characterCount);


			int iLine = 0, nCharactersInLine = 0, iCharacter = 0;
			int iWord = 0, nWordsInLine = 0;
			int iParagraph = 0;

			int iFirstCharNextLine = 0;
			int iFirstCharNextWord = 0;

            mOriginalVertices.Clear();
            mOriginalVertices.Reset( 4 * textInfo.characterCount );
            int iVert=0;

            for (int c=0; c< textInfo.characterCount; ++c)
			{
				if (!textInfo.characterInfo[c].isVisible)
					continue;

				mCharacters.Buffer[iCharacter].Min = textInfo.characterInfo[c].bottomLeft;
				mCharacters.Buffer[iCharacter].Max = textInfo.characterInfo[c].topRight;
				if (mAllCharactersMin.x > mCharacters.Buffer[iCharacter].Min.x) mAllCharactersMin.x = mCharacters.Buffer[iCharacter].Min.x;
				if (mAllCharactersMin.y > mCharacters.Buffer[iCharacter].Min.y) mAllCharactersMin.y = mCharacters.Buffer[iCharacter].Min.y;
				if (mAllCharactersMax.x < mCharacters.Buffer[iCharacter].Max.x) mAllCharactersMax.x = mCharacters.Buffer[iCharacter].Max.x;
				if (mAllCharactersMax.y < mCharacters.Buffer[iCharacter].Max.y) mAllCharactersMax.y = mCharacters.Buffer[iCharacter].Max.y;

				if (c >= iFirstCharNextLine)
				{
					iFirstCharNextLine = textInfo.lineInfo[iLine].lastCharacterIndex+1;
					iLine++;
					nCharactersInLine = 0;
					nWordsInLine = -1;
					iFirstCharNextWord = c-1;
				}

				if (c >= iFirstCharNextWord)
				{
					iFirstCharNextWord = textInfo.wordInfo.Length>iWord ? textInfo.wordInfo[iWord].lastCharacterIndex + 1 : int.MaxValue;
					iWord++;
					nWordsInLine++;
				}

                // Find the mesh and vertex
                int materialIndex = textInfo.characterInfo[c].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[c].vertexIndex;

                for (int i = 0; i < 4; ++i)
                {
                    seVertex.position    = textInfo.meshInfo[materialIndex]. vertices   [vertexIndex + i];
                    seVertex.color       = textInfo.meshInfo[materialIndex]. colors32   [vertexIndex + i];
                    seVertex.uv          = textInfo.meshInfo[materialIndex]. uvs0       [vertexIndex + i];
                    seVertex.normal      = textInfo.meshInfo[materialIndex]. normals    [vertexIndex + i];
                    seVertex.tangent     = textInfo.meshInfo[materialIndex]. tangents   [vertexIndex + i];
                    seVertex.characterID = c;

                    mOriginalVertices.Buffer[iVert++] = seVertex;
                }



                mCharacters.Buffer[iCharacter].Character = textInfo.characterInfo[c].character;
				mCharacters.Buffer[iCharacter].iLine = iLine;
				mCharacters.Buffer[iCharacter].iCharacterInText = iCharacter;
				mCharacters.Buffer[iCharacter].iCharacterInLine = nCharactersInLine++;
				mCharacters.Buffer[iCharacter].iWord = iWord;
				mCharacters.Buffer[iCharacter].iWordInLine = nWordsInLine;
				mCharacters.Buffer[iCharacter].iParagraph = iParagraph;
				mCharacters.Buffer[iCharacter].TopY       = textInfo.lineInfo.Length>iLine ? textInfo.lineInfo[iLine].lineExtents.min.y : mCharacters.Buffer[iCharacter].Min.y;
				mCharacters.Buffer[iCharacter].BaselineY  = textInfo.lineInfo.Length>iLine ? textInfo.lineInfo[iLine].lineExtents.max.y : mCharacters.Buffer[iCharacter].Max.y;
				iCharacter++;
			}
			mCharacters.Size = iCharacter;

			ModifyVertices();

			ExportVerticesToTMPro(textInfo);

			mTMP_Label.UpdateVertexData();
		}

		private void ExportVerticesToTMPro( TMP_TextInfo textInfo )
		{
            int iVert = 0;
            for (int c = 0; c < textInfo.characterCount; ++c)
            {
                if (!textInfo.characterInfo[c].isVisible)
                    continue;

                // Find the mesh and vertex
                int materialIndex = textInfo.characterInfo[c].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[c].vertexIndex;

                for (int i = 0; i < 4; ++i)
                {
                    textInfo.meshInfo[materialIndex].vertices[vertexIndex + i]  = mOriginalVertices.Buffer[iVert].position;
                    textInfo.meshInfo[materialIndex].colors32[vertexIndex + i]  = mOriginalVertices.Buffer[iVert].color;
                    textInfo.meshInfo[materialIndex].uvs0[vertexIndex + i]      = mOriginalVertices.Buffer[iVert].uv;
                    textInfo.meshInfo[materialIndex].normals[vertexIndex + i]   = mOriginalVertices.Buffer[iVert].normal;
                    textInfo.meshInfo[materialIndex].tangents[vertexIndex + i]  = mOriginalVertices.Buffer[iVert].tangent;
                    iVert++;
                }

                //{
                //    textInfo.meshInfo[meshIndex].triangles[indTri++] = i;
                //    textInfo.meshInfo[meshIndex].triangles[indTri++] = i + 1;
                //    textInfo.meshInfo[meshIndex].triangles[indTri++] = i + 2;

                //    textInfo.meshInfo[meshIndex].triangles[indTri++] = i + 2;
                //    textInfo.meshInfo[meshIndex].triangles[indTri++] = i + 3;
                //    textInfo.meshInfo[meshIndex].triangles[indTri++] = i;
                //}
            }
        }
	}
}
#endif
