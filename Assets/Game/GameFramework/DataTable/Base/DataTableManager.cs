using System;
using System.Collections.Generic;
using GameFramework.Debug;
using GameFramework.Res.Base;
using GameFramework.Utility;
using UnityEngine;

namespace GameFramework.DataTable.Base
{
    public sealed class DataTableManager : IDataTableManager
    {
        private Dictionary<string, IDataTable> dataTables;
        private IResourceManager resourceManager;
        private readonly LoadCallback loadCallback;
        public int Count => dataTables?.Count ?? 0;
        
        public DataTableManager()
        {
            dataTables = new Dictionary<string, IDataTable>();
            resourceManager = null;
            loadCallback = new LoadCallback(LoadDataTableSuccessCallback, LoadDataTableFailureCallback);
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public void Shutdown()
        {
            foreach (KeyValuePair<string,IDataTable> dataTable in dataTables)
            {
                dataTable.Value.Shutdown();
            }
            dataTables.Clear();
            dataTables = null;
        }
        
        public void SetResourceManager(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public void LoadDataTable(ResourceLoadInfo resourceLoadInfo,LoadDataTableInfo loadDataTableInfo)
        {
            resourceManager.LoadAsset<TextAsset>(resourceLoadInfo,loadCallback,loadDataTableInfo);
        }
        
        public bool HasDataTable<T>() where T : class, IDataRow, new()
        {
            return HasDataTable(Utility.StringUtility.GetFullName<T>(string.Empty));
        }

        public bool HasDataTable<T>(string name) where T : class, IDataRow, new()
        {
            return HasDataTable(Utility.StringUtility.GetFullName<T>(name));
        }

        private bool HasDataTable(string name)
        {
            return dataTables.ContainsKey(name);
        }
        
        public IDataTable GetDataTable<T>() where T : class, IDataRow, new()
        {
            return GetDataTable(Utility.StringUtility.GetFullName<T>(string.Empty));
        }

        public IDataTable GetDataTable<T>(string name) where T : class, IDataRow, new()
        {
            return GetDataTable(Utility.StringUtility.GetFullName<T>(name));
        }

        private IDataTable GetDataTable(string name)
        {
            if (dataTables.TryGetValue(name, out var dataTable))
            {
                return dataTable;
            }
            return null;
        }
        
        public IDataTable[] GetAllDataTables()
        {
            int index = 0;
            IDataTable[] results = new IDataTable[dataTables.Count];
            foreach (KeyValuePair<string, IDataTable> dataTable in dataTables)
            {
                results[index++] = dataTable.Value;
            }
            return results;
        }

        public void GetAllDataTables(List<IDataTable> results)
        {
            results.Clear();
            foreach (KeyValuePair<string,IDataTable> dataTable in dataTables)
            {
                results.Add(dataTable.Value);
            }
        }

        private IDataTable CreateDataTable(Type dataRowType, string data)
        {
            return CreateDataTable(dataRowType,String.Empty, data);
        }

        private IDataTable CreateDataTable(Type dataRowType, string name, string data)
        {
            if (!typeof(IDataRow).IsAssignableFrom(dataRowType))
            {
                Debuger.LogError(StringUtility.Format("{0} is not is idatarow",dataRowType));
                return null;
            }
            string tableName = Utility.StringUtility.GetFullName(dataRowType, name);
            if (HasDataTable(tableName))
            {
                Debuger.LogError(StringUtility.Format("DataTable is realdy , please check {0}:{1}",dataRowType,name));
                return null;
            }
            Type dataTableType = typeof(DataTable<>).MakeGenericType(dataRowType);
            IDataTable dataTable = (IDataTable)Activator.CreateInstance(dataTableType, name);
            dataTable.LoadData(data);
            dataTables.Add(tableName,dataTable);
            return dataTable;
        }

        private IDataTable CreateDataTable(Type dataRowType, byte[] data)
        {
            return CreateDataTable(dataRowType,String.Empty, data);
        }

        private IDataTable CreateDataTable(Type dataRowType, string name, byte[] data)
        {
            if (!typeof(IDataRow).IsAssignableFrom(dataRowType))
            {
                Debuger.LogError(StringUtility.Format("{0} is not is idatarow",dataRowType));
                return null;
            }
            string tableName = Utility.StringUtility.GetFullName(dataRowType, name);
            if (HasDataTable(tableName))
            {
                Debuger.LogError(StringUtility.Format("DataTable is realdy , please check {0}:{1}",dataRowType,name));
                return null;
            }
            Type dataTableType = typeof(DataTable<>).MakeGenericType(dataRowType);
            IDataTable dataTable = (IDataTable)Activator.CreateInstance(dataTableType, name);
            dataTable.LoadData(data);
            dataTables.Add(tableName,dataTable);
            return dataTable;
        }

        public bool DestroyDataTable<T>() where T : class, IDataRow, new()
        {
            return DestroyDataTable(Utility.StringUtility.GetFullName<T>(string.Empty));
        }

        public bool DestroyDataTable<T>(string name) where T : class, IDataRow, new()
        {
            return DestroyDataTable(Utility.StringUtility.GetFullName<T>(name));
        }
        
        private bool DestroyDataTable(string name)
        {
            IDataTable dataTable = null;
            if (dataTables.TryGetValue(name, out dataTable))
            {
                dataTable.Shutdown();
                return dataTables.Remove(name);
            }
            return false;
        }
        
        private void LoadDataTableSuccessCallback(string soundAssetName, object soundAsset, float duration, object userData)
        {
           
        }

        private void LoadDataTableFailureCallback(string soundAssetName, string errorMessage,object userData)
        {
          
        }
        
    }
}