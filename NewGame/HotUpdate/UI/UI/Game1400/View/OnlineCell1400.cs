using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class OnlineCell1400 : MonoBehaviour
    {
        public Image m_Img_Head;
        public Text userID;
        private void Awake()
        {
            m_Img_Head = transform.Find("Img_Head").GetComponent<Image>();
            userID = transform.Find("Txt_UID").GetComponent<Text>();

            m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + "7");
        }

        public void SetCellInfo(int index)
        {
            userID.text = index.ToString();
            if (Game1400Model.Instance.arrayPlayer == null)
                return;
            m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + ""+ Game1400Model.Instance.arrayPlayer[index].nIconID);
            userID.text = CommonTools.BytesToString(Game1400Model.Instance.arrayPlayer[index].szName);
        }
    }
}
