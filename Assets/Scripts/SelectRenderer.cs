using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap worldTilemap;
    [SerializeField] private RuleTile highlightRuleTile;
    [SerializeField] private Transform target;

    [Header("Debugging")]
    [SerializeField] private Chunk currentChunk;

    private void Awake()
    {
        target = null;
        currentChunk = null;
    }

    private void Update()
    {
        if (target != null)
        {

        }
    }
}
