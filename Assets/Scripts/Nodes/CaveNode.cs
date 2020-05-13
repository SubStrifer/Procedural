using UnityEngine;
using UnityEngine.Tilemaps;

[CreateNodeMenu("Tiles/Cave")]
[ModifiesState(true)]
public class CaveNode : StateNode
{
    [Input] public string tilemap;
    [Input] public Tile tile;
    [Input] public float minValue;
    [Input] public int maxNumber;
    [Input] public int maxLength;

    private void Reset()
    {
        name = "Cave";
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

                int i = state.random.NextInt(0, 4);

                if(state.values.ContainsKey(temp[i]))
                if(state.values[temp[i]] > minValue)
                {
                    newPosition = temp[i];
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

    protected override int CompareToValues(StateNode other)
    {
        return minValue.CompareTo((other as CaveNode).minValue);
    }
}
