using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class WinTipsEffect
	{
		private Text m_Txt_WinGold;
		private SkeletonAnimation m_Spine3D_GoldEffect;
		private SkeletonAnimation m_Spine3D_MassiveWin;
		private SkeletonAnimation m_Spine3D_YouWin;
		private SkeletonAnimation m_Spine3D_JackpotWin;
		private SkeletonAnimation m_Spine3D_HugeWin;
		private SkeletonAnimation m_Spine3D_BigWin;
		private SkeletonAnimation m_Spine3D_ApocalyWin;
		private Button m_Btn_CloseEffect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Txt_WinGold = autoBindTool.GetBindComponent<Text>(0);
			m_Spine3D_GoldEffect = autoBindTool.GetBindComponent<SkeletonAnimation>(1);
			m_Spine3D_MassiveWin = autoBindTool.GetBindComponent<SkeletonAnimation>(2);
			m_Spine3D_YouWin = autoBindTool.GetBindComponent<SkeletonAnimation>(3);
			m_Spine3D_JackpotWin = autoBindTool.GetBindComponent<SkeletonAnimation>(4);
			m_Spine3D_HugeWin = autoBindTool.GetBindComponent<SkeletonAnimation>(5);
			m_Spine3D_BigWin = autoBindTool.GetBindComponent<SkeletonAnimation>(6);
			m_Spine3D_ApocalyWin = autoBindTool.GetBindComponent<SkeletonAnimation>(7);
			m_Btn_CloseEffect = autoBindTool.GetBindComponent<Button>(8);
		}
	}
}
