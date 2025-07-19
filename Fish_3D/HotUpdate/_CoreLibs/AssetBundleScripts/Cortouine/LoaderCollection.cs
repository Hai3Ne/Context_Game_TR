using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using Object = UnityEngine.Object;


namespace Kubility
{
#if UNITY_5_3
	public class GenericLoader<T> :CustomYieldInstruction where T:class
	{
		private ClsTuple<bool,T> m_callback;

		public T MainObject
		{
			get
			{
				return m_callback.field1;
			}
		}

		public override bool keepWaiting {
			get {
				return !m_callback.field0;
			}
		}

		public GenericLoader(ClsTuple<bool,T> callback)
		{
			m_callback = callback;
		}

	}


	public class RefDataLoader<T> :CustomYieldInstruction where T:Object
	{
		private ClsTuple<bool,T,Kubility.KAssetBundleRef> m_callback;

		public KAssetBundleRef Data
		{
			get
			{
				return m_callback.field2;
			}
		}


		public T MainObject
		{
			get
			{
				return m_callback.field1;
			}
		}

		public override bool keepWaiting {
			get {

				return !m_callback.field0 ;
			}
		}

		public RefDataLoader(ClsTuple<bool,T,KAssetBundleRef> callback)
		{
			m_callback = callback;
		}

	}
#else
	public class GenericLoader<T> :IEnumerable where T:class
	{

		public sealed class Enumer:IEnumerator
		{
			private ClsTuple<bool,T> m_callback;

			public bool MoveNext ()
			{
				return !m_callback.field0;
			}
			
			public void Reset ()
			{
				
			}
			
			public object Current {
				get {
					return null;
				}
			}

			public Enumer(ClsTuple<bool,T> callback)
			{
				m_callback = callback;
			}
		}

		public IEnumerator GetEnumerator ()
		{
			Enumer e = new Enumer(m_Tempcallback);
			return e;
		}

		public T MainObject
		{
			get
			{
				return m_Tempcallback.field1;
			}
		}

		ClsTuple<bool,T> m_Tempcallback;
		public GenericLoader(ClsTuple<bool,T> callback)
		{
			m_Tempcallback = callback;
		}
		
	}
	
	
	public class RefDataLoader<T> :IEnumerable where T:Object
	{

		public sealed class Enumer:IEnumerator
		{
			private ClsTuple<bool,T,Kubility.KAssetBundleRef> m_callback;

			public bool MoveNext ()
			{
				return !m_callback.field0 ;
			}
			
			public void Reset ()
			{
				
			}
			
			public object Current {
				get {
					return null;
				}
			}

			public Enumer(ClsTuple<bool,T,KAssetBundleRef> callback)
			{
				m_callback = callback;
			}
		}

		public IEnumerator GetEnumerator ()
		{
			Enumer e = new Enumer(m_tempCallback);
			return e;
		}

		public bool isDone
		{
			get
			{
				return m_tempCallback.field0 ;
			}
		}

		public KAssetBundleRef Data
		{
			get
			{
				return m_tempCallback.field2;
			}
		}

		public T MainObject
		{
			get
			{
				return m_tempCallback.field1;
			}
		}

		ClsTuple<bool,T,KAssetBundleRef> m_tempCallback;
		public RefDataLoader(ClsTuple<bool,T,KAssetBundleRef> callback)
		{
			m_tempCallback = callback;
		}
		
	}
#endif
}


