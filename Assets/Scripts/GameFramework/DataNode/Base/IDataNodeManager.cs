namespace GameFramework.DataNode.Base
{
    public interface IDataNodeManager
    {
        IDataNode Root { get; }
        T GetData<T>(string path);
        T GetData<T>(string path, IDataNode dataNode);
        void SetData<T>(string path, T t);
        void SetData<T>(string path, T t, IDataNode dataNode);
        IDataNode GetNode(string path);
        IDataNode GetNode(string path, IDataNode dataNode);
        IDataNode GetOrAddNode(string path);
        IDataNode GetOrAddNode(string path, IDataNode dataNode);
        void RemoveNode(string path);
        void RemoveNode(string path, IDataNode dataNode);
        void Clear();
    }
}