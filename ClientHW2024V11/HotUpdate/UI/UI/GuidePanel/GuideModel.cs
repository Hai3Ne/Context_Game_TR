using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class GuideModel : Singleton<GuideModel>
    {
        // public MsgData_sLogin loginData;
       // PlayerPrefs.SetString($"MainUIGuide{MainUIModel.Instance.palyerData.m_i8roleID}", $"MainUIGuide{MainUIModel.Instance.palyerData.m_i8roleID}");
      
       public bool bReachCondition(int guideIndex)
        {
            if (!HotStart.ins.m_isShow)
            {
                return false;
            }
            if (PlayerPrefs.HasKey(MainUIModel.Instance.palyerData.m_i8roleID + "===>-=-" + guideIndex))
            {
                return false;
            }
            else
            {
               
                return true;
            }
        }

        public void SetFinish(int guideIndex)
        {
            PlayerPrefs.SetString(MainUIModel.Instance.palyerData.m_i8roleID + "===>-=-" + guideIndex, guideIndex.ToString());
        }
        
        public bool IsCompleted(int guideIndex)
        {
            return PlayerPrefs.HasKey(MainUIModel.Instance.palyerData.m_i8roleID + "===>-=-" + guideIndex);

        }
    }
}
