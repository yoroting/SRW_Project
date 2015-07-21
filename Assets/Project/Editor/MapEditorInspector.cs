using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(MapEditorTool))]
public class MapEditorInspector : Editor
{
    private MapEditorTool _targetScrpt;
    private string _sceneTextFieldString = "1";

    void OnEnable()
    {
        _targetScrpt = (MapEditorTool)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        _sceneTextFieldString = GUILayout.TextField(_sceneTextFieldString, GUILayout.Height(16));
        if (GUILayout.Button("Load Scene", GUILayout.Height(16)))
        {
            _targetScrpt.LoadScene(Convert.ToInt32(_sceneTextFieldString));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Scene", GUILayout.Height(16)))
        {
            _targetScrpt.ClearScene();
        }
        GUILayout.EndHorizontal();

        Repaint();
    }
}
