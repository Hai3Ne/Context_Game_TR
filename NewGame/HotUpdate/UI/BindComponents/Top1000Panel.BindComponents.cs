using TMPro;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Top1000Panel
	{
		private Image m_Img_Blur;
		private TextMeshProUGUI m_TxtM_Pago;
		private TextMeshProUGUI m_TxtM_PressJoga;
		private TextMeshProUGUI m_TxtM_Pago1;
		private TextMeshProUGUI m_TxtM_Pago2;
		private TextMeshProUGUI m_TxtM_Pago3;
		private TextMeshProUGUI m_TxtM_Pago4;
		private RectTransform m_Trans_Effect;
		private SkeletonGraphic m_Spine_GameTips;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Blur = autoBindTool.GetBindComponent<Image>(0);
			m_TxtM_Pago = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
			m_TxtM_PressJoga = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_TxtM_Pago1 = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_TxtM_Pago2 = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
			m_TxtM_Pago3 = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_TxtM_Pago4 = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Trans_Effect = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Spine_GameTips = autoBindTool.GetBindComponent<SkeletonGraphic>(8);
		}
	}
}
