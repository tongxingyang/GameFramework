﻿using GameFramework.Base;
using GameFramework.Debug;
using GameFramework.DevelopTool;
using GameFramework.Localization;
using GameFramework.Setting;
using GameFramework.Sound;
using GameFramework.Utility.Extension;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework
{
    [DisallowMultipleComponent]
    public class GameFramework:SingletonMono<GameFramework>
    {
        #region 基础属性
        
        [Header("基础属性")]
        private float gameSpeedBeforePause = 1f;
        [SerializeField]
        private bool editorResourceMode = false;
        
        [SerializeField]
        private int frameRate = 30;

        [SerializeField]
        private float gameSpeed = 1f;

        [SerializeField]
        private bool runInBackgroiund = true;

        [SerializeField]
        private bool neverSleep = true;

        [SerializeField] 
        private ThreadPriority threadPriority = ThreadPriority.High;

        [SerializeField] 
        private Language language = Language.None;
       
        [Header("Animation设置")] 
        public int AnimationPriority = 50;
        
        [Header("DataNode设置")] 
        public int DataNodePriority = 50;
        
        [Header("DataTable设置")] 
        public int DataTablePriority = 50;
        
        [Header("Download设置")] 
        public int DownloadCount = 2;
        public int DownloadPriority = 50;
        
        [Header("Event设置")] 
        public int EventPriority = 50; 
        
        [Header("Fsm设置")] 
        public int FsmPriority = 50;
        
        [Header("Res设置")] 
        public int ResPriority = 50;
        
        [Header("Setting设置")] 
        public int SettingPriority = 50;

        [Header("Input设置")] 
        public int InputPriority = 50;

        [Header("Sound设置")] 
        [SerializeField] public int SoundPriority = 50;
        public SoundGroupInfo[] SoundGroupInfos;

        [Header("Timer设置")] 
        public int TimerPriority = 50;
        
        [Header("WebRequest设置")] 
        public int WebRequestCount = 2;
        public int WebRequestPriority = 50;
        
        [Header("Localization设置")] 
        public int LocalizationPriority = 50;
        
        [Header("UI设置")] 
        public int UIPriority = 50;
        
        #endregion
        
        
         
        public bool EditorResourceMode
        {
            get => editorResourceMode;
            set => editorResourceMode = value;
        }

        public int FrameRate
        {
            get => frameRate;
            set => Application.targetFrameRate = frameRate = value;
        }

        public float GameSpeed
        {
            get => gameSpeed;
            set => Time.timeScale = gameSpeed = (value >= 0f ? value : 0f);
        }

        public bool RunInBackgroiund
        {
            get => runInBackgroiund;
            set => runInBackgroiund = value;
        }

        public bool NeverSleep
        {
            get => neverSleep;
            set => neverSleep = value;
        }

        public ThreadPriority ThreadPriority
        {
            get => threadPriority;
            set => threadPriority = value;
        }

        public Language Language
        {
            get => language;
            set
            {
                if (value == Language.None)
                {
                    return;
                }
                if (language != value)
                {
                    language = value;
                    Singleton<GameEntry>.GetInstance().GetComponent<SettingComponent>().SetLanguage(language);
                    Singleton<GameEntry>.GetInstance().GetComponent<LocalizationComponent>().Language = language;
                }
            }
        }
        
        public bool IsGamePause => gameSpeed <= 0;

        private GameObject gameSplash = null;

        public GameObject GameSplash
        {
            get
            {
                if (gameSplash == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("UI/PanelSplash");
                    gameSplash = Instantiate(prefab);
                    gameSplash.transform.SetParent(AppConst.GlobalCahce.PanelRoot);
                    RectTransform rectTransform = gameSplash.GetComponent<RectTransform>();
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    gameSplash.transform.localScale = Vector3.one;
                    gameSplash.SetActive(false);
                }
                return gameSplash;
            }
        }
        private GameObject gameMonition = null;

        public GameObject GameMonition
        {
            get
            {
                if (gameMonition == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("UI/PanelMonition");
                    gameMonition = Instantiate(prefab);
                    gameMonition.transform.SetParent(AppConst.GlobalCahce.PanelRoot);
                    RectTransform rectTransform = gameMonition.GetComponent<RectTransform>();
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    gameMonition.transform.localScale = Vector3.one;
                    gameMonition.SetActive(false);
                }
                return gameMonition;
            }
        }
        private GameObject gameLogo = null;

        public GameObject GameLogo
        {
            get
            {
                if (gameLogo == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("UI/PanelLogo");
                    gameLogo = Instantiate(prefab);
                    gameLogo.transform.SetParent(AppConst.GlobalCahce.PanelRoot);
                    RectTransform rectTransform = gameLogo.GetComponent<RectTransform>();
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    gameLogo.transform.localScale = Vector3.one;
                    gameLogo.SetActive(false);
                }
                return gameLogo;
            }
        }

        private void InitGlobalCahce()
        {
            GameObject go = GameObject.Find("UICamera");
            if (go != null)
            {
                AppConst.GlobalCahce.UICamera = go.GetComponent<Camera>();
            }
            go = GameObject.Find("UIRootCanvas");
            if (go != null)
            {
                AppConst.GlobalCahce.UIRootCanvas = go.transform;
                AppConst.GlobalCahce.PanelRoot = go.transform.Find("PanelRoot");
            }
        }

        private void InitLanguage()
        {
            Language system = Singleton<GameEntry>.GetInstance().GetComponent<LocalizationComponent>().SystemLanguage;
            Language save = Singleton<GameEntry>.GetInstance().GetComponent<SettingComponent>().GetLanguage();
            Language = save == Language.None ? system : save;
        }
        
        #region Unity Function

        void Awake()
        {
            if (AppConst.GameConfig.IsShowDevelopInfo)
            {
                this.gameObject.GetOrAddComponent<DevelopComponent>();
            }
            InitGlobalCahce();
            Application.lowMemory += OnLowMemory;
            InitDebuger();
            Application.backgroundLoadingPriority = ThreadPriority;
            Application.runInBackground = RunInBackgroiund;
            Application.targetFrameRate = FrameRate;
            QualitySettings.skinWeights = SkinWeights.TwoBones;
            Screen.sleepTimeout = NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            Time.timeScale = GameSpeed;
            Singleton<GameEntry>.GetInstance().InitComponent(this.gameObject.transform);
            InitLanguage();
            Singleton<GameEntry>.GetInstance().OnAwake();
        }

        void Start()
        {
            Singleton<GameEntry>.GetInstance().OnStart();
        }

        void Update()
        {
            Singleton<GameEntry>.GetInstance().OnUpdate(Time.deltaTime, Time.unscaledDeltaTime);
        }
        
        void LateUpdate()
        {
            Singleton<GameEntry>.GetInstance().OnLateUpdate();
        }

        void FixedUpdate()
        {
            Singleton<GameEntry>.GetInstance().OnFixedUpdate(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
        }

        public void Shutdown()
        {
            Application.lowMemory -= OnLowMemory;
            Singleton<GameEntry>.GetInstance().Shutdown();
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        #endregion

        #region Custom Function

        private void InitDebuger()
        {
//            Debuger.Init(AppConst.Path.DebugerLogFilePath);
//            Debuger.EnableLog = AppConst.GameConfig.DebugerEnableLog;
//            Debuger.EnableSave = AppConst.GameConfig.DebugerEnableSave;
//            Debuger.EnableColor = AppConst.GameConfig.DebugerEnableColor;
        }
        
        private void OnLowMemory()
        {
            Debuger.LogError("Low memory reported...");
        }

        public void PauseGame()
        {
            if (IsGamePause)
            {
                return;
            }
            gameSpeedBeforePause = GameSpeed;
            GameSpeed = 0f;
        }

        public void ResumeGame()
        {
            if (!IsGamePause)
            {
                return;
            }
            GameSpeed = gameSpeedBeforePause;
        }

        public void RestNormalGameSpeed()
        {
            GameSpeed = 1f;
        }
        
        #endregion

        
    }
}