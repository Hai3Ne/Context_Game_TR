using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
namespace HotUpdate
{

 

    public class SpriteAnimation: MonoBehaviour
    {
        private Image ImageSource;
        private int mCurFrame = 0;
        private float mDelta = 0;

        public float FPS = 5;
        public List<Sprite> SpriteFrames;
        public bool IsPlaying = false;
        public bool Foward = true;
        public bool AutoPlay = false;
        public bool Loop = false;
        public Action callback;
        public bool isStart = false;
        public int FrameCount
        {
            get
            {
                return SpriteFrames.Count;
            }
        }

        void Awake()
        {
            ImageSource = GetComponent<Image>();
        }

        void Start()
        {
            if (isStart) return;
            if (AutoPlay)
            {
                Play();
            }
            else
            {
                IsPlaying = false;
            }
        }

        public void SetSprite(int idx)
        {
            ImageSource.sprite = SpriteFrames[idx];
            ImageSource.SetNativeSize();
        }

        public void Play(Action ack = null)
        {
            mCurFrame = 0;
            IsPlaying = true;
            Foward = true;
            callback = ack;
            isStart = true;
        }

        public void PlayReverse()
        {
            IsPlaying = true;
            Foward = false;
        }

        void Update()
        {

            if (!IsPlaying || 0 == FrameCount)
            {
                return;
            }

            mDelta += Time.deltaTime;
            if (mDelta > 1 / FPS)
            {
                mDelta = 0;
                if (Foward)
                {
                    mCurFrame++;
                }
                else
                {
                    mCurFrame--;
                }

                if (mCurFrame >= FrameCount)
                {
                    if (Loop)
                    {
                        mCurFrame = 0;
                    }
                    else
                    {
                        IsPlaying = false;
                        if(callback != null)
                        {
                            callback();
                            callback = null;
                        }
                        return;
                    }
                }
                else if (mCurFrame < 0)
                {
                    if (Loop)
                    {
                        mCurFrame = FrameCount - 1;
                    }
                    else
                    {
                        IsPlaying = false;
                        return;
                    }
                }

                SetSprite(mCurFrame);
            }
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
            }
        }

        public void Stop()
        {
            mCurFrame = 0;
            SetSprite(mCurFrame);
            IsPlaying = false;
        }

        public void Rewind()
        {
            mCurFrame = 0;
            SetSprite(mCurFrame);
            Play();
        }
    }
}