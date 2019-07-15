using System.Collections.Generic;

namespace GameFramework.Animation.MeshAnimation
{
    public class MeshAnimationData
    {
        private static Dictionary<string, MeshAnimationDataItem> animationDatas =
            new Dictionary<string, MeshAnimationDataItem>();

        public static MeshAnimationDataItem GetMeshAnimationDataItem(string roleName)
        {
            if (animationDatas.ContainsKey(roleName) && animationDatas[roleName] != null)
            {
                return animationDatas[roleName];
            }
            return null;
        }

        public static void Clear()
        {
            if (null != animationDatas)
            {
                var itor = animationDatas.GetEnumerator();
                while (itor.MoveNext())
                {
                    itor.Current.Value.Destroy();
                }
                animationDatas.Clear();
                itor.Dispose();
            }
        }
        public static void RemoveAnimationGroupFromCache(string roleName)
        {
            if (animationDatas.ContainsKey(roleName))
            {
                animationDatas[roleName].Destroy();
                animationDatas[roleName] = null;
                animationDatas.Remove(roleName);
            }
        }
        
        public static void LoadMeshAnimationData(string roleName, ExportMeshAnimationData animationData)
        {
            MeshAnimationDataItem animationDataItem;
            if (animationDatas.TryGetValue(roleName, out animationDataItem))
            {
                if (null != animationDataItem)
                {
                    return;
                }
            }
            else
            {
                animationDataItem = new MeshAnimationDataItem(animationData);
                animationDatas[roleName] = animationDataItem;
            }
        }
    }
}