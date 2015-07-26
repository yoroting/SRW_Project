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
    /// <summary>
    /// 場景編號的字串
    /// </summary>
    private string _sceneTextFieldString = "1";
    /// <summary>
    /// 選擇的Tile
    /// </summary>
    private MYGRIDS._TILE _tile = MYGRIDS._TILE._GREEN;

    [MenuItem("Custom/Map Editor Window")]
    static void Init()
    {
        MapEditorWindow window = (MapEditorWindow)EditorWindow.GetWindow(typeof(MapEditorWindow));
        window.Show();
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
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        #region 讀取場景
        GUILayout.BeginHorizontal();
        _sceneTextFieldString = GUILayout.TextField(_sceneTextFieldString, GUILayout.Height(16));
        if (GUILayout.Button("Load Scene", GUILayout.Height(16)))
        {
            _mapEdtor.LoadScene(Convert.ToInt32(_sceneTextFieldString));
        }
        GUILayout.EndHorizontal(); 
        #endregion

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
}
