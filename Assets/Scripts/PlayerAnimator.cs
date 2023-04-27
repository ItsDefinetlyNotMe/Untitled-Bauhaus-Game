using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector2 movementDirection;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnMovement(InputValue input)
    {
        movementDirection = input.Get<Vector2>();

        if (movementDirection.x != 0 || movementDirection.y != 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("X", movementDirection.x);
            animator.SetFloat("Y", movementDirection.y);
        }

        else
            animator.SetBool("isWalking", false);
    }
    public void PlayAttackAnimation(string attackDirection)
    {
        animator.Play(attackDirection);
    }
}
