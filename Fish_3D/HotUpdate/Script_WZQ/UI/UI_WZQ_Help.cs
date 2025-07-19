using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WZQ_Help : UILayer
{
    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "hlep_close":
                Close();
                break;
        }
    }
}
