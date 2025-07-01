//#define OAPI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif

namespace Kubility
{
	public class KSceneManager : MonoSingleTon<KSceneManager> 
    {

        public static bool isLoadingLevel;

        public static void LoadScene(string name)
		{
#if UNITY_5_3 && !OAPI
            isLoadingLevel = true;
            KAssetBundleManger.Instance.nextScene = name;

            SceneManager.LoadScene(name);
            isLoadingLevel = false;
#else
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
#endif
        }

		public static void LoadScene(int name)
		{
#if UNITY_5_3 && !OAPI
            isLoadingLevel = true;
            SceneManager.LoadScene(name);
            isLoadingLevel = false;
#else
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
#endif
        }

		public static AsyncOperation LoadSceneAsync(string name)
		{
#if UNITY_5_3 && !OAPI
            isLoadingLevel = true;
            KAssetBundleManger.Instance.nextScene = name;
            return SceneManager.LoadSceneAsync(name);
#else
			return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
#endif
		}

		public static AsyncOperation LoadSceneAsync(int name)
		{
#if UNITY_5_3 && !OAPI
            isLoadingLevel = true;
            return SceneManager.LoadSceneAsync(name);
#else
			return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
#endif
		}

#if UNITY_5_3
		public static Scene GetSceneByName(string name)
		{
			 return SceneManager.GetSceneByName(name);
		}

		public static Scene GetSceneByPath(string name)
		{

			return SceneManager.GetSceneByPath(name);
		}
#endif

    }
}


