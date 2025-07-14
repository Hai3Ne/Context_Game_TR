//#define FULLLOG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Object = UnityEngine.Object;

namespace Kubility
{
    public enum RefState
    {
        UNAUTO_REF = 1,
        AUTO_REF = 2,
        PRELOAD = 4,
        LOCK = 8,
        DESTTROY = 16,
        UnPreLoad =27,
    }

    public class KAssetBundleRef : AssetObjectInterface, IEquatable<KAssetBundleRef>
    {
        private string _AssetName;

        public string AssetName
        {
            get
            {
                return _AssetName;
            }
        }

        private bool _AutoManaged;

        public bool AutoManaged
        {
            get
            {
                return _AutoManaged;
            }
        }

        private long _TimeStamp;

        public long TimeStamp
        {
            get
            {
                return _TimeStamp;
            }
        }

        private long _Size;

        public long Size
        {
            get
            {
                return _Size;
            }
        }

        private KAssetBundle _AssetBundle;

        /// <summary>
        ///	虽然并不拒绝你去获取assetbundle 但是unload的代价得自己负责后续版本将转为kassetbundle
        /// </summary>
        public KAssetBundle Bundle
        {
            get
            {

                return _AssetBundle;
            }
        }


        /// <summary>
        /// Gets the reference count.
        /// </summary>
        /// <value>The reference count.</value>
        public int refCount
        {
            get
            {

                for (int i = References.Count - 1; i >= 0; --i)
                {
                    Object weakNode = References[i];
                    //real need isalive && nullptr check?
                    if (weakNode == null)
                    {
                        References.RemoveAt(i);
                    }
                }
                return References.Count;
                ;
            }

        }


        private string _AssetLoadName;

        /// <summary>
        /// AB 中真实加载的名字
        /// </summary>
        public string AssetLoadName
        {
            get
            {
                return _AssetLoadName;
            }
        }

        private ABFileTag _FileType;

        /// <summary>
        /// The type of the file.
        /// </summary>
        public ABFileTag FileType
        {
            get
            {
                return _FileType;
            }
        }

        private ABNodeTag _ABNodeTag;

        /// <summary>
        /// The asset bundle.
        /// </summary>
        public ABNodeTag NodeTag
        {
            get
            {
                return _ABNodeTag;
            }
            set
            {

                _ABNodeTag = value;
            }
        }


        private Object _MainObject;

        public Object MainObject
        {
            get
            {
                return _MainObject;
            }
        }

        private KAssetBundleRef[] _selfDepends;

        public KAssetBundleRef[] SelfDepends
        {
            get
            {
                return _selfDepends;
            }
        }

        private ABData _Data;

        public ABData Data
        {
            get
            {
                return _Data;
            }
        }

        private RefState _State;

        public RefState State
        {
            get
            {
                return _State;
            }
        }

        private int LockCount;

        private bool BeingUnLoad;

        List<Object> References;

        public static KAssetBundleRef Create(ABData PData, AssetBundle ab, Object obj, long size)
        {
            bool isOld = false;
			KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(PData.Abname, ref isOld);


            if (node == null)
            {
                node = new KAssetBundleRef(PData, ab, obj, size);
                node.AutoRelease();
            }
            else
            {
                if (isOld)
                    node.SelfInit(PData, ab, obj, size);
                else
                {
                    node._Data = PData;
                    node._AssetName = PData.Abname;
                    node._AssetLoadName = PData.LoadName;
                    if (node._AssetBundle == null)
                        node._AssetBundle = new KAssetBundle(ab, size);

                    if (obj != null)
                        node._MainObject = obj;
                    node._ABNodeTag = (ABNodeTag)PData.RootType;
                    node._FileType = (ABFileTag)PData.FileType;
                }

            }
            return node;
        }


        public static void Destroy(string Key, bool strong = false)
        {
			KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(Key);

            if (node != null)
            {
                if (node._ABNodeTag == ABNodeTag.STANDALONE)
                {
                    node.Release(node.MainObject);
                }
                node.UnLock();
                node.UnLoad(strong);
                Resources.UnloadUnusedAssets();
            }
            else
            {
				LogMgr.LogError("未发现该资源 "+Key);
            }

        }

        public KAssetBundleRef()
        {
            Reset();

        }

        public KAssetBundleRef(ABData pData, AssetBundle ab, Object obj, long size)
        {
            SelfInit(pData, ab, obj, size);
        }

        private void SelfInit(ABData pData, AssetBundle ab, Object obj, long size)
        {
            this._State = RefState.UNAUTO_REF;
            this._Data = pData;
            this._AssetName = Data.Abname;
            this._MainObject = obj;

            this.LockCount = 0;
            this._AssetLoadName = Data.LoadName;
            this._TimeStamp = KAssetBundleManger.GetTimeStamp();
            this._AutoManaged = false;

            this._Size = size;
            this._AssetBundle = new KAssetBundle(ab, size);

            this._ABNodeTag = (ABNodeTag)Data.RootType;
            this._FileType = (ABFileTag)Data.FileType;

            this.BeingUnLoad = false;

            if (this.References == null)
                this.References = new List<Object>();



            if (!ABLoadConfig.Editor_MODE)
            {
                List<KAssetBundleRef> depends = new List<KAssetBundleRef>();
                //牺牲一点开销，不在加载时候插入了
                for (int i = 0; i < Data.MyNeedDepends.Count; ++i)
                {
					KAssetBundleRef DepRef = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(Data.MyNeedDepends[i]);
                    if (DepRef != null)
                    {
                        depends.Add(DepRef);
                    }
                    else
                    {
						LogMgr.LogError("没有找到依赖引用，自身依赖引用缺失  >> " + Data.MyNeedDepends[i]);
                    }

                }


                this._selfDepends = depends.ToArray();
            }


        }

        public void Release(Object gameuser) {
            if (gameuser != null && State != RefState.DESTTROY) {
#if FULLLOG
                LogMgr.Log("引用计数 - 1 " + this.AssetName + " cur =" + References.Count + " next =" + (References.Count - 1));
#endif
                for (int i = 0; i < References.Count; ++i) {
                    Object weakNode = References[i];
                    //real need isalive && nullptr check?
                    if (weakNode != null && weakNode == gameuser) {
                        References.RemoveAt(i);
                        break;
                    }
                }
            } else {
                //LogUtils.LogError("需要Release的游戏对象为空");
            }
        }

        public void Retain(Object gameuser)
        {
            if (gameuser != null)
            {

                if (BeingUnLoad)
                {
					LogMgr.Log("正在卸载资源 retain被拒绝"+ gameuser.ToString());

                }
                else
                {
#if FULLLOG
                    LogMgr.Log("引用计数加1 " + this.AssetName + "  Current = " + (References.Count + 1).ToString());
#endif
                    //允许多个同一对象

                    References.Add(gameuser);

                }


            }
            else
            {
				LogMgr.LogError("需要Retain的游戏对象为空");
            }
        }

        public void AutoRelease()
        {
            if (!_AutoManaged)
            {
				KAssetRefPool.Instance.PushToPool<KAssetBundleRef>(this);
                _AutoManaged = true;
            }

        }

        public void Update(AssetObjectInterface newObject)
        {
            KAssetBundleRef asset = newObject as KAssetBundleRef;

            if (asset._AssetBundle.LogNull())
                this._AssetBundle = asset._AssetBundle;

            if (newObject.Size > 0)
            {
                this._Size = asset.Size;
                if (_AssetBundle != null)
                    this._AssetBundle.UpdateSize(newObject.Size);
            }

            this._ABNodeTag = asset._ABNodeTag;

            if (newObject.MainObject != null)
                this._MainObject = newObject.MainObject;

            if (asset.TimeStamp <= 0)
            {
				LogMgr.LogError("新对象的timestamp 异常");
            }
            else
                this._TimeStamp = asset.TimeStamp;


        }
        /// <summary>
        /// 资源释放锁
        /// </summary>
        public void Lock()
        {
            LockCount |= 0x1;

            this._State |= RefState.LOCK;
        }

        public void SetPreRes(bool state = true)
        {
            if (state)
                this._State |= RefState.PRELOAD;
            else
                this._State &= RefState.UnPreLoad; 
        }

        public void UnLock()
        {
            int flag = LockCount & 0x1;
            int UnLoadFValue = LockCount & 0x2;
            int UnLoadTValue = LockCount & 0x4;
            if (flag == 1 && UnLoadTValue == 1)
            {
                UnLoad(true);
            }
            else if (flag == 1 && UnLoadFValue == 1)
            {
                UnLoad(false);
            }

            LockCount = 0;

            this._State |= AutoManaged ? RefState.AUTO_REF : RefState.UNAUTO_REF;
        }

        /// <summary>
        /// false 为尝试卸载  true 为强势卸载
        /// </summary>
        /// <param name="strong">If set to <c>true</c> strong.</param>
        public void UnLoad(bool strong)
        {
            int flag = LockCount & 0x1;
            if (flag == 1)
            {
                if (strong)
                    LockCount |= 0x2;
                else
                    LockCount |= 0x4;

            }
            else if (State != RefState.DESTTROY && !ABLoadConfig.Editor_MODE)
            {
                BeingUnLoad = true;

                if (Bundle != null || NodeTag == ABNodeTag.SCENEASSET)
                {
#if FULLLOG
                    LogUtils.Log("准备进行检查的资源  ", this.AssetName, this.NodeTag.ToString(), refCount.ToString());
#endif
                    if (refCount == 0)
                    {
                        if ((NodeTag == ABNodeTag.STANDALONE || NodeTag == ABNodeTag.DONTDESTROY || NodeTag == ABNodeTag.SCENEASSET) && strong && SelfDepends != null)
                        {
                            for (int i = 0; i < SelfDepends.Length; ++i)
                            {
                                KAssetBundleRef DepRef = SelfDepends[i];
                                if (DepRef != null)
                                {
                                    DepRef.Release(DepRef.MainObject);

                                    // LogUtils.Log("------依赖卸载assetbundle ", DepRef.NodeTag.ToString(), DepRef.AssetName, DepRef.refCount.ToString());
                                }
                            }
                            //ngui
                            UIDrawCall.ReleaseInactive();
                        }
                        else if (NodeTag != ABNodeTag.STANDALONE && NodeTag != ABNodeTag.DONTDESTROY && NodeTag != ABNodeTag.SCENEASSET && SelfDepends != null)
                        {
                            for (int i = 0; i < SelfDepends.Length; ++i)
                            {
                                KAssetBundleRef DepRef = SelfDepends[i];
                                if (DepRef != null)
                                {
                                    DepRef.Release(DepRef.MainObject);

                                    //LogUtils.Log("------依赖卸载 ASSET  ", DepRef.NodeTag.ToString(), DepRef.AssetName, DepRef.refCount.ToString());
                                }
                            }
                        }


                       // LogUtils.Log("卸载assetbundle ", this.NodeTag.ToString(), this.AssetName);
                        if (this._AssetBundle != null)
                        {
							LogMgr.Log("卸载assetbundle "+ this.NodeTag.ToString()+this.AssetName);
#if UNITY_EDITOR
                            this._AssetBundle.Unload(false);
#else
                            this._AssetBundle.Unload(false);
#endif

                            this._AssetBundle = null;
                            this._MainObject = null;
                            this._Data = null;
                            this._selfDepends = null;

                        }


                        this._State = RefState.DESTTROY;
                        if (AutoManaged)
							KAssetRefPool.Instance.PopPool<KAssetBundleRef>(this);

                    }

                }


                BeingUnLoad = false;
            }


        }

        /// <summary>
        /// 暂时没有更好的办法，先开放这个接口
        /// </summary>
        /// <param name="obj">Object.</param>
        public void UpdateMainObject(Object obj)
        {
            if (obj != null)
                this._MainObject = obj;
        }

        public void Reset()
        {
            if (References != null && refCount > 0)
            {

				LogMgr.LogError("危险数据"+ this.AssetName);
                References.Clear();
            }
            else
            {
                References = new List<Object>();
            }

            this._State = RefState.UNAUTO_REF;
            this._Data = null;
            this._AssetName = "";

            this._AssetLoadName = "";
            this._TimeStamp = KAssetBundleManger.GetTimeStamp();
            this._AutoManaged = false;
            this._MainObject = null;

            this._Size = 0;
            this._AssetBundle = null;

            this._ABNodeTag = ABNodeTag.ASSET;
            this._FileType = ABFileTag.NONE;

            this.BeingUnLoad = false;
            this.LockCount = 0;

            this._selfDepends = null;

        }


        public bool Equals(KAssetBundleRef other)
        {
            if (AssetName.Equals(other))
                return true;
            else
                return false;
        }

		public static UnityEngine.Object GetObjectFromAssetBundle(AssetBundle assetBundle, string LoadName){
			Object asset = null;
			Object[] assets = assetBundle.LoadAllAssets ();
			for (int i = 0; i < assets.Length; i++) {
				if (assets [i].name == LoadName) {
					asset = assets [i];
					break;
				}
			}
			if (asset == null)
				asset = assets [0];
			return asset;
		}
    }


}


