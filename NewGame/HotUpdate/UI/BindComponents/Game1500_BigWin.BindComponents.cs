using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Game1500_BigWin
	{
		private RectTransform m_Trans_Title1;
		private RectTransform m_Trans_Title2;
		private RectTransform m_Trans_Title3;
		private RectTransform m_Trans_SpineTiger;
		private Text m_Txt_Effect;
		private Button m_Btn_CloseEffect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Title1 = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_Title2 = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_Title3 = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_SpineTiger = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Txt_Effect = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_CloseEffect = autoBindTool.GetBindComponent<Button>(5);
		}
	}
}
