using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Item_ShenHua_Message : UIItem {
    private int ItemMaxCount = 20;

    public GameObject mItemMsg;
    public GameObject mItemEx;
    public GameObject mItemResult;
    public UIPanel mPanelMsg;
    public UIScrollView mScrollView;

    public List<GameObject> mMsgList = new List<GameObject>();
    private List<GameObject> msg_list = new List<GameObject>();
    private List<GameObject> result_list = new List<GameObject>();
    private List<GameObject> ex_list = new List<GameObject>();

    public void AddMessage(string left_msg, Color left_col, string right_msg, Color right_col) {
        GameObject obj = null;
        if (this.mMsgList.Count >= ItemMaxCount) {
            this.mMsgList[0].SetActive(false);
            this.mMsgList.RemoveAt(0);
            foreach (var item in ex_list) {
                if (item.activeSelf == false) {
                    obj = item;
                    obj.SetActive(true);
                    break;
                }
            }
        }
        if (obj == null) {
            obj = GameObject.Instantiate(this.mItemEx, this.mScrollView.transform);
            obj.SetActive(true);
            ex_list.Add(obj);
        }
        this.mMsgList.Add(obj);

        UILabel lb_left = obj.transform.Find("item_lb_left").GetComponent<UILabel>();
        UILabel lb_right = obj.transform.Find("item_lb_right").GetComponent<UILabel>();
        lb_left.color = left_col;
        lb_right.color = right_col;
        lb_left.text = left_msg;
        lb_right.text = right_msg;

        obj.transform.localPosition = new Vector3(0, 6 - this.mScrollView.panel.height * 0.5f);
        int height = lb_left.height;
        foreach (var item in this.mMsgList) {
            item.transform.localPosition += new Vector3(0, height);
        }
    }
    public void AddMessage(string msg, Vector3 col,bool is_sys = true)
    {
        GameObject obj = null;
        if (this.mMsgList.Count >= ItemMaxCount)
        {
            this.mMsgList[0].SetActive(false);
            this.mMsgList.RemoveAt(0);
            foreach (var item in msg_list)
            {
                if (item.activeSelf == false)
                {
                    obj = item;
                    obj.SetActive(true);
                    break;
                }
            }
        }

        if (obj == null)
        {
            obj = Instantiate(this.mItemMsg, this.mScrollView.transform);
            obj.SetActive(true);
            msg_list.Add(obj);
        }

        mMsgList.Add(obj);

        UILabel lb = obj.GetComponent<UILabel>();

        lb.color = new Color(col.x, col.y, col.z);
        if (is_sys)
        {
            lb.text = string.Format("[c][ff0000][系统消息]:[-][/c]{0}", msg);
        }
        else
        {
            lb.text = msg;
        }

        lb.transform.localPosition = new Vector3(0, 6 - this.mScrollView.panel.height * 0.5f);
        int height = lb.height;
        foreach (var item in this.mMsgList)
        {
            item.transform.localPosition += new Vector3(0, height);
        }
    }

    public void AddMessage(long leizhu_result, long self_result, long bet, long sex) {
        GameObject obj = null;
        if (this.mMsgList.Count >= ItemMaxCount) {
            this.mMsgList[0].SetActive(false);
            this.mMsgList.RemoveAt(0);
            foreach (var item in result_list) {
                if (item.activeSelf == false) {
                    obj = item;
                    obj.SetActive(true);
                    break;
                }
            }
        }
        if (obj == null) {
            obj = GameObject.Instantiate(this.mItemResult, this.mScrollView.transform);
            obj.SetActive(true);
            result_list.Add(obj);
        }
        this.mMsgList.Add(obj);

        UILabel lb_leizhu_result = obj.transform.Find("item_lb_leizhu_result").GetComponent<UILabel>();
        UILabel lb_self_result = obj.transform.Find("item_lb_self_result").GetComponent<UILabel>();
        UILabel lb_self_bet = obj.transform.Find("item_lb_self_total_result").GetComponent<UILabel>();
        UILabel lb_self_sex = obj.transform.Find("item_lb_sex_result").GetComponent<UILabel>();
        if (leizhu_result > 0)
        {
            lb_leizhu_result.text = string.Format("[c][ffff00]+{0}[-][/c]", leizhu_result);
        }
        else
        {
            lb_leizhu_result.text = string.Format("[c][ffff00]{0}[-][/c]", leizhu_result);
        }
        if (self_result > 0)
        {
            lb_self_result.text = string.Format("[c][ffff00]+{0}[-][/c]", self_result);
        }
        else
        {
            lb_self_result.text = string.Format("[c][ffff00]{0}[-][/c]", self_result);
        }

        lb_self_bet.text = bet.ToString();

        lb_self_sex.gameObject.SetActive(true);
        lb_self_sex.text = sex.ToString();
        obj.transform.localPosition = new Vector3(0, 6 - this.mScrollView.panel.height * 0.5f);
        int height = 140;
        foreach (var item in this.mMsgList)
        {
            item.transform.localPosition += new Vector3(0, height);
        }
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_msg":
                this.mItemMsg = tf.gameObject;
                this.mItemMsg.SetActive(false);
                break;
            case "item_ex":
                this.mItemEx = tf.gameObject;
                this.mItemEx.SetActive(false);
                break;
            case "item_result_info":
                this.mItemResult = tf.gameObject;
                this.mItemResult.SetActive(false);
                break;
            case "scrollview_message":
                this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
        }
    }
}
