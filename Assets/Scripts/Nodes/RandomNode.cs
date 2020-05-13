using UnityEngine;
using XNode;

[CreateNodeMenu("Utils/Random")]
[NodeTint(.3f, .3f, .3f)]
[NodeWidth(228)]
[ModifiesState(false)]
public class RandomNode : Node
{
    [Input] public float minValue;
    [Input] public float maxValue;
    [Output] public float value;

    private void Reset()
    {
        name = "Random";
    }

    protected override void Init()
    {
        
    }
    
    public override object GetValue(NodePort port)
    {
        //Random random = new Random((graph as GeneratorGraph).hashedSeed)
        //todo should return consistent values when using the same seed
        return Random.Range(minValue, maxValue);
    }


}