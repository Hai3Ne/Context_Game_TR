//#define SHOW_LOG
//#define AMIB 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using static Kubility.KTool;
#if AMIB
using Amib;
using Amib.Threading;
#endif

public class KObject
{
	static int count;
	protected bool _isRunning;

	public bool isRunning {
		get {
			return _isRunning;
		}
	}

	protected KObject ()
	{
		count++;
	}

	public static void Dump ()
	{
		UnityEngine.Debug.Log ("Object count left =" + count.ToString ());
	}

	public virtual void OnCreate ()
	{
		#if SHOW_LOG
		LogMgr.Log (this.ToString () + " OnCreate");
		#endif
	}

	public virtual void OnPause ()
	{
		_isRunning = false;
		#if SHOW_LOG
		LogMgr.Log (this.ToString () + " OnPause");
		#endif
	}

	public virtual void OnResume ()
	{
		_isRunning = true;
		#if SHOW_LOG
		LogMgr.Log (this.ToString () + " OnResume");
		#endif
	}

	public virtual void OnDestroy ()
	{
		_isRunning = false;
		#if SHOW_LOG
		LogMgr.Log (this.ToString () + " OnDestroy");
		#endif
		count--;

	}

	public virtual void OnEnter ()
	{
		_isRunning = true;
		#if SHOW_LOG
		LogMgr.Log (this.ToString () + " OnEnter");
		#endif
	}

	public virtual void OnExit ()
	{
		_isRunning = false;
		#if SHOW_LOG
		LogMgr.Log (this.ToString () + " OnExit");
		#endif
	}
}

	
public sealed class KThread : KObject
	{
		public const int Thread_MaxSize = 16;
			#if !AMIB


			delegate void SingleDelegate (object obj);

			SingleDelegate SEv;

			AutoResetEvent _restEv;

			AutoResetEvent restEv {
					get {
							return _restEv;
					}
					set {
							_restEv = value;
					}
			}

			bool kill = false;

			Thread m_thread;

			#else
			static SmartThreadPool Pool = new SmartThreadPool ();

			IWorkItemResult result;


			#endif

			VoidDelegate VoidEv;

			public static bool RaiseAbortException = false;

			#if !AMIB

			KThread ()
			{
					if (m_thread == null) {
							m_thread = new Thread (new ThreadStart (ThreadEvents));
							restEv = new AutoResetEvent (false);
					}
			
			}
			#else
			KThread (IWorkItemResult res, VoidDelegate vev)
			{
							result = res;
							VoidEv = vev;
			}
			#endif

			public static KThread StartTask (VoidDelegate vev, bool autoStart = true)
			{
					#if AMIB
					IWorkItemResult result = null;

					if (autoStart) {

							result = Pool.QueueWorkItem (delegate() {
									vev ();
							});
					}

					KThread th = new KThread (result, vev);

					if(autoStart)
					{
							th._isRunning = true;
					}
					#else

					KThread th= null;
					KThreadPool.Instance.Push_Task (vev, autoStart, delegate(KThread obj) {
							th = obj;

					});
					#endif


					return th;
			}

			#if !AMIB
			void ThreadEvents ()
			{

					try {
							while (!kill) {

									restEv.WaitOne ();


									OnEnter ();

									if (VoidEv != null) {
											VoidEv ();
									}

									if (SEv != null && !kill) {
											SEv (this);
									}

									OnExit ();
							}

							OnDestroy ();
					} catch (ThreadAbortException abortEx) {
							if (RaiseAbortException) {
									Debug.LogError ("ThreadAbortException Error = " + abortEx.ToString ());
							}
					} catch (Exception ex) {
		                	Debug.LogError ("ThreadEvents Error = " + ex.ToString ());

					} finally {
							KThreadPool.Instance.Remove (this);
					}

			}


			#region Force

			public void ForceSuspend ()
			{


					if (m_thread != null && isRunning) {
							OnPause ();
							m_thread.Suspend ();
					}
			}

			public void ForceResume ()
			{

					if (m_thread != null && !isRunning) {
							OnResume ();
							m_thread.Resume ();
					}
			}

			#endregion


			#region Weak

			public void WeakStop ()
			{
					if (m_thread != null) {
							OnPause ();
							restEv.Reset ();
					}
			}

			public void WeakResume ()
			{
					if (m_thread != null && m_thread.ThreadState == ThreadState.WaitSleepJoin) {
							OnResume ();
							restEv.Set ();
					}
			}

			#endregion
			#endif

			/// <summary>
			/// next loop will stop
			/// </summary>
			public void WillKill ()
			{

					#if AMIB
					if (result.LogNull () && !result.IsCanceled) 
					{
							result.Cancel ();
							this._isRunning = false;
					}
					#else

					if (m_thread != null && m_thread.ThreadState != ThreadState.Unstarted) {
							this.kill = true;
							this.restEv.Set ();
					} else if (m_thread.ThreadState == ThreadState.Unstarted) {
							Abort ();
					}
					#endif
			}


			public void Start ()
			{
					#if AMIB
					if (VoidEv != null && result == null) {
							result = Pool.QueueWorkItem (delegate() {
									VoidEv ();
							});

							this._isRunning = true;
					}
					#else

					if (m_thread != null) {
						KThreadPool.Instance.StartTask (KThreadPool.Instance.GetFirstTask ());
					}
					#endif
			}

			public void Abort ()
			{
					#if AMIB
					if (result.LogNull ()) {
							result.Cancel ();
					}

					this._isRunning = false;
					#else

					if (m_thread != null) {
							VoidEv = null;
							SEv = null;
							m_thread.Abort ();
							kill = true;
    
							OnDestroy ();
					} else {
							OnDestroy ();
					}
					#endif

			}

			public static void CloseAll ()
			{
					#if AMIB
					if (Pool != null) {
							Pool.Shutdown ();
					}
					#else
						KThreadPool.Instance.Close();
					#endif
			}

			#if !AMIB
			private class KThreadPool : SingleTon<KThreadPool>
			{
					KThread[] pool;
					LinkedList<KThread> WorkQueue = new LinkedList<KThread> ();
					LinkedList<VoidDelegate> TaskQueue = new LinkedList<VoidDelegate> ();
					LinkedList<KThread> WaitQueue = new LinkedList<KThread> ();
					bool _stop = false;

					public KThreadPool ()
					{
//                for (int i = 0; i < 5; ++i)
//                {
//                    var th = new KThread();
//                    Push_ToWaitQueue(th);
//                }
					}

					public void ResumeAll ()
					{
							_stop = false;
							var enumerator = WorkQueue.GetEnumerator ();
							while (enumerator.MoveNext ()) {
									KThread sub = (KThread)enumerator.Current;
									sub.ForceResume ();
							}
					}

					public void StopAll ()
					{
							_stop = true;
							var enumerator = WorkQueue.GetEnumerator ();
							while (enumerator.MoveNext ()) {
									KThread sub = (KThread)enumerator.Current;
									sub.ForceSuspend ();
							}

					}



					public void Close ()
					{

							_stop = true;
							TaskQueue.Clear ();

							WorkQueue.Clear ();

							WaitQueue.Clear ();
							if(pool != null)
							{
									for(int i=0; i < pool.Length;++i)
									{
											KThread th = pool[i];
											th.Abort();
									}
							}

					}


					void Push_ToWaitQueue (KThread th)
					{
							if (th != null) {
									lock (m_lock) {
											WaitQueue.AddLast (th);
											if( pool == null)
											{
													pool = new KThread[]{th};
											}
											else
											{
													Array.Resize(ref pool,pool.Length);
													pool[pool.Length -1 ]= th;
											}

									}


							}
					}

					void Push_ToWorkQueue (KThread th)
					{
							if (th != null) {
									lock (m_lock) {
											WorkQueue.AddLast (th);
									}

							}
					}

					public void Remove (KThread th)
					{
							lock (m_lock) {
									WorkQueue.Remove (th);
									WaitQueue.Remove (th);
//					th.OnDestroy();
							}
					}

					public void Push_Task (VoidDelegate vev, bool autoStart, Action<KThread> callback = null)
					{
							TaskQueue.AddLast (vev);
							if (callback != null) {
									if (WaitQueue.Count > 0)
											callback (WaitQueue.First.Value);
									else {
											if (WaitQueue.Count + WorkQueue.Count <= Thread_MaxSize) {
													KThread th = new KThread ();
													Push_ToWaitQueue (th);
                        
													callback (WaitQueue.First.Value);
											}
									}

							}
            
            
							if (autoStart)
									StartTask (GetFirstTask ());
					}

					public VoidDelegate GetFirstTask ()
					{
							if (TaskQueue.Count > 0)
									return TaskQueue.First.Value;
							else
									return null;
					}

					public void StartTask (VoidDelegate vev)
					{
							if (_stop || vev == null)
									return;

							TaskQueue.RemoveFirst ();

							if (WaitQueue.Count > 0) {

									KThread th = null;
									lock (m_lock) {
											var en = WaitQueue.GetEnumerator ();
											while (en.MoveNext ()) {
													th = en.Current as KThread;
													if (th.m_thread.ThreadState == ThreadState.Aborted)
															WaitQueue.Remove (th);
													else if (!th.isRunning)
															break;
											}

											if (th == null)
													return;

											th.VoidEv = null;
											th.SEv = null;

											th.VoidEv += vev;
											th.SEv = Check;

											WorkQueue.AddLast (th);
											WaitQueue.Remove (th);
									}
  

									if (th.m_thread.ThreadState == ThreadState.Unstarted) {

											th.restEv.Set ();
											th.m_thread.Start ();
									} else if (th.m_thread.ThreadState == ThreadState.WaitSleepJoin) {
											th.WeakResume ();
									}

							} else {
									if (WaitQueue.Count + WorkQueue.Count <= Thread_MaxSize) {
											KThread th = new KThread ();
											Push_ToWaitQueue (th);

											StartTask (vev);
									}

							}


					}

					void Check (object obj)
					{
							if (_stop)
									return;

							KThread th;

							lock (m_lock) {
									th = (KThread)obj;
									WaitQueue.AddLast (th);
									WorkQueue.Remove (th);
							}

							th.VoidEv = null;
							th.SEv = null;

							if (TaskQueue.Count > 0) {
									StartTask (GetFirstTask ());
							}

							th.WeakStop ();

					}



			}
			#endif

	}