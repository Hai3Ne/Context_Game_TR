using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Core;

using Object = UnityEngine.Object;

public static class EditorHelper
{
 

    public static string GetStringMD5(string sDataIn)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bytValue, bytHash;
        bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
        bytHash = md5.ComputeHash(bytValue);
        md5.Clear();
        string sTemp = "";
        for (int i = 0; i < bytHash.Length; i++)
        {
            sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
        }
        return sTemp.ToLower();
    }

    public static string MD5(string filepath)
    {
        try
        {
            FileStream file = new FileStream(filepath, System.IO.FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
        }

    }
	 

    /// 检测是否有中文字符
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public static bool IsHasCHZN(string inputData)
    {
        Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
        Match m = RegCHZN.Match(inputData);
        return m.Success;
    }


    public static bool IsWindows
    {
        get
        {
            return Environment.OSVersion.Platform == PlatformID.Win32Windows
            || Environment.OSVersion.Platform == PlatformID.Win32NT
            || Environment.OSVersion.Platform == PlatformID.Win32S
            || Environment.OSVersion.Platform == PlatformID.Win32Windows;
        }
    }

    //判断操作系统是否为WindowsXP
    public static bool IsWindowsXP
    {
        get
        {
            return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 1);
        }
    }

    //判断操作系统是否为Windows7
    public static bool IsWindows7
    {
        get
        {
            return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1);
        }
    }

    public static bool IsMac
    {
        get
        {
            return Environment.OSVersion.Platform == PlatformID.MacOSX;
        }
    }

    public static bool IsUnix
    {
        get
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }
    }

    public static bool CheckPython
    {
        get
        {


            if (IsMac)
            {
                return true;
            }
            else if (IsWindows)
            {
                string sPath = Environment.GetEnvironmentVariable("Path");
                return sPath.ToLower().IndexOf("python32") != -1;
            }

            return false;
        }
    }

    static List<string> fileList = new List<string>();

    public static string[] GetAllFilesInDir(string dir, string pattern = null, bool inAssets = false)
    {
        fileList.Clear();
        PushFile(dir, pattern, inAssets);
        return fileList.ToArray();

    }

    public static string GetUnityPath(string path)
    {

        if (path.Contains("\\"))//Application.platform != RuntimePlatform.OSXEditor
        {
            int index = path.IndexOf("Assets\\");
            return path.Substring(index);
        }
        else
        {
            int index = path.IndexOf("Assets/");
            return path.Substring(index);
        }


    }

    static void PushFile(string dir, string pattern, bool inAssets)
    {
        foreach (string d in Directory.GetFileSystemEntries(dir))
        {
            if (File.Exists(d))
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    fileList.Add(d);
                }
                else if (d.EndsWith(pattern))
                {

                    string originPath = d;
                    if (inAssets)
                    {
                        originPath = GetUnityPath(originPath);

                    }

                    fileList.Add(originPath);
                }

            }
            else
            {
                DirectoryInfo d1 = new DirectoryInfo(d);
                if (d1.GetFiles().Length != 0)
                {
                    PushFile(d, pattern, inAssets);
                }
            }
        }


    }

    /// <summary>
    /// 自己也清空
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteDirectory(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            DirectoryInfo[] childs = dir.GetDirectories();
            foreach (DirectoryInfo child in childs)
            {
                child.Delete(true);
            }
            dir.Delete(true);
        }
    }
    /// <summary>
    /// 只清空内容文件
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static bool DeleteFolder(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        else
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    Directory.Delete(d);
                }
            }
        }
        return true;
    }

    public static bool CopyDirectory(string sourcePath, string destinationPath)
    {
        DirectoryInfo info = new DirectoryInfo(sourcePath);
        Directory.CreateDirectory(destinationPath);
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            string destName = Path.Combine(destinationPath, fsi.Name);
            if (destName.EndsWith("DS_Store"))
                continue;

            if (fsi is System.IO.FileInfo)
                File.Copy(fsi.FullName, destName);
            else
            {
                Directory.CreateDirectory(destName);
                CopyDirectory(fsi.FullName, destName);
            }
        }
        return true;
    }


    /// <summary>
    /// 压缩文件
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="destinationZipFilePath"></param>
    public static void CreateZip(string sourceFilePath, string destinationZipFilePath, int level)
    {
        if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            sourceFilePath += System.IO.Path.DirectorySeparatorChar;

        ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
        zipStream.UseZip64 = UseZip64.Off;
        zipStream.SetLevel(level);  // 压缩级别 0-9
        CreateZipFiles(sourceFilePath, zipStream, sourceFilePath);

        zipStream.Finish();
        zipStream.Close();
    }

    /// <summary>
    /// 递归压缩文件
    /// </summary>
    /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
    /// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名</param>
    /// <param name="staticFile"></param>
    private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, string staticFile)
    {
        //		Crc32 crc = new Crc32();
        string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
        foreach (string file in filesArray)
        {
            if (Directory.Exists(file))
            {                     //如果当前是文件夹，递归
                CreateZipFiles(file, zipStream, staticFile);
            }
            else
            {                                            //如果是文件，开始压缩
                FileStream fileStream = File.OpenRead(file);

                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                string tempFile;
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    tempFile = file.Substring(staticFile.LastIndexOf("/") + 1);
                }
                else
                {
                    tempFile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                }

                ZipEntry entry = new ZipEntry(tempFile);

                entry.DateTime = DateTime.Now;
                entry.Flags = 8;
                //				entry.Size = fileStream.Length;
                fileStream.Close();
                //				crc.Reset();
                //				crc.Update(buffer);
                //				entry.Crc = crc.Value;
                zipStream.PutNextEntry(entry);

                zipStream.Write(buffer, 0, buffer.Length);
            }
        }
    }


    public static void processCommand(string command, string argument)
    {
        UnityEngine.Debug.Log("Run command  >>  " + command + " " + argument);

        ProcessStartInfo start = new ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = true;

        if (start.UseShellExecute)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        Process p = Process.Start(start);

        if (!start.UseShellExecute)
        {
            printOutPut(p.StandardOutput, Application.dataPath + "/StandardOutput.txt");
            printOutPut(p.StandardError, Application.dataPath + "/StandardError.txt");
        }

        p.WaitForExit();
        p.Close();
    }

    static void printOutPut(StreamReader stream, string path)
    {
        using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            string Content = stream.ReadToEnd();
            byte[] bys = System.Text.Encoding.UTF8.GetBytes(Content);

            fs.Write(bys, 0, bys.Length);
            fs.Close();
        }

        stream.Close();
    }


    public static void UnZipFile(string zipFilePath, string outPath)
    {
        if (!File.Exists(zipFilePath))
        {
            UnityEngine.Debug.LogError("Cannot find file " + zipFilePath);
            return;
        }

        string baseUnzipPath = outPath;

        using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
        {

            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {

                string directoryName = Path.Combine(baseUnzipPath, Path.GetDirectoryName(theEntry.Name));
                string fileName = Path.Combine(baseUnzipPath, theEntry.Name);
                //				Debugger.Log("dirname = "+ directoryName+" filename = "+ fileName);

                // create directory
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                if (fileName != String.Empty)
                {
                    using (FileStream streamWriter = File.Create(fileName))
                    {// new FileStream(theEntry.Name,FileMode.OpenOrCreate,FileAccess.ReadWrite))

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
