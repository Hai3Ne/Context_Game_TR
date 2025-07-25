using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
namespace HotUpdate
{
    public class FrameAnimation1600 : MonoBehaviour
    {
        [Beebyte.Obfuscator.Skip]
        public int fps = 12; //帧率
        [Beebyte.Obfuscator.Skip]
        public bool auto = true; //是否自动播放
        [Beebyte.Obfuscator.Skip]
        public bool loop = true; //是否循环
        public bool rnd = false; //起始随机帧
        public Sprite[] sprites;

        private SpriteRenderer sr;
        private Image img;
        private bool pause = true; //暂停
        private int idx = 0; //当前帧数
        private float timer = 0f; //计时器
        private Callback callBack;

        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            img = GetComponent<Image>();
            if (auto)
                pause = false;
            if (rnd)
                idx = Random.Range(0, sprites.Length);
        }

        public void play(bool b = true, Callback callBack = null)
        {
            gameObject.SetActive(true);
            pause = !b;
            this.callBack = callBack;
        }

        public void setSprite(int index)
        {
            if (index < sprites.Length)
                sr.sprite = sprites[index];
        }

        void Update()
        {
            if (!pause && fps > 0 && sprites.Length > 0)
            {
                timer += Time.deltaTime;
                float rate = 1f / fps;
                if (rate < timer)
                {
                    timer = (rate > 0f) ? timer - rate : 0f;
                    if (++idx >= sprites.Length)
                    {
                        idx = 0;
                        pause = !loop;
                    }
                    if (sr != null) sr.sprite = sprites[idx];
                    if (img != null) img.sprite = sprites[idx];
                    if (idx == 0)
                        callBack?.Invoke();
                }
            }
        }
    }
}
