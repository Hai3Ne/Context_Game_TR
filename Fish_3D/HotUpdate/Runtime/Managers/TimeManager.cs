using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 时间相关逻辑操作管理
/// </summary>
public class TimeManager : MonoBehaviour {
    private static DateTime _init_date_time = DateTime.Now;
    private static long _pre_time;
    
    /// <summary>
    /// 单位毫秒
    /// </summary>
    public static long CurTime {
        get {
            return (long)(DateTime.Now - _init_date_time).TotalMilliseconds;
        }
        set {
            if (_pre_time > value) {
                value += uint.MaxValue;
            }
            _init_date_time = DateTime.Now.AddMilliseconds(-value);
            _pre_time = value;
        }
    }
    private static DateTime _pre_frame_time;//上一帧时间戳
    private static int _pre_frame_count;//游戏进行总帧数
    private static float _detal_time;//每帧时间间隔
    public static float detalTime {//每帧时间
        get {
            if (Time.frameCount != _pre_frame_count) {
                DateTime now = DateTime.Now;
                if (_pre_frame_count == 0) {//第一帧间隔特殊处理
                    _detal_time = Time.deltaTime;
                } else {
                    _detal_time = (float)(now - _pre_frame_time).TotalSeconds;
                    if (Time.frameCount > _pre_frame_count + 1) {
                        _detal_time /= Time.frameCount - _pre_frame_count;
                    }
                }
                _pre_frame_time = now;
                _pre_frame_count = Time.frameCount;
            }
            return _detal_time;
        }
    }

    private static TimeManager mono;
    public static TimeManager Mono {
        get {
            if (mono == null) {
                TimeManager.InitData();
            }
            return mono;
        }
    }
    public static void InitData() {
        if (mono == null) {
            mono = new GameObject("_time_manager").AddComponent<TimeManager>();
        }
    }
    private class TimeEvent {
        public float mDelayTime;
        public System.Action mAction;
    }
    private static Dictionary<int, TimeEvent> dic_event = new Dictionary<int, TimeEvent>();
    private static List<KeyValuePair<int, TimeEvent>> __list = new List<KeyValuePair<int, TimeEvent>>();
    private static List<VoidDelegate> _thread_call_list = new List<VoidDelegate>();//不同线程通讯
	void Update(){
        if (itervalFunMap.Count > 0) {
            var en = itervalFunMap.Values.GetEnumerator();
            while (en.MoveNext()) {
                en.Current.TryCall();
            }
        }
        if (dic_event.Count > 0) {
            __list.AddRange(dic_event);
            for (int i = 0; i < __list.Count; i++) {
                if (__list[i].Value.mDelayTime > Time.deltaTime) {
                    __list[i].Value.mDelayTime -= Time.deltaTime;
                } else {
                    __list[i].Value.mAction();
                    dic_event.Remove(__list[i].Key);
                }
            }
            __list.Clear();
        }
        if (_thread_call_list.Count > 0) {
            lock (_thread_call_list) {
                foreach (var item in _thread_call_list) {
                    item();
                }
                _thread_call_list.Clear();
            }
        }
	}
    public static void ClearAllCall() {//清除所有事件
        itervalFunMap.Clear();
        dic_event.Clear();
        lock (_thread_call_list) {
            _thread_call_list.Clear();
        }
        TimeManager.Mono.StopAllCoroutines();
    }
    public static void AddCallThread(VoidDelegate call) {//添加线程方法到主线层
        lock (_thread_call_list) {
            _thread_call_list.Add(call);
        }
    }
    public static void AddDelayEvent(int id, float time, System.Action action) {//添加一个事件响应
        dic_event[id] = new TimeEvent {
            mDelayTime = time,
            mAction = action,
        };
    }
    public static void RemoveDelayEvent(int id) {//移除一个事件
        dic_event.Remove(id);
    }
    /// <summary>
    /// 将服务器时间转换成客户端从启动到现在的时间
    /// </summary>
    /// <param name="ticks">服务器ticks</param>
    /// <returns></returns>
    public static float ConvertClientTime(uint ticks) {
        return Time.realtimeSinceStartup - UTool.GetTickCount() * 0.001f + ticks * 0.001f;
    }

    public static void BindUpdate(GameObject obj,System.Action update) {
        TimeManager mm = obj.GetComponent<TimeManager>();
        if (mm == null) {
            mm = obj.AddComponent<TimeManager>();
        }
        mm.StartCoroutine(__update(update));
    }
    private static IEnumerator __update(System.Action update) {
        while (true) {
            update();
            yield return null;
        }
    }
	static long static_TimerID = 0;
	static Dictionary<long, System.Action> itervalFunMap = new Dictionary<long, System.Action>();
	public static long StartTimerInterval(System.Action run){
		if (mono == null) {
			mono = new GameObject().AddComponent<TimeManager>();
		}
		long timerId = static_TimerID++;
		itervalFunMap.Add(timerId, run);
		return timerId;
	}

	public static void ClearIntervalID(long timerId){
		System.Action run;
		if (itervalFunMap.TryGetValue (timerId, out run)) {
			itervalFunMap.Remove (timerId);
		}
	}

    /// <summary>
    /// 延迟执行某些方法
    /// </summary>
    public static void DelayExec(float time, System.Action action) {
        if (mono == null) {
            mono = new GameObject().AddComponent<TimeManager>();
        }
        mono.StartCoroutine(_delay_exec(time, action));
    }
    /// <summary>
    /// 延迟执行某些方法
    /// </summary>
    public static void DelayExec(MonoBehaviour mono, float time, System.Action action) {
        mono.StartCoroutine(_delay_exec(time, action));
    }
    private static IEnumerator _delay_exec(float time, System.Action action) {
        if (time > 0) {
            yield return new WaitForSeconds(time);
        } else {
            yield return null;
        }
        action.TryCall();
    }
    public static void DelayExec(MonoBehaviour mono,float time, float loop, int count, VoidCall call) {
        mono.StartCoroutine(_delay_exec(time, loop, count, call));
    }
    private static IEnumerator _delay_exec(float time, float loop, int count, VoidCall call) {
        if (time > 0) {
            yield return new WaitForSeconds(time);
        }
        for (int i = 0; i < count; i++) {
            call();
            if (loop > 0) {
                yield return new WaitForSeconds(loop);
            } else {
                yield return null;
            }
        }
    }

    public static void ClearAction() {//清除所有延迟未执行的方法
        if (mono != null) {
            mono.StopAllCoroutines();
        }
    }
}
