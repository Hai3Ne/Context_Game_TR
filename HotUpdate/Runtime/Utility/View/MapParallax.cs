using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxItem{
	public Transform Trans;
	public Transform FollowTrans;
	public Vector3 InitedPos;
	public float Speed;
}

public class MapParallax : MonoBehaviour {

	int BaseQueue = 1000;
	float speedRate = 1f;
	bool isSideAlign = false;
	public System.Action onArriveSide;
	List<ParallaxItem> mLayers = new List<ParallaxItem>();
	Vector3 leftPos, rightPos;
	float screenWidth = 0f;
	float realBgWidth = 0f;

	float leftSideX = 0f;
    void Start() { 
		leftPos = Camera.main.ViewportToWorldPoint (new Vector3(0f, 0f, Camera.main.farClipPlane));
		rightPos = Camera.main.ViewportToWorldPoint (new Vector3(1f, 1f, Camera.main.farClipPlane));
		screenWidth =  rightPos.x - leftPos.x;

		Vector3 posOffset = Vector3.zero;
        realBgWidth = Mathf.Floor(this.transform.localScale.x * Camera.main.fieldOfView * 0.5f) - 1;
		if (isSideAlign) {			
			posOffset = Vector3.right * (realBgWidth + screenWidth) * 0.5f;
		}

		for (int i = 0; i < this.transform.childCount; i++) 
		{
			GameObject childGo = this.transform.GetChild (i).gameObject;
			var item = new ParallaxItem ();
			item.Trans = childGo.transform;
			item.Speed = GetLayerSpeed (childGo);
			item.InitedPos = childGo.transform.position;
			item.Trans.position += posOffset;
			mLayers.Add (item);
		}

		leftSideX = (realBgWidth-screenWidth) * 0.5f;///leftPos.x;//mLayers [0].Trans.position.x - screenWidth;
		int cnt = mLayers.Count;
		for (int i = 0; i < cnt; i++) {
			var orgItem = mLayers [i];
			int depth = mLayers [i].Trans.GetSiblingIndex ()+1;
			GameObject copy = GameObject.Instantiate(mLayers[i].Trans.gameObject);
			copy.transform.SetParent (mLayers[i].Trans.parent);
			copy.transform.position = mLayers[i].Trans.position + Vector3.right * realBgWidth;
			copy.transform.localScale = mLayers [i].Trans.localScale;
			copy.transform.localRotation = mLayers [i].Trans.localRotation;
			copy.transform.SetSiblingIndex (depth);
			orgItem.FollowTrans = copy.transform;

			var item = new ParallaxItem ();
			item.Trans = copy.transform;
			item.InitedPos = mLayers [i].InitedPos;
			item.Speed = mLayers [i].Speed;
			item.FollowTrans = mLayers [i].Trans;
			mLayers.Add (item);
		}
		SortDepth ();
		current = this;
	}

	float GetLayerSpeed(GameObject go)
	{
		var comp = go.GetComponent<MapObjParallax> ();
		if (comp != null)
			return comp.speed;
		return 1f;
	}

	bool applyLayerSpeedSame = false;
	public static MapParallax current;
	public static void BgFade(MapParallax oldBG, MapParallax newBG, float fadeSpeed,float time)
    {
        if(oldBG != null)
		    oldBG.speedRate = fadeSpeed;
		newBG.isSideAlign = true;
		newBG.speedRate = fadeSpeed;
		newBG.BaseQueue = 1100;
		newBG.applyLayerSpeedSame = true;
		newBG.onArriveSide = ()=>
        {
            if (oldBG != null && oldBG.gameObject != null)
            {
                Destroy(oldBG.gameObject);
            }
			newBG.speedRate = 1;
			newBG.applyLayerSpeedSame = false;
			newBG.BaseQueue = 1000;
			newBG.SortDepth();
        };

        if(oldBG != null)
            oldBG.add_time = time;
        newBG.add_time = time;
	}

    private float add_time = 0;
    private void UpdatePos(float delta) {
        if (add_time > 0) {
            delta += add_time;
            add_time = 0;
        }
        int i = 0;
        while (i < mLayers.Count) {
            UpdateLayerPos(mLayers[i], delta, 0);
            i++;
        }
        i = 0;
        while (i < mLayers.Count) {
            UpdateLayerPos(mLayers[i], delta, 1);
            i++;
        }
        i = 0;
        while (i < mLayers.Count) {
            Vector3 objInitpos = mLayers[i].InitedPos;
            float offset = objInitpos.x - mLayers[i].Trans.position.x - realBgWidth;
            if (offset >= 0) {
                mLayers[i].Trans.position = objInitpos + Vector3.right * (realBgWidth - offset);
            }
            i++;
        }

        if (onArriveSide != null && mLayers.Count > 0 && mLayers[0].Trans.position.x <= leftSideX) {
            onArriveSide.TryCall();
            onArriveSide = null;
        }
    }

	void Update ()
	{
        this.UpdatePos(Time.deltaTime);
	}

	const float rate = 63f;
	bool UpdateLayerPos(ParallaxItem item, float delta, int type = 0)
	{
		float speed = item.Speed;
		if (applyLayerSpeedSame) {
			speed = 1f;
		}
		if (type == 0) {
			if (item.FollowTrans.position.x >= item.Trans.position.x) {
				float delatx = delta * speed * rate * speedRate;
				Vector3 pos = item.Trans.position;
				pos.x -= delatx;//rightX, leftX, time);
				item.Trans.position = pos;
			}
		} else {
			if (item.FollowTrans.position.x < item.Trans.position.x) {
				item.Trans.position = item.FollowTrans.position + Vector3.right * realBgWidth;	
			}	
		}
		return true;
	}


	public void SortDepth()
	{
		List<Renderer> renders = new List<Renderer> ();
		GetAllRenders (this.transform, renders);
        //Renderer r;
        int count = renders.Count;
        //for (int i = 0; i < count; i++) {
        //    for (int j = 0; j < count; j++) {
        //        if (renders[i].transform.localPosition.y < renders[j].transform.localPosition.y) {
        //            r = renders[i];
        //            renders[i] = renders[j];
        //            renders[j] = r;
        //        }
        //    }
        //}
		#if UNITY_EDITOR
        for (int i = 0; i < count; i++) {
			renders [i].material.renderQueue = BaseQueue + i;
            renders[i].sortingOrder = BaseQueue + renders[i].sortingOrder % 100;
		}
		#else
		for (int i = 0; i < count; i++) {
			renders [i].sharedMaterial.renderQueue = BaseQueue + i;
            renders[i].sortingOrder = BaseQueue + renders[i].sortingOrder % 100;
		}
#endif
    }

	void GetAllRenders(Transform trans, List<Renderer> outRenders)
	{
		Renderer r = trans.GetComponent<Renderer> ();
		if (r != null)
			outRenders.Add (r);
		else {
			ParticleSystem ps = trans.GetComponent<ParticleSystem> ();
			if (ps != null && ps.GetComponent<Renderer>() != null) {
				outRenders.Add (ps.GetComponent<Renderer>());
			}
		}

		if (trans.childCount > 0) 
		{
			for (int i = 0; i < trans.childCount; i++) {
				GetAllRenders (trans.GetChild (i), outRenders);
			}
		}
	}
}
