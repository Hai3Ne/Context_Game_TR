using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum LoadingType {
	ALL,
	GameIn,
	MainHall,
	OnlyFishes,
}

public class GameObjDictLoadItem {
	public string resFormat;
	public Dictionary<uint, GameObject> dicts = new Dictionary<uint, GameObject>();
	public IList<uint> idList;
	public GameObjDictLoadItem(FishResType type, string str, IList<uint> idList, Dictionary<uint, GameObject> outDict)
	{
		this.resFormat = str;
		this.idList = idList;
		this.dicts = outDict;
	}
}

public class ResLoadItem
{
	public Kubility.ResType loadType;
    public GameEnum configType;
	public string resId;
	public long buffLength;
    public Action<object> finishCb;
    public bool mIsUnloadAB;//是否自动卸载ab
    public ResLoadItem(Kubility.ResType type, GameEnum configtype, string str, Action<object> finishFn, bool is_unload = false) {
		this.loadType = type;
		this.resId = str;
        configType = configtype;
        this.finishCb = finishFn;
        this.mIsUnloadAB = is_unload;
	}
}