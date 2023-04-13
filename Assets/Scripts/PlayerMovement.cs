using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float defaultMoveSpeed = 11f;

    private Vector2 movementDirection;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMovement(InputValue input)
    {
        movementDirection = input.Get<Vector2>();
        rb.velocity = movementDirection * defaultMoveSpeed;
    }
}
