using System.Collections;
using System.Collections.Generic;

using System;
using UnityEngine;
namespace HotUpdate {
    public class XxlCtrl : Singleton<XxlCtrl>
    {
        public bool isInit = false;
        public int level = 1;
        public int gold = 1000000;
        public int sign = 1;
        public int signDay = 0;
        public bool signInDay = false;
        public List<int> list;
        public int nowLevel;
        public string name;
        public string uid;
        public void init()
        {
            if (isInit) return;
            isInit = true;
            if (PlayerPrefs.HasKey("GAME-LEVEL"))
            {
                level = PlayerPrefs.GetInt("GAME-LEVEL");
            }
            else
            {
                level = 1;
            }

            if (PlayerPrefs.HasKey("GAME-Name"))
            {
                name = PlayerPrefs.GetString("GAME-Name");
            }
            else
            {
                name = "游客" +  UnityEngine.Random.Range(10000,99999);
                PlayerPrefs.SetString("GAME-Name", name);
            }

            if (PlayerPrefs.HasKey("GAME-UID"))
            {
                uid = PlayerPrefs.GetString("GAME-UID");
            }
            else
            {
                uid = "" + UnityEngine.Random.Range(100000000, 999999999);
                PlayerPrefs.SetString("GAME-UID", uid);
            }

            if (PlayerPrefs.HasKey("GAME-GOLD"))
            {
                gold = PlayerPrefs.GetInt("GAME-GOLD");
            }
            else
            {
                gold = 1000000;
            }

            if (PlayerPrefs.HasKey("GAME-XX"))
            {
                list = new List<int>();
                var str = PlayerPrefs.GetString("GAME-XX");
                var arr = str.Split(",");
                for (int i = 0; i < arr.Length; i++)
                {
                    list.Add(int.Parse(arr[i]));
                }
            }
            else
            {
                list = new List<int>();
            }


            if (PlayerPrefs.HasKey("GAME-SIGN"))
            {
                sign = PlayerPrefs.GetInt("GAME-SIGN");
            }
            else
            {
                sign = 1;
            }

            if (PlayerPrefs.HasKey("GAME-SIGNDay"))
            {
                signDay = PlayerPrefs.GetInt("GAME-SIGNDay");
            }
            else
            {
                signDay = 0;
            }

            signInDay = signDay == new DateTime().Day;
        }

        internal void addLevel(int score)
        {
            var star = 0;
            if(score > 2500)
            {
                star = 3;
            }
            else if (score > 1800)
            {
                star = 2;
            }
            else if(score > 1000)
            {
                star = 1;
            }


            if (XxlCtrl.Instance.nowLevel == XxlCtrl.Instance.level)
            {
                XxlCtrl.Instance.level += 1;
                PlayerPrefs.SetInt("GAME-LEVEL", XxlCtrl.Instance.level);
                list.Add(star);
            }
            else
            {
                list[XxlCtrl.Instance.nowLevel - 1] = star;
            }
            saveList();
        }

        public void saveList()
        {
    
            string result = string.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                result += list[i];
                if (i != list.Count - 1)
                {
                    result += ",";
                }
            }
            PlayerPrefs.SetString("GAME-XX", result);

        }

    }

}
