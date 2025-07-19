using UnityEngine;
using System.Collections;

public class MulText : MonoBehaviour {
    private UILabel mLbText;
    public string mTextKey;

    public void Awake() {
        if (this.mLbText == null) {
            this.mLbText = this.GetComponent<UILabel>();
        }
        this.mLbText.text = StringTable.GetString(this.mTextKey);
	}
}
