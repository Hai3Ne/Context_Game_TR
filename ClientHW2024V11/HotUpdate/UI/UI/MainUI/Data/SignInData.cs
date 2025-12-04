using HotUpdate;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SignInData 
{
    public int signInDay ;//签到天数
    public int IsSignToday;//0,未签到 1，已签到
    public long SignTime;//签到时间
    public Dictionary<int, bool> signInDayDatas = new Dictionary<int, bool>();//签到列表
    public void SetSignInData(SC_SCENE_SHOW_ME_INFO data) 
    {
        for (int i = 1; i < 8; i++)
        {
            signInDayDatas.Add(i, ToolUtil.ValueByBit(data.m_i4SignIn, i));
        }
        var isShow = false;
        foreach (var item in signInDayDatas)
        {
            if (!item.Value)
            {
                isShow = true;
                signInDay = item.Key;
                break;
            }
        }
        if (!isShow)
        {
            signInDay = 8;
        }

        IsSignToday = data.m_iSignDay;
        SignTime = data.m_i8SignInTime;
        Message.Broadcast(MessageName.REFRESH_ROLLBANNER_PANEL);
        // foreach(var item in signInDayDatas)
        // {
        //     UnityEngine.Debug.Log($"[SignInData] Day {item.Key}: signed={item.Value}");
        // }
    }
    
    /// <summary>
    /// 设置今日是否签到
    /// </summary>
    /// <param name="index">0,未签到 1，已签到</param>
    public void SetIsSignToday(int index) 
    {
        IsSignToday = index;
    }

    public void SetSignInDay(int index) 
    {
        var isShow = false;
        signInDayDatas[index] = true;
        foreach (var item in signInDayDatas)
        {
            if (!item.Value)
            {
                isShow = true;
                signInDay = item.Key;
                break;
            }
        }
        if (!isShow)
        {
            signInDay = 8;
        }
        Message.Broadcast(MessageName.REFRESH_ROLLBANNER_PANEL);
    }

    /// <summary>
    /// 设置领取时间
    /// </summary>
    /// <param name="time"></param>
    public void SetSignTime(long time) 
    {
        SignTime = time;
    }
    
    public long GetTimeUntilNextSignIn()
    {
        if (IsSignToday == 0)
        {
            return 0;
        }
        long currentTime = ToolUtil.getServerTime();
        DateTime currentDate = TimeUtil.TimestampToDataTime(currentTime);
        DateTime nextDay = currentDate.Date.AddDays(1);
        long nextDayTimestamp = TimeUtil.DataTimeToTimestamp(nextDay);
    
        return Math.Max(0, nextDayTimestamp - currentTime);
    }

    public string GetCountdownText()
    {
        if (IsSignToday == 0)
        {
            return "您现在就可以得到它!";
        }
    
        long timeLeft = GetTimeUntilNextSignIn();
        TimeSpan time = TimeSpan.FromSeconds(timeLeft);
        var timeText = $"当日奖励乘余时间: {time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        return timeText;
    }
}

