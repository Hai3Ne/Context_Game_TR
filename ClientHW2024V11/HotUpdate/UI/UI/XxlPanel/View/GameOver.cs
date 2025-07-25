using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HotUpdate;
using SEZSJ;

public class GameOver : MonoBehaviour {
    //分数
    private int score = 0;
    //剩余时间
    private int gameTime = 0;

    private Grid gridComponent;
    //当前分数
    public Text scoreText;
    //剩余时间
    public Text timeText;
    //最终得分
    public Text finelScoreText;

    public GameObject OverPanel;

    public AnimationClip overAnimation;

    public AnimationClip TextAnimation;

    public bool isGameOver = false;

    public GameObject Mask;
    [SerializeField]
    public Image Satr1;
    [SerializeField]
    public Image Satr2;
    [SerializeField]
    public Image Satr3;

    private int maxStep = 100;
    public void Start()
    {
        
    }



    public void initGameOver(int gametime , Grid grid) {
        gameTime = gametime;
        gridComponent = grid;
        scoreText.text =  "0";
        timeText.text = (maxStep - gameTime) + "";
    }



    public void addScore(int s) {
        score += s;
        scoreText.text = score + "";
        //    scoreText.GetComponent<Animator>().Play(TextAnimation.name);
        if (score > 2500)
        {
            Satr3.sprite = AtlasSpriteManager.Instance.GetSprite("XxlPanel:xx");
        }
        else if (score > 1800)
        {
            Satr2.sprite = AtlasSpriteManager.Instance.GetSprite("XxlPanel:xx");
        }

        else if (score > 1000)
        {
            Satr1.sprite = AtlasSpriteManager.Instance.GetSprite("XxlPanel:xx");
        }
       

    }

    public void addTime(int t) {
        gameTime += t;
        if(gameTime >= maxStep)
        {
            gameOver();
        }

        timeText.text = (maxStep - gameTime) + "";
        //     timeText.GetComponent<Animator>().Play(TextAnimation.name);
    }
    public void gameOver() {
        isGameOver = true;
        finelScoreText.text = score + "";
        Mask.SetActive(true);
        OverPanel.SetActive(true);
        OverPanel.GetComponent<Animator>().Play(overAnimation.name);
        XxlCtrl.Instance.addLevel(score);
        CoreEntry.gAudioMgr.PlayUISound(301);
    }
}
