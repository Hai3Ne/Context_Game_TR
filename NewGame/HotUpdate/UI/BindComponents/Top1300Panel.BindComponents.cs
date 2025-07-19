using TMPro;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Top1300Panel
	{
		private RectTransform m_Trans_Win;
		private Image m_Img_0;
		private Image m_Img_1;
		private Image m_Img_2;
		private Image m_Img_3;
		private Image m_Img_4;
		private TextMeshProUGUI m_TxtM_Score2;
		private RectTransform m_Trans_Effect;
		private RectTransform m_Trans_GoFreeTimes;
		private Text m_Txt_Times;
		private SkeletonGraphic m_Spine_light;
		private RectTransform m_Trans_Sound;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Win = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Img_0 = autoBindTool.GetBindComponent<Image>(1);
			m_Img_1 = autoBindTool.GetBindComponent<Image>(2);
			m_Img_2 = autoBindTool.GetBindComponent<Image>(3);
			m_Img_3 = autoBindTool.GetBindComponent<Image>(4);
			m_Img_4 = autoBindTool.GetBindComponent<Image>(5);
			m_TxtM_Score2 = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Trans_Effect = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Trans_GoFreeTimes = autoBindTool.GetBindComponent<RectTransform>(8);
			m_Txt_Times = autoBindTool.GetBindComponent<Text>(9);
			m_Spine_light = autoBindTool.GetBindComponent<SkeletonGraphic>(10);
			m_Trans_Sound = autoBindTool.GetBindComponent<RectTransform>(11);
		}
	}
}
