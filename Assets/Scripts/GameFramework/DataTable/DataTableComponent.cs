using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.DataTable.Base;
using GameFramework.Res;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.DataTable
{
    [DisallowMultipleComponent]
    public class DataTableComponent : GameFrameworkComponent
    {
        private DataTableManager dataTableManager;
        
        public int Count => dataTableManager?.Count ?? 0;
        public override int Priority
        {
            get { return 50; }
        }

        public override void OnAwake()
        {
            base.OnAwake();
            dataTableManager = new DataTableManager();
        }

        public override void OnStart()
        {
            base.OnStart();
            dataTableManager.SetResourceManager(Singleton<GameEntry>.GetInstance().GetComponent<ResourceComponent>().GetResourceManager());
        }
        
        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            dataTableManager.OnUpdate(elapseSeconds,realElapseSeconds);
        }
        
        public override void Shutdown()
        {
            base.Shutdown();
            dataTableManager.Shutdown();
        }

        public void LoadDataTable(string dataTableAssetName, int priority, DataTableParams dataTableParams)
        {
            dataTableManager.LoadDataTable(dataTableAssetName,priority,dataTableParams);
        }

        public bool HasDataTable<T>() where T : class, new()
        {
            return dataTableManager.HasDataTable<T>();
        }

        public bool HasDataTable<T>(string name) where T : class, new()
        {
            return dataTableManager.HasDataTable<T>(name);
        }

        public IDataTable GetDataTable<T>() where T : class, new()
        {
            return dataTableManager.GetDataTable<T>();
        }

        public IDataTable GetDataTable<T>(string name) where T : class, new()
        {
            return dataTableManager.GetDataTable<T>(name);
        }

        public IDataTable[] GetAllDataTables()
        {
            return dataTableManager.GetAllDataTables();
        }

        public void GetAllDataTables(List<IDataTable> results)
        {
            dataTableManager.GetAllDataTables(results);
        }

        public IDataTable CreateDataTable<T>(string data) where T : class, new()
        {
            return dataTableManager.CreateDataTable<T>(data);
        }

        public IDataTable CreateDataTable<T>(string name, string data) where T : class, new()
        {
            return dataTableManager.CreateDataTable<T>(name, data);
        }

        public IDataTable CreateDataTable<T>(byte[] data) where T : class, new()
        {
            return dataTableManager.CreateDataTable<T>(data);
        }

        public IDataTable CreateDataTable<T>(string name, byte[] data) where T : class, new()
        {
            return dataTableManager.CreateDataTable<T>(name, data);
        }

        public bool DestoryDataTable<T>() where T : class, new()
        {
            return dataTableManager.DestoryDataTable<T>();
        }

        public bool DestoryDataTable<T>(string name) where T : class, new()
        {
            return dataTableManager.DestoryDataTable<T>(name);
        }
    }
}