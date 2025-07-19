using HotUpdate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAniEvent : MonoBehaviour
{

    public void AnimationBegin(string message)
    {

        Message.Broadcast<GameObject, string>(MessageName.ANIBEGIN, gameObject, message);
    }
    public void AnimationEvent(string message)
    {
        Message.Broadcast<GameObject, string>(MessageName.ANIEVENT, gameObject, message);
    }

    public void AnimationEnd(string message)
    {
        Message.Broadcast<GameObject, string>(MessageName.ANIEnd, gameObject, message);
    }
}
