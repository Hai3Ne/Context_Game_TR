using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹管理者
/// </summary>
public class LKBulletManager {
    private static GameObject[] obj_bullets = new GameObject[4 * 2 + 4];//子弹 
    private static GameObject[] obj_fishnet = new GameObject[4 * 2];//渔网
    public static GameObject PreLoadBullet(bool is_self, int kind) {//子弹预加载
        int index = kind;
        if (is_self == false) {
            index += 4;
        }

        if (obj_bullets[index] == null) {
            GameObject obj = ResManager.LoadAsset<GameObject>(GameEnum.Fish_LK, string.Format(LKGameConfig.Bullet_Path, index + 1));
            obj_bullets[index] = obj;
            return obj;
        } else {
            return obj_bullets[index];
        }
    }
    public static GameObject PreLoadFishNet(bool is_self, int kind) {//渔网预加载
        int index = kind % 4;
        if (is_self == false) {
            index += 4;
        }
        if (obj_fishnet[index] == null) {
            GameObject obj = ResManager.LoadAsset<GameObject>(GameEnum.Fish_LK, string.Format(LKGameConfig.FishNet_Path, index + 1));
            obj_fishnet[index] = obj;
            return obj;
        } else {
            return obj_fishnet[index];
        }
    }
    private static GameObject CreateObj(bool is_self, int kind) {//创建子弹对象
        int index;
        if (kind >= 4 || is_self == false) {//能量炮+别人子弹
            index = kind + 4;
        } else {
            index = kind;
        }
        if (obj_bullets[index] == null) {
            return GameObject.Instantiate(LKBulletManager.PreLoadBullet(true, index), null);
        } else {
            return GameObject.Instantiate(obj_bullets[index],null);
        }
    }
    public static GameObject CreateNetObj(bool is_self, int kind) {//创建渔网对象
        int index = kind % 4;
        if (is_self == false) {
            index += 4;
        }
        if (obj_fishnet[index] == null) {
            return GameObject.Instantiate(LKBulletManager.PreLoadFishNet(true, index), null);
        } else {
            return GameObject.Instantiate(obj_fishnet[index], null);
        }
    }

    public static Dictionary<int, List<LKBullet>> dic_bullet = new Dictionary<int, List<LKBullet>>();//子弹列表
    public static int mBulletID = 0;

    public static void ShootBullet(LKRole role,LKFish fish,Vector2 pos, int id, int kind, float angle, int mul, int handle) {//发射子弹
        GameObject obj = LKBulletManager.CreateObj(role.RoleInfo == RoleManager.Self, kind);
        LKBullet bullet = obj.GetComponent<LKBullet>();
        if (bullet == null) {
            bullet = obj.AddComponent<LKBullet>();
        }
        bullet.transform.position = SceneObjMgr.Instance.MainCam.ScreenToWorldPoint(pos);
        bullet.InitData(role, fish, id, kind, angle, mul, handle);

        List<LKBullet> list;
        if (dic_bullet.TryGetValue(role.RoleInfo.ChairSeat, out list) == false) {
            list = new List<LKBullet>();
            dic_bullet.Add(role.RoleInfo.ChairSeat, list);
        }

        list.Add(bullet);
    }

    public static int GetBulletCount(int seat) {//获取当前玩家发射了多少发子弹
        if (dic_bullet.ContainsKey(seat)) {
            return dic_bullet[seat].Count;
        }
        return 0;
    }

    public static void Update() {
        float delta = TimeManager.detalTime;
        bool is_catch = false;
        foreach (var bullet_list in dic_bullet.Values) {
            for (int i = bullet_list.Count - 1; i >= 0; i--) {//当前场景鱼
                if (bullet_list[i].UpdateBullet(delta)) {
                    bullet_list[i].Destroy();
                    bullet_list.RemoveAt(i);
                    is_catch = true;
                }
            }
        }
        if (is_catch) {
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.casting);
        }
    }

    public static void ClearAllBullet() {//清除场景中所有子弹
        foreach (var item in dic_bullet.Values) {
            foreach (var bullet in item) {
                bullet.Destroy();
            }
        }
        dic_bullet.Clear();
    }
    public static void Clear() {
        LKBulletManager.ClearAllBullet();
        for (int i = 0; i < obj_bullets.Length; i++) {
            obj_bullets[i] = null;
        }
    }
}
