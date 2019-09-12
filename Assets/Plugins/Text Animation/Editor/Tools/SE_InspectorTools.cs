using UnityEngine;
using System.Collections;
using UnityEditor;

namespace I2.TextAnimation
{ 
	public static class I2_InspectorTools
	{
		public static string HelpURL_forum         = "http://inter-illusion.com/forum/i2-text-animation";
		public static string HelpURL_ReleaseNotes  = "http://inter-illusion.com/forum/i2-text-animation/889-release-notes-i2-text-animations";
		public static string HelpURL_Documentation = "http://inter-illusion.com/assets/I2TextAnimationManual/Animation.html";
		public static string HelpURL_AssetStore    = "https://www.assetstore.unity3d.com/#!/content/95580";


        public static string URL_Roadmap = "https://trello.com/b/ImbJ0lQH/i2-TextAnimation-roadmap";

        public static string GetVersion()
		{
			return "1.0.4 f1";
		}

		#region Styles

		public static GUIStyle GUIStyle_Header
		{
			get
			{
				if (mGUIStyle_Header == null)
				{
					mGUIStyle_Header = new GUIStyle("HeaderLabel");
					mGUIStyle_Header.fontSize = 35;
					mGUIStyle_Header.normal.textColor = Color.Lerp(Color.white, Color.gray, 0.5f);
					mGUIStyle_Header.fontStyle = FontStyle.BoldAndItalic;
					mGUIStyle_Header.alignment = TextAnchor.UpperCenter;
				}
				return mGUIStyle_Header;
			}
		}
		static GUIStyle mGUIStyle_Header;

		public static GUIStyle GUIStyle_SmallHeader
		{
			get
			{
				if (mGUIStyle_SmallHeader == null)
				{
					mGUIStyle_SmallHeader = new GUIStyle("HeaderLabel");
					mGUIStyle_SmallHeader.fontSize = 25;
					mGUIStyle_SmallHeader.normal.textColor = Color.Lerp(Color.white, Color.gray, 0.5f);
					mGUIStyle_SmallHeader.fontStyle = FontStyle.BoldAndItalic;
					mGUIStyle_SmallHeader.alignment = TextAnchor.UpperCenter;
				}
				return mGUIStyle_SmallHeader;
			}
		}
		static GUIStyle mGUIStyle_SmallHeader;
		public static GUIStyle GUIStyle_SubHeader
		{
			get
			{
				if (mGUIStyle_SubHeader == null)
				{
					mGUIStyle_SubHeader = new GUIStyle("HeaderLabel");
					mGUIStyle_SubHeader.fontSize = 13;
					mGUIStyle_SubHeader.fontStyle = FontStyle.Normal;
					mGUIStyle_SubHeader.margin.top = -50;
					mGUIStyle_SubHeader.alignment = TextAnchor.UpperCenter;
				}
				return mGUIStyle_SubHeader;
			}
		}
		static GUIStyle mGUIStyle_SubHeader;

		public static GUIStyle GUIStyle_Background
		{
			get
			{
				if (mGUIStyle_Background == null)
				{
					mGUIStyle_Background = new GUIStyle(EditorStyles.textArea);
					mGUIStyle_Background.overflow.left = 50;
					mGUIStyle_Background.overflow.right = 50;
					mGUIStyle_Background.overflow.top = -5;
					mGUIStyle_Background.overflow.bottom = 0;
				}
				return mGUIStyle_Background;
			}
		}
		static GUIStyle mGUIStyle_Background;

        static public GUIStyle Style_LabelRightAligned
        {
            get
            {
                if (mStyle_LabelRightAligned == null)
                {
                    mStyle_LabelRightAligned = new GUIStyle("label");
                    mStyle_LabelRightAligned.alignment = TextAnchor.MiddleRight;
                }
                return mStyle_LabelRightAligned;
            }
        }
        static GUIStyle mStyle_LabelRightAligned;

        static public GUIStyle Style_LabelCenterAligned
        {
            get
            {
                if (mStyle_LabelCenterAligned == null)
                {
                    mStyle_LabelCenterAligned = new GUIStyle("label");
                    mStyle_LabelCenterAligned.alignment = TextAnchor.MiddleCenter;
                }
                return mStyle_LabelCenterAligned;
            }
        }
        static GUIStyle mStyle_LabelCenterAligned;


        static public GUIStyle Style_LabelItalic
        {
            get
            {
                if (mStyle_LabelItalic == null)
                {
                    mStyle_LabelItalic = new GUIStyle("label");
                    mStyle_LabelItalic.fontStyle = FontStyle.Italic;
                }
                return mStyle_LabelItalic;
            }
        }
        static GUIStyle mStyle_LabelItalic;

        #endregion

        [MenuItem("Tools/I2 TextAnimation/Help", false, 92)]
        [MenuItem("Help/I2 TextAnimation")]
        public static void MainHelp()
        {
            Application.OpenURL(I2_InspectorTools.HelpURL_Documentation);
        }
    }
}