using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 潘多拉相关逻辑管理
/// </summary>
public class ScenePandoraMgr {
    public static Dictionary<ushort, uint> dic_fish = new Dictionary<ushort, uint>();
    public static void GlobalInit() {
    }

    public static void Shutdown() {
        dic_fish.Clear();
    }
    public static void OnCatchPandora(SC_GR_CatchPandora cmd) {//潘多拉捕获
        //Debug.LogError(LitJson.JsonMapper.ToJson(cmd));
        if (cmd.Immediate == false) {
            dic_fish[cmd.FishID] = cmd.RealFishCfgID;
        } else {
            dic_fish.Remove(cmd.FishID);
            Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(cmd.FishID);
            if (fish == null) {//找不到潘多拉
                LogMgr.LogError("找不到潘多拉");
                return;
            }
            GameObject old_model = fish.Model;
            fish.ResetCfgID(cmd.RealFishCfgID);
            PandoraAnim anim = TimeManager.Mono.gameObject.AddComponent<PandoraAnim>();
            anim.InitData(old_model, fish);
            ////潘多拉爆开特效
            //GameObject obj = GameObject.Instantiate(FishResManager.Instance.mEffPandoraDie);
            //obj.transform.localPosition = fish.Position;
            //obj.transform.localScale = fish.Transform.localScale * 2;
            //float life = GameUtils.CalPSLife(obj);
            //GameObject.Destroy(obj, life + 0.5f);

            ////fish.SetPostLaunch(0.5f);
            //fish.SetWait(life - 0.5f);
            if (fish.FishID == ConstValue.WorldBossID) {//全服宝箱锁定头像位置特殊处理
                fish.Transform.rotation *= Quaternion.Euler(Vector3.up * 90f);
            } else if (fish.FishID == ConstValue.PirateBoxID) {//海盗宝箱锁定头像位置特殊处理
                fish.Transform.rotation *= Quaternion.Euler(Vector3.up * 90f);
            }
            AudioManager.PlayAudio(FishConfig.Instance.AudioConf.PDLSwitch);
        }
    }
}
