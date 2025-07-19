using Spine.Unity;
using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class smallGame1300
	{
		private SkeletonGraphic m_Spine_Joker;
		private RectTransform m_Trans_Left;
		private RectTransform m_Trans_Bg;
		private RectTransform m_Trans_Btn;
		private RectTransform m_Trans_a;
		private UnityArmatureComponent m_Dragon_Ani;
		private RectTransform m_Trans_Sound;
		private RectTransform m_Trans_Sound1;
		private RectTransform m_Trans_Sound2;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Spine_Joker = autoBindTool.GetBindComponent<SkeletonGraphic>(0);
			m_Trans_Left = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_Bg = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_Btn = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Trans_a = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Dragon_Ani = autoBindTool.GetBindComponent<UnityArmatureComponent>(5);
			m_Trans_Sound = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Trans_Sound1 = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Trans_Sound2 = autoBindTool.GetBindComponent<RectTransform>(8);
		}
	}
}
