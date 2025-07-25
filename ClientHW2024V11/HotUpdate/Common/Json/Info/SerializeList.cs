using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate
{
    public class SerializeList
    {
     
        public static string ListToJson<T>(List<T> l)
        {
            var ser = new SerializationList<T>();
            ser.target = l;
            var str = JsonFx.Json.JsonWriter.Serialize(ser);
            return str;
        }

        public static List<T> ListFromJson<T>(string str)
        {
            var ser1 = JsonFx.Json.JsonReader.Deserialize<SerializationList<T>>(str);
            return ser1.target;
        }
    }
}

