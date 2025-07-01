using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

namespace Kubility
{
	/// <summary>
	/// or struct?
	/// </summary>
	public sealed class KAssetBundle 
	{
		private AssetBundle _assetbundle;
		private AssetBundle assetbundle
		{
			get
			{
				return _assetbundle;

			}
		}

		public Object mainAsset
		{
			get
			{
				return _assetbundle.mainAsset;
			}
		}

		public string name
		{
			get
			{
				return _assetbundle.name;
			}
		}

		private long _size;
		public long size 
		{
			get
			{
				return _size;
			}
		}


		public KAssetBundle(AssetBundle bundle, long psize)
		{
			_size = psize;
			_assetbundle = bundle;
		}

		public void UpdateSize(long psize)
		{
			_size = psize;
		}
		/*
		public T LoadAsset<T>(string name)  where T :Object
		{
			return  _assetbundle.LoadAsset<T>(name);
		}

		public T[] LoadAll<T>()  where T :Object
		{
			return  _assetbundle.LoadAllAssets<T>();
		}
		*/

		public T[] LoadAllSub<T>(string name)  where T :Object
		{
			return  _assetbundle.LoadAssetWithSubAssets<T>(name);
		}

		public void Unload(bool includeAll)
		{
			if(_assetbundle != null)
				_assetbundle.Unload(includeAll);
		}

		public static KAssetBundle LoadFromFile(string name) 
		{
#if UNITY_5_3
			return  new KAssetBundle( AssetBundle.LoadFromFile(name) ,new System.IO.FileInfo(name).Length);
#else
			return  new KAssetBundle( AssetBundle.LoadFromFile(name) ,new System.IO.FileInfo(name).Length);
#endif
		
		}

		public string[] GetAllScenePaths()
		{
			return _assetbundle.GetAllScenePaths();
		}

		public string[] GetAllAssetNames()
		{
			return _assetbundle.GetAllAssetNames();
		}

		///

		public static implicit operator AssetBundle( KAssetBundle bundle)
		{
			if(bundle == null)
			{
				return null;
			}
			return bundle.assetbundle;
		}


	}
}
