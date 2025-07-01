using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FQZSchooseroom : UILayer
{
    /// <summary>
    /// 玩家头像
    /// </summary>
    public UITexture mHead;

    /// <summary>
    /// 玩家昵称
    /// </summary>
    public UILabel mNickName;

    /// <summary>
    /// 玩家id
    /// </summary>
    public UILabel mPlayerId;

    /// <summary>
    /// 玩家拥有的乐豆
    /// </summary>
    public UILabel mLeDou;

    /// <summary>
    /// 房间实例
    /// </summary>
    public GameObject mItemRoom;

    /// <summary>
    /// 房间滑动区域
    /// </summary>
    public UIScrollView mScrollView;

    /// <summary>
    /// vip
    /// </summary>
    public UISprite mVip;

    private int ItemWidth = 436;

    /// <summary>
    /// 服务器列表
    /// </summary>
    public List<tagGameServer> mServerList = new List<tagGameServer>();

    /// <summary>
    /// 初始化数据
    /// </summary>
    public void InitData()
    {
        UpdateUserInfo();
        mServerList = HallHandle.GetServerList(FQZSGameConfig.KindID);

        Item_FQZS_ChooseRoom item;

        float start_x = -ItemWidth * Mathf.Min(3, mServerList .Count- 1) * 0.5f;

        for (int i = 0; i < mServerList.Count; i++)
        {
            item = AddItem<Item_FQZS_ChooseRoom>(mItemRoom, mScrollView.transform);
            item.gameObject.SetActive(true);
            item.SetUIInfo(mServerList[i]);
            item.transform.localPosition = new Vector3(start_x + ItemWidth * i, 0);
        }

        if (mServerList.Count > 4)
        {
            mScrollView.enabled = true;
            mScrollView.ResetPosition();
        }
        else
        {
            mScrollView.enabled = false;
        }
    }

    public override void OnNodeLoad()
    {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, OnNetHandle);

        EventManager.RegisterEvent(GameEvent.Hall_UserInfoChange, OnEventHandle);
        TimeManager.DelayExec(this, UI.AnimTime, () => {
            AudioManager.PlayMusic(GameEnum.All, FishConfig.Instance.AudioConf.datingBgm.ToString());
        });
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_back":
                Close();
                GameSceneManager.BackToHall(GameEnum.None);
                break;
        }
    }

    private void OnEventHandle(GameEvent event_type, object obj)
    {
        switch (event_type)
        {
            case GameEvent.Hall_UserInfoChange:
                UpdateUserInfo();
                break;
        }
    }

    public override void OnExit()
    {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);

        EventManager.UnRegisterEvent(GameEvent.Hall_UserInfoChange, OnEventHandle);
    }

    private void OnNetHandle(NetCmdType type, NetCmdPack pack)
    {
        switch (type)
        {
            case NetCmdType.SUB_GR_PROPERTY_EFFECT://VIP通知
                UpdateUserInfo();
                break;
            case NetCmdType.SUB_GP_USER_FACE_INFO://更改头像
                mHead.uvRect = GameUtils.FaceUVRect(HallHandle.FaceID);
                break;
        }
    }

    public void UpdateUserInfo()
    {
        mNickName.text = HallHandle.NickName;
        mPlayerId.text = string.Format("ID:{0}", HallHandle.GameID);
        mLeDou.text = HallHandle.UserGold.ToString();
        mHead.uvRect = GameUtils.FaceUVRect(HallHandle.FaceID);
        mVip.gameObject.SetActive(HallHandle.MemberOrder > 0);
        mVip.spriteName = string.Format("vip_{0}", HallHandle.MemberOrder);
        mVip.MakePixelPerfect();
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "texture_player":
                mHead = tf.GetComponent<UITexture>();
                break;
            case "lb_name":
                mNickName = tf.GetComponent<UILabel>();
                break;
            case "lb_game_id":
                mPlayerId = tf.GetComponent<UILabel>();
                break;
            case "lb_gold":
                mLeDou = tf.GetComponent<UILabel>();
                break;
            case "item_btn_room":
                mItemRoom = tf.gameObject;
                mItemRoom.SetActive(false);
                break;
            case "scrollview":
                mScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "spr_vip":
                mVip = tf.GetComponent<UISprite>();
                break;
        }
    }
}
