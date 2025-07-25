using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesBetItem : MonoBehaviour
{
    [SerializeField] private GameObject select;
    [SerializeField] private GameObject showBomb;
    [SerializeField] private GameObject hideBomb;
    public void Select() 
    {
        select.SetActive(true);
        showBomb.SetActive(true);
        hideBomb.SetActive(false);
    }

    public void Hide() 
    {
        select.SetActive(false);
        showBomb.SetActive(false);
        hideBomb.SetActive(true);
    }
}
