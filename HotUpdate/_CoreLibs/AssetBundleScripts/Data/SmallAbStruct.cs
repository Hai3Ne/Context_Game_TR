using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kubility;
using System.IO;

public struct SmallAbStruct : IEquatable<SmallAbStruct>
{
    /// <summary>
    /// The tag.
    /// </summary>
    public readonly ABFileTag FileType;

    /// <summary>
    /// The main object.
    /// </summary>
    public readonly UnityEngine.Object MainObject;
    /// <summary>
    /// The path key.
    /// </summary>
    public readonly string AssetName;
    /// <summary>
    /// The main game object.
    /// </summary>

    private GameObject _MainGameObject;

    public GameObject MainGameObject
    {
        get
        {
            return _MainGameObject;
        }
    }


    public SmallAbStruct(ABData data, UnityEngine.Object obj)
    {
        FileType = (ABFileTag)data.FileType;
        if (ABLoadConfig.Editor_MODE || !Application.isPlaying)
        {
            MainObject = obj;
        }
        else
        {
            if (FileType == ABFileTag.Prefab && obj != null)
            {
                GameObject target = obj as GameObject;
                if (target != null)
                {//some gc - =!
                    MainObject = target;
                }
                else
                    MainObject = obj;
            }
            else
                MainObject = obj;
        }



        AssetName = data.Abname;
        _MainGameObject = null;

    }

    /// <summary>
    /// Determines whether this instance is null.
    /// </summary>
    /// <returns><c>true</c> if this instance is null; otherwise, <c>false</c>.</returns>
    public bool IsNull()
    {
        return MainObject == null;
    }

    /// <summary>
    /// Instantiate this instance.
    /// </summary>
    public GameObject Instantiate()
    {
        if (this.FileType != ABFileTag.Prefab)
        {
            throw new ArgumentException("不能使用非预设去实例化");
        }

        if (MainObject.LogNull())
        {
            GameObject target = MainObject as GameObject;
            if (target.LogNull())
            {


                GameObject gobj = GameObject.Instantiate(target);
                //if (gobj.LogNull())
                //{
                //    bool enable = gobj.activeSelf;
                //    AbActivityMonitor monitor = gobj.GetComponent<AbActivityMonitor>();
                //    if (monitor == null)
                //    {
                //        monitor  = gobj.AddComponent<AbActivityMonitor>();
                //    }
                //    monitor.enabled = enable;
                //    monitor.AssetName = this.AssetName;
                //    _MainGameObject = gobj;
                //}

                _MainGameObject = gobj;
                return gobj;
            }
        }
        LogMgr.LogError("Error Info>> this GameObject U want to Ins is Null");
        return null;
    }

    /// <summary>
    /// Determines whether the specified <see cref="SmallAbStruct"/> is equal to the current <see cref="SmallAbStruct"/>.
    /// </summary>
    /// <param name="other">The <see cref="SmallAbStruct"/> to compare with the current <see cref="SmallAbStruct"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="SmallAbStruct"/> is equal to the current <see cref="SmallAbStruct"/>;
    /// otherwise, <c>false</c>.</returns>
    public bool Equals(SmallAbStruct other)
    {
        //		if (!other.MainObject.Equals (MainObject))
        //			return false;
        if (!other.AssetName.Equals(AssetName))
            return false;
        if (!other.FileType.Equals(FileType))
            return false;
        //if(!other.MainGameObject.Equals(MainGameObject)) return false;
        return true;
    }
}

