using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class WealthPanel
	{
		private LoopGridView m_VGridScroll_RankList;
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_VGridScroll_RankList = autoBindTool.GetBindComponent<LoopGridView>(0);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(1);
		}
	}
}
