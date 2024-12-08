using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ChunkType { Forest, Plains, Beach, Mountain, Base, Camp, Chest, Den }

public class Chunk : ScriptableObject
{
    public Vector3Int gridPosition;
    public Vector3Int anchorPosition;
    public List<Vector3Int> containedPositions;
    public Color debugColor;
    public ChunkType chunkType;

    public int size
    {
        get
        {
            return containedPositions.Count;
        }
    }

    public Vector3Int center
    {
        get
        {
            return GetAverageCenter();
        }
    }

    public void Initialize(Vector3Int gridPosition, Vector3Int anchorPosition, Color debugColor)
    {
        this.gridPosition = gridPosition;
        this.anchorPosition = anchorPosition;
        this.debugColor = debugColor;

        // Default values
        chunkType = ChunkType.Forest;
        containedPositions = new List<Vector3Int>();
    }

    public void Define(ChunkType chunkType)
    {
        this.chunkType = chunkType;
    }

    public void AddContainedTile(int x, int y)
    {
        if (containedPositions == null)
            throw new System.Exception("LIST NOT INIT!");

        containedPositions.Add(new Vector3Int(x, y));
    }

    public List<Vector2> GetPerimeter()
    {
        // Find all inner and outer positions
        List<Vector2> outer = new List<Vector2>();
        List<Vector2> inner = new List<Vector2>();

        List<Vector2> perimeterPoints = new List<Vector2>();
        foreach (var position in containedPositions)
        {
            // If only one diagonal adjacent, then inner
            if (IsInnerPosition(position))
            {
                inner.Add(new Vector2(position.x, position.y));
                perimeterPoints.Add(new Vector2(position.x, position.y));
            }
            // If at least 2 diagonal adjacents, then outer
            else if (IsOutterPosition(position))
            {
                outer.Add(new Vector2(position.x, position.y));
                perimeterPoints.Add(new Vector2(position.x, position.y));
            }
        }

        outer.AddRange(inner);

        return outer;
    }

    private Vector3Int GetAverageCenter()
    {
        Vector3Int sum = Vector3Int.zero;

        // Sum all the positions
        foreach (Vector3Int pos in containedPositions)
        {
            sum += pos;
        }

        // Calculate the average
        Vector3Int averageCenter = sum / containedPositions.Count;

        return averageCenter;
    }

    private int GetNullAdjacentCount(Vector3Int position)
    {
        int count = 0;

        if (!containedPositions.Contains(position + new Vector3Int(1, 1)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(1, -1)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(-1, 1)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(-1, -1)))
            count++;

        return count;
    }

    private bool IsInnerPosition(Vector3Int position)
    {
        int count = 0;

        if (!containedPositions.Contains(position + new Vector3Int(1, 1)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(1, -1)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(-1, 1)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(-1, -1)))
            count++;

        // Make sure only 1 adjacent
        return count == 1;
    }

    private bool IsOutterPosition(Vector3Int position)
    {
        int count = 0;

        if (!containedPositions.Contains(position + new Vector3Int(0, 1)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(1, 0)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(-1, 0)))
            count++;
        if (!containedPositions.Contains(position + new Vector3Int(0, -1)))
            count++;

        // If at least 2
        return count > 1;
    }
}
