using System;
using GameFramework.Utility;

namespace GameFramework.DataTable.Base
{
    public interface IDataRow
    {
        int GetKeyValue();
        void Parse(ByteBuffer buffer);
        void Parse(string str);
        void Clear();
    }
}