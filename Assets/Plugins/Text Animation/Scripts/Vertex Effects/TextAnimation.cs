using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;


namespace I2.TextAnimation
{
	[ExecuteInEditMode]
	[AddComponentMenu("I2/Text Animation/I2TextAnimation", 1)]
	[Serializable]
	public partial class TextAnimation : MonoBehaviour
	{
		[NonSerialized] public static ArrayBuffer<SEVertex> mOriginalVertices = new ArrayBuffer<SEVertex>();       // This its a direct access to mSEMesh.mLayers[SEVerticesLayers.Original]

		[NonSerialized] public static ArrayBuffer<SE_Character> mCharacters = new ArrayBuffer<SE_Character>();
		[NonSerialized] public Color mWidgetColor;
		[NonSerialized]public float mCharacterSize = 1;   // When using a text, it is the Font Size, otherwise the Image Height
		[NonSerialized]public float mLineHeight=1;        // When using a text, it is the Font LineHeight, otherwise the Image Height

		[NonSerialized] public Vector2 mAllCharactersMin, mAllCharactersMax;    // Bounding Rect of all vertices in mCharacters
		public static SEVertex seVertex = new SEVertex();



		[NonSerialized]public Rect mRect;
		[NonSerialized]public Vector2 mRectPivot;
		[NonSerialized]public Vector2 mWidgetRectMin, mWidgetRectMax; // Cached from mRect



		protected void OnDestroy()
		{
		}

		protected void OnEnable()
		{
			#if I2_NGUI
				OnEnableNGUI();
			#endif
			#if I2_TMPro
				OnEnableTMPro();
			#endif
			OnEnableUGUI();

			if (Application.isPlaying && _OnEnable_PlayAnim >= 0 && _OnEnable_PlayAnim < _AnimationSlots.Length)
			{
				StopAllAnimations();
				_AnimationSlots[_OnEnable_PlayAnim].Play(this);
			}

			MarkWidgetAsChanged(true, true);
		}

		protected void OnDisable()
		{
			TextAnimationsManager.UnregisterAnimation(this);

			#if I2_NGUI
				OnDisableNGUI();
			#endif
			#if I2_TMPro
				OnDisableTMPro();
			#endif
				OnDisableUGUI();

			// If component was disabled, but not the GO, then retrieve the old material and recreate the vertices
			if (gameObject.activeSelf)
				MarkWidgetAsChanged(true, true);
		}

		public void MarkWidgetAsChanged(bool MarkVertices=true, bool MarkMaterial=false)
		{
			if (mGraphic==null)	mGraphic=GetComponent<Graphic>();
			if (mGraphic != null)
			{
				if (MarkVertices && MarkMaterial)
					mGraphic.SetAllDirty();
				else
				{
					if (MarkMaterial)
						mGraphic.SetMaterialDirty();

					if (MarkVertices)
					{
						mGraphic.SetVerticesDirty();
						//mGraphic.SetLayoutDirty();
					}
				}
				#if UNITY_EDITOR
				if (!Application.isPlaying)
					UnityEditor.EditorUtility.SetDirty(this);
				//mGraphic.Rebuild(CanvasUpdate.PreRender);
				#endif
			}

			#if I2_NGUI
				MarkWidgetAsChanged_NGUI(MarkVertices, MarkMaterial);
			#endif
			#if I2_TMPro
				MarkWidgetAsChanged_TMPro(MarkVertices, MarkMaterial);
			#endif
		}

		public void SetWidgetColor( Color32 color )
		{
			#if I2_NGUI
			SetWidgetColor_NGUI(color);
			#endif

			#if I2_TMPro
			SetWidgetColor_TMPro(color);
			#endif
			SetWidgetColor_UGUI(color);
		}
	}
}
