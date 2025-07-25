using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class AlmsPanel
	{
		private Image m_Img_mask;
		private Image m_Img_Bg;
		private Button m_Btn_leftReceber;
		private TextMeshProUGUI m_TxtM_leftReceber;
		private Button m_Btn_GLeftReceber;
		private TextMeshProUGUI m_TxtM_GleftReceber;
		private RectTransform m_Trans_CoinNum;
		private Image m_Img_leftCoin;
		private Image m_Img_leftRS;
		private Text m_Txt_leftCoinNum;
		private Image m_Img_GoldIcon;
		private Button m_Btn_leftInfo;
		private Button m_Btn_Close;
		private RectTransform m_Rect_tips;
		private RectTransform m_Rect_LeftInfo;
		private Image m_Img_LeftInfomask;
		private Image m_Img_LeftInfoBg;
		private Button m_Btn_LeftInfoClose;
		private TextMeshProUGUI m_TxtM_LeftInfoContent;
		private Button m_Btn_Cancelar;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_mask = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(1);
			m_Btn_leftReceber = autoBindTool.GetBindComponent<Button>(2);
			m_TxtM_leftReceber = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_Btn_GLeftReceber = autoBindTool.GetBindComponent<Button>(4);
			m_TxtM_GleftReceber = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_Trans_CoinNum = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Img_leftCoin = autoBindTool.GetBindComponent<Image>(7);
			m_Img_leftRS = autoBindTool.GetBindComponent<Image>(8);
			m_Txt_leftCoinNum = autoBindTool.GetBindComponent<Text>(9);
			m_Img_GoldIcon = autoBindTool.GetBindComponent<Image>(10);
			m_Btn_leftInfo = autoBindTool.GetBindComponent<Button>(11);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(12);
			m_Rect_tips = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Rect_LeftInfo = autoBindTool.GetBindComponent<RectTransform>(14);
			m_Img_LeftInfomask = autoBindTool.GetBindComponent<Image>(15);
			m_Img_LeftInfoBg = autoBindTool.GetBindComponent<Image>(16);
			m_Btn_LeftInfoClose = autoBindTool.GetBindComponent<Button>(17);
			m_TxtM_LeftInfoContent = autoBindTool.GetBindComponent<TextMeshProUGUI>(18);
			m_Btn_Cancelar = autoBindTool.GetBindComponent<Button>(19);
		}
	}
}
