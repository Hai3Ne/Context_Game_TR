using TMPro;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom1400
	{
		private Button m_Btn_Leave;
		private Button m_Btn_Online;
		private TextMeshProUGUI m_TxtM_Online;
		private Button m_Btn_Trend;
		private ScrollRect m_SRect_Trend;
		private RectTransform m_Trans_TrendContent;
		private RectTransform m_Trans_ElementParent;
		private RectTransform m_Trans_1;
		private RectTransform m_Trans_2;
		private RectTransform m_Trans_3;
		private RectTransform m_Trans_4;
		private RectTransform m_Trans_5;
		private RectTransform m_Trans_6;
		private RectTransform m_Trans_7;
		private RectTransform m_Trans_8;
		private Text m_Txt_1;
		private Text m_Txt_2;
		private Text m_Txt_3;
		private Text m_Txt_4;
		private Text m_Txt_5;
		private Text m_Txt_6;
		private Text m_Txt_7;
		private Text m_Txt_8;
		private Text m_Txt_1my;
		private Text m_Txt_2my;
		private Text m_Txt_3my;
		private Text m_Txt_4my;
		private Text m_Txt_5my;
		private Text m_Txt_6my;
		private Text m_Txt_7my;
		private Text m_Txt_8my;
		private Button m_Btn_1;
		private Button m_Btn_2;
		private Button m_Btn_3;
		private Button m_Btn_4;
		private Button m_Btn_5;
		private Button m_Btn_6;
		private Button m_Btn_7;
		private Button m_Btn_8;
		private RectTransform m_Trans_ElementPos;
		private Image m_Img_Head;
		private Text m_Txt_UID;
		private Text m_Txt_Gold;
		private Button m_Btn_AddGold;
		private RectTransform m_Trans_BetChips;
		private Button m_Btn_Repetir;
		private SkeletonGraphic m_Spine_Apostar;
		private SkeletonGraphic m_Spine_Espere;
		private RectTransform m_Trans_RemainTimes;
		private TextMeshProUGUI m_TxtM_RemainTimes;
		private RectTransform m_Trans_Sound;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Leave = autoBindTool.GetBindComponent<Button>(0);
			m_Btn_Online = autoBindTool.GetBindComponent<Button>(1);
			m_TxtM_Online = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_Btn_Trend = autoBindTool.GetBindComponent<Button>(3);
			m_SRect_Trend = autoBindTool.GetBindComponent<ScrollRect>(4);
			m_Trans_TrendContent = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Trans_ElementParent = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Trans_1 = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Trans_2 = autoBindTool.GetBindComponent<RectTransform>(8);
			m_Trans_3 = autoBindTool.GetBindComponent<RectTransform>(9);
			m_Trans_4 = autoBindTool.GetBindComponent<RectTransform>(10);
			m_Trans_5 = autoBindTool.GetBindComponent<RectTransform>(11);
			m_Trans_6 = autoBindTool.GetBindComponent<RectTransform>(12);
			m_Trans_7 = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Trans_8 = autoBindTool.GetBindComponent<RectTransform>(14);
			m_Txt_1 = autoBindTool.GetBindComponent<Text>(15);
			m_Txt_2 = autoBindTool.GetBindComponent<Text>(16);
			m_Txt_3 = autoBindTool.GetBindComponent<Text>(17);
			m_Txt_4 = autoBindTool.GetBindComponent<Text>(18);
			m_Txt_5 = autoBindTool.GetBindComponent<Text>(19);
			m_Txt_6 = autoBindTool.GetBindComponent<Text>(20);
			m_Txt_7 = autoBindTool.GetBindComponent<Text>(21);
			m_Txt_8 = autoBindTool.GetBindComponent<Text>(22);
			m_Txt_1my = autoBindTool.GetBindComponent<Text>(23);
			m_Txt_2my = autoBindTool.GetBindComponent<Text>(24);
			m_Txt_3my = autoBindTool.GetBindComponent<Text>(25);
			m_Txt_4my = autoBindTool.GetBindComponent<Text>(26);
			m_Txt_5my = autoBindTool.GetBindComponent<Text>(27);
			m_Txt_6my = autoBindTool.GetBindComponent<Text>(28);
			m_Txt_7my = autoBindTool.GetBindComponent<Text>(29);
			m_Txt_8my = autoBindTool.GetBindComponent<Text>(30);
			m_Btn_1 = autoBindTool.GetBindComponent<Button>(31);
			m_Btn_2 = autoBindTool.GetBindComponent<Button>(32);
			m_Btn_3 = autoBindTool.GetBindComponent<Button>(33);
			m_Btn_4 = autoBindTool.GetBindComponent<Button>(34);
			m_Btn_5 = autoBindTool.GetBindComponent<Button>(35);
			m_Btn_6 = autoBindTool.GetBindComponent<Button>(36);
			m_Btn_7 = autoBindTool.GetBindComponent<Button>(37);
			m_Btn_8 = autoBindTool.GetBindComponent<Button>(38);
			m_Trans_ElementPos = autoBindTool.GetBindComponent<RectTransform>(39);
			m_Img_Head = autoBindTool.GetBindComponent<Image>(40);
			m_Txt_UID = autoBindTool.GetBindComponent<Text>(41);
			m_Txt_Gold = autoBindTool.GetBindComponent<Text>(42);
			m_Btn_AddGold = autoBindTool.GetBindComponent<Button>(43);
			m_Trans_BetChips = autoBindTool.GetBindComponent<RectTransform>(44);
			m_Btn_Repetir = autoBindTool.GetBindComponent<Button>(45);
			m_Spine_Apostar = autoBindTool.GetBindComponent<SkeletonGraphic>(46);
			m_Spine_Espere = autoBindTool.GetBindComponent<SkeletonGraphic>(47);
			m_Trans_RemainTimes = autoBindTool.GetBindComponent<RectTransform>(48);
			m_TxtM_RemainTimes = autoBindTool.GetBindComponent<TextMeshProUGUI>(49);
			m_Trans_Sound = autoBindTool.GetBindComponent<RectTransform>(50);
		}
	}
}
