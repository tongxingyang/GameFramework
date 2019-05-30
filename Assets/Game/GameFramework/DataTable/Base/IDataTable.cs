using System;
using System.Collections.Generic;

namespace GameFramework.DataTable.Base
{
    public interface IDataTable
    {
        string Name { get; }
        int Id { get; }
        int Count { get; }
        void Shutdown();
        void LoadData(byte[] bytes);
        void LoadData(string str);
        Type Type { get; }
    }
}