using System;
using System.Collections.Generic;
using GameFramework.DataTable.Base;
using GameFramework.Utility;
using UnityEngine;

namespace GameFramework.DataTable.Core
{
    public class DemoDataTable : IDataRow
    {
        private List<Vector2> id = new List<Vector2>();
        private Quaternion xxx ; //
        private List<string> _list = new List<string>(); // dsdsd
        private Dictionary<string,string> _listw = new Dictionary<string, string>();

        public int GetKeyValue()
        {
            return 1;
        }

        public void Parse(ByteBuffer buffer)
        {
            GetKeyValue();
            throw new System.NotImplementedException();
        }

        public void Parse(string str)
        {
//            string[] cols = DataTableTool.SplitLine(str);
//            for (int i = 0; i < cols.Length; i++)
//            {
//                string[] item = cols[0].Split('|');
//                for (int i = 1; i < item.Length; i++)
//                {
//                    id.Add(new Vector2());
//                }
//            }
//           
//            id = new Rect();
            xxx = new Quaternion();
            
            /*
             *  string listType = type.Split('|')[1];
                string[] datas = data.Split('|');
                if (datas.Length > 0)
                {
                    ValueParse.WriteValue(buffer, (ushort) datas.Length, ValueParse.UShortParse);
                    for (int i = 0; i < datas.Length; i++)
                    {
                        AddParseValue(buffer, listType, datas[i]);
                    }
                }
             * 
             */
        }

        public void Clear()
        {
//            id = new Color32();
            _list.Clear();
            throw new System.NotImplementedException();
        }
    }
}