using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GameFramework.Utility.Json;

namespace GameFramework.Utility.PlayerPrefs
{
    public class PlayerPrefsUtility
    {
        public static void DeleteAll()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
        }

        public static void DeleteKey(string key)
        {
            UnityEngine.PlayerPrefs.DeleteKey(key);
        }

        public static bool HasKey(string key)
        {
            return UnityEngine.PlayerPrefs.HasKey(Encrypt(key, AppConst.PlayerPrefsConfig.Password));
        }

        public static void Save()
        {
            UnityEngine.PlayerPrefs.Save();
        }

        public static float GetFloat(string key, float defaultValue = 0.0f)
        {
            float retValue;

            string strValue = GetString(key);

            if (float.TryParse(strValue, out retValue))
            {
                return retValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            int retValue;

            string strValue = GetString(key);

            if (int.TryParse(strValue, out retValue))
            {
                return retValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            bool retValue;

            string strValue = GetString(key);

            if (bool.TryParse(strValue, out retValue))
            {
                return retValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public static string GetString(string key)
        {
            string strEncryptValue = GetRowString(key);
            return Decrypt(strEncryptValue, AppConst.PlayerPrefsConfig.Password);
        }


        public static string GetString(string key, string defaultValue)
        {
            string strEncryptValue = GetRowString(key, defaultValue);
            return Decrypt(strEncryptValue, AppConst.PlayerPrefsConfig.Password);
        }

        public static T GetObject<T>(string key, T defaultObj = default(T))
        {
            string json = GetString(key);
            if (string.IsNullOrEmpty(json))
            {
                return defaultObj;
            }

            return JsonUtility.ToObject<T>(json);
        }

        public static object GetObject(Type objectType, string key, object defaultObj = null)
        {
            string json = GetString(key);
            if (string.IsNullOrEmpty(json))
            {
                return defaultObj;
            }

            return JsonUtility.ToObject(objectType, json);
        }

        public static void SetFloat(string key, float value)
        {
            string strValue = System.Convert.ToString(value);
            SetString(key, strValue);
        }

        public static void SetInt(string key, int value)
        {
            string strValue = System.Convert.ToString(value);
            SetString(key, strValue);
        }

        public static void SetBool(string key, bool value)
        {
            string strValue = System.Convert.ToString(value);
            SetString(key, strValue);
        }

        public static void SetString(string key, string value)
        {
            UnityEngine.PlayerPrefs.SetString(Encrypt(key, AppConst.PlayerPrefsConfig.Password),
                Encrypt(value, AppConst.PlayerPrefsConfig.Password));
        }

        public static void SetObject(string key, object obj)
        {
            string json = JsonUtility.ToJson(obj);
            if (string.IsNullOrEmpty(json))
            {
                SetString(key, json);
            }
        }

        public static void SetObject<T>(string key, T obj)
        {
            string json = JsonUtility.ToJson(obj);
            if (string.IsNullOrEmpty(json))
            {
                SetString(key, json);
            }
        }

        private static string GetRowString(string key)
        {
            string strEncryptKey = Encrypt(key, AppConst.PlayerPrefsConfig.Password);
            string strEncryptValue = UnityEngine.PlayerPrefs.GetString(strEncryptKey);
            return strEncryptValue;
        }

        private static string GetRowString(string key, string defaultValue)
        {
            string strEncryptKey = Encrypt(key, AppConst.PlayerPrefsConfig.Password);
            string strEncryptDefaultValue = Encrypt(defaultValue, AppConst.PlayerPrefsConfig.Password);
            string strEncryptValue = UnityEngine.PlayerPrefs.GetString(strEncryptKey, strEncryptDefaultValue);
            return strEncryptValue;
        }

        static byte[] GetIV()
        {
            byte[] IV = Encoding.UTF8.GetBytes(AppConst.PlayerPrefsConfig.Salt);
            return IV;
        }

        static string Encrypt(string strPlain, string password)
        {
            if (!AppConst.PlayerPrefsConfig.useSecure)
                return strPlain;
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Rfc2898DeriveBytes rfc2898DeriveBytes =
                    new Rfc2898DeriveBytes(password, GetIV(), AppConst.PlayerPrefsConfig.Iterations);
                byte[] key = rfc2898DeriveBytes.GetBytes(8);
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(key, GetIV()),
                    CryptoStreamMode.Write))
                {
                    memoryStream.Write(GetIV(), 0, GetIV().Length);
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(strPlain);
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("Encrypt Exception: " + e);
                return strPlain;
            }
        }

        static string Decrypt(string strEncript, string password)
        {
            if (!AppConst.PlayerPrefsConfig.useSecure)
                return strEncript;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(strEncript);
                using (var memoryStream = new MemoryStream(cipherBytes))
                {
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    byte[] iv = GetIV();
                    memoryStream.Read(iv, 0, iv.Length);
                    var rfc2898DeriveBytes =
                        new Rfc2898DeriveBytes(password, iv, AppConst.PlayerPrefsConfig.Iterations);
                    byte[] key = rfc2898DeriveBytes.GetBytes(8);
                    using (var cryptoStream =
                        new CryptoStream(memoryStream, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        string strPlain = streamReader.ReadToEnd();
                        return strPlain;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("Decrypt Exception: " + e);
                return strEncript;
            }
        }
    }
}