using DragonBones;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class MinesPanel
	{
		private UnityArmatureComponent m_Dragon_bg;
		private Button m_Btn_Home;
		private Button m_Btn_Shop;
		private TextMeshProUGUI m_TxtM_Golds;
		private Image m_Img_head;
		private TextMeshProUGUI m_TxtM_UID;
		private TextMeshProUGUI m_TxtM_Money;
		private Button m_Btn_Bet1;
		private RectTransform m_Trans_ShowBomb1;
		private RectTransform m_Trans_HideBomb1;
		private Button m_Btn_Bet2;
		private RectTransform m_Trans_ShowBomb2;
		private RectTransform m_Trans_HideBomb2;
		private Button m_Btn_Bet3;
		private RectTransform m_Trans_ShowBomb3;
		private RectTransform m_Trans_HideBomb3;
		private Button m_Btn_Bet4;
		private RectTransform m_Trans_ShowBomb4;
		private RectTransform m_Trans_HideBomb4;
		private Text m_Txt_Valor1;
		private Text m_Txt_Valor2;
		private Text m_Txt_Proxima1;
		private Text m_Txt_Proxima2;
		private Button m_Btn_reduceTen;
		private Button m_Btn_half;
		private Button m_Btn_start;
		private Button m_Btn_PlusTen;
		private Button m_Btn_Multiply2 ;
		private TextMeshProUGUI m_TxtM_Bet;
		private RectTransform m_Rect_Effect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Dragon_bg = autoBindTool.GetBindComponent<UnityArmatureComponent>(0);
			m_Btn_Home = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Shop = autoBindTool.GetBindComponent<Button>(2);
			m_TxtM_Golds = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_Img_head = autoBindTool.GetBindComponent<Image>(4);
			m_TxtM_UID = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_TxtM_Money = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Btn_Bet1 = autoBindTool.GetBindComponent<Button>(7);
			m_Trans_ShowBomb1 = autoBindTool.GetBindComponent<RectTransform>(8);
			m_Trans_HideBomb1 = autoBindTool.GetBindComponent<RectTransform>(9);
			m_Btn_Bet2 = autoBindTool.GetBindComponent<Button>(10);
			m_Trans_ShowBomb2 = autoBindTool.GetBindComponent<RectTransform>(11);
			m_Trans_HideBomb2 = autoBindTool.GetBindComponent<RectTransform>(12);
			m_Btn_Bet3 = autoBindTool.GetBindComponent<Button>(13);
			m_Trans_ShowBomb3 = autoBindTool.GetBindComponent<RectTransform>(14);
			m_Trans_HideBomb3 = autoBindTool.GetBindComponent<RectTransform>(15);
			m_Btn_Bet4 = autoBindTool.GetBindComponent<Button>(16);
			m_Trans_ShowBomb4 = autoBindTool.GetBindComponent<RectTransform>(17);
			m_Trans_HideBomb4 = autoBindTool.GetBindComponent<RectTransform>(18);
			m_Txt_Valor1 = autoBindTool.GetBindComponent<Text>(19);
			m_Txt_Valor2 = autoBindTool.GetBindComponent<Text>(20);
			m_Txt_Proxima1 = autoBindTool.GetBindComponent<Text>(21);
			m_Txt_Proxima2 = autoBindTool.GetBindComponent<Text>(22);
			m_Btn_reduceTen = autoBindTool.GetBindComponent<Button>(23);
			m_Btn_half = autoBindTool.GetBindComponent<Button>(24);
			m_Btn_start = autoBindTool.GetBindComponent<Button>(25);
			m_Btn_PlusTen = autoBindTool.GetBindComponent<Button>(26);
			m_Btn_Multiply2  = autoBindTool.GetBindComponent<Button>(27);
			m_TxtM_Bet = autoBindTool.GetBindComponent<TextMeshProUGUI>(28);
			m_Rect_Effect = autoBindTool.GetBindComponent<RectTransform>(29);
		}
	}
}
