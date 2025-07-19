using TMPro;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class smallGame2
	{
		private RectTransform m_Trans_Title;
		private RectTransform m_Trans_Top;
		private TextMeshProUGUI m_TxtM_Gold;
		private Button m_Btn_AddGold;
		private TextMeshProUGUI m_TxtM_Score;
		private SkeletonGraphic m_Spine_WinGoldTips;
		private TextMeshProUGUI m_TxtM_Bonus;
		private RectTransform m_Trans_Finished;
		private SkeletonGraphic m_Spine_Result;
		private TextMeshProUGUI m_TxtM_WinGold;
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Title = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(1);
			m_TxtM_Gold = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_Btn_AddGold = autoBindTool.GetBindComponent<Button>(3);
			m_TxtM_Score = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
			m_Spine_WinGoldTips = autoBindTool.GetBindComponent<SkeletonGraphic>(5);
			m_TxtM_Bonus = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Trans_Finished = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Spine_Result = autoBindTool.GetBindComponent<SkeletonGraphic>(8);
			m_TxtM_WinGold = autoBindTool.GetBindComponent<TextMeshProUGUI>(9);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(10);
		}
	}
}
