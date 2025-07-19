using UnityEngine;
using System.Collections;
using System;
using System.IO;
public class ClsTuple<T,V>:IDisposable
{
	public T field0;
	public V field1;

	public ClsTuple ()
	{

	}

	public ClsTuple (T t, V v)
	{
		this.field0 = t;
		this.field1 = v;
	}

	public void Dispose ()
	{

		free (field0);
		free (field1);
		GC.SuppressFinalize (this);
	}

	void free (object obj)
	{
		if (obj != null && obj.GetType () == typeof(Stream)) {
			((Stream)obj).Close ();
		}
	}
}


public class ClsTuple<T,V,U>:IDisposable
{
	public T field0;
	public V field1;
	public U field2;

	public ClsTuple ()
	{

	}

	public ClsTuple (T t, V v, U u)
	{
		this.field0 = t;
		this.field1 = v;
		this.field2 = u;
	}

	public void Dispose ()
	{
		free (field0);
		free (field1);
		free (field2);
		GC.SuppressFinalize (this);
	}

	void free (object obj)
	{
		if (obj != null && obj.GetType () == typeof(Stream)) {
			((Stream)obj).Close ();
		}
	}
}

public class ClsTuple<T,V,U,K>:IDisposable
{
	public T field0;
	public V field1;
	public U field2;
	public K field3;

	public ClsTuple ()
	{

	}

	public ClsTuple (T t, V v, U u, K k)
	{
		this.field0 = t;
		this.field1 = v;
		this.field2 = u;
		this.field3 = k;
	}

	public void Dispose ()
	{
		free (field0);
		free (field1);
		free (field2);
		free (field3);
		GC.SuppressFinalize (this);
	}

	void free (object obj)
	{
		if (obj != null && obj.GetType () == typeof(Stream)) {
			((Stream)obj).Close ();
		}
	}
}

public class ClsTuple<T,V,U,K,P>:IDisposable
{
	public T field0;
	public V field1;
	public U field2;
	public K field3;
	public P field4;

	public ClsTuple ()
	{

	}

	public ClsTuple (T t, V v, U u, K k, P p)
	{
		this.field0 = t;
		this.field1 = v;
		this.field2 = u;
		this.field3 = k;
		this.field4 = p;
	}

	public void Dispose ()
	{
		free (field0);
		free (field1);
		free (field2);
		free (field3);
		free (field4);
		GC.SuppressFinalize (this);
	}

	void free (object obj)
	{
		if (obj != null && obj.GetType () == typeof(Stream)) {
			((Stream)obj).Close ();
		}
	}
}

public class ClsTuple<T,V,U,K,P,L>:IDisposable
{
	public T field0;
	public V field1;
	public U field2;
	public K field3;
	public P field4;
	public L field5;

	public ClsTuple ()
	{

	}

	public ClsTuple (T t, V v, U u, K k, P p, L l)
	{
		this.field0 = t;
		this.field1 = v;
		this.field2 = u;
		this.field3 = k;
		this.field4 = p;
		this.field5 = l;
	}

	public void Dispose ()
	{
		free (field0);
		free (field1);
		free (field2);
		free (field3);
		free (field4);
		free (field5);
		GC.SuppressFinalize (this);
	}

	void free (object obj)
	{
		if (obj.GetType () == typeof(Stream)) {
			((Stream)obj).Close ();
		}
	}
}