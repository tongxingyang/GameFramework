using UnityEngine;

namespace GameFramework.DevelopTool
{
    public class FPSCounter : MonoBehaviour
    {
        private const float calcRate = 0.5f;
        private int frameCount = 0;
        private float rateDuration = 0f;
        private int fps = 0;

        void Start()
        {
            this.frameCount = 0;
            this.rateDuration = 0f;
            this.fps = 0;
        }

        void Update()
        {
            ++this.frameCount;
            this.rateDuration += Time.unscaledDeltaTime;
            if (this.rateDuration > calcRate)
            {
                // 计算帧率
                this.fps = (int)(this.frameCount / this.rateDuration);
                this.frameCount = 0;
                this.rateDuration = 0f;
            }
        }

        void OnGUI()
        {
            GUILayout.Label("FPS：" + fps.ToString());
        }
    }
}