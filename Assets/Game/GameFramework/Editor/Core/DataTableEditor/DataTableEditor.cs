using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameFramework.DataTable.Base;
using GameFramework.Editor.Core.DataTableEditor;
using GameFramework.Utility;
using GameFramework.Utility.File;
using UnityEditor;
using UnityEngine;

public class DataTableEditor : EditorWindow
{

    
//    [MenuItem(@"Assets/Tool/DataTable/SelectTable2Bytes")]
//    private static void MakeTable2Bytes()
//    {
//        TextAsset[] textAssets = Selection.GetFiltered<TextAsset>(SelectionMode.DeepAssets);
//        Table2Bytes(textAssets);
//        EditorUtility.DisplayDialog("Finish", "tables "+textAssets[0].name+" processed finish", "OK");
//    }
//    [MenuItem(@"Assets/Tool/DataTable/AllTable2Bytes")]
//    private static void MakeAllTable2Bytes()
//    {
//        TextAsset[] textAssets = Selection.GetFiltered<TextAsset>(SelectionMode.DeepAssets);
//        Table2Bytes(textAssets);
//        EditorUtility.DisplayDialog("Finish", "tables "+textAssets[0].name+" processed finish", "OK");
//    }


    private string dataTableName = "";
    
    public void OnEnable()
    {
        
    }

    public void OnGUI()
    {
        GUILayout.Label("DataTable Setting");
        GUILayout.Space(10);
        EditorGUILayout.LabelField("忽略的DataTable文件:",EditorStyles.boldLabel);
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal("box");
        {
            EditorGUILayout.LabelField("DataTableName:", GUILayout.MinWidth(100));
            dataTableName = EditorGUILayout.TextField(dataTableName);
            if (GUILayout.Button("添加DataTable", GUILayout.MinWidth(100)))
            {
                if (string.IsNullOrEmpty(dataTableName))
                {
                    return;
                }
                DataTableBuildManager.DataTableRule.IgnoreDataTable.Add(dataTableName);
            }
            if (GUILayout.Button("移除DataTable", GUILayout.MinWidth(100)))
            {
                if (string.IsNullOrEmpty(dataTableName))
                {
                    return;
                }
                DataTableBuildManager.DataTableRule.IgnoreDataTable.Remove(dataTableName);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        foreach (string dataTable in DataTableBuildManager.DataTableRule.IgnoreDataTable)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("忽略的DataTable文件 :");
                EditorGUILayout.LabelField(dataTable);
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.Space(15);
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Csv DataTable 目录:", GUILayout.Width(100));
            if (GUILayout.Button("Browse..."))
            {
                string directory = EditorUtility.OpenFolderPanel("选择文件夹", DataTableBuildManager.DataTableRule.DataTablePath, string.Empty);
                if (!string.IsNullOrEmpty(directory))
                {
                    DataTableBuildManager.DataTableRule.DataTablePath = directory;
                }
            }
            
            EditorGUILayout.LabelField("Output Byte DataTable 目录:", GUILayout.Width(100));
            if (GUILayout.Button("Browse..."))
            {
                string directory = EditorUtility.OpenFolderPanel("选择文件夹", DataTableBuildManager.DataTableRule.BytePath, string.Empty);
                if (!string.IsNullOrEmpty(directory))
                {
                    DataTableBuildManager.DataTableRule.BytePath = directory;
                }
            }
            
            EditorGUILayout.LabelField("Output Lua DataTable 目录:", GUILayout.Width(100));
            if (GUILayout.Button("Browse..."))
            {
                string directory = EditorUtility.OpenFolderPanel("选择文件夹", DataTableBuildManager.DataTableRule.LuaPath, string.Empty);
                if (!string.IsNullOrEmpty(directory))
                {
                    DataTableBuildManager.DataTableRule.LuaPath = directory;
                }
            }
            
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal("box");
        {
            DataTableBuildManager.DataTableRule.IsByte = EditorGUILayout.ToggleLeft("生成Byte DataTable", DataTableBuildManager.DataTableRule.IsByte);
            DataTableBuildManager.DataTableRule.IsLua = EditorGUILayout.ToggleLeft("生成Lua  DataTable", DataTableBuildManager.DataTableRule.IsLua);
            DataTableBuildManager.DataTableRule.IsCs = EditorGUILayout.ToggleLeft("生成DataTable Cs Code", DataTableBuildManager.DataTableRule.IsCs);
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("生成Byte File"))
            {
                if (string.IsNullOrEmpty(DataTableBuildManager.DataTableRule.BytePath) ||
                    string.IsNullOrEmpty(DataTableBuildManager.DataTableRule.DataTablePath))
                {
                    return;
                }
                DataTableBuildManager.GenByteDateTableCode();
            }
            
            if (GUILayout.Button("生成Lua File"))
            {
                if (string.IsNullOrEmpty(DataTableBuildManager.DataTableRule.LuaPath) ||
                    string.IsNullOrEmpty(DataTableBuildManager.DataTableRule.DataTablePath))
                {
                    return;
                }
                DataTableBuildManager.GenLuaDateTableCode();
            }
            
            if (GUILayout.Button("生成Lua File"))
            {
                if (string.IsNullOrEmpty(DataTableBuildManager.DataTableRule.ExportCsPath) ||
                    string.IsNullOrEmpty(DataTableBuildManager.DataTableRule.DataTablePath))
                {
                    return;
                }
                DataTableBuildManager.GenCsDateTableCode();
            }
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        GUILayout.Label("Localization Setting");
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Output Localization 目录:", GUILayout.Width(100));
        if (GUILayout.Button("Browse..."))
        {
            string directory = EditorUtility.OpenFolderPanel("选择文件夹", DataTableBuildManager.DataTableRule.LocalizationPath, string.Empty);
            if (!string.IsNullOrEmpty(directory))
            {
                DataTableBuildManager.DataTableRule.LocalizationPath = directory;
            }
        }
        
        EditorGUILayout.LabelField("Localization File:", GUILayout.Width(100));
        if (GUILayout.Button("Browse..."))
        {
            string filePath = EditorUtility.OpenFilePanel("选择文件夹", DataTableBuildManager.DataTableRule.LocalizationPath, string.Empty);
            if (!string.IsNullOrEmpty(filePath))
            {
                DataTableBuildManager.DataTableRule.LocalizationFile = filePath;
            }
        }
        GUILayout.EndHorizontal();
        DataTableBuildManager.DataTableRule.LocalizationFile = EditorGUILayout.TextField(DataTableBuildManager.DataTableRule.LocalizationFile);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("生成 Localization File"))
        {
            if (string.IsNullOrEmpty(DataTableBuildManager.DataTableRule.LocalizationFile) || string.IsNullOrEmpty(DataTableBuildManager.DataTableRule.LocalizationPath))
            {
                return;
            }
            DataTableBuildManager.GenLocalizationCode();
        }
        GUILayout.EndHorizontal();
    }   

    [MenuItem("Editor/Tool/DataTableWindow")]
    private static void ShowWindow()
    {
        DataTableEditor dataTableWindow = EditorWindow.GetWindow<DataTableEditor>(true, "DataTable 工具");
        dataTableWindow.minSize  = new Vector2(1000, 600);
    }
    
}
