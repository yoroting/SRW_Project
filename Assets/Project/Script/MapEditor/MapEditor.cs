#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using MYGRIDS;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MapEditor : MonoBehaviour
{
    /// <summary>
    /// 改變Tile的方式
    /// </summary>
    public enum DrawMode
    {
        /// <summary>
        /// 當Tile被選擇時改變
        /// </summary>
        TileSelect,
        /// <summary>
        /// 當Thing被選擇時改變
        /// </summary>
        ThingSelect,
        /// <summary>
        /// 當點選改變按鍵時
        /// </summary>
        Button,
    }

    public static MapEditor Instance;

    public MyGrids Grids = new MyGrids();				// main grids . only one    // Use this for initialization
    public GameObject TilePlaneObj; // plane of all tiles sprite
    
    /// <summary>
    /// 要給編輯器讀取的場景名稱
    /// </summary>
    [HideInInspector]
    public string MapName;
    /// <summary>
    /// 場景編輯器改變Tile的方式
    /// </summary>
    [HideInInspector]
    public DrawMode Mode = DrawMode.Button;
    
    float fMinOffX;
    float fMaxOffX;
    float fMinOffY;
    float fMaxOffY;

    /// <summary>
    /// Const Data資料
    /// </summary>
    public static ConstDataManager ConstData
    {
        get
        {
            if (_constData == null)
            {
                ConstDataManager.Instance.NormalReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES);
                _constData = ConstDataManager.Instance;
            }

            return _constData;
        }
    }
    private static ConstDataManager _constData;
    void OnEnable()
    {
        Instance = this;

        //Init();
    }

    void OnDisable()
    {
        Instance = null;
    }

    //private void Init()
    //{
    //    if (_constData == null)
    //    {
    //        ConstDataManager.Instance.NormalReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES);
    //        _constData = ConstDataManager.Instance;
    //    }
    //}

    public bool LoadScene(int sceneID)
    {
        ClearScene();

        SCENE_NAME scn = ConstData.GetRow<SCENE_NAME>(sceneID);
        if (scn == null)
        {
            Debug.LogFormat("LoadScene fail with ID {0}", sceneID);
            return false;
        }

        LoadScene(scn.s_MODLE_ID);

        return true;
    }

    /// <summary>
    /// 建立一個新的場景
    /// </summary>
    /// <param name="sceneName">場景名稱</param>
    public void CreateNewScene(string sceneName, int halfWidth, int halfHeight)
    {
        ClearScene();

        MapName = sceneName;

        Grids = new MyGrids();
        Grids.hW = halfWidth;
        Grids.hH = halfHeight;

        CreateSprite();
    }

    /// <summary>
    /// 讀取場景
    /// </summary>
    /// <param name="sceneName">場景名稱</param>
    public void LoadScene(string sceneName)
    {
        ClearScene();

        MapName = sceneName;
        string path = "file://" + Application.dataPath + "/StreamingAssets/scn/" + sceneName + ".scn";

        WWW www = new WWW(path);
        while (!www.isDone)
        {
        }

        Debug.Log("load scn file on:" + path);

        if (Grids.Load(www.bytes) == true)
        {
            CreateSprite();
        }
    }

    public void ClearScene()
    {
        MyTool.DestoryImmediateAllChildren(TilePlaneObj);
    }

    private void CreateSprite()
    {
        // start to create sprite
        for (int i = -Grids.hW; i <= Grids.hW; i++)
        {
            for (int j = -Grids.hH; j <= Grids.hH; j++)
            {
                _TILE t = Grids.GetValue(i, j);

                GameObject cellGameObject = GetTileCellPrefab(i, j, t);
                if (cellGameObject == null)
                {
                    // debug message
                    string err = string.Format("Error: Create Tile Failed in Scene({0}),X({1},Y({2},T({3} )", "SRW000", i, j, t);
                    Debug.Log(err);
                    continue;
                }

                UnitCell cell = cellGameObject.GetComponent<UnitCell>();
                if (cell == null)
                    continue;

                List<MyThing> thingList = null;
                if (Grids.ThingPool.TryGetValue(cell.Loc.GetKey(), out thingList))
                {
                    if (thingList.Count <= 0)
                        continue;

                    foreach (MyThing thingData in thingList)
                    {
                        if (thingData.Cell == null)
                            continue;

                        GameObject thing = ResourcesManager.CreatePrefabGameObj(cellGameObject, "Prefab/Thing");
                        if (thing == null)
                        {
                            Debug.LogFormat("Create thing fail.(Key={0})", cell.Loc.GetKey());
                            continue;
                        }

                        UISprite thingSprite = thing.GetComponent<UISprite>();
                        if (thingSprite == null)
                        {
                            Debug.LogFormat("Thing Sprite is null.(Key={0})", cell.Loc.GetKey());
                            continue;
                        }

                        thing.name = thingData.Layer.ToString();

                        thingSprite.spriteName = MyGrids.GetThingSpriteName(thingData.Cell.Value);
                        thingSprite.depth = thingData.Layer;

                        NGUITools.SetDirty(thingSprite.gameObject);

                    }
                }
            }
        }
        // reget the drag limit 
        Resize();
    }

    GameObject GetTileCellPrefab(int x, int y, _TILE t)
    {
        string tileSpriteName = "";
        SCENE_TILE tile = ConstData.GetRow<SCENE_TILE>((int)t);
        if (tile != null)
        {
            tileSpriteName = tile.s_FILE_NAME;
        }

        GameObject cell = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/TileCell");
        UISprite sprite = cell.GetComponent<UISprite>();
        if (sprite != null)
        {
            sprite.spriteName = tileSpriteName;

        }
        UIDragObject drag = cell.GetComponent<UIDragObject>();
        if (drag != null)
        {
            drag.target = TilePlaneObj.transform;

        }

        if (cell != null)
        {
            SynGridToLocalPos(cell, x, y);

            cell.name = string.Format("Cell({0},{1},{2})", x, y, 0);

            UnitCell unit = cell.GetComponent<UnitCell>();
            if (unit != null)
            {
                unit.X(x);
                unit.Y(y);

            }
            //==========================================================
            UIEventListener.Get(cell).onClick += OnCellClick;
        }

        return cell;
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
                SCENE_TILE tile = ConstData.GetRow<SCENE_TILE>(tileValue);
                if (tile == null)
                    sprite.spriteName = "";
                else
                    sprite.spriteName = tile.s_FILE_NAME;

                NGUITools.SetDirty(sprite.gameObject);
            }
        }
    }

    public void AddThing(int thingValue, int layer)
    {
        foreach (GameObject item in Selection.gameObjects)
        {
            UnitCell cell = item.GetComponent<UnitCell>();
            if (cell == null)
            {
                // 有可能是點到地上物, 所以必須找看看parent
                cell = item.transform.parent.GetComponent<UnitCell>();
                if (cell == null)
                    continue;
            }

            Transform thingTransform = cell.transform.FindChild(layer.ToString());
            if (thingTransform == null)
            {
                GameObject thing = ResourcesManager.CreatePrefabGameObj(cell.gameObject, "Prefab/Thing");
                if (thing == null)
                {
                    Debug.LogFormat("Create thing error.(X={0}, Y={1})", cell.Loc.X, cell.Loc.Y);
                    return;
                }

                UISprite thingSprite = thing.GetComponent<UISprite>();
                if (thingSprite == null)
                {
                    Debug.LogFormat("Thing has no sprit component.");
                    return;
                }

                thing.name = layer.ToString();

                thingSprite.spriteName = "icemount";
                thingSprite.depth = layer;

                Grids.AddThing(cell.X(), cell.Y(), thingValue, layer);

                NGUITools.SetDirty(thingSprite.gameObject);
            }
            else
            {
                UISprite thingSprite = thingTransform.GetComponent<UISprite>();
                if (thingSprite == null)
                {
                    Debug.LogFormat("Thing has no UISprite");
                    return;
                }

                thingSprite.spriteName = "mount";

                Grids.ReplaceThing(cell.X(), cell.Y(), thingValue, layer);

                NGUITools.SetDirty(thingSprite.gameObject);
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
}
#endif