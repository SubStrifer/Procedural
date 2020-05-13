using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

[CreateNodeMenu("Tiles/Path")]
[ModifiesState(true)]
public class PathNode : StateNode
{
    [Input] public string tilemap;
    [Input] public Tile tile;
    [Input] public float maxDelta;
    [Input] public int maxNumber;
    [Input] public int maxLength;

    private void Reset()
    {
        name = "Path";
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
        float minDelta;
        int triesLeft = maxNumber * 10;// Safety, to avoid trying generating infinitely
        int number = 0;
        int length = 0;

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

            length = 0;
            newPosition = position;
            Vector3Int[] temp = new Vector3Int[4];

            while(length < maxLength)
            {
                temp[0] = position + Vector3Int.up;
                temp[1] = position + Vector3Int.down;
                temp[2] = position + Vector3Int.left;
                temp[3] = position + Vector3Int.right;

                minDelta = float.MaxValue;

                for(int i = 0; i < 4; i++)
                {
                    if(!state.values.ContainsKey(temp[i]))
                        continue;

                    float delta = math.abs(state.values[temp[i]] - state.values[position]);
                    if(delta < maxDelta && delta < minDelta)
                    {
                        newPosition = temp[i];
                        minDelta = delta;
                    }
                }

                if(newPosition != position)
                {
                    state.tiles[tilemap][newPosition] = tile;
                    position = newPosition;
                    length++;
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
