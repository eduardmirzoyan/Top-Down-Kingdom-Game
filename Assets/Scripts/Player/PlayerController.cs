using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum ActionState { Idle, Walk, Run }
    private enum FacingDirection { Up, Down, Left, Right }

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidbody2d;

    [Header("Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [Header("Debugging")]
    [SerializeField] private ActionState actionState;
    [SerializeField] private FacingDirection facingDirection;

    public static PlayerController instance;
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
        // Start idle and facing down
        actionState = ActionState.Idle;
        facingDirection = FacingDirection.Down;

        // Play animation based on state
        animator.Play($"{actionState} {facingDirection}");
    }

    public void Move(Vector2 direction, bool isSprint)
    {
        if (direction.magnitude > 1)
            throw new System.Exception("MAG IS GREATER THAN 1");

        if (direction == Vector2.zero)
        {
            // Stop moving
            rigidbody2d.velocity = Vector2.zero;

            // Change state
            actionState = ActionState.Idle;
        }
        else if (isSprint)
        {
            // Face moving direction
            FaceDirection(direction);

            // Move character
            rigidbody2d.velocity = direction * runSpeed;

            // Change state
            actionState = ActionState.Run;
        }
        else
        {
            // Face moving direction
            FaceDirection(direction);

            // Move character
            rigidbody2d.velocity = direction * walkSpeed;

            // Change state
            actionState = ActionState.Walk;
        }

        // Play animation based on state
        animator.Play($"{actionState} {facingDirection}");
    }

    private void FaceDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            facingDirection = FacingDirection.Right;
        }
        else if (direction.x < 0)
        {
            facingDirection = FacingDirection.Left;
        }
        else if (direction.y > 0)
        {
            facingDirection = FacingDirection.Up;
        }
        else if (direction.y < 0)
        {
            facingDirection = FacingDirection.Down;
        }
    }
}
