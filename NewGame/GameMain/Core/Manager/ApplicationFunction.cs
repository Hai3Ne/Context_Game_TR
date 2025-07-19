
﻿using UnityEngine;
using System.Collections;
using SEZSJ;


public class ApplicationFunction : MonoBehaviour {

    float reconnectedTimeForPause = 45f;



    //public int frameRate = 30;

	// Use this for initialization
	void Start () {

     
       
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_THIRDPARTY_EXIT, GE_THIRDPARTY_EXIT);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_LEAVE_GAME, OnLeaveGame);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_TXDWKINFO_RET, OnTxDwkInfo_Ret);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_TXDTTQINFO_RET, OnSC_TXDTTQINFO_RET_Ret);


	}


    void Update()
    {

    }

    /// <summary>
    /// 监听登出
    /// </summary>
    /// <param name="ge"></param>
    /// <param name="parameter"></param>
    void ConfirmLogout(GameEvent ge, EventParameter parameter)
    {
        Debug.Log("ConfirmLogout");
       // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_LOGOUT);
        //NetMgr.netMgr.Reconnect.onBtnGotoLogin();
    }

    void OnLeaveGame(GameEvent ge, EventParameter parameter)
    {
        //返回到角色选择界面
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CLEANUP_USER_DATA, null);

      
    }

    void OnTxDwkInfo_Ret(GameEvent ge, EventParameter parameter)
    {
        
        int mydata = 10;
    }

    void OnSC_TXDTTQINFO_RET_Ret(GameEvent ge, EventParameter parameter)
    {

        int mydata = 10;
    }

    void OnEnable()
    {
        //Application.targetFrameRate = frameRate;
    }

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("OnApplicationPause  " + pause);
        EventParameter ep = EventParameter.Get(pause ? 1 : 0);
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GAME_PAUSE, ep);
        if (pause)
        {
            OnBeginPause();
        }
        else {
            OnEndPause();
        }

    }

    public float focusOffTime = -1;
    public float focusOnTime = -1;
    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            focusOffTime = Time.realtimeSinceStartup;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Focus_Off, EventParameter.Get(focusOffTime - focusOnTime));
        }
        else
        {
            focusOnTime = Time.realtimeSinceStartup;
            if (focusOffTime > 0)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Focus_On, EventParameter.Get(focusOnTime - focusOffTime));
            }
        }
    }

    void OnApplicationQuit()
    {

    }

   
    
    private System.DateTime beginDateTime;
    void OnBeginPause()
    {
        beginDateTime = System.DateTime.Now;
        Debug.Log("Begin Pause...." + beginDateTime);
    }

    void OnEndPause()
    {
        Debug.Log("OnEndPause 1  " + reconnectedTimeForPause);
        double pausedDeltaSecond = System.DateTime.Now.Subtract(beginDateTime).TotalSeconds;
        if (pausedDeltaSecond > reconnectedTimeForPause)
        {
            Debug.Log("OnEndPause 2");

            CoreEntry.gEventMgr.TriggerEvent(SEZSJ.GameEvent.GE_EndPause, null);
         }

    }

    void GE_THIRDPARTY_EXIT(GameEvent ge, EventParameter parameter)
    {
        if (parameter.intParameter == 0)
        {

        }
        else if (parameter.intParameter == 1)
        {
       
            Application.Quit();
        }
    }
}

