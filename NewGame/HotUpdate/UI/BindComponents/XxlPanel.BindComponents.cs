using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class XxlPanel
	{
		private RectTransform m_Rect_bg;
		private LoopListView2 m_VListScroll_View;
		private RectTransform m_Rect_Mine;
		private RectTransform m_Trans_Top;
		private Image m_Img_top1;
		private Image m_Img_top2;
		private Text m_Txt_UserName;
		private Text m_Txt_Uid;
		private RectTransform m_Rect_GoldBgImage;
		private Text m_Txt_GoldNum;
		private Button m_Btn_Gold;
		private Button m_Btn_Head;
		private Image m_Img_Icon;
		private RectTransform m_Trans_Bottom1;
		private Button m_Btn_Shop1;
		private Button m_Btn_Notice1;
		private Button m_Btn_FirstCharge1;
		private Button m_Btn_NoviceTask1;
		private Button m_Btn_Close;
		private Button m_Btn_Guan1;
		private Button m_Btn_Guan2;
		private Button m_Btn_Guan3;
		private Button m_Btn_Guan4;
		private Button m_Btn_Guan5;
		private Button m_Btn_Guan6;
		private Button m_Btn_Guan7;
		private Button m_Btn_Guan8;
		private Button m_Btn_Guan9;
		private Button m_Btn_Guan10;
		private Button m_Btn_Guan11;
		private Button m_Btn_Guan12;
		private Button m_Btn_Guan13;
		private Button m_Btn_Guan14;
		private Button m_Btn_Guan15;
		private RectTransform m_Rect_Mask;
		private RectTransform m_Rect_Over;
		private Text m_Txt_Level2;
		private Button m_Btn_DestroyOver;
		private Button m_Btn_Restart;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_bg = autoBindTool.GetBindComponent<RectTransform>(0);
			m_VListScroll_View = autoBindTool.GetBindComponent<LoopListView2>(1);
			m_Rect_Mine = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Img_top1 = autoBindTool.GetBindComponent<Image>(4);
			m_Img_top2 = autoBindTool.GetBindComponent<Image>(5);
			m_Txt_UserName = autoBindTool.GetBindComponent<Text>(6);
			m_Txt_Uid = autoBindTool.GetBindComponent<Text>(7);
			m_Rect_GoldBgImage = autoBindTool.GetBindComponent<RectTransform>(8);
			m_Txt_GoldNum = autoBindTool.GetBindComponent<Text>(9);
			m_Btn_Gold = autoBindTool.GetBindComponent<Button>(10);
			m_Btn_Head = autoBindTool.GetBindComponent<Button>(11);
			m_Img_Icon = autoBindTool.GetBindComponent<Image>(12);
			m_Trans_Bottom1 = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Btn_Shop1 = autoBindTool.GetBindComponent<Button>(14);
			m_Btn_Notice1 = autoBindTool.GetBindComponent<Button>(15);
			m_Btn_FirstCharge1 = autoBindTool.GetBindComponent<Button>(16);
			m_Btn_NoviceTask1 = autoBindTool.GetBindComponent<Button>(17);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(18);
			m_Btn_Guan1 = autoBindTool.GetBindComponent<Button>(19);
			m_Btn_Guan2 = autoBindTool.GetBindComponent<Button>(20);
			m_Btn_Guan3 = autoBindTool.GetBindComponent<Button>(21);
			m_Btn_Guan4 = autoBindTool.GetBindComponent<Button>(22);
			m_Btn_Guan5 = autoBindTool.GetBindComponent<Button>(23);
			m_Btn_Guan6 = autoBindTool.GetBindComponent<Button>(24);
			m_Btn_Guan7 = autoBindTool.GetBindComponent<Button>(25);
			m_Btn_Guan8 = autoBindTool.GetBindComponent<Button>(26);
			m_Btn_Guan9 = autoBindTool.GetBindComponent<Button>(27);
			m_Btn_Guan10 = autoBindTool.GetBindComponent<Button>(28);
			m_Btn_Guan11 = autoBindTool.GetBindComponent<Button>(29);
			m_Btn_Guan12 = autoBindTool.GetBindComponent<Button>(30);
			m_Btn_Guan13 = autoBindTool.GetBindComponent<Button>(31);
			m_Btn_Guan14 = autoBindTool.GetBindComponent<Button>(32);
			m_Btn_Guan15 = autoBindTool.GetBindComponent<Button>(33);
			m_Rect_Mask = autoBindTool.GetBindComponent<RectTransform>(34);
			m_Rect_Over = autoBindTool.GetBindComponent<RectTransform>(35);
			m_Txt_Level2 = autoBindTool.GetBindComponent<Text>(36);
			m_Btn_DestroyOver = autoBindTool.GetBindComponent<Button>(37);
			m_Btn_Restart = autoBindTool.GetBindComponent<Button>(38);
		}
	}
}
