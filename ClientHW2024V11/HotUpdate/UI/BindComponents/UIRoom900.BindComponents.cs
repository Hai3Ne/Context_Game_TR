using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom900
	{
		private SkeletonGraphic m_Spine_bgEffect;
		private RectTransform m_Trans_Sound;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Spine_bgEffect = autoBindTool.GetBindComponent<SkeletonGraphic>(0);
			m_Trans_Sound = autoBindTool.GetBindComponent<RectTransform>(1);
		}
	}
}
