using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class RaioPanel
	{
		private TextMeshProUGUI m_TxtM_Bet;
		private Button m_Btn_reduce;
		private Button m_Btn_Plus;
		private Button m_Btn_Auto;
		private Button m_Btn_Gire;
		private Button m_Btn_Home;
		private Button m_Btn_Shop;
		private TextMeshProUGUI m_TxtM_Golds;
		private Button m_Btn_Rank;
		private Image m_Img_head;
		private TextMeshProUGUI m_TxtM_Name;
		private TextMeshProUGUI m_TxtM_Golds2;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_TxtM_Bet = autoBindTool.GetBindComponent<TextMeshProUGUI>(0);
			m_Btn_reduce = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Plus = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Auto = autoBindTool.GetBindComponent<Button>(3);
			m_Btn_Gire = autoBindTool.GetBindComponent<Button>(4);
			m_Btn_Home = autoBindTool.GetBindComponent<Button>(5);
			m_Btn_Shop = autoBindTool.GetBindComponent<Button>(6);
			m_TxtM_Golds = autoBindTool.GetBindComponent<TextMeshProUGUI>(7);
			m_Btn_Rank = autoBindTool.GetBindComponent<Button>(8);
			m_Img_head = autoBindTool.GetBindComponent<Image>(9);
			m_TxtM_Name = autoBindTool.GetBindComponent<TextMeshProUGUI>(10);
			m_TxtM_Golds2 = autoBindTool.GetBindComponent<TextMeshProUGUI>(11);
		}
	}
}
