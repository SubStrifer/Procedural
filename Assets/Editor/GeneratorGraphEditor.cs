using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(GeneratorGraph), "GeneratorGraph.Settings")]
public class GeneratorGraphEditor : NodeGraphEditor
{

    public override string GetNodeMenuName(System.Type node)
    {
        return base.GetNodeMenuName(node);
    }
}