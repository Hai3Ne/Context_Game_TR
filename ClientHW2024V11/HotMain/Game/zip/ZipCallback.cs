using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ZipHelper;

public class ZipCallback : UnzipCallback
{
    public long allSize = 0;
    public long curSize = 0;
    public int state = 0;
    public Action<float> action;
    public void OnFinished(bool _result)
    {
        state = _result ? 1 : 0;

    }

    public void OnPostUnzip(ZipEntry _entry)
    {
        curSize += _entry.CompressedSize;
        if(action != null)
        {
            action(curSize/ allSize);
        }
    }

    public bool OnPreUnzip(ZipEntry _entry)
    {

        return true;
    }


}
