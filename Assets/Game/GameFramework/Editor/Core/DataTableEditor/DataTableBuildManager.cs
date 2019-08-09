using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameFramework.Utility;
using GameFramework.Utility.File;
using GameFramework.Utility.PathUtility;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.DataTableEditor
{
    public class DataTableBuildManager
    {
        public static char[] ListSeparator = new char[] {'|'};
        public static char[] SpaceSeparator = new char[] {' '};
        public static char[] eof = {'\r', '\n'};
        public static string LocalizationCodePath = Application.dataPath+"/Game/GameFramework/Localization/enLanguageKey.cs";

        public static string DataTableBuildRulePath =
            "Assets/Game/GameFramework/Editor/EditorAsset/DataTableBuildRules.asset";

        public class DataTableBuildRule : ScriptableObject
        {
            public string DataTablePath = "";
            public HashSet<string> IgnoreDataTable = new HashSet<string>();
            public string LocalizationFile = "";
            public bool IsByte = true;
            public string BytePath = "";
            public bool IsLua = true;
            public string LuaPath = "";
            public string LocalizationPath = "";
            public bool IsCs = true;
            public string ExportCsPath = "";
        }

        private static DataTableBuildRule dataTableRule;

        public static DataTableBuildRule DataTableRule
        {
            get
            {
                if (dataTableRule == null)
                {
                    dataTableRule = AssetDatabase.LoadAssetAtPath<DataTableBuildRule>(DataTableBuildRulePath);
                    if (dataTableRule == null)
                    {
                        dataTableRule = ScriptableObject.CreateInstance<DataTableBuildRule>();
                        AssetDatabase.CreateAsset(dataTableRule, DataTableBuildRulePath);
                        AssetDatabase.SaveAssets();
                    }
                }
                return dataTableRule;
            }
        }

        private static void AddParseValue(ByteBuffer buffer, string type, string data)
        {
            if (type == "byte")
            {
                byte value = 0;
                byte.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.ByteParse);
            }
            else if (type == "sbyte")
            {
                sbyte value = 0;
                sbyte.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.SByteParse);
            }
            else if (type == "bool")
            {
                bool value = false;
                bool.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.BoolParse);
            }
            else if (type == "char")
            {
                char value = ' ';
                char.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.CharParse);
            }
            else if (type == "short")
            {
                short value = 0;
                short.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.ShortParse);
            }
            else if (type == "ushort")
            {
                ushort value = 0;
                ushort.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.UShortParse);
            }
            else if (type == "uint")
            {
                uint value = 0;
                uint.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.UIntParse);
            }
            else if (type == "int")
            {
                int value = 0;
                int.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.IntParse);
            }
            else if (type == "ulong")
            {
                ulong value = 0;
                ulong.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.ULongParse);
            }
            else if (type == "long")
            {
                long value = 0;
                long.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.LongParse);
            }
            else if (type == "float")
            {
                float value = 0;
                float.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.FloatParse);
            }
            else if (type == "double")
            {
                double value = 0;
                double.TryParse(data, out value);
                ValueParse.WriteValue(buffer, value, ValueParse.DoubleParse);
            }
            else if (type == "string")
            {
                ValueParse.WriteValue(buffer, data, ValueParse.StringParse);
            }
            else if (type == "vector2")
            {
                string[] arr = data.Split('@');
                Vector2 value = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
                ValueParse.WriteValue(buffer, value, ValueParse.Vector2Parse);
            }
            else if (type == "vector3")
            {
                string[] arr = data.Split('@');
                Vector3 value = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                ValueParse.WriteValue(buffer, value, ValueParse.Vector3Parse);
            }
            else if (type == "vector4")
            {
                string[] arr = data.Split('@');
                Vector4 value = new Vector4(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]),
                    float.Parse(arr[3]));
                ValueParse.WriteValue(buffer, value, ValueParse.Vector4Parse);
            }
            else if (type == "color32")
            {
                string[] arr = data.Split('@');
                Color32 value = new Color32(byte.Parse(arr[0]), byte.Parse(arr[1]), byte.Parse(arr[2]),
                    byte.Parse(arr[3]));
                ValueParse.WriteValue(buffer, value, ValueParse.Color32Parse);
            }
            else if (type == "color")
            {
                string[] arr = data.Split('@');
                Color value = new Color(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]),
                    float.Parse(arr[3]));
                ValueParse.WriteValue(buffer, value, ValueParse.ColorParse);
            }
            else if (type == "datetime")
            {
                DateTime value = new DateTime(long.Parse(data));
                ValueParse.WriteValue(buffer, value, ValueParse.DateTimeParse);
            }
            else if (type == "rect")
            {
                string[] arr = data.Split('@');
                Rect value = new Rect(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]),
                    float.Parse(arr[3]));
                ValueParse.WriteValue(buffer, value, ValueParse.RectParse);
            }
            else if (type == "quaternion")
            {
                string[] arr = data.Split('@');
                Quaternion value = new Quaternion(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]),
                    float.Parse(arr[3]));
                ValueParse.WriteValue(buffer, value, ValueParse.QuaternionParse);
            }
            else if (type.StartsWith("list"))
            {
                string listType = type.Split('|')[1];
                string[] datas = data.Split('|');
                if (datas.Length > 0)
                {
                    ValueParse.WriteValue(buffer, (ushort) datas.Length, ValueParse.UShortParse);
                    for (int i = 0; i < datas.Length; i++)
                    {
                        AddParseValue(buffer, listType, datas[i]);
                    }
                }
                else
                {
                    ValueParse.WriteValue(buffer, (ushort) 0, ValueParse.UShortParse);
                }
            }
            else if (type.StartsWith("dirc"))
            {
                string ktype = type.Split('|')[1];
                string vtype = type.Split('|')[2];
                string[] datas = data.Split('|');
                if (datas.Length > 0)
                {
                    ValueParse.WriteValue(buffer, (ushort) datas.Length, ValueParse.UShortParse);
                    foreach (string date in datas)
                    {
                        var key = date.Split(':')[0];
                        var value = date.Split(':')[1];
                        AddParseValue(buffer, ktype, key);
                        AddParseValue(buffer, vtype, value);
                    }
                }
                else
                {
                    ValueParse.WriteValue(buffer, (ushort) 0, ValueParse.UShortParse);
                }
            }
        }

        private static string[] SplitLine(string line)
        {
            line = line.TrimEnd(eof);
            return SplitCsvLine(line);
        }

        private static string CsvLineSeperator = @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)";

        public static string[] SplitCsvLine(string line)
        {
            var value = (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(
                    line,
                    CsvLineSeperator, System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value);
            return value.ToArray();
        }

        private static string[] RemoveUnuse(string[] arr, bool[] useList)
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
        
        public static void GenLocalizationCode()
        {
            FileInfo fileInfo = new FileInfo(DataTableRule.LocalizationFile);
            using (StreamReader streamReader = new StreamReader(fileInfo.FullName, Encoding.UTF8))
            {
                var readLine = streamReader.ReadLine();
                if (readLine != null)
                {
                    var heads = readLine.Split(',');
                    StreamWriter[] writes = new StreamWriter[heads.Length];
                    File.WriteAllText(LocalizationCodePath,"");
                    writes[0] = new StreamWriter(LocalizationCodePath,true,Encoding.UTF8);
                    writes[0].WriteLine("namespace GameFramework.Localization");
                    writes[0].WriteLine("{");
                    writes[0].WriteLine("    public enum enLanguageKey");
                    writes[0].WriteLine("    {");
                    for (int i = 1; i < heads.Length; i++)
                    {
                        if (!Directory.Exists(DataTableRule.LocalizationPath)) {
                            Directory.CreateDirectory(DataTableRule.LocalizationPath);
                        }
                        string path = PathUtility.GetCombinePath(DataTableRule.LocalizationPath,
                            $"{heads[i]}.csv");
                        
                        File.WriteAllText(path, "");
                        writes[i] = new StreamWriter(path, true, Encoding.UTF8);
                    }
                    while (true)
                    {
                        string line = streamReader.ReadLine();
                        if(string.IsNullOrEmpty(line)) break;
                        if(line.StartsWith("#")) continue;
                        string[] colums = SplitLine(line);
                        for (int i = 0; i < colums.Length; i++)
                        {
                            if (i == 0)
                            {
                                writes[0].WriteLine("        " + colums[0] + ",");
                            }
                            else
                            {
                                writes[i].WriteLine(colums[i]);
                            }
                        }
                    }

                    writes[0].WriteLine("    }");
                    writes[0].WriteLine("}");
                    
                    foreach (StreamWriter streamWriter in writes)
                    {
                        streamWriter.Flush();
                        streamWriter.Close();
                        streamWriter.Dispose();
                    }
                }
            }
        }

        public static void GenCsDateTableCode()
        {
            string[] files = Directory.GetFiles(DataTableRule.DataTablePath, "*.csv", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                string fileNameExt = Path.GetFileName(file);
                if (DataTableRule.IgnoreDataTable.Contains(fileNameNoExt) || fileNameNoExt == DataTableRule.LocalizationFile)
                {
                    continue;
                }
                FileInfo fileInfo = new FileInfo(file);
                if (fileNameExt != null)
                {
                    string subPath = file.Replace(DataTableRule.DataTablePath, "").Replace(".csv",".cs");
                    string destName = PathUtility.GetCombinePath(DataTableRule.ExportCsPath, subPath);
                    if (DataTableRule.IsCs)
                    {
                        WriteCsCode(fileInfo, destName, Encoding.UTF8);
                    }
                }
            }
        }
        
        public static void GenByteDateTableCode()
        {
            string[] files = Directory.GetFiles(DataTableRule.DataTablePath, "*.csv", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                string fileNameExt = Path.GetFileName(file);
                if (DataTableRule.IgnoreDataTable.Contains(fileNameNoExt) || fileNameNoExt == DataTableRule.LocalizationFile)
                {
                    continue;
                }
                FileInfo fileInfo = new FileInfo(file);
                if (fileNameExt != null)
                {
                    string subPath = file.Replace(DataTableRule.DataTablePath, "").Replace(".csv",".bytes");
                    string destName = PathUtility.GetCombinePath(DataTableRule.BytePath, subPath);
                    if (DataTableRule.IsByte)
                    {
                        WriteBytes(fileInfo, destName, Encoding.UTF8);
                    }
                }
            }
        }
        
        public static void GenLuaDateTableCode()
        {
            string[] files = Directory.GetFiles(DataTableRule.DataTablePath, "*.csv", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                string fileNameExt = Path.GetFileName(file);
                if (DataTableRule.IgnoreDataTable.Contains(fileNameNoExt) || fileNameNoExt == DataTableRule.LocalizationFile)
                {
                    continue;
                }
                FileInfo fileInfo = new FileInfo(file);
                if (fileNameExt != null)
                {
                    string subPath = file.Replace(DataTableRule.DataTablePath, "").Replace(".csv",".lua");
                    string destName = PathUtility.GetCombinePath(DataTableRule.LuaPath, subPath);
                    if (DataTableRule.IsLua)
                    {
//                        WriteBytes(fileInfo, destName, Encoding.UTF8);
                    }
                }
            }
        }
        
        private static void WriteBytes(FileInfo fileInfo, string destName, Encoding encoding)
        {
            using (StreamReader streamReader = new StreamReader(fileInfo.FullName, encoding))
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
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(comment) || string.IsNullOrEmpty(type))
                {
                    return;
                }
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
                        if (string.IsNullOrEmpty(line)) break;
                        if (line.StartsWith("#")) continue;
                        string[] colums = RemoveUnuse(SplitLine(line.TrimEnd(eof)), useIndex);

                        for (int i = 0; i < colums.Length; i++)
                        {
                            AddParseValue(buffer, types[i], colums[i]);
                        }
                        lineCount++;
                    }
                    buffer.Seek(0, SeekOrigin.Begin);
                    ValueParse.WriteValue(buffer, lineCount, ValueParse.IntParse);

                    if (FileUtility.IsFileExist(destName))
                    {
                        FileUtility.DeleteFile(destName);
                    }
                    FileUtility.WriteBytesToFile(destName, buffer.ToBytes());
                }
            }
        }

        private static void WriteCsCode(FileInfo fileInfo, string destName, Encoding encoding)
        {
            using (StreamReader streamReader = new StreamReader(fileInfo.FullName, encoding))
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
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(comment) || string.IsNullOrEmpty(type))
                {
                    return;
                }
                File.WriteAllText(destName,"");
                using (StreamWriter cswrite = new StreamWriter(destName,true,encoding))
                {
                    string[] titles = RemoveUnuse(SplitLine(title.TrimEnd(eof)), useIndex);
                    string[] comments = RemoveUnuse(SplitLine(comment.TrimEnd(eof)), useIndex);
                    string[] types = RemoveUnuse(SplitLine(type.TrimEnd(eof)), useIndex);
                    string csFileName = Path.GetFileNameWithoutExtension(destName);
                    cswrite.WriteLine("using UnityEngine;");
                    cswrite.WriteLine("using System;");
                    cswrite.WriteLine("using System.Collections;");
                    cswrite.WriteLine("using System.Collections.Generic;");
                    cswrite.WriteLine("using GameFramework.DataTable.Base;");
                    cswrite.WriteLine("using GameFramework.Utility;");
                    cswrite.WriteLine("namespace GameFramework.DataTable.Core");
                    cswrite.WriteLine("{");
                    cswrite.WriteLine($"    public class {csFileName} : IDataRow");
                    cswrite.WriteLine("    {");

                    for (int i = 0; i < title.Length; i++)
                    {
                        cswrite.WriteLine(GetDataTypeWithStr);
                    }
                    
                    cswrite.WriteLine("    }");
                    cswrite.WriteLine("}");
                }
               
                
                using (ByteBuffer buffer = new ByteBuffer())
                {
                    int lineCount = 0;
                    buffer.Seek(4, SeekOrigin.Begin);
                    while (true)
                    {
                        string line = streamReader.ReadLine();
                        if (string.IsNullOrEmpty(line)) break;
                        if (line.StartsWith("#")) continue;
                        string[] colums = RemoveUnuse(SplitLine(line.TrimEnd(eof)), useIndex);

                        for (int i = 0; i < colums.Length; i++)
                        {
                            AddParseValue(buffer, types[i], colums[i]);
                        }
                        lineCount++;
                    }
                    buffer.Seek(0, SeekOrigin.Begin);
                    ValueParse.WriteValue(buffer, lineCount, ValueParse.IntParse);

                    if (FileUtility.IsFileExist(destName))
                    {
                        FileUtility.DeleteFile(destName);
                    }
                    FileUtility.WriteBytesToFile(destName, buffer.ToBytes());
                }
            }
        }

        public string GetDataTypeWithStr(string title , string comment, string type)
        {
            string ret = String.Empty;
            if (type == "byte")
            {
                ret = $"        private byte {title} ; // {comment}";
            }
            else if (type == "sbyte")
            {
                ret = $"        private sbyte {title} ; // {comment}";
            }
            else if (type == "bool")
            {
                ret = $"        private bool {title} ; // {comment}";
            }
            else if (type == "char")
            {
                ret = $"        private char {title} ; // {comment}";
            }
            else if (type == "short")
            {
                ret = $"        private short {title} ; // {comment}";
            }
            else if (type == "ushort")
            {
                ret = $"        private ushort {title} ; // {comment}";
            }
            else if (type == "uint")
            {
                ret = $"        private uint {title} ; // {comment}";
            }
            else if (type == "int")
            {
                ret = $"        private int {title} ; // {comment}";
            }
            else if (type == "ulong")
            {
                ret = $"        private ulong {title} ; // {comment}";
            }
            else if (type == "long")
            {
                ret = $"        private long {title} ; // {comment}";
            }
            else if (type == "float")
            {
                ret = $"        private float {title} ; // {comment}";
            }
            else if (type == "double")
            {
                ret = $"        private double {title} ; // {comment}";
            }
            else if (type == "string")
            {
                ret = $"        private string {title} ; // {comment}";
            }
            else if (type == "vector2")
            {
                ret = $"        private Vector2 {title} ; // {comment}";
            }
            else if (type == "vector3")
            {
                ret = $"        private Vector3 {title} ; // {comment}";
            }
            else if (type == "vector4")
            {
                ret = $"        private Vector4 {title} ; // {comment}";
            }
            else if (type == "color32")
            {
                ret = $"        private Color32 {title} ; // {comment}";
            }
            else if (type == "color")
            {
                ret = $"        private Color {title} ; // {comment}";
            }
            else if (type == "datetime")
            {
                ret = $"        private DateTime {title} ; // {comment}";
            }
            else if (type == "rect")
            {
                ret = $"        private Rect {title} ; // {comment}";
            }
            else if (type == "quaternion")
            {
                ret = $"        private Quaternion {title} ; // {comment}";
            }
            else if (type.StartsWith("list"))
            {
                string listType = type.Split('|')[1];
                ret = $"        private List<{GetBaseType(listType)}> {title} = new List<{GetBaseType(listType)}>() ; // {comment}";
            }
            else if (type.StartsWith("dirc"))
            {
                string ktype = type.Split('|')[1];
                string vtype = type.Split('|')[2];
                ret =
                    $"        private Dictionary<{GetBaseType(ktype)},{GetBaseType(vtype)}> {title} = new Dictionary<{GetBaseType(ktype)},{GetBaseType(vtype)}>() ; // {comment}";
            }
            else
            {
                UnityEngine.Debug.LogError("error 不支持的数据类型   "+type);
            }
            return ret;
        }

        private string GetBaseType(string type)
        {
            string ret = String.Empty;
            if (type == "byte")
            {
                ret = "byte";
            }
            else if (type == "sbyte")
            {
                ret = "sbyte";
            }
            else if (type == "bool")
            {
                ret = "bool";
            }
            else if (type == "char")
            {
                ret = "char";
            }
            else if (type == "short")
            {
                ret = "short";
            }
            else if (type == "ushort")
            {
                ret = "ushort";
            }
            else if (type == "uint")
            {
                ret = "uint";
            }
            else if (type == "int")
            {
                ret = "int";
            }
            else if (type == "ulong")
            {
                ret = "ulong";
            }
            else if (type == "long")
            {
                ret = "long";
            }
            else if (type == "float")
            {
                ret = "float";
            }
            else if (type == "double")
            {
                ret = "double";
            }
            else if (type == "string")
            {
                ret = "string";
            }
            else if (type == "vector2")
            {
                ret = "Vector2";
            }
            else if (type == "vector3")
            {
                ret = "Vector3";
            }
            else if (type == "vector4")
            {
                ret = "Vector4";
            }
            else if (type == "color32")
            {
                ret = "Color32";
            }
            else if (type == "color")
            {
                ret = "Color";
            }
            else if (type == "datetime")
            {
                ret = "DateTime";
            }
            else if (type == "rect")
            {
                ret = "Rect";
            }
            else if (type == "quaternion")
            {
                ret = "Quaternion";
            }
            return ret;
        }
        
    }
}