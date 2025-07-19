
﻿using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace SEZSJ
{


    public class timerEvent
    {
        public float time;
        public bool loop;
        public float _curtime;
        public int index;
        public Action callback;
    }

    //游戏逻辑管理器    

    public class TimeMgr : MonoBehaviour
    {

       
        public List<timerEvent> timerList = new List<timerEvent>();

        //游戏开始入口
        void Start()
        {

        }


        //全局时间缩放
        float m_globalScale = 1.0f;
        public float GlobalScale
        {
            get { return m_globalScale; }
            set { m_globalScale = value; }
        }

        //全局缩放

        float m_timeScale= 1;
        public float TimeScale
        {
            get { return Time.timeScale; }
            set { 
                m_timeScale = value;
                Time.timeScale = m_timeScale * m_globalScale; 
            }
        }

        public void AddTimer(float time,bool loop,Action callback,int index)
        {
            timerEvent timer = new timerEvent();
            timer.time = time;
            timer.loop = loop;
            timer.callback = callback;
            timer.index = index;
            timer._curtime = Time.realtimeSinceStartup;
            for(int i = 0;i < timerList.Count;i++)
            {
                if(timerList[i].index == index)
                    return;
            }
            timerList.Add(timer);
        }
        public void RemoveTimer(int index)
        {
            for (int i = timerList.Count - 1;i>=0;i--)
            {
                timerEvent temp = timerList[i];
                if (temp.index == index)
                    timerList.Remove(temp);
            }
        }

        public void Reset()
        {
            timerList.Clear();
        }

        float deltaTime = 0;

        private void Update()
        {
            deltaTime += Time.deltaTime;
            if (timerList.Count <= 0)
                return;
            for (int i = timerList.Count - 1;i>=0;i--)
            {
                if (Time.realtimeSinceStartup - timerList[i]._curtime >= timerList[i].time)
                {
                    timerEvent timer = timerList[i];
                    if (timer.callback != null)
                    {
                        if (timer.loop)
                            timer._curtime = Time.realtimeSinceStartup;
                        else
                        {
                            timerList.Remove(timer);
                        }
                        timer.callback();
                    }
                    if (timerList.Count == i)
                        break;
                    if (i > timerList.Count - 1)
                        break;
                }
            }
        }
    }
 
};//end SG

