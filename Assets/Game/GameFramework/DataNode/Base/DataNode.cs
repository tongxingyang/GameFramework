using System.Collections.Generic;
using GameFramework.FSM;

namespace GameFramework.DataNode.Base
{
    public class DataNode:IDataNode
    {
        private string name;
        public string Name => name;
        public string FullName => parent == null ? name : Utility.StringUtility.Format("{0}{1}{2}", parent.FullName, ".", name);
        private IDataNode parent;
        public IDataNode Parent => parent;
        public int ChildCount => childs?.Count ?? 0;
        private List<IDataNode> childs;
        private object value;
        public DataNode(string name,IDataNode parent)
        {
            this.name = name;
            value = null;
            this.parent = parent;
            childs = null;
        }
        
        public T GetData<T>()
        {
            return (T) value;
        }

        public void SetData<T>(T t)
        {
            value = t;
        }

        public IDataNode GetChild(int index)
        {
            return index >= ChildCount ? null : childs[index];
        }

        public IDataNode GetChild(string name)
        {
            if (childs == null) return null;
            foreach (IDataNode dataNode in childs)
            {
                if (dataNode.Name == name)
                {
                    return dataNode;
                }
            }
            return null;
        }

        public IDataNode GetOrAddChild(string name)
        {
            IDataNode dataNode = GetChild(name);
            if (dataNode != null) return dataNode;
            dataNode = new DataNode(name,this);
            if(childs==null) childs = new List<IDataNode>();
            childs.Add(dataNode);
            return dataNode;
        }

        public IDataNode[] GetAllChild()
        {
            return childs == null ? null : childs.ToArray();
        }

        public void GetAllChild(List<IDataNode> result)
        {
            result.Clear();
            if (childs == null) return;
            foreach (IDataNode dataNode in childs)
            {
                result.Add(dataNode);
            }
        }

        public void RemoveChild(int index)
        {
            IDataNode dataNode = GetChild(index);
            if(dataNode==null) return;
            dataNode.Clear();
            childs.Remove(dataNode);

        }

        public void RemoveChild(string name)
        {
            IDataNode dataNode = GetChild(name);
            if(dataNode==null) return;
            dataNode.Clear();
            childs.Remove(dataNode);
        }

        public void Clear()
        {
            value = null;
            if (childs != null)
            {
                foreach (IDataNode dataNode in childs)
                {
                    dataNode.Clear();
                }
                childs.Clear();
            }
        }

        public string ToDataString()
        {
            return Utility.StringUtility.Format("[{0}] {1}", name, value);
        }
    }
}