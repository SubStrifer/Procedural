using UnityEngine;
using UnityEngine.Tilemaps;

[CreateNodeMenu("Utils/Filter")]
[ModifiesState(true)]
public class FilterNode : StateNode
{
    [Input] public string tilemap;
    [Input] public Tile tile;
    [Input] public float setValue;

    private void Reset()
    {
        name = "Filter";
    }

    protected override void Init()
    {
        statePorts = StatePorts.Both;
        priority = Priority.Low;
    }

    protected override void Execute(GeneratorState state)
    {
        foreach(Vector3Int position in state.positions)
        {
            if(state.tiles[tilemap][position] == tile)
            {
                state.values[position] = setValue;
            }
        }
    }

    protected override int CompareToValues(StateNode other)
    {
        return setValue.CompareTo((other as FilterNode).setValue);
    }
}
