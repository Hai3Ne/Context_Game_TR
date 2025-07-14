using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.IO;

using Object = UnityEngine.Object;

namespace Kubility
{
    public enum YieldError
    {
        None,
        /// <summary>
        /// 超时错误，默认为60 和65s 应该没有需要加载超过这个时间的资源。
        /// </summary>
        TIME_OVER,
        Success,
        UNKNOWN,
    }
#if UNITY_5_3

    [StructLayout(LayoutKind.Sequential)]
    public sealed class KAsyncRequest<T> : CustomYieldInstruction where T : AsyncOperation
    {
        private T _Main;

        public T Main
        {
            get
            {
                return _Main;
            }
        }

        private YieldError _error;

        public YieldError error
        {
            get
            {
                return _error;
            }
        }

        public override bool keepWaiting
        {
            get
            {
                if (_error == YieldError.None)
                {
                    bool value = _Main.isDone;
                    if (value)
                    {
                        this._error = YieldError.Success;
                    }
                    return value;
                }
                else
                {
                    return false;
                }
            }
        }

        public KAsyncRequest(T realRequest)
        {
            this._Main = realRequest;
            this._error = YieldError.None;
            MonoDelegate.Instance.Coroutine_Delay(60f, delegate ()
            {
                if (_error != YieldError.Success || _error != YieldError.UNKNOWN)
                    _error = YieldError.TIME_OVER;
            });

        }

    }


    [StructLayout(LayoutKind.Sequential)]
    public sealed class SceneAsynacRequest : CustomYieldInstruction
    {
        private AsyncOperation _Main;

        public AsyncOperation Main
        {
            get
            {
                return _Main;
            }
        }

        private RefDataLoader<Object> _sub;

        public RefDataLoader<Object> Sub
        {
            get
            {
                return _sub;
            }
        }

        private YieldError _error;

        public YieldError error
        {
            get
            {
                return _error;
            }
        }

        public override bool keepWaiting
        {
            get
            {
                if (_error == YieldError.None)
                {
                    if (!Sub.keepWaiting && _Main == null)
                    {

                        AsyncOperation asyncop = KSceneManager.LoadSceneAsync(Sub.Data.AssetLoadName);


                        if (!default_show)
                        {
                            asyncop.allowSceneActivation = false;
                        }

                        this._Main = asyncop;
                        //--------------next change
                        KSceneManager.isLoadingLevel = true;

                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool default_show;

        public SceneAsynacRequest(RefDataLoader<Object> second, bool show)
        {
            this.default_show = show;
            this._sub = second;
            this._error = YieldError.None;
            MonoDelegate.Instance.Coroutine_Delay(65f, delegate ()
            {
                if (_error != YieldError.Success || _error != YieldError.UNKNOWN)
                    _error = YieldError.TIME_OVER;
            });

        }

    }

#else
	[StructLayout (LayoutKind.Sequential)]
	public sealed class KAsyncRequest<T> : IEnumerable where T :AsyncOperation
	{
		public sealed class Enumer:IEnumerator
		{
			public bool MoveNext ()
			{
				if(_error == YieldError.None)
				{
					bool value = _Main.isDone;
					if(value)
					{
						this._error = YieldError.Success;
					}
					return value;
				}
				else
				{
					return false;
				}
			}
			
			public void Reset ()
			{
				_error = YieldError.None;
			}
			
			public object Current {
				get {
					return null;
				}
			}
			

			
			private YieldError _error;
			
			public YieldError error
			{
				get
				{
					return _error;
				}
			}
			
			private T _Main;

			public Enumer(T realRequest)
			{
				this._Main = realRequest;
				this._error = YieldError.None;
				MonoDelegate.Instance.Coroutine_Delay(60f,delegate()
				{
					if(_error != YieldError.Success || _error != YieldError.UNKNOWN)
						_error = YieldError.TIME_OVER;
				});
				
			}
		}

		public IEnumerator GetEnumerator ()
		{
			Enumer e = new Enumer(TempReq);
			return e;
		}

		private T _Main;
		public T Main
		{
			get
			{
				return TempReq;
			}
		}

		private T TempReq ;

		public KAsyncRequest(T realRequest)
		{
			TempReq = realRequest;
			
		}
		
	}
	
	
	[StructLayout (LayoutKind.Sequential)]
	public sealed class SceneAsynacRequest: IEnumerable 
	{
		public sealed class Enumer:IEnumerator
		{

			private ClsTuple<AsyncOperation,RefDataLoader<Object>> tuple;

			
			private YieldError _error;
			
			public YieldError error
			{
				get
				{
					return _error;
				}
			}
			
			public bool MoveNext ()
			{
				if(_error == YieldError.None)
				{
					if(!tuple.field1.isDone && tuple.field0 == null)
					{
						
						AsyncOperation asyncop = KSceneManager.LoadSceneAsync(tuple.field1.Data.AssetLoadName);
						tuple.field0 = asyncop;
						
						if(!default_show)
						{
							asyncop.allowSceneActivation = false;
						}
						
						return false;
					}
					
					return true;
				}
				else
				{
					return false;
				}
			}
			
			public void Reset ()
			{
				_error = YieldError.None;
			}
			
			public object Current {
				get {
					return null;
				}
			}
			
			private bool default_show ;

			public Enumer(ClsTuple<AsyncOperation,RefDataLoader<Object>> DataTuple,bool show )
			{
				this.default_show = show;
				this.tuple = DataTuple;
				this._error = YieldError.None;
				MonoDelegate.Instance.Coroutine_Delay(65f,delegate()
				{
					if(_error != YieldError.Success || _error != YieldError.UNKNOWN)
						_error = YieldError.TIME_OVER;
				});
				
			}
		}

		public IEnumerator GetEnumerator()
		{
			Enumer enumerator = new Enumer(DataTuple,firstshow);
			return enumerator;
		}

		private AsyncOperation _Main;
		public AsyncOperation Main
		{
			get
			{
				return DataTuple.field0;
			}
		}
		
		private RefDataLoader<Object> _sub;
		public RefDataLoader<Object> Sub
		{
			get
			{
				return DataTuple.field1;
			}
		}

		ClsTuple<AsyncOperation,RefDataLoader<Object>> DataTuple;

		private bool firstshow ;
		
		public SceneAsynacRequest(RefDataLoader<Object> second,bool show )
		{
			DataTuple = new ClsTuple<AsyncOperation, RefDataLoader<Object>>();

			DataTuple.field1 = second;
			firstshow= show;
		}
		
	}
#endif
}
