using System;

public class int256
{
	[TypeInfo(0, 32)]
	public Byte[] Value;
}
public class int256Function
{
	public static void Clear(int256 p)
	{
		for (int i = 0; i < p.Value.Length; ++i)
			p.Value[i] = 0;
	}
	public static void SetBitStates(int256 p, Byte BitIndex, bool States)
	{
		Byte ArrayIndex = Convert.ToByte(BitIndex / 8);
		Byte ChangeIndex = Convert.ToByte(BitIndex %8);
		Byte BitValue = Convert.ToByte(1 << ChangeIndex);
		if (States)
			p.Value[ArrayIndex] |= BitValue;
		else
			p.Value[ArrayIndex] ^= BitValue;
	}
	public static bool GetBitStates(int256 p, Byte BitIndex)
	{
		Byte ArrayIndex = Convert.ToByte(BitIndex / 8);
		Byte ChangeIndex = Convert.ToByte(BitIndex % 8);
		return ((p.Value[ArrayIndex] & (1 << ChangeIndex)) == (1 << ChangeIndex));
	}
}
