using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class Tools {
    public static string GetDownSpdStr(long size) {
        if (size < 0) {
            size = 0;
        }
        if (size > 1000000) {//M
            return string.Format("{0:0.##}M", size / 1024f / 1024f);
        } else if (size > 1000) {//KB
            return string.Format("{0:0.##}KB", size / 1024f);
        } else {//B
            return string.Format("{0:0.##}B", size);
        }
    }
    public static string GetMD5HashFromFile(string fileName) {//获取文件MD5码
        FileStream file = new FileStream(fileName, FileMode.Open);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(file);
        file.Close();

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < retVal.Length; i++) {
            sb.Append(retVal[i].ToString("x2"));
        }
        return sb.ToString();
    }

    public static int GetStrByteLen(string str) {
        int len = 0;
        for (int i = 0; i < str.Length; i++) {
            if (str[i] <= 127) {
                len += 1;
            } else {//中文字符占两个字节长度
                len += 2;
            }
        }
        return len;
    }

    public static long Min(params long[] arr) {
        long num = long.MaxValue;
        for (int i = 0; i < arr.Length; i++) {
            if (num > arr[i]) {
                num = arr[i];
            }
        }
        return num;
    }
    public static float Mod(float num1, float num2) {
        return num1 - Mathf.Floor(num1 / num2) * num2;
    }

    public static string longToStr(long num, int place = 3) {
        string str;
        if (num > 0) {
            str = num.ToString();
        } else {
            str = (-num).ToString();
        }
        StringBuilder sb = new StringBuilder();
        int start_index = str.Length - 1;
        while (start_index >= 0) {
            for (int i = 0; i < place; i++) {
                if (start_index >= 0) {
                    sb.Insert(0, str[start_index--]);
                }
            }
            if (start_index >= 0) {
                sb.Insert(0, ',');
            }
        }
        if (num < 0) {
            sb.Insert(0, '-');
        }
        return sb.ToString();
    }


    public static string MessageColor(string msg) {//系统消息颜色处理
        if (string.IsNullOrEmpty(msg)) {
            return msg;
        }
        int start_index = msg.IndexOf('[');
        if (start_index >= 0) {
            int end_index = msg.IndexOf(']', start_index + 1);
            if (end_index >= 0) {
                string color = msg.Substring(start_index + 1, end_index - start_index - 1);
                string[] vals = color.Split(',');
                if (vals.Length == 3) {
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    int.TryParse(vals[0], out r);
                    int.TryParse(vals[1], out g);
                    int.TryParse(vals[2], out b);
                    return string.Format("{0}[{1:X2}{2:X2}{3:X2}]{4}", msg.Substring(0, start_index), r, g, b, msg.Substring(end_index + 1));
                }
            }
        }
        return msg;
    }

    //单个字符异或运算
    /*static char MakecodeChar(char c,BYTE key)
    {
    return c=c^key;
    }

    //单个字符解密
    static char CutcodeChar(char c,BYTE key)
    {
    return c^key;
    }

    //加密
    static void Makecode(char *pstr,BYTE *pkey)
    {
    int len=strlen(pstr);//获取长度
    for(int i=0;i<len;i++)
    *(pstr+i)=MakecodeChar(*(pstr+i),pkey[i%3]);
    }

    //解密
    static void Cutecode(char *pstr,BYTE *pkey)
    {
    int len=strlen(pstr);
    for(int i=0;i<len;i++)
    *(pstr+i)=CutcodeChar(*(pstr+i),pkey[i%3]);
    }*/

    //加密
    public static string Encode(int pstr, uint dwkey) {
        long nRst = pstr + dwkey + 16486;
        return nRst.ToString();
    }

    //解密
    public static int Decode(string pstr, uint dwkey) {
        return (int)(long.Parse(pstr) - dwkey - 16486);
    }

    public static float Angle(Vector2 from, Vector2 to) {
        float angle = Vector2.Angle(from, to);
        return Vector3.Cross(from, to).z > 0 ? -angle : angle;
    }
    public static Vector2 Rotate(Vector2 pos, float angle) {
        return (Vector2)(Quaternion.AngleAxis(angle, Vector3.forward) * pos);
    }


    //圆形坐标
    public static List<Vector2> BuildCircle(Vector2 center, float radius, int fish_count) {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < fish_count; ++i) {
            list.Add(center + (Vector2)(Quaternion.AngleAxis(360f / fish_count * i, Vector3.forward) * new Vector3(radius, 0)));
        }
        return list;
    }


    //圆形坐标  Z轴表示旋转角度
    public static List<LKPathInfo.PathInfo> BuildCircle(Vector3 center, float radius, int fish_count, float angle) {
        List<LKPathInfo.PathInfo> list = new List<LKPathInfo.PathInfo>();
        LKPathInfo.PathInfo info;
        for (int i = 0; i < fish_count; ++i) {
            info = new LKPathInfo.PathInfo();
            info.NodeType = LKPathInfo.NodeType.Frame;
            info.Angle = 360f / fish_count * i + angle + 90;
            info.TargetPos = center + (Quaternion.AngleAxis(info.Angle-90, Vector3.forward) * new Vector3(radius, 0));
            list.Add(info);
        }
        return list;
    }

    //正玄曲线运动
    public static List<LKPathInfo.PathInfo> BuildSine(float x1, float y1, float x2, float y2, float speed, float init_angle, float height, float cycle) {
        List<LKPathInfo.PathInfo> list = new List<LKPathInfo.PathInfo>();
        Vector2 pos = new Vector3(x1, height * Mathf.Sin(init_angle / cycle * 2 * Mathf.PI) + y1);
        while (pos.x < x2) {
            pos.x += speed;
            pos.y = height * Mathf.Sin((pos.x - x1) / cycle * 2 * Mathf.PI + init_angle) + y1;

            list.Add(new LKPathInfo.PathInfo {
                TargetPos = pos,
                Angle = Mathf.Cos((pos.x - x1) / cycle * 2 * Mathf.PI + init_angle) * Mathf.Rad2Deg,
                NodeType = LKPathInfo.NodeType.Linear,
            });
        }
        return list;
    }
}
