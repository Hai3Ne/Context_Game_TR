using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
public class MonoDelegate : MonoSingleTon<MonoDelegate>
{
	public delegate object MethodNameDelegate<T> (ref T y);

	public void Init(){}
	public void Coroutine_DelayNextFrame (System.Action ev)
	{

		StartCoroutine (NextFrame (ev));
	}

	public IEnumerator Coroutine_Delay (float time, System.Action ev)
	{
		IEnumerator emu = Delay (time, ev);
		this.StartCoroutine (emu);
		return emu;
	}

	Queue<Action> actionList = new Queue<Action>();
	public void Dispatch2MainThread(System.Action func){
		actionList.Enqueue (func);
	}

	void Update(){
		while (actionList.Count > 0) {
			var call = actionList.Dequeue ();
			call.TryCall ();
		}
	}

	public void SendWWWRequest(WWW www, Action cb){
		StartCoroutine (RequestWWW(www,cb));
	}

	IEnumerator RequestWWW(WWW www, Action cb){
		yield return www;
		cb.TryCall ();
	}

	public void StopAllCors()
	{
		StopAllCoroutines();
	}

	public void Lerp (Action<int> callback, int start, int end, float time)
	{
		StartCoroutine (ValueLerp (callback, start, end, time));
	}


    public void Lerp(Action<Vector3> callback, Vector3 start, Vector3 end, float time,Action finishhandler)
    {

        StartCoroutine(ValueLerp(callback, start, end, time, finishhandler));
    }

    IEnumerator ValueLerp(Action<Vector3> callback, Vector3 start, Vector3 end, float time, Action finishhandler)
    {

        float curtime = 0f;
        float val = Time.fixedDeltaTime;

        while (curtime < time)
        {
            yield return new WaitForFixedUpdate();
            curtime += val;
            callback(start + (end - start) * (curtime / time));

        }

        yield return null;
        finishhandler.TryCall();
    }
	IEnumerator ValueLerp (Action<int> callback, int start, int end, float time)
	{

			float curtime = 0f;
			float val = Time.fixedDeltaTime;

			while (curtime < time) {
					yield return new WaitForFixedUpdate ();
					curtime += val;
					callback (start + (int)((end - start) * (curtime / time)));

			}

			yield return null;
	}

	/// <summary>
	/// Lerp the specified callback, start, end and time. float
	/// </summary>
	/// <param name="callback">Callback.</param>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	/// <param name="time">Time.</param>
	public void Lerp (Action<float> callback, float start, float end, float time)
	{
			StartCoroutine (ValueLerp (callback, start, end, time));
	}

	IEnumerator ValueLerp (Action<float> callback, float start, float end, float time)
	{

			float curtime = 0f;
			float val = Time.fixedDeltaTime;

			while (curtime < time) {
					yield return new WaitForFixedUpdate ();
					curtime += val;
					float result = start + (end - start) * (curtime / time);
					callback (result);
	
			}

			yield return null;
	}

	IEnumerator NextFrame ( Action ev)
	{
		yield return new WaitForFixedUpdate();
		if (ev != null) {
				ev ();
		}
	}


	IEnumerator Delay (float time, Action ev)
	{
			yield return new WaitForSeconds (time);
			if (ev != null) {
					ev ();
			}
 
	}

}

