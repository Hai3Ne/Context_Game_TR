using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemUI : MonoBehaviour {
    //暂停面板
    public GameObject stopPanel;
    //暂停面板开始动画
    public AnimationClip stopPanelStartAnimation;
    //暂停面板退出动画
    public AnimationClip stopPanelStopAnimation;
    //暂停
    private bool isGame = true;

    public GameObject scorePanel;

    private Animator scoreAnimator;

    public AnimationClip scoreAnimition;

    private Text spriteCompent;

    private IEnumerator PlayScoreAnimition;

    private GameOver gameOver;
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;
    public bool IsGame {
        get { return isGame; }
    }

    private void Awake()
    {
        if(soundCtrl.ins == null)
        {
            var obj = new GameObject("SOUND");
            DontDestroyOnLoad(obj);
            var cmp = obj.AddComponent<AudioSource>();
            soundCtrl.ins = new soundCtrl();
            soundCtrl.ins.soundCmp = cmp;
            soundCtrl.ins.clip1 = clip1;
            soundCtrl.ins.clip2 = clip2;
            soundCtrl.ins.clip3 = clip3;
            soundCtrl.ins.playBg();
        }
        if (scorePanel != null) {
            gameOver = GetComponent<GameOver>();
            scoreAnimator = scorePanel.GetComponent<Animator>();
            spriteCompent = scorePanel.transform.Find("Score").GetComponent<Text>();
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (stopPanel == null || gameOver.isGameOver) {
            //    Application.Quit();
            }
            if (isGame)
            {
                isGame = false;
           //     stopGame();
            }
            else {
              //  Application.Quit();
            }
        }
    }




    //播放奖励动画
    public void playScore(string scoreSprite) {
        scorePanel.SetActive(false);
        if (PlayScoreAnimition != null) {
            StopCoroutine(PlayScoreAnimition);
        }
        spriteCompent.text = scoreSprite;
        PlayScoreAnimition = ScoreAnimation();
        StartCoroutine(PlayScoreAnimition);
    }

    private IEnumerator BackGame() {
        if (stopPanel != null)
        {
            Animator animator = stopPanel.GetComponent<Animator>();
            if (animator)
            {
                animator.Play(stopPanelStopAnimation.name);
                yield return new WaitForSeconds(stopPanelStopAnimation.length);
                stopPanel.SetActive(false);
                isGame = true;
            }
        }
    }

    private IEnumerator StopGame() {
        if (stopPanel != null)
        {
            Animator animator = stopPanel.GetComponent<Animator>();
            if (animator)
            {
                stopPanel.SetActive(true);
                animator.Play(stopPanelStartAnimation.name);
                yield return new WaitForSeconds(stopPanelStartAnimation.length);
            }
        }
    }

    private IEnumerator ScoreAnimation() {
        if (!scorePanel.activeSelf) {
            scorePanel.SetActive(true);
        }
        if (scoreAnimator)
        {
            scoreAnimator.Play(scoreAnimition.name , 0 , 0);
            yield return new WaitForSeconds(scoreAnimition.length) ;
            scorePanel.SetActive(false);
        }
    }
}
