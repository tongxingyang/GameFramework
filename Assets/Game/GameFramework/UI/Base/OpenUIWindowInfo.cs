using GameFramework.UI.Base;

namespace Game.GameFramework.UI.Base
{
    public class OpenUIWindowInfo
    {
        private  int serialId;
        private UIWindowGroup uiWindowGroup;
        private bool pauseCovered;
        private UIWindowContext uiWindowContext;

        public int SerialId => serialId;
        public UIWindowGroup WindowGroup => uiWindowGroup;
        public bool PauseCovered => pauseCovered;
        public UIWindowContext WindowContext => uiWindowContext;
        
        public OpenUIWindowInfo(int serialId, UIWindowGroup uiWindowGroup, bool pauseCovered, UIWindowContext uiWindowContext)
        {
            this.serialId = serialId;
            this.uiWindowGroup = uiWindowGroup;
            this.pauseCovered = pauseCovered;
            this.uiWindowContext = uiWindowContext;
        }
    }
}