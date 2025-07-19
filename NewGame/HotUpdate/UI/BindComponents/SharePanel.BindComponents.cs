using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class SharePanel
	{
		private TextMeshProUGUI m_TxtM_Gold;
		private Button m_Btn_Fb;
		private Button m_Btn_Whatsapp;
		private Button m_Btn_Copy;
		private TextMeshProUGUI m_TxtM_UID;
		private Toggle m_Tog_Option1;
		private Toggle m_Tog_Option2;
		private Toggle m_Tog_Option3;
		private Toggle m_Tog_Option4;
		private RectTransform m_Rect_RightOption1;
		private RectTransform m_Rect_Grid1;
		private TextMeshProUGUI m_TxtM_InviteTitle1;
		private LoopGridView m_VGridScroll_promoteList;
		private RectTransform m_Rect_RightOption2;
		private RectTransform m_Rect_Grid2;
		private TextMeshProUGUI m_TxtM_InviteTitle2;
		private LoopGridView m_VGridScroll_payList;
		private RectTransform m_Rect_RightOption3;
		private TextMeshProUGUI m_TxtM_ExtractGold;
		private RectTransform m_Rect_RightOption4;
		private LoopGridView m_VGridScroll_RankList;
		private TextMeshProUGUI m_TxtM_MyId;
		private TextMeshProUGUI m_TxtM_Lower;
		private TextMeshProUGUI m_TxtM_MyGold;
		private TextMeshProUGUI m_TxtM_ExpandGold;
		private RectTransform m_Trans_GoldIcon;
		private Button m_Btn_Saque;
		private Button m_Btn_Link;
		private Button m_Btn_Back;
		private Button m_Btn_Refresh;
		private Button m_Btn_Service;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_TxtM_Gold = autoBindTool.GetBindComponent<TextMeshProUGUI>(0);
			m_Btn_Fb = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Whatsapp = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Copy = autoBindTool.GetBindComponent<Button>(3);
			m_TxtM_UID = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
			m_Tog_Option1 = autoBindTool.GetBindComponent<Toggle>(5);
			m_Tog_Option2 = autoBindTool.GetBindComponent<Toggle>(6);
			m_Tog_Option3 = autoBindTool.GetBindComponent<Toggle>(7);
			m_Tog_Option4 = autoBindTool.GetBindComponent<Toggle>(8);
			m_Rect_RightOption1 = autoBindTool.GetBindComponent<RectTransform>(9);
			m_Rect_Grid1 = autoBindTool.GetBindComponent<RectTransform>(10);
			m_TxtM_InviteTitle1 = autoBindTool.GetBindComponent<TextMeshProUGUI>(11);
			m_VGridScroll_promoteList = autoBindTool.GetBindComponent<LoopGridView>(12);
			m_Rect_RightOption2 = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Rect_Grid2 = autoBindTool.GetBindComponent<RectTransform>(14);
			m_TxtM_InviteTitle2 = autoBindTool.GetBindComponent<TextMeshProUGUI>(15);
			m_VGridScroll_payList = autoBindTool.GetBindComponent<LoopGridView>(16);
			m_Rect_RightOption3 = autoBindTool.GetBindComponent<RectTransform>(17);
			m_TxtM_ExtractGold = autoBindTool.GetBindComponent<TextMeshProUGUI>(18);
			m_Rect_RightOption4 = autoBindTool.GetBindComponent<RectTransform>(19);
			m_VGridScroll_RankList = autoBindTool.GetBindComponent<LoopGridView>(20);
			m_TxtM_MyId = autoBindTool.GetBindComponent<TextMeshProUGUI>(21);
			m_TxtM_Lower = autoBindTool.GetBindComponent<TextMeshProUGUI>(22);
			m_TxtM_MyGold = autoBindTool.GetBindComponent<TextMeshProUGUI>(23);
			m_TxtM_ExpandGold = autoBindTool.GetBindComponent<TextMeshProUGUI>(24);
			m_Trans_GoldIcon = autoBindTool.GetBindComponent<RectTransform>(25);
			m_Btn_Saque = autoBindTool.GetBindComponent<Button>(26);
			m_Btn_Link = autoBindTool.GetBindComponent<Button>(27);
			m_Btn_Back = autoBindTool.GetBindComponent<Button>(28);
			m_Btn_Refresh = autoBindTool.GetBindComponent<Button>(29);
			m_Btn_Service = autoBindTool.GetBindComponent<Button>(30);
		}
	}
}
