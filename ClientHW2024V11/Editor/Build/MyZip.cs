using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;


/// <summary>  
/// 文件(夹)压缩、解压缩  
/// </summary>  
public class MyZip
{
    #region 压缩文件
    /// <summary>    
    /// 压缩文件    
    /// </summary>    
    /// <param name="fileNames">要打包的文件列表</param>    
    /// <param name="GzipFileName">目标文件名</param>    
    /// <param name="CompressionLevel">压缩品质级别（0~9）</param>    
    /// <param name="deleteFile">是否删除原文件</param>  
    public static void CompressFile(List<FileInfo> fileNames, string GzipFileName, int CompressionLevel, bool deleteFile)
    {
        ZipOutputStream s = new ZipOutputStream(File.Create(GzipFileName));
        try
        {
            s.SetLevel(CompressionLevel);   //0 - store only to 9 - means best compression    
            foreach (FileInfo file in fileNames)
            {
                FileStream fs = null;
                try
                {
                    fs = file.Open(FileMode.Open, FileAccess.ReadWrite);
                }
                catch
                { continue; }
                //  方法二，将文件分批读入缓冲区    
                byte[] data = new byte[2048];
                int size = 2048;
                ZipEntry entry = new ZipEntry(Path.GetFileName(file.Name));
                entry.DateTime = (file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime);
                s.PutNextEntry(entry);
                while (true)
                {
                    size = fs.Read(data, 0, size);
                    if (size <= 0) break;
                    s.Write(data, 0, size);
                }
                fs.Close();
                if (deleteFile)
                {
                    file.Delete();
                }
            }
        }
        finally
        {
            s.Finish();
            s.Close();
        }
    }
    /// <summary>    
    /// 压缩文件夹    
    /// </summary>    
    /// <param name="dirPath">要打包的文件夹</param>    
    /// <param name="GzipFileName">目标文件名</param>    
    /// <param name="CompressionLevel">压缩品质级别（0~9）</param>    
    /// <param name="deleteDir">是否删除原文件夹</param>  
    public static void CompressDirectory(string dirPath, string GzipFileName, int CompressionLevel, bool deleteDir)
    {
        var dirInfo = new DirectoryInfo(dirPath);
        if(dirInfo == null || !dirInfo.Exists)
        {
            throw new Exception ("directory not exist:" + dirPath);
        }
        dirPath = dirInfo.FullName;
        //压缩文件为空时默认与压缩文件夹同一级目录    
        if (GzipFileName == string.Empty)
        {
            GzipFileName = dirPath.Substring(dirPath.LastIndexOf("//") + 1);
            GzipFileName = dirPath.Substring(0, dirPath.LastIndexOf("//")) + "//" + GzipFileName + ".zip";
        }
        //if (Path.GetExtension(GzipFileName) != ".zip")  
        //{  
        //    GzipFileName = GzipFileName + ".zip";  
        //}  
        using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(GzipFileName)))
        {
            zipoutputstream.SetLevel(CompressionLevel);
            Crc32 crc = new Crc32();
            Dictionary<string, DateTime> fileList = GetAllFies(dirPath);
            foreach (KeyValuePair<string, DateTime> item in fileList)
            {
                FileStream fs = File.OpenRead(item.Key.ToString());
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(item.Key.Substring(dirPath.Length + 1));
                entry.DateTime = item.Value;
                entry.Size = fs.Length;
                fs.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                zipoutputstream.PutNextEntry(entry);
                zipoutputstream.Write(buffer, 0, buffer.Length);
            }
        }
        if (deleteDir)
        {
            Directory.Delete(dirPath, true);
        }
    }
    /// <summary>    
    /// 获取所有文件    
    /// </summary>    
    /// <returns></returns>    
    private static Dictionary<string, DateTime> GetAllFies(string dir)
    {
        Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
        DirectoryInfo fileDire = new DirectoryInfo(dir);
        if (!fileDire.Exists)
        {
            throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
        }
        GetAllDirFiles(fileDire, FilesList);
        GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
        return FilesList;
    }
    /// <summary>    
    /// 获取一个文件夹下的所有文件夹里的文件    
    /// </summary>    
    /// <param name="dirs"></param>    
    /// <param name="filesList"></param>    
    private static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
    {
        foreach (DirectoryInfo dir in dirs)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
            GetAllDirsFiles(dir.GetDirectories(), filesList);
        }
    }
    /// <summary>    
    /// 获取一个文件夹下的文件    
    /// </summary>    
    /// <param name="dir">目录名称</param>    
    /// <param name="filesList">文件列表HastTable</param>    
    private static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
    {
        foreach (FileInfo file in dir.GetFiles("*.*"))
        {
            filesList.Add(file.FullName, file.LastWriteTime);
        }
    }
    #endregion
    #region 解压缩文件
    /// <summary>
    /// 解压缩
    /// </summary>
    /// <param name="sourceFile">源文件</param>
    /// <param name="targetPath">目标路经</param>
    public static bool Decompress(string sourceFile, string targetPath)
    {
        if (!File.Exists(sourceFile))
        {
            throw new FileNotFoundException(string.Format("未能找到文件 '{0}' ", sourceFile));
        }
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        using (var s = new ZipInputStream(File.OpenRead(sourceFile)))
        {
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                if (theEntry.IsDirectory) continue;

                string directorName = Path.Combine(targetPath, Path.GetDirectoryName(theEntry.Name));
                string fileName = Path.Combine(directorName, Path.GetFileName(theEntry.Name));

                if (!Directory.Exists(directorName))
                {
                    Directory.CreateDirectory(directorName);
                }
                if (!String.IsNullOrEmpty(fileName))
                {
                    using (FileStream streamWriter = File.Create(fileName))
                    {
                        int size = 4096;
                        byte[] data = new byte[size];
                        while (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                            size = s.Read(data, 0, data.Length);
                        }
                    }
                }
            }
        }
        return true;
    }
    #endregion
}
