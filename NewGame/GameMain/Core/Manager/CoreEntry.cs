using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


namespace SEZSJ
{

    
    public class CoreEntry : SingletonMonoBehaviour<CoreEntry>
    {
        public enum GAME_PAGE
        {
            LOGIN = 0,
            ACTOR = 1,
            GAME = 2,
        }


        //private static GAME_PAGE m_gamePage = GAME_PAGE.LOGIN;

        private static bool m_bInitUI = false;

        public static bool m_bUseDistort = false;

        public static bool m_bUseExEff = false;  //是否使用简化特效


        static public bool GetInitUI()
        {
            return m_bInitUI;
        }
        //暂停游戏，以触发剧情  add by lzp 
        private static bool gameStart = true;

        public static bool GameStart
        {
            get { return CoreEntry.gameStart; }
            set
            {
                //    Debug.LogError("GameStart: " + value); 
                CoreEntry.gameStart = value;
            }
        }

        //add by Alex 20150327 记录进入场景前的页面名称,因为我们有很多个入口进入战斗(打擂台,打副本.........)
        public static string PanelNameBeforeEnterGame = "PanelChooseStage"; //给默认值防止出错
                                                                            //add by Alex 20150416 系统设置里的音效和音乐开关
        public static AudioSource g_CurrentSceneMusic;
        public static bool cfg_bMusicToggle = true;
       
        public static bool cfg_bEaxToggle = true;
        public static bool cfg_bQualityToggle = true;
        public static Dictionary<int, bool> cfg_bPushToggles = new Dictionary<int, bool>();


        public static bool bMusicToggle
        {
            get
            {
                return cfg_bMusicToggle;
            }
            set
            {
                cfg_bMusicToggle = value;

                GameObject aObj = GameObject.Find("Audio1");
                AudioSource aud = null;
                if (aObj != null)
                {
                    aud = aObj.GetComponent<AudioSource>();
                }
                 
                if(aud != null)
                {
                    aud.mute = !cfg_bMusicToggle;
                    if (cfg_bMusicToggle)
                    {
                        aud.volume = 1.0f;
                        aud.Play();
                    }
                    else
                    {
                        aud.Stop();
                    }
                }

            }
        }

        private static bool cfg_bModelShowToggle = true;
        public static bool bModelShow
        {
            get { return cfg_bModelShowToggle; }
            set
            {
                if (cfg_bModelShowToggle == value)
                {
                    return;
                }

                cfg_bModelShowToggle = value;

                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SETTING_ACTORDISPLAYNUM, null);
            }
        }

        public CoreEntry()
        {
            return;
        }


        public static float lastLoadSceneCompleteTime = 0;
        static bool m_bLoadSceneComplete = true;
        public static bool bLoadSceneComplete
        {
            get { return m_bLoadSceneComplete; }
            set { m_bLoadSceneComplete = value; }
        }

        /////////////////////////////////////////////////

        //凡是全局管理器，Awake函数中，建议不要引用跟其他管理器，初始化的顺序无法保证


        private static LogMgr m_logMgr = null;
        public static LogMgr gLogMgr
        {
            get
            {
                if (ReferenceEquals(null, m_logMgr))
                {
                    m_logMgr = CoreRootObj.AddComponent<LogMgr>();
                }
                return m_logMgr;
            }
            set { m_logMgr = value; }
        }

        /// <summary>
        /// 给外界借用StartCoroutine
        /// </summary>
        public static MonoBehaviour Helper
        {
            get
            {
                return gTimeMgr;
            }
        }


        private static TimeMgr m_TimeMgr = null;
        public static TimeMgr gTimeMgr
        {
            get
            {
                if (ReferenceEquals(null, m_TimeMgr))
                {
                    m_TimeMgr = CoreRootObj.AddComponent<TimeMgr>();
                }
                return m_TimeMgr;
            }
            set { m_TimeMgr = value; }
        }



        private static AudioMgr m_AudioMgr = null;
        public static AudioMgr gAudioMgr
        {
            get
            {
                if (m_AudioMgr == null && CoreRootObj != null)
                {
                    m_AudioMgr = CoreRootObj.AddComponent<AudioMgr>();
                }
                return m_AudioMgr;
            }
        }

        //private static SkillComboMgr m_skillComboMgr = null;
        //public static SkillComboMgr gSkillComboMgr
        //{
        //    get
        //    {
        //        if (m_skillComboMgr == null)
        //        {

        //        }
        //        return m_skillComboMgr;
        //    }
        //    set { m_skillComboMgr = value; }
        //}



        private static BaseTool m_baseTool = null;
        public static BaseTool gBaseTool
        {
            get
            {
                if (ReferenceEquals(null, m_baseTool))
                {
                    m_baseTool = CoreRootObj.AddComponent<BaseTool>();
                }
                return m_baseTool;
            }

        }


        private static MainPanelMgr m_PanelManager = null;
        public static MainPanelMgr gMainPanelMgr
        {
            get
            {
                if (ReferenceEquals(null, m_PanelManager))
                {
                    m_PanelManager = CoreRootObj.AddComponent<MainPanelMgr>();
                }
                return m_PanelManager;
            }

        }


     //   private static LuaMgr m_LuaMgr = null;
    /*    public static LuaMgr gLuaMgr
        {
            get
            {
                if (ReferenceEquals(null, m_LuaMgr))
                {
                    m_LuaMgr = CoreRootObj.AddComponent<LuaMgr>();
                }
                return m_LuaMgr;
            }

        }

*/


        private static ResourceLoader m_rcLoader = null;
        public static ResourceLoader gResLoader
        {
            get
            {
                if (ReferenceEquals(null, m_rcLoader))
                {
                    m_rcLoader = CoreRootObj.GetComponent<ResourceLoader>();
                    if(null == m_rcLoader)
                    {
                        m_rcLoader = CoreRootObj.AddComponent<ResourceLoader>();
                    }
                }
                return m_rcLoader;
            }
        }


        private static ObjectPoolManager m_objPoolMgr = null;
        public static ObjectPoolManager gObjPoolMgr
        {
            get { return m_objPoolMgr; }
            set { m_objPoolMgr = value; }
        }

        // 使用PoolManager的对象池
        private static GameObjPoolMgr m_GameObjPoolMgr = null;
        public static GameObjPoolMgr gGameObjPoolMgr
        {
            get
            {
                if (m_GameObjPoolMgr == null)
                {
                    m_GameObjPoolMgr = CoreRootObj.AddComponent<GameObjPoolMgr>();
                }
                return m_GameObjPoolMgr;
            }
        }


        private static EventMgr m_eventMgr = null;
        public static EventMgr gEventMgr
        {
            get
            {
                //if ( m_gamePage == GAME_PAGE.LOGIN)
                //{
                //    // 指定预初始化以及登陆界面时，不处理，以满足三种情况的战斗逻辑启动（连网、不连网、直接场景启动）
                //}

                //解决多线程访问 的问题
                if (ReferenceEquals(null, m_eventMgr))
                {
                    m_eventMgr = CoreRootObj.AddComponent<EventMgr>();


                    ExceptionCatch except = CoreRootObj.GetComponent<ExceptionCatch>();
                    if (except == null)
                    {
                        CoreRootObj.AddComponent<ExceptionCatch>();
                    }
                }
                return m_eventMgr;
            }
        }

        //public static void SetGamePage(GAME_PAGE page)
        //{
        //    //m_gamePage = page;
        //}


        static GameObject m_coreRootObj;

        public static GameObject CoreRootObj
        {
            get
            {

                if (ReferenceEquals(null, m_coreRootObj))
                {
                    m_coreRootObj = GameObject.Find("CoreRoot");
                    if (null == m_coreRootObj)
                    {
                        m_coreRootObj = new GameObject("CoreRoot");
                    }
                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(m_coreRootObj);
                    }

                }
                return m_coreRootObj;
            }
        }





        public override void Init()
        {
            //避免多次初始化
            //DontDestroyOnLoad(CoreRootObj);

            Shader.DisableKeyword("_FOG_ON");
            Shader.DisableKeyword("DISTORT_OFF");


            //byte[] byteConfig = NGUITools.Load("SoundConfig");
            //if (null != byteConfig)
            //{
            //    string configs = System.Text.Encoding.UTF8.GetString(byteConfig);
            //    string[] userInfo = configs.Split('|');
            //    //启动游戏时读取配置信息进行初始化
            //    CoreEntry.cfg_bMusicToggle = userInfo.Length > 0 ? bool.Parse(userInfo[0]) : true;
            //    CoreEntry.cfg_bEaxToggle = userInfo.Length > 1 ? bool.Parse(userInfo[1]) : true;
            //    CoreEntry.cfg_bQualityToggle = userInfo.Length > 2 ? bool.Parse(userInfo[2]) : true;
            //}

            //byteConfig = NGUITools.Load("PushConfig");
            //if (null != byteConfig)
            //{
            //    string configs = System.Text.Encoding.UTF8.GetString(byteConfig);
            //    LitJson.JsonData data = LitJson.JsonMapper.ToObject(configs);
            //    for (int i = 0; i < data.Count; i++)
            //    {
            //        cfg_bPushToggles[(int)data[i]["id"]] = (bool)data[i]["value"];
            //    }
            //}
            //else
            //{
            //    CsvTable tbl = CsvMgr.Instance.getTable("Push");
            //    foreach (int id in tbl.mIntMap.Keys)
            //    {
            //        cfg_bPushToggles[id] = true;
            //    }
            //}


            //(GameObject.FindObjectOfType(typeof(AudioSource)) as AudioSource).mute = !CoreEntry.cfg_bMusicToggle;


            gEventMgr.enabled = true;
          
            gLogMgr.enabled = true;
            gResLoader.enabled = true;



            ApplicationFunction af = CoreRootObj.GetComponent<ApplicationFunction>();
            //增加应用响应对象
            if (af == null)
            {
                af = CoreRootObj.AddComponent<ApplicationFunction>();
                //UI限制帧数只在一开始处理，防止重复被调用
                SetUIFrameRate();
            }
         

            //MsgPush push = CoreRootObj.GetComponent<MsgPush>();
            //if( push == null)
            //{
            //    push = CoreRootObj.AddComponent<MsgPush>();
            //}

   
        }



        //BUG 
        protected override void OnDestroy()
        {
            //LogMgr.UnityError("___________不能释放这个对象"); 

        }

        public static void CoreInit()
        {


            //设置横屏显示
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;

            //所有全局组件挂在CoreRoot对象下，CoreRoot对象被设为加载场景时不被删除，保证它的全局存在
            //GameObject go = new GameObject("CoreRoot");



            if (m_baseTool == null)
                m_baseTool = CoreRootObj.AddComponent<BaseTool>();

            //    if (    m_teamMgr   ==null )
            //m_teamMgr = CoreRootObj.AddComponent<TeamMgr>();

            //         if (m_challengeTeamManager == null)
            //         m_challengeTeamManager = CoreRootObj.AddComponent<ChallengeTeamManager>();



            //if (m_skillComboMgr == null)
            //    m_skillComboMgr = CoreRootObj.AddComponent<SkillComboMgr>();


            if (m_objPoolMgr == null)
                m_objPoolMgr = CoreRootObj.AddComponent<ObjectPoolManager>();

            if (m_GameObjPoolMgr == null)
                m_GameObjPoolMgr = CoreRootObj.AddComponent<GameObjPoolMgr>();

        }


        public static void InitUI()
        {
            if (m_bInitUI)
            {

                return;
            }

            m_bInitUI = true;


       


        }

        //是否
        static bool m_inFightScene;
        public static bool InFightScene
        {
            get { return m_inFightScene; }
            set { m_inFightScene = value; }
        }


        public static void SetUIFrameRate()
        {
            //SetFrameRate(CsvMgr.GetGlobalConfig().TargetFrame);
        }

        public static void SetBattleFrameRate()
        {
            //SetFrameRate(CsvMgr.GetGlobalConfig().BattleTargetFrame);
        }


        public static void SetFrameRate(int rate)
        {
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                Application.targetFrameRate = rate;
            }
        }



     //   public static LuaTable gCurrentMapDesc;



        public static GameObject LastMainCameraObject = null;



   
  
        static public bool IsEditor()
        {
            // return false;
            return Application.platform == RuntimePlatform.WindowsEditor;
        }

        public bool NeedWaitingForReady()
        {
            return false;
        }



        public override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }
    }
}

