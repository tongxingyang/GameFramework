using System;
using GameFramework.Utility;
using UnityEditorInternal.Profiling.Memory.Experimental;

namespace GameFramework.DataTable.Base
{
    public interface IDataRow
    {
        T GetKeyValue<T>();
        void Parse(ByteBuffer buffer);
        void Parse(string str);
        void Clear();
    }
}