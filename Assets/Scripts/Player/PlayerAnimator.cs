using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static Structs.Direction;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Animator wooshAnimator;
    private Vector2 movementDirection;
    [FormerlySerializedAs("Sound")] [SerializeField] private GameObject sound;
    private bool isInvoked = true;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Y = Animator.StringToHash("Y");
    public GameObject NormalHit;
    public GameObject HeavyHit;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        wooshAnimator = transform.Find("Woosh").gameObject.GetComponent<Animator>();
    }

    public void Movement(InputValue input)
    {
        movementDirection = input.Get<Vector2>().normalized;

        if (movementDirection.x != 0 || movementDirection.y != 0)
        {
            if (isInvoked) { 
                InvokeRepeating(nameof(PlayFootStepSound), 0.3f, 0.5f);
                isInvoked = false;
            }
            animator.SetBool(IsWalking, true);
            animator.SetFloat(X, movementDirection.x);
            animator.SetFloat(Y, movementDirection.y);
        }

        else
        {
            animator.SetBool(IsWalking, false);
            isInvoked = true;
            CancelInvoke(nameof(PlayFootStepSound));
        }
    }
    public void PlayAttackAnimation(Structs.Direction attackDirection,int number)
    {
        string localAttackDirection = "";
        switch(attackDirection)
        {
            case Left:
                localAttackDirection = "AttackLeft";
                NormalHit.GetComponent<RandomSound>().PlayRandom1();
                break;
            case Up:
                localAttackDirection = "AttackUp";
                NormalHit.GetComponent<RandomSound>().PlayRandom1();
                break;
            case Right:
                localAttackDirection = "AttackRight";
                NormalHit.GetComponent<RandomSound>().PlayRandom1();
                break;
            case Down:
                localAttackDirection = "AttackDown";
                NormalHit.GetComponent<RandomSound>().PlayRandom1();
                break;
        }
        localAttackDirection += number.ToString();

        animator.Play(localAttackDirection);

        // Play woosh animation
        wooshAnimator.Play(Regex.Replace(localAttackDirection, "Attack", "Woosh"));
    }

    public void PlayHeavyAttackAnimation(Structs.Direction attackDirection)
    {
        string localAttackDirection = "";
        switch(attackDirection)
        {
            case Left:
                localAttackDirection = "AxeHeavyAttackLeft";
                break;
            case Up:
                localAttackDirection = "AxeHeavyAttackUp";
                break;
            case Right:
                localAttackDirection = "AxeHeavyAttackRight";
                break;
            case Down:
                localAttackDirection = "AxeHeavyAttackDown";
                break;
        }
        animator.Play(localAttackDirection);
    }

    public void SetDirection(Structs.Direction direction)
    {
        animator.SetBool("Left", false);
        animator.SetBool("Up", false);
        animator.SetBool("Right", false);
        animator.SetBool("Down", false);
        switch (direction)
        {
            case Left:
                animator.SetBool("Left", true);
                break;
            case Up:
                animator.SetBool("Up", true);
                break;
            case Right:
                animator.SetBool("Right", true);
                break;
            case Down:
                animator.SetBool("Down", true);
                break;
        }
    }



    private void PlayFootStepSound()
    {
        sound.GetComponent<RandomSound>().PlayRandom1();
    }
}
