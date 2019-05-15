using GameFramework.Base;
using GameFramework.Res.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Res
{
    [DisallowMultipleComponent]
    public class ResourceComponent : GameFrameworkComponent
    {
        public override int Priority
        {
            get { return 120; }
        }
        
        private IResourceManager resourceManager;

        public ResourceComponent()
        {
            if (SingletonMono<GameFramework>.GetInstance().EditorResourceMode)
            {
                resourceManager = new EditorResourceManager();
            }
            else
            {
                resourceManager = new ResourceManager();
            }
        }

        public IResourceManager GetResourceManager()
        {
            return resourceManager;
        }
    }
}