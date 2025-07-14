using System;
using System.Collections.Generic;
using UnityEngine;

public class BossLifePanel
{
    UISlider bossHpBar;
	UISprite bossIcon;
    UISprite mSprBossName;
    private UILabel mLbHpCount;//BOSS血条数量
	Transform menuUITrans;

    private Vector3 init_pos;
	public void Init(Transform trans, UISprite icon, UISlider hpbar, UISprite spr_boss_name,UILabel lb_hp_count)
	{
		menuUITrans = trans;
		bossHpBar = hpbar;
		bossIcon = icon;
		mSprBossName = spr_boss_name;
        this.mLbHpCount = lb_hp_count;
		menuUITrans.gameObject.SetActive (false);
        SceneLogic.Instance.RegisterGlobalMsg(SysEventType.FishEvent_BossComing, OnBossWillComing);

        this.init_pos = this.menuUITrans.transform.localPosition;
	}

	float timePer = 0f;//, hpUpSpeed = 1f;
	byte bossLifeIdx = 0;
	Color tarColor = Color.white;
    int tarCount = -1;//血条数量
	bool tweenBossHpBar = false;

    public void Show() {
        if (mBossComingStage == 2 || mBossComingStage == 1) {
            return;
        }
        Fish boss = SceneLogic.Instance.FishMgr.activedBoss;
        if (boss != null) {
            //if (boss.FishID != ConstValue.WorldBossID && menuUITrans.gameObject.activeSelf == false) {
            if (menuUITrans.gameObject.activeSelf == false) {
                menuUITrans.gameObject.SetActive(true);
                if (boss.FishID == ConstValue.WorldBossID) {
                    menuUITrans.transform.localPosition = new Vector3(0, 80);
                    TweenPosition tp = menuUITrans.GetComponent<TweenPosition>();
                    if (tp != null) {
                        tp.enabled = false;
                    }
                } else {
                    menuUITrans.transform.localPosition = this.init_pos;
                    mSprBossName.spriteName = boss.vo.IconID.ToString();
                    mSprBossName.MakePixelPerfect();
                    bossIcon.spriteName = boss.vo.IconID.ToString();
                }
                tarCount = -1;
                tweenBossHpBar = false;
                this.SetBOSSLift(boss, true);
            }
        } else {
            menuUITrans.gameObject.SetActive(false);
        }
    }
    public void Hide() {
        menuUITrans.gameObject.SetActive(false);
    }

    public void ShowUIAnim() {//进入场景动画
        if (SceneLogic.Instance.FishMgr.activedBoss != null && SceneLogic.Instance.FishMgr.activedBoss.FishID == ConstValue.WorldBossID) {
            menuUITrans.localPosition = new Vector3(0, 80);
        } else {
            menuUITrans.localPosition = new Vector3(0, 80);
            TweenPosition.Begin(menuUITrans.gameObject, SceneGameUIMgr.anim_time, new Vector3(0, -80), false);
        }

    }
    public void HideUIAnim() {//离开场景动画
        TweenPosition.Begin(menuUITrans.gameObject, SceneGameUIMgr.anim_time, new Vector3(0, 80), false);
    }

    public void ResetshInfo(UI_BossChestNum action) {//全服宝箱出现刷新血条信息
        action.mSliderHP.foregroundWidget.color = this.bossHpBar.foregroundWidget.color;
        action.mSliderHP.value = this.bossHpBar.value;
        action.mLbHpCount.text = this.mLbHpCount.text;
    }

    public void SetHPColor(Color color) {
        bossHpBar.foregroundWidget.color = color;
        if (SceneLogic.Instance.WorldBossMgr.action != null) {
            SceneLogic.Instance.WorldBossMgr.action.mSliderHP.foregroundWidget.color = color;
        }
    }
    public void SetHPVal(float val) {
        bossHpBar.value = val;
        if (SceneLogic.Instance.WorldBossMgr.action != null) {
            SceneLogic.Instance.WorldBossMgr.action.mSliderHP.value = val;
        }
    }
    public void SetHPCount(int count) {
        this.mLbHpCount.text = string.Format("x{0}", count);
        if (SceneLogic.Instance.WorldBossMgr.action != null) {
            SceneLogic.Instance.WorldBossMgr.action.mLbHpCount.text = this.mLbHpCount.text;
        }
    }

    public void StartTween() {//原地滚动一次血条
        Fish boss = SceneLogic.Instance.FishMgr.activedBoss;
        if (boss == null) {
            return;
        }
        FishBossVo bossVo = null;
        int lift = 0;
        List<FishBossVo> list = FishConfig.Instance.FishBossConf;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].CfgID == boss.vo.CfgID) {
                if (list[i].ID > bossLifeIdx) {
                    lift++;
                } else if (list[i].ID == bossLifeIdx) {
                    lift++;
                    bossVo = list[i];
                }
            }
        }
        tarCount = lift;
        tarColor = new Color(
            (bossVo.Color >> 16 & 0xFF) / 255f,
            (bossVo.Color >> 8 & 0xFF) / 255f,
            (bossVo.Color & 0xFF) / 255f,
            (bossVo.Color >> 24 & 0xFF) / 255f
        );
        tweenBossHpBar = true;
        timePer = 0f;
    }

    private void SetBOSSLift(Fish boss, bool is_first) {
        if (bossLifeIdx == 0) {
            this.SetHPVal(1);
            is_first = true;
        }
        bossLifeIdx = boss.BossLifeIndex;
        if (bossLifeIdx == 0) {//血管不对则移除
            return;
        }
        List<FishBossVo> list = FishConfig.Instance.FishBossConf;
        FishBossVo bossVo = null;
        int lift = 0;

        for (int i = 0; i < list.Count; i++) {
            if (list[i].CfgID == boss.vo.CfgID) {
                if (list[i].ID > bossLifeIdx) {
                    lift++;
                } else if (list[i].ID == bossLifeIdx) {
                    lift++;
                    bossVo = list[i];
                }
            }
        }
        if (bossVo != null && tarCount != lift) {
            if (is_first == false) {
                this.SetHPColor(tarColor);
                this.SetHPCount(tarCount);
            }
            tarCount = lift;
            tarColor = new Color(
                (bossVo.Color >> 16 & 0xFF) / 255f,
                (bossVo.Color >> 8 & 0xFF) / 255f,
                (bossVo.Color & 0xFF) / 255f,
                (bossVo.Color >> 24 & 0xFF) / 255f
            );
            if (is_first) {
                this.SetHPColor(tarColor);
                this.SetHPVal(1);
                this.SetHPCount(tarCount);
                tweenBossHpBar = false;
            } else {
                tweenBossHpBar = true;
                timePer = 0f;
            }
        }
        if (bossVo != null && bossVo.IsCritPoint) {
            //BaoJiDianManager.ShowBaoJiDian(boss, bossVo);
        } else {
            BaoJiDianManager.HideBaoJiDian();
        }
    }

	public void Update(float delta)
	{
		Fish boss = SceneLogic.Instance.FishMgr.activedBoss;
        if (boss != null) {
            if (mBossComingStage == 2) {
				Vector3 dir = Vector3.right;
				if (boss.Transform.rotation.eulerAngles.y < 0f) {
					dir = Vector3.right;
					boss.Transform.position = ConstValue.BOSS_SHOW_INITED_POS + new Vector3 (-80f, 0f, 0f);
				} else {
					dir = Vector3.left;
					boss.Transform.position = ConstValue.BOSS_SHOW_INITED_POS + new Vector3 (80f, 0f, 0f);
				}
                if (boss.vo.BirthPosition.Length >= 3) {
                    boss.Transform.position += new Vector3(boss.vo.BirthPosition[0], boss.vo.BirthPosition[1], boss.vo.BirthPosition[2]);
                }
				Vector3 pos = boss.Transform.position + dir * 80f;
				SceneObjMgr.Instance.SceneTopCamera.transform.position = ConstValue.BOSS_SHOW_INITED_POS;
				//iTween.MoveTo (boss.Transform.gameObject, iTween.Hash ("position", pos, "time", 0.2f, "islocal", false));
				boss.Transform.position = pos;
				boss.Transform.rotation = boss.OrgRot * Quaternion.Euler (Vector3.up * 90f);
				boss.Anim.Play ("coming");
                boss.ShowTimeTick = 2;
                AnimationClip[] ac = boss.Anim.runtimeAnimatorController.animationClips;
                for (int i = 0; i < ac.Length; i++) {
                    if (ac[i].name == "birth") {
                        boss.ShowTimeTick = ac[i].length + 0.5f;
                        break;
                    }
                }
				boss.IsShowSelf = true;
				SceneObjMgr.Instance.SceneTopCamera.clearFlags = CameraClearFlags.Skybox;
				SceneObjMgr.Instance.SceneTopCamera.cullingMask = 1<<LayerMask.NameToLayer ("Water") | 1<<LayerMask.NameToLayer ("FishLayer") | 1<<LayerMask.NameToLayer ("FishPartLayer");// 0xFFFFFF;
				SceneObjMgr.Instance.UpdateBossBackground (boss.vo);
				mBossComingStage = 3;
				BossAudioVo bossAudio = FishConfig.Instance.BossAudioConf.TryGet(boss.FishCfgID);
				GlobalAudioMgr.Instance.PlayAudioEff (bossAudio.AppearAudio);

                SceneLogic.Instance.LogicUI.BossLifeUI.Show();
				return;
			}
			if (boss.IsShowSelf)
			{
                if (boss.Anim.IsInTransition(0) == false && boss.ShowTimeTick <= 0){// && boss.Anim.GetCurrentAnimatorStateInfo(0).IsName("coming") == false) {
					boss.Anim.Set_Bool (FishAnimatorStatusMgr.LaughHashName, false);
                    SceneObjMgr.Instance.ResetTopCameraParam();
					SceneLogic.Instance.Notifiy(SysEventType.FishEvent_BossAppear,null);
					boss.IsShowSelf = false;
					mBossComingStage = 0;
					Fish.IsBossFirstShowing = false;
				}
			}

            //if (boss.IsDizzy) {
            //    this.SetHPVal(1);
            //    this.SetHPColor(tarColor);
            //    tweenBossHpBar = false;
            //    this.SetHPCount(tarCount);
            //}
            //else 
            if (bossLifeIdx != boss.BossLifeIndex)
			{
                this.SetBOSSLift(boss, false);
			}
        }

        if (tweenBossHpBar) {
            this.SetHPVal(Mathf.Lerp(1f, 0f, timePer));
            timePer += delta;
            if (bossHpBar.value < 0.01f) {
                this.SetHPVal(1);
                this.SetHPColor(tarColor);
                tweenBossHpBar = false;
                this.SetHPCount(tarCount);
            }
        }
	}

	int mBossComingStage = 0;
	GameObject bosscomingEffGo;
	void OnBossWillComing(object data)
	{
		GlobalAudioMgr.Instance.StopBgMusic ();
		Fish.IsBossFirstShowing = true;
        Fish boss = SceneLogic.Instance.FishMgr.activedBoss;
        if (boss != null && boss.FishID != ConstValue.WorldBossID) {
            GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.BossWarning);

            ResManager.LoadAsyn<GameObject>(ResPath.NewUIPath + "Eff_BossComing", (ab_data, obj) => {
                obj = GameUtils.CreateGo(obj, SceneObjMgr.Instance.UIPanelTransform);

                obj.AddComponent<ResCount>().ab_info = ab_data;
                UITexture texture_boss = obj.transform.Find("spr_frame/texture_boss").GetComponent<UITexture>();
                UILabel lb_boss = obj.transform.Find("spr_frame/lb_boss_name").GetComponent<UILabel>();
                if (boss.vo.ComingPosition.Length >= 2) {
                    texture_boss.transform.localPosition = new Vector3(boss.vo.ComingPosition[0], boss.vo.ComingPosition[1]);
                }
                ResManager.LoadAsyn<Texture>(ResPath.NewBossHalfBody + boss.vo.HalfBodyID,(tt_data,img)=>{
                    if (img != null)
                    {
                        Texture tex = img as Texture;
                        texture_boss.mainTexture = tex;
                        texture_boss.MakePixelPerfect();
                    }
                    if(ab_data != null)
                        ResManager.UnloadAB(ab_data, false);
                }, GameEnum.Fish_3D);
                lb_boss.text = StringTable.GetString(boss.vo.Name);

                Animator anim = obj.GetComponent<Animator>();
                float time = anim.GetCurrentAnimatorStateInfo(0).length;
                GameObject.Destroy(obj, time);

                TimeManager.DelayExec(time, OnComoingAnimFinish);
            }, GameEnum.Fish_3D);

            //SceneLogic.Instance.EffectMgr.PlayBossComing(OnComoingAnimFinish, delegate(GameObject arg) {
            //    if (isShutdown) {
            //        if (bosscomingEffGo != null) {
            //            GameObject.Destroy(bosscomingEffGo);
            //        }
            //        return false;
            //    }
            //    return true;
            //});
            mBossComingStage = 1;
        } else {
            mBossComingStage = 2;
        }
	}

	void OnComoingAnimFinish()
	{
		mBossComingStage = 2;
	}

	bool isShutdown = false;
	public void Shutdown()
	{
		GlobalAudioMgr.Instance.StopBgMusic ();
		SceneLogic.Instance.UnRegisterGlobalMsg(SysEventType.FishEvent_BossComing, OnBossWillComing);
		bossHpBar = null;
		bossIcon = null;
		mSprBossName = null;
		menuUITrans = null;
		mBossComingStage = 0;
		isShutdown = true;
	}
}