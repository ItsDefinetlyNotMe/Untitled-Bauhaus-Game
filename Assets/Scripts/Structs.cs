using System;
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
    //[Header("RandomRoomGeneration")]
    [Serializable]
    public struct InteriorPrefab
    {
        public GameObject prefab;
        public Vector2Int size;
        public bool possibleMainObject;
        public Vector2 offset;
    }
    public enum Type
    {
        Enemy,
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
        public Type type;
    }
}