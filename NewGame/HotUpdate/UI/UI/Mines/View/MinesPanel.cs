using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public partial class MinesPanel : PanelBase
    {
        private List<GameObject> minesBets = new List<GameObject>();
        [SerializeField] private List<MinesItem> minesItems = new List<MinesItem>();
        private GoldEffectNew m_Gold_Effect;
        private int Rows = 5;
        private int Cols = 5;
        private DragonBones.UnityArmatureComponent m_Dragon_OpenBox;
        private DragonBones.UnityArmatureComponent m_Dragon_showCoin;
        private DragonBones.UnityArmatureComponent m_Dragon_Bomb;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            minesBets.Add(m_Btn_Bet1.gameObject);
            minesBets.Add(m_Btn_Bet2.gameObject);
            minesBets.Add(m_Btn_Bet3.gameObject);
            minesBets.Add(m_Btn_Bet4.gameObject);
            MinesModel.Instance.minesItems = minesItems;
            //MinesModel.Instance.InitMinesData();
            
        }
        protected override void Start()
        {
            base.Start();
            InitJackPotData();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            SetUpPanel();
            CoreEntry.gAudioMgr.PlayUIMusic(127);

        }



        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyUp(KeyCode.A))
            {
                PlayAnimatior(0);
                //CoreEntry.gAudioMgr.PlayUISound(131);
                //MinesModel.Instance.goldEffect.setData(2, 1000, () => { Debug.Log("ttttttttttttttttttttttt"); },true);
            } 
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Home.onClick.AddListener(OnHomeBtn);
            m_Btn_Shop.onClick.AddListener(OpenShopPanel);
            m_Btn_Bet1.onClick.AddListener(delegate { OnBetBtn(0); });
            m_Btn_Bet2.onClick.AddListener(delegate { OnBetBtn(1); });
            m_Btn_Bet3.onClick.AddListener(delegate { OnBetBtn(2); });
            m_Btn_Bet4.onClick.AddListener(delegate { OnBetBtn(3); });
            m_Btn_Multiply2.onClick.AddListener(OnMultiplyBtn);
            m_Btn_PlusTen.onClick.AddListener(OnPlusBtn);
            m_Btn_half.onClick.AddListener(OnHalfBtn);
            m_Btn_reduceTen.onClick.AddListener(OnReduceBtn);
            m_Btn_start.onClick.AddListener(StartGame);
        }

        public void UnRegisterListener()
        {
            m_Btn_Home.onClick.RemoveListener(OnHomeBtn);
            m_Btn_Shop.onClick.RemoveListener(OpenShopPanel);
            m_Btn_Bet1.onClick.RemoveListener(delegate { OnBetBtn(0); });
            m_Btn_Bet2.onClick.RemoveListener(delegate { OnBetBtn(1); });
            m_Btn_Bet3.onClick.RemoveListener(delegate { OnBetBtn(2); });
            m_Btn_Bet4.onClick.RemoveListener(delegate { OnBetBtn(3); });
            m_Btn_Multiply2.onClick.RemoveListener(OnMultiplyBtn);
            m_Btn_PlusTen.onClick.RemoveListener(OnPlusBtn);
            m_Btn_half.onClick.RemoveListener(OnHalfBtn);
            m_Btn_reduceTen.onClick.RemoveListener(OnReduceBtn);
            m_Btn_start.onClick.RemoveListener(StartGame);
            CoreEntry.gAudioMgr.StopMusic(127);
        }
        #endregion
        public void SetUpPanel() 
        {
            MinesModel.Instance.Bet = 1;
            MinesModel.Instance.BombCount = 1;
            m_TxtM_Bet.text = MinesModel.Instance.Bet.ToString();
            m_TxtM_Golds.text = ToolUtil.ShowF2Num(MainUIModel.Instance.Golds);
            SetUpBetBtn(0);
            CoreEntry.gAudioMgr.PlayUISound(131);
            CoreEntry.gAudioMgr.PlayUISound(131);
            PlayAnimatior(0);
        }
        public void OnReduceBtn()
        {
            if (MinesModel.Instance.minesState == MinesState.Gameing)
            {
                return;
            }
            CoreEntry.gAudioMgr.PlayUISound(130);
            var num = MinesModel.Instance.Bet - 10>0 ? MinesModel.Instance.Bet - 10:1;
            MinesModel.Instance.Bet = num;
            m_TxtM_Bet.text = num.ToString();
        }

        public void OnHalfBtn() 
        {
            if (MinesModel.Instance.minesState == MinesState.Gameing)
            {
                return;
            }
            CoreEntry.gAudioMgr.PlayUISound(130);
            var num = MinesModel.Instance.Bet / 2>=1? Mathf.FloorToInt(MinesModel.Instance.Bet / 2):1 ;
            MinesModel.Instance.Bet = num;
            m_TxtM_Bet.text = num.ToString();
        }
        public void OnPlusBtn() 
        {
            if (MinesModel.Instance.minesState == MinesState.Gameing)
            {
                return;
            }
            CoreEntry.gAudioMgr.PlayUISound(130);
            var num = MinesModel.Instance.Bet + 10 < (MainUIModel.Instance.Golds/ ToolUtil.GetGoldRadio()) ? MinesModel.Instance.Bet + 10 : (MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio());
            MinesModel.Instance.Bet = (int)num;
            m_TxtM_Bet.text = num.ToString();
        }
        public void OnMultiplyBtn()
        {
            if (MinesModel.Instance.minesState == MinesState.Gameing)
            {
                return;
            }
            CoreEntry.gAudioMgr.PlayUISound(130);
            var num = MinesModel.Instance.Bet * 2 < (MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) ? MinesModel.Instance.Bet * 2 : (MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio());
            MinesModel.Instance.Bet = (int)num;
            m_TxtM_Bet.text = num.ToString();
        }
        public void OpenShopPanel() 
        {
            if (MinesModel.Instance.minesState == MinesState.Gameing)
            {
                return;
            }
            MainUICtrl.Instance.OpenShopPanel();
        }
        public void OnHomeBtn()
        {
            if (MinesModel.Instance.minesState== MinesState.Gameing)
            {
                return;
            }
            CoreEntry.gAudioMgr.PlayUISound(130);
            UICtrl.Instance.OpenView("RoomPanel");
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

        private void OnBetBtn(int index) 
        {
            if (MinesModel.Instance.minesState == MinesState.Gameing)
            {
                return;
            }
            CoreEntry.gAudioMgr.PlayUISound(130);
            switch (index) 
            {
                case 0:
                    SetUpBetBtn(0);
                    MinesModel.Instance.BombCount = 1;
                    break;
                case 1:
                    SetUpBetBtn(1);
                    MinesModel.Instance.BombCount = 2;
                    break;
                case 2:
                    SetUpBetBtn(2);
                    MinesModel.Instance.BombCount = 3;
                    break;
                case 3:
                    SetUpBetBtn(3);
                    MinesModel.Instance.BombCount = 4;
                    break;
            }
        }

        private void SetUpBetBtn(int index) 
        {
            for (int i = 0; i < minesBets.Count; i++)
            {
                if (i==index)
                {
                    minesBets[i].GetComponent<MinesBetItem>().Select();
                }
                else
                {
                    minesBets[i].GetComponent<MinesBetItem>().Hide();
                }

            }
        }

        public void GetAni() 
        {
            if (m_Dragon_OpenBox==null)
            {
                var box = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Mines/FirstRes/Dragon_OpenBox") as GameObject;
                box.transform.SetParent(transform);
                box.SetActive(false);
                m_Dragon_OpenBox = box.GetComponent<DragonBones.UnityArmatureComponent>();
                m_Dragon_OpenBox.animation.Stop();
            }
            if (m_Dragon_showCoin == null)
            {
                var coin = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Mines/FirstRes/Dragon_showCoin") as GameObject;
                coin.transform.SetParent(transform);
                coin.SetActive(false);
                m_Dragon_showCoin = coin.GetComponent<DragonBones.UnityArmatureComponent>();
                m_Dragon_showCoin.animation.Stop();
            }
            if (m_Dragon_Bomb == null)
            {
                var bomb = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Mines/FirstRes/Dragon_Bomb") as GameObject;
                bomb.transform.SetParent(transform);
                bomb.SetActive(false);
                m_Dragon_Bomb = bomb.GetComponent<DragonBones.UnityArmatureComponent>();
                m_Dragon_Bomb.animation.Stop();
            }
        }


        public void PlayAnimatior(int index)
        {
            minesItems[index].transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.3f).OnStart(delegate
            {
                GetAni();
                minesItems[index].CloneAni(m_Dragon_OpenBox, m_Dragon_showCoin,m_Dragon_Bomb);
                minesItems[index].transform.DOScale(Vector3.one, 0.3f);
                PlayAnimatior(index + 1);
            });
        }

        /// <summary>
        /// 随机炸弹位置，多个不重复
        /// </summary>
        public void SetMines() 
        {
            List<int> randoms = new List<int>();
            for (int i = 0; i < MinesModel.Instance.minesPairs[MinesModel.Instance.BombCount].Count; i++)
            {
                MinesModel.Instance.minesItems[i].SetUpItem(MinesModel.Instance.minesPairs[MinesModel.Instance.BombCount][i]);
            }
            for (int i = 0; i < MinesModel.Instance.BombCount; i++)
            {
                int r = UnityEngine.Random.Range(0, minesItems.Count - 1);
                while (randoms.Contains(r))
                {
                    r = (r + 1) % minesItems.Count;
                }
                randoms.Add(r);
            }
            for (int i = 0; i < MinesModel.Instance.BombCount; i++)
            {
                minesItems[randoms[i]].SetState(1);
                Debug.LogError($"{randoms[i]}");
            }
            MinesModel.Instance.SetPlusGolds(MinesModel.Instance.Bet);
            MinesModel.Instance.SetNextGolds(MinesModel.Instance.Bet + MinesModel.Instance.minesItems[0].Amount);
            MinesModel.Instance.minesState = MinesState.Gameing;
        }
        /// <summary>
        /// 下注初始化炸弹位置
        /// </summary>
        public void StartGame() 
        {
            CoreEntry.gAudioMgr.PlayUISound(131);
            PlayAnimatior(0);
            CoreEntry.gAudioMgr.PlayUISound(130);
            MinesModel.Instance.InitMinesData();
            MinesModel.Instance.textAction = SetUpPanelText;
            //PlayAnimatior(0);
            switch (MinesModel.Instance.minesState)
            {
                case MinesState.None:
                    MinesModel.Instance.minesState = MinesState.Start;
                    MinesModel.Instance.minesItems.ForEach(x => x.Reset());
                    SetMines();
                    break;
                case MinesState.Start:
                    break;
                case MinesState.Gameing:
                    MinesModel.Instance.minesState = MinesState.GameOver;
                    MinesModel.Instance.goldEffect.setData(3, (long)(MinesModel.Instance.PlusGolds * ToolUtil.GetGoldRadio()), null, false);
                    break;
                case MinesState.GameOver:
                    MinesModel.Instance.minesState = MinesState.Start;
                    MinesModel.Instance.minesItems.ForEach(x => x.Reset());
                    MinesModel.Instance.PlusGolds=0;
                    MinesModel.Instance.NextGolds=0;
                    MinesModel.Instance.GridCount = 25;
                    SetMines();
                    SetUpPanelText();
                    break;
                case MinesState.Settlement:
                    break;

            }
        }

        public void InitJackPotData() 
        {
            var obj = CoreEntry.gResLoader.ClonePre("UI/UITemplate/Gold_EffectNew", m_Rect_Effect.transform, false, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = m_Rect_Effect.sizeDelta;
            rect.anchoredPosition3D = new Vector3(0, 0, 88);
            rect.localScale = Vector3.one;
            m_Gold_Effect = obj.GetComponent<GoldEffectNew>();
            MinesModel.Instance.goldEffect = m_Gold_Effect;
        }

        public void ResetGame() 
        {
            MinesModel.Instance.minesState = MinesState.Start;
            MinesModel.Instance.minesItems.ForEach(x => x.Reset());
            MinesModel.Instance.SetPlusGolds(0);
            MinesModel.Instance.SetNextGolds(0);
            MinesModel.Instance.GridCount = 25;
            m_Txt_Valor1.text = $"0.00X";
            m_Txt_Valor2.text = $"R$ 0.00";
            m_Txt_Proxima1.text = $"R$ 0.00";
            m_Txt_Proxima2.text = $"R$ 0.00";
        }

        public void SetUpPanelText()
        {
            m_Txt_Valor1.text = $"{MinesModel.Instance.PlusGolds.ToString("F2", new CultureInfo("en"))}X";
            m_Txt_Valor2.text = $"R$ {MinesModel.Instance.PlusGolds.ToString("F2", new CultureInfo("en"))}";
            m_Txt_Proxima1.text = $"R$ {MinesModel.Instance.NextGolds.ToString("F2", new CultureInfo("en"))}";
            m_Txt_Proxima2.text = $"R$ {MinesModel.Instance.NextGolds.ToString("F2", new CultureInfo("en"))}";
        }

        public void Shuffle<T>(List<T> array)
        {
            System.Random random = new System.Random();
            for (int i = array.Count; i > 1; i--)
            {
                int j = random.Next(i);
                T temp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = temp;
            }
        }
    }
} 
  