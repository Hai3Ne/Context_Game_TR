using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class BigWin1300
	{
		private SkeletonGraphic m_Spine_BigWin;
		private Text m_Txt_Effect;
		private Button m_Btn_CloseEffect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Spine_BigWin = autoBindTool.GetBindComponent<SkeletonGraphic>(0);
			m_Txt_Effect = autoBindTool.GetBindComponent<Text>(1);
			m_Btn_CloseEffect = autoBindTool.GetBindComponent<Button>(2);
		}
	}
}
