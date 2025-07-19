using System;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	
	public static void Begin(GameObject go, float life = -1f, Action cb = null)
	{
		if (go != null) {
			AutoDestroy ad = go.GetComponent<AutoDestroy> ();
			if (ad == null) {
				ad = go.AddComponent<AutoDestroy> ();			
			}
			ad.duration = life;
			ad.finishCb = cb;
		}else {
			cb.TryCall ();
		}
	}
    public static void Begin(GameObject go, byte client_seat, uint buff_cfg_id) {//根据当前用户BUFF自动销毁对象
        if (go != null) {
            AutoDestroy ad = go.GetComponent<AutoDestroy>();
            if (ad == null) {
                ad = go.AddComponent<AutoDestroy>();
            }
            ad.duration = GameUtils.CalPSLife(go);
            ad.finishCb = null;
            ad.client_seat = client_seat;
            ad.buff_cfg_id = buff_cfg_id;
        }
    }

	public Action finishCb;
	public float duration = 0;
    public byte client_seat;//用户位置
    public uint buff_cfg_id;//buffID
	Animator mAnimator;
	void Start()
	{
		var aim = this.gameObject.GetComponent<Animator> ();
		ParticleSystem[] pslist = this.gameObject.GetComponentsInChildren<ParticleSystem> ();

		if (pslist.Length == 0 && aim != null) {
			mAnimator = aim;
			duration = mAnimator.GetCurrentAnimatorClipInfo (0) [0].clip.length;
		} else {
			duration = duration <= 0 ? GameUtils.CalPSLife (this.gameObject) : duration;
		}
	}

	void Update()
	{
		if (duration > 0) {
			duration -= Time.deltaTime;
		}
		if (duration <= 0) {
            if (buff_cfg_id > 0) {
                //buff没0.5秒检查一次
                if (SceneLogic.Instance.BulletMgr.FindBBufferByID(this.client_seat, this.buff_cfg_id) != null) {
                    duration += 0.5f;
                } else {
                    GameObject.Destroy(this.gameObject);
                    finishCb.TryCall();
                }
            } else {
                GameObject.Destroy(this.gameObject);
                finishCb.TryCall();
            }
		}
	}
}

