using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class LevelEditorObject : MonoBehaviour {

    public struct TileObject
    {
        public LevelEditorTile Instance;
        public bool Checked;
        public int[] OccupiedSegments;
    }

    [HideInInspector] public TileObject[,,] Map;

    [HideInInspector] public GameObject[] ParentDirectories;

    public struct SelectedTile
    {
        public Vector2Int origin;
        public int segment;
    }

    LevelEditorCursor CursorObject;

    [HideInInspector] public LevelEditorDefinitions Definitions;
    [HideInInspector] public int SelectedWorld = 0;

    public Vector2Int MapSize;

    public bool DirectPaintMode = false;

    public struct AdjacentSegmentDef
    {
        public Vector2Int TileShift;
        public int Segment;
    }

    public struct AdjacentSegmentsDef
    {
        public AdjacentSegmentDef[] Adjacent;
    }

    AdjacentSegmentsDef[] SegmentsToCheck;

    [HideInInspector] public Vector2Int MapTileOffset;

    int actualVariant = 0;

    void CheckCursor()
    {
        if (CursorObject != null)
        {
            return;
        }

        LevelEditorCursor[] newcursor = GetComponentsInChildren<LevelEditorCursor>();
        if (newcursor.Length > 0)
        {
            CursorObject = newcursor[0];
        }
        else
        {
            CursorObject = Instantiate(Resources.Load("EditorSystem/LevelEditorCursor", typeof(LevelEditorCursor))) as LevelEditorCursor;
            CursorObject.transform.SetParent(this.transform);
        }
    }

    Transform GetParentDirectory(int worldID)
    {
        Transform trans = this.transform;
        int foundID = -1;

        if (ParentDirectories.Length == 0)
        {
            ParentDirectories = new GameObject[Definitions.Worlds.Length];
        }
        if (ParentDirectories[worldID] == null) // directory is not set
        {
            GameObject[] instances = GameObject.FindGameObjectsWithTag("MapContainer");
            for (int i = 0; i < instances.Length; i++)
            {
                if (instances[i].name == Definitions.Worlds[worldID].TextureSetName)
                {
                    foundID = i;
                }
            }
            if (foundID >= 0) // directory is not set but exists
            {
                ParentDirectories[worldID] = instances[foundID];
            }
            else // directory is not set - creating a new one
            {
                GameObject objToSpawn = new GameObject(Definitions.Worlds[worldID].TextureSetName);
                objToSpawn.transform.SetParent(null);
                objToSpawn.tag = "MapContainer";
                ParentDirectories[worldID] = objToSpawn;
            }
        }
        trans = ParentDirectories[worldID].transform;
        return trans;
    }

    void OnValidate()
    {
        SegmentsToCheck = new AdjacentSegmentsDef[4];

        SegmentsToCheck[0].Adjacent = new AdjacentSegmentDef[3];
        SegmentsToCheck[0].Adjacent[0].Segment = 1;
        SegmentsToCheck[0].Adjacent[0].TileShift = new Vector2Int(-1, 0);
        SegmentsToCheck[0].Adjacent[1].Segment = 2;
        SegmentsToCheck[0].Adjacent[1].TileShift = new Vector2Int(0, -1);
        SegmentsToCheck[0].Adjacent[2].Segment = 3;
        SegmentsToCheck[0].Adjacent[2].TileShift = new Vector2Int(-1, -1);

        SegmentsToCheck[1].Adjacent = new AdjacentSegmentDef[3];
        SegmentsToCheck[1].Adjacent[0].Segment = 0;
        SegmentsToCheck[1].Adjacent[0].TileShift = new Vector2Int(1, 0);
        SegmentsToCheck[1].Adjacent[1].Segment = 2;
        SegmentsToCheck[1].Adjacent[1].TileShift = new Vector2Int(1, -1);
        SegmentsToCheck[1].Adjacent[2].Segment = 3;
        SegmentsToCheck[1].Adjacent[2].TileShift = new Vector2Int(0, -1);

        SegmentsToCheck[2].Adjacent = new AdjacentSegmentDef[3];
        SegmentsToCheck[2].Adjacent[0].Segment = 0;
        SegmentsToCheck[2].Adjacent[0].TileShift = new Vector2Int(0, 1);
        SegmentsToCheck[2].Adjacent[1].Segment = 1;
        SegmentsToCheck[2].Adjacent[1].TileShift = new Vector2Int(-1, 1);
        SegmentsToCheck[2].Adjacent[2].Segment = 3;
        SegmentsToCheck[2].Adjacent[2].TileShift = new Vector2Int(-1, 0);

        SegmentsToCheck[3].Adjacent = new AdjacentSegmentDef[3];
        SegmentsToCheck[3].Adjacent[0].Segment = 0;
        SegmentsToCheck[3].Adjacent[0].TileShift = new Vector2Int(1, 1);
        SegmentsToCheck[3].Adjacent[1].Segment = 1;
        SegmentsToCheck[3].Adjacent[1].TileShift = new Vector2Int(0, 1);
        SegmentsToCheck[3].Adjacent[2].Segment = 2;
        SegmentsToCheck[3].Adjacent[2].TileShift = new Vector2Int(1, 0);

        MapTileOffset.x = MapSize.x / 2;
        MapTileOffset.y = MapSize.y / 2;
        Definitions = this.GetComponent<LevelEditorDefinitions>();

        Map = new TileObject[Definitions.Worlds.Length,MapSize.x, MapSize.y];

        for (int w = 0; w < Definitions.Worlds.Length; w++)
        {
            InitMapData(w);
        }
    }

    public void InitMapData(int world)
    {
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                if (Map[world, i, j].OccupiedSegments == null)
                {
                    Map[world, i, j].OccupiedSegments = new int[4];
                }
                for (int k = 0; k < 4; k++)
                {
                    Map[world, i, j].OccupiedSegments[k] = 0;
                }
            }
        }
    }

    public void FindTilePrefabs()
    {
        Definitions.FindTilePrefabs();
    }

    public bool CheckCursorPosition(SelectedTile tile)
    {
        bool canBePlaced = true;

        GUIStyle style = new GUIStyle();

        Texture2D texture = new Texture2D(1, 1);
        style.normal.background = texture;
        texture.SetPixel(1, 1, new Color32(0, 0, 0, 128));
        texture.Apply();

        if (IsInMapArea(tile))
        {
            DrawCursorRect(tile);

            style.normal.textColor = Color.green;
            Handles.Label(GridToWorldCoor(tile) + Vector2.up * Definitions.Worlds[SelectedWorld].TileSize * 2 + Vector2.left * Definitions.Worlds[SelectedWorld].TileSize / 2, tile.origin.x.ToString() + ", " + tile.origin.y.ToString(), style);
        }
        else
        {
            style.normal.textColor = Color.red;
            Handles.Label(GridToWorldCoor(tile) + Vector2.up * Definitions.Worlds[SelectedWorld].TileSize * 2 + Vector2.left * Definitions.Worlds[SelectedWorld].TileSize / 2, "Out of map", style);
            canBePlaced = false;
        }

        return canBePlaced;
    }

    public bool IsInMapArea (SelectedTile st)
    {
        bool found = true;
        if (st.origin.x > MapSize.x-1 || st.origin.y-1 > MapSize.y || st.origin.x < 1 || st.origin.y < 1) found = false;
        return found;
    }

    public SelectedTile GetTileInfo(Vector3 pos)
    {
        SelectedTile st;
        st.origin = Vector2Int.zero;
        st.origin.x = Mathf.RoundToInt(pos.x / Definitions.Worlds[SelectedWorld].TileSize);
        st.origin.y = Mathf.RoundToInt(pos.y / Definitions.Worlds[SelectedWorld].TileSize);
        st.segment = Mathf.RoundToInt(Mathf.Sign(pos.x / Definitions.Worlds[SelectedWorld].TileSize - st.origin.x) / 2 + 0.5f) + Mathf.RoundToInt(Mathf.Sign(st.origin.y - pos.y / Definitions.Worlds[SelectedWorld].TileSize) / 2 + 0.5f) * 2;

        st.origin.x += MapTileOffset.x;
        st.origin.y -= MapTileOffset.y;
        st.origin.y = -st.origin.y;

        return st;
    }

    public Vector2 GridToWorldCoor(SelectedTile tile)
    {
        Vector2 worldPos = Vector2.zero;
        worldPos.x = (tile.origin.x - MapTileOffset.x) * Definitions.Worlds[SelectedWorld].TileSize;
        worldPos.y = -(tile.origin.y - MapTileOffset.y) * Definitions.Worlds[SelectedWorld].TileSize;

        return worldPos;
    }

    public void DrawCursorRect(SelectedTile tile)
    {
        
        CheckCursor();
        

        CursorObject.transform.position = GridToWorldCoor(tile);

        for (int i = 0; i < CursorObject.CursorTypeObject.Length; i++)
        {
            if (tile.segment == i)
            {
                CursorObject.CursorTypeObject[i].gameObject.SetActive(true);
            }
            else
            {
                CursorObject.CursorTypeObject[i].gameObject.SetActive(false);
            }
        }
    }

    public LevelEditorTile InstantiateTilePrefab (LevelEditorTile tile, Vector3 position, Transform parent)
    {
        LevelEditorTile go = PrefabUtility.InstantiatePrefab(tile) as LevelEditorTile;
        go.transform.position = position;
        go.transform.parent = GetParentDirectory(SelectedWorld);
        return go;
    }

    public int SegmentsToIndex(int[] segments)
    {
        int index = 0;
        for (int i = 0; i < segments.Length; i++)
        {
            index = index * 10 + segments[i];
        }
        return index;
    }

    public void ClearCheckedTiles(int world)
    {
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                Map[world, i, j].Checked = false;
            }
        }
    }

    public void EraseTilePrefabs (int world)
    {
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                if (Map[world, i, j].Instance != null)
                {
                    DestroyImmediate(Map[world, i, j].Instance.gameObject);
                    Map[world, i, j].Instance = null;
                }
                Map[world, i, j].Checked = false;
                for (int k = 0; k < 4; k++)
                {
                    Map[world, i, j].OccupiedSegments[k] = 0;
                }
            }
        }
    }

    public void PlaceSegment(SelectedTile tile, int variant)
    {
        Vector3 pos = GridToWorldCoor(tile);

        if (Map == null)
        {
            InitMapData(SelectedWorld);
        }

        if (Map[SelectedWorld, tile.origin.x, tile.origin.y].Checked)
        {
            return;
        }

        Map[SelectedWorld, tile.origin.x, tile.origin.y].OccupiedSegments[tile.segment] = variant;

        if (Map[SelectedWorld, tile.origin.x, tile.origin.y].Instance !=null)
        {
            DestroyImmediate(Map[SelectedWorld, tile.origin.x, tile.origin.y].Instance.gameObject);
            Map[SelectedWorld, tile.origin.x, tile.origin.y].Instance = null;
        }

        int newTileID = SegmentsToIndex(Map[SelectedWorld, tile.origin.x, tile.origin.y].OccupiedSegments);

        if (Definitions.Worlds[0].SortedTileObjectsByID[newTileID].TileVariants.Count > 0)
        {
            LevelEditorTile go = InstantiateTilePrefab(Definitions.Worlds[SelectedWorld].SortedTileObjectsByID[newTileID].TileVariants[actualVariant % Definitions.Worlds[SelectedWorld].SortedTileObjectsByID[newTileID].TileVariants.Count], pos, this.transform);
            Map[SelectedWorld, tile.origin.x, tile.origin.y].Instance = go;
            actualVariant += 1;
        }

        Map[SelectedWorld, tile.origin.x, tile.origin.y].Checked = true;

        for (int i = 0; i < 3; i++)
        {
            SelectedTile checkTile;
            checkTile.origin = tile.origin + SegmentsToCheck[tile.segment].Adjacent[i].TileShift;
            checkTile.segment = SegmentsToCheck[tile.segment].Adjacent[i].Segment;
            PlaceSegment(checkTile, variant);
        }
    }
}
#endif