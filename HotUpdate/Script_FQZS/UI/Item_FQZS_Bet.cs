using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_Bet : UIItem
{
    /// <summary>
    /// 筹码列表
    /// </summary>
    public List<Item_FQZS_ChouMa> mChouMaLst = new List<Item_FQZS_ChouMa>();

    /// <summary>
    /// 续投按钮背景图
    /// </summary>
    private UISprite mXuTou;

    /// <summary>
    /// 续投按钮文本
    /// </summary>
    private UILabel mXutouLbl;

    /// <summary>
    /// 灰化状态的筹码
    /// </summary>
    private GameObject mBlackChouMa;

    /// <summary>
    /// 筹码激活状态
    /// </summary>
    private GameObject mActiveChouMa;

    /// <summary>
    /// 是否已经续投过了
    /// </summary>
    public bool IsAreadyXuTou = false;

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_xutou":
                XuTou();
                break;
        }
    }

    /// <summary>
    /// 继续上一次的投注
    /// </summary>
    private void XuTou()
    {
        CMD_C_AgainJetton_fqzs req = new CMD_C_AgainJetton_fqzs();
        req.AreaJetton = FQZSGameManager.mXuTouArr;

        NetClient.Send(NetCmdType.SUB_C_AGAIN_JETTON_FQZS, req);

        ChangeXutouState(false);

        IsAreadyXuTou = true;
    }

    public void ChangeXutouState(bool canXutou)
    {
        if (canXutou)
        {
            if (FQZSGameManager.LeiZhuSeat != RoleManager.Self.ChairSeat)
            {
                mXuTou.IsGray = false;
                mXutouLbl.IsGray = false;
                mXuTou.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                mXuTou.IsGray = true;
                mXutouLbl.IsGray = true;
                mXuTou.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            mXuTou.IsGray = true;
            mXutouLbl.IsGray = true;
            mXuTou.GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// 检查是否能够续投
    /// </summary>
    private bool CheckCanXuTou()
    {
        long xutouLeDou = 0;

        for (int i = 0; i < FQZSGameManager.mXuTouArr.Length; i++)
        {
            xutouLeDou += FQZSGameManager.mXuTouArr[i];
        }

        if (xutouLeDou > FQZSGameManager.CurGold)
        {
            //剩余乐豆不足以续投
            return false;
        }

        if (xutouLeDou > FQZSGameManager.GetCurBetMax())
        {
            //上次押注的金额已经超过本次押注上限
            return false;
        }

        return true;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="onSelectCurChouMa"></param>
    public void InitData()
    {
        for (int i = 0; i < mChouMaLst.Count; i++)
        {
            int value = SetChouMaValue(i);
            mChouMaLst[i].InitData(value, onSelectCurChouMa);
        }
    }

    /// <summary>
    /// 设置当前选择的筹码
    /// </summary>
    /// <param name="obj"></param>
    private void onSelectCurChouMa(Item_FQZS_ChouMa obj)
    {
        if (FQZSGameManager.CurrSelectChouMa != null)
        {
            FQZSGameManager.CurrSelectChouMa.mChoose.SetActive(false);
        }
        FQZSGameManager.CurrSelectChouMa = obj;
        FQZSGameManager.CurrSelectChouMa.mChoose.SetActive(true);
    }

    /// <summary>
    /// 设置每种筹码的值
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private int SetChouMaValue(int index)
    {
        switch (index)
        {
            case 0:
                return 1000;
            case 1:
                return 10000;
            case 2:
                return 100000;
            case 3:
                return 500000;
            case 4:
                return 1000000;
            case 5:
                return 5000000;
        }

        return 0;
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "cm1":
            case "cm2":
            case "cm3":
            case "cm4":
            case "cm5":
            case "cm6":
                mChouMaLst.Add(BindItem<Item_FQZS_ChouMa>(tf.gameObject));
                break;
            case "btn_xutou":
                mXuTou = tf.GetComponent<UISprite>();
                break;
            case "lb_xutou":
                mXutouLbl = tf.GetComponent<UILabel>();
                break;
            case "chouma_black":
                mBlackChouMa = tf.gameObject;
                break;
            case "chouma_active":
                mActiveChouMa = tf.gameObject;
                break;
        }
    }

    /// <summary>
    /// 根据游戏状态来设置筹码和中间押注区域的状态
    /// </summary>
    /// <param name="animSelect"></param>
    public void SetState(Item_FQZS_AnimaSelect animSelect)
    {
        SetAllChouMaState();

        animSelect.SetAnimEnable();
    }

    /// <summary>
    /// 设置筹码的状态
    /// </summary>
    /// <param name="state"></param>
    private void SetAllChouMaState()
    {
        for (int i = 0; i < mChouMaLst.Count; i++)
        {
            long max_bet = FQZSGameManager.GetCurBetMax();
            RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, FQZSGameManager.LeiZhuSeat);

            if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
            {
                mActiveChouMa.SetActive(true);
                mBlackChouMa.SetActive(false);

                if ((leizhu != null || FQZSGameManager.IsSysLeiZhu) && FQZSGameManager.CurGold >= mChouMaLst[i].CurrValue && max_bet >= mChouMaLst[i].CurrValue && FQZSGameManager.LeiZhuSeat != RoleManager.Self.ChairSeat)
                {
                    mChouMaLst[i].GetComponent<UISprite>().IsGray = false;
                    mChouMaLst[i].GetComponent<BoxCollider>().enabled = true;
                }
                else
                {
                    mChouMaLst[i].GetComponent<UISprite>().IsGray = true;
                    mChouMaLst[i].GetComponent<BoxCollider>().enabled = false;
                    mChouMaLst[i].GetComponent<Item_FQZS_ChouMa>().SetChooseState(false);
                }
            }
            else
            {
                mActiveChouMa.SetActive(false);
                mBlackChouMa.SetActive(true);
            }
        }

        //设置续投按钮的状态
        if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
        {
            if (!IsCanXuTou())
            {
                ChangeXutouState(false);
            }
            else
            {
                if (!IsAreadyXuTou)
                {
                    if (CheckCanXuTou())
                    {
                        ChangeXutouState(true);
                    }
                    else
                    {
                        ChangeXutouState(false);
                    }
                }
                else
                {
                    ChangeXutouState(false);
                }
            }
        }
        else
        {
            ChangeXutouState(false);
        }
    }

    private bool IsCanXuTou()
    {
        RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, FQZSGameManager.LeiZhuSeat);
        if (FQZSGameManager.IsSysLeiZhu == false && leizhu == null)
            return false;

        for (int i = 0; i < FQZSGameManager.mXuTouArr.Length; i++)
        {
            if (FQZSGameManager.mXuTouArr[i] > 0)
                return true;
        }
     
        return false;
    }
}
