using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class mailItem
	{
		private Image m_Img_Icon;
		private Text m_Txt_read;
		private RectTransform m_Rect_Introduction;
		private Text m_Txt_Title;
		private Text m_Txt_time;
		private Button m_Btn_Show;
		private Text m_Txt_show;
		private Text m_Txt_Day;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Icon = autoBindTool.GetBindComponent<Image>(0);
			m_Txt_read = autoBindTool.GetBindComponent<Text>(1);
			m_Rect_Introduction = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Txt_Title = autoBindTool.GetBindComponent<Text>(3);
			m_Txt_time = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_Show = autoBindTool.GetBindComponent<Button>(5);
			m_Txt_show = autoBindTool.GetBindComponent<Text>(6);
			m_Txt_Day = autoBindTool.GetBindComponent<Text>(7);
		}
	}
}
