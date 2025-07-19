using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRoom700Help : UIHelpPanel
    {
        List<Text> awardNum = new List<Text>();
        List<TextMeshProUGUI> awardDes = new List<TextMeshProUGUI>();

        Transform content;
        protected override void Awake()
        {
            //Transform page1 = transform.Find("Panel/SRect_VipList/ViewPort/Content/CloneItem/Page0");
            //for (int i = 0; i < 3; i++)
            //{
            //    awardNum.Add(page1.Find("Frame/Txt" + i).GetComponent<Text>());
            //    awardDes.Add(page1.Find("Frame/TxtDes" + i).GetComponent<TextMeshProUGUI>());

            //    awardNum[i].text = ToolUtil.ShowF2Num2((long)Game700Model.Instance.BombList[i]);
            //    long temp0 = long.Parse(Game700Model.Instance.awardList[i][0]);
            //    long temp1 = long.Parse(Game700Model.Instance.awardList[i][1]);
            //    long temp2 = long.Parse(Game700Model.Instance.awardList[i][2]);
            //    awardDes[i].text = string.Format("R$ {0}=33.33%  R$ {1}=33.33%  R$ {2}=33.33%", ToolUtil.ShowF2Num2(temp0), ToolUtil.ShowF2Num2(temp1), ToolUtil.ShowF2Num2(temp2));
            //}


            pageCount = 3;
            base.Awake();
            content = transform.Find("Panel/SRect_VipList/ViewPort/Content");
        }


        public override LoopListViewItem2 OnUpdate(LoopListView2 view, int index)
        {

            string itemName = "CloneItem";
            var itemObj = view.NewListViewItem(itemName);
            int count = pageCount;
            int wrapindex = 0 <= index ? index % count : count + ((index + 1) % count) - 1;
            for (int i = 0; i < pageCount; i++)
                itemObj.transform.GetChild(i).gameObject.SetActive(i == wrapindex);
            return itemObj;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            //for (int i = 0; i < content.childCount; i++)
            //{
            //    if (content.GetChild(i).gameObject.activeSelf)
            //    {
            //        awardNum.Clear();
            //        awardDes.Clear();
            //        Transform page1 = content.GetChild(i).transform.Find("Page0");
            //        if(page1 != null)
            //        {
            //            for (int j = 0; j < 3; j++)
            //            {
            //                awardNum.Add(page1.Find("Frame/Txt" + j).GetComponent<Text>());
            //                awardDes.Add(page1.Find("Frame/TxtDes" + j).GetComponent<TextMeshProUGUI>());
            //                awardNum[j].text = ToolUtil.ShowF2Num2((long)Game700Model.Instance.BombList[j]);
            //                long temp0 = long.Parse(Game700Model.Instance.awardList[j][0]);
            //                long temp1 = long.Parse(Game700Model.Instance.awardList[j][1]);
            //                long temp2 = long.Parse(Game700Model.Instance.awardList[j][2]);
            //                awardDes[j].text = string.Format("R$ {0}=33.33%  R$ {1}=33.33%  R$ {2}=33.33%", ToolUtil.ShowF2Num2(temp0), ToolUtil.ShowF2Num2(temp1), ToolUtil.ShowF2Num2(temp2));
            //            }
            //        }
            //    }
            //}
        }

    }
}
