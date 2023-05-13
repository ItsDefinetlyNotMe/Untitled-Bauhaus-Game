using UnityEngine;

public class Structs : MonoBehaviour
{
    public enum Direction
    {
        Left, Up, Right, Down
    }
    public enum PlayerState{
        Moving,Dashing,Attacking
    }
    public enum EnemyState{
        ChargingAttack,Attacking,Recharging,Moving,Fleeing
    }
}
