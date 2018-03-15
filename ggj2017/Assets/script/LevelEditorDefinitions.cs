using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class LevelEditorDefinitions : MonoBehaviour {
    [System.Serializable]
    public struct TileObject
    {
        public List<LevelEditorTile> TileVariants;
    }
    [System.Serializable]
    public struct World
    {
        [HideInInspector] public TileObject[] SortedTileObjectsByID;
        [HideInInspector] public int NumberOfFoundTiles;
        [HideInInspector] public bool Valid;
        public string TextureSetName;
        public int TerrainVariants;
        public int TileSize;
    }
    public World[] Worlds;

    int GetWorldID(string name)
    {
        int id = 0;
        for (int i = 0; i < Worlds.Length; i++)
        {
            if (Worlds[i].TextureSetName == name)
            {
                id = i;
            }
        }
        return id;
    }

    void Start()
    {
        FindTilePrefabs();
    }

    void OnValidate()
    {
        FindTilePrefabs();
    }

    public void FindTilePrefabs()
    {
        for (int i = 0; i < Worlds.Length; i++)
        {
            if (Worlds[i].TerrainVariants > 0)
            {
                Worlds[i].SortedTileObjectsByID = new TileObject[Worlds[i].TerrainVariants * 1111 +1];
                for (int j = 0; j < Worlds[i].SortedTileObjectsByID.Length; j++)
                {
                    Worlds[i].SortedTileObjectsByID[j].TileVariants = new List<LevelEditorTile>();
                }
                
            }
        }

        
        LevelEditorTile[] allTiles = Resources.FindObjectsOfTypeAll(typeof(LevelEditorTile)) as LevelEditorTile[];

        LevelEditorObject ActiveLevelEditor = this.GetComponent<LevelEditorObject>();

        ActiveLevelEditor.Map = new LevelEditorObject.TileObject[Worlds.Length, ActiveLevelEditor.MapSize.x, ActiveLevelEditor.MapSize.y];

        for (int w = 0; w < Worlds.Length; w++)
        {
            ActiveLevelEditor.InitMapData(w);
        }

        for (int i = 0; i < allTiles.Length; i++)
        {
            int worldID = GetWorldID(allTiles[i].TextureSetName);
            if (EditorUtility.IsPersistent(allTiles[i].transform.root.gameObject)) // unused resource prefab
            {                
                if (Worlds[worldID].TerrainVariants > 0 && allTiles[i].UsedQuadrants > 0)
                {                    
                    Worlds[worldID].SortedTileObjectsByID[allTiles[i].UsedQuadrants].TileVariants.Add(allTiles[i]);
                }
            }
            else // used in the scene
            {
                int x = (Mathf.RoundToInt(allTiles[i].transform.position.x / Worlds[worldID].TileSize) + ActiveLevelEditor.MapTileOffset.x);
                int y = -(Mathf.RoundToInt(allTiles[i].transform.position.y / Worlds[worldID].TileSize) - ActiveLevelEditor.MapTileOffset.y);

                ActiveLevelEditor.Map[worldID, x, y].Instance = allTiles[i];
                string formatedSegmentId = string.Format("{0:0000}", allTiles[i].UsedQuadrants);
                for (int s = 0; s < 4; s++)
                {
                    ActiveLevelEditor.Map[worldID, x, y].OccupiedSegments[s] = formatedSegmentId[s] - '0';
                }
            }
        }

        
        for (int i = 0; i < Worlds.Length; i++)
        {
            Worlds[i].NumberOfFoundTiles = 0;
            for (int j = 0; j < Worlds[i].SortedTileObjectsByID.Length; j++)
            {
                Worlds[i].NumberOfFoundTiles += Worlds[i].SortedTileObjectsByID[j].TileVariants.Count;
            }
            //Debug.Log("Level Editor Tiles in World - " + Worlds[i].TextureSetName + "(" + i + ")" + " - Found: " + tilesFound);
        }
        
    }
}

#endif