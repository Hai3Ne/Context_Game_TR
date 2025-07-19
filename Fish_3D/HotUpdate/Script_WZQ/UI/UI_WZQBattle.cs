using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WZQBattle : UILayer {
    private class PieceInfo {
        public byte x;
        public byte y;
        public byte color;
        public GameObject obj;
    }
    public static UI_WZQBattle ui;

    public GameObject mItemPiece;//棋子
    public GameObject mObjLastPieceTick;//上一步棋子标识
    public UILabel mLbPassworld;//房间密码
    public Item_WZQBattle_PlayerInfo[] mItemPlayer = new Item_WZQBattle_PlayerInfo[WZQGameConfig.MaxSeat];
    public GameObject mBtnHuiQi;//悔棋
    public GameObject mBtnHeQi;//和棋
    public GameObject mBtnRenSu;//认输
    //public GameObject mBtnLiKai;//离开
    public GameObject mBtnZhunBei;//准备
    public GameObject mBtnQuXiaoZhunBei;//取消准备
    //public GameObject mBtnStop;//结束游戏
    public UISprite mSprQuXiaoZhunBei;
    public UISprite mSprHuiQi;
    public UILabel mLbHuiQi;
    public UISprite mSprRenShu;
    public UILabel mLbRenShu;
    public Item_WZQBattle_Message item_msg_list;//消息列表
    public UITexture mTextureTable;//桌面
    public GameObject mMenuArrow;
    public GameObject mMenu;
    public GameObject mBtnChangePwd;
    public UISlider mSliderMusic;
    public UISlider mSliderSound;

    private List<PieceInfo> mPieceList = new List<PieceInfo>();//已经下的旗子

    private GameState mGameState;

    public void InitData() {
        this.SetGameState(GameManager.CurGameState);
        if (WZQTableManager.FristHandSeat == RoleManager.Self.ChairSeat) {
            this.SetPlayer(RoleManager.Self, 1);//黑子
            this.SetPlayer(WZQTableManager.mOtherRole, 2);
        } else {
            this.SetPlayer(RoleManager.Self, 2);//白字
            this.SetPlayer(WZQTableManager.mOtherRole, 1);
        }
    }

    public void UpdateColor() {//更新黑白子位置  每局结束后会切换
        bool is_frist = false;//自己是否黑子
        if (WZQTableManager.FristHandSeat == RoleManager.Self.ChairSeat) {
            is_frist = true;
        }
        for (int i = 0; i < this.mItemPlayer.Length; i++) {
            if (this.mItemPlayer[i].mRoleInfo == RoleManager.Self ^ is_frist) {
                this.mItemPlayer[i].SetColor(1);
            } else {
                this.mItemPlayer[i].SetColor(2);
            }
        }
    }

    public void SetPlayer(RoleInfo role, byte color) {//设置用户信息
        if (role == null) {
            if (RoleManager.Self.ChairSeat == 0) {
                this.mItemPlayer[1].InitData(null, color);
            } else {
                this.mItemPlayer[0].InitData(null, color);
            }
        } else {
            this.mItemPlayer[role.ChairSeat].InitData(role, color);
        }
    }

    public void SetGameState(GameState game_state)
    {
        mGameState = game_state;
        //设置游戏状态
        if (game_state == GameState.GAME_STATUS_PLAY) {//游戏中
            //this.mBtnStop.SetActive(true);
            this.mBtnHuiQi.SetActive(true);
            this.mBtnHeQi.SetActive(true);
            this.mBtnRenSu.SetActive(true);
            //this.mBtnLiKai.SetActive(false);
            this.RefershUserState(EnumUserStats.US_PLAYING);
            this.ClearPiece();
            this.RenShuGray(true);
            this.HuiQiGray(true);
        } else {
            //this.mBtnStop.SetActive(false);
            this.mBtnHuiQi.SetActive(false);
            this.mBtnHeQi.SetActive(false);
            this.mBtnRenSu.SetActive(false);
            //this.mBtnLiKai.SetActive(true);
            this.RefershUserState(RoleManager.Self.UserStatus);
        }
    }

    public void RefershUserState(EnumUserStats state) {//刷新用户自身状态
        if (state == EnumUserStats.US_READY) {//准备中
            this.mBtnZhunBei.SetActive(false);
            this.mBtnQuXiaoZhunBei.SetActive(true);
        } else if (state == EnumUserStats.US_SIT) {//坐下状态
            this.mBtnZhunBei.SetActive(true);
            this.mBtnQuXiaoZhunBei.SetActive(false);
        } else {
            this.mBtnZhunBei.SetActive(false);
            this.mBtnQuXiaoZhunBei.SetActive(false);
        }
    }
    public void AddPiece(byte x, byte y, byte color) {//1.黑子  2.白字
        GameObject obj = GameObject.Instantiate(this.mItemPiece, this.mTextureTable.transform);
        obj.SetActive(true);
        Transform tf = obj.transform;
        tf.localScale = Vector3.one;
        tf.localPosition = new Vector3((x - 7f) * WZQGameConfig.PieceWidth, (7f - y) * WZQGameConfig.PieceWidth + 6);
        UISprite spr_piece = obj.GetComponent<UISprite>();
        if (color == 1) {//黑子
            spr_piece.spriteName = "blackzi-min";
        } else {
            spr_piece.spriteName = "whitezi-min";
        }
        this.mPieceList.Add(new PieceInfo {
            x = x,
            y = y,
            color = color,
            obj = obj,
        });

        this.mObjLastPieceTick.SetActive(true);//下棋标识
        this.mObjLastPieceTick.transform.localPosition = tf.localPosition;

        if (this.mPieceList.Count >= 2) {
            this.HuiQiGray(false);
        } else {
            if (color == 1) {//黑子
                if (WZQTableManager.FristHandSeat == RoleManager.Self.ChairSeat) {
                    this.HuiQiGray(false);
                } else {
                    this.HuiQiGray(true);
                }
            } else {
                if (WZQTableManager.FristHandSeat == RoleManager.Self.ChairSeat) {
                    this.HuiQiGray(true);
                } else {
                    this.HuiQiGray(false);
                }
            }
        }
    }
    public void RemoveLast() {//移除最后一步旗子
        if (this.mPieceList.Count > 0) {
            int index = this.mPieceList.Count-1;
            PieceInfo info = this.mPieceList[index];
            this.mPieceList.RemoveAt(index);
            GameObject.Destroy(info.obj);
        }
        if (this.mPieceList.Count > 0) {
            this.mObjLastPieceTick.SetActive(true);//下棋标识
            this.mObjLastPieceTick.transform.localPosition = this.mPieceList[this.mPieceList.Count-1].obj.transform.localPosition;
        } else {
            this.mObjLastPieceTick.SetActive(false);//下棋标识
        }

        if (this.mPieceList.Count >= 2) {
            this.HuiQiGray(false);
        } else if (this.mPieceList.Count == 1) {
            if (this.mPieceList[0].color == 1) {//黑子
                if (WZQTableManager.FristHandSeat == RoleManager.Self.ChairSeat) {
                    this.HuiQiGray(false);
                } else {
                    this.HuiQiGray(true);
                }
            } else {
                if (WZQTableManager.FristHandSeat == RoleManager.Self.ChairSeat) {
                    this.HuiQiGray(true);
                } else {
                    this.HuiQiGray(false);
                }
            }
        } else {
            this.HuiQiGray(true);
        }
    }
    public void ClearPiece() {//清除棋盘
        foreach (var item in this.mPieceList) {
            GameObject.Destroy(item.obj);
        }
        this.mPieceList.Clear();
        this.mObjLastPieceTick.SetActive(false);
    }
    public void SetChessManual(CMD_S_CHESS_MANUAL cmd) {//设置棋谱信息
        this.ClearPiece();
        foreach (var item in cmd.ChessManual) {
            this.AddPiece(item.XPos, item.YPos, item.Color);
        }
    }
    public void PlaceChess(CMD_S_PlaceChess cmd) {//放置棋子
        if (cmd.PlaceUser == WZQTableManager.FristHandSeat) {
            this.AddPiece(cmd.XPos, cmd.YPos, 1);
        } else {
            this.AddPiece(cmd.XPos, cmd.YPos, 2);
        }
        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_Chess);
    }
    public void RegretResult(CMD_S_RegretResult cmd) {//悔棋结果
        SystemMessageMgr.Instance.ShowMessageBox("对方同意悔棋");
        for (int i = 0; i < cmd.RegretCount; i++) {
            this.RemoveLast();
        }
        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_Agree);
    }
    public void RegretFaile(CMD_S_RegretFaile cmd) {//悔棋失败
        switch (cmd.FaileReason) {
            case 1://次数限制
                SystemMessageMgr.Instance.ShowMessageBox("当局悔棋次数已达上限");
                break;
            case 2://玩家拒绝
                SystemMessageMgr.Instance.ShowMessageBox("对方拒绝了你的悔棋请求");
                break;
            case 3://主动取消
                UI.ExitUI<UI_WZQ_Dialog>();
                break;
        }
        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_Reject);
    }
    public void HandleUserScore(SC_GR_UserScore cmd) {//用户分数更新
        for (int i = 0; i < this.mItemPlayer.Length; i++) {
            if (this.mItemPlayer[i].mRoleInfo != null && this.mItemPlayer[i].mRoleInfo.UserID == cmd.UserID) {
                this.mItemPlayer[i].SetGold(cmd.UserScroe.Scroe);
            }
        }
    }
    public void HuiQiGray(bool is_gray) {
        if (is_gray) {
            this.mSprHuiQi.IsGray = true;
            this.mBtnHuiQi.GetComponent<BoxCollider>().enabled = false;
            this.mLbHuiQi.effectStyle = UILabel.Effect.None;
            this.mLbHuiQi.color = Color.gray;
            this.mLbHuiQi.applyGradient = false;
        } else {
            this.mSprHuiQi.IsGray = false;
            this.mBtnHuiQi.GetComponent<BoxCollider>().enabled = true;
            this.mLbHuiQi.effectStyle = UILabel.Effect.Outline8;
            this.mLbHuiQi.color = Color.white;
            this.mLbHuiQi.applyGradient = true;
        }
    }
    public void RenShuGray(bool is_gray) {
        if (is_gray) {
            this.mSprRenShu.IsGray = true;
            this.mBtnRenSu.GetComponent<BoxCollider>().enabled = false;
            this.mLbRenShu.effectStyle = UILabel.Effect.None;
            this.mLbRenShu.color = Color.gray;
            this.mLbRenShu.applyGradient = false;
        } else {
            this.mSprRenShu.IsGray = false;
            this.mBtnRenSu.GetComponent<BoxCollider>().enabled = true;
            this.mLbRenShu.effectStyle = UILabel.Effect.Outline8;
            this.mLbRenShu.color = Color.white;
            this.mLbRenShu.applyGradient = true;
        }
    }
    private void RegretReq()
    {
        //收到悔棋请求
        UI.EnterUI<UI_WZQ_Dialog>(GameEnum.WZQ).InitData(2, AngreeRegret, DontAngreeRegret);

        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_Request);
    }


    private void AngreeRegret()
    {
        NetClient.Send(NetCmdType.SUB_C_REGRET_ANSWER, new CMD_C_RegretAnswer
        {
            Approve = 1,//同意悔棋
        });
    }

    private void DontAngreeRegret()
    {
        NetClient.Send(NetCmdType.SUB_C_REGRET_ANSWER, new CMD_C_RegretAnswer
        {
            Approve = 0,//拒绝悔棋
        });
    }

    private void AngreePeace()
    {
        NetClient.Send(NetCmdType.SUB_C_PEACE_ANSWER, new CMD_C_PeaceAnswer
        {
            Approve = 1,//同意和棋
        });
    }

    private void DontAngreePeace()
    {
        NetClient.Send(NetCmdType.SUB_C_PEACE_ANSWER, new CMD_C_PeaceAnswer
        {
            Approve = 0,//拒绝和棋
        });
    }

    private void PeaceReq()
    {
        //收到和棋请求
        UI.EnterUI<UI_WZQ_Dialog>(GameEnum.WZQ).InitData(1, AngreePeace, DontAngreePeace);

        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_Request);
    }

    private void GameResult(CMD_S_GameEnd game_end) {//游戏结算
        //Debug.LogError(LitJson.JsonMapper.ToJson(game_end));
        this.SetGameState(GameState.GAME_STATUS_FREE);
        //ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<GameObject>(ResPath.UIPath + "Ef_wzq");
        //ResManager.LoadAsset<GameObject>(data, (prefab) =>
        //{
        //    GameObject obj = GameUtils.CreateGo(prefab, SceneObjMgr.Instance.UIPanelTransform);
        //    obj.AddComponent<ResCount>().ab_data = data;
        //    Animator anim = obj.GetComponent<Animator>();
        //    float time = anim.GetCurrentAnimatorStateInfo(0).length;
        //    GameObject.Destroy(obj, time);


        //    RoleInfo[] roles = new RoleInfo[2];
        //    roles[RoleManager.Self.ChairSeat] = RoleManager.Self;
        //    roles[WZQTableManager.mOtherRole.ChairSeat] = WZQTableManager.mOtherRole;
        //    TimeManager.DelayExec(time, () => 
        //    {
        //        UI.EnterUI<UI_WZQ_Result>(GameEnum.WZQ).InitData(game_end, roles);
        //    });
        //    UILabel lb_result = obj.transform.Find("result").GetComponent<UILabel>();
        //    if (game_end.WinUser == WZQTableManager.FristHandSeat) {
        //        lb_result.text = "黑子胜";
        //    } else if (game_end.WinUser == ushort.MaxValue) {
        //        lb_result.text = "平局";
        //    } else {
        //        lb_result.text = "白子胜";
        //    }
        //});

        ResManager.LoadAsset<GameObject>(ResPath.NewUIPath +  "Ef_wzq", (abinfo, asset) =>
        {
            GameObject obj = GameUtils.CreateGo(asset, SceneObjMgr.Instance.UIPanelTransform);
            obj.AddComponent<ResCount>().ab_info = abinfo;
            Animator anim = obj.GetComponent<Animator>();
            float time = anim.GetCurrentAnimatorStateInfo(0).length;
            GameObject.Destroy(obj, time);

            RoleInfo[] roles = new RoleInfo[2];
            roles[RoleManager.Self.ChairSeat] = RoleManager.Self;
            roles[WZQTableManager.mOtherRole.ChairSeat] = WZQTableManager.mOtherRole;
            TimeManager.DelayExec(time, () =>
            {
                UI.EnterUI<UI_WZQ_Result>(GameEnum.WZQ).InitData(game_end, roles);
            });
            UILabel lb_result = obj.transform.Find("result").GetComponent<UILabel>();
            if (game_end.WinUser == WZQTableManager.FristHandSeat)
            {
                lb_result.text = "黑子胜";
            }
            else if (game_end.WinUser == ushort.MaxValue)
            {
                lb_result.text = "平局";
            }
            else
            {
                lb_result.text = "白子胜";
            }
        }, GameEnum.WZQ);

        if (game_end.WinUser == RoleManager.Self.ChairSeat) {//赢
            AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_GameWin);
        } else if (game_end.WinUser == ushort.MaxValue) {//和棋
            AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_GameLose);
        } else {//输
            AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_GameTie);
        }
        UI.ExitUI<UI_WZQ_Wait>();
    }
    private float _pre_update_time;//上次更新棋谱时间
    public void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_S_TABLE_PASS://更新密码
                this.mLbPassworld.text = pack.ToObj<CMD_S_Pass>().PassID;
                this.mLbPassworld.gameObject.SetActive(true);
                this.mBtnChangePwd.SetActive(true);
                this._pre_update_time = Time.realtimeSinceStartup;//30S CD
                break;
            case NetCmdType.SUB_S_CHESS_MANUAL://棋谱信息
                this.SetChessManual(pack.ToObj<CMD_S_CHESS_MANUAL>());
                break;
            case NetCmdType.SUB_S_PLACE_CHESS://放置旗子
                this.PlaceChess(pack.ToObj<CMD_S_PlaceChess>());
                break;
            case NetCmdType.SUB_GF_GAME_STATE://游戏状态更新
                this.SetGameState((GameState)pack.ToObj<SC_GF_GameStatus>().GameStatus);
                break;
            case NetCmdType.SUB_S_GAME_START://游戏开始
                this.SetGameState(GameState.GAME_STATUS_PLAY);
                this.UpdateColor();
                AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_Start);
                break;
            case NetCmdType.SUB_S_GAME_END://游戏结束
                this.GameResult(pack.ToObj<CMD_S_GameEnd>());
                break;
            case NetCmdType.SUB_GR_USER_SCORE://分数更新
                this.HandleUserScore(pack.ToObj<SC_GR_UserScore>());
                break;
            case NetCmdType.SUB_S_REGRET_RESULT://悔棋成功
                this.RegretResult(pack.ToObj<CMD_S_RegretResult>());
                break;
            case NetCmdType.SUB_S_REGRET_FAILE://悔棋失败
                this.RegretFaile(pack.ToObj<CMD_S_RegretFaile>());
                break;
            case NetCmdType.SUB_S_PEACE_ANSWER://和棋失败
                SystemMessageMgr.Instance.ShowMessageBox("和棋失败");
                AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_Reject);
                break;
            case NetCmdType.SUB_S_REGRET_REQ://悔棋请求
                this.RegretReq();
                break;
            case NetCmdType.SUB_S_PEACE_REQ://和棋请求
                this.PeaceReq();
                break;
            case NetCmdType.SUB_S_GIVEUP_FLAG://认输按钮点亮
                this.RenShuGray(false);
                break;
            case NetCmdType.SUB_GF_GAME_SCENE_PLAY://游戏数据同步
            case NetCmdType.SUB_S_BLACK_TRADE://交换对家
                this.UpdateColor();
                break;
        }
    }
    private void OnUserInfoChange(RoleInfo role) {
        if(role.TableID == RoleManager.Self.TableID && role.ChairSeat < this.mItemPlayer.Length){//同桌玩家
            this.mItemPlayer[role.ChairSeat].SetRole(role);
            if (RoleManager.Self == role) {
                this.RefershUserState(role.UserStatus);
            } 
        }
    }
    private void OnUserLeave(RoleInfo role) {
        if (RoleManager.Self == role) {
            this.Close();
            UI.EnterUI<UI_wzqmain>(GameEnum.WZQ).RefreshInfo(1);

        } else if (role.TableID == WZQTableManager.mCurTable && role.ChairSeat < this.mItemPlayer.Length) {//同桌玩家
            this.mItemPlayer[role.ChairSeat].SetRole(null);
        }
    }
    private void OnBegState(ushort[] states) {
        for (int i = 0; i < states.Length; i++) {
            if ((states[i] & 1) > 0) {//1:等待求和
                if (RoleManager.Self.ChairSeat == i)
                {
                    UI.EnterUI<UI_WZQ_Wait>(GameEnum.WZQ).InitData(2, 20);
                }
                else
                {
                    this.PeaceReq();
                }
            } 
            if ((states[i] & 2) > 0) {//2:等待悔棋
                if (RoleManager.Self.ChairSeat == i)
                {
                    UI.EnterUI<UI_WZQ_Wait>(GameEnum.WZQ).InitData(1, 20);
                }
                else
                {
                    this.RegretReq();
                }
            }
        }
    }
    private void OnGameEvent(GameEvent type, object obj) {
        switch (type) {
            case GameEvent.UserEneter:
            case GameEvent.UserStateChange:
            case GameEvent.UserInfoChange:
                this.OnUserInfoChange(obj as RoleInfo);
                break;
            case GameEvent.UserLeaveTable:
                this.OnUserLeave(obj as RoleInfo);
                break;
            case GameEvent.BegStatus:
                this.OnBegState(obj as ushort[]);
                break;
        }
    }
    public override void OnNodeLoad() {
        this.mSprQuXiaoZhunBei.IsGray = true;
        UI_WZQBattle.ui = this;
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_TABLE_PASS, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_CHESS_MANUAL, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GF_GAME_STATE, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PLACE_CHESS, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_START, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_END, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_USER_SCORE, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_REGRET_RESULT, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_REGRET_FAILE, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PEACE_ANSWER, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_REGRET_REQ, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PEACE_REQ, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GIVEUP_FLAG, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GF_GAME_SCENE_PLAY, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_BLACK_TRADE, this.OnNetHandle);

        EventManager.RegisterEvent(GameEvent.UserEneter, this.OnGameEvent);
        EventManager.RegisterEvent(GameEvent.UserStateChange, this.OnGameEvent);
        EventManager.RegisterEvent(GameEvent.UserLeaveTable, this.OnGameEvent);
        EventManager.RegisterEvent(GameEvent.BegStatus, this.OnGameEvent);
        EventManager.RegisterEvent(GameEvent.UserInfoChange, this.OnGameEvent);
        TimeManager.DelayExec(this, UI.AnimTime, () => {
            AudioManager.PlayMusic(GameEnum.WZQ, WZQGameConfig.Audio_BGs[Random.Range(0, WZQGameConfig.Audio_BGs.Length)]);
        });
    }
    public override void OnEnter() { }
    public override void OnExit() {
        UI_WZQBattle.ui = null;
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_TABLE_PASS, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_CHESS_MANUAL, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GF_GAME_STATE, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PLACE_CHESS, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_START, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_END, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_USER_SCORE, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_REGRET_RESULT, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_REGRET_FAILE, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PEACE_ANSWER, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_REGRET_REQ, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PEACE_REQ, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GIVEUP_FLAG, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GF_GAME_SCENE_PLAY, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_BLACK_TRADE, this.OnNetHandle);

        EventManager.UnRegisterEvent(GameEvent.UserEneter, this.OnGameEvent);
        EventManager.UnRegisterEvent(GameEvent.UserStateChange, this.OnGameEvent);
        EventManager.UnRegisterEvent(GameEvent.UserLeaveTable, this.OnGameEvent);
        EventManager.UnRegisterEvent(GameEvent.BegStatus, this.OnGameEvent);
        EventManager.UnRegisterEvent(GameEvent.UserInfoChange, this.OnGameEvent);

        AudioManager.StopMusic();
    }
    private float __place_chess_time;
    private void OnClickTable() {//点击棋面
        if (GameManager.CurGameState == GameState.GAME_STATUS_PLAY && WZQTableManager.mCurRoleSeat == RoleManager.Self.ChairSeat) {
            if (__place_chess_time + 0.5f > Time.realtimeSinceStartup) {//防止重点
                return;
            }
            Vector3 world_pos = UICamera.mainCamera.ScreenToWorldPoint(UICamera.currentTouch.pos);
            Vector3 local_pos = this.mTextureTable.transform.InverseTransformPoint(world_pos);

            byte x = (byte)Mathf.Clamp(Mathf.FloorToInt(local_pos.x / WZQGameConfig.PieceWidth + 7.5f), 0, 14);
            byte y = (byte)Mathf.Clamp(14 - Mathf.FloorToInt((local_pos.y - 6) / WZQGameConfig.PieceWidth + 7.5f), 0, 14);
            foreach (var item in this.mPieceList) {
                if (item.x == x && item.y == y) {
                    //Debug.LogError("重复了");
                    return;
                }
            }
            //放置棋子
            NetClient.Send(NetCmdType.SUB_C_PLACE_CHESS, new CMD_C_PlaceChess {
                XPos = x,
                YPos = y,
            });
            __place_chess_time = Time.realtimeSinceStartup;
        }
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "table_bg"://棋面
                this.OnClickTable();
                break;
            case "btn_menu":
                mMenu.SetActive(!mMenu.activeSelf);
                if (mMenu.activeSelf)
                {
                    mMenuArrow.transform.eulerAngles = Vector3.zero;
                }
                else
                {
                    Vector3 agnel = new Vector3(0, 0, 180);
                    mMenuArrow.transform.eulerAngles = agnel;
                }
                break;
            case "btn_xgmm"://更新密码
                NetClient.Send(NetCmdType.SUB_C_UPDATE_PASS, new CMD_C_UPDATE_PASS());
                break;
            case "btn_huiqi"://悔棋
                if (this.mPieceList.Count > 0) {
                    //UI.EnterUI<UI_WZQ_Dialog>((ui) => {
                    //    ui.InitData(4, () => {
                    NetClient.Send(NetCmdType.SUB_C_REGRET_REQ, new CMD_C_REGRET_REQ());
                    UI.EnterUI<UI_WZQ_Wait>(GameEnum.WZQ).InitData(1, 20);
                        //}, null);
                    //});
                }
                break;
            case "btn_heqi"://和棋
                UI.EnterUI<UI_WZQ_Dialog>(GameEnum.WZQ).InitData(5,
                () =>
                {
                    NetClient.Send(NetCmdType.SUB_C_PEACE_REQ, new CMD_C_PEACE_REQ());
                    UI.EnterUI<UI_WZQ_Wait>(GameEnum.WZQ).InitData(2, 20);
                }, null);
                break;
            case "btn_rensu"://认输
                UI.EnterUI<UI_WZQ_Dialog>(GameEnum.WZQ).InitData(3, () => 
                {
                    NetClient.Send(NetCmdType.SUB_C_GIVEUP_REQ, new CMD_C_IsGiveup
                    {
                        Giveup = 1,//1.认输
                    });
                }, null);
                break;
            case "btn_zhunbei"://准备
                if (RoleManager.Self.GoldNum < 10000) {
                    SystemMessageMgr.Instance.ShowMessageBox("乐豆低于10000乐豆，无法进行游戏");
                } else {
                    NetClient.Send(NetCmdType.SUB_GF_USER_READY, new CS_GF_UserReady());
                }
                break;
            case "btn_quxiaozhunbei"://取消准备
                NetClient.Send(NetCmdType.SUB_GF_Cancel_READY, new CS_GF_CancelReady());
                break;
            case "btn_safebox"://保险箱
                //UI.EnterUI<UI_safebox_new>(ui => {
                //    ui.InitData();
                //});
                UI.EnterUI<UI_safebox_new>(GameEnum.All).InitData();
                break;
            case "btn_setting":
                UI.EnterUI<UI_WZQ_Setting>(GameEnum.WZQ).InitData();
                break;
            case "btn_exit":
                if (mGameState == GameState.GAME_STATUS_PLAY)
                {
                    NetClient.Send(NetCmdType.SUB_C_END_GAME, new CMD_C_END_GAME());
                }
                else
                {
                    NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp
                    {
                        TableID = RoleManager.Self.TableID,
                        ChairID = RoleManager.Self.ChairSeat,
                        ForceLeave = 0,//0.正常离开  1.强制离开
                    });
                }
                break;
            case "btn_help":
                UI.EnterUI<UI_WZQ_Help>(GameEnum.WZQ);
                break;

            default:
                return;
        }

        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_ClickBtn);
    }

    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "table_bg":
                this.mTextureTable = tf.GetComponent<UITexture>();
                break;
            case "item_qizi":
                this.mItemPiece = tf.gameObject;
                this.mItemPiece.SetActive(false);
                break;
            case "item_lastmovetig":
                this.mObjLastPieceTick = tf.gameObject;
                this.mObjLastPieceTick.SetActive(false);
                break;
            case "lb_passworld":
                this.mLbPassworld = tf.GetComponent<UILabel>();
                tf.gameObject.SetActive(false);
                break;
            case "btn_xgmm":
                this.mBtnChangePwd = tf.gameObject;
                this.mBtnChangePwd.SetActive(false);
                break;
            case "item_player_1":
                this.mItemPlayer[0] = this.BindItem<Item_WZQBattle_PlayerInfo>(tf.gameObject);
                this.mItemPlayer[0].SetSeat(0);
                break;
            case "item_player_2":
                this.mItemPlayer[1] = this.BindItem<Item_WZQBattle_PlayerInfo>(tf.gameObject);
                this.mItemPlayer[1].SetSeat(1);
                break;
            case "btn_stop":
                //this.mBtnStop = tf.gameObject;
                break;
            case "btn_huiqi":
                this.mBtnHuiQi = tf.gameObject;
                this.mSprHuiQi = tf.GetComponent<UISprite>();
                break;
            case "lb_huiqi":
                this.mLbHuiQi = tf.GetComponent<UILabel>();
                break;
            case "btn_heqi":
                this.mBtnHeQi = tf.gameObject;
                break;
            case "btn_rensu":
                this.mBtnRenSu = tf.gameObject;
                this.mSprRenShu = tf.GetComponent<UISprite>();
                break;
            case "lb_renshu":
                this.mLbRenShu = tf.GetComponent<UILabel>();
                break;
            case "btn_likai":
                //this.mBtnLiKai = tf.gameObject;
                break;
            case "btn_zhunbei":
                this.mBtnZhunBei = tf.gameObject;
                break;
            case "btn_quxiaozhunbei":
                this.mBtnQuXiaoZhunBei = tf.gameObject;
                this.mSprQuXiaoZhunBei = tf.GetComponent<UISprite>();
                break;
            case "item_msg_list":
                this.item_msg_list = this.BindItem<Item_WZQBattle_Message>(tf.gameObject);
                break;
            case "menu_arrow":
                mMenuArrow = tf.gameObject;
                break;
            case "menu":
                mMenu = tf.gameObject;
                break;
            case "music":
                mSliderMusic = tf.GetComponent<UISlider>();
                break;
            case "sound":
                mSliderSound = tf.GetComponent<UISlider>();
                break;
        }
    }
}
