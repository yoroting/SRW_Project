using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CreateNewSceneWindow : EditorWindow
{
    /// <summary>
    /// MapEditor的component
    /// </summary>
    private MapEditor _mapEditor;
    private string _mapNameTextArea = "scn";
    private string _halfWidthTextArea = "1";
    private string _halfHeighTextArea = "1";
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

        #region
        GUILayout.BeginHorizontal();

        GUILayout.Label("Map Name", GUILayout.Height(16));
        _mapNameTextArea = GUILayout.TextArea(_mapNameTextArea, GUILayout.Height(16));

        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        #region hW、hH
        GUILayout.BeginHorizontal();

        GUILayout.Label("hW", GUILayout.Height(16));
        _halfWidthTextArea = GUILayout.TextArea(_halfWidthTextArea, GUILayout.Height(16));

        GUILayout.Label("hH", GUILayout.Height(16));
        _halfHeighTextArea = GUILayout.TextArea(_halfHeighTextArea, GUILayout.Height(16));

        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(16);

        #region 確定、取消按鍵
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK", GUILayout.Height(16)))
        {
            _mapEditor.CreateNewScene(_mapNameTextArea, Convert.ToInt32(_halfWidthTextArea), Convert.ToInt32(_halfHeighTextArea));
        }
        GUILayout.EndHorizontal();
        #endregion

        Repaint();
    }
}
