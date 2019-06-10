using System;

namespace GameFramework.DataTable.Base
{
    public sealed class LoadDataTableInfo
    {
        private enLoadDataTableType loadDataTableType;
        private object userData;
        public enLoadDataTableType LoadDataTableType => loadDataTableType;
        public object UserData => userData;
        private Type dataRowType;
        public Type DataRowType => dataRowType;
        private Type keyType;
        public Type KeyType => keyType;
        public LoadDataTableInfo(enLoadDataTableType loadDataTableType,Type dataRowType,Type keyType,object userData = null)
        {
            this.loadDataTableType = loadDataTableType;
            this.userData = userData;
            this.dataRowType = dataRowType;
            this.keyType = keyType;
        }
    }
}