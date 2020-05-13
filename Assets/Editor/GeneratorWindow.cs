using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.IMGUI;
using System.Reflection;

/// <summary> Generator Window for customisation of Generation process. </summary>
[ExecuteInEditMode]
public class GeneratorWindow : EditorWindow
{
    private string fileName;
    private Grid grid;
    private ReorderableList layersReorderableList;
    private GeneratorGraph oldGraph;// Needed to swap graphs
    private GeneratorGraph graph;
    private SerializedObject serializedGraph;
    private bool tryFindGrid;

    [MenuItem("Window/Generator")]
    public static void ShowWindow()
    {
        GetWindow<GeneratorWindow>("Generator");
    }

    #region Layers

    private void LayersDrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Layers");
    }

    private void LayersDrawElement(Rect rect, int index, bool isactive, bool isfocused)
    {
        //Get the element we want to draw from the list.
        SerializedProperty element = layersReorderableList.serializedProperty.GetArrayElementAtIndex(index);

        rect.y += 2;
        string old = element.stringValue;
        element.stringValue = EditorGUI.TextField(new Rect(rect.x += 10, rect.y, Screen.width * .8f, EditorGUIUtility.singleLineHeight), element.stringValue);
        //todo undo/redo doesn't work
        if(!old.Equals(element.stringValue) && !string.IsNullOrWhiteSpace(element.stringValue))
        {
            Debug.Log("Layer name changed");
            grid.transform.GetChild(index).name = element.stringValue;
        }
    }

    private void LayersOnAdd(ReorderableList list)
    {
        int index = list.serializedProperty.arraySize;
        list.serializedProperty.InsertArrayElementAtIndex(index);
        list.serializedProperty.GetArrayElementAtIndex(index).stringValue = "New Layer";
        grid.AddTilemap("New Layer");
    }

    private void LayersOnRemove(ReorderableList list)
    {
        list.serializedProperty.DeleteArrayElementAtIndex(list.index);
        grid.RemoveTilemap(list.index);
    }

    private void LayersOnReorder(ReorderableList list, int oldIndex, int newIndex)
    {
        grid.ReorderTilemap(oldIndex, newIndex);
    }

    #endregion

    void OnEnable ()
    {
        InitLayersList();
    }

    void InitLayersList()
    {
        layersReorderableList = new ReorderableList(serializedGraph, serializedGraph.FindProperty("layers"), true, true, true, true);
        layersReorderableList.drawHeaderCallback = LayersDrawHeader;
        layersReorderableList.drawElementCallback = LayersDrawElement;
        layersReorderableList.onAddCallback += LayersOnAdd;
        layersReorderableList.onRemoveCallback += LayersOnRemove;
        layersReorderableList.onReorderCallbackWithDetails += LayersOnReorder;
    }

    void OnGUI()
    {
        // Graph
        graph = (GeneratorGraph)EditorGUILayout.ObjectField("Node Graph", graph, typeof(GeneratorGraph), true);

        if(!graph)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select or create a new Generator Graph");
            EditorGUILayout.EndHorizontal();
            return;
        }

        // Graph changed
        if(graph != oldGraph)
        {
            serializedGraph = new SerializedObject(graph);
            oldGraph = graph;
            grid = null;
            InitLayersList();
        }

        if(serializedGraph == null)
            serializedGraph = new SerializedObject(graph);

        serializedGraph.Update();

        // Seed
        GUILayout.Space(16f);
        graph.customSeed = EditorGUILayout.BeginToggleGroup("Custom seed", graph.customSeed);
        EditorGUILayout.PropertyField(serializedGraph.FindProperty("seed"));
        EditorGUILayout.EndToggleGroup();

        // Grid
        Grid oldGrid = grid;
        grid = (Grid)EditorGUILayout.ObjectField("Grid", grid, typeof(Grid), true);

        // Try to find a Grid if not assigned
        if(!grid && tryFindGrid)
        {
            tryFindGrid = false;
            grid = FindObjectOfType<Grid>();
        }

        // Grid check
        if(!grid)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select or create a new Grid");
            if(GUILayout.Button("Find"))
            {
                grid = FindObjectOfType<Grid>();
            }
            EditorGUILayout.EndHorizontal();
            if(!grid)
                return;
        }   
    
        // Grid changed
        if(grid != oldGrid)
        {
            // Remove all layers
            grid.RemoveTilemaps();
        
            // Create layers
            for(int i = 0; i < serializedGraph.FindProperty("layers").arraySize; i++)
                grid.AddTilemap(serializedGraph.FindProperty("layers").GetArrayElementAtIndex(i).stringValue);
        }

        // Grid size
        EditorGUILayout.PropertyField(serializedGraph.FindProperty("gridSize"));

        // Layers
        if(layersReorderableList == null)
            InitLayersList();
        layersReorderableList.DoLayoutList();

        serializedGraph.ApplyModifiedProperties();

        GUILayout.Space(16f);
        if(GUILayout.Button("Generate"))
        {
            // Remove all layers
            grid.RemoveTilemaps();
        
            // Create layers
            for(int i = 0; i < serializedGraph.FindProperty("layers").arraySize; i++)
                grid.AddTilemap(serializedGraph.FindProperty("layers").GetArrayElementAtIndex(i).stringValue);
                
            GeneratorController.instance.Generate(graph, grid);
        }

        // Remove all tiles from all tilemaps
        GUILayout.Space(16f);
        if(GUILayout.Button("Clear"))
        {
            GeneratorController.instance.ClearGrid(grid);
        }

        // Save and load generated maps
        GUILayout.Space(16f);
        GUILayout.BeginHorizontal();
        fileName = EditorGUILayout.TextField(fileName);
        if(GUILayout.Button("Save"))
        {
            GeneratorController.instance.SaveGrid(fileName);
            Debug.Log("Grid saved");
        }
        if(GUILayout.Button("Load"))
        {
            GeneratorController.instance.LoadGrid(fileName);
            Debug.Log("Grid loaded");
        }
        GUILayout.EndHorizontal();
    }

}
