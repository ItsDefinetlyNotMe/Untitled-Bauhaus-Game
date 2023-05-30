using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static Structs.Direction;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
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
    }

    public void Movement(InputValue input)
    {
        movementDirection = input.Get<Vector2>();

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
        //Debug.Log(localAttackDirection);
        animator.Play(localAttackDirection);
    }


    private void PlayFootStepSound()
    {
        sound.GetComponent<RandomSound>().PlayRandom1();
    }
}
