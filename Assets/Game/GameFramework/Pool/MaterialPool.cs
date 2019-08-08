using System.Collections.Generic;
using GameFramework.Utility;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Pool
{
    public class MaterialPool : Singleton<MaterialPool>
    {
        private const string RunTimeTag = "[RunTime]";
        private const int MaxMaterialCount = 40;
        private readonly Queue<Material> materials = new Queue<Material>();
        private Material emptyMaterial = null;
        private int totalCreate = 0;
        private int totalDelete = 0;

        public override void Init()
        {
            base.Init();
            if (emptyMaterial == null)
            {
                emptyMaterial = Resources.Load<Material>("EmptyMaterial");
            }
        }

        public Material GetMaterial(Material originMaterial)
        {
            if (originMaterial == null)
            {
                return null;
            }
            Material material = null;
            if (materials.Count >= 1)
            {
                material = materials.Dequeue();
            }
            if (material != null)
            {
                material.shader = originMaterial.shader;
                material.CopyPropertiesFromMaterial(originMaterial);
            }
            else
            {
                totalCreate++;
                material = new Material(originMaterial);
            }

            material.name = StringUtility.Format("{0}{1}", RunTimeTag, originMaterial.name);
#if UNITY_EDITOR
            material.hideFlags = HideFlags.DontSave;
#else
            material.hideFlags = HideFlags.HideAndDontSave;
#endif
            return material;
        }

        public void RecycleMaterial(Material material)
        {
            if (material == null)
            {
                return;
            }
            if (materials.Count >= MaxMaterialCount)
            {
                Object.Destroy(material);
                totalDelete++;
            }
            else
            {
                material.shader = emptyMaterial.shader;
                material.CopyPropertiesFromMaterial(emptyMaterial);
                materials.Enqueue(material);
            }
        }

        public void ClearUpMaterials()
        {
            foreach (Material material in materials)
            {
                Object.Destroy(material);
            }
            materials.Clear();
            totalCreate = 0;
            totalDelete = 0;
        }
    }
}