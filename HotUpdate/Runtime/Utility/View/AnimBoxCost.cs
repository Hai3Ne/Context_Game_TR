using UnityEngine;
using System.Collections;

public class AnimBoxCost : MonoBehaviour {
    public UITexture mTextureCurCost;
    public UILabel mLbCurCost;
    public UITexture mTextureTotalCost;
    public UILabel mLbTotalCost;
    public UITexture mTextureRank;
    public UILabel mLbRank;

    private float mShowPosX;
    private float mAnimTime = 0.3f;//显示隐藏动画时间
    private float mWaitTime = 0.2f;//等待时间
    private float mShowTime = 2;//显示时间

    public void SetClientSeat(byte seat) {
        if (seat == 0 || seat == 3) {
            this.transform.localPosition =  new Vector3(-1130, 0);
            this.mShowPosX = 300;
            this.mTextureCurCost.flip = UIBasicSprite.Flip.Nothing;
            this.mTextureTotalCost.flip = UIBasicSprite.Flip.Nothing;
            this.mTextureRank.flip = UIBasicSprite.Flip.Nothing;
        } else {
            this.transform.localPosition =  new Vector3(1130, 0);
            this.mShowPosX = -300;
            this.mTextureCurCost.flip = UIBasicSprite.Flip.Horizontally;
            this.mTextureTotalCost.flip = UIBasicSprite.Flip.Horizontally;
            this.mTextureRank.flip = UIBasicSprite.Flip.Horizontally;
        }
    }
    public void Play(bool is_show_rank) {
        if (is_show_rank) {
            this.StartAnim(this.mTextureCurCost.gameObject);
            TimeManager.DelayExec(this, this.mWaitTime, () => {
                this.StartAnim(this.mTextureTotalCost.gameObject);
            });
            TimeManager.DelayExec(this, this.mWaitTime * 2, () => {
                this.StartAnim(this.mTextureRank.gameObject);
            });
            TimeManager.DelayExec(this, this.mWaitTime * 2 + this.mShowTime + this.mAnimTime * 2, () => {
                GameObject.Destroy(this.gameObject);
            });
        } else {
            this.StartAnim(this.mTextureCurCost.gameObject);
            TimeManager.DelayExec(this, this.mShowTime + this.mAnimTime * 2, () => {
                GameObject.Destroy(this.gameObject);
            });
        }
    }

    public void StartAnim(GameObject obj) {
        this.Show(obj);
        TimeManager.DelayExec(this, this.mShowTime + this.mAnimTime, () => {
            this.Hide(obj);
        });
    }

    public void Show(GameObject obj) {
        Vector3 pos = obj.transform.localPosition;
        pos.x = this.mShowPosX;
        TweenPosition.Begin(obj, this.mAnimTime, pos);
    }
    public void Hide(GameObject obj) {
        TweenAlpha.Begin(obj, this.mAnimTime, 0);
        //Vector3 pos = obj.transform.localPosition;
        //pos.x = 0;
        //TweenPosition.Begin(obj, this.mAnimTime, pos);
    }
}
