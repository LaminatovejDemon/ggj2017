using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LevelEditorObject))]
public class LevelEditor : Editor
{
    bool Drawing = false;
    LevelEditorObject.SelectedTile LastTile;

    public override void OnInspectorGUI()
    {
        bool[] selectedTile;
        DrawDefaultInspector();

        Color defaultColor = GUI.contentColor;

        LevelEditorObject activeEditor = target as LevelEditorObject;

        if (GUILayout.Button("Find Tile Prefabs"))
        {
            activeEditor.FindTilePrefabs();
        }

        selectedTile = new bool[activeEditor.Definitions.Worlds.Length];

        for (int i = 0; i < activeEditor.Definitions.Worlds.Length; i++)
        {
            selectedTile[i] = GUILayout.Toggle(selectedTile[i], activeEditor.Definitions.Worlds[i].TextureSetName);
        }

        for (int i = 0; i < activeEditor.Definitions.Worlds.Length; i++)
        {
            GUILayout.Label("World - " + activeEditor.Definitions.Worlds[i].TextureSetName + " - Tiles Found: " + activeEditor.Definitions.Worlds[i].NumberOfFoundTiles);
        }

        if (GUILayout.Button("Erase World Map"))
        {
            activeEditor.EraseTilePrefabs(activeEditor.SelectedWorld);
        }

        GUI.contentColor = Color.green;
        GUILayout.Label("");
        GUILayout.Label("Controls");
        GUI.contentColor = defaultColor;
        GUILayout.Label("Placing Tiles - Left Mouse Button");
        GUILayout.Label("Erasing Tiles - Left Mouse Button + Shift");
    }

    void OnSceneGUI()
    {
        LevelEditorObject activeEditor = target as LevelEditorObject;
        LevelEditorObject.SelectedTile tile;

        if (activeEditor == null)
            return;

        Ray ray;

        Event e = Event.current;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        tile = activeEditor.GetTileInfo(ray.origin);

        if (!activeEditor.CheckCursorPosition(tile))
        {
            return;
        }

        //Clicks

        if (!activeEditor.DirectPaintMode)
        {
            Drawing = false;
        }

        if (e.isMouse &&  e.type == EventType.MouseDown && e.button == 0)
        {
            Drawing = true;
        }

        if (e.isMouse && e.type == EventType.MouseUp && e.button == 0)
        {
            Drawing = false;
            LastTile.origin = new Vector2Int(-1, -1);
        }

        if (LastTile.origin == tile.origin && LastTile.segment == tile.segment) //painting the same tile
        {
            return;
        }

        bool placingTile = true;
        if (e.shift)
        {
            placingTile = false;
        }

        if (e.button == 0 && Drawing && placingTile)
        {
            
            DrawSegment(activeEditor, tile, 1);          
        }
        else if (e.button == 0 && Drawing && !placingTile)
        {
            
            DrawSegment(activeEditor, tile, 0);
        }
        else if (e.button == 2 && Drawing)
        {
            Debug.Log("Middle Click");
        }

    }

    void DrawSegment(LevelEditorObject activeEditor, LevelEditorObject.SelectedTile tile, int variant)
    {
        activeEditor.PlaceSegment(tile,variant);
        activeEditor.ClearCheckedTiles(activeEditor.SelectedWorld);
        LastTile = tile;
    }
}

#endif
