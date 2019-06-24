using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Core.LocalizationEditor
{
    public class LocalizationEditor:UnityEditor.Editor
    {
        public static string LocalizationCsvPath = Application.dataPath+"/local_skill.csv";
        public static string LocalizationResPath = Application.dataPath+"/Resources/Localization/";
        public static string LocalizationCodePath = Application.dataPath+"/Game/GameFramework/Localization/enLanguageKey.cs";
        public static char[] eof = { '\r', '\n' };
        [MenuItem("Editor/GenLocalization")]
        public static void GenLocalizationCodeAnRes()
        {
            FileInfo fileInfo = new FileInfo(LocalizationCsvPath);
            using (StreamReader streamReader = new StreamReader(fileInfo.FullName, Encoding.UTF8))
            {
                var readLine = streamReader.ReadLine();
                if (readLine != null)
                {
                    var heads = readLine.Split(',');
                    StreamWriter[] writes = new StreamWriter[heads.Length];
                    File.WriteAllText(LocalizationCodePath,"");
                    writes[0] = new StreamWriter(LocalizationCodePath,true,Encoding.UTF8);
//                    writes[0].WriteLine("using UnityEngine;");
//                    writes[0].WriteLine("using System;");
//                    writes[0].WriteLine("using System.Collections;");
//                    writes[0].WriteLine("using System.Collections.Generic;");
                    writes[0].WriteLine("namespace GameFramework.Localization");
                    writes[0].WriteLine("{");
                    writes[0].WriteLine("    public enum enLanguageKey");
                    writes[0].WriteLine("    {");
                    for (int i = 1; i < heads.Length; i++)
                    {
                        if (!Directory.Exists(LocalizationResPath)) {
                            Directory.CreateDirectory(LocalizationResPath);
                        }
                        string path = string.Format(
                            LocalizationResPath+"{0}.csv",
                            heads[i]
                        );
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
        private static string[] SplitLine(string line)
        {
            line = line.TrimEnd(eof);
            return SplitCsvLine(line);
        }

        private static string CsvLineSeperator = @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)";
        public static string[] SplitCsvLine(string line) {
            var value = (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
                    CsvLineSeperator, System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value);
            return value.ToArray();
        }
    }
}