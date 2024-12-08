using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Debugging")]
    [SerializeField] private bool isAlly;

    private void Start()
    {
        isAlly = false;
    }
}
