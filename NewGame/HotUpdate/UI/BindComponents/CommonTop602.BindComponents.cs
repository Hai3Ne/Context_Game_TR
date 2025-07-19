using TMPro;
using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class CommonTop602
	{
		private RectTransform m_Trans_Top;
		private Image m_Img_Vip;
		private Text m_Txt_VIP;
		private Text m_Txt_VipProcess;
		private RectTransform m_Rect_head;
		private Button m_Btn_head;
		private Image m_Img_Head;
		private TextMeshProUGUI m_TxtM_ID;
		private TextMeshProUGUI m_TxtM_Money;
		private Image m_Img_BroadCoin2;
		private Button m_Btn_Rank;
		private RectTransform m_Trans_Rate;
		private Button m_Btn_Help2;
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
		private Button m_Btn_biwu;
		private Button m_Btn_GameRank;
		private Button m_Btn_Task;
		private UnityArmatureComponent m_Dragon_taskRedDot;
		private Button m_Btn_Feng;
		private Button m_Btn_Help;
		private Button m_Btn_Leave;
		private RectTransform m_Rect_Coin;
		private Button m_Btn_Plus;
		private Text m_Txt_golds;
		private RectTransform m_Trans_Tips;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Img_Vip = autoBindTool.GetBindComponent<Image>(1);
			m_Txt_VIP = autoBindTool.GetBindComponent<Text>(2);
			m_Txt_VipProcess = autoBindTool.GetBindComponent<Text>(3);
			m_Rect_head = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Btn_head = autoBindTool.GetBindComponent<Button>(5);
			m_Img_Head = autoBindTool.GetBindComponent<Image>(6);
			m_TxtM_ID = autoBindTool.GetBindComponent<TextMeshProUGUI>(7);
			m_TxtM_Money = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
			m_Img_BroadCoin2 = autoBindTool.GetBindComponent<Image>(9);
			m_Btn_Rank = autoBindTool.GetBindComponent<Button>(10);
			m_Trans_Rate = autoBindTool.GetBindComponent<RectTransform>(11);
			m_Btn_Help2 = autoBindTool.GetBindComponent<Button>(12);
			m_Trans_ScoreTitle = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Txt_Score = autoBindTool.GetBindComponent<Text>(14);
			m_Btn_Min = autoBindTool.GetBindComponent<Button>(15);
			m_Btn_Add = autoBindTool.GetBindComponent<Button>(16);
			m_Txt_Chips = autoBindTool.GetBindComponent<Text>(17);
			m_Tog_Tubo = autoBindTool.GetBindComponent<Toggle>(18);
			m_Btn_Max = autoBindTool.GetBindComponent<Button>(19);
			m_Tog_Auto = autoBindTool.GetBindComponent<Toggle>(20);
			m_Rect_Mask = autoBindTool.GetBindComponent<RectTransform>(21);
			m_Btn_BeginSpin = autoBindTool.GetBindComponent<Button>(22);
			m_Trans_Rorate = autoBindTool.GetBindComponent<RectTransform>(23);
			m_Trans_Normal = autoBindTool.GetBindComponent<RectTransform>(24);
			m_Trans_Grey = autoBindTool.GetBindComponent<RectTransform>(25);
			m_Rect_tips = autoBindTool.GetBindComponent<RectTransform>(26);
			m_Trans_GoAutoSpinNum = autoBindTool.GetBindComponent<RectTransform>(27);
			m_Txt_AutoSpinNum = autoBindTool.GetBindComponent<Text>(28);
			m_Btn_CloseAutoSpin = autoBindTool.GetBindComponent<Button>(29);
			m_Trans_GoFreeTimes = autoBindTool.GetBindComponent<RectTransform>(30);
			m_Txt_Times = autoBindTool.GetBindComponent<Text>(31);
			m_Btn_biwu = autoBindTool.GetBindComponent<Button>(32);
			m_Btn_GameRank = autoBindTool.GetBindComponent<Button>(33);
			m_Btn_Task = autoBindTool.GetBindComponent<Button>(34);
			m_Dragon_taskRedDot = autoBindTool.GetBindComponent<UnityArmatureComponent>(35);
			m_Btn_Feng = autoBindTool.GetBindComponent<Button>(36);
			m_Btn_Help = autoBindTool.GetBindComponent<Button>(37);
			m_Btn_Leave = autoBindTool.GetBindComponent<Button>(38);
			m_Rect_Coin = autoBindTool.GetBindComponent<RectTransform>(39);
			m_Btn_Plus = autoBindTool.GetBindComponent<Button>(40);
			m_Txt_golds = autoBindTool.GetBindComponent<Text>(41);
			m_Trans_Tips = autoBindTool.GetBindComponent<RectTransform>(42);
		}
	}
}
