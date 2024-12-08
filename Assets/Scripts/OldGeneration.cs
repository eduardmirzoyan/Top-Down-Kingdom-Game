using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldGeneration
{
    private int baseRadius;
    private int beachRadius;

    public OldGeneration(int baseRadius, int beachRadius)
    {
        this.baseRadius = baseRadius;
        this.beachRadius = beachRadius;
    }

    public int[,] GenerateWorld(int worldRadius, int padding, float clearingCeiling, float mountainFloor, float scale)
    {
        int acualSize = worldRadius + padding;

        // 0 - Water | 1 - Forest | 2 - Plains |  3 - Beach | 4 - Mountain
        int[,] grid = new int[acualSize * 2 + 1, acualSize * 2 + 1];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // Check if inside map
                if (IsInsideCircle(i, j, acualSize, acualSize, worldRadius))
                {
                    // Check if inside base
                    if (IsInsideCircle(i, j, acualSize, acualSize, baseRadius))
                    {
                        grid[i, j] = 2;
                    }

                    // Check if in beach
                    else if (IsOutsideCircle(i, j, acualSize, acualSize, worldRadius - beachRadius))
                    {
                        grid[i, j] = 3;
                    }

                    else
                    {
                        // Add random pockets of mountains and clearings
                        float noise = GetNoiseAtLocation(i, j, scale);
                        if (noise > mountainFloor)
                        {
                            // Set to mountain
                            grid[i, j] = 4;
                        }
                        else if (noise < clearingCeiling)
                        {
                            // Set to plain
                            grid[i, j] = 2;
                        }
                        else
                        {
                            // Set to forest
                            grid[i, j] = 1;
                        }
                    }
                }
            }
        }

        return grid;
    }

    public void GenerateLayerOne(int minRadius, int maxRadius, int[,] grid)
    {
        // Create camps and chests
        int numCamps = 2;
        int numChests = 2;

        int count = numCamps + numChests;
        float angleIncrement = 360f / count;
        float startingAngle = Random.Range(0f, 360f); // Random starting angle
        int offset = grid.GetLength(0) / 2;

        bool isCamp = false;
        for (int i = 0; i < count; i++)
        {
            int radius = Random.Range(minRadius, maxRadius); // Random radius

            float angle = startingAngle + (i * angleIncrement);
            int x = offset + Mathf.RoundToInt(radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            int y = offset + Mathf.RoundToInt(radius * Mathf.Sin(angle * Mathf.Deg2Rad));

            if (isCamp)
                grid[x, y] = 6;
            else
                grid[x, y] = 7;

            isCamp = !isCamp;
        }
    }

    public void GenerateLayerTwo(int minRadius, int maxRadius, int[,] grid)
    {
        // Create dens and chests
        int numDens = 2;
        int numChests = 2;

        int count = numDens + numChests;
        float angleIncrement = 360f / count;
        float startingAngle = Random.Range(0f, 360f); // Random starting angle
        int offset = grid.GetLength(0) / 2;

        bool isDen = false;
        for (int i = 0; i < count; i++)
        {
            int radius = Random.Range(minRadius, maxRadius); // Random radius

            float angle = startingAngle + (i * angleIncrement);
            int x = offset + Mathf.RoundToInt(radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            int y = offset + Mathf.RoundToInt(radius * Mathf.Sin(angle * Mathf.Deg2Rad));

            if (isDen)
                grid[x, y] = 8;
            else
                grid[x, y] = 7;

            isDen = !isDen;
        }
    }

    public void GenerateLayerThree(int minRadius, int maxRadius, int[,] grid)
    {
        // Create dens and chests
        int numDens = 2;
        int numChests = 2;

        int count = numDens + numChests;
        float angleIncrement = 360f / count;
        float startingAngle = Random.Range(0f, 360f); // Random starting angle
        int offset = grid.GetLength(0) / 2;

        bool isDen = false;
        for (int i = 0; i < count; i++)
        {
            int radius = Random.Range(minRadius, maxRadius); // Random radius

            float angle = startingAngle + (i * angleIncrement);
            int x = offset + Mathf.RoundToInt(radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            int y = offset + Mathf.RoundToInt(radius * Mathf.Sin(angle * Mathf.Deg2Rad));

            if (isDen)
                grid[x, y] = 8;
            else
                grid[x, y] = 7;

            isDen = !isDen;
        }
    }

    public void GenerateLayerFinal(int count, int radius, int[,] grid)
    {
        float angleIncrement = 360f / count;
        float startingAngle = Random.Range(0f, 360f); // Random starting angle
        int offset = grid.GetLength(0) / 2;

        for (int i = 0; i < count; i++)
        {
            float angle = startingAngle + (i * angleIncrement);
            int x = offset + Mathf.RoundToInt(radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            int y = offset + Mathf.RoundToInt(radius * Mathf.Sin(angle * Mathf.Deg2Rad));

            grid[x, y] = 5;
        }
    }

    private float GetNoiseAtLocation(float x, float y, float scale)
    {
        return Mathf.PerlinNoise(x / scale, y / scale);
    }

    private bool IsInsideCircle(int x, int y, int p_x, int p_y, int radius)
    {
        return Mathf.Pow(x - p_x, 2) + Mathf.Pow(y - p_y, 2) < Mathf.Pow(radius, 2);
    }

    private bool IsOutsideCircle(int x, int y, int p_x, int p_y, int radius)
    {
        return Mathf.Pow(x - p_x, 2) + Mathf.Pow(y - p_y, 2) >= Mathf.Pow(radius, 2);
    }
}
