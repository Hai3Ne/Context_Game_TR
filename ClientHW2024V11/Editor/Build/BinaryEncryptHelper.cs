using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class BinaryEncryptHelper 
{
    /// <summary>
    /// 文件解密
    /// </summary>
    /// <param name="encryptedFile"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static byte[] DeEncrypthFile(byte[] encryptedFile)
    {
        byte[] originalFile = new byte[encryptedFile.Length];

        byte keyValue = byte.Parse(GameConst.EncryptKey);
        for (int i = 0; i < encryptedFile.Length; i++)
        {
            originalFile[i] = (byte)(encryptedFile[i] ^ keyValue);
        }

        return originalFile;
    }


    /// <summary>
    /// 文件加密
    /// </summary>
    /// <param name="targetFile"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static byte[] EncryptFile(byte[] targetFile)
    {
        byte[] encryptedFile = new byte[targetFile.Length];
        byte keyValue = byte.Parse(GameConst.EncryptKey);

        for (int i = 0; i < targetFile.Length; i++)
        {
            encryptedFile[i] = (byte)(targetFile[i] ^ keyValue);
        }

        return encryptedFile;
    }
}


