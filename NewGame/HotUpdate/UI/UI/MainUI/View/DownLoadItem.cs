using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownLoadItem : MonoBehaviour
{
    [SerializeField] private GameObject mask;
    [SerializeField] private Text progressTxt;
    [SerializeField] private Button downLoadBtn;
    [SerializeField] private string m_packName;
    [SerializeField] private string m_gameName;
    private void OnDisable()
    {
        CoreEntry.gEventMgr.RemoveListener(GameEvent.SubPackPross, SubPackPross);
        downLoadBtn.onClick.RemoveListener(OnDownLoadBtn);
    }

    private bool IsDown = false;
    private void OnEnable()
    {
        CoreEntry.gEventMgr.AddListener(GameEvent.SubPackPross, SubPackPross);
        downLoadBtn.onClick.AddListener(OnDownLoadBtn);
        if (m_packName != "" && m_packName != null)
        {
            var isDown = CommonTools.CheckSubPack(m_packName);
            mask.SetActive(!isDown || m_packName == "");
            downLoadBtn.gameObject.SetActive(!isDown && m_packName != "");
            progressTxt.gameObject.SetActive(!isDown && mask.activeSelf && IsDown);
        }
    }

    public void SetUpItem(string packName, string gameName)
    {
        m_packName = packName;
        m_gameName = gameName;

        var isDown = CommonTools.CheckSubPack(packName);

        mask.SetActive(!isDown || m_packName == "");
        downLoadBtn.gameObject.SetActive(!isDown && m_packName != "");
        progressTxt.gameObject.SetActive(!isDown && mask.activeSelf && IsDown);
    }

    /// <summary>
    /// 下载分包
    /// </summary>
    /// <param name="packName"></param>
    public void DownLoadSubPack(string packName)
    {
        IsDown = true;
        DownSubPack.Instance.downSubPack(packName);

    }


    /// <summary>
    /// 下载分包进度
    /// </summary>
    /// <param name="ge"></param>
    /// <param name="parameter"></param>
    public void SubPackPross(GameEvent ge, EventParameter parameter)
    {
        bool isComplete = false;
        bool isCreat = false;
        var nameList = parameter.stringParameter.Split('|');
        var packName = nameList[0];
        var curProgress = nameList[1];
        var allSize = nameList[2];
        if (!m_packName.Equals(packName))
        {
            return;
        }
        progressTxt.gameObject.SetActive(true);
        var num = (float.Parse(curProgress, new CultureInfo("en")) / float.Parse(allSize, new CultureInfo("en")));
        mask.GetComponent<Image>().fillAmount = 1 - num;
        var progressNum = Mathf.Min(100, Mathf.Floor(float.Parse(curProgress, new CultureInfo("en")) / float.Parse(allSize, new CultureInfo("en")) * 100));
        progressTxt.text = $"{progressNum}%";
        if (progressNum >= 100)
        {
            progressTxt.gameObject.SetActive(false);
            mask.SetActive(false);
            isComplete = true;
            IsDown = false;
        }
        if (isComplete && !isCreat)
        {
            isCreat = true;
            var str = string.Format("{0}游戏更新已完成", m_gameName);
            ToolUtil.FloattingText(str, MainPanelMgr.Instance.GetPanel("MainUIPanel").transform);
        }

    }

    public void OnDownLoadBtn()
    {
        if (m_packName == "") return;
        var str = m_packName + "|" + 0 + "|" + 1;
        SubPackPross(GameEvent.SubPackPross, EventParameter.Get(str));
        DownLoadSubPack(m_packName);
    }
}
