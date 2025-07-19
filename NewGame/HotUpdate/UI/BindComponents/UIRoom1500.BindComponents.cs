using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom1500
	{
		private RectTransform m_Trans_SpineTiger;
		private RectTransform m_Trans_MoveTarget;
		private RectTransform m_Trans_normalBg;
		private RectTransform m_Trans_specialBg;
		private UnityArmatureComponent m_Dragon_deng;
		private Text m_Txt_SpecialRate;
		private RectTransform m_Trans_Rodar;
		private UnityArmatureComponent m_Dragon_leftEffect;
		private UnityArmatureComponent m_Dragon_midEffect;
		private Image m_Img_Rate;
		private UnityArmatureComponent m_Dragon_rightEffect;
		private RectTransform m_Trans_Sound;
		private RectTransform m_Trans_Sound1;
		private RectTransform m_Trans_Effect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_SpineTiger = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_MoveTarget = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_normalBg = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_specialBg = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Dragon_deng = autoBindTool.GetBindComponent<UnityArmatureComponent>(4);
			m_Txt_SpecialRate = autoBindTool.GetBindComponent<Text>(5);
			m_Trans_Rodar = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Dragon_leftEffect = autoBindTool.GetBindComponent<UnityArmatureComponent>(7);
			m_Dragon_midEffect = autoBindTool.GetBindComponent<UnityArmatureComponent>(8);
			m_Img_Rate = autoBindTool.GetBindComponent<Image>(9);
			m_Dragon_rightEffect = autoBindTool.GetBindComponent<UnityArmatureComponent>(10);
			m_Trans_Sound = autoBindTool.GetBindComponent<RectTransform>(11);
			m_Trans_Sound1 = autoBindTool.GetBindComponent<RectTransform>(12);
			m_Trans_Effect = autoBindTool.GetBindComponent<RectTransform>(13);
		}
	}
}
