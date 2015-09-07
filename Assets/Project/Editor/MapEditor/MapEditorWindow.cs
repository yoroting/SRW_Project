using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

/// <summary>
/// 場景編輯器工具視窗
/// </summary>
public class MapEditorWindow : EditorWindow
{
    /// <summary>
    /// MapEditor的component
    /// </summary>
    private MapEditor _mapEditor;
    /// <summary>
    /// 場景檔路徑位置
    /// </summary>
    private string _sceneFilePath = "";
    /// <summary>
    /// 背景圖片名稱
    /// </summary>
    private string _backgroundName = "";
    /// <summary>
    /// 背景圖路徑
    /// </summary>
    private string _backgroundPath = "";
    /// <summary>
    /// 地上物要種的layer
    /// </summary>
    private static string _thingLayerTextFieldString = "1";
    /// <summary>
    /// 選擇要種的Tile
    /// </summary>
    private static MYGRIDS._TILE _tile = MYGRIDS._TILE._GREEN;
    /// <summary>
    /// 選擇要種的Thing
    /// </summary>
    private static MYGRIDS._THING _thing = MYGRIDS._THING._NULL;

    [MenuItem("Custom/Map Editor Window")]
    public static void Init()
    {
        MapEditorWindow window = (MapEditorWindow)EditorWindow.GetWindow(typeof(MapEditorWindow));
        window.position = new Rect(512, 384, 400, 200);
        window.Show();

        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnGUI()
    {
        if (_mapEditor == null)
        {
            if (GameObject.Find("MapEditor") == null)
                return;

            _mapEditor = GameObject.Find("MapEditor").GetComponent<MapEditor>();
            if (_mapEditor == null)
                return;
        }

        #region 地圖名
        GUILayout.BeginHorizontal();
        GUILayout.Label("Map Name", GUILayout.Height(16));
        _mapEditor.MapName = GUILayout.TextArea(_mapEditor.MapName, GUILayout.Height(16));
        if (GUILayout.Button("Load Scene", GUILayout.Height(16)))
        {
            //_mapEdtor.LoadScene(_mapEdtor.MapName);
            _sceneFilePath = EditorUtility.OpenFilePanel(
                "Select Scene File",
                "Assets/StreamAssets/",
                "scn");
            if (_sceneFilePath.Length != 0)
            {
                _mapEditor.LoadScene(System.IO.Path.GetFileNameWithoutExtension(_sceneFilePath));
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        #region 背景圖
        GUILayout.BeginHorizontal();
        GUILayout.Label("Background", GUILayout.Height(16));
        GUILayout.Label(_backgroundName, GUILayout.Height(16));
        if (GUILayout.Button("Select", GUILayout.Height(16)))
        {
            _backgroundPath = EditorUtility.OpenFilePanel(
                "Select Background",
                "Assets/Project/Resources/Art/MAP",
                "png");
            if (_backgroundPath.Length != 0)
            {
                _backgroundName = System.IO.Path.GetFileNameWithoutExtension(_backgroundPath);
                _mapEditor.ChangeBackground("Art/MAP/" + _backgroundName);
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        #region 著色模式
        GUILayout.BeginHorizontal();
        MapEditor.Instance.Mode = (MapEditor.DrawMode)EditorGUILayout.EnumPopup("Draw Mode", MapEditor.Instance.Mode, GUILayout.Height(16));
        GUILayout.EndHorizontal();
        #endregion

        //GUILayout.Space(16);

        //#region 改變Tile Layer
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Layer", GUILayout.Height(16));
        //_tileLayer = GUILayout.TextArea(_tileLayer, GUILayout.Height(16));       
        //if (GUILayout.Button("Change Layer", GUILayout.Height(16)))
        //{
        //    //_mapEdtor.ChangeTileValue((int)_tile);
        //}
        //GUILayout.EndHorizontal();
        //#endregion

        GUILayout.Space(16);

        #region 設定Tile
        GUILayout.BeginHorizontal();
        _tile = (MYGRIDS._TILE)EditorGUILayout.EnumPopup("Tile", _tile, GUILayout.Height(16));
        if (GUILayout.Button("Change Tile Value", GUILayout.Height(16)))
        {
            _mapEditor.ChangeTileValue((int)_tile);
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        #region 改變地上物
        GUILayout.BeginHorizontal();
        _thing = (MYGRIDS._THING)EditorGUILayout.EnumPopup("Thing", _thing, GUILayout.Height(16));
        GUILayout.Label("Layer", GUILayout.Height(16));
        _thingLayerTextFieldString = GUILayout.TextField(_thingLayerTextFieldString, GUILayout.Height(16));
        if (GUILayout.Button("Change Thing Value", GUILayout.Height(16)))
        {
            _mapEditor.AddThing((int)_thing, Convert.ToInt32(_thingLayerTextFieldString));
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        #region 清空場景、存檔
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Scene", GUILayout.Height(16)))
        {
            CreateNewSceneWindow window = (CreateNewSceneWindow)EditorWindow.GetWindow(typeof(CreateNewSceneWindow));
            window.position = new Rect(512, 384, 200, 20);
            window.Show();
        }
        if (GUILayout.Button("Clear Scene", GUILayout.Height(16)))
        {
            _mapEditor.ClearScene();
        }
        if (GUILayout.Button("Save Scene", GUILayout.Height(16)))
        {
            string mapPath = Application.dataPath + "/StreamingAssets/scn/" + _mapEditor.MapName + ".scn";
            _mapEditor.SaveGrid(mapPath);
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        Repaint();
    }

    public static void OnSceneGUI(SceneView sceneView)
    {
        if (MapEditor.Instance != null && MapEditor.Instance.Mode == MapEditor.DrawMode.TileSelect)
        {
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {
                MapEditor.Instance.ChangeTileValue((int)_tile);
            }
        }
        else if (MapEditor.Instance != null && MapEditor.Instance.Mode == MapEditor.DrawMode.ThingSelect)
        {
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {
                MapEditor.Instance.AddThing((int)_thing, Convert.ToInt32(_thingLayerTextFieldString));
            }
        }
    }
}
