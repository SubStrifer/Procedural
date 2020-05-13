using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = Unity.Mathematics.Random;

[System.Serializable]
public class GeneratorState : ICloneable
{
    public Dictionary<Vector3Int, float> values;// Values for noise generators, etc.

    // Shared between all states
    public uint hashedSeed;
    public Vector2Int gridSize;
    public Grid grid;
    public Random random;//todo random should be globally accessible

    List<string> layers;
    public Dictionary<string, Dictionary<Vector3Int, Tile>> tiles;
    public List<Vector3Int> positions;

    public GeneratorState(Grid grid, Vector2Int gridSize, List<string> layers, Dictionary<string, Dictionary<Vector3Int, Tile>> tiles = null)
    {
        this.grid = grid;
        this.gridSize = gridSize;
        this.layers = layers;
        values = new Dictionary<Vector3Int, float>();
        this.tiles = tiles;

        // Create a List for iterations
        if(positions == null)
        {
            positions = new List<Vector3Int>();

            for(Vector3Int position = new Vector3Int(); position.x < gridSize.x; position.x++)
            for(position.y = 0; position.y < gridSize.y; position.y++)
                positions.Add(position);
        }

        if(tiles == null)
        {
            this.tiles = new Dictionary<string, Dictionary<Vector3Int, Tile>>();
            // Set layerMask values
            foreach(string layer in layers)
            {
                this.tiles.Add(layer, new Dictionary<Vector3Int, Tile>());
            }
        }

        // Set all values to 0f
        for(Vector3Int position = new Vector3Int(); position.x < gridSize.x; position.x++)
        for(position.y = 0; position.y < gridSize.y; position.y++)
                values[position] = 0f;
    }

    public object Clone()
    {
        GeneratorState state = new GeneratorState(grid, gridSize, layers, tiles);
        state.hashedSeed = hashedSeed;
        state.values = new Dictionary<Vector3Int, float>(values);
        state.random = random;
        return state;
    }

}
