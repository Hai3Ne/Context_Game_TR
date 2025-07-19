using UnityEngine;
using System.Collections;


namespace SEZSJ
{
    //byte转换为string

    public class ByteToString
    {
        public static string toString(byte[] src)
        {
            //byte[] actionName = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, src);
            string sActionName = System.Text.Encoding.UTF8.GetString(src).Replace("\0", null);

            return sActionName;
        }
    }


    public class BaseTool : MonoBehaviour
    {
        private int m_intQueueValue = 1;
        private static BaseTool m_Instance = null;

        public static BaseTool instance
        {
            get { return m_Instance; }
        }

        void Awake()
        {
            m_Instance = this;
        }

        //获取32位序列值
        public int GetIntQueueValue()
        {
            return m_intQueueValue++;
        }


        public Transform FindChildTransform(GameObject aimObj, string transformName)
        {
            Transform[] transforms = aimObj.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transforms.Length; ++i)
            {
                if (transforms[i].name.Equals(transformName))
                {
                    return transforms[i];
                }
            }

            return null;
        }


 
        /// <summary>
        /// 统一设置position入口，方便查找来源
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="pos"></param>
        static public void SetPosition(Transform tr, Vector3 pos)
        {
            //ActorObj actor = tr.GetComponent<ActorObj>();
            // if(actor != null)
            // {
            //    if( actor.mActorType == ActorType.AT_REMOTE_PLAYER)
            //     {
            //             int nBreak = 1;
            //     }
            // }

            tr.position = pos;
        }

        static public void SetLocalPosition(Transform tr, Vector3 pos)
        {
            tr.localPosition = pos;
        }

        static public void ResetTransform(Transform tr)
        {
            if (tr != null)
            {
                tr.localPosition = Vector3.zero;
                tr.localRotation = Quaternion.identity;
                tr.localScale = Vector3.one;
            }
        }

        static public void CopyTransform(Transform tr, Transform target)
        {
            if (tr != null && target != null)
            {
                tr.localPosition = target.localPosition;
                tr.localRotation = target.localRotation;
                tr.localScale = target.localScale;
            }
        }

        static public Color FormatColor(string col)
        {
            string[] colorStr = col.Split(new char[] { ',' });
            float r, g, b, a;
            r = g = b = a = 1f;
            if (colorStr.Length > 0)
            {
                r = FromatColorComponent(colorStr[0]);
            }

            if (colorStr.Length > 1)
            {
                g = FromatColorComponent(colorStr[1]);
            }

            if (colorStr.Length > 2)
            {
                b = FromatColorComponent(colorStr[2]);
            }

            if (colorStr.Length > 3)
            {
                a = FromatColorComponent(colorStr[3]);
            }

            return new Color(r, g, b, a);
        }

        static public float FromatColorComponent(string com)
        {
            float ret = 1f;
            int r_int;
            if (int.TryParse(com, out r_int))
            {
                ret = (float)r_int / 255;
            }

            return ret;
        }


        static public object DeepCopy(object src, System.Reflection.BindingFlags flag =
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        {
            object dest = null;
            if (src != null)
            {

                System.Type type = src.GetType();

                System.Reflection.FieldInfo[] fields = type.GetFields(flag);

                dest = System.Activator.CreateInstance(type);
           

                if (dest != null)
                {
                    System.Type destType = dest.GetType();

                    for (int i = 0; i < fields.Length; ++i)
                    {
                        object val = null;

                        if (fields[i].FieldType.IsArray)
                        {
                            object srcObj = fields[i].GetValue(src);
                            System.Array array = srcObj as System.Array;
                            if (array != null)
                            {
                                if (array.Length > 0)
                                {
                                    object first = array.GetValue(0);
                                    System.Array destArray = System.Array.CreateInstance(first.GetType(), array.Length);
                                    for (int j = 0; j < destArray.Length; ++j)
                                    {
                                        object v = array.GetValue(j);
                                        destArray.SetValue(v, j);
                                    }

                                    val = destArray;
                                }
                            }
                        }
                        //处于性能考虑，先不深拷贝泛型数组的内容
                        //else if (fields[i].FieldType.IsGenericType)
                        //{

                        //    object srcObj = fields[i].GetValue(src);
                        //    IList list = srcObj as IList;
                        //    if (list != null)
                        //    {
                        //        if (list.Count > 0)
                        //        {
                        //            object first = list[0];
                        //            IList destArray = System.Activator.CreateInstance(fields[i].FieldType) as IList;
                        //            for (int j = 0; j < list.Count; ++j)
                        //            {
                        //                object v = list[j];
                        //                destArray.Add(v);
                        //            }

                        //            val = destArray;
                        //        }
                        //    }
                        //}
                        else
                        {
                            val = fields[i].GetValue(src);
                        }
                        System.Reflection.FieldInfo finfo = destType.GetField(fields[i].Name, flag);
                        finfo.SetValue(dest, val);


                    }
                }

            }
            return dest;
        }

        static bool CheckValid(float v, float min, float max = float.MaxValue)
        {

            if (float.IsNaN(v))
            {
                return false;
            }


            if (v >= min && v <= max)
            {
                return true;
            }

            return false;
        }

        static bool CheckValid(int v, int min, int max = int.MaxValue)
        {

            if (v >= min && v <= max)
            {
                return true;
            }

            return false;
        }
    }
}

