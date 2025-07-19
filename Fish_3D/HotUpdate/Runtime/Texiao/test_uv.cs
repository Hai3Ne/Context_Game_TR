using UnityEngine;
using System.Collections;

public class test_uv : MonoBehaviour
{
    public float xspeed = 0;
    public float yspeed = 0;
    private Vector2 v2;
    private Material mMaterial;
    void Start()
    {
        v2 = Vector2.zero;
        this.mMaterial = GetComponent<Renderer>().material;
        //AssetBundleManager.GetBundle("dataconfig", getdata);
    }
    void Update()
    {
        v2.x += Time.fixedDeltaTime*xspeed;
        v2.y += Time.fixedDeltaTime*yspeed;
        this.mMaterial.mainTextureOffset = v2;
    }
 

}
