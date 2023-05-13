using System.Collections.Generic;
using UnityEngine;

namespace TestRandomWorldGeneration {
    public class SpawnRandomEnemies : MonoBehaviour
    {
        [Header("Enemy")]
        [SerializeField] private Structs.EnemyPrefab[] enemyPrefabs;
        [SerializeField] private float percentageOfMainEnemy;
        
        [Header("Difficulty")]
        [SerializeField] private Structs.DifficultyStruct[] difficulties;

        private List<Vector2> possibleSpawnPositions = new();
        private List<Structs.EnemyPrefab> enemiesToSpawn = new();
        private Structs.EnemyPrefab mainEnemy;

        [Header("Matrix")]
        private float[,] tileMatrix;

        private int floorTileCount;
        private int spaceToFill;
        private int indexOfMainEnemy;
        private int indexOfSideEnemy;
        private int strengthToFillWithMainEnemy;
        private int deadEnemiesCounter = 0;
        private int difficulty = 30;
        private int currentRoomDifficulty;


        public void StartEnemySpawning(ref float[,] newTileMatrix, int newFloorTileCount, int remainingSpace)
        {
            tileMatrix = newTileMatrix;
            floorTileCount = newFloorTileCount;
            spaceToFill = (int)(remainingSpace * 0.5);

            do
            {
                indexOfMainEnemy = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                mainEnemy = enemyPrefabs[indexOfMainEnemy];

            } while (!DoesEnemyFitInRoom(mainEnemy.size));

            strengthToFillWithMainEnemy = (int)(difficulty * percentageOfMainEnemy);

            do
            {
                enemiesToSpawn.Add(mainEnemy);
                currentRoomDifficulty += mainEnemy.strength;
            } while (currentRoomDifficulty < strengthToFillWithMainEnemy);

            do
            {
                indexOfSideEnemy = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                if (indexOfSideEnemy == indexOfMainEnemy)
                    continue;

                Structs.EnemyPrefab sideEnemy = enemyPrefabs[indexOfSideEnemy];

                if (currentRoomDifficulty + sideEnemy.strength <= difficulty)
                {
                    if (DoesEnemyFitInRoom(sideEnemy.size))
                    {
                        enemiesToSpawn.Add(sideEnemy);
                        currentRoomDifficulty += sideEnemy.strength;
                    }
                }
            }while (currentRoomDifficulty != difficulty);
        }

        //TODO call from enemy script when enemy dies
        public void IncreseDeadEnemiesCounter()
        {
            deadEnemiesCounter++;
        }

        /// <summary>
        /// Generate all possible spawn positions for a given small enemy 
        /// </summary>
        private void GenerateSmallEnemyPositionList()
        {
            possibleSpawnPositions.Clear();

            for (int x = 0; x < floorTileCount; x++)
            {
                for (int y = 0; y < floorTileCount; y++)
                {
                    //valid positions are floor, border and in front of exit doors
                    if ((int)tileMatrix[x, y] == 1 || (int)tileMatrix[x, y] == 2 || (int)tileMatrix[x, y] == 5)
                    {
                        float worldX = x - floorTileCount / 2; //reposition to the center of the scene
                        float worldY = y - floorTileCount / 2;

                        possibleSpawnPositions.Add(new Vector2(worldX, worldY));
                    }
                }
            }
        }

        /// <summary>
        /// Generate all possible spawn positions for a given big enemy
        /// </summary>
        /// <param name="size"> x,y of enemy size </param>
        private bool DoesEnemyFitInRoom(Vector2Int size)
        {
            if (size.x * size.y == 1)
                return true;

            //reset values
            bool positionNotValid = false;

            //iterate over every tile in matrix
            for (int x = 0; x < floorTileCount - size.x; x++)
                for (int y = 0; y < floorTileCount - size.y; y++)
                {
                    //check if current tile is floor, border tile or in front of exit door
                    if ((int)tileMatrix[x, y] == 1 || (int)tileMatrix[x, y] == 2 || (int)tileMatrix[x, y] == 5)
                    {
                        //iterate over every tile that would be occupied by the new prefab
                        for (int i = 0; i < size.x; i++)
                        {
                            for (int j = 0; j < size.y; j++)
                            {
                                //check for every tile if it is free
                                if (tileMatrix[x + i, y + j] < 1f || (int)tileMatrix[x + i, y + j] == 3 || (int)tileMatrix[x, y] == 4)
                                {
                                    //if it is not free, check next tile
                                    positionNotValid = true;
                                    break;
                                }
                            }

                            //if it is not free, check next tile
                            if (positionNotValid)
                                break;
                        }

                        //if you checked every tile for current object position, add spawnPosition to list
                        if (!positionNotValid)
                        {
                            return true;
                        }

                        //reset bool
                        positionNotValid = false;
                    }
                }
            return false;
        }

        private void ResetEverythin()
        {
            deadEnemiesCounter = 0;
        }



        
    }
}