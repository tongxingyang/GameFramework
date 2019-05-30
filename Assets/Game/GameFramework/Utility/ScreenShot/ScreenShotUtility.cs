using System;
using System.Collections;
using GameFramework.Utility.File;
using UnityEngine;

namespace GameFramework.Utility.ScreenShot
{
    public class ScreenShotUtility
    {
        
        public static IEnumerator CaptrueScreenShot(string savePath,Rect rect,Vector2 dest,TextureFormat format,Action<Texture2D> callBack)
        {
            yield return new WaitForEndOfFrame();
            Texture2D texture2D = new Texture2D((int)rect.width,(int)rect.height,format,false);
            texture2D.ReadPixels(rect,(int)dest.x,(int)dest.y);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            if (FileUtility.IsDirectoryExist(savePath))
            {
                FileUtility.CreateDirectory(savePath);
            }
            if (FileUtility.IsFileExist(savePath))
            {
                FileUtility.DeleteFile(savePath);
            }
            FileUtility.WriteBytesToFile(savePath,bytes,bytes.Length);
            callBack?.Invoke(texture2D);
        }

        public static IEnumerator CaptureScreenShotWithOneCamera(string savePath,Camera camera,Rect rect,Vector2 dest,TextureFormat format,Action<Texture2D> callBack)
        {
            yield return new WaitForEndOfFrame();
            RenderTexture renderTexture = new RenderTexture((int)rect.width,(int)rect.height,0);
            RenderTexture lastRenderTexture = camera.targetTexture;
            RenderTexture lastActiveRenderTexture = RenderTexture.active;
            camera.targetTexture = renderTexture;
            camera.Render();
            RenderTexture.active = renderTexture;
            Texture2D texture2D = new Texture2D((int)rect.width,(int)rect.height,format,false);
            texture2D.ReadPixels(rect,(int)dest.x,(int)dest.y);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            camera.targetTexture = lastRenderTexture;
            RenderTexture.active = lastActiveRenderTexture;
            UnityEngine.Object.Destroy(renderTexture);
            if (FileUtility.IsDirectoryExist(savePath))
            {
                FileUtility.CreateDirectory(savePath);
            }
            if (FileUtility.IsFileExist(savePath))
            {
                FileUtility.DeleteFile(savePath);
            }
            FileUtility.WriteBytesToFile(savePath,bytes,bytes.Length);
            callBack?.Invoke(texture2D);
        }

        public static IEnumerator CaptureScreenShotWithMultiCamera(string savePath, Camera[] cameras, Rect rect,
            Vector2 dest, TextureFormat format ,Action<Texture2D> callBack)
        {
            yield return new WaitForEndOfFrame();
            RenderTexture renderTexture = new RenderTexture((int)rect.width,(int)rect.height,0);
            Texture2D texture2D = new Texture2D((int)rect.width,(int)rect.height,format,false);

            for (int i = 0; i < cameras.Length; i++)
            {
                RenderTexture tempTargeTexture = cameras[i].targetTexture;
                cameras[i].targetTexture = renderTexture;
                cameras[i].Render();
                cameras[i].targetTexture = tempTargeTexture;
            }
            RenderTexture lastActiveRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(rect,(int)dest.x,(int)dest.y);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            RenderTexture.active = lastActiveRenderTexture;
            UnityEngine.Object.Destroy(renderTexture);
            if (FileUtility.IsDirectoryExist(savePath))
            {
                FileUtility.CreateDirectory(savePath);
            }
            if (FileUtility.IsFileExist(savePath))
            {
                FileUtility.DeleteFile(savePath);
            }
            FileUtility.WriteBytesToFile(savePath,bytes,bytes.Length);
            callBack?.Invoke(texture2D);
        }
        
    }
}