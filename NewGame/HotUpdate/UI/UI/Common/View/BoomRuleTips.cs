using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoomRuleTips : PanelBase
{
    [SerializeField] private Button btnclose;

    // Start is called before the first frame update
    void OnEnable()
    {
        btnclose.onClick.AddListener(onClose);
    }

    // Update is called once per frame
    void OnDisable()
    {
        btnclose.onClick.RemoveListener(onClose);
    }

    private void onClose()
    {
        MainPanelMgr.Instance.Close("BoomRuleTips");
    }
}
