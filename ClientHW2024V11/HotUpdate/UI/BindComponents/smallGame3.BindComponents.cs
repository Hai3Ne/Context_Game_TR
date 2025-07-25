using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class smallGame3
	{
		private RectTransform m_Trans_Bg0;
		private RectTransform m_Trans_Top;
		private RectTransform m_Trans_Left;
		private RectTransform m_Trans_Bg;
		private RectTransform m_Trans_Right;
		private Button m_Btn_SAIR;
		private Button m_Btn_Parar;
		private RectTransform m_Trans_Grey;
		private TextMeshProUGUI m_TxtM_Gold;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Bg0 = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_Left = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_Bg = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Trans_Right = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Btn_SAIR = autoBindTool.GetBindComponent<Button>(5);
			m_Btn_Parar = autoBindTool.GetBindComponent<Button>(6);
			m_Trans_Grey = autoBindTool.GetBindComponent<RectTransform>(7);
			m_TxtM_Gold = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
		}
	}
}
