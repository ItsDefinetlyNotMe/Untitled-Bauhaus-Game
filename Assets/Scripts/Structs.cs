using System;
using UnityEngine;

public class Structs : MonoBehaviour
{
    public enum Direction
    {
        Left, Up, Right, Down
    }
    public enum PlayerState{
        Moving,Dashing,Attacking,Charging
    }
    public enum EnemyState{
        ChargingAttack,Attacking,Recharging,Moving,Fleeing,Idle
    }

    public enum ThorState
    {
        ChargeAttack,
        ThrowHammer,
        SummonLightning,
        HammerSlamAttack,
        GenerateHammer,
        GetUp,
        Moving,
        BaseAttack,
        Phase2Start
    }

    //[Header("RandomRoomGeneration")]
    [Serializable]
    public struct InteriorPrefab
    {
        public GameObject prefab;
        public Vector2Int size;
        public bool possibleMainObject;
        public Vector2 offset;
        public Type type;
        public int difficulty;
    }
    public enum Type
    {
        Interior,
        Trap
    }

    public enum DifficultyType
    {
        Valhalla,
        Helheim
    }

    [Serializable]
    public struct DifficultyStruct
    {
        public DifficultyType difficultyType;
        public int difficulty;
    }
    //[Header("EnemySpawn")]
    [Serializable]
    public struct EnemyPrefab
    {
        public GameObject prefab;
        public int strength;
        public Vector2Int size;
    }
}
