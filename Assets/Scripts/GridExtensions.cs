using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GridExtensions
{
    public static void ReorderTilemap(this Grid grid, int oldIndex, int newIndex)
    {
        grid.transform.GetChild(oldIndex).SetSiblingIndex(newIndex);
        grid.RefreshOrdering();
    }

    public static void AddTilemap(this Grid grid, string name)
    {
        // Create a new Tilemap
        GameObject tilemap = new GameObject(name);
        tilemap.AddComponent<Tilemap>();
        tilemap.AddComponent<TilemapRenderer>();
        tilemap.transform.SetParent(grid.transform);
        grid.RefreshOrdering();
    }

    public static void RemoveTilemap(this Grid grid, int index)
    {
        GameObject.DestroyImmediate(grid.transform.GetChild(index).gameObject);
        grid.RefreshOrdering();
    }

    public static void RemoveTilemaps(this Grid grid)
    {
        // Remove all children from the Grid
        List<GameObject> children = new List<GameObject>();

        // Getting all children and putting them to a new list
        for(int i = 0; i < grid.transform.childCount; i++)
            children.Add(grid.transform.GetChild(i).gameObject);
        
        // Loop through children and destroy them
        foreach(GameObject go in children)
            GameObject.DestroyImmediate(go);
    }

    public static void RefreshOrdering(this Grid grid)
    {
        TilemapRenderer[] tilemaps = grid.GetComponentsInChildren<TilemapRenderer>();
        for(int i = 0; i < tilemaps.Length; i++)
        {
            tilemaps[i].sortingOrder = tilemaps.Length - i;
        }
    }
}
