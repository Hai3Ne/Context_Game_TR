using UnityEngine;
using System.Collections;

public class UILanguage : MonoBehaviour {
    public string Key;

    public void Awake() {
        UILabel lb = this.GetComponent<UILabel>();
        if (lb != null) {
            lb.text = StringTable.GetString(this.Key);
        }
    }


}
