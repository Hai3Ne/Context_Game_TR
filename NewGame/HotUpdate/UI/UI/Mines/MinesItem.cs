using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DragonBones;
using HotUpdate;
using SEZSJ;
using System;
using Animation = DragonBones.Animation;
using System.Globalization;

public class MinesItem : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Button clickBtn;
    [SerializeField] private UnityArmatureComponent box;
    [SerializeField] private UnityArmatureComponent coin;
    [SerializeField] private UnityArmatureComponent bomb;
    private Tweener tweener;
    [SerializeField] private int id;
    private int state;//0 奖励， 1 炸弹
    private bool isOpened;
    private float amount { get; set; }
    public float Amount { get { return amount; } }
    [SerializeField] private int row;
    [SerializeField] private int col;
    public int Row { get { return row; } set { row = value; } }
    public int Col { get { return col; } set { col = value; } }
    public Tween Tween;
    public Vector3 Position;
    void Start() 
    {
       
    }

    void OnEnable() 
    {
        Position = transform.position;
        clickBtn.onClick.AddListener(OnClick);
    }

    void OnDisable() 
    {
        clickBtn.onClick.RemoveListener(OnClick);
        state = 0;
    }

    public void SetUpItem(MinesItemData data)
    {
        id = data.id;
        state = data.state;
        amount = float.Parse(data.amount, new CultureInfo("en"));

    }

    public void CloneAni(UnityArmatureComponent obj1, UnityArmatureComponent obj2, UnityArmatureComponent obj3) 
    {
        if (box==null)
        {
            box = Instantiate(obj1);
            box.transform.SetParent(transform);
            box.transform.localPosition = Vector3.zero;
            box.transform.localScale = new Vector3(100f,100f,100f);
            box.gameObject.SetActive(false);
        }
        if (coin==null)
        {
            coin = Instantiate(obj2);
            coin.transform.SetParent(transform);
            coin.transform.localPosition = Vector3.zero;
            coin.transform.localScale = new Vector3(100f, 100f, 100f);
            coin.gameObject.SetActive(false);

        }
        if (bomb==null)
        {
            bomb = Instantiate(obj3);
            bomb.transform.SetParent(transform);
            bomb.transform.localPosition = Vector3.zero;
            bomb.transform.localScale = new Vector3(100f, 100f, 100f);
            bomb.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 重置
    /// </summary>
    public void Reset() 
    {
        bg.gameObject.SetActive(true);
        bomb.gameObject.SetActive(false);
        box.gameObject.SetActive(false);
        state = 0;
        isOpened = false;
    }

    public void OnClick() 
    {
        if (MinesModel.Instance.minesState!= MinesState.Gameing)
        {
            ToolUtil.FloattingText("Please start the game after betting", MainPanelMgr.Instance.GetPanel("MinesPanel").transform);
            return;
        }
        isOpened = true;
        //是否点击到炸弹 
        if (isMine())
        {
            ShowBomb();
           
        }
        else
        {
            ShowReward();
        }
    }

    public void SetState(int index) 
    {
        state = index;
    }

    bool isMine() 
    {
        return state == 1;
    }

    /// <summary>
    /// 显示炸弹
    /// </summary>
    void ShowBomb() 
    {
        MinesModel.Instance.minesState = MinesState.GameOver;
        bg.gameObject.SetActive(false);
        bomb.gameObject.SetActive(true);
        bomb.animationName = "a1";
        CommonTools.PlayArmatureAni(bomb.transform, "a1", 1);
        CoreEntry.gAudioMgr.PlayUISound(128);
        bomb.AddDBEventListener(DragonBones.EventObject.COMPLETE, (string type, DragonBones.EventObject eventObject) =>
        {
            bomb.RemoveDBEventAllListener(DragonBones.EventObject.COMPLETE);
            bomb.animationName = "a2";
            CommonTools.PlayArmatureAni(bomb.transform, "a2", 1);
            bomb.AddDBEventListener(DragonBones.EventObject.COMPLETE, (string type1, DragonBones.EventObject eventObject1) =>
            {
                bomb.RemoveDBEventAllListener(DragonBones.EventObject.COMPLETE);
                CommonTools.PlayArmatureAni(bomb.transform, "a2",0);
                CoreEntry.gAudioMgr.PlayUISound(136);
                MinesModel.Instance.minesItems.FindAll(x=>!x.isOpened).ForEach(x=>x.ShowAllReward());

            });
        });
    } 

    /// <summary>
    /// 显示奖励
    /// </summary>
    void ShowReward() 
    {
        
        bg.gameObject.SetActive(false);
        box.gameObject.SetActive(true);
        box.animationName = "a1";
        CommonTools.PlayArmatureAni(box.transform, "a1",1);
        coin.gameObject.SetActive(true);
        coin.animationName = "a1";
        CommonTools.PlayArmatureAni(coin.transform, "a1", 1);
        var num = MinesModel.Instance.plusQueue[MinesModel.Instance.BombCount].Dequeue();
        MinesModel.Instance.PlusGolds += float.Parse(num, new CultureInfo("en"));
        MinesModel.Instance.SetNextGolds(float.Parse(num, new CultureInfo("en")));
        ToolUtil.FloattingTextMines($"+{num}", this.transform);
        if (MinesModel.Instance.textAction != null)
        {
            MinesModel.Instance.textAction();
        }
        CoreEntry.gAudioMgr.PlayUISound(129);
        box.AddDBEventListener(DragonBones.EventObject.COMPLETE, (string type, DragonBones.EventObject eventObject) =>
        {
            box.RemoveDBEventAllListener(DragonBones.EventObject.COMPLETE);
            box.animationName = "a2";
            CommonTools.PlayArmatureAni(box.transform, "a2", 1);
            box.AddDBEventListener(DragonBones.EventObject.COMPLETE, (string type1, DragonBones.EventObject eventObject1) =>
            {
                box.RemoveDBEventAllListener(DragonBones.EventObject.COMPLETE);
                CommonTools.PlayArmatureAni(box.transform, "a2", 1);
                MinesModel.Instance.GridCount--;
                if (MinesModel.Instance.GridCount==MinesModel.Instance.BombCount)
                {
                    Debug.Log($"<color=#ffff00>游戏结束！结算</color>");
                    MinesModel.Instance.minesState = MinesState.GameOver;
                    CoreEntry.gAudioMgr.PlayUISound(136);
                    MinesModel.Instance.minesItems.FindAll(x => !x.isOpened).ForEach(x => x.ShowAllReward());
                    MinesModel.Instance.SetPlusGolds(0);
                    MinesModel.Instance.SetNextGolds(0);
                    MinesModel.Instance.GridCount = 25;
                    MinesModel.Instance.ShowWinAni();
                }
            });
        });
    }
    /// <summary>
    /// 显示剩余宝箱
    /// </summary>
    public void ShowAllReward() 
    {
        if (isMine())
        {
            bg.gameObject.SetActive(false);
            bomb.gameObject.SetActive(true);
            bomb.animationName = "a3";
            CommonTools.PlayArmatureAni(bomb.transform, "a3", 1);
        }
        else
        {
            bg.gameObject.SetActive(false);
            box.gameObject.SetActive(true);
            box.animationName = "a3";
            CommonTools.PlayArmatureAni(box.transform, "a3", 1);
        }
       

    }

    public void PlayAniamtor() 
    {
        transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.3f).OnComplete(delegate
        {
           transform.DOScale(Vector3.one, 0.3f);
        });
    }
}
