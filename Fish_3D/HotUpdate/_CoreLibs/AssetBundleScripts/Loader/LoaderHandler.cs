using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Kubility
{
		public enum LoaderType
		{
				Default,
				AsyncLoad,
				YieldLoad,
				SyncLoad,
				SceneLoad,
				/// <summary>
				/// 预加载 不增加引用
				/// </summary>
				PreLoad,
		}


	public static class LoaderHandler
	{
		public delegate void FinishHandler<T> (T callbackTarget) where T :UnityEngine.Object;
		public delegate void ProgressHandler (string fullInfo,long HasReceived,long Total,string Filename,Exception ex);

		public static void TryCall<T>(this LoaderHandler.FinishHandler<T> callback, T callbacktarget) where T :UnityEngine.Object
		{
			if (callback != null)
				callback(callbacktarget);
		}

		public static void TryCall(this LoaderHandler.ProgressHandler callback, string fullInfo,long HasReceived,long Total,string Filename,Exception ex)
		{
			if (callback != null)
				callback (fullInfo, HasReceived, Total, Filename, ex);
		}
	}
}
