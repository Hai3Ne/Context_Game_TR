using UnityEngine;
using System.Collections.Generic;



//全局更新,只有必要更新放此
public class GlobalUpdate
{
	static List<IRunUpdate> updateList = new List<IRunUpdate>();
	static List<IRunUpdate> fixedUpdateList = new List<IRunUpdate>();
	public static void RegisterUpdate(IRunUpdate runInst)
	{
		if (!updateList.Contains (runInst))
			updateList.Add (runInst);
	}

	public static void RegisterFixedUpdate(IRunUpdate runInst)
	{
		if (!fixedUpdateList.Contains (runInst))
			fixedUpdateList.Add (runInst);
	}
	public static void UnRegisterUpdate(IRunUpdate runInst)
	{
		if (updateList.Contains (runInst))
			updateList.Remove (runInst);
	}

    public static void GlobalInit()
    {
		RegisterUpdate (HttpServer.Instance);
		RegisterUpdate (FishNetAPI.Instance);
		RegisterUpdate (GlobalEffectMgr.Instance);
		RegisterUpdate (GlobalAudioMgr.Instance);
		RegisterUpdate (SDKMgr.Instance);
    }

    public static void LateUpdate(float delta)
    {
        for (int i = 0, count = updateList.Count; i < count; i++)
        {
            if(i < updateList.Count)
                updateList[i].Update(delta);
        }
    }

	public static void FixedUpdate(float delta)
	{
		short n = (short)fixedUpdateList.Count;
		for (short i = 0; i < n; i++) {
			fixedUpdateList [i].Update (delta);
		}
	}
}
