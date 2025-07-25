using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TournamentItem : MonoBehaviour
{
    [SerializeField] private Text name;
    [SerializeField] private GameObject nameBg;
    [SerializeField] private Image head;
    [SerializeField] private Image headBg;
    [SerializeField] private Image headBg1;
    [SerializeField] private GameObject noneName;
    Sprite midSprite;
    private long id;
    public void SetUpItem(SCommArenaData data) 
    {

       
        if (data!=null)
        {
            id = data.n64Charguid;
            //name.text = data.playerName;
            name.text = ToolUtil.MaskText(CommonTools.BytesToString(data.szName), 5);
            headBg.gameObject.SetActive(true);
            headBg1.gameObject.SetActive(false);
            name.gameObject.SetActive(true);
            noneName.SetActive(false);
            var url = Encoding.UTF8.GetString(data.szHeadUrl).Replace("\0", null);
            if (url == null || url == "")
            {
                head.sprite = null;
                return;
            }
            UICtrl.Instance.StartCoroutine(GetHeadImage(url, id));
         
        }
        else
        {
            name.text = "";
            head.sprite = null;
            headBg.gameObject.SetActive(false);
            headBg1.gameObject.SetActive(true);
            name.gameObject.SetActive(false);
            noneName.SetActive(true);
        }
       

    }

    public IEnumerator GetHeadImage(string headimgurl, long myId)
    {
        if (headimgurl == null || headimgurl == "")
        {
            head.sprite = null;
            yield break;
        }

        if (headimgurl != null)
        {
            if (MainUIModel.Instance.HeadUrl.ContainsKey(headimgurl))
            {
                if (MainUIModel.Instance.HeadUrl[headimgurl] == null)
                {
                    yield break;
                }
                Texture2D texture2d = MainUIModel.Instance.HeadUrl[headimgurl];
                Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                head.sprite = sprite;
                yield break;
            }

            WWW www = new WWW(headimgurl);
            yield return www;

            if (www.error == null)
            {
                Texture2D texture2d = www.texture;
                if (!MainUIModel.Instance.HeadUrl.ContainsKey(headimgurl))
                    MainUIModel.Instance.HeadUrl.Add(headimgurl, texture2d);
                MainUIModel.Instance.HeadUrl[headimgurl] = texture2d;
                if (id != myId) yield break;
                Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                head.sprite = sprite;
                midSprite = head.sprite;

            }
            else
            {
                head.sprite = null;
                Debug.Log("下载出错" + "," + www.error);
            }
        }
    }

}
