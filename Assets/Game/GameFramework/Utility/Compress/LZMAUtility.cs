using System;
using System.IO;
using GameFramework.Debug;
using GameFramework.Utility.File;
using SevenZip.Compression.LZMA;

namespace GameFramework.Utility.Compress
{
    public class LZMAUtility
    {
        static Decoder decoder = new Decoder();
        static Encoder encoder = new Encoder();
        
        static bool Compress(Stream inputFs, Stream outputFs)
        {
            try
            {
                encoder.WriteCoderProperties(outputFs);
                byte[] data = BitConverter.GetBytes(inputFs.Length);
                outputFs.Write(data, 0, data.Length);
                encoder.Code(inputFs, outputFs, inputFs.Length, -1, null);
                outputFs.Flush();
                outputFs.Close();
                inputFs.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(ex.Message);
            }

            return false;
        }
        
        public static bool CompressFileLzma(string inFile, string outFile)
        {
            try
            {
                if (!FileUtility.IsFileExist(inFile))
                    return false;
                FileStream input = new FileStream(inFile, FileMode.Open);
                FileStream output = new FileStream(outFile, FileMode.OpenOrCreate);

                return Compress(input, output);
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(ex.Message);
            }

            return false;
        }

        static bool DeCompress(Stream inputFs, Stream outputFs)
        {
            try
            {
                byte[] properties = new byte[5];
                inputFs.Read(properties, 0, 5);

                byte[] fileLengthBytes = new byte[8];
                inputFs.Read(fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);
                decoder.SetDecoderProperties(properties);
                decoder.Code(inputFs, outputFs, inputFs.Length, fileLength, null);
                outputFs.Flush();
                outputFs.Close();
                inputFs.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(ex.Message);
            }
            return false;
        }
        
        public static bool DecompressFileLzma(string inFile, string outFile)
        {
            try
            {
                if (!FileUtility.IsFileExist(inFile))
                    return false;

                FileStream input = new FileStream(inFile, FileMode.Open);
                FileStream output = new FileStream(outFile, FileMode.OpenOrCreate);
                return DeCompress(input, output);
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(ex.Message);
            }
            return false;
        }
    }
}