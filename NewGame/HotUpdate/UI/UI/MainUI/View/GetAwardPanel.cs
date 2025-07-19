using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate {
    public partial class GetAwardPanel : PanelBase
    {
        public enum ThingsIcon
        {
            Coins=9,

        }
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        protected override void Update()
        {
            base.Update();
        }


        #region 事件绑定
        public void RegisterListener()
        {

            
        }

        public void UnRegisterListener()
        {
        }
        #endregion


    }
}
