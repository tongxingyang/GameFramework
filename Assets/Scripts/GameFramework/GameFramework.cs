using GameFramework.Base;
using GameFramework.Debug;
using GameFramework.Sound;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework
{
    [DisallowMultipleComponent]
    public class GameFramework:SingletonMono<GameFramework>
    {
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
        
        [Header("音效设置")]
        [SerializeField]
        public SoundGroupInfo[] SoundGroupInfos;

        
        public bool EditorResourceMode
        {
            get { return editorResourceMode; }
            set { editorResourceMode = value; }
        }

        public int FrameRate
        {
            get { return frameRate; }
            set { Application.targetFrameRate = frameRate = value; }
        }

        public float GameSpeed
        {
            get { return gameSpeed; }
            set { Time.timeScale = gameSpeed = (value >= 0f ? value : 0f); }
        }

        public bool RunInBackgroiund
        {
            get { return runInBackgroiund; }
            set { runInBackgroiund = value; }
        }

        public bool NeverSleep
        {
            get { return neverSleep; }
            set { neverSleep = value; }
        }

        public bool IsGamePause => gameSpeed <= 0;

        #region Unity Function

        void Awake()
        {
            Application.lowMemory += OnLowMemory;
            InitDebuger();
            Application.backgroundLoadingPriority = ThreadPriority.High;
            Application.runInBackground = RunInBackgroiund;
            Application.targetFrameRate = FrameRate;
            QualitySettings.skinWeights = SkinWeights.TwoBones;
            Screen.sleepTimeout = NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            Time.timeScale = GameSpeed;
            Singleton<GameEntry>.GetInstance().InitComponent(this.gameObject.transform);
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
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        void OnDestroy()
        {
            
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
            if (GameSpeed == 1)
            {
                return;
            }
            GameSpeed = 1f;
        }
        
        #endregion

        
    }
}