using UnityEngine;
using System.Collections;

/// <summary>
/// BOSS暴击点管理
/// </summary>
public class BaoJiDianManager : MonoBehaviour {
    private static BaoJiDianManager baojidian;

    private Transform mTranBaoJiDian;//暴击点
    private Fish mBoss;
    private FishBossVo mFishBoss;
    private BoxCollider mLockBox;//当前显示区域
    private float mNextRefershTime;//下次刷新时间

    public void InitData(Fish boss, FishBossVo bossVo) {
        this.mBoss = boss;
        this.mFishBoss = bossVo;
        //     Kubility.KAssetBundleManger.Instance.LoadGameObject(ResPath.EffPath + "UI/Ef_baojidian_1", SceneLogic.Instance.LogicUI.BattleUI, delegate(SmallAbStruct obj) {
        //         if (this.mTranBaoJiDian != null) {
        //             GameObject.Destroy(this.mTranBaoJiDian.gameObject);
        //         }
        //this.mTranBaoJiDian = GameUtils.ResumeShader(obj.MainGameObject).transform;
        //         this.CreateLockBox();
        //         this.RefershPos();
        //     });

        if (this.mTranBaoJiDian != null)
        {
            GameObject.Destroy(this.mTranBaoJiDian.gameObject);
        }

        GameObject obj = ResManager.LoadAndCreate(GameEnum.Fish_3D, ResPath.NewEffpath + "UI/Ef_baojidian_1", SceneLogic.Instance.LogicUI.BattleUI);
        this.mTranBaoJiDian = GameUtils.ResumeShader(obj).transform;
        this.CreateLockBox();
        this.RefershPos();
    }

    public void LateUpdate() {
        if (Time.realtimeSinceStartup > this.mNextRefershTime) {
            this.CreateLockBox();
        }
        this.RefershPos();
    }

    private void CreateLockBox() {//创建锁定区域
        this.mLockBox = this.mBoss.RandomGetLockPos();
        this.mNextRefershTime = Time.realtimeSinceStartup + this.mFishBoss.PointCD;
    }
    private void RefershPos() {
        if (this.mTranBaoJiDian != null && this.mLockBox != null) {
            Vector3 wp = this.mLockBox.transform.TransformPoint(this.mLockBox.center);
            wp = Utility.MainCam.WorldToScreenPoint(wp);
            wp.z = 0;
            this.mTranBaoJiDian.position = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(wp);
        }
    }

    public void OnDestroy() {
        if (this.mTranBaoJiDian != null) {
            GameObject.Destroy(this.mTranBaoJiDian.gameObject);
        }
    }


    public static void ShowBaoJiDian(Fish boss, FishBossVo bossVo) {//显示暴击点
        if(baojidian == null){
            baojidian = boss.Model.AddComponent<BaoJiDianManager>();
        }
        baojidian.InitData(boss, bossVo);
    }
    public static void HideBaoJiDian() {//隐藏暴击点
        if (baojidian != null) {
            GameObject.Destroy(baojidian);
        }
    }

}
