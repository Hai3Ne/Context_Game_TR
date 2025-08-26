using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RankItemChild : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image head;
    [SerializeField] private Text IdTxt;
    [SerializeField] private Text nameTxt;
    [SerializeField] private Text goldTxt;
    [SerializeField] private Button ClaimBtn;
    [SerializeField] private Image reward;
    [SerializeField] private Text rewardNum;
    [SerializeField] private Image iconIdTxt;
    [SerializeField] private Image Bg;
    Sprite midSprite;
    private string hearUrl;
    SCommRankData itemData;
    private long id;
    public void SetUpItem(SCommRankData data,int rankType,int rank) 
    {
        transform.Find("bg/rank1").gameObject.SetActive(rank == 1);
        transform.Find("bg/rank2").gameObject.SetActive(rank == 2);
        transform.Find("bg/rank3").gameObject.SetActive(rank == 3);
        var rewardConfig = ConfigCtrl.Instance.Tables.TbGameRank.DataList;
        var rewardData = rewardConfig.Find(x => x.Type == rankType && x.Rankid == rank);
        rewardNum.text = $"{ToolUtil.AbbreviateNumberf0(rewardData.Award)}";
        if (data == null)
        {
            IdTxt.gameObject.SetActive(rank > 3);
            IdTxt.text = rank.ToString();
            nameTxt.text = "虚拟以待";
            goldTxt.text = "";
            //rewardNum.text = "";
            Bg.gameObject.SetActive(true);
            head.gameObject.SetActive(false);
        }
        else
        {
            itemData = data;
            id = data.n64Charguid;
            head.gameObject.SetActive(true); 
            Bg.gameObject.SetActive(false);
            //icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Rank:item_silver_{rank}");
            //SetItemBg(rank);
            //IdTxt.gameObject.SetActive(rank >3);
            //iconIdTxt.gameObject.SetActive(data.rank > 3);
            //icon.gameObject.SetActive(rank <= 3);
            //icon.SetNativeSize();
            IdTxt.text = rank.ToString();
            IdTxt.gameObject.SetActive(rank > 3);
            //nameTxt.text = data.playerName;
            nameTxt.text = CommonTools.BytesToString(data.szName);
            goldTxt.text = ToolUtil.AbbreviateNumberf0(data.n64Total);
            //reward.sprite = AtlasSpriteManager.Instance.GetSprite($"NoviceGiftPanel:huangj_00");
            var url = Encoding.UTF8.GetString(data.szHeadUrl).Replace("\0", null);
            if (url == null || url == "")
            {
                head.sprite = null;
                return;
            }
            UICtrl.Instance.StartCoroutine(GetHeadImage(url, data.n64Charguid));
        }
   
      
    }

    private void SetItemBg(int rankNum) 
    {
        switch (rankNum)
        {
            case 1:
                Bg.sprite = AtlasSpriteManager.Instance.GetSprite($"Rank:frame_silver_medal");
                break;
            case 2:
                Bg.sprite = AtlasSpriteManager.Instance.GetSprite($"Rank:frame_silver_trophy");
                break;
            case 3:
                Bg.sprite = AtlasSpriteManager.Instance.GetSprite($"Rank:frame_bronze_medal");
                break;
            default:
                Bg.sprite = AtlasSpriteManager.Instance.GetSprite($"Rank:frame_rank");
                break;
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
                Debug.Log("���س���" + "," + www.error);
            }
        }
    }

}
