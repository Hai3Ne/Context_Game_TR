using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public class
        UICtrl : SingletonMonoBehaviour<UICtrl>
    {
        public override void Init()
        {
            startInit();
        }

        public async void startInit()
        {
           
            MainPanelMgr.Instance.Init();
            ConfigCtrl.Instance.initCfg(()=>
            {
                loadInit();
            });
            

        }

        public void loadInit()
        {
            if (!GameConst.isEditor)
            {
                var arr = ConfigCtrl.Instance.Tables.TbGameRoomConfig.DataList;
                var list = new List<string>();

                var mainSubArr = AppConst.SubPackArr.Split('|');
                for (int i = 0; i < arr.Count; i++)
                {
                    if (!list.Contains(arr[i].PackName))
                        list.Add(arr[i].PackName);
                }

                for (int j = 0; j < mainSubArr.Length; j++)
                {
                    if (!list.Contains(mainSubArr[j]))
                    {
                        list.Add(mainSubArr[j]);
                    }
                }
                HotStart.ins.SubPackNameArr = list;
                LoadModule.Instance.LoadSubPack();
            }

            MainPanelMgr.Instance.initCfg(ConfigCtrl.Instance.MainPanelList);
            LoginCtrl.Instance.init();

            NetMgr.netMgr.init();
        }
        public static string getTextForStreamingAssets(string path)
        {
#if UNITY_IOS
            if (File.Exists(path)) {
                var bytes = File.ReadAllText(path);
                return bytes;
            }
            else
            {
                Debug.Log("error : " + path);
                return null;          //??????????
            }

#else
            //Debug.Log("localPath =  " + localPath);
            WWW t_WWW = new WWW(path);
            while (!t_WWW.isDone)
            {

            }
            if (t_WWW.error != null)
            {
                Debug.Log("error : " + path);
                return null;          //读取文件出错
            }
             return t_WWW.text;
#endif

        }
        public void initGame()
        {

        }

 

        public void OpenView(string name, object param = null, bool hasEffect = false, System.Action finishCallBack = null)
        {
           
            MainPanelMgr.Instance.ShowPanel(name, param, hasEffect, finishCallBack);
        }

        public void ShowLoading(bool isRecoont = false)
        {
            MainPanelMgr.Instance.ShowDialog("CommonLoading", false, isRecoont);
        }

        public void ShowMainUIPanel() 
        {
            MainPanelMgr.Instance.ShowDialog("MainUIPanel");
        }

        public void CloseLoading()
        {
            MainPanelMgr.Instance.Close("CommonLoading");
        }

        public void ShowUIPanel(string name)
        {
            MainPanelMgr.Instance.ShowPanel(name);
        }

        public void OpenTips()
        {

        }

        public void CloseAllView()
        {
            MainPanelMgr.Instance.HideAllPanel();
        }

        public void CloseView(string name)
        {
            MainPanelMgr.Instance.Close(name);
        }

        public void CloseTips()
        {

        }
    }
}

