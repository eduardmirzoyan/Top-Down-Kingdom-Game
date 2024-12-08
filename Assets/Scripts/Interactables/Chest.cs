using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private bool isOpen;

    private void Awake()
    {
        isOpen = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen)
        {
            print("OPENED!");

            animator.Play("Open");

            isOpen = true;
        }

    }
}
