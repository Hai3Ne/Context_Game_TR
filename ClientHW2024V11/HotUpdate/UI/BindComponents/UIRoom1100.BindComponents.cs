using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom1100
	{
		private SkeletonGraphic m_Spine_Bg;
		private RectTransform m_Trans_SlotBg;
		private SkeletonGraphic m_Spine_0;
		private SkeletonGraphic m_Spine_1;
		private SkeletonGraphic m_Spine_2;
		private SkeletonGraphic m_Spine_3;
		private SkeletonGraphic m_Spine_4;
		private RectTransform m_Trans_Rates;
		private SkeletonGraphic m_Spine_Cash;
		private RectTransform m_Trans_Sound;
		private RectTransform m_Trans_Sound2;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Spine_Bg = autoBindTool.GetBindComponent<SkeletonGraphic>(0);
			m_Trans_SlotBg = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Spine_0 = autoBindTool.GetBindComponent<SkeletonGraphic>(2);
			m_Spine_1 = autoBindTool.GetBindComponent<SkeletonGraphic>(3);
			m_Spine_2 = autoBindTool.GetBindComponent<SkeletonGraphic>(4);
			m_Spine_3 = autoBindTool.GetBindComponent<SkeletonGraphic>(5);
			m_Spine_4 = autoBindTool.GetBindComponent<SkeletonGraphic>(6);
			m_Trans_Rates = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Spine_Cash = autoBindTool.GetBindComponent<SkeletonGraphic>(8);
			m_Trans_Sound = autoBindTool.GetBindComponent<RectTransform>(9);
			m_Trans_Sound2 = autoBindTool.GetBindComponent<RectTransform>(10);
		}
	}
}
