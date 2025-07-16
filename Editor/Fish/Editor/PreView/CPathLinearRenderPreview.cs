using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CPathLinearRender))]
public class CPathLinearRenderPreview : Editor {
	public override bool HasPreviewGUI ()
	{
		return true;
	}
	public override void OnPreviewSettings ()
	{
		GUILayout.Label ("文本", "preLabel");
	}

	public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        InitPreview();
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        m_PreviewUtility.BeginPreview(r, background);
        Camera camera = m_PreviewUtility.camera;
		camera.transform.position = Camera.main.transform.position;
        camera.Render();
        m_PreviewUtility.EndAndDrawPreview(r);
    }

    private PreviewRenderUtility m_PreviewUtility;
    private GameObject m_PreviewInstance;

    private void InitPreview()
    {
        if (m_PreviewUtility == null)
        {
            m_PreviewUtility = new PreviewRenderUtility(true);
			m_PreviewUtility.camera.CopyFrom (Camera.main);
        }
    }

    private void DestroyPreview()
    {
        if (m_PreviewUtility != null)
        {
            // 务必要进行清理，才不会残留生成的摄像机对象等
            m_PreviewUtility.Cleanup();
            m_PreviewUtility = null;
        }
    }


    void OnDestroy()
    {
        DestroyPreview();
    }
}
