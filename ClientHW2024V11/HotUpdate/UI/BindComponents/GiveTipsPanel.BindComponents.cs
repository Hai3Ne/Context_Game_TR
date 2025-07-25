using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class GiveTipsPanel
	{
		private Image m_Img_Mask;
		private Image m_Img_Bg;
		private Button m_Btn_Close;
		private RectTransform m_Rect_panel1;
		private Text m_Txt_SendId;
		private Text m_Txt_FromId;
		private Text m_Txt_Name;
		private Text m_Txt_Gold;
		private Text m_Txt_Time;
		private Button m_Btn_Give;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Mask = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(1);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(2);
			m_Rect_panel1 = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Txt_SendId = autoBindTool.GetBindComponent<Text>(4);
			m_Txt_FromId = autoBindTool.GetBindComponent<Text>(5);
			m_Txt_Name = autoBindTool.GetBindComponent<Text>(6);
			m_Txt_Gold = autoBindTool.GetBindComponent<Text>(7);
			m_Txt_Time = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_Give = autoBindTool.GetBindComponent<Button>(9);
		}
	}
}
