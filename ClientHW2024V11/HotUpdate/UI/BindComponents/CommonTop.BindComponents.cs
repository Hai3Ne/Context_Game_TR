using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class CommonTop
	{
		private RectTransform m_Trans_Top;
		private Image m_Img_Vip;
		private Text m_Txt_VIP;
		private Text m_Txt_VipProcess;
		private RectTransform m_Rect_head;
		private Button m_Btn_head;
		private RectTransform m_Rect_Coin;
		private Image m_Img_coinBg;
		private Image m_Img_Coin;
		private Image m_Img_Coin2;
		private RectTransform m_Trans_Gold;
		private Text m_Txt_golds;
		private Button m_Btn_Plus;
		private Button m_Btn_Leave;
		private Image m_Img_Head;
		private Text m_Txt_ID;
		private TextMeshProUGUI m_TxtM_Money;
		private Image m_Img_BroadCoin2;
		private Button m_Btn_Rank;
		private RectTransform m_Trans_Rate;
		private Button m_Btn_Help;
		private RectTransform m_Trans_ScoreTitle;
		private Text m_Txt_Score;
		private Button m_Btn_Min;
		private Button m_Btn_Add;
		private Text m_Txt_Chips;
		private Toggle m_Tog_Tubo;
		private Button m_Btn_Max;
		private Toggle m_Tog_Auto;
		private RectTransform m_Rect_Mask;
		private Button m_Btn_BeginSpin;
		private RectTransform m_Trans_Rorate;
		private RectTransform m_Trans_Normal;
		private RectTransform m_Trans_Grey;
		private RectTransform m_Rect_tips;
		private RectTransform m_Trans_GoAutoSpinNum;
		private Text m_Txt_AutoSpinNum;
		private Button m_Btn_CloseAutoSpin;
		private RectTransform m_Trans_GoFreeTimes;
		private Text m_Txt_Times;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Img_Vip = autoBindTool.GetBindComponent<Image>(1);
			m_Txt_VIP = autoBindTool.GetBindComponent<Text>(2);
			m_Txt_VipProcess = autoBindTool.GetBindComponent<Text>(3);
			m_Rect_head = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Btn_head = autoBindTool.GetBindComponent<Button>(5);
			m_Rect_Coin = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Img_coinBg = autoBindTool.GetBindComponent<Image>(7);
			m_Img_Coin = autoBindTool.GetBindComponent<Image>(8);
			m_Img_Coin2 = autoBindTool.GetBindComponent<Image>(9);
			m_Trans_Gold = autoBindTool.GetBindComponent<RectTransform>(10);
			m_Txt_golds = autoBindTool.GetBindComponent<Text>(11);
			m_Btn_Plus = autoBindTool.GetBindComponent<Button>(12);
			m_Btn_Leave = autoBindTool.GetBindComponent<Button>(13);
			m_Img_Head = autoBindTool.GetBindComponent<Image>(14);
			m_Txt_ID = autoBindTool.GetBindComponent<Text>(15);
			m_TxtM_Money = autoBindTool.GetBindComponent<TextMeshProUGUI>(16);
			m_Img_BroadCoin2 = autoBindTool.GetBindComponent<Image>(17);
			m_Btn_Rank = autoBindTool.GetBindComponent<Button>(18);
			m_Trans_Rate = autoBindTool.GetBindComponent<RectTransform>(19);
			m_Btn_Help = autoBindTool.GetBindComponent<Button>(20);
			m_Trans_ScoreTitle = autoBindTool.GetBindComponent<RectTransform>(21);
			m_Txt_Score = autoBindTool.GetBindComponent<Text>(22);
			m_Btn_Min = autoBindTool.GetBindComponent<Button>(23);
			m_Btn_Add = autoBindTool.GetBindComponent<Button>(24);
			m_Txt_Chips = autoBindTool.GetBindComponent<Text>(25);
			m_Tog_Tubo = autoBindTool.GetBindComponent<Toggle>(26);
			m_Btn_Max = autoBindTool.GetBindComponent<Button>(27);
			m_Tog_Auto = autoBindTool.GetBindComponent<Toggle>(28);
			m_Rect_Mask = autoBindTool.GetBindComponent<RectTransform>(29);
			m_Btn_BeginSpin = autoBindTool.GetBindComponent<Button>(30);
			m_Trans_Rorate = autoBindTool.GetBindComponent<RectTransform>(31);
			m_Trans_Normal = autoBindTool.GetBindComponent<RectTransform>(32);
			m_Trans_Grey = autoBindTool.GetBindComponent<RectTransform>(33);
			m_Rect_tips = autoBindTool.GetBindComponent<RectTransform>(34);
			m_Trans_GoAutoSpinNum = autoBindTool.GetBindComponent<RectTransform>(35);
			m_Txt_AutoSpinNum = autoBindTool.GetBindComponent<Text>(36);
			m_Btn_CloseAutoSpin = autoBindTool.GetBindComponent<Button>(37);
			m_Trans_GoFreeTimes = autoBindTool.GetBindComponent<RectTransform>(38);
			m_Txt_Times = autoBindTool.GetBindComponent<Text>(39);
		}
	}
}
