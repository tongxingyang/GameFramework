using System;
using GameFramework.Debug;

namespace GameFramework.DataNode.Base
{
    public sealed class DataNodeManager : IDataNodeManager
    {
        private const string RootName = "rootnode";
        private IDataNode root;
        public IDataNode Root => root;

        public DataNodeManager()
        {
            root = new DataNode(RootName,null);
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public void Shutdown()
        {
            Clear();
            root = null;
        }
        
        public T GetData<T>(string path)
        {
            return GetData<T>(path, root);
        }

        public T GetData<T>(string path, IDataNode dataNode)
        {
            IDataNode current = GetNode(path, dataNode);
            if (current == null)
            {
                Debuger.LogError("get Data Node Error Path :"+path);
                return default(T);
            }
            return current.GetData<T>();

        }

        public void SetData<T>(string path, T t)
        {
            SetData<T>(path,t,root);
        }

        public void SetData<T>(string path, T t, IDataNode dataNode)
        {
            IDataNode current = GetOrAddNode(path, dataNode);
            current.SetData<T>(t);
        }

        public IDataNode GetNode(string path)
        {
            return GetNode(path, root);
        }

        public IDataNode GetNode(string path, IDataNode dataNode)
        {
            IDataNode current = dataNode;
            string[] splitPath = GetSplitPath(path);
            foreach (string s in splitPath)
            {
                current = current.GetChild(s);
                if (current == null) return null;
            }
            return current;
        }

        public IDataNode GetOrAddNode(string path)
        {
            return GetOrAddNode(path, root);
        }

        public IDataNode GetOrAddNode(string path, IDataNode dataNode)
        {
            IDataNode current = dataNode;
            string[] splitPath = GetSplitPath(path);
            foreach (string s in splitPath)
            {
                current = current.GetOrAddChild(s);
            }
            return current;
        }

        public void RemoveNode(string path)
        {
            RemoveNode(path,root);
        }

        public void RemoveNode(string path, IDataNode dataNode)
        {
            IDataNode current = dataNode;
            IDataNode parent = current.Parent;
            string[] splitPath = GetSplitPath(path);
            foreach (string s in splitPath)
            {
                parent = current;
                current = current.GetChild(s);
                if(current==null)return;
            }
            parent?.RemoveChild(current.Name);
        }

        private string[] GetSplitPath(string path)
        {
            return path.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries);
        }
        
        public void Clear()
        {
            root.Clear();
        }
    }
}