using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class VersionPanel : MonoBehaviour
    {
        private int gameType = 0;
        void Awake()
        {
           
            GetBindComponents(gameObject);
        }

        private void OnEnable()
        {
          
            m_Btn_Go.onClick.AddListener(onClickGo);
        }

        public void init(int type)
        {
            gameType = type;
            if (gameType == 1)
            {
                m_Txt_dest.text = "发现版本更新,请退出重进";
                m_Txt_go.text = "确 定";
            }
            else {
                m_Txt_dest.text = "发现大版本更新,是否前往链接";
                m_Txt_go.text = "前 往";
            }

        }

        private void onClickClose()
        {
            LoginCtrl.Instance.VersionPanel = null;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void onClickGo()
        {
            if (gameType == 0)
            {
                Application.OpenURL("https://a.lywl2025.com/apk/com.zhlx.mitxj.apk");
            }
            else {
                Application.Quit();
            }
            
        }

        private void OnDisable()
        {
            
            m_Btn_Go.onClick.RemoveListener(onClickGo);
        }
    }
}