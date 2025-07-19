using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class RoomTypeData
    {
        public int roomId;
        public int id;
        public int roomType;
    }

    public partial class CommonUpdatePanel : PanelBase
    {

        private RoomTypeData roomTypeData;


        private List<string[]> updateFileArr = new List<string[]>();
        private float m_MaxCount = 0.99f;
        private float m_Count = 0f;
        private float m_Index = 0.001f;
        private Dictionary<string, string[]> oldFileArr = new Dictionary<string, string[]>();
        private Dictionary<string, string[]> newFileArr = new Dictionary<string, string[]>();
        private bool isUpdate = false;
        private bool isPause = false;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }
        // Start is called before the first frame update
        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener(MessageName.ENTER_GAME_ROOM, EnterRoom);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);
            m_Count = 0f;
            m_MaxCount = 0.4f;
            m_Index = 0.001f;

            isUpdate = false;
            roomTypeData = (RoomTypeData)param;
            var canvas = this.GetComponent<CanvasScaler>();
            var panel = transform.Find("Panel1");
            var pane2 = transform.Find("Panel2");
            bool showPanel1 = roomTypeData.id != 10 && roomTypeData.id != 13 && roomTypeData.id != 14;
            panel.gameObject.SetActive(showPanel1);
            pane2.gameObject.SetActive(!showPanel1);
            if(!showPanel1)
            {
                m_Trans_game10.gameObject.SetActive(roomTypeData.id == 10);
                m_Trans_game13.gameObject.SetActive(roomTypeData.id == 13);
                m_Trans_game14.gameObject.SetActive(roomTypeData.id == 14);
            }
            if (roomTypeData.id == 12)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                canvas.referenceResolution = new Vector2(0, 750);
                m_Trans_hor.gameObject.SetActive(true);
                m_Trans_ver.gameObject.SetActive(false);
            }
            else
            {       
                Screen.orientation = ScreenOrientation.Portrait;
                canvas.referenceResolution = new Vector2(750, 0);
                m_Trans_hor.gameObject.SetActive(false);
                m_Trans_ver.gameObject.SetActive(true);
            }

            if (!GameConst.isEditor)
            {
                StartCoroutine(checkUpdateSub());
            }
            else
            {
                isUpdate = true;
                m_Index = 0.02f;
                m_MaxCount = 0.9f;
                MainUICtrl.Instance.SendEnterGameRoom(roomTypeData.id, roomTypeData.roomType);
            }

        }

        IEnumerator checkUpdateSub()
        {
            var configData = ConfigCtrl.Instance.Tables.TbGameRoomConfig.Get(roomTypeData.roomId);

            var name = configData.PackName;


            var version = HotStart.ins.resVersion;
    
            var strSave = GameConst.DataPath + AppConst.SubPackName + "/" + name + "/";

            var strPath = GameConst.CdnUrl + AppConst.SubPackName + "/" + name + "/";
            var strDownSave = GameConst.DataPath + "DirSubVersion/" + AppConst.SubPackName + "/" + name + "/";
            var localVersionPath = strSave + "version.ver";
            var localVersion = 1000;

            var dir = Path.GetDirectoryName(strSave);
            DirectoryInfo dirOutDir = new DirectoryInfo(dir);
            if (!dirOutDir.Exists)
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(localVersionPath))
            {
                var txt = File.ReadAllText(localVersionPath);
                localVersion = int.Parse(txt, new CultureInfo("en"));
            }

            if (version <= localVersion)
            {
                isUpdate = true;
                m_Index = 0.02f;
                m_MaxCount = 0.9f;
                MainUICtrl.Instance.SendEnterGameRoom(roomTypeData.id, roomTypeData.roomType);
                yield break;
            }
            string outfile;
            string[] files;
            updateFileArr.Clear();
            oldFileArr.Clear();
            newFileArr.Clear();
            var isWait = true;
            var localFile = strSave + "files.txt";
            if (File.Exists(localFile))
            {
                var lines = File.ReadAllLines(localFile);
                foreach (var item in lines)
                {
                    string[] fs = item.Split('|');
                    oldFileArr.Add(fs[0], fs);
                }
            }
            else
            {
                var subPath = Util.AppContentPath() + AppConst.SubPackName + "/" + name + "/" + "files.txt";
                yield return StartCoroutine(SaveAssetFiles1(subPath, (text) =>
                {

                    if (!Directory.Exists(strSave))
                    {
                        Directory.CreateDirectory(strSave);
                    }
                    File.WriteAllText(localFile, text);
                    isWait = false;
                }));
                yield return new WaitWhile(() => isWait);
                var lines = File.ReadAllLines(localFile);
                foreach (var item in lines)
                {
                    string[] fs = item.Split('|');
                    oldFileArr.Add(fs[0], fs);
                }
            }

            isWait = true;
            yield return StartCoroutine(SaveAssetFiles(strPath, strDownSave, () =>
            {
                isWait = false;
            }));
            yield return new WaitWhile(() => isWait);
            if (File.Exists(strDownSave + "files.txt"))
            {
                files = File.ReadAllLines(strDownSave + "files.txt");
                foreach (var file in files)
                {
                    var des1 = CommonTools.DecryptDES(file);
                    string[] fs = des1.Split('|');
                    newFileArr.Add(fs[0], fs);
                }
            }

            float AllfileSize = 0;
            int curSize = 0;
            float curPro = 0;
            var loadCont = 0;
            foreach (var item in newFileArr)
            {
                if (oldFileArr.ContainsKey(item.Key))
                {
                    if (oldFileArr[item.Key][1] != item.Value[1])
                    {
                        AllfileSize = AllfileSize + int.Parse(item.Value[2], new CultureInfo("en"));
                        updateFileArr.Add(item.Value);
                    }
                }
                else
                {

                    AllfileSize = AllfileSize + int.Parse(item.Value[2], new CultureInfo("en"));
                    updateFileArr.Add(item.Value);
                }
            }
            if (updateFileArr.Count > 0)
            {
                foreach (var file in updateFileArr)
                {
                    //下载资源
                    StartCoroutine(SaveAssetBundle(strPath, strDownSave, file, (size) => {
                        //资源下载完成后的回调
                        loadCont++;
                        if (!isPause)
                        {
                            curPro = m_Count;
                            isPause = true;

                        }
                        if (loadCont >= updateFileArr.Count)
                        {
                            File.WriteAllText(strSave + "version.ver", HotStart.ins.resVersion + "");
                          
                            var newPath1 = GameConst.DataPath + "DirSubVersion/" + AppConst.SubPackName + "/" + name;
                            if (Directory.Exists(newPath1))
                            {
                                CopyFolder(newPath1, GameConst.DataPath + "/" + AppConst.SubPackName + "/" + name);
                                Directory.Delete(newPath1, true);
                            }
                            isPause = false;
                            m_Count = 0.9f;
                            m_Img_Pross.fillAmount = 0.9f;
                            m_Img_Pross1.fillAmount = 0.9f;
                            isUpdate = true;
                            m_Index = 0.02f;
                            m_MaxCount = 0.9f;
                            MainUICtrl.Instance.SendEnterGameRoom(roomTypeData.id, roomTypeData.roomType);
                        }
                        else
                        {
                            curSize += size;
                            m_Img_Pross.fillAmount = curPro + (curSize / AllfileSize) * (0.9f - curPro);
                            m_Img_Pross1.fillAmount = curPro + (curSize / AllfileSize) * (0.9f - curPro);
                        }

                    }));
                }
            }
            else
            {
                
                isUpdate = true;
                m_Index = 0.02f;
                m_MaxCount = 0.9f;
                File.WriteAllText(strSave + "version.ver", HotStart.ins.resVersion + "");
                MainUICtrl.Instance.SendEnterGameRoom(roomTypeData.id, roomTypeData.roomType);
            }



            yield break;
        }

        IEnumerator SaveAssetFiles1(string path, Action<string> DownLoad = null)
        {
            //本地上的文件路径
            var originPath = path;
            Debug.Log(originPath);

#if UNITY_IOS
            if (File.Exists(originPath))
            {
                var bytes = File.ReadAllText(path);
                if (DownLoad != null)
                    DownLoad(bytes);
            }
            else
            {
                Debug.LogError("error : " + originPath);
            }
#else


            WWW request = new WWW(originPath);
            yield return request;

            if (request.error != null)
            {
                request.Dispose();
                StartCoroutine(SaveAssetFiles1(path, DownLoad));
            }
            else
            {
                //下载完成后执行的回调
                if (request.isDone)
                {
                    if (DownLoad != null)
                        DownLoad(request.text);

                    request.Dispose();
                }
            }
#endif
            yield break;
        }


        IEnumerator SaveAssetFiles(string path, string savePath, Action DownLoad = null)
        {
            //服务器上的文件路径
            string originPath = path + "files.txt";
            originPath = Uri.EscapeDataString(originPath);
            originPath = originPath.Replace("%2F", "/");
            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(originPath))
            {
                request.timeout = 30;
                yield return request.SendWebRequest();

                if (request.isHttpError || request.isNetworkError)
                {
                    request.Dispose();
                    StartCoroutine(SaveAssetFiles(path, savePath, DownLoad));
                }
                else
                {
                    //下载完成后执行的回调
                    if (request.isDone)
                    {
                        byte[] results = request.downloadHandler.data;

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
            string originPath = path + fileArr[0];
            originPath = Uri.EscapeDataString(originPath);
            originPath = originPath.Replace("%2F", "/");

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


        private int _tryCountResLimit;

        public void FinishCallBack()
        {
            Message.Broadcast(MessageName.SHOW_NOTICE_STATE, 2);
            UICtrl.Instance.CloseView("CommonUpdatePanel");
        }

        public void EnterRoom()
        {
            Debug.Log("EnterRoom");
            m_MaxCount = 1f;
        }
        public void gotoRoom()
        {
            switch (MainUIModel.Instance.RoomData.nGameType)
            {
                case 1:
                    //50线  90 70  -8 326 1
                    Game500Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom500", null, false, FinishCallBack);
                    break;
                case 2:
                    //9线奖池  110 70 -18  2
                    Game700Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom700", null, false, FinishCallBack);
                    break;
                case 3:
                    //9线炸弹  100 70 -4.45 3
                    Game700Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom700", null, false, FinishCallBack);
                    break;
                case 4:
                    //宙斯  100 70 -12.48 4
                    UICtrl.Instance.OpenView("ZeusPanel", MainUIModel.Instance.RoomData.nRoomType, false, FinishCallBack);
                    break;
                case 5:
                    //单线  105  70 16 5
                    Game800Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom800", null, false, FinishCallBack);
                    break;
                case 6:
                    //9线足球  110 70 -18  2
                    Game601Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom601", null, false, FinishCallBack);
                    break;
                case 7:
                    //9线足球  110 70 -18  2
                    Game1100Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1100", null, false, FinishCallBack);
                    break;
                case 8:
                    //9线足球  110 70 -18  2
                    Game900Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom900", null, false, FinishCallBack);
                    break;
                case 9:
                    //9线足球  110 70 -18  2
                    Game1300Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1300", null, false, FinishCallBack);
                    break;
                case 10:
                    //老虎
                    Game1200Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1200", null, false, FinishCallBack);
                    break;
                case 11:
                    //万圣节
                    Game1000Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1000", null, false, FinishCallBack);
                    break;
                case 12:
                    //豪车飘逸
                    Game1400Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1400", true, true, FinishCallBack);
                    break;
                case 13://牛
                    Game1500Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1500", null, false, FinishCallBack);
                    break;
                case 14:
                    //兔子
                    Game1600Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1600", null, false, FinishCallBack);
                    break;
                case 20:
             
                   
                    UICtrl.Instance.OpenView("HappyGridPanel", null, false, FinishCallBack);
                    break;
                default:
                    break;
            }

        }

        public void ReloadGame()
        {
            if (isUpdate)
            {
                MainUICtrl.Instance.SendEnterGameRoom(roomTypeData.id, roomTypeData.roomType);
            }
            else
            {
                // StartCoroutine(checkUpdateSub());
            }
        }

        // Update is called once per frame
        protected override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener(MessageName.ENTER_GAME_ROOM, EnterRoom);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
        }
        protected override void Update()
        {
            base.Update();
            if (isPause == true) return;
            if (m_Count >= 1)
            {
                return;
            }
            if (m_Count < m_MaxCount)
            {
                m_Count += m_Index;
                m_Img_Pross.fillAmount = m_Count;
                m_Img_Pross1.fillAmount = m_Count;
                if (m_Count >= 1)
                {
                    gotoRoom();
                }
            }
        }
    }
}

