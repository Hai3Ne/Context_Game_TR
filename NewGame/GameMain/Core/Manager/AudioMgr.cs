using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace SEZSJ
{
    public class AudioConfig
    {
        public int id;
        public string path;
        public int type;
        public float Volume;
    }
    //管理器    
    public class AudioMgr : MonoBehaviour
    {
        AudioSource musicCmp;
        AudioSource soundCmp;
        public List<AudioConfig> audioList = new List<AudioConfig>();
        public bool musicMute = false;
        public bool soundMute = false;
        //游戏开始入口
        void Awake()
        {
            musicCmp = gameObject.AddComponent<AudioSource>();
            soundCmp = gameObject.AddComponent<AudioSource>();

            GetAudioPrefs();
            ReLoadMute();

        }

        public void SetAudioMusicPrefs(int num)
        {
            PlayerPrefs.SetInt("Audio_music", num);
            musicMute = num == 1;
            musicCmp.mute = musicMute;
        }

        public void SetAudioSoundPrefs(int num)
        {
            PlayerPrefs.SetInt("Audio_sound", num);
            soundMute = num == 1;
            soundCmp.mute = soundMute;
        }

        public void ReLoadMute()
        {
            musicCmp.mute = musicMute;
            soundCmp.mute = soundMute;
        }

        public void GetAudioPrefs()
        {
            if (PlayerPrefs.HasKey("Audio_music"))
            {
                musicMute = PlayerPrefs.GetInt("Audio_music") == 1;
            }
            else
            {
                musicMute = false;
            }

            if (PlayerPrefs.HasKey("Audio_sound"))
            {
                soundMute = PlayerPrefs.GetInt("Audio_sound") == 1;
            }
            else
            {
                soundMute = false;
            }
        }
        void Start()
        {
        }

        public void initCfg(List<AudioConfig> list)
        {
            audioList = list;
        }


        public AudioConfig GetAudioConfig(int audioID)
        {
            AudioConfig config = null;
            for (int i = 0; i < audioList.Count; i++)
            {
                if (audioList[i].id == audioID)
                {
                    config = audioList[i];
                    break;
                }
            }

            return config;
        }

        /// <summary>
        /// 音量设置
        /// </summary>
        public void setVolume(float volume)
        {
            musicCmp.volume = Mathf.Min(1, volume);
        }

        /// <summary>
        /// 静音
        /// </summary>
        public void setMute(bool isMute)
        {
            musicCmp.mute = isMute;
        }

        public bool IsPlaying()
        {
            if (musicCmp != null)
            {
                return musicCmp.isPlaying;
            }
            return false;
        }

        public bool IsSoundCmpPlaying()
        {
            if (soundCmp != null)
            {
                return soundCmp.isPlaying;
            }
            return false;
        }

        /// <summary>
        /// 重新播放
        /// </summary>
        public void ResetMusic()
        {
            musicCmp.Play();

        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        public void PauseMusic()
        {
            musicCmp.Pause();

        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void StopMusic(int id)
        {
            AudioConfig config = GetAudioConfig(id);
            if (config == null)
            {
                LogMgr.UnityError("无效音效ID:" + id);
                return;
            }
            var finalPath = config.path;
            if (musicCmp == null || musicCmp.clip == null)
                return;
            if (finalPath == musicCmp.clip.name)
            {
                musicCmp.Stop();
                musicCmp.clip = null;
            }

        }
        /// <summary>
        /// 音效播放
        /// </summary>
        /// <param name="id"></param>
        public void PlayUIMusic(int id)
        {
            AudioConfig config = GetAudioConfig(id);
            if (config == null)
            {
                LogMgr.UnityError("无效音效ID:" + id);
                return;
            }
            var finalPath =  config.path;
            AudioClip clip = (AudioClip)CoreEntry.gResLoader.LoadAudio(finalPath, LoadModule.AssetType.AudioMp3);
            if (clip == null)
            {
                LogMgr.UnityError("音效配置错误 路径:" + finalPath + "  id" + id);
                return;
            }
            clip.name = finalPath;
            musicCmp.clip = clip;
            musicCmp.loop = true;
            musicCmp.volume = config.Volume;
            musicCmp.Play();
           
        }

        public void StopSound( GameObject obj = null)
        {
            if(obj == null)
            {
                if (soundCmp != null)
                {
                    soundCmp.Stop();
                }
            }
            else
            {
                var cmp = obj.GetComponent<AudioSource>();
                if (cmp != null)
                {
                    cmp.Stop();
                }
            }

        }
        public void PlayUISound(int id,GameObject obj = null)
        {
            AudioConfig config = GetAudioConfig(id);
            if (config == null)
            {
                LogMgr.UnityError("无效音效ID:" + id);
                return;
            }

            var finalPath =  config.path;
            AudioClip clip = (AudioClip)CoreEntry.gResLoader.Load(finalPath, typeof(AudioClip));
            if (clip == null)
            {
                LogMgr.UnityError("音效配置错误 路径:" + finalPath + "  id" + id);
                return;
            }
            if(obj == null)
            {
                soundCmp.PlayOneShot(clip, config.Volume);
            }
            else
            {
                var cmp = obj.GetComponent<AudioSource>();
                if(cmp == null)
                {
                    cmp = obj.AddComponent<AudioSource>();
                }
                cmp.mute = soundMute;
                cmp.PlayOneShot(clip, config.Volume);
            }
            
        }
    }
};//end SG

