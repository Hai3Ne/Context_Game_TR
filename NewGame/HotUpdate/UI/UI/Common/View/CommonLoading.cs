using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommonLoading : PanelBase
{
    private Text text;
    protected override void Awake()
    {
        base.Awake();
        text = gameObject.transform.Find("text").GetComponent<Text>();


    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        var param1 = param == null ? false : (bool)param;
        text.gameObject.SetActive(param1);
        text.text = "重新连接";
    }

    protected override void OnDisable()
    {
        base.OnDisable();
      
    }



}
