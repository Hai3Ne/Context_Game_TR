using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VersionJsonInfo  {

    public int MainVersionID;
    public int SubVersionID;
    public int ThirdVersionID;
    public Dictionary<string,string> AndroidDownLoadList = new Dictionary<string,string>();
    public Dictionary<string,string> IOSDownLoadList = new Dictionary<string,string>();
	public Dictionary<string,string> StandAloneList = new Dictionary<string,string>();

    public Dictionary<string,string> getList()
    {
        if(KApplication.isIOS)
            return IOSDownLoadList;
        else if(KApplication.isAndroid)
            return AndroidDownLoadList;
        else
			return StandAloneList;
    }
}
