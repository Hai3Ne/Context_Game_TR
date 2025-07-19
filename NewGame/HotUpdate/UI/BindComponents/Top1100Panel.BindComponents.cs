using Spine.Unity;
using TMPro;
using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Top1100Panel
	{
		private Image m_Img_Blur;
		private SkeletonGraphic m_Spine_Title;
		private RectTransform m_Trans_Image;
		private RectTransform m_Trans_BroadCast;
		private RectTransform m_Trans_Info;
		private Image m_Img_Head;
		private TextMeshProUGUI m_TxtM_ID;
		private TextMeshProUGUI m_TxtM_Money;
		private Text m_Txt_Rate;
		private RectTransform m_Trans_InfoMove;
		private Image m_Img_HeadMove;
		private TextMeshProUGUI m_TxtM_IDMove;
		private TextMeshProUGUI m_TxtM_MoneyMove;
		private Text m_Txt_RateMove;
		private Button m_Btn_Rank;
		private RectTransform m_Trans_Middle;
		private RectTransform m_Trans_Mask;
		private RectTransform m_Trans_Rates;
		private RectTransform m_Trans_Effect;
		private RectTransform m_Trans_TfFree;
		private UnityArmatureComponent m_Dragon_Free;
		private RectTransform m_Trans_FreeTimes;
		private Text m_Txt_FreeTimes;
		private Button m_Btn_Continue;
		private TextMeshProUGUI m_TxtM_Times;
		private RectTransform m_Trans_FreeGameEnd;
		private Text m_Txt_GetFreeGold;
		private Button m_Btn_Coletar;
		private SkeletonGraphic m_Spine_lightning ;
		private RectTransform m_Trans_Cash;
		private RectTransform m_Trans_CashRate;
		private SkeletonGraphic m_Spine_Result;
		private Text m_Txt_ElementCount;
		private Text m_Txt_Double;
		private SkeletonGraphic m_Spine_ResultIcon;
		private Text m_Txt_WinRate;
		private SkeletonGraphic m_Spine_Test;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Blur = autoBindTool.GetBindComponent<Image>(0);
			m_Spine_Title = autoBindTool.GetBindComponent<SkeletonGraphic>(1);
			m_Trans_Image = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_BroadCast = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Trans_Info = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Img_Head = autoBindTool.GetBindComponent<Image>(5);
			m_TxtM_ID = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_TxtM_Money = autoBindTool.GetBindComponent<TextMeshProUGUI>(7);
			m_Txt_Rate = autoBindTool.GetBindComponent<Text>(8);
			m_Trans_InfoMove = autoBindTool.GetBindComponent<RectTransform>(9);
			m_Img_HeadMove = autoBindTool.GetBindComponent<Image>(10);
			m_TxtM_IDMove = autoBindTool.GetBindComponent<TextMeshProUGUI>(11);
			m_TxtM_MoneyMove = autoBindTool.GetBindComponent<TextMeshProUGUI>(12);
			m_Txt_RateMove = autoBindTool.GetBindComponent<Text>(13);
			m_Btn_Rank = autoBindTool.GetBindComponent<Button>(14);
			m_Trans_Middle = autoBindTool.GetBindComponent<RectTransform>(15);
			m_Trans_Mask = autoBindTool.GetBindComponent<RectTransform>(16);
			m_Trans_Rates = autoBindTool.GetBindComponent<RectTransform>(17);
			m_Trans_Effect = autoBindTool.GetBindComponent<RectTransform>(18);
			m_Trans_TfFree = autoBindTool.GetBindComponent<RectTransform>(19);
			m_Dragon_Free = autoBindTool.GetBindComponent<UnityArmatureComponent>(20);
			m_Trans_FreeTimes = autoBindTool.GetBindComponent<RectTransform>(21);
			m_Txt_FreeTimes = autoBindTool.GetBindComponent<Text>(22);
			m_Btn_Continue = autoBindTool.GetBindComponent<Button>(23);
			m_TxtM_Times = autoBindTool.GetBindComponent<TextMeshProUGUI>(24);
			m_Trans_FreeGameEnd = autoBindTool.GetBindComponent<RectTransform>(25);
			m_Txt_GetFreeGold = autoBindTool.GetBindComponent<Text>(26);
			m_Btn_Coletar = autoBindTool.GetBindComponent<Button>(27);
			m_Spine_lightning  = autoBindTool.GetBindComponent<SkeletonGraphic>(28);
			m_Trans_Cash = autoBindTool.GetBindComponent<RectTransform>(29);
			m_Trans_CashRate = autoBindTool.GetBindComponent<RectTransform>(30);
			m_Spine_Result = autoBindTool.GetBindComponent<SkeletonGraphic>(31);
			m_Txt_ElementCount = autoBindTool.GetBindComponent<Text>(32);
			m_Txt_Double = autoBindTool.GetBindComponent<Text>(33);
			m_Spine_ResultIcon = autoBindTool.GetBindComponent<SkeletonGraphic>(34);
			m_Txt_WinRate = autoBindTool.GetBindComponent<Text>(35);
			m_Spine_Test = autoBindTool.GetBindComponent<SkeletonGraphic>(36);
		}
	}
}
