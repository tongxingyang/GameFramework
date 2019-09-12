using UnityEngine;
using UnityEditor;
using System.Collections;

namespace I2.TextAnimation
{
    public static class UpgradeManagerHelper
    {
        public static bool HasAttributeOfType<T>(this System.Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0;
        }
    }
}