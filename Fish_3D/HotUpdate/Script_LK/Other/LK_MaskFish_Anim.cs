using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 蒙面鱼王变鱼效果
/// </summary>
public class LK_MaskFish_Anim : MonoBehaviour {
    private Transform mTf;
    private Vector3 mInitScale;
    private LKFish mNewFish;//蒙面鱼王变的鱼

    public void InitData(LKFish new_fish) {
        this.mTf = this.transform;
        this.mInitScale = this.mTf.localScale;
        this.mNewFish = new_fish;
    }

    public void OnDestroy() {
        if (this.mNewFish != null) {
            //蒙面鱼王捕获效果
            GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_MengMianYu, null);
            obj.transform.localPosition = this.mNewFish.Position;
            GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
            this.mNewFish.gameObject.SetActive(true);
        }
    }

	void Update () {
        this.mTf.localScale = this.mInitScale * (1 + 0.3f * Mathf.Sin(10 * Time.frameCount));
	}
}
