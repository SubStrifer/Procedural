using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary> Places tiles given some rules. </summary>
[CreateNodeMenu("Tiles/Tile")]
[ModifiesState(false)]
public class TileNode : StateNode
{
    //[Input] public Tilemap tilemap;
    [Input] public string tilemap;
    [Input] public Tile tile;
    [Input] public float minValue;
    [Input][Range(.0f, 1f)] public float probability;

    private void Reset()
    {
        name = "Tile";
    }

    protected override void Init()
    {
        statePorts = StatePorts.Input;
        priority = Priority.High;
    }
    
    protected override void Execute(GeneratorState state)
    {
        foreach(KeyValuePair<Vector3Int, float> pair in state.values)
        {
            //todo could speed up if probability isn't used
            if(pair.Value >= minValue && state.random.NextFloat() < probability)
            {
                state.tiles[tilemap][pair.Key] = tile;
            }
        }
    }

    protected override int CompareToValues(StateNode other)
    {
        return minValue.CompareTo((other as TileNode).minValue);
    }

}