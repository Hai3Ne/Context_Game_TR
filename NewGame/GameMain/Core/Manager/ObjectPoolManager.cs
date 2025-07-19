/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : 
* @date     : 2014-xx-xx
*/

using UnityEngine;


namespace SEZSJ
{


    public class ObjectPoolManager : MonoBehaviour
    {
        // Use this for initialization


        void Start()
        {

        }


        public void Update()
        {
           
        }

        public void ReleaseObjectPool()
        {
      
        }

        public GameObject ObtainObject(int iResid) 
        { 
     

            return null;
        }


    

        public void PushToPool(int resID, int maxNum = 20)
        {
           
        }




        GameObject m_ObjectPoolRoot;
        public GameObject ObjectPoolRoot
        {
            get
            {
                if (m_ObjectPoolRoot == null)
                {
                    m_ObjectPoolRoot = new GameObject("ObjectPoolRoot");
                }
                return m_ObjectPoolRoot;
            }
        }
    }
}

