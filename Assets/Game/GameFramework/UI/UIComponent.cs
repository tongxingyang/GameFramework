using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.UI.UIExtension;
using GameFramework.Utility.Extension;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI
{
    [DisallowMultipleComponent]
    public class UIComponent : GameFrameworkComponent
    {
        public UIFont MainFont = null;
        private CanvasScaler canvasScaler;
        public override int Priority => SingletonMono<GameFramework>.GetInstance().UIPriority;
#if UNITY_EDITOR
        private int resolutionWidth = 0;
        private int resolutionHeight = 0;
#endif
        
        public override void OnAwake()
        {
            base.OnAwake();
            canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
            MatchCanvas();
        }

        public override void OnStart()
        {
            base.OnStart();
            Font.textureRebuilt += OnFontTextureRebuilt;
            System.Type fontUpdateTrackerType = typeof(UnityEngine.UI.FontUpdateTracker);
            rebuildForFont = fontUpdateTrackerType.GetMethod("RebuildForFont", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
#if UNITY_EDITOR
            if (resolutionWidth != Screen.width || resolutionHeight != Screen.height)
            {
                resolutionWidth = Screen.width;
                resolutionHeight = Screen.height;
                MatchCanvas();
            }
#endif
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            if(rebuildForFont == null) return;
            foreach (FontNode fontNode in fontUpdateList)
            {
                if (fontNode.IsNeedRebuild)
                {
                    Font font = fontNode.RebuildFont;
                    rebuildForFont.Invoke(null, new object[] {font});
                    fontNode.IsNeedRebuild = false;
                }
            }
        }

        private void MatchCanvas()
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = AppConst.UiConfig.GameResolution;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            if (Screen.width / canvasScaler.referenceResolution.x > Screen.height / canvasScaler.referenceResolution.y)
            {
                canvasScaler.matchWidthOrHeight = 1;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0;
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Font.textureRebuilt -= OnFontTextureRebuilt;
            fontUpdateList.Clear();
        }

        #region DynamicFontRebuild

        private class FontNode
        {
            private bool isNeedRebuild = false;
            public Font RebuildFont { get; set; }

            public bool IsNeedRebuild
            {
                get => RebuildFont != null && isNeedRebuild;
                set => isNeedRebuild = value;
            }
            
            public FontNode(Font font)
            {
                RebuildFont = font;
            }
        }
        
        private System.Reflection.MethodInfo rebuildForFont = null;
        private List<FontNode> fontUpdateList = new List<FontNode>();

        private void OnFontTextureRebuilt(Font font)
        {
            bool findThisFont = false;
            foreach (FontNode node in fontUpdateList)
            {
                if (node.RebuildFont == font)
                {
                    node.IsNeedRebuild = true;
                    findThisFont = true;
                    break;
                }
            }

            if (!findThisFont)
            {
                FontNode fontNode = new FontNode(font) {IsNeedRebuild = true};
                fontUpdateList.Add(fontNode);
            }
        }
        
        #endregion
    }
}