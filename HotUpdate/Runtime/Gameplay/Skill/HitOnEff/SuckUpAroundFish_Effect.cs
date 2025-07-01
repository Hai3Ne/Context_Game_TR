using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuckUpAroundFish_Effect : BaseSkillEffect
{
	protected override void Start ()
	{
		base.Start ();
	}

    public override bool Update(float delta) {
        return false;
    }

	public override void Destroy() {
		base.Destroy ();	
	}
}
