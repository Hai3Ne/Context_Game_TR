using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom1600
	{
		private UnityArmatureComponent m_Dragon_1;
		private UnityArmatureComponent m_Dragon_2;
		private UnityArmatureComponent m_Dragon_3;
		private UnityArmatureComponent m_Dragon_RollFreeEffect2;
		private Image m_Img_Para;
		private RectTransform m_Trans_SpineTiger;
		private RectTransform m_Trans_MoveTarget;
		private RectTransform m_Trans_normalBg;
		private Image m_Img_RollBg0;
		private Image m_Img_RollBg1;
		private Image m_Img_RollBg2;
		private RectTransform m_Trans_FreeBg;
		private UnityArmatureComponent m_Dragon_RollFreeEffect;
		private RectTransform m_Trans_Mask0;
		private RectTransform m_Trans_ElementEffectBg0;
		private RectTransform m_Trans_Lines0;
		private RectTransform m_Trans_ElementEffect0;
		private RectTransform m_Trans_Mask1;
		private RectTransform m_Trans_ElementEffectBg1;
		private RectTransform m_Trans_Lines1;
		private RectTransform m_Trans_ElementEffect1;
		private RectTransform m_Trans_Mask2;
		private RectTransform m_Trans_ElementEffectBg2;
		private RectTransform m_Trans_Lines2;
		private RectTransform m_Trans_ElementEffect2;
		private RectTransform m_Trans_SpawnGoldPos;
		private RectTransform m_Trans_Left;
		private RectTransform m_Trans_Right;
		private RectTransform m_Trans_LeftMove;
		private RectTransform m_Trans_RightMove;
		private RectTransform m_Trans_Sound1;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Dragon_1 = autoBindTool.GetBindComponent<UnityArmatureComponent>(0);
			m_Dragon_2 = autoBindTool.GetBindComponent<UnityArmatureComponent>(1);
			m_Dragon_3 = autoBindTool.GetBindComponent<UnityArmatureComponent>(2);
			m_Dragon_RollFreeEffect2 = autoBindTool.GetBindComponent<UnityArmatureComponent>(3);
			m_Img_Para = autoBindTool.GetBindComponent<Image>(4);
			m_Trans_SpineTiger = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Trans_MoveTarget = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Trans_normalBg = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Img_RollBg0 = autoBindTool.GetBindComponent<Image>(8);
			m_Img_RollBg1 = autoBindTool.GetBindComponent<Image>(9);
			m_Img_RollBg2 = autoBindTool.GetBindComponent<Image>(10);
			m_Trans_FreeBg = autoBindTool.GetBindComponent<RectTransform>(11);
			m_Dragon_RollFreeEffect = autoBindTool.GetBindComponent<UnityArmatureComponent>(12);
			m_Trans_Mask0 = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Trans_ElementEffectBg0 = autoBindTool.GetBindComponent<RectTransform>(14);
			m_Trans_Lines0 = autoBindTool.GetBindComponent<RectTransform>(15);
			m_Trans_ElementEffect0 = autoBindTool.GetBindComponent<RectTransform>(16);
			m_Trans_Mask1 = autoBindTool.GetBindComponent<RectTransform>(17);
			m_Trans_ElementEffectBg1 = autoBindTool.GetBindComponent<RectTransform>(18);
			m_Trans_Lines1 = autoBindTool.GetBindComponent<RectTransform>(19);
			m_Trans_ElementEffect1 = autoBindTool.GetBindComponent<RectTransform>(20);
			m_Trans_Mask2 = autoBindTool.GetBindComponent<RectTransform>(21);
			m_Trans_ElementEffectBg2 = autoBindTool.GetBindComponent<RectTransform>(22);
			m_Trans_Lines2 = autoBindTool.GetBindComponent<RectTransform>(23);
			m_Trans_ElementEffect2 = autoBindTool.GetBindComponent<RectTransform>(24);
			m_Trans_SpawnGoldPos = autoBindTool.GetBindComponent<RectTransform>(25);
			m_Trans_Left = autoBindTool.GetBindComponent<RectTransform>(26);
			m_Trans_Right = autoBindTool.GetBindComponent<RectTransform>(27);
			m_Trans_LeftMove = autoBindTool.GetBindComponent<RectTransform>(28);
			m_Trans_RightMove = autoBindTool.GetBindComponent<RectTransform>(29);
			m_Trans_Sound1 = autoBindTool.GetBindComponent<RectTransform>(30);
		}
	}
}
