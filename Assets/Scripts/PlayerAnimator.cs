using UnityEngine;
using UnityEngine.InputSystem;
using static Direction;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector2 movementDirection;
    public GameObject Sound;
    private bool isInvoked = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnMovement(InputValue input)
    {
        movementDirection = input.Get<Vector2>();

        if (movementDirection.x != 0 || movementDirection.y != 0)
        {
            if (isInvoked) { 
                InvokeRepeating("PlayFootStepSound", 0.3f, 0.5f);
                isInvoked = false;
            }
            animator.SetBool("isWalking", true);
            animator.SetFloat("X", movementDirection.x);
            animator.SetFloat("Y", movementDirection.y);
        }

        else
        {
            animator.SetBool("isWalking", false);
            isInvoked = true;
            CancelInvoke("PlayFootStepSound");
        }
    }
    public void PlayAttackAnimation(Direction attackDirection,int number)
    {
        string localAttackDirection = "";
        switch(attackDirection)
        {
            case LEFT:
                localAttackDirection = "AttackLeft";
                break;
            case UP:
                localAttackDirection = "AttackUp";
                break;
            case RIGHT:
                localAttackDirection = "AttackRight";
                break;
            case DOWN:
                localAttackDirection = "AttackDown";
                break;
        }
        localAttackDirection += number.ToString();
        Debug.Log(localAttackDirection);
        animator.Play(localAttackDirection);
    }


    private void PlayFootStepSound()
    {
        Sound.GetComponent<RandomSound>().PlayRandom1();
    }
}
