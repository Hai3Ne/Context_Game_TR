using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class UIChip1400 : MonoBehaviour
    {
        private Image chipBg;
        int chipType = 0;
        private void Awake()
        {
            GetBindComponents(gameObject);
            chipBg = m_Btn_Chip.GetComponent<Image>();
        }

        public void SetChipValue(int type)
        {
            this.chipType = type;
            if(MainUIModel.Instance.bNormalGame)
                m_Txt_Chips.text = string.Format("<color=#{0}>{1}</color>",Game1400Model.Instance.chipTextColor[type], ToolUtil.ShowF2Num2(Game1400Model.Instance.chipValues[type])); 
            else
            {
                m_Txt_Chips.fontSize = 25;
                if (Game1400Model.Instance.chipValues[type] >= 1000000)
                {
                    m_Txt_Chips.text = string.Format("<color=#{0}>{1}</color>", Game1400Model.Instance.chipTextColor[type], ToolUtil.ShowF2Num2(Game1400Model.Instance.chipValues[type] / 10000) + "万");
                }
                else
                    m_Txt_Chips.text = string.Format("<color=#{0}>{1}</color>", Game1400Model.Instance.chipTextColor[type], ToolUtil.ShowF2Num2(Game1400Model.Instance.chipValues[type] / 10000) + "万");
            }
              
            chipBg.sprite = AtlasSpriteManager.Instance.GetSprite("Game1400:" + "img_pcbmsjb_d" + (type+1));
            ShowSelect();

            m_Txt_Chips.GetComponent<Outline>().effectColor = Game1400Model.Instance.outLineColor[type];
        }

        public void ShowSelect(bool bShow = false)
        {
            m_Trans_select.gameObject.SetActive(bShow);
        }

        public void SetChipColor()
        {
            if (Game1400Model.Instance.chipValues[chipType] < Game1400Model.Instance.currentGold)
            {
                chipBg.sprite = AtlasSpriteManager.Instance.GetSprite("Game1400:" + "img_pcbmsjb_d0" );
            }
            else
            {
                chipBg.sprite = AtlasSpriteManager.Instance.GetSprite("Game1400:" + "img_pcbmsjb_d" + (chipType + 1));
            }
        }



        protected void OnEnable()
        {
            RegisterListener();
        }
   

        public void RegisterListener()
        {
            Message.AddListener<int>(MessageName.GE_ClickChips1400, ChickChips1400);// 1400房间下注结果返回
            m_Btn_Chip.onClick.AddListener(ClickChip);
        }

        public void UnRegisterListener()
        {
            Message.RemoveListener<int>(MessageName.GE_ClickChips1400, ChickChips1400);// 1400房间下注结果返回
            m_Btn_Chip.onClick.RemoveListener(ClickChip);
        }

        private void ChickChips1400(int type)
        {
            ShowSelect(chipType == type);
        }

        private void ClickChip()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            Game1400Model.Instance.selectChipType = chipType;
            Message.Broadcast(MessageName.GE_ClickChips1400, chipType);       
        }
        protected void OnDisable()
        {
            UnRegisterListener();
        }
    }
}
