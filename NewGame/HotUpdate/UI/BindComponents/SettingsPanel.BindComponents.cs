using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class SettingsPanel
	{
		private Image m_Img_mask;
		private Image m_Img_Bg;
		private Button m_Btn_Close;
		private ToggleGroup m_TGroup_Toggle;
		private Toggle m_Tog_On;
		private Toggle m_Tog_Off;
		private Text m_Txt_music;
		private Button m_Btn_Music;
		private Text m_Txt_efito;
		private Button m_Btn_Efeito;
		private Button m_Btn_Save;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_mask = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(1);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(2);
			m_TGroup_Toggle = autoBindTool.GetBindComponent<ToggleGroup>(3);
			m_Tog_On = autoBindTool.GetBindComponent<Toggle>(4);
			m_Tog_Off = autoBindTool.GetBindComponent<Toggle>(5);
			m_Txt_music = autoBindTool.GetBindComponent<Text>(6);
			m_Btn_Music = autoBindTool.GetBindComponent<Button>(7);
			m_Txt_efito = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_Efeito = autoBindTool.GetBindComponent<Button>(9);
			m_Btn_Save = autoBindTool.GetBindComponent<Button>(10);
		}
	}
}
