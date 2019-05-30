using GameFramework.Base;
using GameFramework.DataNode.Base;
using GameFramework.DataTable.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.DataNode
{
    [DisallowMultipleComponent]
    public class DataNodeComponent :GameFrameworkComponent
    {
        private DataNodeManager dataNodeManager;
        public override int Priority => SingletonMono<GameFramework>.GetInstance().DataNodePriority;

        public override void OnAwake()
        {
            base.OnAwake();
            dataNodeManager = new DataNodeManager();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            dataNodeManager.OnUpdate(elapseSeconds,realElapseSeconds);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            dataNodeManager.Shutdown();
        }

        public T GetData<T>(string path)
        {
            return dataNodeManager.GetData<T>(path);
        }

        public T GetData<T>(string path, IDataNode dataNode)
        {
            return dataNodeManager.GetData<T>(path, dataNode);
        }

        public void SetData<T>(string path, T t)
        {
            dataNodeManager.SetData<T>(path,t);
        }

        public void SetData<T>(string path, T t, IDataNode dataNode)
        {
            dataNodeManager.SetData<T>(path,t,dataNode);
        }

        public IDataNode GetNode(string path)
        {
            return dataNodeManager.GetNode(path);
        }

        public IDataNode GetNode(string path, IDataNode dataNode)
        {
            return dataNodeManager.GetNode(path, dataNode);
        }

        public IDataNode GetOrAddNode(string path)
        {
            return dataNodeManager.GetOrAddNode(path);
        }

        public IDataNode GetOrAddNode(string path, IDataNode dataNode)
        {
            return dataNodeManager.GetOrAddNode(path, dataNode);
        }

        public void RemoveNode(string path)
        {
            dataNodeManager.RemoveNode(path);
        }

        public void RemoveNode(string path, IDataNode dataNode)
        {
            dataNodeManager.RemoveNode(path,dataNode);
        }

        public void Clear()
        {
            dataNodeManager.Clear();
        }
    }
}