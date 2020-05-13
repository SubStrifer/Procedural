using UnityEngine;

[CreateNodeMenu("Generators/Maze")]
public class MazeNode : StateNode
{
    [Input] public float minValue;
    [Input][Range(.0f, 1f)] public float density;
    [Input][Range(.0f, 1f)] public float connectionsDensity;
    [Input] public bool border;

    private void Reset()
    {
        name = "Maze";
    }

    protected override void Init()
    {
        statePorts = StatePorts.Both;
        priority = Priority.Medium;
    }

    protected override void Execute(GeneratorState state)
    {
        foreach(Vector3Int position in state.positions)
        {
            // Borders
            if (position.x == 0 || position.y == 0 || position.x == state.gridSize.x - 1 || position.y == state.gridSize.y - 1)
            {
                if(border)
                    state.values[position] = 1f;
            }
            else if (position.x % 2 == 0 && position.y % 2 == 0)
            {
                if (state.values[position] > minValue && state.random.NextFloat(0f, 1f) < density)
                {
                    state.values[position] = 1f;

                    if(state.random.NextFloat(0f, 1f) > connectionsDensity)
                        continue;

                    int a = 0, b = 0;

                    if(state.random.NextBool())
                        a = (state.random.NextFloat(0f, 1f) < .5f ? -1 : 1);
                    else
                        b = (state.random.NextFloat(0f, 1f) < .5f ? -1 : 1);
                    state.values[position + new Vector3Int(a, b, 0)] = 1f;
                }
                else
                    state.values[position] = 0f;
            }
        }
    }

    protected override int CompareToValues(StateNode other)
    {
        return minValue.CompareTo((other as MazeNode).minValue);
    }

}