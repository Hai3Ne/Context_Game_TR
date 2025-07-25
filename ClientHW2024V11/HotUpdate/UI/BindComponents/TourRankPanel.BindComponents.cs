using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class TourRankPanel
	{
		private Button m_Btn_Close;
		private Text m_Txt_MyHeart;
		private Text m_Txt_CountTime;
		private Button m_Btn_rule;
		private Toggle m_Tog_Options1;
		private Text m_Txt_PageRewardTxt1;
		private Text m_Txt_PageTxt1;
		private Toggle m_Tog_Options2;
		private Text m_Txt_PageRewardTxt2;
		private Text m_Txt_PageTxt2;
		private Toggle m_Tog_Options3;
		private Text m_Txt_PageRewardTxt3;
		private Text m_Txt_PageTxt3;
		private Toggle m_Tog_Options4;
		private Text m_Txt_PageRewardTxt4;
		private Text m_Txt_PageTxt4;
		private Toggle m_Tog_Options5;
		private Text m_Txt_PageRewardTxt5;
		private Text m_Txt_PageTxt5;
		private Toggle m_Tog_Options6;
		private Text m_Txt_PageRewardTxt6;
		private Text m_Txt_PageTxt6;
		private Toggle m_Tog_Options7;
		private Text m_Txt_PageRewardTxt7;
		private Text m_Txt_PageTxt7;
		private Image m_Img_ProessBg1;
		private RectTransform m_Trans_Effect1;
		private Image m_Img_ProessBg2;
		private RectTransform m_Trans_Effect2;
		private Image m_Img_ProessBg3;
		private RectTransform m_Trans_Effect3;
		private Image m_Img_ProessBg4;
		private RectTransform m_Trans_Effect4;
		private Image m_Img_ProessBg5;
		private RectTransform m_Trans_Effect5;
		private Image m_Img_ProessBg6;
		private RectTransform m_Trans_Effect6;
		private Image m_Img_ProessBg7;
		private RectTransform m_Trans_Effect7;
		private Toggle m_Tog_Select;
		private LoopGridView m_VGridScroll_TournamentList;
		private Text m_Txt_Num;
		private Text m_Txt_MyRank;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Txt_MyHeart = autoBindTool.GetBindComponent<Text>(1);
			m_Txt_CountTime = autoBindTool.GetBindComponent<Text>(2);
			m_Btn_rule = autoBindTool.GetBindComponent<Button>(3);
			m_Tog_Options1 = autoBindTool.GetBindComponent<Toggle>(4);
			m_Txt_PageRewardTxt1 = autoBindTool.GetBindComponent<Text>(5);
			m_Txt_PageTxt1 = autoBindTool.GetBindComponent<Text>(6);
			m_Tog_Options2 = autoBindTool.GetBindComponent<Toggle>(7);
			m_Txt_PageRewardTxt2 = autoBindTool.GetBindComponent<Text>(8);
			m_Txt_PageTxt2 = autoBindTool.GetBindComponent<Text>(9);
			m_Tog_Options3 = autoBindTool.GetBindComponent<Toggle>(10);
			m_Txt_PageRewardTxt3 = autoBindTool.GetBindComponent<Text>(11);
			m_Txt_PageTxt3 = autoBindTool.GetBindComponent<Text>(12);
			m_Tog_Options4 = autoBindTool.GetBindComponent<Toggle>(13);
			m_Txt_PageRewardTxt4 = autoBindTool.GetBindComponent<Text>(14);
			m_Txt_PageTxt4 = autoBindTool.GetBindComponent<Text>(15);
			m_Tog_Options5 = autoBindTool.GetBindComponent<Toggle>(16);
			m_Txt_PageRewardTxt5 = autoBindTool.GetBindComponent<Text>(17);
			m_Txt_PageTxt5 = autoBindTool.GetBindComponent<Text>(18);
			m_Tog_Options6 = autoBindTool.GetBindComponent<Toggle>(19);
			m_Txt_PageRewardTxt6 = autoBindTool.GetBindComponent<Text>(20);
			m_Txt_PageTxt6 = autoBindTool.GetBindComponent<Text>(21);
			m_Tog_Options7 = autoBindTool.GetBindComponent<Toggle>(22);
			m_Txt_PageRewardTxt7 = autoBindTool.GetBindComponent<Text>(23);
			m_Txt_PageTxt7 = autoBindTool.GetBindComponent<Text>(24);
			m_Img_ProessBg1 = autoBindTool.GetBindComponent<Image>(25);
			m_Trans_Effect1 = autoBindTool.GetBindComponent<RectTransform>(26);
			m_Img_ProessBg2 = autoBindTool.GetBindComponent<Image>(27);
			m_Trans_Effect2 = autoBindTool.GetBindComponent<RectTransform>(28);
			m_Img_ProessBg3 = autoBindTool.GetBindComponent<Image>(29);
			m_Trans_Effect3 = autoBindTool.GetBindComponent<RectTransform>(30);
			m_Img_ProessBg4 = autoBindTool.GetBindComponent<Image>(31);
			m_Trans_Effect4 = autoBindTool.GetBindComponent<RectTransform>(32);
			m_Img_ProessBg5 = autoBindTool.GetBindComponent<Image>(33);
			m_Trans_Effect5 = autoBindTool.GetBindComponent<RectTransform>(34);
			m_Img_ProessBg6 = autoBindTool.GetBindComponent<Image>(35);
			m_Trans_Effect6 = autoBindTool.GetBindComponent<RectTransform>(36);
			m_Img_ProessBg7 = autoBindTool.GetBindComponent<Image>(37);
			m_Trans_Effect7 = autoBindTool.GetBindComponent<RectTransform>(38);
			m_Tog_Select = autoBindTool.GetBindComponent<Toggle>(39);
			m_VGridScroll_TournamentList = autoBindTool.GetBindComponent<LoopGridView>(40);
			m_Txt_Num = autoBindTool.GetBindComponent<Text>(41);
			m_Txt_MyRank = autoBindTool.GetBindComponent<Text>(42);
		}
	}
}
