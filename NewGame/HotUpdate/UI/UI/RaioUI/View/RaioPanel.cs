using DG.Tweening;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{


    public partial class RaioPanel : PanelBase
    {
        [SerializeField] private List<GameObject> cellList = new List<GameObject>();
        [SerializeField] private RectTransform rtf;
        private Tweener tweenMove;
        private Tweener tweenBounce;
        private float startY = 0;
        private float offsetY = 0;
        private float maxY = 321.15f;
        private int times = 30;
        private bool isRolling = false;
        private float duration = 0.055f;


        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            startY = 0;
            SetUpPanel();
        }



        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }


        #region 事件绑定
        public void RegisterListener()
        {
            //m_Btn_Gire.onClick.AddListener(StartRoll);
            m_Btn_Home.onClick.AddListener(OnHomeBtn);
            m_Btn_Rank.onClick.AddListener(OnClickRankBtn);
        }

        public void UnRegisterListener()
        {
            //m_Btn_Gire.onClick.RemoveListener(StartRoll);
            m_Btn_Home.onClick.RemoveListener(OnHomeBtn);
            m_Btn_Rank.onClick.RemoveListener(OnClickRankBtn);
        }

        public void OnHomeBtn()
        {
            UICtrl.Instance.OpenView("MainUIPanel");
            //UICtrl.Instance.OpenView("RoomPanel");
        }

        private void OnClickRankBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < ZeusModel.Instance.awardList.Count; i++)
            {
                if (ZeusModel.Instance.awardList[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(ZeusModel.Instance.awardList[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList);
        }

        public void SetUpPanel()
        {
            m_TxtM_Golds.text = ToolUtil.ShowF2Num(MainUIModel.Instance.Golds);
            m_TxtM_Golds2.text = ToolUtil.ShowF2Num(MainUIModel.Instance.Golds);
        }
        #endregion



    }
}
