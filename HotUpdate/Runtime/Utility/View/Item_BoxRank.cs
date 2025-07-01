using UnityEngine;
using System.Collections;

public class Item_BoxRank : MonoBehaviour {
    public UILabel mLbRank;
    public UISprite mSprRank;
    public UITexture mTexturePlayer;
    public UILabel mLbName;
    public UILabel mLbNick;
    public UILabel mLbScore;//分数
    public UILabel mLbTick;//没有玩家时候给予提示

    public void InitData(int rank) {
        this.SetRank(rank);
        this.mTexturePlayer.gameObject.SetActive(false);
        this.mLbName.text = string.Empty;
        this.mLbNick.text = string.Empty;
        this.mLbScore.text = string.Empty;
        this.mLbTick.gameObject.SetActive(true);
    }
    public void InitData(int rank,tagUserWBData info) {
        this.SetRank(rank);
        this.mTexturePlayer.gameObject.SetActive(true);
        this.mTexturePlayer.mainTexture = FishResManager.Instance.playerIconTexAltas;
        this.mTexturePlayer.uvRect = GameUtils.FaceUVRect(info.FaceID);
        this.mLbName.text = info.NickName;
        this.mLbNick.text = FishConfig.GetTitle(info.TitleScore);
        this.mLbScore.text = info.Score.ToString();
        this.mLbTick.gameObject.SetActive(false);
    }
    private void SetRank(int rank) {
        if (rank > 3) {
            this.mLbRank.gameObject.SetActive(true);
            this.mSprRank.gameObject.SetActive(false);
            this.mLbRank.text = rank.ToString();
        } else {
            this.mLbRank.gameObject.SetActive(false);
            this.mSprRank.gameObject.SetActive(true);
            switch (rank) {
                case 1:
                    this.mSprRank.spriteName = "first";
                    break;
                case 2:
                    this.mSprRank.spriteName = "send";
                    break;
                case 3:
                default:
                    this.mSprRank.spriteName = "third";
                    break;
            }
        }
    }
}
