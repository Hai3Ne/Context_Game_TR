using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class BetSizeCell : PanelBase
    {
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        public void Init(int index)
        {
            m_TxtM_Content.text = index+ "";
        }
    }
}
