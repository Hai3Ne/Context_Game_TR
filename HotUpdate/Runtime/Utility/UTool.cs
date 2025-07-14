using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class  UTool 
{
    public static uint GetTickCount()
    {
        return (uint)TimeManager.CurTime;
    }
}
