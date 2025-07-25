using System.Collections;

namespace SenLib
{
    public delegate void LoadedCallback(object data);
    public delegate void ProgressHandle(Loader loader, float rate);

    public class Loader
    {
        public enum LoaderType
        {
            BUNDLE,         // AssetBundle
            ASSET,          // Asset目录下的资源
            SCENE,          // 场景
        }

        public enum LoaderState
        {
            NONE,
            LOADING,        // 加载中
            FINISHED,       // 完成
        }
        
        protected LoaderType m_type;            // 加载器类型
        protected Loader(LoaderType type)
        {
            m_type = type;
        }

        protected string m_path;                // 资源完整路径
        protected bool m_async;                 // 是否异步
        protected LoaderState m_state;          // 加载状态

        protected ProgressHandle m_onProgress;  // 加载进度
        protected LoadedCallback m_onloaded;    // 加载完成回调通知

        protected System.Diagnostics.Stopwatch m_watch;

        public LoaderType Type { get { return m_type; } }
        public string Path { get { return m_path; } }
        public bool IsAsync { get { return m_async; } }
        public bool IsFinish { get { return m_state == LoaderState.FINISHED; } }

        public virtual void Init(string path, LoadedCallback onloaded, bool async = true)
        {
            m_path = path;
            m_async = async;
            m_state = LoaderState.NONE;

            m_onloaded = onloaded;
            m_watch = new System.Diagnostics.Stopwatch();
        }

        /// <summary>
        /// 继承的类该接口统一交给 LoadModule 调用，其他地方不要调用
        /// </summary>
        public virtual void Load()
        {
            m_watch.Reset();
            m_watch.Start();
            m_state = LoaderState.LOADING;
            OnLoadProgress(0f);
        }

        public virtual void Stop()
        {
            Reset();
        }

        public virtual void Update()
        {

        }

        public virtual void Reset()
        {
            m_path = "";
            m_async = true;
        }

        protected virtual void OnLoadProgress(float rate)
        {
            if (m_onProgress != null)
            {
                m_onProgress(this, rate);
            }
        }

        protected virtual void OnLoadCompleted(object data)
        {
            m_state = LoaderState.FINISHED;

			if (m_onloaded != null)
			{
				m_onloaded(data);
			}

			OnLoadProgress(1f);
        }
    }
}