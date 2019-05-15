using System.Collections.Generic;
using System.Security.Cryptography;
using GameFramework.Utility;
using GameFramework.Utility.File;
using UnityEngine;

public class Launch:MonoBehaviour
{
    public Camera Camera;
    private Texture2D _texture2D;
    private Texture2D _icon;
    void Start()
    {
//        Debuger.Init(AppConst.Path.DebugerLogFilePath);
//        Debuger.EnableLog = AppConst.GameConfig.DebugerEnableLog;
//        Debuger.EnableSave = AppConst.GameConfig.DebugerEnableSave;
//        Debuger.EnableColor = AppConst.GameConfig.DebugerEnableColor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
           
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
           
        }
        
    }

    void OnDestroy()
    {
    }
}
