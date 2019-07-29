using System.Collections.Generic;
using GameFramework.Res.Base;

namespace GameFramework.DataTable.Base
{
    public interface IDataTableManager
    {
        int Count { get; }
        void SetResourceManager(IResourceManager resourceManager);
        void LoadDataTable(ResourceLoadInfo resourceLoadInfo,LoadDataTableInfo loadDataTableInfo);
        bool HasDataTable<T>() where T : class, IDataRow, new();
        bool HasDataTable<T>(string name) where T : class, IDataRow, new();
        IDataTable GetDataTable<T> () where T : class, IDataRow, new();
        IDataTable GetDataTable<T> (string name) where T : class, IDataRow, new();
        IDataTable[] GetAllDataTables();
        void GetAllDataTables(List<IDataTable> results);
        bool DestroyDataTable<T>() where T : class, IDataRow, new();
        bool DestroyDataTable<T>(string name) where T : class, IDataRow, new();
    }
}