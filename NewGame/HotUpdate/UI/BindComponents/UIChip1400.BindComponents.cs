using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIChip1400
	{
		private RectTransform m_Trans_select;
		private Button m_Btn_Chip;
		private Text m_Txt_Chips;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_select = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Btn_Chip = autoBindTool.GetBindComponent<Button>(1);
			m_Txt_Chips = autoBindTool.GetBindComponent<Text>(2);
		}
	}
}
