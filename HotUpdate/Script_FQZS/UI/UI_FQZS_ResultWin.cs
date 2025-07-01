using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FQZS_ResultWin : UILayer
{
    /// <summary>
    /// 结算界面的玩家列表
    /// </summary>
    public List<Item_FQZS_ResultPlayerInfo> mPlayerResultLst = new List<Item_FQZS_ResultPlayerInfo>();
    /// <summary>
    /// 擂主名称
    /// </summary>
    public UILabel mLeiZhuName;

    /// <summary>
    /// 擂主输赢
    /// </summary>
    public UILabel mLeiZhuReslut;

    /// <summary>
    /// 擂主输赢标记
    /// </summary>
    public UISprite mLeiZhuResultIcon;

    /// <summary>
    /// 本家输赢标记
    /// </summary>
    public UISprite mSelfResultIcon;

    /// <summary>
    /// 本家的名字
    /// </summary>
    public UILabel mSelfName;

    /// <summary>
    /// 本家输赢
    /// </summary>
    public UILabel mSelfResult;

    /// <summary>
    /// 最终选中的动物
    /// </summary>
    public UISprite mAnima;

    /// <summary>
    /// 设置界面信息
    /// </summary>
    /// <param name="leizhuName"></param>
    /// <param name="leizhuResult"></param>
    /// <param name="selfName"></param>
    /// <param name="selfResult"></param>
    public void InitData(string leizhuName,long leizhuResult,string selfName, long selfResult,byte animcode, tagScoreRankInfo[] playerInfos)
    {
        mLeiZhuName.text = leizhuName;

        if (leizhuResult < 0)
        {
            mLeiZhuResultIcon.spriteName = "lose";

        }
        else if (leizhuResult == 0)
        {
            mLeiZhuResultIcon.spriteName = string.Empty;
        }
        else
        {
            mLeiZhuResultIcon.spriteName = "win";
        }
        mLeiZhuReslut.text = leizhuResult.ToString();

        mSelfName.text = selfName;
        if (selfResult < 0)
        {
            mSelfResultIcon.spriteName = "lose";
            AudioManager.PlayAudio(GameEnum.FQZS, FQZSGameConfig.SOUND_LOST);
        }
        else if (selfResult == 0)
        {
            mSelfResultIcon.spriteName = string.Empty;
        }
        else
        {
            mSelfResultIcon.spriteName = "win";
            AudioManager.PlayAudio(GameEnum.FQZS, FQZSGameConfig.SOUND_WIN);
        }

        mSelfResult.text = selfResult.ToString();

        mAnima.spriteName = GetAnimaNameWithCode(animcode);
        mAnima.MakePixelPerfect();
        mAnima.transform.localScale = new Vector3(1.5f, 1.5f);

        for (int i = 0; i < playerInfos.Length; i++)
        {
            mPlayerResultLst[i].SetUI(playerInfos[i].NickName, playerInfos[i].WinScore);
        }

        StartCoroutine(CloseSelf());
    }

    /// <summary>
    /// 结算界面打开5秒后删除
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseSelf()
    {
        yield return new WaitForSeconds(5);
        Close();
    }

    private string GetAnimaNameWithCode(byte code)
    {
        switch (code)
        {
            case 1:
                return "shayu";
            case 4:
                return "yanzi";
            case 5:
                return "tuzi";
            case 6:
                return "gezi";
            case 7:
                return "xiongmao";
            case 8:
                return "kongque";
            case 9:
                return "houzi";
            case 10:
                return "laoying";
            case 11:
                return "shizi";
            case 0:
                return "tongchi";
            case 13:
                return "jinsha";
            case 12:
                return "tongpei";
        }

        return null;
    }



    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "leizhuname":
                mLeiZhuName = tf.GetComponent<UILabel>();
                break;
            case "leizhuresluticon":
                mLeiZhuResultIcon = tf.GetComponent<UISprite>();
                break;
            case "leizhuresult":
                mLeiZhuReslut = tf.GetComponent<UILabel>();
                break;
            case "selfreslut":
                mSelfResult = tf.GetComponent<UILabel>();
                break;
            case "selfresluticon":
                mSelfResultIcon = tf.GetComponent<UISprite>();
                break;
            case "selfname":
                mSelfName = tf.GetComponent<UILabel>();
                break;
            case "selectanima":
                mAnima = tf.GetComponent<UISprite>();
                break;
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
                mPlayerResultLst.Add(BindItem<Item_FQZS_ResultPlayerInfo>(tf.gameObject));
                break;
        }
    }
}
