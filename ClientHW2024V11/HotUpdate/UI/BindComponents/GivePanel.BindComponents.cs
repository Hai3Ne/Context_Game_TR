using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class GivePanel
	{
		private Image m_Img_Mask;
		private Image m_Img_Bg;
		private Button m_Btn_Close;
		private Toggle m_Tog_Btn1;
		private Toggle m_Tog_Btn2;
		private RectTransform m_Rect_panel1;
		private InputField m_Input_text1;
		private InputField m_Input_text2;
		private LoopGridView m_HGridScroll_List;
		private Button m_Btn_Give;
		private RectTransform m_Rect_panel2;
		private LoopGridView m_VGridScroll_List;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Mask = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(1);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(2);
			m_Tog_Btn1 = autoBindTool.GetBindComponent<Toggle>(3);
			m_Tog_Btn2 = autoBindTool.GetBindComponent<Toggle>(4);
			m_Rect_panel1 = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Input_text1 = autoBindTool.GetBindComponent<InputField>(6);
			m_Input_text2 = autoBindTool.GetBindComponent<InputField>(7);
			m_HGridScroll_List = autoBindTool.GetBindComponent<LoopGridView>(8);
			m_Btn_Give = autoBindTool.GetBindComponent<Button>(9);
			m_Rect_panel2 = autoBindTool.GetBindComponent<RectTransform>(10);
			m_VGridScroll_List = autoBindTool.GetBindComponent<LoopGridView>(11);
		}
	}
}
