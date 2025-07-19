using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate
{
    public class SerializeDictionary
    {
        public static string DicToJson<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            var ser = new SerializeDictionary<TKey, TValue>();
            ser.target = dic;
            return JsonFx.Json.JsonWriter.Serialize(ser);
        }

        public static Dictionary<TKey, TValue> DicFromJson<TKey, TValue>(string str)
        {
            return JsonFx.Json.JsonReader.Deserialize<SerializeDictionary<TKey, TValue>>(str).target;
        }
    }
}

