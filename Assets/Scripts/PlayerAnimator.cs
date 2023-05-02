using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector2 movementDirection;
    public GameObject Sound;

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
            InvokeRepeating("PlayFootStepSound", 0.3f, 0.5f);
        }

        else
        {
            animator.SetBool("isWalking", false);
            CancelInvoke("PlayFootStepSound");
        }
    }
    public void PlayAttackAnimation(string attackDirection)
    {
        animator.Play(attackDirection);
    }


    private void PlayFootStepSound()
    {
        Sound.GetComponent<RandomSound>().PlayRandom1();
    }
}
