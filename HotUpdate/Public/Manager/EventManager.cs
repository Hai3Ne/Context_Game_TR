using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {
    private class EventBase {
        public GameEvent NetCmdType;
        public virtual void Exec(params object[] objs) { }
    }
    private class EventInfo : EventBase {
        public List<VoidCall<GameEvent>> call_list = new List<VoidCall<GameEvent>>();

        public override void Exec(params object[] objs) {
            for (int i = 0; i < call_list.Count; i++) {
                call_list[i](this.NetCmdType);
            }
        }
    }
    private class EventInfo<T> : EventBase {
        public List<VoidCall<GameEvent, T>> call_list = new List<VoidCall<GameEvent, T>>();

        public override void Exec(params object[] objs) {
            for (int i = 0; i < call_list.Count; i++) {
                call_list[i](this.NetCmdType, (T)objs[0]);
            }
        }
    }
    private class EventInfo<T1,T2> : EventBase {
        public List<VoidCall<GameEvent, T1, T2>> call_list = new List<VoidCall<GameEvent, T1, T2>>();

        public override void Exec(params object[] objs) {
            for (int i = 0; i < call_list.Count; i++) {
                call_list[i](this.NetCmdType, (T1)objs[0], (T2)objs[1]);
            }
        }
    }

    //无参数
    private static Dictionary<GameEvent, EventBase> dic0_event = new Dictionary<GameEvent, EventBase>();
    public static void RegisterEvent(GameEvent type, VoidCall<GameEvent> call) {
        EventBase info;
        if (dic0_event.TryGetValue(type, out info) == false) {
            info = new EventInfo {
                NetCmdType = type,
            };
            dic0_event.Add(type, info);
        }
        (info as EventInfo).call_list.Add(call);
    }
    public static void UnRegisterEvent(GameEvent type, VoidCall<GameEvent> call) {
        EventBase info;
        if (dic0_event.TryGetValue(type, out info)) {
            EventInfo e1 = info as EventInfo;
            if (e1.call_list.Remove(call)) {
                if (e1.call_list.Count == 0) {
                    dic0_event.Remove(type);
                }
            } else {
                LogMgr.LogError("找不到当前类型事件：" + type);
            }
        } else {
            LogMgr.LogError("找不到指定事件：" + type);
        }
    }
    public static bool Notifiy(GameEvent type) {
        EventBase info;
        if (dic0_event.TryGetValue(type, out info)) {
            info.Exec();
            return true;
        } else {
            return false;
        }
    }
    //单个参数
    private static Dictionary<GameEvent, EventBase> dic1_event = new Dictionary<GameEvent, EventBase>();
    public static void RegisterEvent(GameEvent type, VoidCall<GameEvent, object> call) {
        EventBase info;
        if (dic1_event.TryGetValue(type, out info) == false) {
            info = new EventInfo<object>{
                NetCmdType = type,
            };
            dic1_event.Add(type, info);
        }
        (info as EventInfo<object>).call_list.Add(call);
    }
    public static void UnRegisterEvent(GameEvent type, VoidCall<GameEvent, object> call) {
        EventBase info;
        if (dic1_event.TryGetValue(type, out info)) {
            EventInfo<object> e1 = info as EventInfo<object>;
            if (e1.call_list.Remove(call)) {
                if (e1.call_list.Count == 0) {
                    dic1_event.Remove(type);
                }
            } else {
                LogMgr.LogError("找不到当前类型事件：" + type);
            }
        } else {
            LogMgr.LogError("找不到指定事件：" + type);
        }
    }
    public static bool Notifiy(GameEvent type, object obj) {
        EventBase info;
        if (dic1_event.TryGetValue(type, out info)) {
            info.Exec(obj);
            return true;
        } else {
            return false;
        }
    }
    //两个参数
    private static Dictionary<GameEvent, EventBase> dic2_event = new Dictionary<GameEvent, EventBase>();
    public static void RegisterEvent<T1,T2>(GameEvent type, VoidCall<GameEvent, T1,T2> call) {
        EventBase info;
        if (dic2_event.TryGetValue(type, out info) == false) {
            info = new EventInfo<T1,T2> {
                NetCmdType = type,
            };
            dic2_event.Add(type, info);
        }
        (info as EventInfo<T1, T2>).call_list.Add(call);
    }
    public static void UnRegisterEvent<T1, T2>(GameEvent type, VoidCall<GameEvent, T1, T2> call) {
        EventBase info;
        if (dic2_event.TryGetValue(type, out info)) {
            EventInfo<T1, T2> e2 = info as EventInfo<T1, T2>;
            if (e2.call_list.Remove(call)) {
                if (e2.call_list.Count == 0) {
                    dic2_event.Remove(type);
                }
            } else {
                LogMgr.LogError("找不到当前类型事件：" + type);
            }
        } else {
            LogMgr.LogError("找不到指定事件：" + type);
        }
    }
    public static bool Notifiy<T1, T2>(GameEvent type, T1 t1, T2 t2) {
        EventBase info;
        if (dic2_event.TryGetValue(type, out info)) {
            info.Exec(t1, t2);
            return true;
        } else {
            return false;
        }
    }


}
