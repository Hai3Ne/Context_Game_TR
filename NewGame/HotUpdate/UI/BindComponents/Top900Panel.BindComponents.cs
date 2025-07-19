using TMPro;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Top900Panel
	{
		private Text m_Txt_Pagar5;
		private Text m_Txt_Pagar6;
		private Text m_Txt_Pagar7;
		private Text m_Txt_Pagar8;
		private Text m_Txt_Pagar9;
		private TextMeshProUGUI m_TxtM_Diamond1;
		private SkeletonGraphic m_Spine_effect1;
		private TextMeshProUGUI m_TxtM_Diamond2;
		private SkeletonGraphic m_Spine_effect2;
		private TextMeshProUGUI m_TxtM_Diamond3;
		private SkeletonGraphic m_Spine_effect3;
		private TextMeshProUGUI m_TxtM_Diamond4;
		private SkeletonGraphic m_Spine_effect4;
		private TextMeshProUGUI m_TxtM_Diamond5;
		private SkeletonGraphic m_Spine_effect5;
		private RectTransform m_Trans_Effect;
		private RectTransform m_Trans_TfFree;
		private SkeletonGraphic m_Spine_Free;
		private RectTransform m_Trans_FreeTimes;
		private Text m_Txt_FreeTimes;
		private Button m_Btn_Continue;
		private TextMeshProUGUI m_TxtM_Times;
		private RectTransform m_Trans_FreeGameEnd;
		private Text m_Txt_GetFreeGold;
		private Button m_Btn_Coletar;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Txt_Pagar5 = autoBindTool.GetBindComponent<Text>(0);
			m_Txt_Pagar6 = autoBindTool.GetBindComponent<Text>(1);
			m_Txt_Pagar7 = autoBindTool.GetBindComponent<Text>(2);
			m_Txt_Pagar8 = autoBindTool.GetBindComponent<Text>(3);
			m_Txt_Pagar9 = autoBindTool.GetBindComponent<Text>(4);
			m_TxtM_Diamond1 = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_Spine_effect1 = autoBindTool.GetBindComponent<SkeletonGraphic>(6);
			m_TxtM_Diamond2 = autoBindTool.GetBindComponent<TextMeshProUGUI>(7);
			m_Spine_effect2 = autoBindTool.GetBindComponent<SkeletonGraphic>(8);
			m_TxtM_Diamond3 = autoBindTool.GetBindComponent<TextMeshProUGUI>(9);
			m_Spine_effect3 = autoBindTool.GetBindComponent<SkeletonGraphic>(10);
			m_TxtM_Diamond4 = autoBindTool.GetBindComponent<TextMeshProUGUI>(11);
			m_Spine_effect4 = autoBindTool.GetBindComponent<SkeletonGraphic>(12);
			m_TxtM_Diamond5 = autoBindTool.GetBindComponent<TextMeshProUGUI>(13);
			m_Spine_effect5 = autoBindTool.GetBindComponent<SkeletonGraphic>(14);
			m_Trans_Effect = autoBindTool.GetBindComponent<RectTransform>(15);
			m_Trans_TfFree = autoBindTool.GetBindComponent<RectTransform>(16);
			m_Spine_Free = autoBindTool.GetBindComponent<SkeletonGraphic>(17);
			m_Trans_FreeTimes = autoBindTool.GetBindComponent<RectTransform>(18);
			m_Txt_FreeTimes = autoBindTool.GetBindComponent<Text>(19);
			m_Btn_Continue = autoBindTool.GetBindComponent<Button>(20);
			m_TxtM_Times = autoBindTool.GetBindComponent<TextMeshProUGUI>(21);
			m_Trans_FreeGameEnd = autoBindTool.GetBindComponent<RectTransform>(22);
			m_Txt_GetFreeGold = autoBindTool.GetBindComponent<Text>(23);
			m_Btn_Coletar = autoBindTool.GetBindComponent<Button>(24);
		}
	}
}
