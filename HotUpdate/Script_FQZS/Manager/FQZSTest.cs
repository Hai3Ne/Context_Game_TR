using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FQZSTest : MonoBehaviour
{
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            GameManager.EnterGame(GameEnum.FQZS, new tagGameServer() { ServerAddr  = "192.168.1.127", ServerPort = 42002 });
        }
	}
}
