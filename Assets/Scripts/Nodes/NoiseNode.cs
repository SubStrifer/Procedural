using UnityEngine;
using Unity.Mathematics;

/// <summary> Noise generator updates generator state values which can be used later by other nodes. </summary>
[CreateNodeMenu("Generators/Noise")]
[ModifiesState(true)]
public class NoiseNode : StateNode
{
    public NoiseType noiseType;
    [Input][NodeEnum] public NoiseMode noiseMode;
    [Input] public float noiseScale;

    private void Reset()
    {
        name = "Noise";
    }

    protected override void Init()
    {
        statePorts = StatePorts.Both;
        priority = Priority.Low;
    }

    protected override void Execute(GeneratorState state)
    {
        if(noiseScale == 0f)
            noiseScale = 1f;
        float noiseOffset = state.hashedSeed / 1000000f;
        
        foreach(Vector3Int position in state.positions)
        {
            float sample = 0f;
            float2 point = new float2((float)position.x / noiseScale + noiseOffset, 
                        (float)position.y / noiseScale + noiseOffset);

            switch(noiseType)
            {
                case NoiseType.Perlin:
                    sample = noise.cnoise(point);
                    sample = math.unlerp(-1f, 1f, sample);
                    break;
                case NoiseType.Simplex:
                    sample = noise.snoise(point);
                    sample = math.unlerp(-1f, 1f, sample);
                    break;
                case NoiseType.CellularX:
                    sample = noise.cellular(point).x; break;
                case NoiseType.CellularY:
                    sample = noise.cellular(point).y; break;
            }
            
            switch(noiseMode)
            {
                case NoiseMode.Override:
                    state.values[position] = sample; break;
                case NoiseMode.Add:
                    state.values[position] += sample; break;
                case NoiseMode.Subtract:
                    state.values[position] -= sample; break;
                case NoiseMode.Multiply:
                    state.values[position] *= sample; break;
            }
        }

    }

    public enum NoiseType
    {
        Perlin,
        Simplex,
        CellularX,
        CellularY
    }
    
    public enum NoiseMode
    {
        Override,
        Add,
        Subtract,
        Multiply
    }
}