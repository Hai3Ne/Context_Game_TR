using DragonBones;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom1100SmallGame
	{
		private UnityArmatureComponent m_Dragon_Bg;
		private RectTransform m_Trans_TfSlot;
		private UnityArmatureComponent m_Dragon_0;
		private UnityArmatureComponent m_Dragon_1;
		private Button m_Btn_Roll;
		private UnityArmatureComponent m_Dragon_2;
		private RectTransform m_Trans_Tips;
		private Text m_Txt_FinishedRate;
		private TextMeshProUGUI m_TxtM_Times;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Dragon_Bg = autoBindTool.GetBindComponent<UnityArmatureComponent>(0);
			m_Trans_TfSlot = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Dragon_0 = autoBindTool.GetBindComponent<UnityArmatureComponent>(2);
			m_Dragon_1 = autoBindTool.GetBindComponent<UnityArmatureComponent>(3);
			m_Btn_Roll = autoBindTool.GetBindComponent<Button>(4);
			m_Dragon_2 = autoBindTool.GetBindComponent<UnityArmatureComponent>(5);
			m_Trans_Tips = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Txt_FinishedRate = autoBindTool.GetBindComponent<Text>(7);
			m_TxtM_Times = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
		}
	}
}
