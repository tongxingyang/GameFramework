using System;
using System.Text;
using GameFramework.Debug;
using UnityEngine;

namespace GameFramework.Utility.Json
{
    public class JsonUtility
    {
        public static string ToJson(object obj)
        {
            try
            {
                return JsonUtility.ToJson(obj);
            }
            catch (Exception e)
            {
                Debuger.LogError("cant not to string  object name : "+obj+" "+e.Message);
                return String.Empty;
            }
        }

        public static byte[] ToJsonData(object obj)
        {
            return Encoding.UTF8.GetBytes(ToJson(obj));
        }

        public static T ToObject<T>(string json)
        {
            try
            {
                return UnityEngine.JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debuger.LogError("cant not to object  type name : "+typeof(T).Name+" "+e.Message);
                return default(T);
            }
        }

        public static object ToObject(Type objectTpye, string json)
        {
            try
            {
                return UnityEngine.JsonUtility.FromJson(json,objectTpye);
            }
            catch (Exception e)
            {
                Debuger.LogError("cant not to object  type name : "+objectTpye.Name+" "+e.Message);
                return null;
            }
        }

        public static T ToObject<T>(byte[] jsondata)
        {
            return ToObject<T>(Encoding.UTF8.GetString(jsondata));
        }

        public static object ToObject(Type objectType, byte[] jsondata)
        {
            return ToObject(objectType, Encoding.UTF8.GetString(jsondata));
        }
    }
}