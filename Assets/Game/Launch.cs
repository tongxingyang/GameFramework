using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using GameFramework.Base;
using GameFramework.DataNode;
using GameFramework.DataNode.Base;
using GameFramework.Download.Base;
using GameFramework.Utility;
using GameFramework.Utility.File;
using GameFramework.Utility.Singleton;
using UnityEngine;

public class Launch:MonoBehaviour
{
    public Camera Camera;
    private Texture2D _texture2D;
    private Texture2D _icon;
    private DownloadAgent downloadAgent;
    
    
    public AnimationCurve FadeAnimationCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
    
    void Start()
    {
//        Debuger.Init(AppConst.Path.DebugerLogFilePath);
//        Debuger.EnableLog = AppConst.GameConfig.DebugerEnableLog;
//        Debuger.EnableSave = AppConst.GameConfig.DebugerEnableSave;
//        Debuger.EnableColor = AppConst.GameConfig.DebugerEnableColor;
//        downloadAgent = gameObject.AddComponent<DownloadAgent>();
//        downloadAgent.Initialize();
        
//        Dictionary<int,int> value = new Dictionary<int, int>();
////        Console.WriteLine("--------------------");
//        using (FileStream fileStream = new FileStream("/Users/ww/Desktop/serverdir/tengxun/text.txt",FileMode.CreateNew,FileAccess.Write))  
//        {
//            using (StreamWriter streamWriter = new StreamWriter(fileStream))
//            {
//                for (int i = 0; i < 100000; i++)
//                {
//                    UnityEngine.Object obj = new UnityEngine.Object();
//                    int hashcode = obj.GetHashCode();
//                    streamWriter.WriteLine(i+"    "+hashcode);
//                    int index = 0;
//                    if (value.TryGetValue(hashcode, out index))
//                    {
//                        Debug.LogError("chongfu  " + i + "  " + hashcode);
//                    }
//                    else
//                    {
//                        value.Add(hashcode, i);
//                    }
//                }
//
//            }
//        }
//        Console.WriteLine("done");
//        Console.ReadKey();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
//            Debug.LogError(Application.persistentDataPath); // 22 - 33
//            downloadAgent.OnStart(new DownloadTask(Application.persistentDataPath + "/testdown.pdf",
//                "http://127.0.0.1/shuxue.pdf", (a, b) => { }, (a, b, c) => { }, (a, b) => { }, 100, 50000));
//            Debug.LogError("kaishi   "+DateTime.Now);
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Time.timeScale = 2;
        }
        
        Debug.Log(Time.time+"    "+Time.unscaledTime+"         "+Time.realtimeSinceStartup+"   "+Time.deltaTime+"         "+Time.unscaledDeltaTime+"       ");
        
       

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
