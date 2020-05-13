using UnityEngine;

[CreateNodeMenu("Utils/Math")]
[ModifiesState(true)]
public class MathNode : StateNode
{
    public MathType mathType;
    public float value;

    private void Reset()
    {
        name = "Math";
    }

    protected override void Init()
    {
        statePorts = StatePorts.Both;
        priority = Priority.Low;
    }

    protected override void Execute(GeneratorState state)
    {
        switch(mathType)
        {
            case MathType.Add:
                foreach(Vector3Int position in state.positions)
                        state.values[position] += value;
                break;
            case MathType.Subtract:
                foreach(Vector3Int position in state.positions)
                        state.values[position] -= value;
                break;
            case MathType.Multiply:
                foreach(Vector3Int position in state.positions)
                        state.values[position] *= value;
                break;
        }
    }
}

public enum MathType
{
    Add,
    Subtract,
    Multiply
}