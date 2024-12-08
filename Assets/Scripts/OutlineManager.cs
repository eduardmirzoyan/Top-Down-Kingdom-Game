using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OutlineManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private RuleTile ruleTile;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private Transform playerTransform;
    [SerializeField, ReadOnly] private Chunk chunk;

    public static OutlineManager instance;
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

    private void Start()
    {
        playerTransform = PlayerController.instance.transform;
    }

    public void SelectChunk(Chunk chunk)
    {
        if (this.chunk == null)
        {
            this.chunk = chunk;
        }
        else
        {

        }
    }
}
