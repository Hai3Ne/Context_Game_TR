//#define SHOWLOG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Kubility
{

	public class KAssetRefPool :  SingleTon <KAssetRefPool>
    {
        private bool BeingUnload;


        private long ManagedLoadCount;

        public long LoadCount
        {
            get
            {
                return ManagedLoadCount;
            }
        }

        private Dictionary<string, AssetObjectInterface> CurentPool;

        private Queue<AssetObjectInterface> OldPool;

        private Queue<AssetObjectInterface> PrePool;

        public KAssetRefPool()
        {
            this.CurentPool = new Dictionary<string, AssetObjectInterface>();

            this.OldPool = new Queue<AssetObjectInterface>();

            this.PrePool = new Queue<AssetObjectInterface>();

            this.ManagedLoadCount = 0;

            this.BeingUnload = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The get.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T TryGet<T>(string Key) where T : class, AssetObjectInterface
        {
            if (CurentPool.ContainsKey(Key) && CurentPool[Key] != null)
            {
             // LogMgr.Log("---11 "+ Key);
                return (T)CurentPool[Key];
            }
            else
            {
              // LogMgr.Log("---22" + Key);
                if (OldPool.Count > 0)
                {
                   // LogMgr.Log("---33" + Key);
                    var node = OldPool.Dequeue();
                    //LogMgr.Log("---44" + Key + " -> "+node.AssetName);
                    node.Reset();
                   // LogMgr.Log("---55" + Key);
                    return (T)node;

                }
                //LogMgr.Log("---66");
                return null;
            }

        }

        public T TryGet<T>(string Key, ref bool Old) where T : class, AssetObjectInterface
        {
            if (CurentPool.ContainsKey(Key))
            {
                //LogMgr.Log("---111 " + Key);
                Old = false;
                return (T)CurentPool[Key];
            }
            else
            {
                 //LogMgr.Log("---222" + Key);
                if (OldPool.Count > 0)
                {
                    //LogMgr.Log("---333" + Key);
                    var node = OldPool.Dequeue();
                    //LogMgr.Log("---444" + Key);
                    node.Reset();
                    //LogMgr.Log("---555" + Key);
                    Old = true;
                    return (T)node;

                }
                //LogMgr.Log("---666" + Key);
                Old = false;
                return null;
            }

        }

        public AssetObjectInterface TryGet(ABData Key)
        {
            return TryGet<AssetObjectInterface>(Key.Abname);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset">Asset.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void PushToPool<T>(T asset) where T : class, AssetObjectInterface
        {

            if (!this.BeingUnload && asset != null)
            {

                if (CurentPool.ContainsKey(asset.AssetName))
                {
                    AssetObjectInterface node = CurentPool[asset.AssetName];
                    if (asset.Size != node.Size)
                    {

                        if (node.Size > 0)
                        {
                            ManagedLoadCount -= node.Size;
                        }

                        if (asset.Size > 0)
                            ManagedLoadCount += asset.Size;
                        else
                        {
							LogMgr.LogError("错误的资源大小"+ asset.AssetName);
                        }
                    }

                    node.Update(asset);


                }
                else
                {
                    CurentPool.Add(asset.AssetName, asset);
                    //可能会有计算错误的问题 当remove的常驻ref加入
                    if (asset.Size > 0)
                        ManagedLoadCount += asset.Size;

                }
#if SHOWLOG
                //LogUtils.Log ("一代缓存池数量为 ", CurentPool.Count.ToString (), " 二代对象对象池数量", OldPool.Count.ToString (), "当前总资源加载字节数", LoadCount.ToString () + "字节");
#endif
            }
            else if (asset != null)
            {

                if (!PrePool.Contains(asset))
                {
                    PrePool.Enqueue(asset);
                }

            }

        }


        public void PopPool<T>(T asset) where T : AssetObjectInterface, IEquatable<T>
        {
            if (CurentPool.Remove(asset.AssetName))
            {
                OldPool.Enqueue(asset);
                ManagedLoadCount -= asset.Size;

            }
#if SHOWLOG
            LogUtils.Log("销毁资源一代缓存池数量为 ", CurentPool.Count.ToString(), " 二代对象对象池数量", OldPool.Count.ToString(), "当前总资源加载字节数", LoadCount.ToString() + "字节", asset.AssetName);
#endif
        }

        public void DumpLog()
        {
            BeingUnload = true;
			LogMgr.Log("当前总资源加载字节数"+LoadCount.ToString() + "字节, 一代缓存池数量为 "+CurentPool.Count.ToString()+ " 二代对象对象池数量"+ OldPool.Count.ToString());


            this.CurentPool.ForEach((p) =>
            {

				LogMgr.Log("一代缓存池, 资源名：" + p.AssetName+ "大小为：" + p.Size.ToString()+ "时间戳" + p.TimeStamp.ToString());

                AssetLoaderDataInterface dataInter = p as AssetLoaderDataInterface;
                if (dataInter != null)
                {
					LogMgr.Log("一代缓存池, 资源名：" + dataInter.DataRef.AssetName+ "计数：" + dataInter.DataRef.refCount
                            +"文件类型" + dataInter.DataRef.FileType.ToString()+"节点类型 " + dataInter.DataRef.NodeTag.ToString());
                }
                else
                {
                    AssetObjectInterface AssetObj = p as AssetObjectInterface;
                    if (AssetObj != null)
                    {
						LogMgr.Log("一代缓存池, 资源名：" + AssetObj.AssetName+"计数：" + AssetObj.refCount+"文件类型" + AssetObj.FileType+"节点类型 " + AssetObj.NodeTag.ToString());
                    }
                }
            });

            this.OldPool.ForEach((p) =>
            {

				LogMgr.Log("二代对象对象池, 资源名：" + p.AssetName+ "大小为：" + p.Size+"时间戳" + p.TimeStamp.ToString());

                AssetLoaderDataInterface dataInter = p as AssetLoaderDataInterface;
                if (dataInter != null)
                {
					LogMgr.Log("二代对象对象池,资源名：" + dataInter.DataRef.AssetName+"计数：" + dataInter.DataRef.refCount+"文件类型" + dataInter.DataRef.FileType+"节点类型 " + dataInter.DataRef.NodeTag);
                }
                else
                {
                    AssetObjectInterface AssetObj = p as AssetObjectInterface;
                    if (AssetObj != null)
                    {
						LogMgr.Log("一代缓存池,资源名：" + AssetObj.AssetName+ "计数：" + AssetObj.refCount+ "文件类型" + AssetObj.FileType.ToString()+"节点类型 " + AssetObj.NodeTag.ToString());
                    }
                }
            });

            BeingUnload = false;
        }

        public void UnLoadAll()
        {
            if (BeingUnload)
            {
				LogMgr.Log("过于频繁的清理对象了~!");
                return;
            }
			else if (!KAssetDispather.Instance.CanUnLoad)
            {
				LogMgr.Log("场景正在加载拒绝异常卸载!");
                return;
            }
            else if (!ABLoadConfig.Editor_MODE)
            {
                BeingUnload = true;
#if SHOWLOG
                LogUtils.Log("卸载前当前ab内存占用", ManagedLoadCount.ToString());
#endif
                UnLoadCurentPool(true);
                UnLoadOldPool();
                UnLoadPrePool();
#if SHOWLOG
                LogUtils.Log("卸载之后ab内存占用", ManagedLoadCount.ToString());
#endif
                BeingUnload = false;
            }


        }

        public void UnLoadUnUsed()
        {

            if (BeingUnload)
            {
				LogMgr.Log("过于频繁的清理对象了~!");
                return;
            }
			else if (!KAssetDispather.Instance.CanUnLoad)
            {
				LogMgr.Log("场景正在加载拒绝异常卸载!");
                return;
            }
            else if (!ABLoadConfig.Editor_MODE)
            {
                BeingUnload = true;
#if SHOWLOG
                LogUtils.Log("卸载前当前ab内存占用", ManagedLoadCount.ToString());
#endif

                long half = ABLoadConfig.Asset_POOL_MaxSize / 2;
                long Neast = ABLoadConfig.Asset_POOL_MaxSize * 2 / 3;

                float delayTime = 0.1f;

                //if (KAssetBundleManger.CanClose)
                //    KAssetBundleManger.CloseApk();

                if (ManagedLoadCount < half)
                {
                    UnLoadCurentPool();

                }
                else if (ManagedLoadCount < Neast)
                {
                    UnLoadCurentPool();
                    UnLoadOldPool();
                    delayTime += 0.1f;
                }
                else
                {
                    LogMgr.Log("内存警告  ");
                    //危险 
                    UnLoadCurentPool(true);
                    UnLoadOldPool();
                    //delayTime += 1f;
                }
#if SHOWLOG
                LogUtils.Log("卸载之后ab内存占用", ManagedLoadCount.ToString());
#endif

                BeingUnload = false;
                while (PrePool.Count > 0)
                {
                    AssetObjectInterface asset = PrePool.Dequeue();
                    if (asset != null)
                    {
                        PushToPool<AssetObjectInterface>(asset);
                    }
                }
            }


        }

        private void UnLoadCurentPool(bool strong = false)
        {
            //long time = KAssetBundleManger.GetTimeStamp();
            //Todo
            List<string> keys = new List<string>(CurentPool.Keys);
            const int SingleDestroy = 8;
            int CurrentIndex = 0;

            for (int i = 0; i < keys.Count; ++i)
            {
                string key = keys[i];

                if (CurentPool.ContainsKey(key))
                {
                    AssetObjectInterface asset = CurentPool[key];

                    if (asset.refCount == 0 || strong)
                    {

                        int state = (int)asset.State;
                        int preload = (int)RefState.PRELOAD;
                        int result = state & preload;

                        if (result == preload && !strong)
                        {
                            LogMgr.Log("---------------这是预加载资源------------  " + asset.AssetName);
                            asset.SetPreRes(false);
                        }
                        else if (asset.refCount == 0 && asset.NodeTag != ABNodeTag.STANDALONE && asset.NodeTag != ABNodeTag.DONTDESTROY && asset.NodeTag != ABNodeTag.SCENEASSET)
                        {
                            asset.UnLock();
                            asset.UnLoad(strong);
                        }

                        if (asset.refCount == 0 || strong)
                        {
                            CurrentIndex++;
                            if (CurrentIndex > SingleDestroy && !strong)
                                break;
                        }

                    }
                }

            }



            //            CurentPool.Sort((a, b) =>
            //            {

            //                return (int)(b.TimeStamp - a.TimeStamp);
            //            });

            //            const int SingleDestroy = 8;
            //            int CurrentIndex = 0;
            //            if (CurentPool.Count > 0)
            //            {
            //                long MaxTimeStamp = CurentPool[0].TimeStamp;

            //                for (int i = CurentPool.Count - 1; i >= 0; i--)
            //                {
            //                    if (i >= CurentPool.Count)
            //                    {
            //                        continue;
            //                    }
            //                    AssetObjectInterface asset = CurentPool[i];
            //                    long TimeStamp = asset.TimeStamp;
            //                    long delta = MaxTimeStamp - TimeStamp;

            //#if SHOWLOG
            //                    if (i == CurentPool.Count - 1)
            //                        LogUtils.Log("最大时间差", delta.ToString());

            //#endif


            //                    if (asset.refCount == 0 || strong || delta >= ABLoadConfig.Default_DeltaTimeSpan)
            //                    {
            //                        if (asset.refCount == 0 && asset.NodeTag != ABNodeTag.STANDALONE && asset.NodeTag != ABNodeTag.DONTDESTROY)
            //                        {
            //                            asset.UnLock();
            //                        }

            //                        asset.UnLoad(false);
            //                        if (asset.refCount == 0 || strong)
            //                        {
            //                            CurrentIndex++;
            //                            if (CurrentIndex > SingleDestroy && !strong)
            //                                break;
            //                        }

            //                    }

            //                }
            //            }


        }

        private void UnLoadOldPool()
        {
            while (OldPool.Count > 0)
            {
                AssetObjectInterface asset = OldPool.Dequeue();

                if (asset != null)
                    asset.UnLoad(true);
            }
        }

        private void UnLoadPrePool()
        {
            while (PrePool.Count > 0)
            {
                AssetObjectInterface asset = PrePool.Dequeue();

                if (asset != null)
                    PushToPool<AssetObjectInterface>(asset);
            }
        }

    }
}
