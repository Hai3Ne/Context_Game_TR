using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class MainUIPanel
	{
		private RectTransform m_Trans_Panel;
		private Button m_Btn_FirstCharge;
		private Button m_Btn_Wealth;
		private Button m_Btn_TourRank;
		private RectTransform m_Trans_Right;
		private Button m_Btn_NoviceTask;
		private UnityArmatureComponent m_Dragon_signRedDot;
		private Button m_Btn_Wish;
		private Button m_Btn_Notice;
		private RectTransform m_Trans_Top;
		private Image m_Img_top1;
		private Image m_Img_top2;
		private Button m_Btn_Head;
		private Image m_Img_Icon;
		private Text m_Txt_Uid;
		private Text m_Txt_UserName;
		private RectTransform m_Rect_CoinBgImage;
		private Text m_Txt_CoinNum;
		private Button m_Btn_Coin;
		private RectTransform m_Rect_GoldBgImage;
		private Text m_Txt_GoldNum;
		private Button m_Btn_Gold;
		private RectTransform m_Trans_Guide;
		private RectTransform m_Rect_TipsBgImage1;
		private Button m_Btn_Game1;
		private RectTransform m_Rect_TipsBgImage2;
		private Button m_Btn_Game2;
		private RectTransform m_Rect_TipsBgImage3;
		private Button m_Btn_Game3;
		private RectTransform m_Trans_Bottom;
		private Button m_Btn_Mail;
		private UnityArmatureComponent m_Dragon_mailRedDot;
		private Button m_Btn_Task;
		private UnityArmatureComponent m_Dragon_taskRedDot;
		private Button m_Btn_Tournament;
		private Button m_Btn_Rank;
		private Button m_Btn_Shop;
		private RectTransform m_Trans_Bottom1;
		private Button m_Btn_Shop1;
		private Button m_Btn_FirstCharge1;
		private Button m_Btn_Notice1;
		private Button m_Btn_NoviceTask1;
		private Image m_Img_GuideMask;
		private RectTransform m_Trans_Guide1;
		private RectTransform m_Rect_Tap;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Panel = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Btn_FirstCharge = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Wealth = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_TourRank = autoBindTool.GetBindComponent<Button>(3);
			m_Trans_Right = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Btn_NoviceTask = autoBindTool.GetBindComponent<Button>(5);
			m_Dragon_signRedDot = autoBindTool.GetBindComponent<UnityArmatureComponent>(6);
			m_Btn_Wish = autoBindTool.GetBindComponent<Button>(7);
			m_Btn_Notice = autoBindTool.GetBindComponent<Button>(8);
			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(9);
			m_Img_top1 = autoBindTool.GetBindComponent<Image>(10);
			m_Img_top2 = autoBindTool.GetBindComponent<Image>(11);
			m_Btn_Head = autoBindTool.GetBindComponent<Button>(12);
			m_Img_Icon = autoBindTool.GetBindComponent<Image>(13);
			m_Txt_Uid = autoBindTool.GetBindComponent<Text>(14);
			m_Txt_UserName = autoBindTool.GetBindComponent<Text>(15);
			m_Rect_CoinBgImage = autoBindTool.GetBindComponent<RectTransform>(16);
			m_Txt_CoinNum = autoBindTool.GetBindComponent<Text>(17);
			m_Btn_Coin = autoBindTool.GetBindComponent<Button>(18);
			m_Rect_GoldBgImage = autoBindTool.GetBindComponent<RectTransform>(19);
			m_Txt_GoldNum = autoBindTool.GetBindComponent<Text>(20);
			m_Btn_Gold = autoBindTool.GetBindComponent<Button>(21);
			m_Trans_Guide = autoBindTool.GetBindComponent<RectTransform>(22);
			m_Rect_TipsBgImage1 = autoBindTool.GetBindComponent<RectTransform>(23);
			m_Btn_Game1 = autoBindTool.GetBindComponent<Button>(24);
			m_Rect_TipsBgImage2 = autoBindTool.GetBindComponent<RectTransform>(25);
			m_Btn_Game2 = autoBindTool.GetBindComponent<Button>(26);
			m_Rect_TipsBgImage3 = autoBindTool.GetBindComponent<RectTransform>(27);
			m_Btn_Game3 = autoBindTool.GetBindComponent<Button>(28);
			m_Trans_Bottom = autoBindTool.GetBindComponent<RectTransform>(29);
			m_Btn_Mail = autoBindTool.GetBindComponent<Button>(30);
			m_Dragon_mailRedDot = autoBindTool.GetBindComponent<UnityArmatureComponent>(31);
			m_Btn_Task = autoBindTool.GetBindComponent<Button>(32);
			m_Dragon_taskRedDot = autoBindTool.GetBindComponent<UnityArmatureComponent>(33);
			m_Btn_Tournament = autoBindTool.GetBindComponent<Button>(34);
			m_Btn_Rank = autoBindTool.GetBindComponent<Button>(35);
			m_Btn_Shop = autoBindTool.GetBindComponent<Button>(36);
			m_Trans_Bottom1 = autoBindTool.GetBindComponent<RectTransform>(37);
			m_Btn_Shop1 = autoBindTool.GetBindComponent<Button>(38);
			m_Btn_FirstCharge1 = autoBindTool.GetBindComponent<Button>(39);
			m_Btn_Notice1 = autoBindTool.GetBindComponent<Button>(40);
			m_Btn_NoviceTask1 = autoBindTool.GetBindComponent<Button>(41);
			m_Img_GuideMask = autoBindTool.GetBindComponent<Image>(42);
			m_Trans_Guide1 = autoBindTool.GetBindComponent<RectTransform>(43);
			m_Rect_Tap = autoBindTool.GetBindComponent<RectTransform>(44);
		}
	}
}
