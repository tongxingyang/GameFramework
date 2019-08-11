using System;
using System.Collections.Generic;

namespace GameFramework.DataTable.Base
{
    public interface IDataTable 
    {
        Type RowType { get; }
        string Name { get; }
        int Count { get; }
        int MinIndex { get; set; } 
        int MaxIndex { get; set; }
        void Shutdown();
        void LoadData(byte[] bytes);
        void LoadData(string str);
    }
}