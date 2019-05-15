using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core
{
    [InitializeOnLoad]
    public class WelcomeScreen : EditorWindow
    {
        static WelcomeScreen()
        {
            int isShow = PlayerPrefs.GetInt("ShowWelcomeScreen", 1);
            if (isShow == 1)
            {
                EditorApplication.update += OnUpdate;
            }
        }

        static void OnUpdate()
        {
            bool isOpen = EditorApplication.ExecuteMenuItem("Editor/Welcome Screen");
            if (isOpen)
            {
                EditorApplication.update -= OnUpdate;
            }
        }
        private bool isShowFlag = true;
        private Texture dataTableTexture;
        private Rect dataTableRect = new Rect(70f,344f,250f,30f);
        private Rect mToggleButtonRect = new Rect(280f, 385f, 125f, 20f);
        public void OnEnable()
        {
            isShowFlag = PlayerPrefs.GetInt("ShowWelcomeScreen", 1) == 1;
        }

        public void OnGUI()
        {
            isShowFlag = GUI.Toggle(mToggleButtonRect, isShowFlag, "显示对话框");
            PlayerPrefs.SetInt("ShowWelcomeScreen", isShowFlag ? 1 : 0);
        }
        
        Texture LoadTexture(string name) {
            string path = "Assets/Scripts/GameFramework/Editor/EditorResources/Welcome/";
            return (Texture)AssetDatabase.LoadAssetAtPath(path + name, typeof(Texture));
        }
        [MenuItem("Editor/Welcome Screen",false)]
        public static void ShowWindow()
        {
            WelcomeScreen welcomeScreen = EditorWindow.GetWindow<WelcomeScreen>(true, "Welcome");
            welcomeScreen.minSize = welcomeScreen.maxSize = new Vector2(500, 500);
        }
    }
}