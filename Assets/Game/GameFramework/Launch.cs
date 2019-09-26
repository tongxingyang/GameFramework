using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using GameFramework;
using GameFramework.Base;
using GameFramework.DataNode;
using GameFramework.DataNode.Base;
using GameFramework.Debug;
using GameFramework.Download.Base;
using GameFramework.UI.UITools;
using GameFramework.Utility;
using GameFramework.Utility.File;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.UI;

public class Launch:MonoBehaviour
{
    public Camera Camera;
    private Texture2D _texture2D;
    private Texture2D _icon;
    public CooldownImage CooldownImage;
    public Text LabText;
    public AnimationCurve FadeAnimationCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));

    public RectTransform YY1;
    public RectTransform YY2;
    public RectTransform YY3;
    public GuideImage GuideImage;
    
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
    void Start()
    {
        InitGlobalCahce();
//        Debuger.Init(AppConst.Path.DebugerLogFilePath);
//        Debuger.EnableLog = AppConst.GameConfig.DebugerEnableLog;
//        Debuger.EnableSave = AppConst.GameConfig.DebugerEnableSave;
//        Debuger.EnableColor = AppConst.GameConfig.DebugerEnableColor;

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GuideImage.ShowCricleMask(new Color(0,0,0,0.8f), YY1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GuideImage.ShowCricleMask(new Color(0,0,0,0.8f), YY2);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GuideImage.ShowCricleMask(new Color(0,0,0,0.8f), YY3);
        }
        return;
        if (Input.GetKeyDown(KeyCode.A))
        {
            CooldownImage.SetParam(10,10,LabText, () =>
            {
                Debug.LogError("完成");
            },false);
//            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/test.asset");
//            
////            Debug.LogError(Application.persistentDataPath); // 22 - 33
////            downloadAgent.OnStart(new DownloadTask(Application.persistentDataPath + "/testdown.pdf",
////                "http://127.0.0.1/shuxue.pdf", (a, b) => { }, (a, b, c) => { }, (a, b) => { }, 100, 50000));
////            Debug.LogError("kaishi   "+DateTime.Now);
//            Time.timeScale = 0;
//            Debug.LogError("-------------11");
//            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/test.asset");
//
//             AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/test.asset");
            
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
//                        {
//                string old = AssetBundle.LoadFromFile(Application.persistentDataPath+"/test.asset").LoadAsset<TextAsset>("version 1.json").text;
//                UnityEngine.Debug.LogError("old   "+old);
//            }
////            {
////                string news = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/xx/test.asset").LoadAsset<TextAsset>("version 1.json").text;
////                UnityEngine.Debug.LogError("new   "+news);
////            }
//            //
//            return;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
//            Time.timeScale = 2;
        }
        
//        Debug.Log(Time.time+"    "+Time.unscaledTime+"         "+Time.realtimeSinceStartup+"   "+Time.deltaTime+"         "+Time.unscaledDeltaTime+"       ");
        
       

//        if (Input.GetKeyDown(KeyCode.T))
//        {
//
////            string str = "a/b/c/d";
////            string[] strs = str.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries);
////            foreach (string s in strs)
////            {
////                Debug.LogError(s);
////            }
//
//            IDataNode node = Singleton<GameEntry>.GetInstance().DataNodeComponent.GetOrAddNode("a/b/c/d");
//            node.SetData(100);
//            Debug.LogError(node.FullName);
//            Debug.LogError(node.Name);
//            Debug.LogError(Singleton<GameEntry>.GetInstance().DataNodeComponent.GetNode("b/c/d",Singleton<GameEntry>.GetInstance().DataNodeComponent.GetNode("a")).GetData<int>());
//        }
        
    }

    void OnDestroy()
    {
    }
}
