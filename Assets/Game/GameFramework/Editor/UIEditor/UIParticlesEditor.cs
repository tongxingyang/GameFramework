using GameFramework.UI.UIExtension;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.GameFramework.Editor.UIEditor
{
    public class UIParticlesEditor
    {
        [CustomEditor(typeof(UIParticles))]
        public class UiParticlesEditor : GraphicEditor
        {
    
            private SerializedProperty m_RenderMode;
            private SerializedProperty m_StretchedSpeedScale;
            private SerializedProperty m_StretchedLenghScale;
            private SerializedProperty m_IgnoreTimescale;
            private SerializedProperty m_RenderedMesh;
    
            protected override void OnEnable()
            {
                base.OnEnable();
    
                m_RenderMode = serializedObject.FindProperty("m_RenderMode");
                m_StretchedSpeedScale = serializedObject.FindProperty("m_StretchedSpeedScale");
                m_StretchedLenghScale = serializedObject.FindProperty("m_StretchedLenghScale");
                m_IgnoreTimescale = serializedObject.FindProperty("m_IgnoreTimescale");
                m_RenderedMesh = serializedObject.FindProperty("m_RenderedMesh");
            }
    
    
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
    
                UIParticles uiParticleSystem = (UIParticles) target;
    
                if (GUILayout.Button("Apply to nested particle systems"))
                {
                    var nested = uiParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
                    foreach (var particleSystem in nested)
                    {
                        if (particleSystem.GetComponent<UIParticles>() == null)
                            particleSystem.gameObject.AddComponent<UIParticles>();
                    }
                }
    
                EditorGUILayout.PropertyField(m_RenderMode);
    
                if (uiParticleSystem.RenderMode == UiParticleRenderMode.StreachedBillboard)
                {
                    EditorGUILayout.PropertyField(m_StretchedSpeedScale);
                    EditorGUILayout.PropertyField(m_StretchedLenghScale);
                }
                
                if (uiParticleSystem.RenderMode == UiParticleRenderMode.Mesh)
                {
                    EditorGUILayout.PropertyField(m_RenderedMesh);
                }
    
                EditorGUILayout.PropertyField(m_IgnoreTimescale);
                serializedObject.ApplyModifiedProperties();
            }
            
            [MenuItem("GameObject/UI/UI Particle System", false, 2101)]
			public static void AddScrollView(MenuCommand menuCommand)
			{
				GameObject gameObject = new GameObject("Particle System");
				gameObject.AddComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
				gameObject.AddComponent<UIParticles>();
				PlaceUIElementRoot(gameObject, menuCommand);
			}
			
			private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
			{
				GameObject parent = menuCommand.context as GameObject;
				if (parent == null || parent.GetComponentInParent<Canvas>() == null)
				{
					parent = GetOrCreateCanvasGameObject();
				}
				string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
				element.name = uniqueName;
				Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
				Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
				GameObjectUtility.SetParentAndAlign(element, parent);
				if (parent != menuCommand.context) 
					SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());
	
				Selection.activeGameObject = element;
			}
			
			
			private static GameObject GetOrCreateCanvasGameObject()
			{
				GameObject selectedGo = Selection.activeGameObject;
	
				Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
				if (canvas != null && canvas.gameObject.activeInHierarchy)
					return canvas.gameObject;
	
				canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
				if (canvas != null && canvas.gameObject.activeInHierarchy)
					return canvas.gameObject;
	
				return CreateNewUI();
			}
			
			private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
			{
				SceneView sceneView = SceneView.lastActiveSceneView;
				if (sceneView == null && SceneView.sceneViews.Count > 0)
					sceneView = SceneView.sceneViews[0] as SceneView;
	
				if (sceneView == null || sceneView.camera == null)
					return;
	
				Vector2 localPlanePosition;
				Camera camera = sceneView.camera;
				Vector3 position = Vector3.zero;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2f, camera.pixelHeight / 2f), camera, out localPlanePosition))
				{
					localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
					localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;
	
					localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
					localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);
	
					position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
					position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;
	
					Vector3 minLocalPosition;
					minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
					minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;
	
					Vector3 maxLocalPosition;
					maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
					maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;
	
					position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
					position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
				}
	
				itemTransform.anchoredPosition = position;
				itemTransform.localRotation = Quaternion.identity;
				itemTransform.localScale = Vector3.one;
			}
			
			private static GameObject CreateNewUI()
			{
				var root = new GameObject("Canvas")
				{
					layer = LayerMask.NameToLayer("UI")
				};
				Canvas canvas = root.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				root.AddComponent<CanvasScaler>();
				root.AddComponent<GraphicRaycaster>();
				Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
	
				CreateEventSystem(false);
				return root;
			}
	
			private static void CreateEventSystem(bool select, GameObject parent = null)
			{
				var esys = Object.FindObjectOfType<EventSystem>();
				if (esys == null)
				{
					var eventSystem = new GameObject("EventSystem");
					GameObjectUtility.SetParentAndAlign(eventSystem, parent);
					esys = eventSystem.AddComponent<EventSystem>();
					eventSystem.AddComponent<StandaloneInputModule>();
					Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
				}
	
				if (select && esys != null)
				{
					Selection.activeGameObject = esys.gameObject;
				}
			}
        }
    }
}