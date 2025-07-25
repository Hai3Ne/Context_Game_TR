using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class XxlGamePanel
	{
		private Button m_Btn_close;
		private Text m_Txt_Level;
		private RectTransform m_Rect_Mask;
		private RectTransform m_Rect_Stop;
		private Text m_Txt_level1;
		private Button m_Btn_Destroy;
		private Button m_Btn_continue;
		private RectTransform m_Rect_Over;
		private Text m_Txt_Level2;
		private Button m_Btn_DestroyOver;
		private Button m_Btn_Restart;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_close = autoBindTool.GetBindComponent<Button>(0);
			m_Txt_Level = autoBindTool.GetBindComponent<Text>(1);
			m_Rect_Mask = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Rect_Stop = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Txt_level1 = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_Destroy = autoBindTool.GetBindComponent<Button>(5);
			m_Btn_continue = autoBindTool.GetBindComponent<Button>(6);
			m_Rect_Over = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Txt_Level2 = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_DestroyOver = autoBindTool.GetBindComponent<Button>(9);
			m_Btn_Restart = autoBindTool.GetBindComponent<Button>(10);
		}
	}
}
