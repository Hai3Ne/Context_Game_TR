using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class RankItem
    {
        public long n64RoleID;  // 玩家角色GUID
        public int nIconID; // 头像ID
        public long n64Gold;    // 中奖金币
        public int nRate;       //中奖倍数
        public byte[] szName = new byte[32];	// 名字
    }


    public class UIRoomRank : PanelBase
    {
        protected GameObject CloneItem;
        protected RectTransform Content;
        protected RectTransform parent;
        protected List<GameObject> CloneList = new List<GameObject>();
        protected override void Awake()
        {
            Button BtnClose = transform.Find("Panel/BtnClose").GetComponent<Button>();
            BtnClose.onClick.AddListener(ClickBtnClose);
            Content = transform.Find("Panel/Content").GetComponent<RectTransform>();
            parent = transform.Find("Panel/Content/ScrollView/ViewPort/Content").GetComponent<RectTransform>();
            CloneItem = transform.Find("Panel/CloneItem").gameObject;
        }

        public void ClickBtnClose()
        {
            MainPanelMgr.Instance.Close(transform.name);
        }

        private void SetContentPos(int gameID)
        {
            //if (gameID == 1000)
            //    Content.anchoredPosition = new Vector2(-28,-40);
            //else if(gameID == 900)
            //    Content.anchoredPosition = new Vector2(443, 13);
            //else if (gameID == 1100)
            //    Content.anchoredPosition = new Vector2(381, -11);
            //else
                //Content.anchoredPosition = new Vector2(0, -606f);
        }
  
        public virtual void InitData(List<RankItem> rankList,int gameID = 0)
        {
            for (int i = 0; i < CloneList.Count; i++)
                CloneList[i].gameObject.SetActive(false);
            for (int i = 0; i < rankList.Count; i++)
            {
                if (rankList[i].nIconID > 0)
                {
                    if(i >= CloneList.Count)
                    {
                        var go = ToolUtil.ClonePrefab(CloneItem, parent, "item");
                        //GameObject go = CommonTools.AddSubChild(parent.gameObject, "UI/Prefabs/UIRankPanel/FirstRes/UIRoom500Rank");
                        go.gameObject.SetActive(true);
                        CloneList.Add(go.gameObject);
                    }
                    var rankItem = CloneList[i].transform.GetComponent<UIRoomRankItem>();

                    rankItem.InitData(rankList[i],gameID);
                }
            }
            SetContentPos(gameID);
        }

        public static RankItem ConvertToRankItem(SGame1AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;

            return item;
        }
        public static RankItem ConvertToRankItem(SGame2AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;

            return item;
        }
        public static RankItem ConvertToRankItem(SGame4AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;

            return item;
        }

        public static RankItem ConvertToRankItem(SGame5AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;
            return item;
        }
        public static RankItem ConvertToRankItem(SGame6AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;
            return item;
        }
        public static RankItem ConvertToRankItem(SGame7AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.nRate = info.nRate;
            item.szName = info.szName;
            return item;
        }
        public static RankItem ConvertToRankItem(SGame8AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;
            return item;
        }
        public static RankItem ConvertToRankItem(SGame11AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;

            return item;
        }

        public static RankItem ConvertToRankItem(SGame19AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;

            return item;
        }

        public static RankItem ConvertToRankItem(SGame15AwardInfo info)
        {
            RankItem item = new RankItem();
            item.n64RoleID = info.n64RoleID;
            item.nIconID = info.nIconID;
            item.n64Gold = info.n64Gold;
            item.szName = info.szName;

            return item;
        }
    }
}
