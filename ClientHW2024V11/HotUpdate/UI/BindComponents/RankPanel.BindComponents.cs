using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class RankPanel
	{
		private RectTransform m_Rect_bg;
		private Toggle m_Tog_Page1;
		private Toggle m_Tog_Page2;
		private Toggle m_Tog_Page3;
		private LoopGridView m_VGridScroll_RankList;
		private Button m_Btn_LastRank;
		private Text m_Txt_lastBtnTxt;
		private Image m_Img_head;
		private Text m_Txt_MyRankTxt;
		private Text m_Txt_MyConsume;
		private Text m_Txt_MyName;
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_bg = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Tog_Page1 = autoBindTool.GetBindComponent<Toggle>(1);
			m_Tog_Page2 = autoBindTool.GetBindComponent<Toggle>(2);
			m_Tog_Page3 = autoBindTool.GetBindComponent<Toggle>(3);
			m_VGridScroll_RankList = autoBindTool.GetBindComponent<LoopGridView>(4);
			m_Btn_LastRank = autoBindTool.GetBindComponent<Button>(5);
			m_Txt_lastBtnTxt = autoBindTool.GetBindComponent<Text>(6);
			m_Img_head = autoBindTool.GetBindComponent<Image>(7);
			m_Txt_MyRankTxt = autoBindTool.GetBindComponent<Text>(8);
			m_Txt_MyConsume = autoBindTool.GetBindComponent<Text>(9);
			m_Txt_MyName = autoBindTool.GetBindComponent<Text>(10);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(11);
		}
	}
}
