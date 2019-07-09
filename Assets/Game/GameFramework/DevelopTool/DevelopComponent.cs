using UnityEngine;

namespace GameFramework.DevelopTool
{
    public partial class DevelopComponent : MonoBehaviour
    {
        public static DevelopComponent Instance = null;

        void Awake()
        {
            Instance = this;
            _fpsInfo.Init();
            Application.logMessageReceived += _logInfo.OnLogMessageReceived;
            SetShowType(ShowType.ShowNon);
        }

        private DevelopDebugFps _fpsInfo = new DevelopDebugFps();
        private DevelopDebugSystem _sysInfo = new DevelopDebugSystem();
        private DevelopDebugSelect _selectInfo = new DevelopDebugSelect();
        private DevelopDebugProfiler _profilerInfo = new DevelopDebugProfiler();
        private DevelopDebugLog _logInfo = new DevelopDebugLog();

        private static float _myScreenWidth = 960f;
        private static float _myScreenHeight = 640f;

        private static float _myScaleWidth = (Screen.width / _myScreenWidth);
        private static float _myScaleHeight = (Screen.height / _myScreenHeight);

        private static float _myScale = _myScaleWidth <= _myScaleHeight ? _myScaleWidth : _myScaleHeight;

        internal static readonly Rect _defaultIconRect = new Rect(10f, 10f, 60f, 60f);
        internal static readonly Rect _defaultWindowRect = new Rect(10f, 10f, 600f, 750f);

        private Rect _iconRect = _defaultIconRect;
        private Rect _windowRect = _defaultWindowRect;

        private Rect _dragRect = new Rect(0f, 0f, 0f, 25f);

        private ShowType _showType = ShowType.ShowFps;

        private enum ShowType : int
        {
            ShowNon = 0,
            ShowFps,
            ShowSelect,
            ShowLog,
            ShowSystem,
            ShowProfiler,
        }


        void Update()
        {
            _fpsInfo.Update();
            if (UnityEngine.Input.GetKeyDown(KeyCode.F4) && _showType == ShowType.ShowNon)
            {
                if (_showType == ShowType.ShowNon)
                {
                    SetShowType(ShowType.ShowFps);
                }
                else
                {
                    SetShowType(ShowType.ShowNon);
                }
            }
        }

        private void SetShowType(ShowType type)
        {
            _showType = type;
        }

        private void OnGUI()
        {
            if (_showType == ShowType.ShowNon) return;
            GUISkin cachedGuiSkin = GUI.skin;
            Matrix4x4 cachedMatrix = GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity,
                new Vector3(_myScale, _myScale, 1));
            switch (_showType)
            {
                case ShowType.ShowFps:
                    _iconRect = GUILayout.Window((int) _showType, _iconRect, DrawFps, "<b>FPS</b>");
                    break;
                case ShowType.ShowLog:
                    _windowRect = GUILayout.Window((int) _showType, _windowRect, _logInfo.DrawLog, "<b>日志信息</b>");
                    break;
                case ShowType.ShowSelect:
                    _windowRect = GUILayout.Window((int) _showType, _windowRect, _selectInfo.DrawSelect, "<b>调试选项</b>");
                    break;
                case ShowType.ShowSystem:
                    _windowRect = GUILayout.Window((int) _showType, _windowRect, _sysInfo.DrawSystem, "<b>系统信息</b>");
                    break;
                case ShowType.ShowProfiler:
                    _windowRect = GUILayout.Window((int) _showType, _windowRect, _profilerInfo.DrawProfiler,
                        "<b>内存信息</b>");
                    break;
                case ShowType.ShowNon:
                    break;
            }
            GUI.matrix = cachedMatrix;
            GUI.skin = cachedGuiSkin;
        }

        protected void DrawItem(string title, string content)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(title, GUILayout.Width(240f));
                GUILayout.Label(content);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawFps(int windowId)
        {
            GUI.DragWindow(_dragRect);
            Color32 color = Color.white;
            Color32 colorLog = Color.red;
            string title = string.Format("<color=#{0}{1}{2}{3}><b>FPS:{4}</b></color>", color.r.ToString("x2"),
                color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"), _fpsInfo.Fps.ToString("F2"));

            string error = string.Format("<color=#{0}{1}{2}{3}><b>Log:{4}</b></color>", colorLog.r.ToString("x2"),
                colorLog.g.ToString("x2"), colorLog.b.ToString("x2"), colorLog.a.ToString("x2"),
                _logInfo.GetCount() + "");
            if (GUILayout.Button(title + " " + error, GUILayout.Width(150f), GUILayout.Height(35f)))
            {
                SetShowType(ShowType.ShowSelect);
            }
        }

        #region DevelopDebugSelect

        public class DevelopDebugSelect
        {
            public void DrawSelect(int windowId)
            {
                GUI.DragWindow(Instance._dragRect);
                GUILayout.Label("<b>F4关闭调试!</b>");
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Instance.SetShowType(ShowType.ShowFps);
                }
                GUILayout.Space(10);
                if (GUILayout.Button("日志信息", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Instance.SetShowType(ShowType.ShowLog);
                }
                GUILayout.Space(5);
                if (GUILayout.Button("系统信息", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Instance.SetShowType(ShowType.ShowSystem);
                }
                GUILayout.Space(5);
                if (GUILayout.Button("内存信息", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Instance.SetShowType(ShowType.ShowProfiler);
                }
                GUILayout.Space(5);
                if (GUILayout.Button("显示Fps", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Instance.SetShowType(ShowType.ShowFps);
                }
                GUILayout.Space(5);
                if (GUILayout.Button("关闭调试", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Instance.SetShowType(ShowType.ShowNon);
                }
            }
        }

        #endregion
    }
}