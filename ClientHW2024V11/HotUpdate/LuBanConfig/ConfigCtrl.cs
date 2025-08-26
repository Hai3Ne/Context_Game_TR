using Bright.Serialization;
using cfg;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
namespace HotUpdate
{
    public class ConfigCtrl : SingletonMonoBehaviour<ConfigCtrl>
    {
        public List<MainPanelUICfg> MainPanelList = new List<MainPanelUICfg>();
        public Tables Tables { get; set; }
        public async void initCfg(Action callback)
        {
            Tables = await LoadAllConfig();
            GetUISoundList();
            GetMainPanelUIList();
            if (callback != null)
                callback();
        }

        public void GetUISoundList()
        {
            List<AudioConfig> audioList = new List<AudioConfig>();
            for (int i = 0; i < Tables.TbSounds_Config.DataList.Count; i++)
            {
                var data = Tables.TbSounds_Config.DataList[i];
                AudioConfig cfg = new AudioConfig();
                cfg.id = data.Id;
                cfg.path = data.FileName;
                cfg.type = data.AudioType;
                cfg.Volume = data.TriggerRatio;
                audioList.Add(cfg);
            }
            CoreEntry.gAudioMgr.initCfg(audioList);
        }
        public void GetMainPanelUIList()
        {
            MainPanelList.Clear();
            for (int i = 0; i < Tables.TbUIPanel_Config.DataList.Count; i++)
            {
                MainPanelUICfg cfg = new MainPanelUICfg();
                var data = Tables.TbUIPanel_Config.DataList[i];
                cfg.id = data.Id;
                cfg.ignoreguide = data.Ignoreguide;
                cfg.panelName = data.PanelName;
                cfg.prefabPath = data.PrefabPath;
                cfg.AnimationType = data.AnimationType;
                cfg.cache = data.Cache;
                cfg.enumLayerType = data.EnumLayerType;
                cfg.fullview = data.Fullview;
                cfg.preload = data.Preload;
                cfg.type = data.Type;
                cfg.subTypeLayer = data.SubTypeLayer;
                MainPanelList.Add(cfg);
            }
        }

        public async Task<Tables> LoadAllConfig()
        {
            Tables tables = new Tables();
            await tables.LoadAsync(file => ConfigLoader(file));
            return tables;
        }

        private static async Task<ByteBuf> ConfigLoader(string file)
        {
            string filePath = string.Empty;
            if (GameConst.isEditor)
            {
                //编辑器模式
                filePath = $"{Application.dataPath}/../LubanTools/GenerateDatas/LubanConfig/{file}.bytes";
                // filePath = $"{Application.dataPath}/../Assets/StreamingAssets/LubanConfig/{file}.bytes";
                if (!File.Exists(filePath))
                {
                    Debug.LogError("filepath:" + filePath + " not exists");
                    return null;
                }
            }
            else
            {

                if (!GameConst.isEditor)
                {
                    //单机包模式和热更模式 读取沙盒目录
                    var path = "LubanConfig/" + file + ".bytes";

                    filePath = GameConst.DataPath + path;

                    if (!File.Exists(filePath))
                    {
                        filePath = Util.AppContentPath() + path;
                    }
                    else
                    {
#if UNITY_ANDROID && !UNITY_EDITOR
            filePath = "file://" + filePath;
#endif
                    }


                    // filePath = GameConst.DataPath + "LubanConfig/" + file + ".bytes" +  Path.Combine(GameConst.DataPath, $"LubanConfig/{file}.bytes");
                }
                else
                {
                    //单机包模式和热更模式 读取沙盒目录
                    string streamPath = Util.AppContentPath();
                    filePath = streamPath + "LubanConfig/" + file + ".bytes";
                }
            }

            byte[] data = null;
            await new CoroutineAwaiter(getTextForStreamingAssets(filePath,(newByte)=>
            {
                data = newByte;
            })).Task;

           
            if (!GameConst.isEditor)
            {
                //data = CommonTools.DeEncrypthFile(data);
            }
            return new ByteBuf(data);
        }



        public static IEnumerator getTextForStreamingAssets(string path,Action<byte[]> callback)
        {

#if UNITY_IOS
            if (File.Exists(path))
            {
                var bytes = File.ReadAllBytes(path);
                if (callback != null)
                    callback(bytes);
            }
            else
            {
                Debug.Log("error : " + path);
                yield return null;          //读取文件出错
            }
 
#else
            //Debug.Log("localPath =  " + localPath);
            WWW t_WWW = new WWW(path);
            yield return t_WWW;

            if (t_WWW.error != null)
            {
                Debug.Log("error : " + path);
                yield return null;          //读取文件出错
            }
            if (t_WWW.isDone)
            {
                if (callback != null)
                    callback(t_WWW.bytes);
            }
#endif
            yield break;
        }

        public override void Init()
        {
    
        }
    }
}
