using UnityEngine;
using System.Collections;
using MYGRIDS;

[ExecuteInEditMode]
public class MapEditor : MonoBehaviour
{
    public cMyGrids Grids = new cMyGrids();				// main grids . only one    // Use this for initialization
    public GameObject TilePlaneObj; // plane of all tiles sprite
    float fMinOffX;
    float fMaxOffX;
    float fMinOffY;
    float fMaxOffY;
    /// <summary>
    /// 是否已經初始化
    /// </summary>
    private bool _init = false;

    void Start()
    {
    }

    void OnEnable()
    {
        if (!_init)
        {
            Init();
            _init = true;
        }
        //// load scene file
        //if (LoadScene(1) == false)
        //{
        //    Debug.LogFormat("stageloding:LoadScene fail with ID {0} ", 1);
        //    return;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        i++;
    }

    private void Init()
    {
        ConstDataManager.Instance.NormalReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES);
    }

    public bool LoadScene(int nScnid)
    {
        ConstDataManager.Instance.NormalReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES);

        string dataPathRelativeAssets = "scn/";
        string rootPath = null;

        rootPath = "file://" + Application.dataPath + "/StreamingAssets/" + dataPathRelativeAssets + "SRW000" + ".scn";

        // real to binary
        WWW www = new WWW(rootPath);
        //		if(endFunc != null){
        //			yield return www;
        //		}else{
        while (!www.isDone)
        {

        }
        //		}
        //		string txt = Application.dataPath;
        //		string txt2 = Application.persistentDataPath;
        //		string utext = txt + "/" +txt2;
        //		Debug.Log( "unity data path :"+utext );

        Debug.Log("load scn file on:" + rootPath);

        if (Grids.Load(www.bytes) == true)
        //if( Grids.Load( rootPath )==true )
        //if( Grids.Load( filename )==true )
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

        // change bgm '
        // all stage have start event for speicial bgm
        //GameSystem.PlayBGM ( scn.n_BGM );


        return true;
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
