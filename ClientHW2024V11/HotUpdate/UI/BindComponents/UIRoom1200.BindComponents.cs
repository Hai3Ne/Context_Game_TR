using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom1200
	{
		private RectTransform m_Trans_Bg;
		private RectTransform m_Trans_TopBg;
		private RectTransform m_Trans_SpineTiger;
		private Text m_Txt_SpecialRate;
		private UnityArmatureComponent m_Dragon_BetiHouLeft;
		private UnityArmatureComponent m_Dragon_BetiHouRight;
		private RectTransform m_Trans_MoveTarget;
		private UnityArmatureComponent m_Dragon_SpecialEffect;
		private UnityArmatureComponent m_Dragon_SpecialKuangEffect;
		private UnityArmatureComponent m_Dragon_SpecialEffectLuoXia0;
		private UnityArmatureComponent m_Dragon_SpecialEffectLuoXia1;
		private UnityArmatureComponent m_Dragon_SpecialEffectLuoXia2;
		private RectTransform m_Trans_Sound;
		private RectTransform m_Trans_Sound1;
		private RectTransform m_Trans_Effect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Bg = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_TopBg = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_SpineTiger = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Txt_SpecialRate = autoBindTool.GetBindComponent<Text>(3);
			m_Dragon_BetiHouLeft = autoBindTool.GetBindComponent<UnityArmatureComponent>(4);
			m_Dragon_BetiHouRight = autoBindTool.GetBindComponent<UnityArmatureComponent>(5);
			m_Trans_MoveTarget = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Dragon_SpecialEffect = autoBindTool.GetBindComponent<UnityArmatureComponent>(7);
			m_Dragon_SpecialKuangEffect = autoBindTool.GetBindComponent<UnityArmatureComponent>(8);
			m_Dragon_SpecialEffectLuoXia0 = autoBindTool.GetBindComponent<UnityArmatureComponent>(9);
			m_Dragon_SpecialEffectLuoXia1 = autoBindTool.GetBindComponent<UnityArmatureComponent>(10);
			m_Dragon_SpecialEffectLuoXia2 = autoBindTool.GetBindComponent<UnityArmatureComponent>(11);
			m_Trans_Sound = autoBindTool.GetBindComponent<RectTransform>(12);
			m_Trans_Sound1 = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Trans_Effect = autoBindTool.GetBindComponent<RectTransform>(14);
		}
	}
}
