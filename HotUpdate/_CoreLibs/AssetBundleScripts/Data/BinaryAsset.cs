using System;
using UnityEngine;
using System.Runtime.CompilerServices;

public class BinaryAsset : UnityEngine.Object
{
	public byte[] bytes { get; set;}

	public BinaryAsset (){}
	public override string ToString ()
	{
		return base.ToString ();
	}
}