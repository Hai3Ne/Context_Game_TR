using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using SEZSJ;
using DG.Tweening;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Linq;
using Spine.Unity;
using TMPro;
using System.Globalization;

namespace HotUpdate
{


    public class ToolUtil:MonoBehaviour
    {
        public static string randStr = "0123456789abcdefghijklmnopqrstuvwxyz";
        
        public static string GetRandomCode(int num)
        {
            char[] chars = randStr.ToCharArray();
            System.Text.StringBuilder strRan = new System.Text.StringBuilder();
            System.Random ran = new System.Random();
            for (int i = 0; i < num; i++)
            {
                strRan.Append(chars[ran.Next(0, 36)]);
            }
            return strRan.ToString();
        }
        public static string HMACSHA1(string ext,string key)
        {
            var strExt = ext + "aes";
            var strKey = key + "&";
            var byteData = System.Text.Encoding.UTF8.GetBytes(strExt);
            var byteKey = System.Text.Encoding.UTF8.GetBytes(strKey);
            System.Security.Cryptography.HMACSHA1 hmac = new System.Security.Cryptography.HMACSHA1(byteKey);
            var result = Convert.ToBase64String(hmac.ComputeHash(byteData));
            return result;
        }

        public static void str2Bytes(string strInput, byte[] output)
        {

            if (strInput.Length <= output.Length)
                System.Text.Encoding.UTF8.GetBytes(strInput).CopyTo(output, 0);
            else
                LogMgr.LogError("协议参数超过长度");
        }


        private static Spine.AnimationState.TrackEntryDelegate ac = null;
        public static void PlayAnimation(Transform trans, string animation, bool loop = false, System.Action func = null,int trackIndex = 0,bool bInit = true)
        {
            trans.gameObject.SetActive(true);
            SkeletonGraphic self = trans.GetComponent<SkeletonGraphic>();
            Spine.Animation ani = self.SkeletonData.FindAnimation(animation);
            if(bInit)
                self.Initialize(true);
            if (ani == null)
                Util.Log($"SkeletonGraphicEx PlayAnimation not found animation:{animation} asset:{self.skeletonDataAsset.name}");
            else
                self.AnimationState.SetAnimation(trackIndex, animation, loop);
            ac = delegate
            {
                self.AnimationState.Complete -= ac;
                ac = null;
                func?.Invoke();
            };
            self.AnimationState.Complete += ac;
        }

        public static void PlayTwoAnimation(Transform trans, string animation,string animationloop, bool loop = false, System.Action func = null)
        {
            trans.gameObject.SetActive(true);
            SkeletonGraphic self = trans.GetComponent<SkeletonGraphic>();
            Spine.Animation ani = self.SkeletonData.FindAnimation(animation);
                self.Initialize(true);
            if (ani == null)
                Util.Log($"SkeletonGraphicEx PlayAnimation not found animation:{animation} asset:{self.skeletonDataAsset.name}");
            else
            {
                self.AnimationState.SetAnimation(0, animation, loop);
                self.AnimationState.AddAnimation(0, animationloop, false, 0);
            }
           
            ac = delegate
            {
                self.AnimationState.Complete -= ac;
                ac = null;
                func?.Invoke();
            };
            self.AnimationState.Complete += ac;
        }



        public static void Play3DAnimation(Transform trans, string animation, bool loop = false, System.Action func = null, int trackIndex = 0, bool bInit = true)
        {
            trans.gameObject.SetActive(true);
            SkeletonAnimation self = trans.GetComponent<SkeletonAnimation>();
            if (bInit)
                self.Initialize(true);
            self.state.SetAnimation(trackIndex, animation, loop);
            ac = delegate
            {
                self.AnimationState.Complete -= ac;
                ac = null;
                func?.Invoke();
            };
            self.AnimationState.Complete += ac;
        }

        public static void Play3DAnimationAndLoop(Transform trans, List<string> animals, System.Action func = null)
        {
            trans.gameObject.SetActive(true);
            SkeletonAnimation self = trans.GetComponent<SkeletonAnimation>();
            self.Initialize(true);
            self.state.SetAnimation(0, animals[0], false);
            for (int i = 1;i < animals.Count;i++)
            {
                self.AnimationState.AddAnimation(0, animals[i], true, 0);
            }

            ac = delegate
            {
                self.AnimationState.Complete -= ac;
                ac = null;
                func?.Invoke();
            };
            self.AnimationState.Complete += ac;
        }



        public static void StopAnimation(Transform trans)
        {
            SkeletonGraphic self = trans.GetComponent<SkeletonGraphic>();
            self.AnimationState.SetEmptyAnimation(0, 0);
        }


        /// <summary>
        /// 获取动作 时长
        /// </summary>
        public static float GetAnimationDuration(Transform trans, string AnimationName)
        {

            SkeletonGraphic self = trans.GetComponent<SkeletonGraphic>();
            Spine.Animation ani = self.SkeletonData.FindAnimation(AnimationName);
            if (ani != null)
            {
                return ani.Duration;
            }
            return 0.0f;
        }

        /// <summary>
        /// 获取动作 时长
        /// </summary>
        public static float Get3DAnimationDuration(Transform trans, string AnimationName)
        {

            SkeletonAnimation self = trans.GetComponent<SkeletonAnimation>();
            Spine.Animation ani = self.skeletonDataAsset.GetSkeletonData(true).FindAnimation(AnimationName);
            if (ani != null)
            {
                return ani.Duration;
            }
            return 0.0f;
        }

        public static float GetDragonAnimationDuration(Transform trans,string AnimationName)
        {
            return trans.GetComponent<DragonBones.UnityArmatureComponent>().animation.GetState(AnimationName)._animationData.duration;
        }

        public static RectTransform ClonePrefab(GameObject clone, Transform parent, string szName)
        {
            var obj = UnityEngine.Object.Instantiate(clone);
            var objTRS = obj.GetComponent<RectTransform>();
            obj.name = szName;
            objTRS.SetParent(parent, true);
            objTRS.localScale = Vector3.one;
            objTRS.anchoredPosition3D = Vector3.zero;
            return objTRS; 
        }

        /// <summary>
        /// 保留2位  向下取整
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ShowF2Num(long num)
        {
            //num = num / 100;
            //num = num * 100;
            var num1 = (Double)num / GetGoldRadio();
            if(MainUIModel.Instance.bNormalGame)
                return num1.ToString("F2", new CultureInfo("en"));
            else
                return num1.ToString("F0", new CultureInfo("en"));
        }
        public static string ShowF2NumSign(long num)
        {
            num = num / 100;
            num = num * 100;
            var num1 = (Double)num / GetGoldRadio();
            return num1.ToString("F1", new CultureInfo("en"));
        }
        public static string ShowF2NumTask(long num)
        {
            num = num / 100;
            num = num * 100;
            var num1 = (Double)num / GetGoldRadio();
            return num1.ToString();
        }

        public static string ShowF2Num2(long num)
        {
            var num1 = (Double)num / GetGoldRadio();
            return num1.ToString("0.##", new CultureInfo("en"));
        }

        public static string ShowF0Num(long num)
        {
            var num1 = (Double)num / GetGoldRadio();
            return num1.ToString("F0", new CultureInfo("en"));
        }

        public static string ShowF2Num2Shop(long num)
        {
            var num1 = (Double)num / 100;//价格不改变
            return num1.ToString("0.##", new CultureInfo("en"));
        }

        //public static string ShowF2Num2Shop2(long num)
        //{
        //    var num1 = (Double)num / 100;//价格不改变
        //    return num1.ToString("0.##", new CultureInfo("en"));
        //}

        /// <summary>
        /// 数字滚动(百分比)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns> 
        public static Sequence RollText(long score, long newScore, Text text, float time = 0.3f, Action callback = null,Action callBack1 = null, Ease ease = Ease.Linear)
        {
            var mScoreSequence = DOTween.Sequence();
            mScoreSequence.Append(DOTween.To(delegate (float value)
            {
         
                text.text = ShowF2Num((long)value);
                callBack1?.Invoke();
                if (value == newScore)
                {
                    if (callback != null)
                        callback();
                }
    
            }, score, newScore, time).SetEase(ease));
            return mScoreSequence;
        }

        public static Sequence RollText2(long score, long newScore, Text text, float time = 0.3f, Action callback = null)
        {
            var mScoreSequence = DOTween.Sequence();
            mScoreSequence.Append(DOTween.To(delegate (float value)
            {

                text.text = ShowF2Num((long)value);
                if (value == newScore)
                {
                    if (callback != null)
                        callback();
                    mScoreSequence.Kill();
                    mScoreSequence = null;
                }

            }, score, newScore, time).SetEase(Ease.Linear));
            return mScoreSequence;
        }


        public static Sequence RollText(long score, long newScore, TextMeshProUGUI text, float time = 0.3f, Action callback = null,string extraTxt= null)
        {
            var mScoreSequence = DOTween.Sequence();
            mScoreSequence.Append(DOTween.To(delegate (float value)
            {
                if (extraTxt == null)
                    text.text = ShowF2Num((long)value);
                else
                    text.text = string.Format(extraTxt, ShowF2Num((long)value)); 
                if (value == newScore)
                {
                    if (callback != null)
                        callback();
                }
            }, score, newScore, time).SetEase(Ease.Linear));
            return mScoreSequence;
        }

        /// <summary>
        /// 转换时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertDateTimep(DateTime time)
        {
            return ((time.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

        }

        /// <summary>
        /// 服务器时间
        /// </summary>
        /// <returns></returns>
        public static long getServerTime()
        {
            return ConvertDateTimep(DateTime.Now) - NetMgr.netMgr.offTime;
        }

        public static bool ValueByBit(long data, int bit)
        {
            if (bit > 64 || bit < 0)

                return false;

            return ((data >> bit) & 1) == 1; //将要取值得位右移到第0位并将左侧第0位以外的位全部置0
        }


        public static void FlyTo(RectTransform graphic)
        {
            RectTransform rt = graphic;
            Sequence mySequence = DOTween.Sequence();
            Tweener move1 = rt.DOLocalMoveY(rt.position.y + 50,5f);
            mySequence.Append(move1);
            mySequence.AppendInterval(1);

        }

        public static void FloattingText(string contant,Transform parnet, Action callback =null)
        {
            var text = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Common/FloatingText") as GameObject;
            RectTransform floatingTextT = text.transform as RectTransform;
            floatingTextT.SetParent(parnet);
            floatingTextT.transform.localPosition = Vector3.zero;
            floatingTextT.localScale = Vector3.one;
            FloatingText floatingText = floatingTextT.GetComponent<FloatingText>();
            floatingText.contant = contant;
            //floatingText.color = color;
            floatingText.transform.DOLocalMoveY(floatingText.transform.position.y + 50f, 2f).OnComplete(delegate
            {
                // CoreEntry.gResLoader.ClearPrefabs();
                Destroy(floatingText.gameObject);
                if (callback!=null)
                {
                    callback();    
                }
            });

        }

        public static void FloattingTextMines(string contant, Transform parnet, Action callback = null) 
        {
            var text = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Common/MinesFloatingText") as GameObject;
            text.transform.SetParent(parnet);
            text.transform.localPosition = Vector3.zero;
            text.GetComponent<Text>().text = contant;
            text.transform.DOScale(new Vector3(0.6f,0.6f,0.6f),0.3f).OnComplete(delegate 
            {
                text.transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.3f).OnComplete(delegate
                {
                    text.transform.DOLocalMoveY(text.transform.position.y + 50f, 2f).OnComplete(delegate
                    {
                        Destroy(text.gameObject);
                        if (callback != null)
                        {
                            callback();
                        }
                    });
                });
            });
            
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="path"></param>
        public static void SetRawTexture(RawImage raw,string path) 
        {
            CoreEntry.gResLoader.LoadTextureAsync(path, (object data) =>
            {
                if (raw!=null&&data!=null&&typeof(Texture2D) ==data.GetType())
                {
                    raw.texture = (Texture2D)data;
                }
            });
        }

        public static void SetTexture(Image raw, string path)
        {
            CoreEntry.gResLoader.LoadTextureAsync(path, (object data) =>
            {
                if (raw != null && data != null && typeof(Texture2D) == data.GetType())
                {
                    var texture2d = (Texture2D)data;
                    raw.sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                    raw.GetComponent<RectTransform>().sizeDelta = new Vector2(texture2d.width, texture2d.height); ;
                }
            });
        }


        static bool isScrolling = false;
        public static IEnumerator AddNoticeMessage(string msg, Text m_TxtMsg,int loop,GameObject parent) 
        {
            parent.SetActive(true);
            Queue<string> m_MsgQueue = new Queue<string>();
            m_MsgQueue.Enqueue(msg);
            if (isScrolling) 
                yield break;
            float beginX = 350;
            float leftX = 320;
            while (m_MsgQueue.Count > 0)
            {
                float duration = 10f;
                float speed = 50f;
                m_TxtMsg.text = m_MsgQueue.Dequeue();
                float txtWidth = m_TxtMsg.preferredWidth;//文本自身的长度.
                Vector3 pos = m_TxtMsg.rectTransform.localPosition;
                float distance = beginX - leftX + txtWidth;
                duration = distance / speed;
                isScrolling = true;
                while (loop-- > 0)
                {
                    Debug.Log(loop);
                    m_TxtMsg.rectTransform.localPosition = new Vector3(beginX, pos.y, pos.z);
                    m_TxtMsg.rectTransform.DOLocalMoveX(-distance, duration).SetEase(Ease.Linear);
                    yield return new WaitForSeconds(duration);
                }
                yield return null;
            }
            isScrolling = false;
            parent.SetActive(false);
            m_TxtMsg.transform.position = new Vector3(700f,-17f,0f);
            Debug.Log($"<color=#ffff00>结束播报</color>");
            yield break;
        }

        public static bool ByteIsNull(byte[] str)
        {
            bool isNull = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != 0)
                {
                    isNull = true;
                    break;
                }
                else
                {
                    isNull = false;
                }
            }
            return isNull;
        }

        public static bool IsMatch(string input, string pattern)
        {
            return IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsMatch(string input, string pattern, RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, pattern, options);
        }

        public static bool IsMail(string str)
        {
            return IsMatch(str, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.None);
        }

        public static bool CheckPwd(string str) 
        {
            return !IsMatch(str, @"^[a-zA-Z0-9]*$", RegexOptions.None);
        }

        public static IEnumerator DelayResponse(Button btn, float delayTime)
        {
            btn.interactable = false;
            yield return new WaitForSeconds(delayTime);
            btn.interactable = true;
        }

        public static SkeletonDataAsset BuildSkeletonDataAsset(string packPath,string skeletonPath)
        {
            SkeletonDataAsset sda;
            sda = ScriptableObject.CreateInstance<SkeletonDataAsset>();
            sda.fromAnimation = new string[0];
            sda.toAnimation = new string[0];
            sda.duration = new float[0];
            sda.scale = 0.01f;
            sda.defaultMix = 0.2f;
            string file1 = FileHelper.CheckBundleName("UI/Spine/" + packPath);
   
            string path1 = FileHelper.SearchFilePath("UI_Bundles", file1);
             path1 = FileHelper.GetAPKPath(path1);
            AssetBundle bound = AssetBundle.LoadFromFile(path1, 0, AppConst.ByteNum);
         
            SpineAtlasAsset[] arrAtlasData = new SpineAtlasAsset[1];
            for (int i = 0; i < arrAtlasData.Length; i++)
            {
                SpineAtlasAsset atlasdata = ScriptableObject.CreateInstance<SpineAtlasAsset>();
                
                atlasdata.atlasFile = CoreEntry.gResLoader.LoadTextAsset("UI/Spine/" + skeletonPath + ".atlas.txt", LoadModule.AssetType.Txt) as TextAsset;
                atlasdata.materials = new Material[1];
                atlasdata.materials[0] = CoreEntry.gResLoader.LoadMaterial("Assets/ResData/UI/Spine/" + skeletonPath + "_Material.mat");
                arrAtlasData[i] = atlasdata;
            }
            sda.atlasAssets = arrAtlasData;
            sda.skeletonJSON = CoreEntry.gResLoader.LoadTextAsset("UI/Spine/" + skeletonPath + ".json", LoadModule.AssetType.Txt) as TextAsset;
            return sda;
        }

        public static string MaskString(string text, char maskChar = '*', int startIndex = 3, int endIndex = -4)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int len = text.Length;
            if (len < (startIndex - 1) + (-endIndex))
            {
                return text;
            }

            string maskString = new string(maskChar, len - ((startIndex - 1) + (-endIndex)));
            StringBuilder result = new StringBuilder(text);

            result.Remove(startIndex, len - ((startIndex - 1) + (-endIndex)));
            result.Insert(startIndex, maskString);

            return result.ToString();
        }

        public static float GetGoldRadio()
        {            
            return MainUIModel.Instance.bNormalGame ? 10000 : 1;
        }

        public static string GetCurrencySymbol()
        {
            return MainUIModel.Instance.bNormalGame ? "R$" : "金币";
        }

        public static string GetOldCurrency()
        {
            return "R$";
        }

        public static void CreatMainUIClickEffect(Transform parnet, Vector3 pos)
        {
            Camera camera = MainPanelMgr.Instance.uiCamera;
            var effect = CoreEntry.gResLoader.ClonePre("UI/Prefabs/ClickEffect/MainUIClickEffect") as GameObject;
            effect.transform.SetParent(parnet);
            effect.transform.localScale = new Vector3(50f, 50f, 50f);
            RectTransform fxRectTrans = effect.GetComponent<RectTransform>();
            Vector2 fxLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parnet.GetComponent<RectTransform>(), pos, camera, out fxLocalPos);
            fxRectTrans.SetParent(parnet);
            fxRectTrans.anchoredPosition3D = fxLocalPos;
            Destroy(effect, 0.5f);
        }

        public static string SplitNum(string number, int count) {
            var outStr = "";
            var index = number.IndexOf(".");
            if (index >= 0)
            {
                var str = number.Substring(index + 1, number.Length - index - 1);
                var strCount = count - str.Length;
                if (strCount > 0) {
                    for (int i = 0; i < strCount; i++)
                    {
                        str += "0";
                    }
                }
                else
                {
                   str =  str.Substring(0, 2);
                }
                outStr = number.Substring(0, index + 1) + str;
            }
            else {
                outStr = number + ".";
                for (int i = 0; i < count; i++)
                {
                    outStr += "0";
                }
            }
            return outStr;
        }

        public static string AbbreviateNumber(long number, bool isreward = false)
        {
            if (number >= 100000000)
            {
                var num = (double)number / 100000000;
                var str = SplitNum(num.ToString(""),2);
                return isreward ? num.ToString("") + "亿" : str + "亿";
            }
            else if (number >= 10000)
            {
                var num1 = (double)number / 10000;
                var str = SplitNum(num1.ToString(""), 2);
                return isreward ? str + "万" : str + "万";
            }
            else
            {
                return number.ToString();
            }
        }

        public static string AbbreviateNumberF0(long number, bool isreward = false)
        {
            if (number >= 100000000)
            {
                var num = (double)number / 100000000;
                return isreward ? num.ToString("") + "亿" : num.ToString("f0") + "亿";
            }
            else if (number >= 10000)
            {
                var num1 = (double)number / 10000;
                return isreward ? num1.ToString("f0") + "万" : num1.ToString("f0") + "万";
            }
            else
            {
                return number.ToString();
            }
        }

        public static string AbbreviateNumberf0(long number, bool isreward = false)
        {
            if (number >= 100000000)
            {
                var num = (double)number / 100000000;
                return isreward ? num.ToString("") + "亿" : num.ToString("0.##") + "亿";
            }
            else if (number >= 10000)
            {
                var num1 = (double)number / 10000;
                return isreward ? num1.ToString("0.##") + "万" : num1.ToString("0.##") + "万";
            }
            else
            {
                return number.ToString();
            }
        }

        public static string AbbreviateNumberf02(long number, bool isreward = false)
        {
            if (number >= 100000000)
            {
                var num = (double)number / 100000000;
                var str = SplitNum(num.ToString(""), 2);
                return isreward ? num.ToString("") + "亿" : str + "亿";
            }
            else if (number >= 10000)
            {
                var num1 = (double)number / 10000;
                return isreward ? num1.ToString("f0") + "万" : num1.ToString("f0") + "万";
            }
            else
            {
                return number.ToString();
            }
        }

        public static string ShowF2Num2(long num, float radio = 10000.00f)
        {
            var num1 = (Double)num / radio;
            return num1.ToString("0.##", new CultureInfo("en"));
        }

        public static string AbbreviateNumber(float number, bool isreward = false)
        {
            if (number >= 100000000)
            {
                var num = number / 100000000f;
                var str = SplitNum(num.ToString(""), 3);
                return isreward ? num.ToString("") + "亿" : str + "亿";
            }
            else if (number >= 10000)
            {
                return (number / 10000f).ToString("f1") + "万";
            }
            else
            {
                return number.ToString();
            }
        }

        public static string AbbreviateLongNumber(long number)
        {
            if (number >= 100000000)
            {
                var num = number / 100000000f;
                var str = SplitNum(num.ToString(""), 3);
                return str + "亿";
            }
            else if (number >= 10000)
            {
                var num = number / 10000f;
                var str = SplitNum(num.ToString(""), 1);
                return str + "万";
            }
            else
            {
                return number.ToString();
            }
        }

        public static string SplitByDot(long num, string split = ",")
        {
            var temp = "";
            string tempNum = num.ToString();
            for (int i = 0; i < tempNum.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                    temp = "," + temp;
                temp = tempNum.Substring(tempNum.Length - i - 1, 1) + temp;
            }
            return temp;
        }

        //public static string SplitByDot(long num, string split = ",")
        //{
        //    var temp = "";
        //    char[] tempNum = num.ToString().ToCharArray();
        //    for (int i = 0; i < tempNum.Length; i++)
        //    {
        //        if (i > 0 && i % 4 == 0)
        //            temp = "," + temp;
        //        temp = tempNum[tempNum.Length -1 - i] + temp;
        //    }
        //    return temp;
        //}

        public static string AbbreviateTourmarmentNumber(long number, bool isreward = false)
        {
            if (number >= 100000000)
            {
                return isreward ? (number / 100000000f).ToString("") + "亿" : (number / 100000000f).ToString("") + "亿";
            }
            else if (number >= 10000)
            {
                return (number / 10000).ToString("") + "万";
            }
            else
            {
                return number.ToString();
            }
        }



        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix时间戳是从1970-01-01 00:00:00开始的秒数或毫秒数
            // 此处以秒为单位，如果是毫秒则除以1000
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static string MaskText(string text, int maxLength)
        {
            string pattern = @"[\u4e00-\u9fa5\d\w]"; // 匹配中文、数字或字母的正则表达式
            int length = 0; // 当前文本长度
            string maskedText = ""; // 掩码后的文本
            foreach (char c in text)
            {
                if (Regex.IsMatch(c.ToString(), pattern)) // 如果当前字符为中文、数字或字母
                {
                    if (length < maxLength - 1) // 如果当前长度未超过maxLength - 1
                    {
                        maskedText += c; // 将当前字符添加到掩码后的文本中
                        length++; // 长度加1
                    }
                    else if (length == maxLength - 1) // 如果当前长度等于maxLength - 1
                    {
                        maskedText += "*"; // 将*号添加到掩码后的文本中
                        length++; // 长度加1
                    }
                }
                else // 如果当前字符不是中文、数字或字母
                {
                    maskedText += c; // 将当前字符添加到掩码后的文本中
                    length++; // 长度加1
                }

                if (length >= maxLength) // 如果当前长度已经达到maxLength
                {
                    break; // 停止遍历字符
                }
            }
            return maskedText;
        }

        /// <summary>
        /// 验证身份证号
        /// </summary>
        /// <param name="idCardNumber"></param>
        /// <returns></returns>
        public static bool ValidateIDCardNumber(string idCardNumber)
        {
            // 身份证号的正则表达式
            string pattern = @"(^\d{15}$)|(^\d{17}([0-9]|X)$)";

            // 验证身份证号格式
            bool isValid = Regex.IsMatch(idCardNumber, pattern);

            return isValid;
        }

        public static IEnumerator GetHeadImage(string headimgurl, Image headIcon)
        {
            if (headimgurl == null || headimgurl == "")
            {
                headIcon.sprite = null;
                yield break;
            }

            if (headimgurl != null)
            {
                if (MainUIModel.Instance.HeadUrl.ContainsKey(headimgurl))
                {
                    if (MainUIModel.Instance.HeadUrl[headimgurl] == null)
                    {
                        yield break;
                    }
                    Texture2D texture2d = MainUIModel.Instance.HeadUrl[headimgurl];
                    Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                    headIcon.sprite = sprite;
                    yield break;
                }
                MainUIModel.Instance.HeadUrl.Add(headimgurl, null);
                WWW www = new WWW(headimgurl);
                yield return www;
                if (www.error == null)
                {
                    Texture2D texture2d = www.texture;
                    Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                    headIcon.sprite = sprite;
                    MainUIModel.Instance.HeadUrl[headimgurl] = texture2d;
                }
                else
                {
                    Debug.Log("下载出错" + "," + www.error);
                }
            }
        }
    }



}

public static class DateTimeExtensions
{
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        return (long)Math.Floor((dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds);
    }
}