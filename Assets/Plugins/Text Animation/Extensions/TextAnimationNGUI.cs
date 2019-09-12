using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

#if I2_NGUI


/**************************************************************
 * If you are getting errors in this file:
 * 
 * - Check that you have NGUI installed in your project
 *
 * - If you don't have NGUI installed, then remove I2_NGUI from 
 *      your "Scripting Define Symbols"  (Unity Editor Menu => Edit \ Project Settings \ Player)
 *      
 * - If NGUI is installed, then add I2_NGUI to the Scripting Define Symbols
 * - Move The Assets\Plugin\I2 folder to Assets folder  (Assets\I2)
 * 
 *                   Requires NGUI 3.11.0 or higher
 *      
 *  HELP: http://inter-illusion.com/assets/I2TextAnimationManual/NGUIIntegration.html    
 *      
 ***************************************************************/


namespace I2.TextAnimation
{
	public partial class TextAnimation
	{
		[System.NonSerialized]
		public UIWidget mNGUI_Widget;


		protected void OnEnableNGUI()
		{
			mNGUI_Widget = GetComponent<UIWidget>();
			if (mNGUI_Widget != null)
			{
				mNGUI_Widget.geometry.onCustomWrite += NGUI_ModifyVertices;
			}
		}

		protected void OnDisableNGUI()
		{
			if (mNGUI_Widget != null)
			{
				mNGUI_Widget.geometry.onCustomWrite -= NGUI_ModifyVertices;
			}
		}

		void SetWidgetColor_NGUI( Color32 color )
		{
			if (mNGUI_Widget!=null)
				mNGUI_Widget.color = color;
		}

		public void MarkWidgetAsChanged_NGUI(bool MarkVertices = true, bool MarkMaterial = false)
		{
			if (mNGUI_Widget)
			{
				if (MarkMaterial)
					mNGUI_Widget.RemoveFromPanel();
				mNGUI_Widget.MarkAsChanged();
			}
		}

		public void NGUI_ModifyVertices(List<Vector3> vertices, List<Vector2> uvs, List<Color> colors, List<Vector3> normals, List<Vector4> tangents, List<Vector4> uvs2)
		{
			//--[ Cache ]------------

			mWidgetColor = mNGUI_Widget.color;
			if (mWidgetColor.a < 1/255f && GetPlayingAnimation()==null)
				return;

			Bounds bounds = mNGUI_Widget.CalculateBounds();
			mRect = Rect.MinMaxRect(bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);
			mRectPivot.x = Mathf.InverseLerp(mRect.min.x, mRect.max.x, mRect.center.x);
			mRectPivot.y = Mathf.InverseLerp(mRect.min.y, mRect.max.y, mRect.center.y);

			UILabel label = mNGUI_Widget as UILabel;
			if (label != null)
			{
				mCharacterSize = label.printedSize.y;// fontSize;
				mLineHeight = label.height;
			}
			else
				mCharacterSize = mLineHeight = 1;

			ImportVerticesFromNGUI(vertices, uvs, colors, normals, tangents, uvs2);
			ModifyVertices();
			ExportVerticesToNGUI(vertices, uvs, colors, normals, tangents, uvs2);
		}

		void ImportVerticesFromNGUI(List<Vector3> vertices, List<Vector2> uvs, List<Color> colors, List<Vector3> normals, List<Vector4> tangents, List<Vector4> uvs2)
		{
			int nVerts = mNGUI_Widget.geometry.verts.Count;
			int iFirstVert = vertices.Count - nVerts;

			int iLine = 0, nCharactersInLine = 0;
			int iWord = 0, nWordsInLine = 0;
			int iParagraph = 0;
			UILabel label = mNGUI_Widget as UILabel;
			string strText = (label == null ? null : label.processedText);
			string orgText = (label == null) ? strText : label.text;
			int iOrgChar = 0;
			char orgChr = (char)0;


			mOriginalVertices.Reset(nVerts);
			mCharacters.Reset(mOriginalVertices.Size / 4);

			mAllCharactersMin = MathUtils.v2max;
			mAllCharactersMax = MathUtils.v2min;
			var cIndex = 0;
			//var MatrixWidgetToPanel = mNGUI_Widget.panel.worldToLocal * mNGUI_Widget.cachedTransform.localToWorldMatrix;
			var MatrixPanelToWidget = mNGUI_Widget.cachedTransform.worldToLocalMatrix * mNGUI_Widget.panel.cachedTransform.localToWorldMatrix;
			for (int c = 0; c < strText.Length; ++c)
			{
				char chr = (strText == null || strText.Length<=c) ? (char)0 : strText[c];
				bool orgHasWhiteSpace = false;
				while (iOrgChar < orgText.Length && orgChr != chr)
				{
					orgChr = orgText[iOrgChar];
					orgHasWhiteSpace |= char.IsWhiteSpace(orgChr);

					iOrgChar++;
					if (chr == '\n')
						break;
				}

				bool isWhiteSpace = char.IsWhiteSpace( chr );
				if (orgHasWhiteSpace && !isWhiteSpace)
				{
					iWord++;
					nWordsInLine++;
				}
				if (chr == '\n' && orgChr=='\n')
					iParagraph++;

				if (chr == '\n')
				{
					iLine++;
					nCharactersInLine = 0;
					nWordsInLine = 0;
					continue;
				}
				if (isWhiteSpace) continue;
				
				mCharacters.Buffer[cIndex].Min = MathUtils.v2max;
				mCharacters.Buffer[cIndex].Max = MathUtils.v2min;

				for (int i = 0; i < 4; i++)
				{
					int idx = iFirstVert + cIndex * 4 + i;

					seVertex.position = vertices[idx];
					seVertex.color = colors[idx];
					seVertex.uv = uvs[idx];
					//seVertex.normal = normals[idx];
					//seVertex.tangent = tangents[idx];
					seVertex.characterID = cIndex;

					seVertex.position = MatrixPanelToWidget.MultiplyPoint3x4(seVertex.position);

					if (mCharacters.Buffer[cIndex].Min.x > seVertex.position.x) mCharacters.Buffer[cIndex].Min.x = seVertex.position.x;
					if (mCharacters.Buffer[cIndex].Min.y > seVertex.position.y) mCharacters.Buffer[cIndex].Min.y = seVertex.position.y;
					if (mCharacters.Buffer[cIndex].Max.x < seVertex.position.x) mCharacters.Buffer[cIndex].Max.x = seVertex.position.x;
					if (mCharacters.Buffer[cIndex].Max.y < seVertex.position.y) mCharacters.Buffer[cIndex].Max.y = seVertex.position.y;

					mOriginalVertices.Buffer[cIndex * 4 + i] = seVertex;
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
					mCharacters.Buffer[cIndex].iCharacterInText = cIndex;
					mCharacters.Buffer[cIndex].iWord = iWord;
					mCharacters.Buffer[cIndex].iWordInLine = nWordsInLine;
					mCharacters.Buffer[cIndex].iParagraph = iParagraph;
					mCharacters.Buffer[cIndex].TopY = iLine * mLineHeight;
					mCharacters.Buffer[cIndex].BaselineY = iLine * mLineHeight;

					cIndex++;
				}
			}
			mCharacterSize /= iLine+1;
			mCharacters.Size = cIndex;
			mOriginalVertices.Size = cIndex*4;
		}

		void ExportVerticesToNGUI(List<Vector3> vertices, List<Vector2> uvs, List<Color> colors, List<Vector3> normals, List<Vector4> tangents, List<Vector4> uvs2)
		{
			int nVerts = mNGUI_Widget.geometry.verts.Count;
			int iFirstVert = vertices.Count - nVerts;

			vertices.RemoveRange(iFirstVert, nVerts);
			uvs.RemoveRange(iFirstVert, nVerts);
			colors.RemoveRange(iFirstVert, nVerts);
			if (uvs2!=null)
				uvs2.RemoveRange(iFirstVert, nVerts);
			if (normals != null)
			{
				normals.RemoveRange(iFirstVert, nVerts);
				tangents.RemoveRange(iFirstVert, nVerts);
			}

			var MatrixWidgetToPanel = mNGUI_Widget.panel.worldToLocal * mNGUI_Widget.cachedTransform.localToWorldMatrix;

			Vector4 v4 = MathUtils.v4zero;

			for (int i = 0; i < mOriginalVertices.Size; ++i)
			{
				vertices.Add( MatrixWidgetToPanel.MultiplyPoint3x4(mOriginalVertices.Buffer[i].position ));
				colors.Add(mOriginalVertices.Buffer[i].color);
				uvs.Add(mOriginalVertices.Buffer[i].uv);

				if (uvs2 != null)
				{
					v4.x = mOriginalVertices.Buffer[i].uv1.x;
					v4.y = mOriginalVertices.Buffer[i].uv1.y;
					v4.z = mOriginalVertices.Buffer[i].tangent.w;
					uvs2.Add(v4);
				}
				if (normals != null)
				{
					normals.Add(mOriginalVertices.Buffer[i].normal);
					tangents.Add(mOriginalVertices.Buffer[i].tangent);
				}
			}
		}
	}
}

#endif