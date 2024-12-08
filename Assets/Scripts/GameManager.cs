using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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
        WorldRenderer.instance.GenerateWorld();
        MapBoundaryManager.instance.GenerateBoundary();
        CameraManager.instance.Initialize();
    }
}
