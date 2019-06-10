using System;
using System.Collections.Generic;

namespace GameFramework.DataTable.Base
{
    public interface IDataTable 
    {
        Type RowType { get; }
        Type KeyType { get; }
        string Name { get; }
        int Count { get; }
        void Shutdown();
        void LoadData(byte[] bytes);
        void LoadData(string str);
    }
}