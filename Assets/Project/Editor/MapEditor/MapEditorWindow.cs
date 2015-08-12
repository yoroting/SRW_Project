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
    private MapEditor _mapEdtor;
    private string _sceneFilePath = "";
    /// <summary>
    /// 場景編號的字串
    /// </summary>
    private string _sceneTextFieldString = "1";
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
        if (_mapEdtor == null)
        {
            if (GameObject.Find("MapEditor") == null)
                return;

            _mapEdtor = GameObject.Find("MapEditor").GetComponent<MapEditor>();
        }

        #region 地圖名
        GUILayout.BeginHorizontal();
        GUILayout.Label("Map Name", GUILayout.Height(16));
        _mapEdtor.MapName = GUILayout.TextArea(_mapEdtor.MapName, GUILayout.Height(16));
        if (GUILayout.Button("Load Scene", GUILayout.Height(16)))
        {
            //_mapEdtor.LoadScene(_mapEdtor.MapName);
            _sceneFilePath = EditorUtility.OpenFilePanel(
                "Select Scene File",
                "./Assets/StreamAssets/",
                "scn");
            if (_sceneFilePath.Length != 0)
            {
                _mapEdtor.LoadScene(System.IO.Path.GetFileNameWithoutExtension(_sceneFilePath));
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        //#region 讀取場景
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Scene ID", GUILayout.Height(16));
        //_sceneTextFieldString = GUILayout.TextField(_sceneTextFieldString, GUILayout.Height(16));
        //if (GUILayout.Button("Load Scene By ID", GUILayout.Height(16)))
        //{
        //    _mapEdtor.LoadScene(Convert.ToInt32(_sceneTextFieldString));
        //}
        //GUILayout.EndHorizontal(); 
        //#endregion

        //GUILayout.Space(16);

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
            _mapEdtor.ChangeTileValue((int)_tile);
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
            _mapEdtor.AddThing((int)_thing, Convert.ToInt32(_thingLayerTextFieldString));
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        #region 清空場景、存檔
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Scene", GUILayout.Height(16)))
        {
            _mapEdtor.ClearScene();
        }
        if (GUILayout.Button("Save Scene", GUILayout.Height(16)))
        {
            string mapPath = Application.dataPath + "/StreamingAssets/scn/" + _mapEdtor.MapName + ".scn";
            _mapEdtor.SaveGrid(mapPath);
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
