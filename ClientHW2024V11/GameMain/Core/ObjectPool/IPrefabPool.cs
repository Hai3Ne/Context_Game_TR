/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : 
* @date     : 2014-11-03
*/


using UnityEngine;
using System.Collections;

namespace SEZSJ
{

public interface IPrefabPool<T> where T : Object
    {
        int UnrecycledPrefabCount { get; }

        int AvailablePrefabCount { get; }

        int AvailablePrefabCountMaximum { get; }

        T ObtainPrefabInstance();

        void RecyclePrefabInstance(T prefab);
    }

};//End SG



