using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = Unity.Mathematics.Random;

/// <summary> Tilemap Generator. </summary>
[ExecuteInEditMode]
[InitializeOnLoad]
public class GeneratorController
{
    private static GeneratorController _instance;
    public static GeneratorController instance
    {
        get
        {       
            if(_instance == null)
            {
                _instance = new GeneratorController();
                LoadTiles();
            }
            return _instance;
        }
    }

    private Grid grid;
    public static Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

    static GeneratorGraph _graph;
    static Thread _generationThread;
    static bool updateAdded = false;
    private static float time;
    private const string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    static void UpdateAlways()
    {
        if(_generationThread == null)
            return;
        else if(!_generationThread.IsAlive)
        {
            _generationThread.Abort();
            _generationThread = null;
            SetTiles(_graph.state);
        }
    }

    /// <summary> Create tiles for tilemaps using current settings. </summary>
    public void Generate(GeneratorGraph graph, Grid grid)
    {
        this.grid = grid;
        time = (float)EditorApplication.timeSinceStartup;

        _graph = graph;
        if(!updateAdded)
        {
            EditorApplication.update += UpdateAlways;
            updateAdded = true;
        }

        // Used only during generation
        GeneratorState state = new GeneratorState(grid, graph.gridSize, graph.layers);

        // Setting seed
        if(!graph.customSeed)
        {
            graph.seed = "";
            // Temporary hashed seed from the current time
            state.hashedSeed = (uint)System.DateTime.Now.GetHashCode();
            state.random = new Random(state.hashedSeed);
            // Generate random string
            for(int i = 0; i < 16; i++)
            {
                graph.seed += glyphs[state.random.NextInt(0, glyphs.Length)];
            }
            state.hashedSeed = (uint)graph.seed.GetHashCode();
        }
        else
        {
            state.hashedSeed = (uint)graph.seed.GetHashCode();
        }

        state.random = new Random(state.hashedSeed); 

        // Execute Nodes
        _generationThread = graph.Execute(state);

    }

    public void ClearGrid(Grid grid)
    {
        this.grid = grid;

        foreach(Tilemap tilemap in grid.GetComponentsInChildren<Tilemap>())
            tilemap.ClearAllTiles();
    }

    public static void SetTiles(GeneratorState state)
    {
        Debug.Log("Generated (" + (Mathf.Round(((float)EditorApplication.timeSinceStartup - time) * 100f) / 100f) + "s)");
        foreach(string layer in state.tiles.Keys)
        {
            Tilemap tm = System.Array.Find(_graph.state.grid.GetComponentsInChildren<Tilemap>(), 
                        t => t.name.Equals(layer));

            foreach(KeyValuePair<Vector3Int, Tile> pair in state.tiles[layer])
            {
                tm.SetTile(pair.Key, pair.Value);
            }

        }
        Debug.Log("Tiles set (" + (Mathf.Round(((float)EditorApplication.timeSinceStartup - time) * 100f) / 100f) + "s)");
    }

    /// <summary> Instantiate basic Grid. </summary>
    public Grid InstantiateGrid()
    {
        if(!grid)
        {
            //grid = GameObject.FindObjectOfType<Grid>();
            if(!grid)
            {
                GameObject g = new GameObject("Grid");
                grid = g.AddComponent<Grid>();
            }
            // Add tilemaps to dictionary  
            //tilemaps.Clear();
            //foreach(Tilemap tilemap in mainGrid.GetComponentsInChildren<Tilemap>(true))
            //        tilemaps[tilemap.name] = tilemap;
            LoadTiles();
        }
        return grid;
    }

    private static void LoadTiles()
    {
        foreach(Tile tile in Resources.LoadAll<Tile>("ProceduralGenerator/Tiles"))
        {
            tiles[tile.name] = tile;
        }
    }

    #region Serialization

    public void SaveGrid(string file)
    {
        GridData gridData = new GridData();
        gridData.tilemaps = new List<TilemapData>();
        Vector3Int position = new Vector3Int();

        foreach(Tilemap tilemap in grid.GetComponentsInChildren<Tilemap>())
        {
            TilemapData tilemapData = new TilemapData {name = tilemap.name, tiles = new List<TileData>()};
            gridData.tilemaps.Add(tilemapData);
            for(int x = tilemap.origin.x; x < tilemap.size.x; x++)
            {
                position.x = x;
                for(int y = tilemap.origin.y; y < tilemap.size.y; y++)
                {
                    position.y = y;
                    if(tilemap.HasTile(position))
                    {
                        tilemapData.tiles.Add(new TileData {name = tilemap.GetTile(position).name, position = new Vector2Int(position.x, position.y)});
                    }
                }
            }
        }
        string json = JsonUtility.ToJson(gridData);
        File.WriteAllText(Application.dataPath + "\\" + file + ".json", json);
    }

    public void LoadGrid(string file)
    {
        LoadTiles();
        ClearGrid(grid);

        string json = File.ReadAllText(Application.dataPath + "\\" + file + ".json");
        GridData gridData = JsonUtility.FromJson<GridData>(json);

        // Remove all layers
        grid.RemoveTilemaps();
    
        // Setup layers
        foreach(TilemapData tilemap in gridData.tilemaps)
            grid.AddTilemap(tilemap.name);

        // Setting tiles
        foreach(TilemapData tilemapData in gridData.tilemaps)
        {
            Tilemap tm = System.Array.Find(_graph.state.grid.GetComponentsInChildren<Tilemap>(), 
                        t => t.name.Equals(tilemapData.name));

            foreach(TileData tileData in tilemapData.tiles)
            {
                tm.SetTile(new Vector3Int(tileData.position.x, tileData.position.y, 0), tiles[tileData.name]);
            }
        }
    }

    [System.Serializable]
    class GridData
    {
        public List<TilemapData> tilemaps;
    }

    [System.Serializable]
    class TilemapData
    {
        public string name;
        public List<TileData> tiles;
    }

    [System.Serializable]
    class TileData
    {
        public string name;
        public Vector2Int position;
    }

    #endregion
}
