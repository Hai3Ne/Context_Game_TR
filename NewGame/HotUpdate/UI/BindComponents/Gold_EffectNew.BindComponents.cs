using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Gold_EffectNew
	{
		private UnityArmatureComponent m_Dragon_YouWin;
		private UnityArmatureComponent m_Dragon_JackPot;
		private UnityArmatureComponent m_Dragon_BigWin;
		private Text m_Txt_Effect;
		private Button m_Btn_CloseEffect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Dragon_YouWin = autoBindTool.GetBindComponent<UnityArmatureComponent>(0);
			m_Dragon_JackPot = autoBindTool.GetBindComponent<UnityArmatureComponent>(1);
			m_Dragon_BigWin = autoBindTool.GetBindComponent<UnityArmatureComponent>(2);
			m_Txt_Effect = autoBindTool.GetBindComponent<Text>(3);
			m_Btn_CloseEffect = autoBindTool.GetBindComponent<Button>(4);
		}
	}
}
