using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class GameTips1600
	{
		private RectTransform m_Trans_GameTips;
		private Image m_Img_Icon;
		private Text m_Txt_Count;
		private Text m_Txt_Rate;
		private RectTransform m_Trans_LeftGameTips;
		private Text m_Txt_LeftCount;
		private Image m_Img_LeftIcon;
		private Text m_Txt_LeftRate;
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_GameTips = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Img_Icon = autoBindTool.GetBindComponent<Image>(1);
			m_Txt_Count = autoBindTool.GetBindComponent<Text>(2);
			m_Txt_Rate = autoBindTool.GetBindComponent<Text>(3);
			m_Trans_LeftGameTips = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Txt_LeftCount = autoBindTool.GetBindComponent<Text>(5);
			m_Img_LeftIcon = autoBindTool.GetBindComponent<Image>(6);
			m_Txt_LeftRate = autoBindTool.GetBindComponent<Text>(7);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(8);
		}
	}
}
