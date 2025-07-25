using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class TimeUtil :MonoBehaviour
    {

        public void SetTimed(string time)
        {
            transform.GetComponent<Text>().text = time;
        }
        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime TimestampToDataTime(long unixTimeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            Console.WriteLine(dt.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            return dt;
        }
        /// <summary>
        /// DateTime转时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DataTimeToTimestamp(DateTime dateTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(dateTime); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
            Console.WriteLine(timeStamp);
            return timeStamp;
        }
        /// <summary>
        /// 计算时间差
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="DateTime2"></param>
        /// <returns></returns>
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2, string showStr = null)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                if (ts.TotalSeconds <= 0)
                {
                    dateDiff = $"00:00:00";
                    return dateDiff;
                }
                if(ts.Days <= 0)
                {
                    dateDiff = $"{showStr}{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
                }
                else
                {
                    dateDiff = $"{showStr}{ts.Days}天  {ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
                }
                
            }
            catch
            {
                Debug.LogError("计算失败！");
            }
            return dateDiff;
        }

        public static string DateDiffByDay(string str,DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dateDiff = $"{str} {ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
            }
            catch
            {
                Debug.LogError("计算失败！");
            }
            return dateDiff;
        }

        public static IEnumerator TimeCountDown(float GameTime, TextMeshProUGUI timeCountDown)
        {
            float TimeLeft;
            TimeLeft = GameTime; 

            while (true)
            {
                int _minute =(int) TimeLeft / 60; 
                float _second = TimeLeft % 60; 

                timeCountDown.text = $"00:{_minute.ToString("00")}:{_second.ToString("00")}";

                if (TimeLeft <= 0) 
                {
                    yield break; 
                }

                yield return new WaitForSeconds(1f);
                TimeLeft--;
            }
        }
        public static IEnumerator TimeCountDown(float GameTime, Text timeCountDown, Action action = null)
        {
            float TimeLeft;
            TimeLeft = GameTime;

            while (true)
            {
                float _second = TimeLeft ;
                timeCountDown.text = $"{_second.ToString("00")}s";
                if (TimeLeft <= 0)
                {
                    if (action!=null)
                    {
                        action();
                    }
                    yield break;
                }
                yield return new WaitForSeconds(1f);
                TimeLeft--;
            }
        }
    }
}
