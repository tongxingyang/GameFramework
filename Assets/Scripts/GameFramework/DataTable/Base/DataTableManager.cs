using System;
using System.Collections.Generic;
using GameFramework.Debug;
using GameFramework.Res.Base;
using ICSharpCode.SharpZipLib.Core;

namespace GameFramework.DataTable.Base
{
    public sealed class DataTableManager : IDataTableManager
    {
        private Dictionary<string, IDataTable> dataTables;
        private IResourceManager resourceManager;
        private LoadAssetCallbacks loadAssetCallBacks;
        
        public int Count => dataTables?.Count ?? 0;

        public DataTableManager()
        {
            dataTables = new Dictionary<string, IDataTable>();
            resourceManager = null;
            loadAssetCallBacks = new LoadAssetCallbacks(LoadDataTableSuccessCallback, LoadDataTableFailureCallback, LoadDataTableUpdateCallback,LoadDataTableDependencyAssetCallback);
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
        }
        
        public void SetResourceManager(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public void LoadDataTable(string dataTableAssetName,int priority, DataTableParams dataTableParams)
        {
            //resourceManager.LoadAsset(dataTableAssetName, priority, loadAssetCallBacks, dataTableParams); todo txy
        }

        public bool HasDataTable<T>() where T : class, new()
        {
            return HasDataTable(Utility.StringUtility.GetFullName<T>(string.Empty));
        }

        public bool HasDataTable<T>(string name) where T : class, new()
        {
            return HasDataTable(Utility.StringUtility.GetFullName<T>(name));
        }

        private bool HasDataTable(string name)
        {
            return dataTables.ContainsKey(name);
        }

        public IDataTable GetDataTable<T>() where T : class, new()
        {
            return GetDataTable(Utility.StringUtility.GetFullName<T>(string.Empty));
        }

        public IDataTable GetDataTable<T>(string name) where T : class, new()
        {
            return GetDataTable(Utility.StringUtility.GetFullName<T>(name));
        }

        private IDataTable GetDataTable(string name)
        {
            IDataTable dataTable = null;
            if (dataTables.TryGetValue(name, out dataTable))
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

        public IDataTable CreateDataTable<T>(string data) where T : class, new()
        {
            return CreateDataTable<T>(String.Empty, data);
        }

        public IDataTable CreateDataTable<T>(string name, string data) where T : class, new()
        {
            if (HasDataTable<T>(name))
            {
                Debuger.LogError("DataTable is realdy add please check");
                return null;
            }
            DataTable<T> dataTable = new DataTable<T>(name);
            dataTable.LoadData(data);
            dataTables.Add(Utility.StringUtility.GetFullName<T>(name),dataTable);
            return dataTable;
        }

        public IDataTable CreateDataTable<T>(byte[] data) where T : class, new()
        {
            return CreateDataTable<T>(String.Empty, data);
        }

        public IDataTable CreateDataTable<T>(string name, byte[] data) where T : class, new()
        {
            if (HasDataTable<T>(name))
            {
                Debuger.LogError("DataTable is realdy add please check");
                return null;
            }
            DataTable<T> dataTable = new DataTable<T>(name);
            dataTable.LoadData(data);
            dataTables.Add(Utility.StringUtility.GetFullName<T>(name),dataTable);
            return dataTable;
        }

        public bool DestoryDataTable<T>() where T : class, new()
        {
            return DestoryDataTable(Utility.StringUtility.GetFullName<T>(string.Empty));
        }

        public bool DestoryDataTable<T>(string name) where T : class, new()
        {
            return DestoryDataTable(Utility.StringUtility.GetFullName<T>(name));
        }

        private bool DestoryDataTable(string name)
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

        private void LoadDataTableFailureCallback(string soundAssetName, string errorMessage,
            object userData)
        {
          
        }

        private void LoadDataTableUpdateCallback(string soundAssetName, float progress, object userData)
        {
           
        }

        private void LoadDataTableDependencyAssetCallback(string soundAssetName, string dependencyAssetName,
            int loadedCount, int totalCount, object userData)
        {
          
        }
        
    }
}