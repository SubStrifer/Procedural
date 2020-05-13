using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;
using System;

[CreateNodeMenu("")]
public class StateNode : Node, IComparable<StateNode>
{
    // Showing and hiding Input and Output state ports
    private StatePorts _statePorts;
    protected StatePorts statePorts
    {
        get => _statePorts;
        set
        {
            switch(value)
            {
                case StatePorts.Input:
                    if(!HasPort("inputState"))
                        AddDynamicInput(typeof(GeneratorState), ConnectionType.Override, fieldName: "inputState");
                    if(HasPort("outputState"))
                        RemoveDynamicPort("outputState");
                    break;
                case StatePorts.Output:
                    if(HasPort("inputState"))
                        RemoveDynamicPort("inputState");
                    if(!HasPort("outputState"))
                        AddDynamicOutput(typeof(GeneratorState), fieldName: "outputState");
                    break;
                case StatePorts.Both:
                    if(!HasPort("inputState"))
                        AddDynamicInput(typeof(GeneratorState), ConnectionType.Override, fieldName: "inputState");
                    if(!HasPort("outputState"))
                        AddDynamicOutput(typeof(GeneratorState), fieldName: "outputState");
                    break;
            }
            _statePorts = value;
        }
    }

    protected Priority priority;

    public void ExecuteRecursively(GeneratorState state)
    {
        Execute(state);

        if(!HasPort("outputState"))
            return;

        bool multipleConnections = GetOutputPort("outputState").ConnectionCount > 1 ? true : false;

        // Select all StateNodes
        List<StateNode> nodes = GetOutputPort("outputState").GetConnections().Select(x => x.node).OfType<StateNode>().ToList();

        // Sort nodes
        nodes.Sort();

        // Execution
        foreach(StateNode node in nodes)
        {
            // Clone the state for a node if necessary
            bool clone = false;

            if(multipleConnections)
            foreach(ModifiesStateAttribute attribute in node.GetType().GetCustomAttributes(typeof(ModifiesStateAttribute), true))
            {
                if(attribute.modify)
                {
                    clone = true;
                    break;
                }
            }

            node.ExecuteRecursively(clone ? (GeneratorState)state.Clone() : state);
        }
    }

    protected virtual void Execute(GeneratorState state) { } 

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public int CompareTo(StateNode other)
    {
        // Comparing the same type
        if(GetType() == other.GetType())
            return CompareToValues(other);

        // Compare priorities
        return (int)priority - (int)other.priority;
    }

    protected virtual int CompareToValues(StateNode other)
    {
        if (object.ReferenceEquals(other, null))
            return 1;// All instances are greater than null

        // Base class is always equal
        return 0;
    }

    public enum StatePorts
    {
        None,
        Input,
        Output,
        Both
    }

    protected enum Priority
    {
        High,
        Medium,
        Low
    }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class ModifiesStateAttribute : System.Attribute
{
    public bool modify { get; private set; }

    public ModifiesStateAttribute(bool modify = true)
    {
        this.modify = modify;
    }
}