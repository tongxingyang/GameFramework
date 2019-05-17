using GameFramework.Base;
using GameFramework.Download.Base;
using UnityEngine;

namespace GameFramework.Download
{
    [DisallowMultipleComponent]
    public class DownloadComponent : GameFrameworkComponent
    {
        private DownloadManager downloadManager;
        public override void OnAwake()
        {
            base.OnAwake();
            downloadManager = new DownloadManager();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            downloadManager.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            downloadManager.Shutdown();
        }
    }
}