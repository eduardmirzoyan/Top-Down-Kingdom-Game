using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField, ReadOnly] private PlayerController playerController;
    [SerializeField, ReadOnly] private Vector2 moveDirection;
    [SerializeField, ReadOnly] private bool isSprinting;

    private void Start()
    {
        moveDirection = Vector2.zero;

        playerController = PlayerController.instance;
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        playerController.Move(moveDirection, isSprinting);
    }
}
