using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class PersonAtlasPanel
	{
		private Image m_Img_Mask;
		private Image m_Img_Bg;
		private Text m_Txt_Title;
		private Button m_Btn_Close;
		private Button m_Btn_Select;
		private Text m_Txt_Select;
		private LoopGridView m_VGridScroll_HeadList;
		private ToggleGroup m_TGroup_Content;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Mask = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(1);
			m_Txt_Title = autoBindTool.GetBindComponent<Text>(2);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(3);
			m_Btn_Select = autoBindTool.GetBindComponent<Button>(4);
			m_Txt_Select = autoBindTool.GetBindComponent<Text>(5);
			m_VGridScroll_HeadList = autoBindTool.GetBindComponent<LoopGridView>(6);
			m_TGroup_Content = autoBindTool.GetBindComponent<ToggleGroup>(7);
		}
	}
}
