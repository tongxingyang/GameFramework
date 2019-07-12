using UnityEngine;

namespace GameFramework.DevelopTool
{
    public partial class DevelopComponent
    {
        public class DevelopDebugSystem
        {
            public void DrawSystem(int windowId)
            {
                GUI.DragWindow(Instance._dragRect);
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Instance.SetShowType(ShowType.ShowSelect);
                }
                GUILayout.Space(10);
                GUILayout.Label("<b>PersistentDataPath</b>");
                GUILayout.Label("<b>设备信息</b>");
                GUILayout.BeginVertical("box");
                {
                    Instance.DrawItem("操作系统:" , SystemInfo.operatingSystem);
                    Instance.DrawItem("系统内存:" , SystemInfo.systemMemorySize + "MB");
                    Instance.DrawItem("处理器:" , SystemInfo.processorType);
                    Instance.DrawItem("处理器数量:" , SystemInfo.processorCount.ToString());
                    Instance.DrawItem("设备模式:" , SystemInfo.deviceModel);
                    Instance.DrawItem("设备名称:" , SystemInfo.deviceName);
                    Instance.DrawItem("设备类型:" , SystemInfo.deviceType.ToString());
                    Instance.DrawItem("设备标识:" , SystemInfo.deviceUniqueIdentifier);
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>显卡信息</b>");
                GUILayout.BeginVertical("box");
                {
                    Instance.DrawItem("显卡:" , SystemInfo.graphicsDeviceName);
                    Instance.DrawItem("显卡类型:" , SystemInfo.graphicsDeviceType.ToString());
                    Instance.DrawItem("显存:" , SystemInfo.graphicsMemorySize + "MB");
                    Instance.DrawItem("显卡标识:" , SystemInfo.graphicsDeviceID.ToString());
                    Instance.DrawItem("显卡供应商:" , SystemInfo.graphicsDeviceVendor);
                    Instance.DrawItem("显卡供应商标识码:" , SystemInfo.graphicsDeviceVendorID.ToString());
                    Instance.DrawItem("Shader Level:", GetShaderLevelString(SystemInfo.graphicsShaderLevel));
                    Instance.DrawItem("Supports Shadows:", SystemInfo.supportsShadows.ToString());
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>Quality信息</b>");
                GUILayout.BeginVertical("box");
                {
                    string value = "";
                    if (QualitySettings.GetQualityLevel() == 0)
                    {
                        value = "[最低]";
                    }else if (QualitySettings.GetQualityLevel() == QualitySettings.names.Length - 1)
                    {
                        value = "[最高]";
                    }
                    Instance.DrawItem("图形质量: ",QualitySettings.names[QualitySettings.GetQualityLevel()]+value);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("降低一级图形质量"))
                    {
                        QualitySettings.DecreaseLevel();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("提高一级图形质量"))
                    {
                        QualitySettings.DecreaseLevel();
                    }
                    GUILayout.EndHorizontal();
                    Instance.DrawItem("Master Texture Limit:", QualitySettings.masterTextureLimit.ToString());
                    Instance.DrawItem("Blend Weights:", QualitySettings.skinWeights.ToString());
                    Instance.DrawItem("VSync Count:", QualitySettings.vSyncCount.ToString());
                    Instance.DrawItem("LOD Bias:", QualitySettings.lodBias.ToString());
                    Instance.DrawItem("Maximum LOD Level:", QualitySettings.maximumLODLevel.ToString());
                }
                GUILayout.EndVertical();
                
                GUILayout.Label("<b>Screen信息</b>");
                GUILayout.BeginVertical("box");
                { 
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("全屏显示"))
                    {
                        Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height,!Screen.fullScreen);
                    }
                    GUILayout.EndHorizontal();
                    Instance.DrawItem("DPI :", Screen.dpi.ToString());
                    Instance.DrawItem("分辨率 :", Screen.currentResolution.ToString());
                }
                GUILayout.EndVertical();
            }

            private string GetShaderLevelString(int shaderLevel)
            {
                return string.Format("Shader Model {0}.{1}", (shaderLevel / 10).ToString(),
                    (shaderLevel % 10).ToString());
            }
        }
    }
}