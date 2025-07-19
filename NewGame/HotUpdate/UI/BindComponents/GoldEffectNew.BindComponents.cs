using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class GoldEffectNew
	{
		private SkeletonGraphic m_Spine_YouWin;
		private SkeletonGraphic m_Spine_JackPot;
		private SkeletonGraphic m_Spine_BigWin;
		private SkeletonGraphic m_Spine_SuperWin;
		private Text m_Txt_Effect;
		private Button m_Btn_CloseEffect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Spine_YouWin = autoBindTool.GetBindComponent<SkeletonGraphic>(0);
			m_Spine_JackPot = autoBindTool.GetBindComponent<SkeletonGraphic>(1);
			m_Spine_BigWin = autoBindTool.GetBindComponent<SkeletonGraphic>(2);
			m_Spine_SuperWin = autoBindTool.GetBindComponent<SkeletonGraphic>(3);
			m_Txt_Effect = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_CloseEffect = autoBindTool.GetBindComponent<Button>(5);
		}
	}
}
