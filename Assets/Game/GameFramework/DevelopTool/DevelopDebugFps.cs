using UnityEngine;

namespace GameFramework.DevelopTool
{
    public partial class DevelopComponent
    {
        public class DevelopDebugFps
        {
            private const float calcRate = 0.5f;
            private int frameCount = 0;
            private float rateDuration = 0f;
            public int Fps = 0;

            public void Init()
            {
                this.frameCount = 0;
                this.rateDuration = 0f;
                this.Fps = 0;
            }

            public void Update()
            {
                ++this.frameCount;
                this.rateDuration += Time.unscaledDeltaTime;
                if (this.rateDuration > calcRate)
                {
                    this.Fps = (int)(this.frameCount / this.rateDuration);
                    this.frameCount = 0;
                    this.rateDuration = 0f;
                }
            }
        }
    }
}