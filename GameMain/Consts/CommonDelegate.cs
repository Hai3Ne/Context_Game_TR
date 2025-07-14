using UnityEngine;
using System.Collections;

public delegate void VoidDelegate();
public delegate void VoidCall();
public delegate void VoidCall<T1>(T1 t1);
public delegate void VoidCall<T1, T2>(T1 t1, T2 t2);
public delegate void VoidCall<T1, T2, T3>(T1 t1, T2 t2,T3 t3);