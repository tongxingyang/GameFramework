using System;
using System.Collections.Generic;
using GameFramework.DataTable.Base;
using GameFramework.Utility;
using UnityEngine;

namespace GameFramework.DataTable.Core
{
    public class DemoDataTable : IDataRow
    {
        private Quaternion xxx ; //
        private List<string> _list = new List<string>(); // dsdsd
        private Dictionary<string,string> _listw = new Dictionary<string, string>();
        
        public T GetKeyValue<T>()
        {
            throw new System.NotImplementedException();
        }

        public void Parse(ByteBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public void Parse(string str)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}