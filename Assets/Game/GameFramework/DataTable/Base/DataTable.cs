using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Utility;

namespace GameFramework.DataTable.Base
{

    public class DataTable<TValue> : IEnumerable<TValue> ,IDataTable where TValue : class, IDataRow ,new()
    {
        private Dictionary<int, TValue> dataSet = null;
        public Type RowType => typeof(TValue);
        public string Name { get; }
        public int Count => dataSet?.Count ?? 0;
        public int MinIndex { get; set; }
        public int MaxIndex { get; set; }
        public TValue MinData { get; set; }
        public TValue MaxData { get; set; }
        public TValue this[int key] => GetDataRow(key);
        private readonly List<TValue> listCache = new List<TValue>();
        public DataTable(string name)
        {
            Name = name;
            MinData = null;
            MaxData = null;
            dataSet = new Dictionary<int, TValue>();
            MinIndex = -1;
            MaxIndex = -1;
        }
        
        public bool HasDataRow(int key)
        {
            return dataSet.ContainsKey(key);
        }
        
        public bool HasDataRow(Predicate<TValue> condition)
        {
            foreach (KeyValuePair<int,TValue> valuePair in dataSet)
            {
                if (condition(valuePair.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public TValue GetDataRow(int key)
        {
            if (dataSet.TryGetValue(key, out var dataRow))
            {
                return dataRow;
            }
            return null;
        }
        
        public TValue GetDataRow(Predicate<TValue> condition)
        {
            foreach (KeyValuePair<int,TValue> valuePair in dataSet)
            {
                if (condition(valuePair.Value))
                {
                    return valuePair.Value;
                }
            }
            return null;
        }
        
        public TValue[] GetDataRows(Predicate<TValue> condition)
        {
            listCache.Clear();
            foreach (KeyValuePair<int,TValue> valuePair in dataSet)
            {
                if (condition(valuePair.Value))
                {
                    listCache.Add(valuePair.Value);
                }
            }
            return listCache.ToArray();
        }
        
        public void GetDataRows(Predicate<TValue> condition, List<TValue> results)
        {
            results.Clear();
            foreach (KeyValuePair<int,TValue> valuePair in dataSet)
            {
                if (condition(valuePair.Value))
                {
                    results.Add(valuePair.Value);
                }
            }
        }
        
        public TValue[] GetDataRows(Comparison<TValue> comparison)
        {
            listCache.Clear();
            foreach (KeyValuePair<int,TValue> valuePair in dataSet)
            {
                listCache.Add(valuePair.Value);
            }
            listCache.Sort(comparison);
            return listCache.ToArray();
        }

        public void GetDataRows(Comparison<TValue> comparison, List<TValue> results)
        {
            results.Clear();
            foreach (KeyValuePair<int,TValue> valuePair in dataSet)
            {
                results.Add(valuePair.Value);
            }
            results.Sort(comparison);
        }
        
        public TValue[] GetDataRows(Predicate<TValue> condition, Comparison<TValue> comparison)
        {
            listCache.Clear();
            foreach (KeyValuePair<int,TValue> dataRow in dataSet)
            {
                if (condition(dataRow.Value))
                {
                    listCache.Add(dataRow.Value);
                }
            }
            listCache.Sort(comparison);
            return listCache.ToArray();
        }

        public void GetDataRows(Predicate<TValue> condition, Comparison<TValue> comparison, List<TValue> results)
        {
            results.Clear();
            foreach (KeyValuePair<int,TValue> dataRow in dataSet)
            {
                if (condition(dataRow.Value))
                {
                    results.Add(dataRow.Value);
                }
            }
            results.Sort(comparison);
        }

        public TValue[] GetAllDataRows()
        {
            int index = 0;
            TValue[] results = new TValue[dataSet.Count];
            foreach (KeyValuePair<int, TValue> dataRow in dataSet)
            {
                results[index++] = dataRow.Value;
            }
            return results;
        }

        public void GetAllDataRows(List<TValue> results)
        {
            results.Clear();
            foreach (KeyValuePair<int,TValue> dataRow in dataSet)
            {
                results.Add(dataRow.Value);
            }
        }

        private bool AddDataRow(int key,TValue val)
        {
            if (HasDataRow(key))
            {
                return false;
            }
            dataSet.Add(key,val);
            if (MinData == null || MinIndex.CompareTo(key) > 0)
            {
                MinData = val;
                MinIndex = key;
            }
            if (MaxData == null || MaxIndex.CompareTo(key) < 0)
            {
                MaxData = val;
                MaxIndex = key;
            }
            return true;
        }
        
        public void Shutdown()
        {
            foreach (KeyValuePair<int,TValue> dataRow in dataSet)
            {
                dataRow.Value.Clear();
            }
            listCache.Clear();
            dataSet.Clear();
            dataSet = null;
        }

        public void LoadData(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 4)
            {
                return;
            }
            using (ByteBuffer buffer = new ByteBuffer())
            {
                int linesCount = buffer.ReadInt();
                for (int i = 0; i < linesCount; i++)
                {
                    TValue value = new TValue();
                    value.Parse(buffer);
                    AddDataRow(value.GetKeyValue(), value);
                }
            }
        }

        public void LoadData(string str)
        {
            if (str == String.Empty)
            {
                return;
            }
            string content = str.Replace("\r", "");
            string[] lines = content.Split('\n');
            for (int i = 4; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#"))
                {
                    continue;
                }
                TValue value = new TValue();
                value.Parse(lines[i]);
                AddDataRow(value.GetKeyValue(), value);
            }
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            return dataSet.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dataSet.Values.GetEnumerator();
        }
    }
}