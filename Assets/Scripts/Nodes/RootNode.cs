[CreateNodeMenu("Entry")]
[ModifiesState(false)]
public class RootNode : StateNode
{
    private void Reset()
    {
        name = "Entry";
    }

    protected override void Init()
    {
        statePorts = StatePorts.Output;
    }

    protected override void Execute(GeneratorState state)
    {
        
    }
}
