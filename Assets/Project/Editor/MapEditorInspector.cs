using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MapEditor))]
public class MapEditorInspector : Editor
{
    private MapEditor _targetScrpt;

    void OnEnable()
    {
        _targetScrpt = (MapEditor)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.TextField("1", GUILayout.Height(16));
        if (GUILayout.Button("Load Scene", GUILayout.Height(16)))
        {
            _targetScrpt.LoadScene(1);
        }
        GUILayout.EndHorizontal();

        Repaint();
    }
}
