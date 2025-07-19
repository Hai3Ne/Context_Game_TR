using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class ExchangePanel
	{
		private RectTransform m_Rect_bg;
		private Toggle m_Tog_Gift;
		private Toggle m_Tog_Card;
		private LoopGridView m_VGridScroll_DoublePackList;
		private Button m_Btn_Close;
		private RectTransform m_Rect_Tips;
		private Text m_Txt_CanExchangeNum;
		private Text m_Txt_CanExchangeTimes;
		private Text m_Txt_Tips;
		private Button m_Btn_TipsClose;
		private Image m_Img_Icon;
		private Image m_Img_gold;
		private Text m_Txt_lab1;
		private Text m_Txt_lab2;
		private Button m_Btn_duihuan;
		private Text m_Txt_ExchangeNum;
		private Text m_Txt_ItemNum;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_bg = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Tog_Gift = autoBindTool.GetBindComponent<Toggle>(1);
			m_Tog_Card = autoBindTool.GetBindComponent<Toggle>(2);
			m_VGridScroll_DoublePackList = autoBindTool.GetBindComponent<LoopGridView>(3);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(4);
			m_Rect_Tips = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Txt_CanExchangeNum = autoBindTool.GetBindComponent<Text>(6);
			m_Txt_CanExchangeTimes = autoBindTool.GetBindComponent<Text>(7);
			m_Txt_Tips = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_TipsClose = autoBindTool.GetBindComponent<Button>(9);
			m_Img_Icon = autoBindTool.GetBindComponent<Image>(10);
			m_Img_gold = autoBindTool.GetBindComponent<Image>(11);
			m_Txt_lab1 = autoBindTool.GetBindComponent<Text>(12);
			m_Txt_lab2 = autoBindTool.GetBindComponent<Text>(13);
			m_Btn_duihuan = autoBindTool.GetBindComponent<Button>(14);
			m_Txt_ExchangeNum = autoBindTool.GetBindComponent<Text>(15);
			m_Txt_ItemNum = autoBindTool.GetBindComponent<Text>(16);
		}
	}
}
