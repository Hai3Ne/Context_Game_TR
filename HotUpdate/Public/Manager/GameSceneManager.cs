using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager {
    /// <summary>
    /// 返回执行类型房间选项
    /// </summary>
    /// <param name="type"></param>
    public static void BackToHall(GameEnum type) {
        if (GameManager.CurGameEnum != GameEnum.None) {
            AudioManager.StopAllAudio();
        }
        GameManager.SetGameEnum(GameEnum.None);
        switch (type)
        {
            case GameEnum.Fish_3D://3D捕鱼
                ResVersionManager.CheckVersion(GameEnum.Fish_3D, () =>
                {
                    if (UI.GetUI<UI_3D_Main>() == null)
                    {
                        UI.ExitAllUI();
                        UI.EnterUI<UI_3D_Main>(GameEnum.Fish_3D).InitData();
                    }
                });
                break;
            case GameEnum.Fish_LK://李逵劈鱼
                ResVersionManager.CheckVersion(GameEnum.Fish_LK, () => 
                {
                    //检测资源是否下载
                    if (UI.GetUI<UI_LK_Main>() == null)
                    {
                        UI.ExitAllUI();
                        UI.EnterUI<UI_LK_Main>(GameEnum.Fish_LK).InitData();
                    }
                });
                break;
            case GameEnum.FQZS://飞禽走兽
                ResVersionManager.CheckVersion(GameEnum.FQZS, () =>
                {
                    //检测资源是否下载
                    if (UI.GetUI<UI_FQZSchooseroom>() == null)
                    {
                        UI.ExitAllUI();
                        UI.EnterUI<UI_FQZSchooseroom>(GameEnum.FQZS).InitData();
                    }
                });
                break;
            case GameEnum.SH://神话
                ResVersionManager.CheckVersion(GameEnum.SH, () => 
                {
                    if (UI.GetUI<UI_SHchooseroom>() == null)
                    {
                        UI.ExitAllUI();
                        UI.EnterUI<UI_SHchooseroom>(GameEnum.SH).InitData();
                    }
                });
                break;
            case GameEnum.WZQ://五子棋
                ResVersionManager.CheckVersion(GameEnum.WZQ, () =>
                {
                    if (UI.GetUI<UI_wzqmain>() == null)
                    {
                        UI.ExitAllUI();
                        UI.EnterUI<UI_wzqmain>(GameEnum.WZQ).InitData();
                    }
                });
                break;
            default://大厅
                if (UI.GetUI<UI_mainhall_new>() == null) {
                    UI.ExitAllUI();

                    //UI.EnterUI<UI_mainhall_new>(null);
                    UI.EnterUI<UI_mainhall_new>(GameEnum.All);
                }
                break;
        }
    }
}
