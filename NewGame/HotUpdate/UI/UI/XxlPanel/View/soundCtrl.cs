using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundCtrl
{
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;
    public static soundCtrl ins = null;

    public AudioSource soundCmp;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void playClick()
    {
        soundCmp.PlayOneShot(clip1);
    }
    public void playBg()
    {
        soundCmp.clip = clip3;
        soundCmp.loop = true;
        soundCmp.Play();


    }
    public void playxiaochu()
    {
        soundCmp.PlayOneShot(clip2);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
