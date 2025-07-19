using UnityEngine;
using System.Collections.Generic;
using System;
namespace HotUpdate
{
    // List<T>
    [Serializable]
    public class SerializationList<T>
    {
        [SerializeField]
        public List<T> target;
        public List<T> ToList() { 
            return target;
        }

        public SerializationList()
        {
            
        }
    }
}

