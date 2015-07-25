using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class MapEditorWindow : EditorWindow
{
    private MapEditor _mapEdtor;
    private string _sceneTextFieldString = "1";
    private string _tileValueText = "1";

    [MenuItem("Custom/Map Editor Window")]
    static void Init()
    {
        MapEditorWindow window = (MapEditorWindow)EditorWindow.GetWindow(typeof(MapEditorWindow));
        window.Show();
    }

    void OnGUI()
    {
        if (_mapEdtor == null)
            _mapEdtor = GameObject.Find("MapEditor").GetComponent<MapEditor>();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Map Name", GUILayout.Height(16));
        _mapEdtor.MapName = GUILayout.TextArea(_mapEdtor.MapName, GUILayout.Height(16));
        GUILayout.EndHorizontal();

        GUILayout.Space(16);


        GUILayout.BeginHorizontal();
        _sceneTextFieldString = GUILayout.TextField(_sceneTextFieldString, GUILayout.Height(16));
        if (GUILayout.Button("Load Scene", GUILayout.Height(16)))
        {
            _mapEdtor.LoadScene(Convert.ToInt32(_sceneTextFieldString));
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(16);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Layer", GUILayout.Height(16));
        _tileValueText = GUILayout.TextArea(_tileValueText, GUILayout.Height(16));
        if (GUILayout.Button("Change Tile Value", GUILayout.Height(16)))
        {
            OnChangeTileClick();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(16);

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

        GUILayout.Space(16);

        Repaint();
    }

    private void OnChangeTileClick()
    {
        _mapEdtor.ChangeTileValue(Convert.ToInt32(_tileValueText));
    }
}
