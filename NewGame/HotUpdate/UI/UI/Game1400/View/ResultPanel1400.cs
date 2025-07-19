using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public partial class ResultPanel1400 : PanelBase
    {
        List<Text> textList = new List<Text>();

        List<int> aniIndex = new List<int>() { 1,5,2,6,3,8,4,7};
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            Init();
        }

        private void Init()
        {
            textList.Clear();
            textList.Add(m_Txt_0);
            textList.Add(m_Txt_1);
            textList.Add(m_Txt_2);
            textList.Add(m_Txt_3);
            textList.Add(m_Txt_4);
            textList.Add(m_Txt_5);
            Transform ccc = m_Spine_Result.transform.GetChild(0).GetChild(0).Find("ccc");
            for(int i = 0;i < textList.Count; i++)
            {
                textList[i].transform.SetParent(ccc.GetChild(i));
                textList[i].transform.localScale = Vector3.one;
                textList[i].transform.localPosition = new Vector3(52,1.63f,0);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            int index = Game1400Model.Instance.winPos % 8;
            if (index == 0)
                index = 8;
            index = aniIndex[index - 1];
            ToolUtil.PlayAnimation(m_Spine_Result.transform, index+"a",false,()=> 
            {
                ToolUtil.PlayAnimation(m_Spine_Result.transform, index + "b", true, () =>
                {
                });
            });

            if(Game1400Model.Instance.n64PowerGold <0)///输钱  183 
                CoreEntry.gAudioMgr.PlayUISound(183);
            else
                CoreEntry.gAudioMgr.PlayUISound(187);
            int index2 = Game1400Model.Instance.ucShowArea % 8;
            if (index2 == 0)
                index2 = 8;
            m_TxtM_DoubleRate.text = index2 + "";
            for(int i = 1;i < textList.Count;i++)
            {
                if (Game1400Model.Instance.arrayAward[i - 1].n64Gold > 0)
                    textList[i].text = string.Format("{0}    <color=#ffffff>  +{1}</color>", CommonTools.BytesToString(Game1400Model.Instance.arrayAward[i - 1].szName), ToolUtil.AbbreviateNumberF0(Game1400Model.Instance.arrayAward[i - 1].n64Gold));
                else
                    textList[i].text = "";
            }

            var uid = MainUIModel.Instance.palyerData.m_i8roleID.ToString();
            string userName = $"玩家{uid.Substring(uid.Length - 4, 4)}";
            textList[0].text = string.Format("{0}    <color=#ffffff>  +{1}</color>", userName, ToolUtil.AbbreviateNumberF0(Game1400Model.Instance.n64PowerGold));
        }

        public void RegisterListener()
        {
            m_Btn_Confirme.onClick.AddListener(ClickBtnConfirme);
        }

        public void UnRegisterListener()
        {
            m_Btn_Confirme.onClick.RemoveListener(ClickBtnConfirme);
        }


        private void ClickBtnConfirme()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            gameObject.SetActive(false);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
        }
    }
}
