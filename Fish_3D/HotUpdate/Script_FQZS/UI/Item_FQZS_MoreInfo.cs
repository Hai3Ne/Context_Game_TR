using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Item_FQZS_MoreInfo : UIItem
{
    private enum MsgType
    {
        Sys,
        SelfBet,
        Result,
    }

    /// <summary>
    /// 更多按钮的窗口
    /// </summary>
    public GameObject mMoreBtnWindow;

    /// <summary>
    /// 在线玩家的数量
    /// </summary>
    public UILabel mOnlineNumLbl;

    /// <summary>
    /// 信息窗口
    /// </summary>
    public UIScrollView mScrollViewMessage;

    /// <summary>
    /// 系统消息预设
    /// </summary>
    public GameObject SysMessage;

    /// <summary>
    /// 其他消息预设
    /// </summary>
    public GameObject mResultMsg;

    /// <summary>
    /// 自己押注的信息预设
    /// </summary>
    public GameObject mSelfBetMsg;

    /// <summary>
    /// 结算消息Clone列表
    /// </summary>
    private List<GameObject> mResultMsgLst = new List<GameObject>();

    /// <summary>
    /// 自己下注消息的Clone列表
    /// </summary>
    private List<GameObject> mSelfBetMsgLst = new List<GameObject>();

    /// <summary>
    /// 系统消息的Clone列表
    /// </summary>
    private List<GameObject> mSysMessageLst = new List<GameObject>();

    /// <summary>
    /// 音乐按钮背景图
    /// </summary>
    public UISprite mMusicSpr;

    /// <summary>
    /// 音乐停下来的时候显示的图片
    /// </summary>
    public UISprite mMusicStopSpr;

    /// <summary>
    /// 音乐文字
    /// </summary>
    public UILabel mMusicLbl;

    /// <summary>
    /// 在线玩家
    /// </summary>
    public UIScrollView mScrollViewOnLine;

    /// <summary>
    /// 退出游戏委托
    /// </summary>
    private Action QuitFunc;

    /// <summary>
    /// 消息列表
    /// </summary>
    public List<GameObject> mMsgList = new List<GameObject>();

    /// <summary>
    /// 在线玩家列表
    /// </summary>
    private List<GameObject> mOnLineRoleLst = new List<GameObject>();

    /// <summary>
    /// 添加在线玩家的预设
    /// </summary>
    public GameObject mAddRoleObj;

    /// <summary>
    /// 在线玩家数量
    /// </summary>
    private int OnLineNum = 0;

    private GameObject mOpenArrow;

    private GameObject mCloseArrow;

    private void Awake()
    {
        mOnLineRoleLst.Clear();
    }

    public void InitData(Action quitFunc)
    {
        QuitFunc = quitFunc;
    }


    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_more":
                ShowMoreBtnWin();
                break;
            case "btn_safebox":
                ShowSafeBox();
                break;
            case "btn_rule":
                ShowRule();
                break;
            case "btn_exit":
                Quit();
                break;
            case "btn_setting":
                UI.EnterUI<UI_FQZS_Setting>(GameEnum.FQZS).InitData();
                break;
        }
    }

    /// <summary>
    /// 打开安全箱
    /// </summary>
    private void ShowSafeBox() {
        //UI.EnterUI<UI_safebox_new>(ui => {
        //    ui.InitData();
        //});

        UI.EnterUI<UI_safebox_new>(GameEnum.All).InitData();
    }

    /// <summary>
    /// 打开规则说明
    /// </summary>
    private void ShowRule()
    {
        UI.EnterUI<UI_FQZS_Rule>(GameEnum.FQZS);
    }

    /// <summary>
    /// 添加系统消息
    /// </summary>
    /// <param name="context"></param>
    public void AddSysMessage(string context,bool isSys = false)
    {
        if (isSys)
        {
            context = string.Format("[c][ff0000][系统消息]:[-][/c]{0}", context);
        }

        CreateMessageObj(MsgType.Sys, context);

        StartCoroutine(ScorllViewend());
    }

    /// <summary>
    /// 添加在线玩家
    /// </summary>
    /// <param name="roleinfo"></param>
    public void AddRole(RoleInfo roleinfo)
    {
        Item_FQZS_AddRoleItem item = AddItem<Item_FQZS_AddRoleItem>(mAddRoleObj, mScrollViewOnLine.transform);
        item.SetInfo(roleinfo);
        mOnLineRoleLst.Add(item.gameObject);
        for (int i = 0; i < mOnLineRoleLst.Count; i++)
        {
            mOnLineRoleLst[i].transform.localPosition = new Vector3(0, -24 * i);
        }

        OnLineNum++;

        mOnlineNumLbl.text = "在线玩家:" + OnLineNum;
    }

    /// <summary>
    /// 有玩家离线了
    /// </summary>
    /// <param name="roleinfo"></param>
    public void RemoveRole(RoleInfo roleinfo)
    {
        if (mOnLineRoleLst.Count > 0)
        {
            for (int i = 0; i < mOnLineRoleLst.Count; i++)
            {
                if (mOnLineRoleLst[i].GetComponent<Item_FQZS_AddRoleItem>().mRoleInfo.NickName.Equals(roleinfo.NickName))
                {
                    Destroy(mOnLineRoleLst[i]);
                    mOnLineRoleLst.RemoveAt(i);
                    OnLineNum--;
                    break;
                }
            }

            for (int i = 0; i < mOnLineRoleLst.Count; i++)
            {
                mOnLineRoleLst[i].transform.localPosition = new Vector3(0, -24 * i);
            }

            mOnlineNumLbl.text = "在线玩家:" + OnLineNum;
        }
    }

    /// <summary>
    /// 添加自己押注的信息
    /// </summary>
    /// <param name="context"></param>
    public void AddSelfBetMessage(string selfBet)
    {
        if (string.IsNullOrEmpty(selfBet))
        {
            CreateMessageObj(MsgType.SelfBet, "您的竞猜为:,0");
        }
        else
        {
            string betTitle = "[c][AF955F]您的竞猜为:[-][/c]";
            CreateMessageObj(MsgType.Sys, betTitle);
            string[] strArray = selfBet.Split('#');

            for (int i = 0; i < strArray.Length; i++)
            {
                if (string.IsNullOrEmpty(strArray[i]))
                    continue;
                CreateMessageObj(MsgType.SelfBet, strArray[i]);
            }
        }

        StartCoroutine(ScorllViewend());
    }

    /// <summary>
    /// 添加本局结算消息
    /// </summary>
    /// <param name="selectanima"></param>
    /// <param name="leizhuresult"></param>
    /// <param name="selfresult"></param>
    /// <param name="selftotal"></param>
    /// <param name="selffuwu"></param>
    public void AddResultMessage(string selectanima, string leizhuresult, string selfresult, string selftotal, string selffuwu)
    {
        string curResult = string.Format("{0},{1},{2},{3},{4}", selectanima, leizhuresult, selfresult, selftotal, selffuwu);

        CreateMessageObj(MsgType.Result, curResult);

        mScrollViewMessage.verticalScrollBar.enabled = false;


        StartCoroutine(ScorllViewend());
    }

    IEnumerator ScorllViewend()
    {
        yield return new WaitForSeconds(0.15f);

        if (mScrollViewMessage.verticalScrollBar.alpha > 0)
        {
            mScrollViewMessage.verticalScrollBar.enabled = true;
            mScrollViewMessage.verticalScrollBar.value = 1;
        }
    }

    private void CreateMessageObj(MsgType type ,string context)
    {
        GameObject obj = null;
        UILabel lb = null;

        if (type == MsgType.SelfBet)
        {
            string[] array = context.Split(',');
            if (array.Length != 2)
            {
                LogMgr.LogError("传入的消息格式不正确");
                return;
            }

            obj = GetCurMsgObj(type);

            lb = obj.GetComponent<UILabel>();

            lb.text = array[0];
            lb.transform.Find("item_lb_bet_gold").GetComponent<UILabel>().text = array[1];
        }
        else if (type == MsgType.Sys)
        {
            obj = GetCurMsgObj(type);

            lb = obj.GetComponent<UILabel>();

            lb.text = context;
        }
        else if (type == MsgType.Result)
        {
            string[] array = context.Split(',');
            if (array.Length != 5)
            {
                LogMgr.LogError("传入的消息格式不正确");
                return;
            }

            obj = GetCurMsgObj(type);

            UILabel select_anima = obj.transform.Find("lb_select_anima").GetComponent<UILabel>();
            select_anima.text = string.Format("【{0}】", array[0]);
            UILabel leizhu_result = obj.transform.Find("lb_leizhu_result").GetComponent<UILabel>();
            leizhu_result.text = array[1];

            UILabel self_result = obj.transform.Find("lb_self_result").GetComponent<UILabel>();
            self_result.text = array[2];
            UILabel self_total = obj.transform.Find("lb_self_total").GetComponent<UILabel>();
            self_total.text = array[3];
            UILabel self_fuwu = obj.transform.Find("lb_self_fuwu").GetComponent<UILabel>();
            self_fuwu.text = array[4];

            lb = obj.GetComponent<UILabel>();
        }

        if (mMsgList.Count > 0)
        {
            float lastObjHalfHight = mMsgList[mMsgList.Count - 1].GetComponent<UILabel>().height / 2;

            float curLblHalfHight = lb.height / 2;

            float curLblHight = mMsgList[mMsgList.Count - 1].transform.localPosition.y - lastObjHalfHight - curLblHalfHight;

            obj.transform.localPosition = new Vector3(0, curLblHight);
        }
        else
        {
            obj.transform.localPosition = new Vector3(0, obj.transform.localPosition.y);
        }

        mMsgList.Add(obj);
    }

    private GameObject GetCurMsgObj(MsgType type)
    {
        GameObject obj = null;

        List<GameObject> lst = null;

        GameObject msgPrefab = null;

        switch (type)
        {
            case MsgType.Sys:
                lst = mSysMessageLst;
                msgPrefab = SysMessage;
                break;
            case MsgType.SelfBet:
                lst = mSelfBetMsgLst;
                msgPrefab = mSelfBetMsg;
                break;
            case MsgType.Result:
                lst = mResultMsgLst;
                msgPrefab = mResultMsg;
                break;
        }

        if (mMsgList.Count >= 20)
        {
            mMsgList[0].SetActive(false);
            mMsgList.RemoveAt(0);

            for (int i = 0; i < lst.Count; i++)
            {
                if (!lst[i].activeSelf)
                {
                    obj = lst[i];
                    obj.SetActive(true);
                    break;
                }
            }
        }

        if (obj == null)
        {
            obj = Instantiate(msgPrefab, mScrollViewMessage.transform);
            obj.SetActive(true);
            lst.Add(obj);
        }

        return obj;
    }

    /// <summary>
    ///退出游戏
    /// </summary>
    private void Quit()
    {
        if (QuitFunc != null)
            QuitFunc();
    }

    /// <summary>
    /// 显示更多按钮窗口
    /// </summary>
    public void ShowMoreBtnWin()
    {
        mMoreBtnWindow.SetActive(!mMoreBtnWindow.activeSelf);

        mCloseArrow.SetActive(mMoreBtnWindow.activeSelf);
        mOpenArrow.SetActive(!mCloseArrow.activeSelf);
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "more_button_win":
                mMoreBtnWindow = tf.gameObject;
                break;
            case "onlinenum":
                mOnlineNumLbl = tf.GetComponent<UILabel>();
                break;
            case "item_lb_result_message":
                mResultMsg = tf.gameObject;
                break;
            case "item_lb_sysmsg":
                SysMessage = tf.gameObject;
                break;
            case "scrollview_message":
                mScrollViewMessage = tf.GetComponent<UIScrollView>();
                break;
            case "btn_music":
                mMusicSpr = tf.GetComponent<UISprite>();
                break;
            case "off":
                mMusicStopSpr = tf.GetComponent<UISprite>();
                break;
            case "lb_music":
                mMusicLbl = tf.GetComponent<UILabel>();
                break;
            case "scrollview_zaixian":
                mScrollViewOnLine = tf.GetComponent<UIScrollView>();
                break;
            case "item_user":
                mAddRoleObj = tf.gameObject;
                break;
            case "item_lb_betmsg":
                mSelfBetMsg = tf.gameObject;
                break;
            case "open_arrow":
                mOpenArrow = tf.gameObject;
                break;
            case "close_arrow":
                mCloseArrow = tf.gameObject;
                break;
        }
    }
}
