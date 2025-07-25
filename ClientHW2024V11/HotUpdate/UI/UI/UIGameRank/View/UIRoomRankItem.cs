using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public class UIRoomRankItem : MonoBehaviour
    {
        Vector3 initPos = new Vector3(0,-1.1f,0);
        Transform Info;
        public Transform RateParent;
        protected Image ImgHead;
        protected Text TxtUid;
        protected Text TxtGold;
        private void Awake()
        {
            Info = transform.Find("Info");
            ImgHead = transform.Find("Head").GetComponent<Image>();
            TxtUid = Info.Find("TxtUid").GetComponent<Text>();
            TxtGold = Info.Find("TxtGold").GetComponent<Text>();
            RateParent = transform.Find("RateParent");
        }

        public virtual void InitData(RankItem item,int gameID = 0)
        {
            gameObject.SetActive(true);
            TxtUid.text = CommonTools.BytesToString(item.szName);
           // Debug.LogError("--------------"+ item.nIconID);
           // TxtUid.text = "U" + item.n64RoleID + "";
            TxtGold.text = ToolUtil.ShowF2Num2(item.n64Gold);
            string imgurl = "" + item.nIconID;
            ImgHead.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
            Info.localPosition = initPos;

            RateParent.gameObject.SetActive(gameID == 1100);
            if (gameID == 1100)
            {
                if(RateParent.childCount <= 0)
                {
                    GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1100/FirstRes/Rank");
                    go.transform.SetParent(RateParent, true);
                    //CommonTools.PlayArmatureAni(go.transform, row1 < 0 ? "a0" : "a1", 0, () => { });
                    go.transform.localScale = new Vector3(1, 1, 1);
                    go.transform.localPosition = new Vector3(0, 0, 0);
                }
                RateParent.GetChild(0).GetChild(1).GetComponent<Text>().text = item.nRate + "X";
            }
        }

    }
}