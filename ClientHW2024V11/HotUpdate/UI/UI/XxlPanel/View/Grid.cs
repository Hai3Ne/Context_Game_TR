using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public enum CodeType
    {
        EMPTY,
        NORMAL,
        ROW,
    }

    [System.Serializable]
    public struct Codes
    {
        public CodeType type;
        public GameObject codePrefab;
    }

    public float fillTime = 0.1f;

    public int Grid_H = 0;

    public int Grid_V = 0;

    public GameObject bgCode;

    public Codes[] NomalCodes;

    public Dictionary<CodeType, GameObject> piecePrefabDict;

    public Sprite five;

    public Sprite fow;

    public Sprite lianxu;

    public GameObject camera;

    private CodePrefab[,] gridCodes;

    private CodePrefab StartCode;

    private CodePrefab EndCode;

    private SystemUI SystemUIComponent;

    private GameOver gameOverComponent;

    public SystemUI getSystemUI {
        get { return SystemUIComponent; }
    }

    private void Awake()
    {
        piecePrefabDict = new Dictionary<CodeType, GameObject>();

        SystemUIComponent = GetComponent<SystemUI>();

        gameOverComponent = GetComponent<GameOver>();

        gameOverComponent.initGameOver(0 , this);
        //初始化网格大小
        gridCodes = new CodePrefab[Grid_H , Grid_V];
        systemUI = GetComponent<SystemUI>();
        //初始化预制体对象
        for (int i = 0; i < NomalCodes.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(NomalCodes[i].type))
            {
                piecePrefabDict.Add(NomalCodes[i].type, NomalCodes[i].codePrefab);
            }
        }
    }

    void Start () {
        GridDraw();

        //创建空节点
        for (int i = 0; i < Grid_H; i++)
        {
            for (int j = 0; j < Grid_V; j++)
            {
                SpawnNewPiece(i, j, CodeType.EMPTY);
            }
        }

        StartCoroutine(fillAll());
    }

    //绘制网格
    private void GridDraw() {
        camera.transform.position = new Vector3(0, 0, -10);
        for (int i = 0; i < Grid_H; i++) {
            for (int j = 0; j < Grid_V; j++) {
                Vector3 pos = NumToVector(i, j);
                GameObject BGCode = Instantiate(bgCode, pos, Quaternion.identity);
                BGCode.transform.parent = transform;
            }
        }
    }

    public void clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var obj = transform.GetChild(i);
            Destroy(obj);
        }

        gridCodes = new CodePrefab[Grid_H, Grid_V];
        GridDraw();
    }

    //创建方块对象
    public void SpawnNewPiece(int x, int y , CodeType type) {

        GameObject code = Instantiate(piecePrefabDict[type], NumToVector(x , y ) , Quaternion.identity);

        code.transform.parent = transform;

        CodePrefab codePrefab = code.GetComponent<CodePrefab>();

        codePrefab.initPos(x ,y , this , type);

        gridCodes[x, y] = codePrefab;
    }

    //将数组转换为坐标
    public Vector2 NumToVector(int x, int y) {
        Vector2 v = new Vector2(x - (int)(Grid_H*0.5f + 0.5f) , (int)(Grid_V*0.5f - 1.5f) - y - 0.42f);
        return v;
    }

    //填充所有空节点
    public IEnumerator fillAll() {
        bool isClear = true;
        int count = 0;
        while (isClear) {
            yield return new WaitForSeconds(fillTime);
            while (!fillRow())
            {
                yield return new WaitForSeconds(fillTime);
            }
            isClear = clearAllCode();
            count++;
        }
        if (count >= 2) {
            SystemUIComponent.playScore("x" + count);
            gameOverComponent.addScore(count * 5);
        }
    }

    //行单位填充空节点
    private bool fillRow() {
        //用于判断是否全部填充
        bool isNeed = true;

        for (int i = 0; i < Grid_H; i++)
        {
            for (int y = 0; y < Grid_V; y++)
            {
                //判断当前结点是否为空节点
                if (gridCodes[i, y].getCodeType == CodeType.EMPTY)
                {
                    isNeed = false;

                    Destroy(gridCodes[i, y].gameObject);

                    GameObject code = Instantiate(piecePrefabDict[CodeType.NORMAL], NumToVector(i, 0), Quaternion.identity);

                    code.transform.parent = transform;
           
                    CodePrefab codePrefab = code.GetComponent<CodePrefab>();

                    codePrefab.initPos(i, 0, this, CodeType.NORMAL);

                    codePrefab.getColorComponent.setRangeSprite();
         
                    for (int c = y-1; c >= 0; c--)
                    {
                        gridCodes[i, c].getMoveComponent.MoveToPos(i, c + 1, fillTime);

                        gridCodes[i, c + 1] = gridCodes[i, c];
                    }
             
                    codePrefab.getMoveComponent.MoveToPos(i, 0, fillTime);

                    gridCodes[i, 0] = codePrefab;

                    break;
                }
            }
        }

        return isNeed;
    }

    //清除所有满足消除条件的的方块
    public bool clearAllCode() {
        bool isClear = false;
        for (int x = 0; x < Grid_H; x ++) {
            for (int y = 0; y < Grid_V; y++)
            {
                if (gridCodes[x, y].getCodeType != CodeType.EMPTY) {
                    List<CodePrefab> cle = getMatch(gridCodes[x , y] , x , y);
                    if (cle.Count > 0) {
                        isClear = true;
                        scoreManager(cle.Count);
                        foreach (CodePrefab c in cle) {
                            SpawnNewPiece(c.getX, c.getY, CodeType.EMPTY);
                            c.ClearCode();
                        }
                    }
                }
            }
        }

        return isClear;
    }

    //匹配当前指定对象是否有消除的对象
    public List<CodePrefab> getMatch(CodePrefab code , int x , int y) {
        List<CodePrefab> codeMatch = new List<CodePrefab>();
        List<CodePrefab> horizontalCode = new List<CodePrefab>();
        List<CodePrefab> verticalCode = new List<CodePrefab>();
        bool isH = false;
        bool isV = false;

        if (code.getCodeType == CodeType.EMPTY) {
            return codeMatch;
        }

        //右遍历
        for (int i = x + 1; i < Grid_H; i++) {
            if (code.getX == i && code.getY == y) {
                break;
            }
            if (gridCodes[i, y].getCodeType != CodeType.EMPTY &&  gridCodes[i , y].getColorComponent.getColor == code.getColorComponent.getColor)
            {
                horizontalCode.Add(gridCodes[i, y]);
            }
            else {
                break;
            }
        }

        //左遍历
        for (int i = x - 1; i >= 0; i--)
        {
            if (code.getX == i && code.getY == y)
            {
                break;
            }
            if (gridCodes[i, y].getCodeType != CodeType.EMPTY &&  gridCodes[i, y].getColorComponent.getColor == code.getColorComponent.getColor)
            {
                horizontalCode.Add(gridCodes[i, y]);
            }
            else
            {
                break;
            }
        }

        //下遍历
        for (int i = y + 1; i < Grid_V; i++)
        {
            if (code.getX == x && code.getY == i)
            {
                break;
            }
            if (gridCodes[x , i].getCodeType != CodeType.EMPTY && gridCodes[x, i].getColorComponent.getColor == code.getColorComponent.getColor)
            {
                verticalCode.Add(gridCodes[x, i]);
            }
            else
            {
                break;
            }
        }
        //上遍历
        for (int i = y - 1; i >= 0; i--)
        {
            if (code.getX == x && code.getY == i)
            {
                break;
            }
            if (gridCodes[x, i].getCodeType != CodeType.EMPTY && gridCodes[x, i].getColorComponent.getColor == code.getColorComponent.getColor)
            {
                verticalCode.Add(gridCodes[x , i]);
            }
            else
            {
                break;
            }
        }

        if (horizontalCode.Count >= 2) {
            isH = true;
            foreach (CodePrefab c in horizontalCode) {
                //Debug.Log("横轴  颜色 : " + c.getColorComponent.getColor + "坐标: " + c.getX + "," + c.getY);
                codeMatch.Add(c);
            }
        }

        if (verticalCode.Count >= 2) {
            isV = true;
            foreach (CodePrefab c in verticalCode)
            {
                //Debug.Log("纵轴   颜色 : " + c.getColorComponent.getColor + "坐标: " + c.getX + "," + c.getY);
                codeMatch.Add(c);
            }
        }

        if (isH || isV) {
            //Debug.Log("颜色 : " + code.getColorComponent.getColor + "坐标: " + x + "," + y);

            codeMatch.Add(code);
            
        }

        return codeMatch;
    }

    public void DownCode(CodePrefab sCode) {
        StartCode = sCode;
    }

    public void EnterCode(CodePrefab eCode) {
        EndCode = eCode;
    }
    private SystemUI systemUI;
    //滑动手势判断
    public void UpCode() {
        if (systemUI.IsGame && !gameOverComponent.isGameOver) {
            if (StartCode != null && EndCode != null)
            {
                int X = EndCode.getX - StartCode.getX;
                int Y = EndCode.getY - StartCode.getY;

                //横轴滑动
                if (Mathf.Abs(X) > Mathf.Abs(Y))
                {
                    //右滑
                    if (X > 0)
                    {
                        Debug.Log("向右滑动");
                        EndCode = gridCodes[StartCode.getX + 1, StartCode.getY];
                    }
                    //左滑
                    else if (X < 0)
                    {
                        Debug.Log("向左滑动");
                        EndCode = gridCodes[StartCode.getX - 1, StartCode.getY];
                    }
                }
                //纵轴滑动
                else if (Mathf.Abs(X) < Mathf.Abs(Y))
                {
                    if (Y > 0)
                    {
                        Debug.Log("向下滑动");
                        EndCode = gridCodes[StartCode.getX, StartCode.getY + 1];
                    }
                    else if (Y < 0)
                    {
                        Debug.Log("向上滑动");
                        EndCode = gridCodes[StartCode.getX, StartCode.getY - 1];
                    }
                }
                else
                {
                    EndCode = null;
                }
                StartCoroutine(MoveCode());

                StartCode = null;
                EndCode = null;
            }
        }

    }

    private IEnumerator MoveCode()
    {
        if (StartCode != null && EndCode != null)
        {
            List<CodePrefab> startCodeS = getMatch(StartCode, EndCode.getX, EndCode.getY);
            List<CodePrefab> endCodeS = getMatch(EndCode, StartCode.getX, StartCode.getY);
            if (startCodeS.Count > 0 || endCodeS.Count > 0)
            {
                int StartX = EndCode.getX;
                int StartY = EndCode.getY;
                int EndX = StartCode.getX;
                int EndY = StartCode.getY;
                //互换数组中的位置 
                gridCodes[StartX, StartY] = StartCode;
                gridCodes[EndX, EndY] = EndCode;

                //交换位置
                StartCode.getMoveComponent.MoveToPos(StartX, StartY, fillTime);
                EndCode.getMoveComponent.MoveToPos(EndX, EndY, fillTime);
                gameOverComponent.addTime(1);
                yield return new WaitForSeconds(fillTime);

                scoreManager(startCodeS.Count);
                scoreManager(endCodeS.Count);

                foreach (CodePrefab endC in endCodeS)
                {
                    SpawnNewPiece(endC.getX, endC.getY, CodeType.EMPTY);
                    endC.ClearCode();
                }
                foreach (CodePrefab startC in startCodeS)
                {
                    SpawnNewPiece(startC.getX, startC.getY, CodeType.EMPTY);
                    startC.ClearCode();
                }

                yield return new WaitForSeconds(fillTime * 2);

                //填充空的节点
                StartCoroutine(fillAll());
            }
            else
            {
                gameOverComponent.addTime(1);
                StartCode.getMoveComponent.MoveBackToPos(EndCode.getX, EndCode.getY, fillTime);
                EndCode.getMoveComponent.MoveBackToPos(StartCode.getX, StartCode.getY, fillTime);
            }

        }
    }
    //分数增加
    public void scoreManager(int CodeCount)
    {
        CoreEntry.gAudioMgr.PlayUISound(302);

        if (CodeCount == 4)
        {
            gameOverComponent.addScore(8);
 
            SystemUIComponent.playScore("x4");

        }
        else if (CodeCount == 5)
        {
            gameOverComponent.addScore(15);
   
            SystemUIComponent.playScore("x5");
        }
        else if (CodeCount > 5)
        {
            gameOverComponent.addScore((int)(CodeCount*2.5f));

        }
        else {
            gameOverComponent.addScore(3);
        }
    }
}
