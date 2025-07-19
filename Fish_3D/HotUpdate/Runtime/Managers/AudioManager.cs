using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager {
    private class AudioInfo {
        public AudioSource AudioSource;
        public bool IsLoading;//是否正在加载
    }
    private const string music_volume = "game_music_volume";
    private const string audio_volume = "game_audio_volume";

    public static Dictionary<string, AudioClip> dic_audio = new Dictionary<string, AudioClip>();
    private static AudioListener mListener;
    private static AudioSource LoopAudio = null;
    private static AudioSource bgmLoopAudio = null;//bg
    private static List<AudioInfo> mAudioList = new List<AudioInfo>();//音效列表  
    private const int MaxAudioCount = 20;

    private static float _audio_vol;//音效音量
    public static float AudioVolume {
        set {
            _audio_vol = value;
            PlayerPrefs.SetFloat(audio_volume, value);
            PlayerPrefs.Save();
            if (LoopAudio != null) {
                LoopAudio.volume = value;
            }
            foreach (var item in mAudioList) {
                if (item.AudioSource != null) {
                    item.AudioSource.volume = value;
                }
            }
        }
        get {
            return _audio_vol;
        }
    }
    private static float _music_vol;//音乐音量
    public static float MusicVolume {
        set {
            _music_vol = value;
            PlayerPrefs.SetFloat(music_volume, value);
            PlayerPrefs.Save();
            if (bgmLoopAudio != null) {
                bgmLoopAudio.volume = value;
            }
        }
        get {
            return _music_vol;
        }
    }

    public static void InitData() {
        Camera cam = Camera.main;
        if (cam == null)
            cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
        if (mListener == null) {
            mListener = cam.gameObject.GetComponent<AudioListener>();
        }
        if (mListener == null)
            mListener = cam.gameObject.AddComponent<AudioListener>();

        LoopAudio = mListener.gameObject.AddComponent<AudioSource>();
        bgmLoopAudio = mListener.gameObject.AddComponent<AudioSource>();

        if (PlayerPrefs.HasKey(music_volume) && PlayerPrefs.HasKey(audio_volume)) {
            AudioVolume = PlayerPrefs.GetFloat(audio_volume);
            MusicVolume = PlayerPrefs.GetFloat(music_volume);
        } else {
            AudioPerferenceInfo mPerfercene = LocalSaver.ReadData<AudioPerferenceInfo>(ConstValue.AudioPerferceFile);
            if (mPerfercene == null) {
                mPerfercene = GameParams.Instance.audioPerferce;
            }
            AudioVolume = mPerfercene.audioVolume;
            MusicVolume = mPerfercene.bgmVolume;
        }
        LoopAudio.volume = AudioVolume;
        bgmLoopAudio.volume = MusicVolume;
    }
    public static void Clear() {
        dic_audio.Clear();
    }
    private static void TryGetAudio(string name, VoidCall<AudioClip> call) {
        AudioClip audio;
        if (dic_audio.TryGetValue(name, out audio)) {
            call(audio);
            return;
        }

        string audioUrl = ResPath.NewAudioPath + name;
        ResManager.LoadAsyn<AudioClip>(audioUrl, (ab_data, c) =>
        {
            dic_audio[name] = c;
            call(c);
            if(ab_data !=null)
                ResManager.UnloadAB(ab_data);
        },GameEnum.Fish_3D);
    }
    private static void TryGetAudio(GameEnum type, string name, VoidCall<AudioClip> call) {
        AudioClip audio;
        if (dic_audio.TryGetValue(name, out audio)) {
            call(audio);
            return;
        }

        string audioUrl = ResPath.NewAudioPath + name;
        ResManager.LoadAsyn<AudioClip>(audioUrl, (abinfo, ac) => {
            dic_audio[name] = ac;
            call(ac);
            ResManager.ClearLoadList();
        }, type);
    }
    private static AudioInfo GetAudio() {//获取看空闲音效源
        foreach (var item in mAudioList) {
            if (item.IsLoading == false && item.AudioSource.isPlaying == false) {
                return item;
            }
        }
        if (mAudioList.Count < AudioManager.MaxAudioCount) {
            AudioInfo _ai = new AudioInfo {
                AudioSource = mListener.gameObject.AddComponent<AudioSource>(),
                IsLoading = false,
            };
            mAudioList.Add(_ai);
            return _ai;
        } else {
            return null;
        }
    }
    public static void PlayAudio(uint audio_id, bool is_loop = false, float volume = 1) {//播放音效
        if (audio_id == 0) {
            return;
        }
        AudioManager.PlayAudio(GameEnum.Fish_3D,audio_id.ToString(), is_loop, volume);
    }
    //public static void PlayAudio(string name,bool is_loop = false, float volume = 1) {//播放音效
    //    if (is_loop == false && AudioManager.AudioVolume == 0) {//声音为0的时候直接不放博音效
    //        return;
    //    }
    //    AudioInfo _ai = null;
    //    if (is_loop == false) {
    //        _ai = AudioManager.GetAudio();
    //        if (_ai == null) {
    //            return;
    //        } else {
    //            _ai.IsLoading = true;
    //        }
    //    }

    //    AudioManager.TryGetAudio(name, audioObj => {
    //        if (_ai != null) {
    //            _ai.IsLoading = false;
    //        }
    //        if (audioObj != null) {
    //            if (is_loop) {
    //                if (LoopAudio == null)
    //                    return;
    //                LoopAudio.Stop();
    //                LoopAudio.loop = true;
    //                LoopAudio.clip = audioObj;
    //                LoopAudio.Play();
    //                LoopAudio.volume = AudioVolume * volume;
    //            } else if(_ai != null){
    //                _ai.AudioSource.clip = audioObj;
    //                _ai.AudioSource.volume = AudioVolume * volume;
    //                _ai.AudioSource.Play();
    //                //NGUITools.PlaySound(audioObj, AudioVolume * volume);
    //            }
    //        }
    //    });
    //}
    public static void PlayAudio(GameEnum type, string name, bool is_loop = false, float volume = 1) {//播放音效
        if (is_loop == false && AudioManager.AudioVolume == 0) {//声音为0的时候直接不放博音效
            return;
        }
        AudioInfo _ai = null;
        if (is_loop == false) {
            _ai = AudioManager.GetAudio();
            if (_ai == null) {
                return;
            } else {
                _ai.IsLoading = true;
            }
        }
        AudioManager.TryGetAudio(type, name, audioObj => {
            if (_ai != null) {
                _ai.IsLoading = false;
            }
            if (audioObj != null) {
                if (is_loop) {
                    if (LoopAudio == null)
                        return;
                    LoopAudio.Stop();
                    LoopAudio.loop = true;
                    LoopAudio.clip = audioObj;
                    LoopAudio.Play();
                    LoopAudio.volume = AudioVolume * volume;
                } else if (_ai != null) {
                    _ai.AudioSource.clip = audioObj;
                    _ai.AudioSource.volume = AudioVolume * volume;
                    _ai.AudioSource.Play();
                    //NGUITools.PlaySound(audioObj, AudioVolume * volume);
                }
            }
        });
    }
    public static void StopLoopAudio() {//关闭循环音效
        if (LoopAudio != null) {
            LoopAudio.Stop();
            LoopAudio.loop = false;
        }
    }
    public static void StopAllAudio() {//关闭所有音效
        AudioManager.StopLoopAudio();
        foreach (var item in mAudioList) {
            if (item.AudioSource.isPlaying) {
                item.AudioSource.Stop();
            }
        }
    }
    public static void PlayMusic(string name, float volume = 1) {//播放背景音乐
        if (bgmLoopAudio == null) {
            return;
        }
        AudioManager.TryGetAudio(GameEnum.Fish_3D, name, ac => {
            if (bgmLoopAudio.isPlaying && bgmLoopAudio.clip == ac) {
                return;//背景音乐如果跟上次一样，则不更改
            }
            bgmLoopAudio.Stop();
            bgmLoopAudio.clip = ac;
            bgmLoopAudio.loop = true;
            bgmLoopAudio.volume = AudioManager.MusicVolume * volume;
            bgmLoopAudio.Play();
        });
    }
    public static void PlayMusic(GameEnum type, string name, float volume = 1) {//播放背景音乐
        if (bgmLoopAudio == null) {
            return;
        }
        AudioManager.TryGetAudio(type, name, ac => {
            if (bgmLoopAudio.isPlaying && bgmLoopAudio.clip == ac) {
                return;//背景音乐如果跟上次一样，则不更改
            }
            bgmLoopAudio.Stop();
            bgmLoopAudio.clip = ac;
            bgmLoopAudio.loop = true;
            bgmLoopAudio.volume = AudioManager.MusicVolume * volume;
            bgmLoopAudio.Play();
        });
    }
    public static void StopMusic() {
        if (bgmLoopAudio != null)
            bgmLoopAudio.Stop();
    }

    public static void PlayMusic()
    {
        if (bgmLoopAudio != null)
            bgmLoopAudio.Play();
    }
}
