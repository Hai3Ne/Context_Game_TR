using TMPro;
using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class TaskPanel
	{
		private RectTransform m_Rect_bg;
		private Toggle m_Tog_Page1;
		private TextMeshProUGUI m_TxtM_PageTxt1;
		private UnityArmatureComponent m_Dragon_Red1;
		private Toggle m_Tog_Page2;
		private TextMeshProUGUI m_TxtM_PageTxt2;
		private UnityArmatureComponent m_Dragon_Red2;
		private Toggle m_Tog_Page3;
		private TextMeshProUGUI m_TxtM_PageTxt3;
		private UnityArmatureComponent m_Dragon_Red3;
		private LoopGridView m_VGridScroll_HeadList;
		private Button m_Btn_Close;
		private Text m_Txt_Time;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_bg = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Tog_Page1 = autoBindTool.GetBindComponent<Toggle>(1);
			m_TxtM_PageTxt1 = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_Dragon_Red1 = autoBindTool.GetBindComponent<UnityArmatureComponent>(3);
			m_Tog_Page2 = autoBindTool.GetBindComponent<Toggle>(4);
			m_TxtM_PageTxt2 = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_Dragon_Red2 = autoBindTool.GetBindComponent<UnityArmatureComponent>(6);
			m_Tog_Page3 = autoBindTool.GetBindComponent<Toggle>(7);
			m_TxtM_PageTxt3 = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
			m_Dragon_Red3 = autoBindTool.GetBindComponent<UnityArmatureComponent>(9);
			m_VGridScroll_HeadList = autoBindTool.GetBindComponent<LoopGridView>(10);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(11);
			m_Txt_Time = autoBindTool.GetBindComponent<Text>(12);
		}
	}
}
