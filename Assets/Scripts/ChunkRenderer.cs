using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TilemapRenderer tilemapRenderer;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private RuleTile highlightTile;
    [SerializeField] private Collider2D collider2d;

    [Header("Data")]
    [SerializeField, ReadOnly] private Chunk chunk;

    public void Initialize(Chunk chunk)
    {
        this.chunk = chunk;

        foreach (var position in chunk.containedPositions)
        {
            tilemap.SetTile(position, highlightTile);
            tilemap.SetTileFlags(position, TileFlags.None);
            tilemap.SetColor(position, chunk.debugColor);
        }

        tilemapRenderer.enabled = false;

        name = $"Chunk [{chunk.gridPosition}]";
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        print($"Touching Chunk: {chunk.gridPosition}, Distance {other.Distance(collider2d).distance}");

        // Select this chunk
        // OutlineManager.instance.SelectChunk(chunk);
    }

    private void OnDrawGizmosSelected()
    {
        if (chunk != null)
        {
            Gizmos.color = Color.red;
            foreach (var position in chunk.GetPerimeter())
            {
                Vector3 pos = (Vector3)position + new Vector3(0.5f, 0.5f);
                Gizmos.DrawWireSphere(pos, 0.25f);
            }
        }
    }
}
