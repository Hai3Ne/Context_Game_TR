using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom1300
	{
		private RectTransform m_Trans_smallGame;
		private RectTransform m_Trans_Sound;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_smallGame = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_Sound = autoBindTool.GetBindComponent<RectTransform>(1);
		}
	}
}
