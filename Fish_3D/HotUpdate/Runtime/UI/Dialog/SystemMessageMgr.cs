using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum SysMessageType
{
	SMT_CHAT	= 0x0001, //聊天消息
	SMT_EJECT	= 0x0002, //弹出消息

	SMT_GLOBAL	= 0x0004,//全局消息

	SMT_PROMPT	= 0x0008,//提示消息

	SMT_TABLE_ROLL	= 0x0010,//滚动消息

	SMT_CLOSE_ROOM	=			0x0100,								//关闭房间
	SMT_CLOSE_GAME	=			0x0200,								//关闭游戏-离开当前桌子
	SMT_CLOSE_LINK	=			0x0400,								//中断连接
	SMT_AUTOCLOSE_BOX=			0x0800,								//自动关闭
}


public struct SystMsg{
	public string message;
	public ushort msgTypeId;
	public SystMsg(ushort mid, string mstr)
	{
		this.msgTypeId = mid;
		this.message = mstr;
	}
}


public class SystemMessageMgr : MonoSingleTon<SystemMessageMgr> {
	
	public void ShowMessageBox(string msg, float life = 0f)
	{
		WndManager.Instance.GetController<MessageBoxUIController>().PushMsgMove(msg);
	}

    public void ShowMessageBox(string msg,Vector3 pos)
    {
        WndManager.Instance.GetController<MessageBoxUIController>().PushMsgStatic(msg, pos, 1.5f);
    }

    public void DisposeMessageBox()
    {
        WndManager.Instance.GetController<MessageBoxUIController>().Dispose();
    }

	public void DialogShow(string langeKey, System.Action confirm, System.Action cancel)
	{
		string msgInfo = StringTable.GetString (langeKey);
		if (msgInfo!= null)
			HandlePormptMsg(msgInfo,confirm, cancel);
	}

	public void DialogShow(string msgInfo, float lifeTime = 0f)
	{
        this.DialogShow(msgInfo, null);
        //msgInfo = StringTable.GetString (msgInfo);
        //HandlePormptMsg(new SystMsg(0, msgInfo));
	}


    System.Action mOkfun;
    public void DialogShowByText(string msgInfo, System.Action okfun) {
        mOkfun = okfun;
        var ctrl = WndManager.Instance.ShowUI(EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay(msgInfo), true);
        ctrl.OnCloseEvent += handleDialogClose;
    }
	public void DialogShow(string msgInfo, System.Action okfun){
		mOkfun = okfun;
		msgInfo = StringTable.GetString (msgInfo);
		var ctrl = WndManager.Instance.ShowUI (EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay(msgInfo), true);
		ctrl.OnCloseEvent += handleDialogClose;
	}

	void handleDialogClose(IUIController ctrl){
		ctrl.OnCloseEvent -= handleDialogClose;
		mOkfun.TryCall();
		mOkfun = null;
	}

	public void HandleErrorCode(SC_GR_ErrorCode errorCode)
	{
		ErrorCodeVo errCodevo = FishConfig.Instance.ErrorCodeConf.TryGet(errorCode.ErrorID);
		if (errCodevo != null) {
			string infos = string.Format(errCodevo.ErrorDescCN, errorCode.Param1, errorCode.Param2, errorCode.Param3, errorCode.Param4);
            //HandlePormptMsg (new SystMsg(0, infos));
            SystemMessageMgr.Instance.ShowMessageBox(infos);
		}
	}

    public void HandleSysMessage(SC_GR_GF_SystemMessage sysMsg) {
        if (sysMsg != null) {
            HandleMessageByType(sysMsg.Type, Tools.MessageColor(sysMsg.Message));
        }
    }
    public void HandleSysMessage(SC_GR_CM_SystemMessage cmSysmsg) {
        if (cmSysmsg != null) {
            HandleMessageByType(cmSysmsg.Type, Tools.MessageColor(cmSysmsg.Message));
        }
    }

    List<SystMsg> promptMessageQueue = new List<SystMsg>();
	Queue<ClsTuple<string, System.Action, System.Action>> promptMessageCbQueue = new Queue<ClsTuple<string, System.Action, System.Action>>();

	IUIController promptCtrl = null;
	void HandleMessageByType(ushort MsgType, string msgContent)
	{
        if ((MsgType & (ushort)SysMessageType.SMT_TABLE_ROLL) != 0 || (MsgType & (ushort)SysMessageType.SMT_PROMPT) != 0 || (MsgType & (ushort)SysMessageType.SMT_CHAT) != 0) {
			WndManager.Instance.GetController<ScrollingMessageUIController>().PushRollMsg(msgContent);
		}
		else if ((MsgType & (ushort)SysMessageType.SMT_EJECT) != 0) {
			HandlePormptMsg (new SystMsg(MsgType, msgContent));
		}
	}

	public void HandlePormptMsg(SystMsg msgvo)
	{
		if (promptCtrl == null || promptCtrl.IsActive == false) {
			promptCtrl = WndManager.Instance.ShowUI (EnumUI.PromptMsgUI, msgvo.message);
			if (promptCtrl != null) {
				promptCtrl.BindData = msgvo.msgTypeId;
				promptCtrl.OnCloseEvent += OnClosePrompt;
			}
		} else {
            bool is_add = true;//取出重复消息
            for (int i = 0; i < promptMessageQueue.Count; i++) {
                if (promptMessageQueue[i].message == msgvo.message && promptMessageQueue[i].msgTypeId == msgvo.msgTypeId) {
                    is_add = false;
                    break;
                }
            }
            if (is_add) {
                promptMessageQueue.Add(msgvo);
            }
		}
	}

	void OnClosePrompt(IUIController ctrl)
	{
		ctrl.OnCloseEvent -= OnClosePrompt;
		ushort msgtype = (ushort)promptCtrl.BindData;
		if (msgtype != 0) {
            if (//(msgtype & (ushort)SysMessageType.SMT_CLOSE_GAME) != 0 ||
                (msgtype & (ushort)SysMessageType.SMT_CLOSE_ROOM) != 0 ||
                (msgtype & (ushort)SysMessageType.SMT_CLOSE_LINK) != 0) {
                GameSceneManager.BackToHall(GameManager.CurGameEnum);
                //SDKMgr.ExitGame();
                return;
            }
		}
		promptCtrl = null;
		if (promptMessageQueue.Count > 0) {
			var msgv = promptMessageQueue[0];
            promptMessageQueue.RemoveAt(0);
			HandlePormptMsg (msgv);
		}
	}



	public void HandlePormptMsg(string msgContent, System.Action confirmCb, System.Action cancelCb)
	{
		if (promptCtrl == null || promptCtrl.IsActive == false) {
			promptCtrl = WndManager.Instance.ShowUI (EnumUI.PromptMsgUI, msgContent);
			promptCtrl.OnCloseEvent += OnClosePrompt2;

			PromptSysMessageController sysPropt = promptCtrl as PromptSysMessageController;
			sysPropt.onCancelCb = cancelCb;
			sysPropt.onConfirmCb = confirmCb;
		} else {
			promptMessageCbQueue.Enqueue (new ClsTuple<string, System.Action, System.Action>(msgContent, confirmCb, cancelCb));
		}
	}

	void OnClosePrompt2(IUIController ctrl)
	{
		ctrl.OnCloseEvent -= OnClosePrompt2;
		promptCtrl = null;

		if (promptMessageCbQueue.Count > 0) {
			var tuple = promptMessageCbQueue.Dequeue ();
			HandlePormptMsg (tuple.field0,tuple.field1, tuple.field2);
		}
	}

}
