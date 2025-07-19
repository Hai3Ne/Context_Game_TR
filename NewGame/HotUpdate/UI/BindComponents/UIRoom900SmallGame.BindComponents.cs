using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom900SmallGame
	{
		private SkeletonGraphic m_Spine_Bg;
		private RectTransform m_Trans_TfSlot;
		private RectTransform m_Trans_0;
		private RectTransform m_Trans_1;
		private Button m_Btn_Roll;
		private RectTransform m_Trans_2;
		private RectTransform m_Trans_Tips;
		private Text m_Txt_FinishedRate;
		private TextMeshProUGUI m_TxtM_Times;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Spine_Bg = autoBindTool.GetBindComponent<SkeletonGraphic>(0);
			m_Trans_TfSlot = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_0 = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_1 = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Btn_Roll = autoBindTool.GetBindComponent<Button>(4);
			m_Trans_2 = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Trans_Tips = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Txt_FinishedRate = autoBindTool.GetBindComponent<Text>(7);
			m_TxtM_Times = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
		}
	}
}
