using System;
using System.Collections.Generic;
using System.Reflection;
public interface IRunUpdate
{
	void Update (float delta);
}

public class TypeInfo : System.Attribute
{
    public ushort    Count;
    public short    Index;
    public bool     VarType;
    public TypeInfo(short index)
    {
        Index = index;
        Count = 0;
        VarType = false;
    }

    public TypeInfo(short index, bool bVarType)
    {
        Index = index;
        Count = 1;
        VarType = bVarType;
    }
    public TypeInfo(short index, ushort count)
    {
        if (count < 1)
            Count = 0;
        else
            Count = count;
        Index = index;
        VarType = false;
    }
}

public class CustomTypeParse : System.Attribute
{
}

