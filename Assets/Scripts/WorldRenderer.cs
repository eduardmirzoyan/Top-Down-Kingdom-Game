using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap worldTilemap;
    [SerializeField] private Tilemap debugTilemap;
    [SerializeField] private Tile whiteTile;
    [SerializeField] private Color waterColor;
    [SerializeField] private Color plainsColor;
    [SerializeField] private Color forestColor;
    [SerializeField] private Color beachColor;
    [SerializeField] private Color mountainColor;
    [SerializeField] private Color chestColor;
    [SerializeField] private Color campColor;
    [SerializeField] private Color denColor;
    [SerializeField] private Color baseColor;

    [Header("Tiles")]
    [SerializeField] private Tilemap oceanTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap plainsTilemap;
    [SerializeField] private Tilemap forestTilemap;
    [SerializeField] private Tilemap mountainTilemap;
    [SerializeField] private AnimatedTile waterTile;
    [SerializeField] private RuleTile groundTile;
    [SerializeField] private RuleTile grassTile;
    [SerializeField] private RuleTile darkGrassTile;
    [SerializeField] private RuleTile rockTile;

    [Header("World Settings")]
    [SerializeField] private int mapRadius;
    [SerializeField] private int mapPadding;

    [Header("Basic Settings")]
    [SerializeField] private int baseRadius;

    [Header("Layer Settings")]
    [SerializeField] private Vector2Int layerOneRadius;
    [SerializeField] private Vector2Int layerTwoRadius;
    [SerializeField] private Vector2Int layerThreeRadius;

    [Header("Chunk Settings")]
    [SerializeField] private int chunkSize;
    [SerializeField] private float cutoffPercentage;
    [SerializeField] private int terrainChance;

    [Header("Prefabs")]
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private GameObject chestPrefab;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private ProceduralGeneration proceduralGeneration;

    public static WorldRenderer instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void GenerateWorld()
    {
        // Create outline background first
        int worldSize = 2 * (mapRadius + mapPadding);
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                Vector3Int position = new Vector3Int(i, j);

                // Create ocean
                oceanTilemap.SetTile(position, waterTile);

                // If on map, set ground
                if (IsInsideCircle(i, j, worldSize / 2, worldSize / 2, mapRadius))
                {
                    groundTilemap.SetTile(position, groundTile);
                }

                // worldTilemap.SetColor(position, waterColor);
                // worldTilemap.SetTile(position, whiteTile);
            }
        }

        // Generate world
        proceduralGeneration = new ProceduralGeneration();
        var chunkMap = proceduralGeneration.GenerateWorld(mapRadius, mapPadding, chunkSize, cutoffPercentage);
        proceduralGeneration.GenerateTerrain(chunkMap, terrainChance);
        proceduralGeneration.GenerateBase(chunkMap, baseRadius, new Vector3Int(worldSize / 2, worldSize / 2));
        proceduralGeneration.GenerateBeach(chunkMap);
        proceduralGeneration.GenerateLayerOne(layerOneRadius.x, layerOneRadius.y, mapRadius + mapPadding, chunkMap);
        proceduralGeneration.GenerateLayerTwo(layerTwoRadius.x, layerTwoRadius.y, mapRadius + mapPadding, chunkMap);
        proceduralGeneration.GenerateLayerThree(layerThreeRadius.x, layerThreeRadius.y, mapRadius + mapPadding, chunkMap);

        // Render world
        RenderChunks(chunkMap);

        if (debugMode)
            DebugRenderChunks(chunkMap);
    }

    private void RenderChunks(Chunk[,] chunks)
    {
        foreach (var chunk in chunks)
        {
            // Don't consider null chunks
            if (chunk == null)
                continue;

            // Render Tiles
            RenderTiles(chunk);

            // Fill with content
            FillChunk(chunk);
        }
    }

    private void DebugRenderChunks(Chunk[,] chunks)
    {
        foreach (var chunk in chunks)
        {
            // Don't consider null chunks
            if (chunk == null)
                continue;

            foreach (var position in chunk.containedPositions)
            {
                // Set color based on debug mode
                debugTilemap.SetTile(position, whiteTile);
                debugTilemap.SetTileFlags(position, TileFlags.None);
                debugTilemap.SetColor(position, chunk.debugColor);
            }

            // Mark anchor
            Vector3Int anchorPosition = chunk.anchorPosition;
            debugTilemap.SetColor(anchorPosition, Color.black);

            // Mark center
            Vector3Int centerPosition = chunk.center;
            debugTilemap.SetColor(centerPosition, Color.white);
        }
    }

    private void RenderTiles(Chunk chunk)
    {
        switch (chunk.chunkType)
        {
            case ChunkType.Plains:

                foreach (var position in chunk.containedPositions)
                {
                    plainsTilemap.SetTile(position, grassTile);
                }

                break;
            case ChunkType.Forest:

                foreach (var position in chunk.containedPositions)
                {
                    plainsTilemap.SetTile(position, grassTile);
                    forestTilemap.SetTile(position, darkGrassTile);
                }

                break;
            case ChunkType.Mountain:

                foreach (var position in chunk.containedPositions)
                {
                    forestTilemap.SetTile(position, darkGrassTile);
                    mountainTilemap.SetTile(position, rockTile);
                }

                break;
            case ChunkType.Beach:

                // Do nothing

                break;
            case ChunkType.Base:

                // TODO

                break;
            case ChunkType.Camp:

                // TODO

                break;
            case ChunkType.Chest:

                // TODO

                break;
            case ChunkType.Den:

                // TODO

                break;
        }

        // Create object
        Instantiate(chunkPrefab, oceanTilemap.transform).GetComponent<ChunkRenderer>().Initialize(chunk);
    }


    private void FillChunk(Chunk chunk)
    {
        switch (chunk.chunkType)
        {
            case ChunkType.Forest:

                // Fill with trees

                break;
            case ChunkType.Base:

                // TODO

                break;
            case ChunkType.Camp:

                // TODO

                break;
            case ChunkType.Chest:

                var chunkCenter = chunk.center;
                var centerWorld = groundTilemap.GetCellCenterWorld(chunkCenter);
                Instantiate(chestPrefab, centerWorld, Quaternion.identity, groundTilemap.transform);

                break;
            case ChunkType.Den:

                // TODO

                break;
        }
    }

    public void ClearWorld()
    {
        worldTilemap.ClearAllTiles();
        debugTilemap.ClearAllTiles();
        oceanTilemap.ClearAllTiles();
        groundTilemap.ClearAllTiles();
        plainsTilemap.ClearAllTiles();
        forestTilemap.ClearAllTiles();
        mountainTilemap.ClearAllTiles();
    }

    private void OnDrawGizmosSelected()
    {
        int worldRadius = mapRadius + mapPadding;
        var position = transform.position + new Vector3(worldRadius, worldRadius);

        // Draw bounds for map
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, mapRadius);

        // Draw bounds for world
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(position, Vector3.one * 2 * worldRadius);
        Gizmos.DrawWireSphere(position, worldRadius);

        // Draw bounds for starting area
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, baseRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(position, layerOneRadius.x);
        Gizmos.DrawWireSphere(position, layerOneRadius.y);

        Gizmos.DrawWireSphere(position, layerTwoRadius.x);
        Gizmos.DrawWireSphere(position, layerTwoRadius.y);

        Gizmos.DrawWireSphere(position, layerThreeRadius.x);
        Gizmos.DrawWireSphere(position, layerThreeRadius.y);

        // Show node boxes

        int tilesPerNode = 2 * mapRadius / chunkSize;

        float offset = mapPadding + tilesPerNode / 2;
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                Vector3 location = new Vector3(i * tilesPerNode + offset, j * tilesPerNode + offset);
                if (SquareInsideCircle(location, tilesPerNode, new Vector2(worldRadius, worldRadius), mapRadius, cutoffPercentage))
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(location, tilesPerNode * Vector3.one);
                }
                else
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(location, tilesPerNode * Vector3.one);
                }

            }
        }
    }

    private bool IsInsideCircle(float x, float y, float p_x, float p_y, float radius)
    {
        return Mathf.Pow(x - p_x, 2) + Mathf.Pow(y - p_y, 2) < Mathf.Pow(radius, 2);
    }

    private bool SquareInsideCircle(Vector2 squareCenter, float squareSize, Vector2 circleCenter, float circleRadius, float cutoffPercentage)
    {
        float distance = Vector2.Distance(squareCenter, circleCenter);
        return distance + cutoffPercentage * (squareSize / 2) <= circleRadius;
    }
}
