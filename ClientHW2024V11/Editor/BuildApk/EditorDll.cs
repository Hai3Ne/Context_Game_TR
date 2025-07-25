using UnityEngine;
using System.Collections;
using System.IO;


public class EditorDll {

    static EditorDll instance;
    public static EditorDll Instance { get {
        if (instance == null)
            instance = new EditorDll();
        return instance;
    } }


    int key = 659875436;

    public void Do(string dllPath)
    {
        byte[] bytes = File.ReadAllBytes(dllPath);

        for (int i = 0; i < 999999; i++)
        {
            bytes[i] = (byte)(bytes[i] ^ key);
        }
        File.WriteAllBytes(dllPath, bytes);
    }

}
