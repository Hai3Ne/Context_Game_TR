using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace HotUpdate
{
    public class UIRoom800Help : UIHelpPanel
    {

        List<TextMeshProUGUI> betNum = new List<TextMeshProUGUI>();
        List<TextMeshProUGUI> rateList = new List<TextMeshProUGUI>();

        Transform content;
        protected override void Awake()
        {
            pageCount = 5;


            Transform page1 = transform.Find("Panel/SRect_VipList/ViewPort/Content/CloneItem/Page4");
            for (int i = 0; i < 5; i++)
            {
                betNum.Add(page1.Find("Txt" + i).GetComponent<TextMeshProUGUI>());
                betNum[i].text = ToolUtil.ShowF2Num2((long)(Game800Model.Instance.GearList[i] * ToolUtil.GetGoldRadio()));

                rateList.Add(page1.Find("TxtDes" + i).GetComponent<TextMeshProUGUI>());

                string num0 = ToolUtil.ShowF2Num2((Game800Model.Instance.RateList[i] - 1) * (long)ToolUtil.GetGoldRadio());
                string num1 = ToolUtil.ShowF2Num2((Game800Model.Instance.RateList[i] + 1) * (long)ToolUtil.GetGoldRadio());

                rateList[i].text = string.Format("On <color=#FFE57D>{0}%~{1}%</color>Jackpot", num0, num1);
            }
            base.Awake();
            content = transform.Find("Panel/SRect_VipList/ViewPort/Content");
        }

        protected override void OnEnable()
        {

            base.OnEnable();

            for (int i = 0; i < content.childCount; i++)
            {
                if (content.GetChild(i).gameObject.activeSelf)
                {
                    betNum.Clear();
                    rateList.Clear();
                    Transform page1 = content.GetChild(i).transform.Find("Page4");
                    if (page1 != null)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            betNum.Add(page1.Find("Txt" + j).GetComponent<TextMeshProUGUI>());
                            betNum[j].text = ToolUtil.ShowF2Num2((long)(Game800Model.Instance.GearList[j] * ToolUtil.GetGoldRadio()));

                            rateList.Add(page1.Find("TxtDes" + j).GetComponent<TextMeshProUGUI>());

                            string num0 = ToolUtil.ShowF2Num2((Game800Model.Instance.RateList[j] - 1) * (long)ToolUtil.GetGoldRadio());
                            string num1 = ToolUtil.ShowF2Num2((Game800Model.Instance.RateList[j] + 1) * (long)ToolUtil.GetGoldRadio());

                            rateList[j].text = string.Format("On <color=#FFE57D>{0}%~{1}%</color>Jackpot", num0, num1);
                        }
                    }
                }
            }



        }
    }
}
