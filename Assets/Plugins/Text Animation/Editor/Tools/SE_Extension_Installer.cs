using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace I2.TextAnimation
{
    public class SE_Extension_Installer : EditorWindow
    {
        #region Variables 

        public class PatchItem
        {
            public string FileName;
            public string ExistingLine;
            public string NewLine;
            public bool uiExpanded = false;
            public string Status;
        }
        public string DisclaimerMessage;
        public string ExtensionName = "Unknown";
        public List<PatchItem> PatchData = new List<PatchItem>();
        public string ScriptingDefineSymbols;

        public string RequiredFile, RequiredLine;

        Vector2 scrollPos = Vector2.zero;
        string PatchResult;
        bool VersionSupported;
        #endregion

        #region Editor

        public void OnEnable()
        {
            PatchResult = string.Empty;
            LoadPatchData();
            EvaluatePatchStatus();
            EvaluateRequiredVersion();
        }

        public void OnGUI()
        {
            if (!VersionSupported && !string.IsNullOrEmpty(DisclaimerMessage))
            {
                GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.red;
                    GUILayout.Label("NGUI Version not supported", EditorStyles.boldLabel, GUITools.DontExpandWidth);
                    GUI.color = Color.white;
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(DisclaimerMessage, GUITools.DontExpandWidth);
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (var patch in PatchData)
            {
                GUILayout.BeginVertical("Box");
                GUILayout.BeginHorizontal();
                patch.uiExpanded = GUILayout.Toggle(patch.uiExpanded, patch.FileName, EditorStyles.foldout);
                GUILayout.FlexibleSpace();
                OnGUI_PatchState(patch.Status);
                GUILayout.EndHorizontal();

                if (patch.uiExpanded)
                {
                    GUILayout.Label("Previous Line:");
                    GUILayout.Label(patch.ExistingLine, EditorStyles.textArea, GUILayout.ExpandHeight(false));
                    GUILayout.Label("New Line:");
                    GUILayout.Label(patch.NewLine, EditorStyles.textArea, GUILayout.ExpandHeight(false));
                }
                GUILayout.EndVertical();
            }

            GUILayout.BeginHorizontal();
                GUILayout.Label("Scripting Define Symbols", GUITools.DontExpandWidth);
                GUILayout.Label(ScriptingDefineSymbols, EditorStyles.textArea, GUILayout.ExpandHeight(false));
                var settings = string.Join(" ", EditorUserBuildSettings.activeScriptCompilationDefines);
                OnGUI_PatchState(settings.Contains(ScriptingDefineSymbols) ? "Applied" : "Can Be Patched");
            GUILayout.EndHorizontal();


            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Apply Patch"))
                ApplyPatch();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Remove Patch"))
                RemovePatch();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(PatchResult))
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(PatchResult);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        void OnGUI_PatchState(string Status)
        {
            GUILayout.Label(Status, EditorStyles.miniLabel, GUITools.DontExpandWidth);
            if (Status == "Applied")
            {
                GUI.color = Color.green;
                GUILayout.Label("", GUILayout.Width(17));
                Rect r = GUILayoutUtility.GetLastRect(); r.xMin += 3; r.yMin -= 3; r.xMax += 2; r.yMax += 2;
                GUI.Label(r, new GUIContent("\u2713", "Applied"), EditorStyles.whiteLargeLabel);
                GUI.color = Color.white;
            }
            else
            if (Status == "File Not Found")
            {
                GUI.color = Color.red;
                GUILayout.Label("", GUILayout.Width(17));
                Rect r = GUILayoutUtility.GetLastRect(); r.xMin += 3; r.yMin -= 3; r.xMax += 2; r.yMax += 2;
                GUI.Label(r, new GUIContent("\u2717", "File not found"), EditorStyles.whiteLargeLabel);
                GUI.color = Color.white;
            }
            else
            if (Status == "Line Not Found")
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                float Width = 15;
                rect.xMin = rect.xMax + 1;
                rect.xMax = rect.xMin + rect.height;
                GUI.DrawTexture(rect, GUI.skin.GetStyle("CN EntryWarn").normal.background);
                GUI.Label(rect, new GUIContent("", "'Previous Line' not found"));
                GUILayout.Space(Width);
            }
            else
            if (Status == "Can Be Patched")
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                float Width = 15;
                rect.xMin = rect.xMax + 1;
                rect.xMax = rect.xMin + rect.height;
                GUI.DrawTexture(rect, GUI.skin.GetStyle("CN EntryInfo").normal.background);
                GUI.Label(rect, new GUIContent("", "File can be patched"));
                GUILayout.Space(Width);
            }
        }

        #endregion

        #region Util

        void EvaluateRequiredVersion()
        {
            VersionSupported = true;

            if (!string.IsNullOrEmpty(RequiredFile))
            {
                try
                {
                    string FileName = GetFileName(RequiredFile);
                    if (string.IsNullOrEmpty(FileName))
                        throw new System.Exception();

                    string FileText = File.ReadAllText(FileName);
                    if (string.IsNullOrEmpty(FileText))
                        throw new System.Exception();

                    if (!string.IsNullOrEmpty(RequiredLine) && !FileText.Contains(RequiredLine))
                        throw new System.Exception();
                }
                catch (System.Exception)
                {
                    VersionSupported = false;
                }
            }
        }

        void EvaluatePatchStatus()
        {
            bool AllPatched = true;
            foreach (var patch in PatchData)
            {
                patch.Status = IsPatchApplied(patch);
                AllPatched &= (patch.Status == "Applied");
            }

            if (AllPatched)
                PatchResult = ExtensionName + " has been Patched to support TextAnimation";
            else
                PatchResult = string.Empty;
        }

        public virtual void LoadPatchData()
        {
        }

        public void ApplyPatch()
        {
            bool Success = true;
            foreach (var patch in PatchData)
                Success &= ModifyFile(patch.FileName, patch.ExistingLine, patch.NewLine, true);
            ApplyScriptingDefineSymbols(ScriptingDefineSymbols, null);

            if (Success)
            {
                EvaluatePatchStatus();
                Debug.Log(ExtensionName + " has been Patched to support TextAnimation");
                AssetDatabase.Refresh();
            }
            else
            {
                EvaluatePatchStatus();
                PatchResult = "Patch Failed";
                Debug.LogError("Patch Failed");
                AssetDatabase.Refresh();
            }
        }

        public void ApplyScriptingDefineSymbols(string AddSymbol, string RemoveSymbol)
        {
            foreach (BuildTargetGroup target in System.Enum.GetValues(typeof(BuildTargetGroup)))
                if (target != BuildTargetGroup.Unknown && !target.HasAttributeOfType<System.ObsoleteAttribute>())
                {
                    ApplyScriptingDefineSymbols(target, AddSymbol, RemoveSymbol);
                }

            // Force these one (in a recent Unity version, iPhone has the same Enum Number than iOS and iPhone is deprecated, so iOS was been skipped)
            ApplyScriptingDefineSymbols(BuildTargetGroup.iOS, AddSymbol, RemoveSymbol);
        }

        public void ApplyScriptingDefineSymbols(BuildTargetGroup target, string AddSymbol, string RemoveSymbol)
        { 
            string Settings = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            List<string> symbols = new List<string>(Settings.Split(';'));
            symbols.Remove(RemoveSymbol);
            if (!string.IsNullOrEmpty(AddSymbol) && !symbols.Contains(AddSymbol))
                symbols.Add(AddSymbol);
            symbols.Remove(string.Empty);

            var newSettings = string.Join(";", symbols.ToArray());
            if (newSettings != Settings)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, newSettings);
        }


        public void RemovePatch()
		{
			bool Success = true;
			foreach (var patch in PatchData)
				Success &= ModifyFile (patch.FileName, patch.NewLine, patch.ExistingLine, false);
            ApplyScriptingDefineSymbols(null, ScriptingDefineSymbols);

            if (Success)
			{
				EvaluatePatchStatus();
				Debug.LogFormat ("<color=#FFEB04>{0} Patch has been removed</color>", ExtensionName);
				AssetDatabase.Refresh();
			}
			else
			{
				EvaluatePatchStatus();
				//PatchResult = "Patch Removed";
				Debug.LogError ("Failed to remove Patch");
				AssetDatabase.Refresh();
			}
		}

		string IsPatchApplied( PatchItem patch )
		{
			string FileName = GetFileName(patch.FileName);
			if (string.IsNullOrEmpty(FileName))
				return "File Not Found";

			string FileText = File.ReadAllText(FileName);
            FileText = FileText.Replace("\r\n", "\n");
			
			if (FileText.Contains(patch.NewLine))//StringContains(FileText, patch.NewLine))
                return "Applied";

            if (FileText.Contains(patch.ExistingLine))//StringContains(FileText, patch.ExistingLine))
                return "Can Be Patched";

            return "Line Not Found";
		}

        bool StringContains( string text, string subText )
        {
            if (text.Contains(subText))
                return true;

            if (subText.Contains("\n") && text.Contains(subText.Replace("\n", "\r\n")))
                return true;
            return false;
        }

        string ReplaceString(string text, string subText, string newText)
        {
            text = text.Replace(subText, newText);

            if (subText.Contains("\n"))
                text = text.Replace(subText.Replace("\n", "\r\n"), newText);

            return text;
        }


        string GetFileName( string patchFileName )
		{
			string[] paths = AssetDatabase.FindAssets(patchFileName);
			for (int i=0; i<paths.Length; ++i)
			{
				string s = AssetDatabase.GUIDToAssetPath(paths[i]);
				if (s.EndsWith("/"+patchFileName+".cs") || s.EndsWith("/" + patchFileName + ".shader"))
				{
					return Application.dataPath.Remove (Application.dataPath.Length-"Assets".Length) + s;
				}
			}
			return null;
		}

		bool ModifyFile(string FileName, string text, string newText, bool showErrors)
		{
			var FullFileName = GetFileName(FileName);
			if (string.IsNullOrEmpty(FullFileName))
			{
				if (showErrors)
					Debug.LogError("Patch couldn't find file: <i>" + FileName + "</i>");
				return false;
			}

			string FileText=null;

			try{
				FileText = File.ReadAllText(FullFileName);
			}
			catch(System.Exception){}

			if (string.IsNullOrEmpty(FileText))
			{
				Debug.LogError("Unable to open file " + FullFileName);
				return false;
			}

            FileText = FileText.Replace("\r\n", "\n");

            if (!FileText.Contains(text))//!StringContains(FileText, text))
			{
				if (FileText.Contains(newText))//StringContains(FileText, newText))
					return true;

				Debug.LogError("File " + FileName + " is incompatible.\nUnable to find line:\n\n<i>" + text + "</i>\n\n to replace it with:\n\n<i>" + newText + "</i>\n\nCheck the support forum for a solution: " + "<color=#CCCCFF>http://inter-illusion.com/forum</color>");
				return false;
			}
			else
			{
                //FileText = ReplaceString(FileText, text, newText);
                FileText = FileText.Replace(text, newText);

                File.WriteAllText (FullFileName, FileText);
				return true;
			}
		}

		#endregion
	}
}