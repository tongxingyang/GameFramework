using System;

namespace GameFramework.DataTable.Base
{
    public sealed class LoadDataTableInfo
    {
        public enLoadDataTableType LoadDataTableType { get; }
        public object UserData { get; }
        public Type DataRowType { get; }
        public Type KeyType { get; }
        public LoadDataTableInfo(enLoadDataTableType loadDataTableType,Type dataRowType,Type keyType ,object userData = null)
        {
            this.LoadDataTableType = loadDataTableType;
            this.UserData = userData;
            this.DataRowType = dataRowType;
            this.KeyType = keyType;
        }
    }
}