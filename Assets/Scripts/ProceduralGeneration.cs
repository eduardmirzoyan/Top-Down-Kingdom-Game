using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration
{
    public Chunk[,] GenerateWorld(int mapRadius, int mapPadding, int nodeGridSize, float cutoffPercentage)
    {
        int worldRadius = mapRadius + mapPadding;
        int tilesPerNode = 2 * mapRadius / nodeGridSize;

        // 0 - Water | 1 - Forest | 2 - Plains |  3 - Beach | 4 - Mountain
        Color[,] worldGrid = new Color[worldRadius * 2 + 1, worldRadius * 2 + 1];

        Vector3Int[,] nodeGrid = new Vector3Int[nodeGridSize, nodeGridSize];
        Chunk[,] chunkMap = new Chunk[nodeGridSize, nodeGridSize];

        Color[,] colorsGrid = new Color[nodeGridSize, nodeGridSize];

        // Create chunks and their center
        for (int i = 0; i < nodeGridSize; i++)
        {
            for (int j = 0; j < nodeGridSize; j++)
            {
                // Get world center location
                var centerOffset = mapPadding + tilesPerNode / 2;
                Vector2Int worldCenterPosition = new Vector2Int(centerOffset + i * tilesPerNode, centerOffset + j * tilesPerNode);

                // Debug.Log($"Node [{i}, {j}]; Position: {worldCenterPosition}");

                // Make sure point is within circle
                if (SquareInsideCircle(worldCenterPosition, tilesPerNode, new Vector2(worldRadius, worldRadius), mapRadius, cutoffPercentage))
                {
                    // Generate random offset
                    int xOffset = mapPadding + Random.Range(0, tilesPerNode);
                    int yOffset = mapPadding + Random.Range(0, tilesPerNode);

                    // Random position within square and color
                    Vector3Int position = new Vector3Int(i * tilesPerNode + xOffset, j * tilesPerNode + yOffset);
                    Color randomColor = Random.ColorHSV();

                    // Create new chunk
                    var chunk = ScriptableObject.CreateInstance<Chunk>();
                    chunk.Initialize(new Vector3Int(i, j), position, randomColor);
                    chunkMap[i, j] = chunk;

                    // Set anchor location
                    nodeGrid[i, j] = position;

                    // Assign random color
                    colorsGrid[i, j] = randomColor;

                    // DEBUGGING (show color)
                    worldGrid[position.x, position.y] = Color.black;
                }
            }
        }

        // Now assign tiles to chunk
        for (int i = 0; i < worldGrid.GetLength(0); i++)
        {
            for (int j = 0; j < worldGrid.GetLength(1); j++)
            {
                // Check if inside map
                if (IsInsideCircle(i, j, worldRadius, worldRadius, mapRadius))
                {
                    var offset = mapPadding + tilesPerNode / 2;

                    int gridX = (i - offset) / tilesPerNode;
                    int gridY = (j - offset) / tilesPerNode;

                    float nearestDistance = float.MaxValue;
                    Vector2Int nearestPoint = Vector2Int.zero;

                    // Check neighbors
                    for (int a = -1; a <= 1; a++)
                    {
                        for (int b = -1; b <= 1; b++)
                        {
                            int x = gridX + a;
                            int y = gridY + b;

                            // Bound check
                            if (IsOutOfBounds(x, y, nodeGridSize))
                                continue;

                            float distance = Vector3.Distance(new Vector3Int(i, j), nodeGrid[x, y]);
                            if (distance < nearestDistance)
                            {
                                nearestDistance = distance;
                                nearestPoint = new Vector2Int(x, y);
                            }
                        }
                    }

                    // Set color
                    worldGrid[i, j] = colorsGrid[nearestPoint.x, nearestPoint.y];

                    // Add to nearest chunk
                    chunkMap[nearestPoint.x, nearestPoint.y].AddContainedTile(i, j);
                }
                else
                {
                    // Water
                    worldGrid[i, j] = Color.cyan;
                }
            }
        }

        return chunkMap;
    }

    public void GenerateBase(Chunk[,] chunkMap, float checkRadius, Vector3Int mapCenter)
    {
        // Check certain radius from spawn, finding largest segment within

        // int largestSize = 0;
        // Chunk largestChunk = null;
        // foreach (var chunk in chunkMap)
        // {
        //     if (chunk == null)
        //         continue;

        //     // If center is within radius
        //     float distance = Vector2.Distance(chunk.center, mapCenter);
        //     if (distance <= checkRadius)
        //     {
        //         // Check largest
        //         int size = chunk.size;
        //         if (size > largestSize)
        //         {
        //             // Update largest
        //             largestSize = size;
        //             largestChunk = chunk;
        //         }
        //     }
        // }

        // if (largestChunk != null)
        // {
        //     largestChunk.Define(ChunkType.Base);
        // }
        // else
        //     throw new System.Exception("SUITABLE CHUNK NOT FOUND!");

        // Find chunk closest to the center

        float closestDistance = float.MaxValue;
        Chunk closestChunk = null;
        foreach (var chunk in chunkMap)
        {
            if (chunk == null)
                continue;

            // If center is within radius
            float distance = Vector3.Distance(chunk.center, mapCenter);
            if (distance <= closestDistance)
            {
                // Update largest
                closestDistance = distance;
                closestChunk = chunk;
            }
        }

        if (closestChunk != null)
        {
            closestChunk.Define(ChunkType.Base);
        }
        else
            throw new System.Exception("SUITABLE CHUNK NOT FOUND!");
    }

    public void GenerateBeach(Chunk[,] chunkMap)
    {
        // Find all the chunks bordering the edge of the map
        int nodeGridSize = chunkMap.GetLength(0);
        for (int i = 0; i < nodeGridSize; i++)
        {
            for (int j = 0; j < nodeGridSize; j++)
            {
                var chunk = chunkMap[i, j];
                if (chunk == null)
                    continue;

                // Check if any cardinal neightbor is a null or out of bounds
                foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.left, Vector2Int.down, Vector2Int.right })
                {
                    int x = i + direction.x;
                    int y = j + direction.y;

                    // If nightbor is out of bounds or null, then we are on an edge
                    if (IsOutOfBounds(x, y, nodeGridSize) || chunkMap[x, y] == null)
                    {
                        // Set tile to beach and dip
                        chunk.Define(ChunkType.Beach);
                        break;
                    }
                }
            }
        }
    }

    public void GenerateTerrain(Chunk[,] chunkMap, int spawnChance)
    {
        // Randomly set tiles to open plains or mountains
        foreach (var chunk in chunkMap)
        {
            if (chunk == null)
                continue;

            // Roll to see if we spawn terrain
            if (Random.Range(0, 100) < spawnChance)
            {
                // 50/50 plains or mountain
                if (Random.Range(0, 2) == 0)
                {
                    chunk.Define(ChunkType.Plains);
                }
                else
                {
                    chunk.Define(ChunkType.Mountain);
                }
            }
        }
    }

    public void GenerateLayerOne(int minRadius, int maxRadius, int center, Chunk[,] chunkMap)
    {
        // Create camps and chests
        int numCamps = 2;
        int numChests = 2;

        int count = numCamps + numChests;
        float angleIncrement = 360f / count;
        float startingAngle = Random.Range(0f, 360f); // Random starting angle
        int offset = center;

        // Generate points
        bool isCamp = false;
        for (int i = 0; i < count; i++)
        {
            // Generate random point in tarus
            int radius = Random.Range(minRadius, maxRadius); // Random radius
            float angle = startingAngle + (i * angleIncrement);
            int x = offset + Mathf.RoundToInt(radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            int y = offset + Mathf.RoundToInt(radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3Int randomPoint = new Vector3Int(x, y);

            // Find the closest chunk to the random point
            float closestDistance = float.MaxValue;
            Chunk closestChunk = null;
            foreach (var chunk in chunkMap)
            {
                if (chunk == null)
                    continue;

                // If center is within radius
                float distance = Vector3Int.Distance(chunk.center, randomPoint);
                if (distance < closestDistance)
                {
                    // Update largest
                    closestDistance = distance;
                    closestChunk = chunk;
                }
            }

            // Define that chunk accordingly
            if (isCamp)
                closestChunk.Define(ChunkType.Camp);
            else
                closestChunk.Define(ChunkType.Chest);

            isCamp = !isCamp;
        }
    }

    public void GenerateLayerTwo(int minRadius, int maxRadius, int center, Chunk[,] chunkMap)
    {
        // Create dens and chests
        int numDens = 2;
        int numChests = 2;

        int count = numDens + numChests;
        float angleIncrement = 360f / count;
        float startingAngle = Random.Range(0f, 360f); // Random starting angle
        int offset = center;

        // Generate points
        bool isDen = false;
        for (int i = 0; i < count; i++)
        {
            // Generate random point in tarus
            int radius = Random.Range(minRadius, maxRadius); // Random radius
            float angle = startingAngle + (i * angleIncrement);
            int x = offset + Mathf.RoundToInt(radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            int y = offset + Mathf.RoundToInt(radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3Int randomPoint = new Vector3Int(x, y);

            // Find the closest chunk to the random point
            float closestDistance = float.MaxValue;
            Chunk closestChunk = null;
            foreach (var chunk in chunkMap)
            {
                if (chunk == null)
                    continue;

                // If center is within radius
                float distance = Vector3.Distance(chunk.center, randomPoint);
                if (distance < closestDistance)
                {
                    // Update largest
                    closestDistance = distance;
                    closestChunk = chunk;
                }
            }

            // Define that chunk accordingly
            if (isDen)
                closestChunk.Define(ChunkType.Den);
            else
                closestChunk.Define(ChunkType.Chest);

            isDen = !isDen;
        }
    }

    public void GenerateLayerThree(int minRadius, int maxRadius, int center, Chunk[,] chunkMap)
    {
        // Create dens and chests
        int numDens = 2;
        int numChests = 2;

        int count = numDens + numChests;
        float angleIncrement = 360f / count;
        float startingAngle = Random.Range(0f, 360f); // Random starting angle
        int offset = center;

        // Generate points
        bool isDen = false;
        for (int i = 0; i < count; i++)
        {
            // Generate random point in tarus
            int radius = Random.Range(minRadius, maxRadius); // Random radius
            float angle = startingAngle + (i * angleIncrement);
            int x = offset + Mathf.RoundToInt(radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            int y = offset + Mathf.RoundToInt(radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3Int randomPoint = new Vector3Int(x, y);

            // Find the closest chunk to the random point
            float closestDistance = float.MaxValue;
            Chunk closestChunk = null;
            foreach (var chunk in chunkMap)
            {
                if (chunk == null)
                    continue;

                // If center is within radius
                float distance = Vector3.Distance(chunk.center, randomPoint);
                if (distance < closestDistance)
                {
                    // Update largest
                    closestDistance = distance;
                    closestChunk = chunk;
                }
            }

            // Define that chunk accordingly
            if (isDen)
                closestChunk.Define(ChunkType.Den);
            else
                closestChunk.Define(ChunkType.Chest);

            isDen = !isDen;
        }
    }

    private bool IsInsideCircle(int x, int y, int p_x, int p_y, int radius)
    {
        return Mathf.Pow(x - p_x, 2) + Mathf.Pow(y - p_y, 2) < Mathf.Pow(radius, 2);
    }

    private bool SquareInsideCircle(Vector2 squareCenter, float squareSize, Vector2 circleCenter, float circleRadius, float cutoffPercentage)
    {
        float distance = Vector2.Distance(squareCenter, circleCenter);
        return distance + cutoffPercentage * (squareSize / 2) <= circleRadius;
    }

    private bool IsOutOfBounds(int x, int y, int bounds)
    {
        return x < 0 || x >= bounds || y < 0 || y >= bounds;
    }
}
