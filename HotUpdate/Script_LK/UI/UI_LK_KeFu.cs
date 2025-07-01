using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LK_KeFu : UILayer {

    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "bg":
                this.Close();
                break;
            case "btn_close":
                this.Close();
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) { }
}
