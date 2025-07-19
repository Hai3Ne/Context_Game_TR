using HotUpdate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconItem : MonoBehaviour
{
    [SerializeField] private Image Head;
    [SerializeField] private GameObject Select;
    [SerializeField] private Toggle toggle;
    [SerializeField] private ToggleGroup togGroup;
    [SerializeField] private int id;

    public void OnEnable()
    {
        
    }
    public void SetUpHead(int index,ToggleGroup group)
    {

        id = index;
        var spriteName = AtlasSpriteManager.Instance.GetSprite("Head:" + ConfigCtrl.Instance.Tables.TbHead_Config.Get(index).Icon);
        Debug.Log($"<color=#ffff00>{group}</color>");
        if (toggle.group==null)
        {
            toggle.group = group;
        }

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(ChangeIconId);
  
        Head.sprite = spriteName;

        if (index== MainUIModel.Instance.IconId)
        {
            toggle.isOn = true;
            toggle.interactable = false;
        }
        else
        {
            toggle.interactable = true;
            toggle.SetIsOnWithoutNotify(false);
        }
    }

    public void ChangeIconId(bool isOn) 
    {

        if (isOn)
        {
            MainUIModel.Instance.IconId = id;
            toggle.interactable = false;
        }
        else
        {
            toggle.interactable = true;
        }
    }


}
