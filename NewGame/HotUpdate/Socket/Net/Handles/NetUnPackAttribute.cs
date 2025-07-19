/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	NetHandlerAttribute
作    者:	HappLI
描    述:	消息句柄
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SEZSJ
{
    public class NetUnPackAttribute : Attribute
    {
    }
    public class NetUnPackResponseAttribute : Attribute
    {
        public int mid;
        public NetUnPackResponseAttribute(int mid)
        {
            this.mid = mid;
        }
    }
}
