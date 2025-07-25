using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class FileUtility
{
    public static void CopyFile(string srcPath, string destPath)
    {
        var dir = Path.GetDirectoryName(destPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.Copy(srcPath, destPath);
    }

    public static void MoveFile(string srcPath, string destPath)
    {
        var dir = Path.GetDirectoryName(destPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (!File.Exists(srcPath))
        {
            MyLog.LogError("文件不存在：" + srcPath);
            return;

        }
        File.Move(srcPath, destPath);
    }

    public delegate bool MoveCondition(string srcPath);

    public static void MoveDirectory(string srcPath, string destPath, MoveCondition conditionFunc)
    {
        DirectoryInfo dir = new DirectoryInfo(srcPath);
        if (dir == null || !dir.Exists)
        {
            MyLog.LogError("文件夹不存在：" + srcPath);
            return;
        }

        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
        foreach (FileSystemInfo i in fileinfo)
        {
            if (i is DirectoryInfo)     //判断是否文件夹
            {
                MoveDirectory(i.FullName, destPath + "/" + i.Name, conditionFunc);    //递归调用复制子文件夹
            }
            else
            {
                if (conditionFunc(i.FullName))
                {
                    //MyLog.Log("MoveFile === " + i.FullName + " -> " + destPath + "/" + i.Name);
                    MoveFile(i.FullName, destPath + "/" + i.Name);
                }
            }
        }
    }


    public static void CopyDirectory(string srcPath, string destPath, string ignoreExt = "")
    {
        DirectoryInfo dir = new DirectoryInfo(srcPath);
        if (dir == null || !dir.Exists)
        {
            MyLog.LogError("文件夹不存在：" + srcPath);
            return;
        }

        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
        }

        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
        foreach (FileSystemInfo i in fileinfo)
        {
            if (i is DirectoryInfo)     //判断是否文件夹
            {
                CopyDirectory(i.FullName, destPath + "/" + i.Name);    //递归调用复制子文件夹
            }
            else
            {
                if (string.IsNullOrEmpty(ignoreExt) || i.Extension != ignoreExt)
                {
                    //MyLog.Log("i.FullName:" + i.FullName + " -> " + destPath + "/" + i.Name);
                    File.Copy(i.FullName, destPath + "/" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                }
            }
        }
    }

    public static void DelectDir(string srcPath)
    {
        if (!Directory.Exists(srcPath))
        {
            return;
        }
        DirectoryInfo dir = new DirectoryInfo(srcPath);
        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
        foreach (FileSystemInfo i in fileinfo)
        {
            if (i is DirectoryInfo)            //判断是否文件夹
            {
                DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                subdir.Delete(true);          //删除子目录和文件
            }
            else
            {
                File.Delete(i.FullName);      //删除指定文件
            }
        }
    }
}
