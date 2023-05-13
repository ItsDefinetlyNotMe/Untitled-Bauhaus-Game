using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestRandomWorldGeneration {

    public enum Type
    {
        ENEMY,
        TRAP
    }
    
    [Serializable]
    public struct EnemyPrefab
    {
        public GameObject prefab;
        public int strength;
        public Vector2Int size;
        public Type type;
    }

    public class SpawnRandomEnemies : MonoBehaviour
    {
        private float[,] tileMatrix;
        [SerializeField] private List<EnemyPrefab> enemyPrefabs = new();
        

        public void StartEnemySpawning(ref float[,] newTileMatrix)
        {
            tileMatrix = newTileMatrix;
        }

        
    }
}