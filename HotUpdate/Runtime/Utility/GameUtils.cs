using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net;
using Kubility;
using ICSharpCode.SharpZipLib.GZip;
using UnityEngine.Networking;

public static class GameUtils 
{
	public static uint ClientVer
	{
		get { return 10000;}
	}

    public static uint ConvertToVersion(byte product_ver, byte main_ver, byte sub_ver, byte build_ver) {
        return (uint)((product_ver << 24) + (main_ver << 16) + (sub_ver << 8) + build_ver);
    }

	public static T CreateGo<T>(string resourceID, Transform parentCon){
		GameObject prefab = Resources.Load<GameObject> (resourceID);
		GameObject go = GameObject.Instantiate (prefab);
		go.transform.SetParent (parentCon);
		return go.GetComponent<T> ();
	}
    public static string GetUnID() {//获取唯一ID
        string dev_id = PlayerPrefs.GetString("dev_un_id", string.Empty);
        if (string.IsNullOrEmpty(dev_id)) {
            int id = UnityEngine.Random.Range(1000000, 10000000);
            dev_id = id.ToString("x8");
            PlayerPrefs.SetString("dev_un_id", dev_id);
            PlayerPrefs.Save();
        }
        return dev_id;
    }
	public static string GetMachineID(){
        return CalMD5(GameUtils.GetUnID());
	}

	public static GameObject CreateGo(UnityEngine.Object prefab){ return CreateGo (prefab, null, Vector3.zero, Quaternion.identity);	}
	public static GameObject CreateGo(UnityEngine.Object prefab, Transform parentCon){ return CreateGo (prefab, parentCon, Vector3.zero, Quaternion.identity);	}
	public static GameObject CreateGo(UnityEngine.Object prefab, Transform parentCon, Vector3 startPos, Quaternion rot)
	{
		if (prefab == null)
			return null;
		GameObject go = GameObject.Instantiate (prefab, startPos, rot) as GameObject;
		go.SetActive (true);
		if (parentCon != null)
			go.transform.SetParent (parentCon);
		go.transform.localScale = Vector3.one;
		return go;
	}

	public static string StringFormat(string format, params object[] args)
	{
		return string.Format(format, args);
	}

	public static string GetDateTime(long time)
	{
		DateTime nowTime = DateTime.Now;
		DateTime dateTime = nowTime.AddSeconds (time).ToLocalTime ();
		return dateTime.ToString ("yyyy-MM-dd HH:mm");
	}

	public static string GetDateTimeNow()
	{
		return DateTime.Now.ToString("t");
	}


	public static Texture2D CaptureCamera(Camera camera, Rect rect, int mapId)
	{
		RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
		camera.targetTexture = rt;
		camera.Render();
		RenderTexture.active = rt;
		Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
		screenShot.ReadPixels(rect, 0, 0);
		screenShot.Apply();
		camera.targetTexture = null;
		RenderTexture.active = null;
		GameObject.Destroy(rt);
		return screenShot;
	}


    private static IEnumerator _down_load_file(string downloadpath, string localpath, Action<bool> call) {
        UnityWebRequest www = UnityWebRequest.Get(downloadpath);

        yield return www.Send();

        if (www.isDone && string.IsNullOrEmpty(www.error)) {
            LogMgr.Log("下载完成:" + localpath);
            File.WriteAllBytes(localpath, www.downloadHandler.data);
            call(true);
        } else {
            LogMgr.Log("下载失败:" + localpath + "   error:" + www.error);
            call(false);
        }
    }
    public static void DownLoadFile(MonoBehaviour mono, string downloadpath, string localpath, Action<bool> call) {
        mono.StartCoroutine(_down_load_file(downloadpath, localpath, call));
    }
    private static IEnumerator _down_load_text(string downloadpath, string localpath, Action<bool, string> call) {
        UnityWebRequest www = UnityWebRequest.Get(downloadpath);

        yield return www.Send();

        if (www.isDone && www.responseCode == 200 && string.IsNullOrEmpty(www.error)) {
            LogMgr.Log("下载完成:" + downloadpath);
            if (string.IsNullOrEmpty(localpath) == false) {
                File.WriteAllBytes(localpath, www.downloadHandler.data);
            }
            call(true, www.downloadHandler.text);
        } else {
            LogMgr.Log("下载失败:" + downloadpath + "   error:" + www.error);
            call(false,www.error);
        }
    }
    public static void DownLoadTxt(MonoBehaviour mono, string downloadpath, string localpath, Action<bool, string> call) {
        mono.StartCoroutine(_down_load_text(downloadpath, localpath, call));
    }
    private static IEnumerator __down_load_www(string url, Action<UnityWebRequest> call) {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.Send();
        call(www);
    }
    public static void DownLoadWWW(MonoBehaviour mono, string url, Action<UnityWebRequest> call) {
        mono.StartCoroutine(__down_load_www(url, call));
    }
    private static IEnumerator __down_load_texture(UITexture texture, string url) {
        int count = 0;
        do {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.Send();
            Texture2D t = new Texture2D(texture.width, texture.height);
            if (t.LoadImage(www.downloadHandler.data)) {
                texture.mainTexture = t;
                break;
            } else {
                LogMgr.LogError("down img error:" + www.error);
            }
        } while (count++ < 3);//失败重复三次
    }
    public static void DownLoadTexture(MonoBehaviour mono, UITexture texture, string url) {
        mono.StartCoroutine(__down_load_texture(texture, url));

    }
	static Dictionary<string, HttpWebRequest> ReqDict = new Dictionary<string, HttpWebRequest> ();

	public static void Close ()
	{
		List<string> Keys = new List<string> (ReqDict.Keys);

		for (int i = 0; i < Keys.Count; ++i) {
			HttpWebRequest req = ReqDict [Keys [i]];
			if (req != null) {
				req.Abort ();
				LogMgr.Log ("下载终止 -> " + Keys [i]);
			}
		}

		ReqDict.Clear ();
		ReqDict = null;
	}

	public static void ResumeFromBreakPoint (string downloadpath, string localpath, LoaderHandler.ProgressHandler handler,bool sk = false)
	{
		HttpRequestUtils.MethodToAccessSSL ();
		long filelen = 0;
		string Dirname = Path.GetDirectoryName (localpath);
		if (!Directory.Exists (Dirname)) {
			Directory.CreateDirectory (Dirname);
		}

		if (File.Exists (localpath)) {
			FileInfo file = new FileInfo (localpath);
			if (file == null) {
				LogMgr.LogError ("或许被意外删除 " + localpath);
			} else {
				var delta = System.DateTime.Now - file.LastWriteTime;
				if (delta.TotalDays >= 1) {
					LogMgr.LogError ("这是一个特殊的文件，hash名字相同，但是文件改变过了，是历史版本遗留的文件！");
					file.Delete ();
				} else {
					filelen = file.Length;
				}
			}
		}

		Stream DataStream = null;
		ThreadPool.QueueUserWorkItem ((cbvalue) => {
			try {
				using (DataStream = new FileStream (localpath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
					long oldSize = 0;
					LogMgr.Log(downloadpath);
					HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (downloadpath);
//					if (filelen > 0 && !sk) {
//						HttpWebRequest .AddRange ("bytes", (int)filelen);
//					}

					request.Timeout = 5000;
					request.ReadWriteTimeout = 5000;
					request.KeepAlive = false;
					request.AllowWriteStreamBuffering = false;
					request.AllowAutoRedirect = true;
					request.AutomaticDecompression = DecompressionMethods.None;

					//目前不可能同时多个线程，所以不做lock，提高性能
					//lock ()
					ReqDict.Add (downloadpath, request);
					LogMgr.Log("downloadpath "+downloadpath);
					HttpWebResponse response = (HttpWebResponse)request.GetResponse ();

					if (response.Headers ["Content-Range"] == null)
						oldSize = 0;

					//long to float   may cause error
					long totalSize = response.ContentLength + oldSize;
					using (Stream stream = response.GetResponseStream ()) {
						if (DataStream != null && oldSize > 0) {
							DataStream.Seek (0, SeekOrigin.End);
						}
						Read (DataStream, stream, oldSize, totalSize, handler);
					}
					LogMgr.Log ("下载完成 -> " + downloadpath + " Size = " + totalSize);

					if (DataStream != null)
						DataStream.Close ();
					response.Close ();
					request.Abort ();
				}
			} catch (Exception ex) {
				LogMgr.LogError (ex);
				handler.TryCall ("100%", 1, 1, "", ex);
			} finally {
				ReqDict.Remove (downloadpath);
				if (DataStream != null)
					DataStream.Close ();
			}
		});
	}

	static void Read (Stream DataIntStream, Stream DataOutStream, long oldSize, long totalSize, LoaderHandler.ProgressHandler handler)
	{
		byte[] bys = new byte[8192];
		var convert = new ByteBuffer((int)bys.Length);

		int readLen = DataOutStream.Read (bys, 0, bys.Length);
        //int maxLen = readLen;
		long total = readLen + oldSize;
		while (readLen > 0) 
		{
			if (DataIntStream != null)
				DataIntStream.Write (bys, 0, readLen);

			convert.write(bys,readLen);
			if (handler != null && total != totalSize) {
				float progress = (float)total / totalSize;
				progress = (int)(progress * 100) * 0.01f;
				handler.TryCall (string.Format ("{0}%  ", progress * 100), total, totalSize, null, null);
			}

			readLen = DataOutStream.Read (bys, 0, bys.Length);

			total += readLen;
		}

		string s = System.Text.Encoding.Default.GetString (convert.ConverToBytes());
		if (handler != null)
			handler.TryCall (s, totalSize, totalSize, null, null);
	}
    
	//如果角度在指定范围内
	const float LOCK_FISH_DIST_SQR = 0.07f * 0.07f;
	public static bool CheckLauncherAngle(Vector3 ScreenPos, Vector3 ViewPos, Vector3 scrStartPoint, Vector3 viewStartPoint)
	{
		Vector3 dir = ScreenPos - scrStartPoint;
		dir.Normalize();
		float dot = Vector2.Dot(dir, Vector2.up);
		if (dot < -0.5f && (ViewPos - viewStartPoint).sqrMagnitude > LOCK_FISH_DIST_SQR)
		{
			return false;
		}
		return true;
	}
	public static bool IsInScreen(Vector3 scrPos)
	{
		if (scrPos.x < 0 || scrPos.x > Screen.width || scrPos.y < 0 || scrPos.y > Screen.height)
		{
			return false;
		}
		return true;
	}

	//
	public static Vector3 WorldToNGUI(Vector3 pos, Transform uiTrans = null)
	{
		pos = Camera.main.WorldToScreenPoint(pos);
		pos.z = 0;
		pos = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(pos);
		pos.z = 0f;
		if (uiTrans != null)
			pos = uiTrans.InverseTransformPoint (pos);
		return pos;
	}

	public static Vector3 NGUIToWorld(Vector3 pos, float z = 0)
	{
		pos.z = 0;
		pos = SceneObjMgr.Instance.UICamera.WorldToScreenPoint(pos);              //NGUI坐标转到Screen标
		pos.z = ConstValue.NEAR_Z + z;                  //Z轴等与主像机近裁剪面
		return Camera.main.ScreenToWorldPoint(pos);
	}

	public static string IntToIp(long ipInt)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append((ipInt >> 24) & 0xFF).Append(".");
        sb.Append((ipInt >> 16) & 0xFF).Append(".");
        sb.Append((ipInt >> 8) & 0xFF).Append(".");
        sb.Append(ipInt & 0xFF);
        return sb.ToString();
    }

	public static void SplitShort2Byte(ushort shortVal, out byte byte0, out byte byte1)
	{
		byte0 = (byte)shortVal;
		byte1 = (byte)(shortVal >> 8);
	}

	public static bool Interect(this Vector4 rect, Vector4 r2)
	{
		bool xInterect = rect.x > r2.x && rect.x < r2.z || rect.z > r2.x && rect.z < r2.z || (rect.x <= r2.x && rect.z >= r2.z);
		bool yInterect = rect.y > r2.w && rect.y < r2.y || rect.w > r2.w && rect.w < r2.y || (rect.y >= r2.y && rect.w <= r2.w);

		return xInterect && yInterect;
	}

	public static bool InterectVect(Vector3 pos, float range, Vector3 targetPos)
	{
        float xpan = Math.Abs(pos.x - targetPos.x);
		if (xpan > range)
			return false;
        float ypan = Math.Abs(pos.y - targetPos.y);
		if (ypan > range)
			return false;
        return xpan * xpan + ypan * ypan <= range * range;
	}

	public static bool InterectVect(Vector2 pos0, float radius, Vector2 pos2, float raduis2)
	{
		float len = radius + raduis2;
		float xpan, ypan;
		xpan = Math.Abs (pos0.x - pos2.x);
		if (xpan > len)
			return false;
		ypan = Math.Abs (pos0.y - pos2.y);
		if (ypan > len)
			return false;
		return Mathf.Pow (xpan, 2f) + Mathf.Pow (ypan, 2f) > Mathf.Pow(len,2f);		
	}

	static bool InterectPloy2Pos(Vector2[] ploySides, Vector2 pos)
	{
		for (int i = 0; i < ploySides.Length; i++) {
			int k = (i == ploySides.Length - 1) ? 0 : (i + 1);
			Vector2 v0 = ploySides [k] - ploySides [i];
			Vector2 v1 = pos - ploySides [i];
			Vector3 vv = Vector3.Cross (new Vector3 (v0.x, v0.y, 0f), new Vector3 (v1.x, v1.y, 0f));
			float ang = Vector3.Dot (vv.normalized, Vector3.back);
			if (ang < 0) {
				return false;
			}
		}
		return true;
	}

	public static bool IsPointInPolygon(Vector3 tarPos, Vector3 pos0, Vector3 pos1)
	{
		Vector3 dr0 = tarPos - pos0;
		Vector3 dr1 = tarPos - pos1;
		if (Vector3.Dot (dr0.normalized, dr1.normalized) < 0.5f)
			return true;
		else {
			return dr1.sqrMagnitude < 5.0f;
		}
	}

    public static bool IsPointInPolygon(Vector3[] poly, Vector3 pt) {
        if (poly == null)
            return false;
        bool c = false;
        Vector3 pre = poly[poly.Length - 1];
        Vector3 pos;
        for (int i = 0; i < poly.Length; i++) {
            pos = poly[i];
            if (
                ((pos.y <= pt.y) != (pre.y <= pt.y))
                && (pt.x < (pre.x - pos.x) * (pt.y - pos.y) / (pre.y - pos.y) + pos.x)) {
                c = !c;
            }
            pre = pos;
        }
        return c;
    }

    static byte[][] BoxDic = new byte[][]{
		new byte[]{1,2,3,7,4,5},
		new byte[]{0,4,5,6,2,3},
		new byte[]{0,1,5,6,7,3},
		new byte[]{0,1,2,6,7,4},
        
		new byte[]{0,1,5,6,7,3},
		new byte[]{0,1,2,6,7,4},
		new byte[]{1,2,3,7,4,5},
		new byte[]{0,4,5,6,2,3},
	};
    public static void PoList2Ver2List(Vector3[] world_pos, Vector3[] vec_list) {
        byte idx = 0;
        for (byte i = 1; i < 8; i++) {
            if (world_pos[idx].z > world_pos[i].z) {
                idx = i;
            }
        }
        byte[] boxs = BoxDic[idx];
        for (int i = 0; i < boxs.Length; i++) {
            vec_list[i] = Utility.MainCam.WorldToScreenPoint(world_pos[boxs[i]]);
        }
    }

	static byte[][] linesDic = new byte[][]{
		new byte[]{1,3,4},
		new byte[]{0,2,5},
		new byte[]{1,3,6},
		new byte[]{0,2,7},

		new byte[]{0,5,7},
		new byte[]{1,4,6},
		new byte[]{2,5,7},
		new byte[]{3,4,6}
	};

    static Vector3[] TempChkVector3 = new Vector3[8];
    public static void polish2Vector2List(Vector3[] screenpos, List<Vector3> vec_list) {
        byte idx = 0;
        for (byte i = 1; i < screenpos.Length; i++) {
            if (screenpos[i].y < screenpos[idx].y)
                idx = i;
        }

        byte k = 0;
        byte now = idx, next = idx, pre = idx;

        Vector3 boudDir = Vector3.left;
        do {
            pre = now;
            now = next;
            if (k > 5)
                break;
            TempChkVector3[k] = screenpos[now];
            k++;
            next = CalNext(screenpos, now, pre, linesDic[now], ref boudDir);
        } while (next != idx);
        vec_list.Clear();
        for (int j = 0; j < k; j++) {
            vec_list.Add(TempChkVector3[j]);
        }
    }
	public static Vector3[] polish2Vector2List(Vector3[] screenpos)
	{
		byte idx = 0;
		for (byte i = 1; i < screenpos.Length; i++) {
			if (screenpos [i].y < screenpos [idx].y)
				idx = i;
		}

		byte k = 0;
		byte now = idx, next = idx, pre = idx;

		Vector3 boudDir = Vector3.left;
		do
		{
			pre = now;
			now = next;
			if (k > 5)
				break;
			TempChkVector3[k] = screenpos[now];
			k++;
			next = CalNext (screenpos, now, pre, linesDic [now], ref boudDir);
		}while(next != idx);
		Vector3[] poly = new Vector3[k];
		for (int j = 0; j < k; j++)
			poly[j] = TempChkVector3[j];
		return poly;
	}
	
	static byte CalNext(Vector3[] screenpos, byte idx, byte pre, byte[] ps, ref Vector3 boudDir)
	{
		Vector3 dir, bDir = boudDir;
		float minCos = -1f;
		byte k = ps[0];
		for (byte i = 0; i < ps.Length; i++)
		{
			if (ps [i] == pre)
				continue;
			dir = screenpos[ps[i]] - screenpos[idx];
			dir = dir.normalized;
			Vector3 crv = Vector3.Cross (boudDir, dir);
			if (crv.z <= 0f)
			{
				float cos = Vector3.Dot(boudDir, dir);
				if (cos > minCos)
				{
					k = ps[i];
					minCos = cos;
					bDir = dir;
				}
			}
		}
		boudDir = bDir;
		return k;
	}

	public static bool RectContain2D(this Vector4 rect, Vector3 pos)
	{
		return pos.x >= rect.x && pos.x < rect.z && pos.y < rect.y && pos.y >= rect.w;	
	}

	public static uint Min (params uint[] values)
	{
		uint minv = uint.MaxValue;
		for (int i = 0; i < values.Length; i++) {
			minv = Math.Min (values [i], minv);
		}
		return minv;
	}
    public static string ToTimeStr(float s) {//单位秒
        int ticks = (int)s;
        int sec = ticks % 60;
        int min = (ticks / 60) % 60;
        int hours = ticks / 3600;
        return string.Format("{0:00}:{1:00}:{2:00}", hours, min, sec);
    }
    public static string ToTimeStrToMin(float s) {//单位秒
        int ticks = (int)s;
        int sec = ticks % 60;
        int min = ticks / 60;
        return string.Format("{0:00}:{1:00}", min, sec);
    }

    public static string ToTimeStr(uint ticks) {//单位毫秒
        ticks /= 1000;
        uint sec = ticks % 60;
        uint min = (ticks / 60) % 60;
        uint hours = ticks / 3600;
        return string.Format("{0:00}:{1:00}:{2:00}", hours, min, sec);
    }
	public static string FormatTime(long ticks, string formatStr = "{0}:{1}")
	{
		TimeSpan ts = new TimeSpan (ticks);
		if (ts.Hours == 0) {
			return string.Format (formatStr, ts.Minutes <= 9 ? "0"+ts.Minutes : ts.Minutes.ToString(), ts.Seconds <= 9 ? "0"+ts.Seconds : ts.Seconds.ToString());
		}
		else
			return string.Format (formatStr, ts.Hours <= 9 ? "0"+ts.Hours : ts.Hours.ToString(), ts.Minutes <= 9 ? "0"+ts.Minutes : ts.Minutes.ToString());
	}

	public static float CalEffectObjDuratioin(GameObject effect){
		var anim = effect.GetComponentInChildren<Animator> ();
		float life = GameUtils.CalPSLife(effect);
		if (anim != null) {
            life = Math.Max(life, anim.GetCurrentAnimatorStateInfo(0).length);
		}
		return life;
	}

	public static float CalPSLife(GameObject psGo, bool isCalLoop = false)
	{
		ParticleSystem[] pslist = psGo.GetComponentsInChildren<ParticleSystem> ();
		float maxTime = 0;
		foreach (var ps in pslist) {
			if (isCalLoop == false) {
				if (ps.main.loop == true)
					continue;
			}
			float dunartion = 0f;

            if (ps.emission.rateOverTime.constantMin <= 0) {
                dunartion = ps.main.startDelayMultiplier + ps.main.startLifetimeMultiplier;
			} else {
                dunartion = ps.main.startDelayMultiplier + Mathf.Max(ps.main.duration, ps.main.startLifetimeMultiplier);
			}
			maxTime = Mathf.Max (dunartion, maxTime);
		}
		return maxTime;
	}

    //设置粒子渲染队列
    public static void SetPSRenderQueue(GameObject obj,int order_layer, int queue) {
        Renderer[] pslist = obj.GetComponentsInChildren<Renderer>();
        foreach (var randerer in pslist) {
            if (queue > 0) {
                foreach (var item in randerer.materials) {
                    item.renderQueue = queue;
                }
            }
            //if (queue > 0) {
            //    randerer.material.renderQueue = queue;
            //}
            if (order_layer > 0) {
                randerer.sortingOrder = order_layer;
            }
        }
    }
    //调整粒子Scale   pos_s:无特殊情况不需要赋值
    public static void SetPSScale(Transform tf, float scale, float pos_s = 0) {
        if (pos_s == 0) {
            tf.localScale = tf.localScale * scale;
            for (int i = 0; i < tf.childCount; i++) {
                GameUtils.SetPSScale(tf.GetChild(i), scale, 1);
            }
        } else {
            tf.localPosition /= pos_s;
            if (tf.GetComponent<ParticleSystem>() != null) {
                tf.localScale = tf.localScale * scale;

                for (int i = 0; i < tf.childCount; i++) {
                    GameUtils.SetPSScale(tf.GetChild(i), scale, pos_s);
                }
            } else {
                for (int i = 0; i < tf.childCount; i++) {
                    GameUtils.SetPSScale(tf.GetChild(i), scale, 1);
                }
            }
        }
    }

	public static void PlayPS(GameObject psGo)
	{
		if (psGo == null)
			return;
		if (psGo.activeSelf == false)
			psGo.SetActive (true);
		ParticleSystem[] pslist = psGo.GetComponentsInChildren<ParticleSystem> (true);
		Animator[] anims = psGo.GetComponentsInChildren<Animator> ();
        for (int i = 0; i < pslist.Length; i++) {
            //pslist [i].gameObject.SetActive (true);
            pslist[i].Clear();
            pslist[i].Play();
        }
		Array.ForEach (anims, x => x.TryPlay ("run",0,0));
	}

	public static void StopPS(GameObject psGo)
	{
		if (psGo == null)
			return;
		ParticleSystem[] pslist = psGo.GetComponentsInChildren<ParticleSystem> ();
		for (int i = 0; i < pslist.Length; i++) {
			pslist [i].gameObject.SetActive (false);
		}
	}



	public static void SetGOLayer(GameObject go, int layer)
	{
		go.layer = layer;
        foreach (Transform item in go.transform) {
            SetGOLayer(item.gameObject, layer);
        }
        //for (int i = 0; i < go.transform.childCount; i++) {
        //    SetGOLayer (go.transform.GetChild (i).gameObject, layer);
        //}
	}

	public static void NormalizeQuaternion (ref Quaternion q)
	{
		float sum = 0;
		for (int i = 0; i < 4; ++i)
			sum += q[i] * q[i];
		float magnitudeInverse = 1 / Mathf.Sqrt(sum);
		for (int i = 0; i < 4; ++i)
			q[i] *= magnitudeInverse;  
	}

	public static void PlayAnimator(Animator anim, string statename = "run")
	{
		if (anim == null)
			return;
		if (!anim.enabled)
			anim.enabled = true;
		if (anim.HasState(0, Animator.StringToHash(statename)))
			anim.Play(statename, 0, 0f);
	}


	public static byte[] CompressData(byte[] data)
	{
		byte[] buff = new byte[data.Length+1];
		if (data.Length < 20000) {
			buff [0] = 0x0;
			Array.Copy (data, 0, buff, 1, data.Length);
			return buff;
		}

		byte[] bytes = null;
		int orgLen = data.Length;
		using (MemoryStream ms = new MemoryStream ()) {
			using (GZipOutputStream zipOutput = new GZipOutputStream (ms)) {
				zipOutput.Write (data, 0, data.Length);
			}
			bytes = ms.ToArray ();
		}

		using (MemoryStream ms = new MemoryStream ()) {
			using (BinaryWriter bw = new BinaryWriter (ms)) {
				bw.Write ((byte)1);
				bw.Write (orgLen);
				bw.Write (bytes, 0, bytes.Length);
			}
			buff = ms.ToArray ();
		}
		return buff;
	}

	public static byte[] UnCompressData(byte[] data)
	{
		byte[] buff = new byte[data.Length - 1];
		Array.Copy (data, 1, buff, 0, buff.Length);
		if (data[0] == 0x0)
			return buff;

		int len = BitConverter.ToInt32 (buff, 0);
		using (MemoryStream ms = new MemoryStream (buff, 4, buff.Length-4)) {
			using (GZipInputStream zipOutput = new GZipInputStream (ms)) {
				buff = new byte[len];
				zipOutput.Read (buff, 0, buff.Length);
			}
		}
		return buff;
	}

	public static bool IsPrefab(GameObject effObj)
	{
		return effObj.scene.isLoaded == false;
	}

	static Shader grayShader = null;
	public static void IsGray(this UISprite sp, bool isgray)
	{
		if (grayShader == null) {
			grayShader = Shader.Find ("Unlit/Transparent Gray");
		}
		if (isgray) {
			sp.shader = grayShader;	
		} else if (sp.shader == grayShader) {
			sp.shader = sp.material.shader;
		}
	}

	public static void SetGray(GameObject go, bool isgray){
		UISprite[] sps = go.GetComponentsInChildren<UISprite> (go);
        //UISprite sp;
		for (int i = 0; i < sps.Length; i++){
            sps[i].IsGray = isgray;
            //sps [i].IsGray(isgray);
		}
	}

	public static List<T> SplitIntNumberString<T>(string str, string splitChar=","){
		if (string.IsNullOrEmpty (str) || str.Trim() == "") {
			return new List<T> ();
		}
		string[] strlist = str.Split (new string[]{ splitChar }, StringSplitOptions.None);
		List<T> numList = new List<T> ();
		for (int i = 0; i < strlist.Length; i++) {
			numList.Add (String2Number<T>(strlist [i]));
		}
		return numList;		
	}

	public static string Join2String<T>(IList<T> l, string splitChar=","){
		string str = "";
		for (int i = 0; i < l.Count; i++) {			
			str += l [i].ToString ();
			if (i < l.Count-1)
				str += splitChar;
		}
		return str;
	}

	public static T String2Number<T>(string str) {
		object o = default(T);
		System.Type dtype = typeof(T);
		if (dtype == typeof(uint))
			o = uint.Parse(str);
		else if (dtype == typeof(int))
			o = int.Parse(str);
		else if (dtype == typeof(bool))
			o = bool.Parse(str);
		else if (dtype == typeof(byte))
			o = byte.Parse(str);
		else if (dtype == typeof(short))
			o = short.Parse(str);
		else if (dtype == typeof(ushort))
			o = ushort.Parse(str);
		else if (dtype == typeof(float))
			o = float.Parse(str);
		else if (dtype == typeof(string))
			o = str;
		return (T)o;
	}

	public static string CalMD5(string str){
        if (str == null) {
            return string.Empty;
        }
		byte[] tmpbytes = System.Text.ASCIIEncoding.ASCII.GetBytes (str);
		tmpbytes = new System.Security.Cryptography.MD5CryptoServiceProvider ().ComputeHash (tmpbytes);
		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < tmpbytes.Length; i++) {
			sb.Append(tmpbytes[i].ToString("X2"));
		}
		return sb.ToString ();
	}


    public static List<Vector3> CreateFishPos(Mesh mesh, uint density) {//根据mesh顶点信息，生成鱼的坐标
        if (density == 0) {
            density = 1;
        }
        float _f = 1f / density;
        float[] cc = new float[density + 1];
        for (int i = 0; i < cc.Length; i++) {
            cc[i] = _f * i;
        }

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Vector3 pos;
        Vector3 pos_1, pos_2, pos_3;
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < triangles.Length; i += 3) {
            pos_1 = vertices[triangles[i]];
            pos_2 = vertices[triangles[i+1]];
            pos_3 = vertices[triangles[i+2]];
            for (int x = 0; x < cc.Length; x++) {
                for (int y = 0; y < cc.Length-x; y++) {
                    pos = pos_1 * cc[x] + pos_2 * cc[y] + pos_3 * cc[cc.Length-1 - x - y];
                    if (list.Contains(pos) == false) {
                        list.Add(pos);
                    }
                }
            }
        }

        return list;
    }
    public static Rect FaceUVRect(uint faceID) {//人物头像UV处理
        if (faceID > 29) {
            faceID = 0;
        }
        Rect r = new Rect();
        r.width = 0.1f;
        r.height = 1 / 3f;
        r.x = (faceID % 10) * r.width;
        r.y = (2 - faceID / 10) * r.height;
        return r;
    }

	public static GameObject ResumeShader(GameObject go) {
		Renderer[] renders = go.GetComponentsInChildren<Renderer> (true);
		foreach (var r in renders) {
			if (r == null || r.sharedMaterials == null)
				continue;
			foreach (var mt in r.sharedMaterials) {
				if (mt != null) {
                    //mt.shader = Shader.Find (ConvertSharderName(mt.shader.name));
				}
                //else
                //    LogMgr.Log (go.name);
			}
		}
		return go;
	}

	static string ConvertSharderName(string shaderName){
		if (shaderName == "Particles/Additive" || shaderName == "Particles/Additive (Soft)") {
			return "Mobile/Particles/Additive";
		} else if (shaderName == "Particles/Alpha Blended" || shaderName == "Particles/Alpha Blended Premultiply") {
			return "Mobile/Particles/Alpha Blended";
		} else if (shaderName == "Particles/Multiply" || shaderName == "Particles/Multiply (Double)")
			return "Mobile/Particles/Multiply";
		return shaderName;
	}

	public static void hidePanelBar(UISprite barBg, float time, Action cb){
        TweenWidth.Begin(barBg, time, 0).SetOnFinished(() => {
            cb.TryCall();
        });
        //ItweenGO.ValueTo<float> (barBg.width, 0, time, iTween.EaseType.linear, delegate(float obj) {
        //    barBg.width = (int)obj;
        //}, delegate {
        //    cb.TryCall();
        //});
	}

    public static void showPanelBar(UISprite barBg, float time, int width, Action cb) {
        TweenWidth.Begin(barBg, time, width).SetOnFinished(() => {
            cb.TryCall();
        });
        //ItweenGO.ValueTo<float>(0, width, time, iTween.EaseType.linear, delegate(float obj) {
        //    barBg.width = (int)obj;
        //}, delegate {
        //    cb.TryCall();	
        //});
	}

    public static string SubStringByWidth(UILabel lb, string text, int width) {
        int height = lb.fontSize;
        lb.overflowMethod = UILabel.Overflow.ClampContent;
        lb.width = width;
        lb.height = height;

        string strOut;

        if (lb.Wrap(text, out strOut, height) == false) {
            lb.overflowMethod = UILabel.Overflow.ResizeFreely;
            if (string.IsNullOrEmpty(strOut)) {
                return text;
            } else {
                return string.Format("{0}...", strOut);
            }
        } else {
            lb.overflowMethod = UILabel.Overflow.ResizeFreely;
            return text;
        }
    }

	public static string Unicode2Utf8(string strMsg){
		return System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(UnicodeToString(strMsg)));
	}

	public static byte[] Utf8GetBytes(string str){
		return System.Text.Encoding.UTF8.GetBytes (str);	
	}

    public static string ToGB2312(byte[] bytes) {
        return Encoding.GetEncoding(936).GetString(bytes);
    }

	public static string GetString(byte[] bytes, string sEcoding, string tEncoding){
		string str = System.Text.Encoding.GetEncoding (sEcoding).GetString (bytes);
		return System.Text.Encoding.GetEncoding(tEncoding).GetString(System.Text.Encoding.GetEncoding(tEncoding).GetBytes(str));
	}

	public static string UnicodeToString(string srcText)
	{
		string dst = "";
		string src = srcText;
		int len = srcText.Length / 6;
		for (int i = 0; i <= len - 1; i++)
		{
			string str = "";
			str = src.Substring(0, 6).Substring(2);
			src = src.Substring(6);
			byte[] bytes = new byte[2];
			bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
			bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
			dst += System.Text.Encoding.Unicode.GetString(bytes);
		}
		return dst;
	}

    //遍历所有节点
    public static void Traversal(Transform trans, VoidCall<string, Transform> call) {
        call(trans.name, trans);
        for (int i = 0; i < trans.childCount; i++) {
            Traversal(trans.GetChild(i), call);
        }
    }

	public static void DestroyAllChild(Transform trans){
		GameObject[] gos = new GameObject[trans.childCount];
		for (int i = 0; i < gos.Length; i++) {
			gos [i] = trans.GetChild (i).gameObject;
		}
		for (int i = 0; i < gos.Length; i++) {
			GameObject.Destroy (gos[i].gameObject);
		}
	}

	public static T FindChild<T>(GameObject obj, string name) where T : UnityEngine.Component {
		T[] comps = obj.GetComponentsInChildren<T> (true);
		for (int i = 0; i < comps.Length; i++) {
			if (comps [i].gameObject.name == name) {
				return comps [i];
			}
		}
		return null;
	}

	public static string FormatGoldNum(int goldNum){
		if (goldNum >= 10000) {
			string str = (goldNum * 1.0f / 10000).ToString ("f2");
			if (str.Substring (str.Length - 1) == "0") {
				if (str.Substring (str.Length - 2, 1) == "0")
					return str.Substring (0, str.Length - 3) + "W";
				else
					return str.Substring (0, str.Length - 1)+"W";
			}
			return str+"W";
		}
		return goldNum.ToString ();
    }
    public static float GetTime(this Animator anim,int layer = 0) {//获取当前动画当前播放时间
        var state = anim.GetCurrentAnimatorStateInfo(layer);
        return state.length * state.normalizedTime;
    }
    public static void SetTime(this Animator anim,float time,int layer = 0) {//设置指定动画时间
        var state = anim.GetCurrentAnimatorStateInfo(layer);
        anim.Update(time - state.length * state.normalizedTime);
    }
}
