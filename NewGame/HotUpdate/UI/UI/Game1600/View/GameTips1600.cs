using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class GameTips1600 : PanelBase
    {
        private List<int> elementRate = new List<int> {2,3,5,10,50,100,200};
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            RegisterListener();
        }

        protected override void OnDisable()
        {
            UnRegisterListener();
        }

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(ClosePanel);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(ClosePanel);
        }

        public void OpenTips(int ele,int pos)
        {
            if (m_Img_LeftIcon.transform.childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(m_Img_LeftIcon.transform.GetChild(0).gameObject);
            if (m_Img_Icon.transform.childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(m_Img_Icon.transform.GetChild(0).gameObject);
            transform.gameObject.SetActive(true);
            transform.localPosition = new Vector3(0,-53,0);
            m_Trans_LeftGameTips.gameObject.SetActive(pos < 6);
            m_Trans_GameTips.gameObject.SetActive(pos >=6);

            m_Btn_Close.transform.localPosition = Game1600Model.Instance.gameTipsPos[pos];
            m_Trans_GameTips.localPosition = Game1600Model.Instance.gameTipsPos[pos];
            m_Trans_LeftGameTips.localPosition = Game1600Model.Instance.gameTipsPos[pos];
            m_Txt_Count.text = "3";
            m_Txt_Rate.text = elementRate[ele - 1].ToString();
            m_Img_Icon.sprite = AtlasSpriteManager.Instance.GetSprite("Game1600:" + "Tb" + ele);
            m_Img_Icon.SetNativeSize();

            m_Txt_LeftCount.text = "3";
            m_Txt_LeftRate.text = elementRate[ele - 1].ToString();
            m_Img_LeftIcon.sprite = AtlasSpriteManager.Instance.GetSprite("Game1600:" + "Tb" + ele);
            m_Img_LeftIcon.SetNativeSize();
            GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/Tb" + ele);
            go.transform.SetParent(pos < 6? m_Img_LeftIcon.transform: m_Img_Icon.transform, true);
            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(0.65f, 0.65f, 1);
            go.transform.localPosition = new Vector3(6, 0, 0);
            string aniName = "spawn";
            if (ele == 7)
                aniName = "idle";
            ToolUtil.PlayAnimation(go.transform, aniName, false);
        }

        public void ClosePanel()
        {
            transform.gameObject.SetActive(false);

        }
    }
}
