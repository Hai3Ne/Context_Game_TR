using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_LeiZhu : UIItem
{
    /// <summary>
    /// 没有擂主时，显示头像
    /// </summary>
    public GameObject mObjNoLeiZhu;

    /// <summary>
    /// 擂主的头像
    /// </summary>
    public UITexture mLeiZhuHead;

    /// <summary>
    /// 擂主的乐豆
    /// </summary>
    public UILabel mLeiZhuLeDou;

    /// <summary>
    /// 擂主连擂次数
    /// </summary>
    public UILabel mLianLei;

    /// <summary>
    /// 擂主昵称
    /// </summary>
    public UILabel mLeiZhuName;

    /// <summary>
    /// 初始化擂主信息
    /// </summary>
    /// <param name="leizhu"></param>
    /// <param name="list"></param>
    /// <param name="count"></param>
    public void InitData(RoleInfo leizhu, int count, Item_FQZS_MoreInfo moreInfo)
    {
        SetLeiZhu(leizhu, count, moreInfo,true);
    }

    /// <summary>
    /// 刷新擂主信息
    /// </summary>
    /// <param name="leiZhuSeat"></param>
    public void RefrshLsiZhuInfo(Item_FQZS_MoreInfo moreInfo,bool isAddShangLeiMsg = false)
    {
        RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, FQZSGameManager.LeiZhuSeat);

        SetLeiZhu(leizhu, FQZSGameManager.LeiZhuTimes, moreInfo, isAddShangLeiMsg);
    }

    public void SetLeiZhu(RoleInfo leizhu, int count, Item_FQZS_MoreInfo moreInfo,bool isAddShangLeiMsg)
    {
        if (leizhu != null) {
            this.mObjNoLeiZhu.SetActive(false);
            this.mLeiZhuHead.gameObject.SetActive(true);

            mLeiZhuName.text = leizhu.NickName;
            mLeiZhuLeDou.text = string.Format("[D4A488FF]乐豆：[-]{0}", Tools.longToStr(FQZSGameManager.LeiZhuLeDou, 3));

            mLeiZhuHead.uvRect = GameUtils.FaceUVRect(leizhu.FaceID);
        }
        else
        {
            this.mObjNoLeiZhu.SetActive(true);
            this.mLeiZhuHead.gameObject.SetActive(false);

            if (FQZSGameManager.IsSysLeiZhu)
            {
                mLeiZhuName.text = "系统守擂";
                mLeiZhuLeDou.text = "[D4A488FF]乐豆：[-]1,000,000,000";
            }
            else
            {
                mLeiZhuName.text = "等待上擂";
                mLeiZhuLeDou.text = "[D4A488FF]乐豆：[-]0";
            }
        }
        mLianLei.text = string.Format("[D4A488FF]连擂：[-]{0}", Mathf.Min(99, count).ToString());
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "leizhu_tx":
                mLeiZhuHead = tf.GetComponent<UITexture>();
                break;
            case "spr_no_leizhu":
                this.mObjNoLeiZhu = tf.gameObject;
                break;
            case "ledou":
                mLeiZhuLeDou = tf.GetComponent<UILabel>();
                break;
            case "lianlei":
                mLianLei = tf.GetComponent<UILabel>();
                break;
            case "leizhuname":
                mLeiZhuName = tf.GetComponent<UILabel>();
                break;
        }
    }
}
