using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private Text text;
    public void Awake()
    {
        text = transform.Find("bg/Text").GetComponent<Text>();
    }

    /// <summary>
    /// 设置Text内容
    /// </summary>
    public string contant
    {
        get { return text.text; }
        set { text.text = value; }
    }

    /// <summary>
    /// 设置颜色
    /// </summary>
    public Color color
    {
        set { text.color = value; }
        get { return text.color; }
    }
}