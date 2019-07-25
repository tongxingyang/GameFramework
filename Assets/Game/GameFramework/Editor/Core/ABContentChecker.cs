namespace GameFramework.Editor.Core
{
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;
    using UnityEngine;

    public class Checker : EditorWindow
    {
        List<Object> allObjects = new List<Object>();
        Vector2 pos;

        private void OnGUI()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(pos))
            {
                pos = scroll.scrollPosition;
                foreach (var obj in allObjects)
                {
                    EditorGUILayout.ObjectField(obj, obj.GetType(), false);
                }
            }
        }

        [MenuItem("Assets/AssetBundle/Content Checker")]
        static void Open()
        {
            var window = CreateInstance<Checker>();
            window.ReadContent();
            window.Show();
        }

        void ReadContent()
        {
            UnityEngine.AssetBundle bundle = null;
            try
            {
                bundle = UnityEngine.AssetBundle.LoadFromFile(Application.dataPath + AssetDatabase.GetAssetPath(Selection.activeObject).Remove(0, 6));

                if (bundle != null)
                {
                    SerializedObject so = new SerializedObject(bundle);

                    foreach (SerializedProperty content in so.FindProperty("m_PreloadTable"))
                    {
                        allObjects.Add(content.objectReferenceValue);
                    }

                    allObjects = allObjects
                        .Where(c => c != null)
                        .Where(c => c is Component == false )
                        .Where(c => c is GameObject == false)
                        .Distinct().OrderBy(c => c.name).ToList();
                    bundle.Unload(false);
                }
                else
                {
                    Debug.LogWarning("this is not assetbundle?");
                    Close();
                }
            }
            finally
            {
                if( bundle != null)
                    bundle.Unload(false);
            }
        }
    }
}