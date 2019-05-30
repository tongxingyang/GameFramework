using System;
using System.Collections.Generic;
using GameFramework.Debug;
using GameFramework.Utility.File;
using UnityEngine;
using ZXing.Common;

namespace GameFramework.Utility.QRcode
{
    using ZXing;
    public class QRcodeUtility
    {
        public static string DecodeQRcodeImage(Texture2D texture2D)
        {
            try
            {
                Color32LuminanceSource color32LuminanceSource = new Color32LuminanceSource(texture2D.GetPixels32(),texture2D.width,texture2D.height);
                Binarizer binarizer = new GlobalHistogramBinarizer(color32LuminanceSource);
                BinaryBitmap binaryBitmap = new BinaryBitmap(binarizer);
                MultiFormatReader multiFormatReader = new MultiFormatReader();
                Result result = multiFormatReader.decode(binaryBitmap);
                if (result != null)
                {
                    return result.Text;
                }
                else
                {
                    Debuger.LogError("Decode QRcode Image Error");
                    return String.Empty;
                }
            }
            catch (Exception e)
            {
                Debuger.LogError("Decode QRcode Image Error "+e.Message);
                return String.Empty;
            }
        }
        
        public static Texture2D EncodeQRcodeImage(string content, int width, int height,TextureFormat textureFormat = TextureFormat.ARGB32,bool isSave = false, string savePath = "")
        {
            Texture2D texture2D = null;
            BitMatrix bitMatrix = null;
            try
            {
                MultiFormatWriter multiFormatWriter = new MultiFormatWriter();
                Dictionary<EncodeHintType,object> hits = new Dictionary<EncodeHintType, object>();
                hits.Add(EncodeHintType.CHARACTER_SET,"UTF-8");
                hits.Add(EncodeHintType.MARGIN,1);
                hits.Add(EncodeHintType.ERROR_CORRECTION,ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
                bitMatrix = multiFormatWriter.encode(content, BarcodeFormat.QR_CODE, width, height, hits);
            }
            catch (Exception e)
            {
                Debuger.LogError("Endode QRcode Error content:  "+content+"  "+e.Message);
                return null;
            }
            texture2D = new Texture2D(width,height,textureFormat,false);
            for (int i = 0; i < bitMatrix.Height; i++)
            {
                for (int j = 0; j < bitMatrix.Width; j++)
                {
                    if (bitMatrix[i, j])
                    {
                        texture2D.SetPixel(i, j, Color.black);
                    }
                    else
                    {
                        texture2D.SetPixel(i,j,Color.white);
                    }
                }
            }
            texture2D.Apply();
            
            if (isSave)
            {
                SaveQRcodeImage(texture2D, savePath);
            }
            return texture2D;
        }

        public static Texture2D EncodeQRcodeImageWithIcon(string content, int width, int height, Texture2D icon,TextureFormat textureFormat = TextureFormat.ARGB32,bool isSave = false, string savePath = "")
        {
            Texture2D texture2D = null;
            BitMatrix bitMatrix = null;
            try
            {
                MultiFormatWriter multiFormatWriter = new MultiFormatWriter();
                Dictionary<EncodeHintType,object> hits = new Dictionary<EncodeHintType, object>();
                hits.Add(EncodeHintType.CHARACTER_SET,"UTF-8");
                hits.Add(EncodeHintType.MARGIN,1);
                hits.Add(EncodeHintType.ERROR_CORRECTION,ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
                bitMatrix = multiFormatWriter.encode(content, BarcodeFormat.QR_CODE, width, height, hits);
            }
            catch (Exception e)
            {
                Debuger.LogError("Encode QRCode Image With Icon Error "+e.Message);
                return null;
            }
            
            texture2D = new Texture2D(width,height,textureFormat,false);
            
            int halfWidth = texture2D.width / 2;
            int halfHeight = texture2D.height / 2;
            int halfWidthOfIcon = icon.width / 2;
            int halfHeightOfIcon = icon.height / 2;
            for (int i = 0; i < bitMatrix.Height; i++)
            {
                for (int j = 0; j < bitMatrix.Width; j++)
                {
                    
                    var centerOffsetX = i - halfWidth;
                    var centerOffsetY = j - halfHeight;
                    if (Mathf.Abs(centerOffsetX) <= halfWidthOfIcon && Mathf.Abs(centerOffsetY) <= halfHeightOfIcon)
                    {
                        texture2D.SetPixel(i, j,
                            icon.GetPixel(centerOffsetX + halfWidthOfIcon, centerOffsetY + halfHeightOfIcon));
                    }
                    else
                    {
                        if (bitMatrix[i, j])
                        {
                            texture2D.SetPixel(i, j, Color.black);
                        }
                        else
                        {
                            texture2D.SetPixel(i,j,Color.white);
                        }
                    }
                  
                }
            }
            
            texture2D.Apply();
            if (isSave)
            {
                SaveQRcodeImage(texture2D,savePath);
            }
            return texture2D;
        }

        
        public static Texture2D QRcodeImageAddBackGroundImage(Texture2D Bg,Texture2D Qr,bool isSave = false,string savePath = "")
        {
            Texture2D texture2D = UnityEngine.Object.Instantiate(Bg) as Texture2D;
            Vector2 uv = new Vector2((Bg.width-Qr.width)/Bg.width,(Bg.height-Qr.height)/Bg.height);
            for (int i = 0; i < Qr.width; i++)
            {
                for (int j = 0; j < Qr.height; j++)
                {
                    float w = uv.x * Bg.width - Qr.width + i;
                    float h = uv.y * Bg.height - Qr.height + j;
                    Color codeColor = Qr.GetPixel(i, j);
                    texture2D.SetPixel((int)w, (int)h, codeColor);
                }
            }
            texture2D.Apply();
            if (isSave)
            {
                SaveQRcodeImage(texture2D,savePath);
            }
            return texture2D;
        }


        public static void SaveQRcodeImage(Texture2D texture2D,string savePath)
        {
            if (FileUtility.IsDirectoryExist(savePath))
            {
                FileUtility.CreateDirectory(savePath);
            }
            if (FileUtility.IsFileExist(savePath))
            {
                FileUtility.DeleteFile(savePath);
            }
            byte[] bytes = texture2D.EncodeToPNG();
            FileUtility.WriteBytesToFile(savePath,bytes,bytes.Length);
        }
        
    }
}