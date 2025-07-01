using UnityEngine;
using System.IO;
using System.Collections.Generic;
public class Audio
{
    AudioClip m_MyAudioClip;
	public Audio(AudioClip clip)
    {
		m_MyAudioClip = clip;
    }

    public AudioClip MyAudioClip
    {
        get { return m_MyAudioClip; }
    }
}



public class GlobalAudioMgr : SingleTon<GlobalAudioMgr>,IRunUpdate
{
    AudioSource LoopAudio = null;
	AudioSource bgmLoopAudio = null;//bg
	AudioPerferenceInfo mPerfercene;
    public bool GlobalInit()
    {
        AudioManager.InitData();
        return true;
    }


    public void StopBgMusic()
    {
        AudioManager.StopMusic();
    }

    public void PlayBGMusic(uint audioID) {
        AudioManager.PlayMusic(audioID.ToString());
    }

    public void PlayOrdianryMusic(uint audioID, bool bLoop = false, bool bStopLoop = false, float volume = 1) {
        if (bStopLoop) {
            AudioManager.StopLoopAudio();
        }

        AudioManager.PlayAudio(audioID, bLoop, volume);
    }

	public void PlayAudioEff(uint[] audioID){
		if (audioID != null && audioID.Length > 0) {
			int idx = Random.Range (0, audioID.Length);
			if (idx >= 0) {
				this.PlayAudioEff (audioID [idx]);
			}
		} else {
			LogMgr.LogWarning ("Audio Array is Empty.");
		}
	}

	public void PlayAudioEff(uint audioID, bool bLoop = false, bool bStopLoop = false, float volume = 1,GameEnum type = GameEnum.Fish_3D)
	{
		if (audioID == 0)
			return;
        if (type == GameEnum.Fish_3D)
            AudioManager.PlayAudio(GameEnum.Fish_3D, audioID.ToString());
        else
            AudioManager.PlayAudio(GameEnum.All, audioID.ToString());
    }

    private class TimeInfo {
        public uint mAudioID;
        public float mDelayTime;
    }
    private List<TimeInfo> mDelayList = new List<TimeInfo>();
    public void DelayPlayAudioEff(uint audioID, float delayTime = 0f) {
        for (int i = 0; i < mDelayList.Count; i++) {
            if (mDelayList[i].mAudioID == audioID) {
                mDelayList[i].mDelayTime = delayTime;
                return;
            }
        }

        mDelayList.Add(new TimeInfo {
            mAudioID = audioID,
            mDelayTime = delayTime,
        });
    }

    public float BGMVolume {
        set { AudioManager.MusicVolume = value; }
        get { return AudioManager.MusicVolume; }
    }

    public float AudioVolume {
        set { AudioManager.AudioVolume = value; }
        get { return AudioManager.AudioVolume; }
    }
    public void SavePerfercene() {
    }

    public void Update(float delta) {
        if (mDelayList.Count > 0) {
            for (int i = mDelayList.Count-1; i >= 0; i--) {
                if (mDelayList[i].mDelayTime > delta) {
                    mDelayList[i].mDelayTime -= delta;
                } else {
                    this.PlayAudioEff(mDelayList[i].mAudioID);
                    mDelayList.RemoveAt(i);
                }
            }
        }
    }

    public void Shutdown()
    {

    }

    public void Clear()
    {

    }
}
