using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class ResultPanel1400
	{
		private SkeletonGraphic m_Spine_Result;
		private Button m_Btn_Confirme;
		private TextMeshProUGUI m_TxtM_DoubleRate;
		private Text m_Txt_0;
		private Text m_Txt_1;
		private Text m_Txt_2;
		private Text m_Txt_3;
		private Text m_Txt_4;
		private Text m_Txt_5;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Spine_Result = autoBindTool.GetBindComponent<SkeletonGraphic>(0);
			m_Btn_Confirme = autoBindTool.GetBindComponent<Button>(1);
			m_TxtM_DoubleRate = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_Txt_0 = autoBindTool.GetBindComponent<Text>(3);
			m_Txt_1 = autoBindTool.GetBindComponent<Text>(4);
			m_Txt_2 = autoBindTool.GetBindComponent<Text>(5);
			m_Txt_3 = autoBindTool.GetBindComponent<Text>(6);
			m_Txt_4 = autoBindTool.GetBindComponent<Text>(7);
			m_Txt_5 = autoBindTool.GetBindComponent<Text>(8);
		}
	}
}
