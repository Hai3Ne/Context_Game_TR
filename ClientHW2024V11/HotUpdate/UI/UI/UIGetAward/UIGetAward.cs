using DragonBones;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIGetAward : PanelBase
    {
        [SerializeField] private UnityArmatureComponent animator;
        [SerializeField] public Image icon;
        [SerializeField] private Button btnOver;
        Text TxtRewardGold;
        Action callBack;

        protected override void Awake()
        {
            Button BtnClose = transform.Find("BtnClose").GetComponent<Button>();
            BtnClose.onClick.AddListener(ClickClose);
            TxtRewardGold = transform.Find("icon_gold/Trans_Award/TxtRewardGold").GetComponent<Text>();
            if(btnOver == null) 
                btnOver = transform.Find("BtnOverlay").GetComponent<Button>();
            btnOver.onClick.AddListener(ClickClose);
        }

        protected async override void OnEnable()
        {
            base.OnEnable();
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.015f));
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.015f));

        }

        private void ClickClose()
        {
            MainPanelMgr.Instance.Close(transform.name);
            if (callBack != null)
                callBack();

        }

        public void SetJackPotNum(float num, Action callBack = null, long id = 9)
        {
            //CommonTools.PlayArmatureAni(animator.transform, "newAnimation", 1);
            CoreEntry.gAudioMgr.PlayUISound(48);
            this.callBack = callBack;
            if(id == 9)
            {
                var iconNum = num.ToString("F0");
                TxtRewardGold.text = "x" + iconNum + "";//  $"{iconNum}金币";
                icon.sprite = AtlasSpriteManager.Instance.GetSprite("Common:" + "ziyuan_icon_1");
                icon.transform.localScale = new Vector3(2f, 2f, 2f);
                icon.SetNativeSize();
            }
            else if(id == 14)
            {
                var iconNum = ((double)num /100f);
                TxtRewardGold.text = "x" + iconNum + "";//  $"{iconNum}金币";
                icon.sprite = AtlasSpriteManager.Instance.GetSprite("Common:" + "ziyuan_icon_2");
                icon.transform.localScale = new Vector3(2f, 2f, 2f);
                icon.SetNativeSize();
            }
            else if(id == 15)
            {
                var iconNum = ((double)num / 100f);
                TxtRewardGold.text = "x" + iconNum + "";//  $"{iconNum}金币";
                icon.sprite = AtlasSpriteManager.Instance.GetSprite("Common:" + "zfb");
                icon.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                icon.SetNativeSize();
            }
  
            if (MainPanelMgr.Instance.IsShow("ConfirmBuyPanel"))
            {
                MainPanelMgr.Instance.Close("ConfirmBuyPanel");
            }
            if (MainPanelMgr.Instance.IsShow("ShopPackPanel"))
            {
                MainPanelMgr.Instance.Close("ShopPackPanel");
            }
            if (MainPanelMgr.Instance.IsShow("FirstChargePanel"))
            {
                MainPanelMgr.Instance.Close("FirstChargePanel");
            }
        }
    }
}
