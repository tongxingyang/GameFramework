using System;
using System.Collections.Generic;

namespace GameFramework.DataTable.Base
{
    public class DataTable<T> : IDataTable where T :class ,new()
    {
        private string name;
        public string Name => name;
        public int Id { get; }
        public int Count => dataSet?.Count ?? 0;
        public Type Type => typeof(T);
        protected Dictionary<int, T> dataSet = null;
        private int minIndex = -1;
        private T minIdData;
        public T MinIdData => minIdData;
        private int maxIndex = -1;
        private T maxIdData;
        public T MaxIdData => maxIdData;

        public T this[int id] => GetDataRow(id);
        
        public DataTable(string name)
        {
            this.name = name;
            minIdData = null;
            maxIdData = null;
            dataSet = new Dictionary<int, T>();
            minIndex = -1;
            maxIndex = -1;
        }

        public bool HasDataRow(int id)
        {
            return dataSet.ContainsKey(id);
        }

        public bool HasDataRow(Predicate<T> condition)
        {
            foreach (KeyValuePair<int,T> valuePair in dataSet)
            {
                if (condition(valuePair.Value))
                {
                    return true;
                }
            }
            return false;
        }
        
        public T GetDataRow(int id)
        {
            T dataRow = null;
            if (dataSet.TryGetValue(id, out dataRow))
            {
                return dataRow;
            }
            return null;
        }

        public T GetDataRow(Predicate<T> condition)
        {
            foreach (KeyValuePair<int,T> valuePair in dataSet)
            {
                if (condition(valuePair.Value))
                {
                    return valuePair.Value;
                }
            }
            return null;
        }

        public T[] GetDataRows(Predicate<T> condition)
        {
            List<T> results = new List<T>();
            foreach (KeyValuePair<int,T> valuePair in dataSet)
            {
                if (condition(valuePair.Value))
                {
                    results.Add(valuePair.Value);
                }
            }
            return results.ToArray();
        }

        public void GetDataRows(Predicate<T> condition, List<T> results)
        {
            results.Clear();
            foreach (KeyValuePair<int,T> valuePair in dataSet)
            {
                if (condition(valuePair.Value))
                {
                    results.Add(valuePair.Value);
                }
            }
        }

        public T[] GetDataRows(Comparison<T> comparison)
        {
            List<T> results = new List<T>();
            foreach (KeyValuePair<int,T> valuePair in dataSet)
            {
                results.Add(valuePair.Value);
            }
            results.Sort(comparison);
            return results.ToArray();
        }

        public void GetDataRows(Comparison<T> comparison, List<T> results)
        {
            results.Clear();
            foreach (KeyValuePair<int,T> valuePair in dataSet)
            {
                results.Add(valuePair.Value);
            }
            results.Sort(comparison);
        }
        
        public T[] GetDataRows(Predicate<T> condition, Comparison<T> comparison)
        {
            List<T> results = new List<T>();
            foreach (KeyValuePair<int, T> dataRow in dataSet)
            {
                if (condition(dataRow.Value))
                {
                    results.Add(dataRow.Value);
                }
            }
            results.Sort(comparison);
            return results.ToArray();
        }

        public void GetDataRows(Predicate<T> condition, Comparison<T> comparison, List<T> results)
        {
            results.Clear();
            foreach (KeyValuePair<int, T> dataRow in dataSet)
            {
                if (condition(dataRow.Value))
                {
                    results.Add(dataRow.Value);
                }
            }
            results.Sort(comparison);
        }

        public T[] GetAllDataRows()
        {
            int index = 0;
            T[] results = new T[dataSet.Count];
            foreach (KeyValuePair<int, T> dataRow in dataSet)
            {
                results[index++] = dataRow.Value;
            }

            return results;
        }

        public void GetAllDataRows(List<T> results)
        {
            results.Clear();
            foreach (KeyValuePair<int, T> dataRow in dataSet)
            {
                results.Add(dataRow.Value);
            }
        }

        protected bool AddDataRow(int id,T val)
        {
            if (HasDataRow(id))
            {
                return false;
            }
            dataSet.Add(id,val);
            if (minIdData == null || minIndex > id)
            {
                minIdData = val;
                minIndex = id;
            }
            if (maxIdData == null || maxIndex < id)
            {
                maxIdData = val;
                maxIndex = id;
            }
            return true;
        }
        
        public void Shotdown()
        {
            dataSet.Clear();
        }

        public virtual void LoadData(byte[] bytes){}

        public virtual void LoadData(string str){}

    }
}