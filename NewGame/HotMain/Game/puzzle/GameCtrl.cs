using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCtrl : MonoBehaviour
{
    public Button botonPlay;
    public Button BackPlay;
    public GameObject StartNode;
    public GameObject LevelNode;
    public GameObject GameNode;
    public GameObject GamePreView;

    public Button BackBtn;
    public Button TipsBtn;
    public GameObject GameBg;

    public ScrollRect ScrollNode;
    public GameObject panel;
    public Transform ScrollViewContent;

    public GameObject[] prefabsPuzzle;
    //数字资源整合
    [Header("SFX")]
    public AudioSource clickSFX;
    public AudioSource completeSFX;
    public AudioSource pauseSFX;
    public AudioSource barajarSFX;
    public AudioSource fixFondoSFX;
    public AudioSource fixGrupoSFX;
    public AudioSource errorSFX;
    public AudioSource desplegarSFX;

    public Sprite[] spriteList;
    Sprite spritePreseleccionado;
    public SpriteRenderer bgAyuda;
    private float gameTime;
    // Start is called before the first frame update
    void Start()
    {
        botonPlay.onClick.AddListener(CargarMasPuzzles);
        BackPlay.onClick.AddListener(levelBtn);
        BackBtn.onClick.AddListener(BackBtnEvent);
        TipsBtn.onClick.AddListener(TipsEvent);
        var btn = GamePreView.GetComponent<Button>();
        btn.onClick.AddListener(GamePreViewEvent);
    }


    public void TipsEvent()
    {
        GamePreView.SetActive(true);
    }
    public void GamePreViewEvent()
    {
        GamePreView.SetActive(false);
    }
    public void BackBtnEvent()
    {
        GameBg.SetActive(false);
        if (newPuzzle)
        {
            newPuzzle.SetActive(false);
            Destroy(newPuzzle);
            newPuzzle = null;
        }
        CargarMasPuzzles();
    }
    public void levelBtn()
    {
        clickSFX.Play();
        this.StartNode.SetActive(true);
        this.LevelNode.SetActive(false);
        GameNode.SetActive(false);
    }
    //触发关卡选择功能(主界面PLAY响应事件)
    public void CargarMasPuzzles()
    {
        //始放关卡加载声音
        clickSFX.Play();
        //显示自定义图品图片按钮
        this.StartNode.SetActive(false);
        this.LevelNode.SetActive(true);
        GameNode.SetActive(false);
        this.loadList();

    }

    public int level = 1;
    private bool ayudaActivada;
    private GameObject newPuzzle;
    public int puzzlePreseleccionado;
    public Image miniaturaDeAyuda;

    public void loadList()
    {
        if (PlayerPrefs.HasKey("LEVEL"))
        {
            level = PlayerPrefs.GetInt("LEVEL");
        }
        else
        {
            level = 1;
        }
   
        if (ScrollViewContent.childCount > 0)
        {
            for (int i = ScrollViewContent.childCount - 1; i >= 0; i--)
            {
                var obj = ScrollViewContent.GetChild(i);
                obj.gameObject.SetActive(false);
                Destroy(obj.gameObject);
            }
        }

        for (int i = 0; i < level + 1; i++)
        {
            var obj = Instantiate(panel.gameObject);
            obj.SetActive(true);
            loadItem(obj,i);
            obj.transform.SetParent(ScrollViewContent);
        }
        var num = level + 1;
        var rect = ScrollViewContent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, num * 408 + level*40 + 10);

    }
    public void loadItem(GameObject obj,int level)
    {
        var img = obj.transform.Find("rectMask/Image").GetComponent<Image>();
        var index = level % 5;
        img.sprite = spriteList[index];
        img.gameObject.SetActive(true);
        var btn = obj.transform.Find("Button").GetComponent<Button>();
        btn.onClick.AddListener(delegate
        {
            gotoLevel(level + 1);
        });
    }
    public void gotoLevel(int level)
    {
        LevelNode.gameObject.SetActive(false);
        GameNode.gameObject.SetActive(true);
        Vector3 posicionPuzzle = Vector3.zero;
        var index = level / 5;
        var type = index % 5;
        switch (type)
        {
            case 0:
                posicionPuzzle = new Vector3(0.2f, 0f, 0);
                break;
            case 1:
                posicionPuzzle = new Vector3(0.2f, 0f, 0);
                break;
            case 2:
                posicionPuzzle = Vector3.zero;
                break;
            case 3:
                posicionPuzzle = Vector3.zero;
                break;
            case 4:
                posicionPuzzle = Vector3.zero;
                break;
        }
        puzzlePreseleccionado = type;
        spritePreseleccionado = spriteList[(level - 1) % 5];
        GameObject newPuzzle = Instantiate(prefabsPuzzle[type], posicionPuzzle, prefabsPuzzle[type].transform.rotation) as GameObject;
        newPuzzle.name = prefabsPuzzle[type].name; //我们从名称中删除“（克隆）”
        this.newPuzzle = newPuzzle;
        LoadTexture(level);
        miniaturaDeAyuda.sprite = spritePreseleccionado;

        GameBg.SetActive(true);

        StartCoroutine(waitImg());
 

    }
    IEnumerator waitImg()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        MovePieces moverPiezas = this.gameObject.GetComponent<MovePieces>();
        moverPiezas.assemblePieces = GameObject.FindGameObjectWithTag("MatrizPuzzle").GetComponent<AssemblePieces>();
        DesactivarAyudaBG();
        DesordenarPiezas(newPuzzle.transform); //随机碎片

        gameTime = Time.time;
        yield return new WaitForSecondsRealtime(0.5f);
        moverPiezas.puzzlePlaying = true;
        yield break;
    }

    void LoadTexture(int level)
    {
        GameObject[] piezasPuzzle = GameObject.FindGameObjectsWithTag("PiezaPuzzle");

        for (int i = 0; i < piezasPuzzle.Length; i++)
        {
            piezasPuzzle[i].GetComponent<Renderer>().material.mainTexture = spriteList[(level - 1)%5].texture;
        }

    }

    void DesordenarPiezas(Transform newPuzzle)
    { //随机碎片
        this.gameObject.GetComponent<MovePieces>().positionZ = 0.0f;
        List<int> listaprofoundes = new List<int>();
        int profound = 0;
        foreach (Transform son in newPuzzle)
        {
            profound = Random.Range(-newPuzzle.childCount, 0);
            while (listaprofoundes.Contains(profound))
            {
                profound++;
                if (profound > 0)
                {
                    profound = -newPuzzle.childCount;
                }
            }
            listaprofoundes.Add(profound);
            StartCoroutine(Lerpson(son, profound * 0.001f, 2.25f));
        }
        barajarSFX.Play();
    }
    IEnumerator Lerpson(Transform piece, float profound, float pieceVelocity)
    {
        Vector3 posInicial = piece.position;
        Vector3 posFinal = new Vector3(Random.Range(-2.6f, 2.6f), Random.Range(5.2f, 5.5f), profound);
        float t = 0;
        while (t < 0.5f)
        {
            t += pieceVelocity * Time.deltaTime;
            piece.position = Vector3.Lerp(posInicial, posFinal, t * 2);
            yield return null;
        }
        yield return null;
    }


    void DesactivarAyudaBG()
    {
        ayudaActivada = false;
        bgAyuda.sprite = null;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowEnd()
    {
        Debug.Log("END");
    }
}
