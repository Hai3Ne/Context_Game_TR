using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class HUDText : MonoBehaviour
{
    /// <summary>
    /// 漂浮文字生成到某个父Canvas
    /// </summary>
    public Canvas ParnetCanvas;
    /// <summary>
    /// 漂浮文字预制体
    /// </summary>
    public GameObject TextPrefab;
    /// <summary>
    /// 时间
    /// </summary>
    public float FadeTime = 0.56f;

    public void Add(string content, Vector3 worldpos, Color color)
    {
        GameObject text = Instantiate(TextPrefab) as GameObject;
        RectTransform floatingTextT = text.transform as RectTransform;
        floatingTextT.SetParent(ParnetCanvas.transform, false);
        floatingTextT.localScale = Vector3.one;
        FloatingText floatingText = floatingTextT.GetComponent<FloatingText>();
        floatingText.contant = content;
        floatingText.color = color;
        floatingText.transform.DOLocalMoveY(floatingText.transform.position.y+50f,2f).OnComplete(delegate
        {
            Destroy(floatingText.gameObject);
        });

    }

}