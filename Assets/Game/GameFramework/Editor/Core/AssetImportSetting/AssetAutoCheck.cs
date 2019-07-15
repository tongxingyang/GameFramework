using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.AssetImportSetting
{
    public static class AssetAutoCheck
    {
        [AttributeUsage(AttributeTargets.Method)]
        public class AssetAutoCheckAttribute:Attribute
        {
            public AssetAutoCheckAttribute(string name)
            {
                Name = name;
            }

            public string Name { private set; get; }
        }
        
        public static void StartAssetCheck(string path)
        {
            TextLogger.Instance.Path = path;
            var checkItems = GetAllCheckItem();
            for (int i = 0; i < checkItems.Length; i++)
            {
                var item = checkItems[i];
                TextLogger.Instance.WriteLine("=====================================");
                if (string.IsNullOrEmpty(item.Name))
                {
                    TextLogger.Instance.WriteLine(item.Method.Name);
                }
                else
                {
                    TextLogger.Instance.WriteLine(item.Name);
                }
                TextLogger.Instance.WriteLine("=====================================");
                try
                {
                    item.Method.Invoke(null, null);
                }
                catch (Exception e)
                {
                    TextLogger.Instance.WriteLine(" Error 执行" + item.Name + "时发生错误:");
                    TextLogger.Instance.WriteLine(e.ToString());
                }
            }
        }
        
        private static CheckItemVo[] GetAllCheckItem()
        {
            List<CheckItemVo> result = new List<CheckItemVo>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            Type autoCheckItemType = typeof(AssetAutoCheckAttribute);
            foreach (Assembly assembly in assemblys)
            {
                if (!assembly.GetName().Name.StartsWith("Assembly-CSharp"))
                {
                    continue;
                }

                Type[] types = assembly.GetExportedTypes();
                foreach (Type type in types)
                {
                    var allMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int i = 0; i < allMethods.Length; i++)
                    {
                        var methodInfo = allMethods[i];
                        var attributes = methodInfo.GetCustomAttributes(autoCheckItemType, true);
                        if (attributes.Length < 1)
                        {
                            continue;
                        }
                        AssetAutoCheckAttribute checkItem = attributes[0] as AssetAutoCheckAttribute;
                        if (checkItem == null)
                        {
                            continue;
                        }

                        CheckItemVo itemVo = new CheckItemVo
                        {
                            Method = methodInfo,
                            Name = checkItem.Name
                        };
                        result.Add(itemVo);
                    }
                }
            }
            return result.ToArray();
        }

        class CheckItemVo
        {
            public string Name;
            public MethodInfo Method;
        }
        
        
        
//        [MenuItem("Tools/AssetCheck/检测AssetBundle交叉依赖")]
//        static void CheckAssetBundleCross()
//        {
//            StringBuilder builder = new StringBuilder();
//            string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
//            foreach (string assetBundleName in allAssetBundleNames)
//            {
//                string[] dependencies = AssetDatabase.GetAssetBundleDependencies(assetBundleName, true);//获取一个bundle依赖的所有bundle
//                foreach (string dependency in dependencies)
//                {
//                    string[] dependencies2 = AssetDatabase.GetAssetBundleDependencies(dependency, true);
//                    foreach (string dependency2 in dependencies2)
//                    {
//                        if (string.Equals(assetBundleName, dependency2))
//                        {
//                            builder.AppendFormat("{0} <-> {1}\n", assetBundleName, dependency);
//                        }
//                    }
//                }
//            }
//
//            string txt = builder.ToString();
//            if (!string.IsNullOrEmpty(txt))
//            {
//                UnityEngine.Debug.LogError($"以下ab出现交叉依赖:\n{txt}");
//            }
//            else
//            {
//                UnityEngine.Debug.Log("没有出现交叉依赖");
//            }
//        }
//        
//        [@MenuItem("Tools/AssetCheck/材质使用Standard.shader")]
//        static void CheckMaterial()
//        {
//            string[] assets = AssetDatabase.FindAssets("t:Material", new string[] { "Assets" });
//            foreach (string asset in assets)
//            {
//                string path = AssetDatabase.GUIDToAssetPath(asset);
//                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
//                if (mat.shader && mat.shader.name == "Standard")
//                {
//                    mat.shader = Shader.Find("Standard");
//                }
//            }
//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
//        }
//
//        [MenuItem("辅助工具/检测/检测是否使用Standard-Material")]
//        static void CheckPrefab()
//        {
//            Material defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Data/Materials/Default_Material.mat");
//
//            string[] assets = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets" });
//            foreach (string asset in assets)
//            {
//                string path = AssetDatabase.GUIDToAssetPath(asset);
//                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//                Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
//                foreach (Renderer renderer in renderers)
//                {
//                    bool need = false;
//                    Material[] mats = renderer.sharedMaterials;
//                    for (int i = 0; i < mats.Length; ++i)
//                    {
//                        Material mat = mats[i];
//                        if (mat != null && mat.name == "Default-Material")
//                        {
//                            need = true;
//                            mats[i] = defaultMaterial;
//                        }
//                    }
//
//                    if (need)
//                    {
//                        renderer.sharedMaterials = mats;
//                    }
//                }
//            }
//
//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
//        }
        
    }
    
}