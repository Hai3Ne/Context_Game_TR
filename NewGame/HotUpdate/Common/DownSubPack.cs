using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;


namespace HotUpdate
{
    public class DownSubProgressEvent
    {
        public EVersionState state;
        public long curPro;         //当前进度值
        public long dataLength;     //数据总量
        public string info;
        public int DownCount;
        public int DownCurCount;

    }

    public class DownSubPack : SingletonMonoBehaviour<DownSubPack>
    {
        private List<string[]> updateFileArr = new List<string[]>();
        private Dictionary<string, DownSubProgressEvent> subProssDic = new Dictionary<string, DownSubProgressEvent>();
        public void downSubPack(string name)
        {
            if (!subProssDic.ContainsKey(name))
            {
                subProssDic[name] = new DownSubProgressEvent();
                subProssDic[name].curPro = 0;
            }
            DownSubPackFile(name);

        }

        public void DownSubPackFile(string name)
        {
            updateFileArr.Clear();
            var _cdnUrl = GameConst.CdnUrl;


            var strPath = _cdnUrl + AppConst.SubPackName + "/" + name + "/";
    
            var strSave = GameConst.DataPath + "DirSubVersion/" + AppConst.SubPackName + "/" + name + "/";
            Debug.LogError(strSave);
            var dir = Path.GetDirectoryName(strSave);
            DirectoryInfo dirOutDir = new DirectoryInfo(dir);

            if (!dirOutDir.Exists)
            {
                Directory.CreateDirectory(dir);
            }
/*            var isWait = true;
            yield return StartCoroutine(SaveAssetFiles(strPath, strSave, () =>
            {
                isWait = false;
            }));*/

          //  yield return new WaitWhile(() => isWait);

            var files = File.ReadAllLines(GameConst.DataPath  + "files.txt");
            int allSize = 0;
            var list = new List<string[]>();
            var subPack = "";
            foreach (var file in files)
            {
                string[] fs = file.Split('|');
                if (fs.Length > 1)
                {
                    if (subPack == name)
                    {
                        list.Add(fs);
                        allSize = allSize + int.Parse(fs[2], new CultureInfo("en"));
                    }

                }
                else
                {
                    if (file.IndexOf("====START") >= 0)
                    {
                        subPack = file.Replace("====START", "");
                    }
                    else if (file.IndexOf("====END") >= 0)
                    {
                        subPack = "";
                    }
                }
               
            }
            updateFileArr = list;
            var loadCont = 0;
            subProssDic[name].DownCount = updateFileArr.Count;
            subProssDic[name].DownCurCount = 0;
            foreach (var file in updateFileArr)
            {
                //下载资源
                StartCoroutine(SaveAssetBundle(strPath, strSave, file, (size) => {
                    //资源下载完成后的回调

                    loadCont++;
                    subProssDic[name].DownCurCount = loadCont;

                    if (loadCont >= subProssDic[name].DownCount)
                    {
                        File.WriteAllText(strSave + "version.ver", HotStart.ins.resVersion + "");
                  
                        var newPath1 = GameConst.DataPath + "DirSubVersion/"+ AppConst.SubPackName + "/" + name;
                        if (Directory.Exists(newPath1))
                        {
                            CopyFolder(newPath1, GameConst.DataPath + "/" + AppConst.SubPackName + "/" + name);
                            Directory.Delete(newPath1, true);
                        }


                        LoadModule.Instance.LoadSubPack();
                        //分包更新进度
                        subProssDic[name].curPro = subProssDic[name].curPro + size;
                        subProssDic[name].dataLength = allSize;
                        var str = name + "|" + subProssDic[name].curPro + "|" + allSize;
                        Message.Broadcast(MessageName.REFRESH_MAINUIDOWN_PANEL + name, Mathf.Min((float)subProssDic[name].curPro / allSize, 1),true);
                    }
                    else
                    {
                        //分包更新进度
                        subProssDic[name].curPro = subProssDic[name].curPro + size;
                        subProssDic[name].dataLength = allSize;
                        var str = name + "|" + subProssDic[name].curPro + "|" + allSize;
                        Message.Broadcast(MessageName.REFRESH_MAINUIDOWN_PANEL + name, Mathf.Min((float)subProssDic[name].curPro / allSize, 1), false);
                    }
                }));
            }
        }
        private void CopyFolder(string srcPath, string tarPath)
        {
            if (!Directory.Exists(srcPath))
            {
                Debug.Log("CopyFolder is finish.");
                return;
            }

            if (!Directory.Exists(tarPath))
            {
                Directory.CreateDirectory(tarPath);
            }

            //获得源文件下所有文件
            List<string> files = new List<string>(Directory.GetFiles(srcPath));
            files.ForEach(f =>
            {
                string destFile = Path.Combine(tarPath, Path.GetFileName(f));
                File.Copy(f, destFile, true); //覆盖模式
            });

            //获得源文件下所有目录文件
            List<string> folders = new List<string>(Directory.GetDirectories(srcPath));
            folders.ForEach(f =>
            {
                string destDir = Path.Combine(tarPath, Path.GetFileName(f));
                CopyFolder(f, destDir); //递归实现子文件夹拷贝
            });
        }
        IEnumerator SaveAssetFiles(string path, string savePath, Action DownLoad = null)
        {
            //服务器上的文件路径
            Debug.LogError(path);
            string originPath = path + "files.txt";
            originPath = Uri.EscapeDataString(originPath);
            originPath = originPath.Replace("%2F", "/");
            Debug.LogError(originPath);
            WWW request = new WWW(originPath);
            {
                yield return request;

                if (request.error != null)
                {
                    request.Dispose();
                    StartCoroutine(SaveAssetFiles(path, savePath, DownLoad));
                }
                else
                {
                    //下载完成后执行的回调
                    if (request.isDone)
                    {
                        byte[] results = request.bytes;

                        var dir = Path.GetDirectoryName(savePath + "files.txt");
                        DirectoryInfo dirOutDir = new DirectoryInfo(dir);
                        if (!dirOutDir.Exists)
                        {
                            Directory.CreateDirectory(dir);
                        }
                        FileInfo fileInfo = new FileInfo(savePath + "files.txt");
                        FileStream fs = fileInfo.Create();
                        //fs.Write(字节数组, 开始位置, 数据长度);
                        fs.Write(results, 0, results.Length);
                        fs.Flush(); //文件写入存储到硬盘
                        fs.Close(); //关闭文件流对象
                        fs.Dispose(); //销毁文件对象
                        request.Dispose();
                        if (DownLoad != null)
                            DownLoad();
                    }
                }
            }
        }


        IEnumerator SaveAssetBundle(string path, string savePath, string[] fileArr, Action<int> DownLoad = null)
        {
            //服务器上的文件路径
            string originPath = path  + fileArr[0];
           // originPath = Uri.EscapeDataString(originPath);
            //originPath = originPath.Replace("%2F", "/");
            if (File.Exists(savePath + fileArr[0]))
            {
                if (DownLoad != null)
                    DownLoad(int.Parse(fileArr[2], new CultureInfo("en")));
                yield break;
            }
            Debug.Log(originPath);
            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(originPath))
            {
                request.timeout = 30;
                yield return request.SendWebRequest();

                if (request.isHttpError || request.isNetworkError)
                {

                    Debug.LogError(fileArr[0]);

                    request.Dispose();
                    StartCoroutine(SaveAssetBundle(path, savePath, fileArr, DownLoad));
                }
                else
                {
                    if (!request.isDone)
                        Debug.LogError(request.isDone);
                    //下载完成后执行的回调
                    if (request.isDone)
                    {
                        byte[] results = request.downloadHandler.data;

                        var dir = Path.GetDirectoryName(savePath  + fileArr[0]);
                        DirectoryInfo dirOutDir = new DirectoryInfo(dir);

                        if (!dirOutDir.Exists)
                        {
                            Directory.CreateDirectory(dir);
                        }



                        FileInfo fileInfo = new FileInfo(savePath  + fileArr[0]);
                        FileStream fs = fileInfo.Create();
                        //fs.Write(字节数组, 开始位置, 数据长度);
                        fs.Write(results, 0, results.Length);
                        fs.Flush(); //文件写入存储到硬盘
                        fs.Close(); //关闭文件流对象
                        fs.Dispose(); //销毁文件对象
                        request.Dispose();
                        if (DownLoad != null)
                            DownLoad(int.Parse(fileArr[2], new CultureInfo("en")));

                    }

                }
            }
        }


        public override void Init()
        {

        }
    }
}

