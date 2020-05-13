using UnityEngine;
using UnityEngine.Tilemaps;

[CreateNodeMenu("Tiles/River")]
[ModifiesState(true)]
public class RiverNode : StateNode
{
    [Input] public string tilemap;
    [Input] public Tile tile;
    [Input] public int maxNumber;

    private void Reset()
    {
        name = "River";
    }

    protected override void Init()
    {
        statePorts = StatePorts.Input;
        priority = Priority.Medium;
    }
    
    protected override void Execute(GeneratorState state)
    {
        int x, y;
        Vector3Int position, newPosition;
        float value;
        int triesLeft = maxNumber * 10;// Safety, to avoid trying generating infinitely
        int number = 0;

        while(triesLeft > 0 && number < maxNumber)
        {
            x = state.random.NextInt(1, state.gridSize.x - 1);
            y = state.random.NextInt(1, state.gridSize.y - 1);
            position = new Vector3Int(x, y, 0);

            if(state.tiles[tilemap][position] == tile)
            {
                triesLeft--;
                continue;
            }

            newPosition = position;
            value = state.values[position];
            Vector3Int[] temp = new Vector3Int[4];

            while(true)
            {
                // Find the lowest value among four neighbours
                temp[0] = position + Vector3Int.up;
                temp[1] = position + Vector3Int.down;
                temp[2] = position + Vector3Int.left;
                temp[3] = position + Vector3Int.right;

                for(int i = 0; i < 4; i++)
                {
                    if(state.values.ContainsKey(temp[i]))
                    if(state.values[temp[i]] < state.values[position] && state.values[temp[i]] < value)
                    {
                        newPosition = temp[i];
                        value = state.values[temp[i]];
                    }
                }

                if(newPosition != position)
                {
                    state.tiles[tilemap][newPosition] = tile;
                    position = newPosition;
                }
                else
                {
                    number++;
                    break;
                }
            }
        }
    }

}
