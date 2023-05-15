using Enemies;
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
        private int difficulty = 10;

        private bool isFirstWave = true;

        public void StartEnemySpawning(ref float[,] newTileMatrix, int newFloorTileCount, int remainingSpace)
        {
            ResetEverything();

            tileMatrix = newTileMatrix;
            floorTileCount = newFloorTileCount;
            spaceToFill = (int)(remainingSpace * 0.5);

            GenerateEnemyList();
            InstantiateFirstWave();
        }


        private void Awake()
        {
            HittableEnemy.onEnemyDeath += SpawnOneEnemy;
        }

        private void InstantiateFirstWave()
        {
            int spaceFilled = 0;
            do
            {
                int index = UnityEngine.Random.Range(0, enemiesToSpawn.Count);
                Structs.EnemyPrefab newEnemy = enemiesToSpawn[index];
                int enemySize = newEnemy.size.x * newEnemy.size.y;

                if (enemySize > 1)
                    InstantiateBigEnemy(newEnemy);
                else
                    InstantiateSmallEnemy(newEnemy);

                spaceFilled += enemySize;

                enemiesToSpawn.RemoveRange(index, 1);

                if (enemiesToSpawn.Count == 0)
                    spaceFilled = spaceToFill;

            } while (spaceFilled < spaceToFill);

            isFirstWave = false;
        }

        private void SpawnOneEnemy()
        {
            if (enemiesToSpawn.Count == 0)
                return;

            int index = UnityEngine.Random.Range(0, enemiesToSpawn.Count);
            Structs.EnemyPrefab newEnemy = enemiesToSpawn[index];
            int enemySize = newEnemy.size.x * newEnemy.size.y;

            if (enemySize > 1)
                InstantiateBigEnemy(newEnemy);
            else
                InstantiateSmallEnemy(newEnemy);

            enemiesToSpawn.RemoveRange(index, 1);
        }


        /// <summary>
        /// Instantiate enemies of size 1x1
        /// </summary>
        /// <param name="enemyPrefab"> enemy to instantiate </param>
        private void InstantiateSmallEnemy(Structs.EnemyPrefab enemyPrefab)
        {
            //Generate possible position list
            GenerateSmallEnemyPositionList();

            if (possibleSpawnPositions.Count <= 0)
                return;

            //Instantiate enemy
            int posIndex = UnityEngine.Random.Range(0, possibleSpawnPositions.Count);
            GameObject newEnemy = Instantiate(enemyPrefab.prefab, possibleSpawnPositions[posIndex], Quaternion.identity);
            newEnemy.transform.parent = gameObject.transform;

            //Mark position in matrix
            BlockWorldPositionInMatrix(possibleSpawnPositions[posIndex]);
        }


        /// <summary>
        /// Instantiate enemies of bigger size
        /// </summary>
        /// <param name="enemyPrefab"> enemy to instantiate </param>
        private void InstantiateBigEnemy(Structs.EnemyPrefab enemyPrefab)
        {
            //Generate possible position list
            GenerateBigEnemyPositionList(enemyPrefab.size);

            if (possibleSpawnPositions.Count <= 0)
                return;

            //Instantiate enemy
            int posIndex = UnityEngine.Random.Range(0, possibleSpawnPositions.Count);
            GameObject newEnemy = Instantiate(enemyPrefab.prefab, possibleSpawnPositions[posIndex], Quaternion.identity);
            newEnemy.transform.parent = gameObject.transform;

            //Mark position in matrix
            for (int x = 0; x < enemyPrefab.size.x; x++)
            {
                for (int y = 0; y < enemyPrefab.size.y; y++)
                {
                    BlockWorldPositionInMatrix(new Vector2(possibleSpawnPositions[posIndex].x + x, possibleSpawnPositions[posIndex].y + y));
                }
            }
        }

        private void GenerateEnemyList()
        {
            int indexOfMainEnemy;
            int currentRoomDifficulty = 0;
            do
            {
                indexOfMainEnemy = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                mainEnemy = enemyPrefabs[indexOfMainEnemy];

            } while (!DoesEnemyFitInRoom(mainEnemy.size));

            int strengthToFillWithMainEnemy = (int)(difficulty * percentageOfMainEnemy);

            do
            {
                enemiesToSpawn.Add(mainEnemy);
                currentRoomDifficulty += mainEnemy.strength;
            } while (currentRoomDifficulty < strengthToFillWithMainEnemy);

            do
            {
                int indexOfSideEnemy = UnityEngine.Random.Range(0, enemyPrefabs.Length);
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
            } while (currentRoomDifficulty != difficulty);
        }

        /// <summary>
        /// Changes number of position in matrix to instantiated enemy (6)
        /// </summary>
        /// <param name="pos"> World position x,y of object </param>
        private void BlockWorldPositionInMatrix(Vector2 pos)
        {
            if (!isFirstWave)
                return;

            int matX = (int)(pos.x + floorTileCount / 2);
            int matY = (int)(pos.y + floorTileCount / 2);

            tileMatrix[matX, matY] = 6;
        }

        /// <summary>
        /// Generate all possible spawn positions for a given big enemy 
        /// </summary>
        /// <param name="size"> x,y of enemy size </param>
        private void GenerateBigEnemyPositionList(Vector2Int size)
        {
            //reset values
            possibleSpawnPositions.Clear();
            bool positionNotValid = false;

            //iterate over every tile in matrix
            for (int x = 0; x < floorTileCount - size.x; x++)
                for (int y = 0; y < floorTileCount - size.y; y++)
                {
                    //check if current tile is floor or border tile
                    if ((int)tileMatrix[x, y] == 1 || (int)tileMatrix[x, y] == 2 || (int)tileMatrix[x, y] == 5)
                    {
                        //iterate over every tile that would be occupied by the new prefab
                        for (int i = 0; i < size.x; i++)
                        {
                            for (int j = 0; j < size.y; j++)
                            {
                                //check for every tile if it is free
                                if (tileMatrix[x + i, y + j] < 1f || (int)tileMatrix[x + i, y + j] == 3 || (int)tileMatrix[x + i, y + j] == 4)
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

                        //if you checked every tile for current enemy position, add spawnPosition to list
                        if (!positionNotValid)
                        {
                            float worldX = x - floorTileCount / 2; //reposition to the center of the scene
                            float worldY = y - floorTileCount / 2;

                            possibleSpawnPositions.Add(new Vector2(worldX, worldY));
                        }

                        //reset bool
                        positionNotValid = false;
                    }
                }
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

                        //if you checked every tile for current enemy position, add spawnPosition to list
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

        private void ResetEverything()
        {
            enemiesToSpawn.Clear();
            isFirstWave = true;
        }

        private void OnDisable()
        {
            HittableEnemy.onEnemyDeath -= SpawnOneEnemy;
        }


    }
}