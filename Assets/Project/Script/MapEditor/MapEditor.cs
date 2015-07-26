using UnityEngine;
using System.Collections;
using MYGRIDS;
using UnityEditor;

[ExecuteInEditMode]
public class MapEditor : MonoBehaviour
{
    public static MapEditor Instance;

    private static MapEditor _instance;
    public MyGrids Grids = new MyGrids();				// main grids . only one    // Use this for initialization
    public GameObject TilePlaneObj; // plane of all tiles sprite
    [HideInInspector]
    public string MapName;
    float fMinOffX;
    float fMaxOffX;
    float fMinOffY;
    float fMaxOffY;

    /// <summary>
    /// Const Data資料
    /// </summary>
    private static ConstDataManager _constData;
    void OnEnable()
    {
        Instance = this;

        Init();
    }

    void OnDisable()
    {
        Instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        if (_constData == null)
        {
            ConstDataManager.Instance.NormalReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES);
            _constData = ConstDataManager.Instance;
        }
    }

    public bool LoadScene(int sceneID)
    {
        Init();

        ClearScene();

        SCENE_NAME scn = _constData.GetRow<SCENE_NAME>(sceneID);
        if (scn == null)
        {
            Debug.LogFormat("LoadScene fail with ID {0}", sceneID);
            return false;
        }

        string rootPath = "file://" + Application.dataPath + "/StreamingAssets/scn/" + scn.s_MODLE_ID + ".scn";
        WWW www = new WWW(rootPath);
        while (!www.isDone)
        {
        }

        MapName = scn.s_MODLE_ID;
        Debug.Log("load scn file on:" + rootPath);

        if (Grids.Load(www.bytes) == true)
        {
            // start to create sprite
            for (int i = -Grids.hW; i <= Grids.hW; i++)
            {
                for (int j = -Grids.hH; j <= Grids.hH; j++)
                {
                    _TILE t = Grids.GetValue(i, j);

                    GameObject cell = GetTileCellPrefab(i, j, t);
                    if (cell == null)
                    {
                        // debug message
                        string err = string.Format("Error: Create Tile Failed in Scene({0}),X({1},Y({2},T({3} )", "SRW000", i, j, t);
                        Debug.Log(err);

                    }
                }
            }
            // reget the drag limit 
            Resize();
        }

        return true;
    }

    public void ClearScene()
    {
        MyTool.DestoryImmediateAllChildren(TilePlaneObj);
    }

    GameObject GetTileCellPrefab(int x, int y, _TILE t)
    {
        SCENE_TILE tile = ConstDataManager.Instance.GetRow<SCENE_TILE>((int)t);
        if (tile != null)
        {
            //tile.s_FILE_NAME;
            GameObject cell = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/TileCell");
            UISprite sprite = cell.GetComponent<UISprite>();
            if (sprite != null)
            {
                sprite.spriteName = tile.s_FILE_NAME;

            }
            UIDragObject drag = cell.GetComponent<UIDragObject>();
            if (drag != null)
            {
                drag.target = TilePlaneObj.transform;

            }

            // tranform
            //			float locx =0, locy =0;
            //			Grids.GetRealXY(ref locx , ref locy , new iVec2( x , y ) );			
            //			Vector3 pos = new Vector3( locx , locy , 0 );
            if (cell != null)
            {
                SynGridToLocalPos(cell, x, y);
                //cell.transform.localPosition = pos; 

                cell.name = string.Format("Cell({0},{1},{2})", x, y, 0);

                UnitCell unit = cell.GetComponent<UnitCell>();
                if (unit != null)
                {
                    unit.X(x);
                    unit.Y(y);

                }
                //==========================================================
                UIEventListener.Get(cell).onClick += OnCellClick;
                //	UIEventListener.Get(cell).onPress += OnCellPress;

            }


            return cell;
        }
        //_TILE._GREEN

        return null;
    }

    public void Resize()
    {
        Grids.SetPixelWH(Config.TileW, Config.TileH);  // re size

        fMaxOffX = (Grids.TotalW - Screen.width) / 2;
        fMinOffX = -1 * fMaxOffX;

        fMaxOffY = (Grids.TotalH - Screen.height) / 2;
        fMinOffY = -1 * fMaxOffY;

    }

    public void SynGridToLocalPos(GameObject obj, int nx, int ny)
    {
        Vector3 v = obj.transform.localPosition;
        v.x = Grids.GetRealX(nx);
        v.y = Grids.GetRealY(ny);
        obj.transform.localPosition = v;
    }

    public void ChangeTileValue(int tileValue)
    {
        foreach (GameObject item in Selection.gameObjects)
        {
            UnitCell cell = item.GetComponent<UnitCell>();
            if (cell == null)
                continue;

            Grids.SetValue(cell.X(), cell.Y(), (MYGRIDS._TILE)tileValue);

            UISprite sprite = cell.GetComponent<UISprite>();
            if (sprite != null)
            {
                SCENE_TILE tile = _constData.GetRow<SCENE_TILE>(tileValue);
                if (tile == null)
                    sprite.spriteName = "";
                else
                    sprite.spriteName = tile.s_FILE_NAME;

                NGUITools.SetDirty(sprite.gameObject);
            }
        }
    }

    public void SaveGrid(string path)
    {
        Grids.Save(path);
    }

    void OnCellClick(GameObject go)
    {
    }

    void OnCellEvent(iVec2 Loc)
    {
        if (Loc == null)
            return;

        Debug.LogFormat("Loc={0}, {1}", Loc.X, Loc.Y);
    }

}
