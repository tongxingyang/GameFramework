using System.Collections.Generic;
using GameFramework.Base;
using GameFramework.DataTable.Base;
using GameFramework.Res;
using GameFramework.Res.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.DataTable
{
    [DisallowMultipleComponent]
    public class DataTableComponent : GameFrameworkComponent
    {
        private DataTableManager dataTableManager;
        
        public int Count => dataTableManager?.Count ?? 0;
        public override int Priority => SingletonMono<GameFramework>.GetInstance().DataTablePriority;

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

        public void LoadDataTable(ResourceLoadInfo resourceLoadInfo, LoadDataTableInfo loadDataTableInfo)
        {
            dataTableManager.LoadDataTable(resourceLoadInfo,loadDataTableInfo);
        }

        bool HasDataTable<T>() where T : class, IDataRow, new()
        {
            return dataTableManager.HasDataTable<T>();
        }

        bool HasDataTable<T>(string name) where T : class, IDataRow, new()
        {
            return dataTableManager.HasDataTable<T>(name);
        }

        IDataTable GetDataTable<T>() where T : class, IDataRow, new()
        {
            return dataTableManager.GetDataTable<T>();
        }

        IDataTable GetDataTable<T>(string name) where T : class, IDataRow, new()
        {
            return dataTableManager.GetDataTable<T>(name);
        }

        IDataTable[] GetAllDataTables()
        {
            return dataTableManager.GetAllDataTables();
        }

        void GetAllDataTables(List<IDataTable> results)
        {
            dataTableManager.GetAllDataTables(results);
        }

        bool DestroyDataTable<T>() where T : class, IDataRow, new()
        {
            return dataTableManager.DestroyDataTable<T>();
        }

        bool DestroyDataTable<T>(string name) where T : class, IDataRow, new()
        {
            return dataTableManager.DestroyDataTable<T>(name);
        }
    }
}