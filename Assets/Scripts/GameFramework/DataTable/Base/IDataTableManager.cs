using System.Collections.Generic;
using GameFramework.Res.Base;

namespace GameFramework.DataTable.Base
{
    public interface IDataTableManager
    {
        int Count { get; }
        void SetResourceManager(IResourceManager resourceManager);
        void LoadDataTable(string dataTableAssetName,int priority, DataTableParams dataTableParams);
        bool HasDataTable<T>() where T : class, new();
        bool HasDataTable<T>(string name) where T : class, new();
        IDataTable GetDataTable<T> () where T : class, new();
        IDataTable GetDataTable<T> (string name) where T : class, new();
        IDataTable[] GetAllDataTables();
        void GetAllDataTables(List<IDataTable> results);
        IDataTable CreateDataTable<T>(string data) where T : class, new();
        IDataTable CreateDataTable<T>(string name,string data) where T : class, new();
        IDataTable CreateDataTable<T>(byte[] data) where T : class, new();
        IDataTable CreateDataTable<T>(string name,byte[] data) where T : class, new();
        bool DestoryDataTable<T>() where T : class, new();
        bool DestoryDataTable<T>(string name) where T : class, new();
    }
}