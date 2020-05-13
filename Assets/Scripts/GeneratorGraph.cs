using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Threading;

[CreateAssetMenu]
public class GeneratorGraph : NodeGraph
{
    public string seed;
    public bool customSeed;
    public Vector2Int gridSize;
    public List<string> layers = new List<string>();
    public GeneratorState state;
    Thread generationThread;

    private void ExecuteThread()
    {     
        // Execute recursively
        RootNode root = (RootNode)nodes.Find(n => n is RootNode);
        root.ExecuteRecursively(state);   
    }

    public Thread Execute(GeneratorState state)
    {
        // Find and check the RootNode
        RootNode root = (RootNode)nodes.Find(n => n is RootNode);
        if(root == null)
        {
            Debug.LogWarning("No Entry node in the Graph, cannot generate the map.");
            return null;
        }
        
        this.state = state;
        generationThread = new Thread(ExecuteThread);
        generationThread.Start();
        return generationThread;
    }

}
