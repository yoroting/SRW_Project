using UnityEngine;
using System.Collections;

public class MapEditorTool : MonoBehaviour
{
    public void LoadScene(int sceneID)
    {
        MapEditor mapEditor = GetComponent<MapEditor>();
        if (mapEditor == null)
            return;

        mapEditor.LoadScene(sceneID);
    }

    public void ClearScene()
    {
        MapEditor mapEditor = GetComponent<MapEditor>();
        if (mapEditor == null)
            return;

        mapEditor.ClearScene();
    }
}
