using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
public class DrawPathCure : MonoBehaviour 
{
	//绘制线段材质
	public Material material;
	
	private List<Vector3> lineInfo = new List<Vector3>();

	// Use this for initialization
	void Start ()
	{
		//初始化鼠标线段链表
		lineInfo = new List<Vector3>();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		//将每次鼠标经过的位置存储进链表
		lineInfo.Add(Input.mousePosition);
		
	}

	/*private static void DrawPathHelper(Vector3[] path, Color color, string method){
	Vector3[] vector3s = PathControlPointGenerator(path);

	//Line Draw:
	Vector3 prevPt = Interp(vector3s,0);
	Gizmos.color=color;
	int SmoothAmount = path.Length*20;
	for (int i = 1; i <= SmoothAmount; i++) {
		float pm = (float) i / SmoothAmount;
		Vector3 currPt = Interp(vector3s,pm);
		if(method == "gizmos"){
			Gizmos.DrawLine(currPt, prevPt);
		}else if(method == "handles"){
			//Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			UnityEditor.Handles.DrawLine(currPt, prevPt);
		}
		prevPt = currPt;
	}
}	*/
	void OnGUI()
	{
		GUILayout.Label("X.Position :" + Input.mousePosition.x);
		GUILayout.Label("Y.Position :" + Input.mousePosition.y);
	}

	void OnDrawGizmos()
	{
		lineInfo.Clear ();
		if (CPathLinearRender.CurrentSelectedObj != null)
		{
			
			for (int i = 0; i < CPathLinearRender.CurrentSelectedObj.transform.childCount; i++) {
				Vector3 pos = CPathLinearRender.CurrentSelectedObj.transform.GetChild (i).position;
				pos = Camera.main.WorldToScreenPoint (pos);
				lineInfo.Add (pos);
			}
		}
		OnPostRender ();
	}
	//GL的绘制方法系统回调
	void OnPostRender()
	{
		if(!material)
		{
			Debug.LogError("material == null");
			return;
		}
		//材质通道，0为默认。
		material.SetPass(0);
		//绘制2D图像
		GL.LoadOrtho();
		//得到鼠标点信息总数量
		GL.Begin(GL.LINES);
		//遍历鼠标点的链表
		int size = lineInfo.Count;
		
		for(int i =0; i < size - 1;i++)
		{
			Vector3 start = lineInfo[i];
			Vector3 end = lineInfo[i+1];
			//绘制线
			DrawLine(start.x,start.y,end.x,end.y);
		}
		//结束绘制
		GL.End();
		
	}
	
	void DrawLine(float x1,float y1,float x2,float y2)
	{
		//绘制线段
		GL.Vertex(new Vector3(x1 / Screen.width,y1 / Screen.height,0));
		GL.Vertex(new Vector3(x2 / Screen.width, y2 / Screen.height,0));
	}

}
#endif