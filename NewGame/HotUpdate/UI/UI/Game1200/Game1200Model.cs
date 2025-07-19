using cfg.Game;
using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public static class SlotData_1200
    {
        public static int column = 3;
        public static int elementFree = 1;
        public static int jackpotElement = 11;
        public static int elementWild = 7;
        public static int elementCount = 14;
        public static float rollTime = 0.080f;
        public static float fastRollTimes = 0.05f;
        public static int rollTimes = 6;
        public static float rollAccTime = 0.06f;
        public static int rollAccTimes = 27;
        public static float rollElasticityTimes = 0.15f;

        public static int specialRollTimes = 17;
        public static float specialRollTime = 0.1f;
    }

    public class TO_Spin_1200
    {
        public List<int> Elements = new List<int>();
        public int FreeTimes = 0;
        public long WinGold = 0;
        public float rate = 0;

        public int SpecialGame = 0;
        public long LastWindGold = 0;//上一把赢的钱
        public long n64Gold = 10;//当前金币
        public long n64SunGold = 0;//太阳模式总金币
        public long n64FreeGold = 0;//免费游戏模式总金币
        public int nModelGame = 0;//特俗游戏总次数 太阳模式或免费模式
        public long n64RSPowerGold = 0;//奖池金币

    }
    public class Game1200Model : Singleton<Game1200Model>
    {
        public int nRoomType = 1;//进入的房间类型
        public int nBet;
        public int nBet1;
        public List<SGame2AwardInfo> arrayAward = new List<SGame2AwardInfo>();

        public TO_Spin_1200 toSpin { get; private set; } = new TO_Spin_1200(); //免费数据

        public List<int> slotResult = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行
        public List<int> specialResult = new List<int>();//特俗图标列表

        // line数据
        public List<KeyValuePair<int, int>> lines = new List<KeyValuePair<int, int>>();

        public List<int> GearList = new List<int>();//下注挡位

        public Dictionary<int, List<int>> lineData = new Dictionary<int, List<int>>();

        //public int lastCount = 0;//上次免费次数

        //public bool bShowFreeAni = false;
        public List<int> elementRate3 = new List<int> { 3, 5, 8, 10, 25, 100,250};

        public List<int> elementList = new List<int>();
        /// <summary>
        /// 元素对应的概率   总的 435
        /// </summary>
        public List<int> PRelementList = new List<int>() { 2000, 3600, 4800, 5800, 6400, 61200, 7300, 7600, 7800,71200,8200,8500,8800,9100,9400,9700,10000 };

        public List<Vector3> gameTipsPos = new List<Vector3> { new Vector3(-156, 204, 0), new Vector3(-156, 0, 0), new Vector3(-156, -191, 0), new Vector3(76, 204, 0), new Vector3(76, 0, 0), new Vector3(76, -191, 0), new Vector3(145, 204, 0), new Vector3(145, 0, 0),new Vector3(145, -191, 0) };

        
      
        public int specialElement = 0;
        public List<List<int>> specialGameLists = new List<List<int>>();

        /// <summary>
        /// 中的线
        /// </summary>
        public List<int> lineList = new List<int>();

        ///// <summary>
        ///// 第一次出现特俗游戏转的久
        ///// </summary>
        //public bool bFirstAppearSpecialGame;
        /// <summary>
        /// 开始旋转特俗游戏
        /// </summary>
        public bool bStartRollSpecialGame;
        public bool bConfirmSpecialElement = false;
        /// <summary>
        /// 是否都中线
        /// </summary>
        public int bAllLine = 0;
        /// <summary>
        /// 游戏状态  免费游戏模式状态（0普通游戏 1免费游戏模式 2触发假免费游戏） 3免费游戏模式中
        /// </summary>
        public int gameStates = 0;
        /// <summary>
        /// 免费游戏模式基础图标
        /// </summary>
        public int ucLogo = 1;
        public void Init(SC_GAME10_ROOM_INFO roomInfo)
        {
            ucLogo = roomInfo.ucLogo;
            nBet = roomInfo.nBet;
            if (nBet <= 0)
            {
                gameStates = 0;
                bStartRollSpecialGame = false;
                bConfirmSpecialElement = false;
            }               
            else
            {
                gameStates = 3;
                bStartRollSpecialGame = true;
                bConfirmSpecialElement = true;
            }
            List<byte> tempResult = roomInfo.arrayLogo.ToList();
            slotResult.Clear();
            if (tempResult.Count == 9)
            {
                slotResult.Add(tempResult[0]);
                slotResult.Add(tempResult[3]);
                slotResult.Add(tempResult[6]);
                slotResult.Add(tempResult[1]);
                slotResult.Add(tempResult[4]);
                slotResult.Add(tempResult[7]);
                slotResult.Add(tempResult[2]);
                slotResult.Add(tempResult[5]);
                slotResult.Add(tempResult[8]);
            }
            Message.Broadcast(MessageName.GAME_INIT);
        }

        public void InitConfig()
        {
            var config0 = ConfigCtrl.Instance.Tables.TbGame1200_Gear.Get(nRoomType);
            GearList.Clear();
            GearList.Add(config0.Gear1);
            GearList.Add(config0.Gear2);
            GearList.Add(config0.Gear3);
            GearList.Add(config0.Gear4);
            if (config0.Gear5 != 0)
                GearList.Add(config0.Gear5);

            lineData.Clear();
            for (int i = 1; i <=5; i++)
            {
                List<int> temp3 = new List<int>();
                var config = ConfigCtrl.Instance.Tables.TbGame1200_Line_Config.Get(i);
                temp3.Add(config.Roulette1);
                temp3.Add(config.Roulette2);
                temp3.Add(config.Roulette3);
                lineData.Add(i, temp3);
            }
        }

        public void SetSpinData(SC_GAME10_BET_RET msg)
        {
            slotResult.Clear();
            List<byte> tempResult = msg.arrayLogo.ToList();
            if (tempResult.Count == 9)
            {
                slotResult.Add(tempResult[0]);
                slotResult.Add(tempResult[3]);
                slotResult.Add(tempResult[6]);
                slotResult.Add(tempResult[1]);
                slotResult.Add(tempResult[4]);
                slotResult.Add(tempResult[7]);
                slotResult.Add(tempResult[2]);
                slotResult.Add(tempResult[5]);
                slotResult.Add(tempResult[8]);
            }

            //for(int i = 0;i < slotResult.Count;i++)
            //{
            //    Debug.Log("======"+ slotResult[i]);
            //}

            lines.Clear();
            List<byte> tempLine = msg.arrayLine.ToList();
            for (int i = 0; i < tempLine.Count; i++)
            {
                if (tempLine[i] > 0)
                    lines.Add(new KeyValuePair<int, int>(i + 1, tempLine[i]));
            }
            toSpin.rate = msg.n64CommPowerGold / ((float)(msg.nAllBet == 0 ? nBet1*5 : msg.nAllBet*5));
          
            toSpin.WinGold = msg.n64CommPowerGold ;
            toSpin.n64Gold = msg.n64Gold;
            bAllLine = msg.ucAllLine;
            if(bAllLine == 1)
                toSpin.rate = msg.n64CommPowerGold*5 / ((float)(msg.nAllBet == 0 ? nBet1 * 5 : msg.nAllBet * 5));
            gameStates = msg.ucFreeGame;
            ucLogo = msg.ucLogo;
            //Debug.LogError("<<<<" + msg.ucFreeGame + "====" + toSpin.WinGold + "===" + bAllLine+"===="+ ucLogo+"===="+Time.realtimeSinceStartup);
            MainUIModel.Instance.Golds = toSpin.n64Gold;
            //Message.Broadcast(MessageName.GE_BroadCast_Jackpot1200);
        }


        public void RandomSpinData()
        {
            slotResult.Clear();
            for (int i = 0; i < 9; i++)
            {
                int element = 1;
                int rand = UnityEngine.Random.Range(0, 7);
                for (int k = 0; k < PRelementList.Count; k++)
                {
                    if (rand < PRelementList[k])
                    {
                        element = k + 1;
                        break;
                    }
                }
                element = rand + 1;
                slotResult.Add(element);
            }
            lines.Clear();
            elementList.Clear();
            if (UnityEngine.Random.Range(0, 6) <20)
            {
                gameStates = 1;
                specialGameLists.Clear();
                specialElement = 1;
                bConfirmSpecialElement = false;
                specialGameLists.Add(new List<int>() { 1, 0, 1, 0, 1, 0, 0, 0, 1 });
                specialGameLists.Add(new List<int>() { 1, 1, 1, 0, 1, 0, 0, 0, 1 });
                specialGameLists.Add(new List<int>() { 1, 1, 1, 0, 1, 0, 0, 1, 1 });
                specialGameLists.Add(new List<int>() { 1, 1, 1, 1, 1, 1,1 , 1, 1 });

                slotResult = specialGameLists[0];
                specialGameLists.RemoveAt(0);
            }
            else
            {
        
                bool bHasOneLine = UnityEngine.Random.Range(1, 6) <= 2;

                int element3 = 0;
                if (bHasOneLine) //随机中1条线
                {
                    int index = UnityEngine.Random.Range(1, 6);
                    element3 = UnityEngine.Random.Range(1, 10);// 线上的元素
                    int rand = UnityEngine.Random.Range(0, 10001);
                    for (int k = 0; k < PRelementList.Count; k++)
                    {
                        if (rand < PRelementList[k])
                        {
                            element3 = k + 1;
                            break;
                        }
                    }

                    rand = UnityEngine.Random.Range(0, 7);
                    element3 = rand + 1;
                    List<int> elements = lineData[3];
                    int randomCount = 3;// UnityEngine.Random.Range(3, 6);
                    for (int j = 0; j < randomCount; j++)
                    {
                        int pos = j * 3 + elements[j] - 1;
                        slotResult[pos] = element3;
                    }
                    List<int> elements_2 = lineData[5];
                    for (int j = 0; j < randomCount; j++)
                    {
                        int pos = j * 3 + elements_2[j] - 1;
                        slotResult[pos] = element3;
                    }
                }

                for (int i = 1; i <= 5; i++)
                {
                    List<int> elementPos = lineData[i];
                    int sameElementCount = 0;
                    int element = slotResult[elementPos[0] - 1];
                    for (int k = 0; k < elementPos.Count; k++)
                    {
                        int pos = (k) * 3 + elementPos[k] - 1;
                        if (element == slotResult[pos])
                            sameElementCount++;
                        else
                            break;
                    }
                    if (sameElementCount >= 3)
                    {
                        KeyValuePair<int, int> tempLine = new KeyValuePair<int, int>(i, sameElementCount);
                        lines.Add(tempLine);
                        elementList.Add(element);
                    }
                }

                if (lines.Count > 0)
                {
                    toSpin.WinGold = 0;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        int rate = 0;
                        element3 = elementList[i] - 1;
                        if (element3 >= 0)
                        {
                            int count = lines[i].Value;
                            if (count == 3)
                                rate = elementRate3[element3];
                            else if (count == 4)
                                rate = elementRate3[element3];
                            else
                                rate = elementRate3[element3];
                        }
                        else
                        {
                            rate = UnityEngine.Random.Range(1, 6);
                        }

                        toSpin.WinGold += nBet1 * rate;
                    }
                    toSpin.rate = toSpin.WinGold / nBet1;
                    toSpin.WinGold = toSpin.WinGold;
                    toSpin.n64Gold += toSpin.WinGold;
                    toSpin.n64Gold -= nBet1;
                }
                else
                {
                    toSpin.rate = 0;
                    toSpin.WinGold = 0;
                    toSpin.n64Gold -= nBet1;
                }
            }

            PlayerPrefs.SetInt("DanJi", (int)toSpin.n64Gold);
        }



        public void MovePos(Transform trans, Transform target,Action callBack,float duration =1f,bool bottomToTop = true,bool bChangeDir = false)
        {
            //float dis_x = Math.Abs(trans.position.x - target.position.x);
            //float dis_y = Math.Abs(trans.position.y - target.position.y);
            float x = (trans.position.x + target.position.x) / 2;
            float y = (trans.position.y + target.position.y) / 2;
            bool lessZero = trans.position.x - target.position.x <= 0;
            float extra = x + (lessZero ? -1.5f : 1.5f)* (bottomToTop?1:-1);
            Vector3 midPos = new Vector3(extra, y+1, 0);
            Vector3[] pathvec = Bezier2Path(trans.position, midPos, target.position);
            trans.DOPath(pathvec, duration).SetEase(Ease.Linear).OnWaypointChange((Index)=> 
            {
                if (!bChangeDir)
                    return;
                if (Index < 1)
                    return;
               
                float dis_x = Math.Abs(trans.position.x - pathvec[Index].x);
                float dis_y = Math.Abs(trans.position.y - pathvec[Index].y);
                float rad = Mathf.Atan(dis_x / dis_y);
                float anger = rad * Mathf.Rad2Deg;
                if (trans.position.x < pathvec[Index].y)
                { }
                else
                    anger = -anger;
                trans.eulerAngles = new Vector3(0, 0, anger);
            }).OnComplete(()=> 
            {
                callBack?.Invoke();
            });
        }

        private float _pointCount = 10;
        //获取二阶贝塞尔曲线路径数组
        private Vector3[] Bezier2Path(Vector3 startPos, Vector3 controlPos, Vector3 endPos)
        {
            Vector3[] path = new Vector3[(int)_pointCount];
            for (int i = 1; i <= _pointCount; i++)
            {
                float t = i / _pointCount;
                path[i - 1] = Bezier2(startPos, controlPos, endPos, t);
            }
            return path;
        }
        // 2阶贝塞尔曲线
        public static Vector3 Bezier2(Vector3 startPos, Vector3 controlPos, Vector3 endPos, float t)
        {
            return (1 - t) * (1 - t) * startPos + 2 * t * (1 - t) * controlPos + t * t * endPos;
        }

        public List<int> GetAllLines()
        {
            lineList.Clear();
            for(int i = 0;i < lineData.Count; i++)
            {
                List<int> posList = lineData[i+1];
                bool bHasAllElement = true;
                for(int j = 0;j < posList.Count;j++)
                {
                    int temp = 3 * j+ posList[j];
                    if (slotResult[temp - 1] == 0)
                        bHasAllElement = false;
                }
                if (bHasAllElement)
                    lineList.Add((i+1));
            }
            return lineList;
        }

        public bool bHasElement(int ele)
        {
            for(int i = 0;i < slotResult.Count;i++)
            {
                if (slotResult[i] == ele)
                    return true;
            }
            return false;
        }

        public int GetElement(int exceptElement)
        {
            for (int i = 0; i < slotResult.Count; i++)
            {
                if (slotResult[i] != exceptElement)
                    return slotResult[i];
            }
            return 1;
        }
    }
}
