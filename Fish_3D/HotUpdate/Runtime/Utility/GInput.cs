using System;
using UnityEngine;
public class GInput
{
	static bool mEnable = true;
	public static bool Enabled
	{
		set { mEnable = value;}
		get { return mEnable;}

	}

	public static bool IsBlock3D {
		get ;
		set;
	}
	public static Vector3 mousePosition
	{
		get { return Input.mousePosition;}
	}

	public static bool GetMouseButtonUp(int button)
	{
		if (Enabled)
			return Input.GetMouseButtonUp(button);
		return true;
	}

	public static bool GetMouseButton(int button)
	{
		if (Enabled)
			return Input.GetMouseButton(button);
		return false;
	}

    public static bool GetKeyDown(KeyCode code) {
        if (Enabled) {
            return Input.GetKeyDown(code);
        }
        return false;
    }
    public static bool GetKey(KeyCode code) {
        if (Enabled) {
            return Input.GetKey(code);
        }
        return false;
    }

	public static bool  GetKeyUp(KeyCode code){
		if (Enabled)
			return Input.GetKeyUp (code);
		return false;
	}

	public static bool GetMouseButtonDown(int button)
	{
		if (Enabled)
			return Input.GetMouseButtonDown (button);
		return false;
	}
}