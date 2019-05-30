using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameFramework.DataTable.Base;
using GameFramework.Utility;
using GameFramework.Utility.File;
using UnityEditor;
using UnityEngine;

public class DataTableWindow : EditorWindow
{
    public static string SrcDir = "";
    public static string DestDir = "";
    public static char[] ListSeparator = new char[] { '|' };
    public static char[] SpaceSeparator = new char[] { ' ' };
    public static char[] eof = { '\r', '\n' };
    
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
    
    private static void Table2Bytes(string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            FileInfo fileInfo = new FileInfo(paths[i]);
            int index = fileInfo.Name.LastIndexOf('.');
            string destName = DestDir + fileInfo.Name.Substring(0, index) + ".bytes";
            WriteBytes(fileInfo,destName,Encoding.UTF8);
        }
    }

    private static void WriteBytes(FileInfo fileInfo, string destName, Encoding encoding)
    {
        using (StreamReader streamReader = new StreamReader(fileInfo.FullName,encoding))
        {
            string title = streamReader.ReadLine();
            string comment = streamReader.ReadLine();
            string mode = streamReader.ReadLine();
            string[] modes = SplitLine(mode);
            bool[] useIndex = new bool[modes.Length];
            
            for (int i = 0; i < modes.Length; i++)
            {
                useIndex[i] = modes[i].Equals("A") || modes[i].Equals("C");
            }
            
            string type = streamReader.ReadLine();
            string[] titles = RemoveUnuse(SplitLine(title.TrimEnd(eof)), useIndex);
            string[] comments = RemoveUnuse(SplitLine(comment.TrimEnd(eof)), useIndex);
            string[] types = RemoveUnuse(SplitLine(type.TrimEnd(eof)), useIndex);
            using (ByteBuffer buffer = new ByteBuffer())
            {
                int lineCount = 0;
                buffer.Seek(4, SeekOrigin.Begin);
                while (true)
                {
                    string line = streamReader.ReadLine();
                    if(string.IsNullOrEmpty(line)) break;
                    if(line.StartsWith("#")) continue;
                    string[] colums = RemoveUnuse(SplitLine(line.TrimEnd(eof)), useIndex);
                    
                    for (int i = 0; i < colums.Length; i++)
                    {
                        AddParseValue(buffer,types[i],colums[i]);
                    }
                    lineCount++;
                }
                buffer.Seek(0, SeekOrigin.Begin);
                ValueParse.WriteValue(buffer,lineCount,ValueParse.IntParse);
                
                if (FileUtility.IsFileExist(destName))
                {
                    FileUtility.DeleteFile(destName);
                }
                FileUtility.WriteBytesToFile("Assets/TestTable.bytes",buffer.ToBytes());
            }
        }
    }

    
    private static void AddParseValue(ByteBuffer buffer,string type, string data)
    {
        if (type == "byte")
        {
            byte value = 0;
            byte.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.ByteParse);
        }else if (type == "sbyte")
        {
            sbyte value = 0;
            sbyte.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.SByteParse);
        }else if (type == "bool")
        {
            bool value = false;
            bool.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.BoolParse);
        }else if (type == "char")
        {
            char value = ' ';
            char.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.CharParse);
        }else if (type == "short")
        {
            short value = 0;
            short.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.ShortParse);
        }else if (type == "ushort")
        {
            ushort value = 0;
            ushort.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.UShortParse);
        }else if (type == "uint")
        {
            uint value = 0;
            uint.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.UIntParse);
        }else if (type == "int")
        {
            int value = 0;
            int.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.IntParse);
        }else if (type == "ulong")
        {
            ulong value = 0;
            ulong.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.ULongParse);
        }else if (type == "long")
        {
            long value = 0;
            long.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.LongParse);
        }else if (type == "float")
        {
            float value = 0;
            float.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.FloatParse);
        }else if (type == "double")
        {
            double value = 0;
            double.TryParse(data, out value);
            ValueParse.WriteValue(buffer,value,ValueParse.DoubleParse);   
        }else if (type == "string")
        {
            ValueParse.WriteValue(buffer,data,ValueParse.StringParse);   
        }else if (type == "vector2")
        {
            string[] arr = data.Split('@');
            Vector2 value = new Vector2(float.Parse(arr[0]),float.Parse(arr[1]));
            ValueParse.WriteValue(buffer,value,ValueParse.Vector2Parse);
        }else if (type == "vector3")
        {
            string[] arr = data.Split('@');
            Vector3 value = new Vector3(float.Parse(arr[0]),float.Parse(arr[1]),float.Parse(arr[2]));
            ValueParse.WriteValue(buffer,value,ValueParse.Vector3Parse);
        }else if (type == "vector4")
        {
            string[] arr = data.Split('@');
            Vector4 value = new Vector4(float.Parse(arr[0]),float.Parse(arr[1]),float.Parse(arr[2]),float.Parse(arr[3]));
            ValueParse.WriteValue(buffer,value,ValueParse.Vector4Parse);
        }else if (type == "color32")
        {
            string[] arr = data.Split('@');
            Color32 value = new Color32(byte.Parse(arr[0]),byte.Parse(arr[1]),byte.Parse(arr[2]),byte.Parse(arr[3]));
            ValueParse.WriteValue(buffer,value,ValueParse.Color32Parse);
        }else if (type == "color")
        {
            string[] arr = data.Split('@');
            Color value = new Color(float.Parse(arr[0]),float.Parse(arr[1]),float.Parse(arr[2]),float.Parse(arr[3]));
            ValueParse.WriteValue(buffer,value,ValueParse.ColorParse);
        }else if (type == "datetime")
        {
            DateTime value = new DateTime(long.Parse(data));
            ValueParse.WriteValue(buffer,value,ValueParse.DateTimeParse);
        }else if (type == "rect")
        {
            string[] arr = data.Split('@');
            Rect value = new Rect(float.Parse(arr[0]),float.Parse(arr[1]),float.Parse(arr[2]),float.Parse(arr[3]));
            ValueParse.WriteValue(buffer,value,ValueParse.RectParse);
        }else if (type == "quaternion")
        {
            string[] arr = data.Split('@');
            Quaternion value = new Quaternion(float.Parse(arr[0]),float.Parse(arr[1]),float.Parse(arr[2]),float.Parse(arr[3]));
            ValueParse.WriteValue(buffer,value,ValueParse.QuaternionParse);
        }else if (type.StartsWith("list"))
        {
            string listType = type.Split('|')[1];
            string[] datas = data.Split('|');
            if (datas.Length > 0)
            {
                ValueParse.WriteValue(buffer,(byte)datas.Length,ValueParse.ByteParse);
                for (int i = 0; i < datas.Length; i++)
                {
                    AddParseValue(buffer, listType, datas[i]);
                }
            }
            else
            {
                ValueParse.WriteValue(buffer,(byte)0,ValueParse.ByteParse);
            }
           
        }else if (type.StartsWith("dirc"))
        {
            string ktype = type.Split('|')[1];
            string vtype = type.Split('|')[2];
            string[] datas = data.Split('|');
            if (datas.Length > 0)
            {
                ValueParse.WriteValue(buffer,(byte)datas.Length,ValueParse.ByteParse);
                for (int i = 0; i < datas.Length; i++)
                {
                    var key = datas[i].Split(':')[0];
                    var value = datas[i].Split(':')[1];
                    AddParseValue(buffer, ktype, key);
                    AddParseValue(buffer, vtype, value);
                }
            }
            else
            {
                ValueParse.WriteValue(buffer,(byte)0,ValueParse.ByteParse);
            }
            
        }
    }
    
    private static string[] SplitLine(string line)
    {
        line = line.TrimEnd(eof);
        return line.Split(',');
    }

    private static string[] RemoveUnuse(string[] arr,bool[] useList)
    {
       List<string> list = new List<string>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (useList[i])
            {
                list.Add(arr[i]);
            }
        }
        return list.ToArray();
    }
    
    public void OnEnable()
    {
        
    }

    public void OnGUI()
    {
        GUILayout.Label("DataTable Setting");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("搜索",GUILayout.Width(50f)))
        {
            
        }
        GUILayout.Label("加载所有的DataTable");
        GUILayout.EndHorizontal();
    }

    DataTableWindow()
    {
        titleContent = new GUIContent("DataTable Editor");
    }
        

    [MenuItem("Editor/Tool/DataTableWindow")]
    private static void ShowWindow()
    {
        DataTableWindow dataTableWindow = EditorWindow.GetWindow<DataTableWindow>(true, "DataTable");
        dataTableWindow.minSize  = new Vector2(500, 600);
    }
    
}
