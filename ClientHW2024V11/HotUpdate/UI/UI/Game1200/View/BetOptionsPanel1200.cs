using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotUpdate
{
    public partial class BetOptionsPanel1200 : PanelBase
    {
        ScorllViewAutoHandler ScrollBaseSize;
        ScorllViewAutoHandler ScrollBetLevel;
        ScorllViewAutoHandler ScrollBetAmount;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            ScrollBaseSize = m_Trans_ScrollBaseSize.GetComponent<ScorllViewAutoHandler>();
            ScrollBetLevel = m_Trans_ScrollBetLevel.GetComponent<ScorllViewAutoHandler>();
            ScrollBetAmount = m_Trans_ScrollBetAmount.GetComponent<ScorllViewAutoHandler>();
        }

        public void Open()
        {
            transform.gameObject.SetActive(true);

            ScrollBaseSize.Init(3);
            ScrollBetLevel.Init(4);
            ScrollBetAmount.Init(5);
        }
    }
}
