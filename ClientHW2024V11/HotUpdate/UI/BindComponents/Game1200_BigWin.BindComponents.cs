using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Game1200_BigWin
	{
		private RectTransform m_Trans_Quan;
		private RectTransform m_Trans_1;
		private RectTransform m_Trans_2;
		private RectTransform m_Trans_3;
		private SkeletonGraphic m_Spine_YouWin;
		private RectTransform m_Trans_Title1;
		private RectTransform m_Trans_Title2;
		private RectTransform m_Trans_Title3;
		private Text m_Txt_Effect;
		private Button m_Btn_CloseEffect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Quan = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_1 = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_2 = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_3 = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Spine_YouWin = autoBindTool.GetBindComponent<SkeletonGraphic>(4);
			m_Trans_Title1 = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Trans_Title2 = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Trans_Title3 = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Txt_Effect = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_CloseEffect = autoBindTool.GetBindComponent<Button>(9);
		}
	}
}
