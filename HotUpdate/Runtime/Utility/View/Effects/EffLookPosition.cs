using UnityEngine;
using System.Collections;

/// <summary>
/// 瞄准特效脚本，跟随鼠标移动
/// </summary>
public class EffLookPosition : MonoBehaviour {
    private float z;
    public void InitData(SkillVo skill) {

        //特效大小随着技能半径更改
        EffectVo colliderVo = FishConfig.Instance.EffectConf.TryGet(skill.EffID0);
		LogMgr.Log("colliderVo.Value2:" + colliderVo.Value2);
        float scale = colliderVo.Value2 / 220f;
        Transform tf = this.transform.GetChild(0);
        tf.localScale = tf.localScale * scale;


        Vector3 scene_pos = Utility.MainCam.WorldToScreenPoint(transform.position);
        this.z = scene_pos.z;
    }

    public void LateUpdate() {
        Vector3 scene_pos = new Vector3(GInput.mousePosition.x, GInput.mousePosition.y, this.z);
        transform.position = Utility.MainCam.ScreenToWorldPoint(scene_pos);
    }
}
