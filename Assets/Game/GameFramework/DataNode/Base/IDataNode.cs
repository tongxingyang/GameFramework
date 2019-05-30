using System.Collections.Generic;

namespace GameFramework.DataNode.Base
{
    public interface IDataNode
    {
        string Name { get; }
        string FullName { get; }
        IDataNode Parent { get; }
        int ChildCount { get; }
        T GetData<T>();
        void SetData<T>(T t);
        IDataNode GetChild(int index);
        IDataNode GetChild(string name);
        IDataNode GetOrAddChild(string name);
        IDataNode[] GetAllChild();
        void GetAllChild(List<IDataNode> result);
        void RemoveChild(int index);
        void RemoveChild(string name);
        void Clear();
        string ToDataString();
    }
}