using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eff_LK_ShanDian : MonoBehaviour {
    public GameObject mObjLine;

    public void Awake() {
        this.mObjLine = this.transform.Find("anim_line").gameObject;
        this.mObjLine.SetActive(false);
    }

    public void InitData(Vector2 base_pos, List<Vector2> poss) {
        Vector3 pos = base_pos;
        pos.z -= 20;
        this.transform.localPosition = pos;

        float base_width = 1137;
        GameObject line;
        foreach (var item in poss) {
            float angle = Tools.Angle(item - base_pos, Vector2.right);
            float dis = Vector2.Distance(base_pos, item);
            line = GameObject.Instantiate(this.mObjLine,this.transform);
            line.transform.localEulerAngles = new Vector3(0, 0, angle);
            line.transform.localScale = Vector3.one * dis / base_width;
            line.SetActive(true);
        }
    }
}
