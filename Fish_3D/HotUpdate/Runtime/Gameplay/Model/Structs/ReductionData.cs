using System;
using System.Collections.Generic;

public class ReductionData
{
    public ReductionData(float speed, float d1, float d2, float d3)
    {
        Speed = speed;
        Duration1 = d1;
        Duration2 = d2;
        Duration3 = d3;
    }
	public FISH_DELAY_TYPE DelayType = FISH_DELAY_TYPE.DELAY_ATTACK;
    public float Speed;
    public float Duration1;
    public float Duration2;
    public float Duration3;
}

public class ActionSpeedAlter
{
	static int id_static = 1;
	public ActionSpeedAlter(float actionRate, float fadeTime =0f){
		actionSpeed = actionRate;
		fadeInTime = fadeTime;
		id = id_static;
	}
	public int id;
	public float fadeInTime = 0f;
	public float actionSpeed = 0f;
}