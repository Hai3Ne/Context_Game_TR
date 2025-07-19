using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 网络事件管理
/// </summary>
public class NetEventManager {
    public class EventInfo {
        public NetCmdType NetCmdType;
        public List<VoidCall<NetCmdType, NetCmdPack>> call_list = new List<VoidCall<NetCmdType, NetCmdPack>>();

        public void Exec(NetCmdPack obj) {
            for (int i = 0; i < call_list.Count; i++) {
                call_list[i](this.NetCmdType, obj);
            }
        }
    }
    public static Dictionary<NetCmdType, EventInfo> dic_event = new Dictionary<NetCmdType, EventInfo>();

    public static void RegisterEvent(NetCmdType type, VoidCall<NetCmdType, NetCmdPack> call) {
        EventInfo info;
        if (dic_event.TryGetValue(type,out info) == false) {
            info = new EventInfo {
                NetCmdType = type,
            };
            dic_event.Add(type, info);
        }
        info.call_list.Add(call);
    }
    public static void UnRegisterEvent(NetCmdType type, VoidCall<NetCmdType, NetCmdPack> call) {
        EventInfo info;
        if (dic_event.TryGetValue(type, out info)) {
            if (info.call_list.Remove(call)) {
                if (info.call_list.Count == 0) {
                    dic_event.Remove(type);
                }
            } else {
                LogMgr.LogError("找不到当前类型事件：" + type);
            }
        } else {
            LogMgr.LogError("找不到指定事件：" + type);
        }
    }
    public static bool Notifiy(NetCmdType type, NetCmdPack obj) {
        EventInfo info;
        if (dic_event.TryGetValue(type, out info)) {
            info.Exec(obj);
            return true;
        } else {
            return false;
        }
    }
}
