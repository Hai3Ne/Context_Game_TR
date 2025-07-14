using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Kubility
{
		public class KCoroutineTask
		{
				static List<KCoroutineTask> TaskList = new List<KCoroutineTask>();

				IEnumerator task;

				IEnumerator ReapeatTask;

				Action callback;

				public KCoroutineTask (IEnumerator e)
				{
						this.task = e;
						KCoroutine.StartNewCoroutine(task);

						TaskList.TryAdd(this);
				}

				public KCoroutineTask (Action call)
				{

						callback = call;

						TaskList.TryAdd(this);
				}

				public void ReStart (IEnumerator e)
				{
						this.task = e;
						KCoroutine.StopNewCoroutine(task);
						KCoroutine.StartNewCoroutine(e);
				}


				public void Repeat(Action call,float dely,float RepeatDelta)
				{
						callback = call;
						if(ReapeatTask != null)
								KCoroutine.StopNewCoroutine(ReapeatTask);
						
						ReapeatTask = ReaptToDo(dely,RepeatDelta);
						KCoroutine.StartNewCoroutine(ReapeatTask);
				}

				public void Repeat(IEnumerator call,float dely,float RepeatDelta)
				{
						this.task =call;
						if(ReapeatTask != null)
								KCoroutine.StopNewCoroutine(ReapeatTask);

						ReapeatTask = ReaptToDo(dely,RepeatDelta);
						KCoroutine.StartNewCoroutine(ReapeatTask);
				}

				public void Stop ()
				{
						if(task != null)
								KCoroutine.StopNewCoroutine(task);
						
						if(ReapeatTask != null)
								KCoroutine.StopNewCoroutine(ReapeatTask);

				}

				public static void Close()
				{
						for(int i=0; i < TaskList.Count;++i)
						{
								TaskList[i].Stop();

						}

						TaskList.Clear();
				}


				IEnumerator ReaptToDo(float delay,float reapeatDelta)
				{
						yield return new WaitForSeconds(delay);

						while(true)
						{
								callback.TryCall();
								if(task != null)
										KCoroutine.StartNewCoroutine(task);
								yield return new WaitForSeconds(reapeatDelta);
						}
				}
		}

		public class KCoroutine : MonoSingleTon<KCoroutine>
		{
				//这是同步的！不需要担心单例！
				public static void StartNewCoroutine (IEnumerator e)
				{
						Instance.StartCoroutine(e);
				}

				public static void StopNewCoroutine (IEnumerator e)
				{
						Instance.StopCoroutine (e);
				}

				public static void StopAllCoroutine()
				{
						Instance.StopAllCoroutines();
				}
		}
}
