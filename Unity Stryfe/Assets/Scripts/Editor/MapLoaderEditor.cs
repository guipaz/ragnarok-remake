using UnityEngine;
using System.Collections;
using UnityEditor;
using MapLoader;

[CustomEditor(typeof(MapLoader.MapManager))]
public class TerrainCreatorEditor : Editor {

    int width = 5;
    int height = 5;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Draw"))
        {
            MapLoader.MapManager.Instance.BuildMap();
        }
    }
}
