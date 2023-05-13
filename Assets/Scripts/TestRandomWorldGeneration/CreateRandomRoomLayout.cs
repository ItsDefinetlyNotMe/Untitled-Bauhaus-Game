using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static Structs;

namespace TestRandomWorldGeneration
{
    public class CreateRandomRoomLayout : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject wallLeft;
        [SerializeField] private GameObject wallRight;
        [SerializeField] private GameObject wallUp;
        [SerializeField] private GameObject wallDown;
        [SerializeField] private GameObject cornerLeftUp;
        [SerializeField] private GameObject cornerLeftDown;
        [SerializeField] private GameObject cornerRightUp;
        [SerializeField] private GameObject cornerRightDown;
        [SerializeField] private GameObject wallUpCornerRight;
        [SerializeField] private GameObject wallUpCornerLeft;
        [SerializeField] private GameObject wallUpCornerRightCornerLeft;
        [SerializeField] private GameObject wallDownCornerRight;
        [SerializeField] private GameObject wallDownCornerLeft;
        [SerializeField] private GameObject wallDownCornerRightCornerLeft;
        [SerializeField] private GameObject floor;
        
        [Header("Matrix")]
        [SerializeField] private int minNumberOfTiles;
        [SerializeField] private int maxNumberOfTiles;

        private int numberOfMaxTiles;
        private int numberOfActiveTiles = 1;

        //1 means floor tile,
        //2 means border tile,
        //3 means instantiated object,
        //4 means entry door,
        //5 means exit door.
        private float[,] tileMatrix;

        private List<Tuple<int, int>> newTiles = new();

        private CreateRandomRoomInterior createRandomRoomInterior;

    
        private void Start()
        {
            createRandomRoomInterior = gameObject.GetComponent<CreateRandomRoomInterior>();

            StartRoomGeneration(Direction.Up); //TODO delete this line when object is in normal scene with doors
        }

    
        /// <summary>
        /// Generate room with floor and walls
        /// </summary>
        public void StartRoomGeneration(Direction doorDirection)
        {
            ResetEverything();
        
            //Create matrix
            numberOfMaxTiles = UnityEngine.Random.Range(minNumberOfTiles, maxNumberOfTiles + 1);
            tileMatrix = new float[numberOfMaxTiles, numberOfMaxTiles];

            GenerateMatrix();

            InstantiateFloor();
            InstantiateWalls();

            SetDoorDirections(doorDirection);

            //create room interior
            createRandomRoomInterior.SetInteriorVariables(ref tileMatrix, numberOfMaxTiles);
        }

        private void SetDoorDirections(Direction entryDirection)
        {
            Direction exitDirection1;
            Direction exitDirection2;

            switch (entryDirection)
            {
                case Direction.Left:
                    entryDirection = Direction.Right;
                    break;

                case Direction.Up:
                    entryDirection = Direction.Down;
                    break;

                case Direction.Right:
                    entryDirection = Direction.Left;
                    break;

                case Direction.Down:
                    entryDirection = Direction.Up;
                    break;
            }

            do
            {
                exitDirection1 = (Direction)UnityEngine.Random.Range(0, 3);
            } while (exitDirection1 == entryDirection);
            
            do
            {
                exitDirection2 = (Direction)UnityEngine.Random.Range(0, 3);
            } while (exitDirection2 == entryDirection || exitDirection2 == exitDirection1);

            SearchDoorSpawnPosition(entryDirection, true);
            SearchDoorSpawnPosition(exitDirection1, false);
            SearchDoorSpawnPosition(exitDirection2, false);
        }

        private void SearchDoorSpawnPosition(Direction doorDirection, bool isEntry)
        {
            bool tileFound = false;
            List<Vector2Int> possibleDoorPositions = new();

            switch (doorDirection)
            {
                //search for upmost floor tiles
                case Direction.Left:
                    for (int x = 0; x < numberOfMaxTiles; x++)
                    {
                        for (int y = 0; y < numberOfMaxTiles; y++)
                        {
                            if ((int)tileMatrix[x, y] == 1)
                            {
                                tileFound = true;
                                possibleDoorPositions.Add(new Vector2Int(x, y));
                            }
                        }
                        if (tileFound)
                            break;
                    }
                    break;

                case Direction.Up:
                    for (int y = 0; y < numberOfMaxTiles; y++)
                    {
                        for (int x = 0; x < numberOfMaxTiles; x++)
                        {
                            if ((int)tileMatrix[x, y] == 1)
                            {
                                tileFound = true;
                                possibleDoorPositions.Add(new Vector2Int(x, y));
                            }
                        }
                        if (tileFound)
                            break;
                    }
                    break;

                case Direction.Right:
                    for (int x = numberOfMaxTiles - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < numberOfMaxTiles; y++)
                        {
                            if ((int)tileMatrix[x, y] == 1)
                            {
                                tileFound = true;
                                possibleDoorPositions.Add(new Vector2Int(x, y));
                            }
                        }
                        if (tileFound)
                            break;
                    }
                    break;

                case Direction.Down:
                    for (int y = numberOfMaxTiles - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < numberOfMaxTiles; x++)
                        {
                            if ((int)tileMatrix[x, y] == 1)
                            {
                                tileFound = true;
                                possibleDoorPositions.Add(new Vector2Int(x, y));
                            }
                        }
                        if (tileFound)
                            break;
                    }
                    break;
            }

            Vector2Int matDoorPos = possibleDoorPositions[UnityEngine.Random.Range(0, possibleDoorPositions.Count)];

            if (isEntry)
                tileMatrix[matDoorPos.x, matDoorPos.y] = 4;
            else
                tileMatrix[matDoorPos.x, matDoorPos.y] = 5;

            float worldX = matDoorPos.x - numberOfMaxTiles / 2;
            float worldY = matDoorPos.y - numberOfMaxTiles / 2;

            //TODO instantiate door prefabs
        }


        private void GenerateMatrix()
        {
            int x = numberOfMaxTiles / 2;
            int y = numberOfMaxTiles / 2;
            tileMatrix[x, y] = 1;
            newTiles.Add(new Tuple<int, int>(x, y));

            do
            {
                UpdateProbabilities();
                newTiles.Clear();
            }
            while (!AddTiles());
        }

    
        private void PrintTileMatrix()
        {
            print("---------------------TileMatrix--------------------");
            string printString = "";
            for (int x = 0; x < numberOfMaxTiles; x++)
            {
                for (int y = 0; y < numberOfMaxTiles; y++)
                {
                    printString += (tileMatrix[x, y]);
                    printString += "   ";
                }
                print(printString);
                printString = "";
            }
        }

        private void CountOnesInMatrix()
        {
            int floorTileCount = 0;

            for (int x = 0; x < numberOfMaxTiles; x++)
            {
                for (int y = 0; y < numberOfMaxTiles; y++)
                {
                    if ((int)tileMatrix[x, y] == 1)
                        floorTileCount++;
                }
            }

            print("current number of ones: " + floorTileCount);
        }

    
        /// <summary>
        /// Update probabilities around tiles
        /// </summary>
        private void UpdateProbabilities()
        {
            foreach (Tuple<int, int> tile in newTiles)
            {
                CalculateProbability(tile.Item1, tile.Item2 + 1);
                CalculateProbability(tile.Item1, tile.Item2 - 1);
                CalculateProbability(tile.Item1 + 1, tile.Item2);
                CalculateProbability(tile.Item1 - 1, tile.Item2);
            }
        }

    
        /// <summary>
        /// For matrix calculate probability of possible tiles
        /// </summary>
        private void CalculateProbability(int x, int y)
        {
            if ((int)tileMatrix[x, y] != 1)
            {
                float probability = 0;
                probability += tileMatrix[x - 1, y];
                probability += tileMatrix[x + 1, y];
                probability += tileMatrix[x, y - 1];
                probability += tileMatrix[x, y + 1];
                probability /= 4;
            
                //set chance for holes and make sure to not implicitly add floor tile
                if (probability >= 0.9f)
                {
                    probability = 0.9f;
                }
                tileMatrix[x, y] = probability;
            }
        }

    
        /// <summary>
        /// Add tiles to list newTiles
        /// </summary>
        private bool AddTiles()
        {
            for (int x = 0; x < numberOfMaxTiles; x++)
            {
                for (int y = 0; y < numberOfMaxTiles; y++)
                {
                    //stop if enough tiles are added
                    if (numberOfActiveTiles >= numberOfMaxTiles)
                        return true;
                
                    //skip if already floor
                    if ((int)tileMatrix[x, y] == 1)
                        continue;

                    //random number between 0 and 1 (inclusive)
                    float rand = UnityEngine.Random.value;

                    //add floor tile to matrix and newTiles list
                    if (tileMatrix[x, y] > rand)
                    {
                        numberOfActiveTiles++;
                        tileMatrix[x, y] = 1;
                        newTiles.Add(new Tuple<int, int>(x, y));
                    }
                }
            }
            return false;
        }

    
        /// <summary>
        /// Instantiate floor tile prefabs with world coordinates
        /// </summary>
        private void InstantiateFloor()
        {
            for (int x = 0; x < numberOfMaxTiles; x++)
            {
                for (int y = 0; y < numberOfMaxTiles; y++)
                {
                    if ((int)tileMatrix[x, y] == 1)
                    {
                        //reposition to the center of the scene
                        float worldX = x - numberOfMaxTiles / 2;
                        float worldY = y - numberOfMaxTiles / 2;

                        GameObject newFloorTile = Instantiate(floor, new Vector3(worldX, worldY, 0), Quaternion.identity);
                        newFloorTile.transform.parent = gameObject.transform;
                    }
                }
            }
        }

    
        private void ResetEverything()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            tileMatrix = null;
            numberOfActiveTiles = 1;
            numberOfMaxTiles = 0;
            newTiles.Clear();
        }

    
        /// <summary>
        /// Instantiate wall tile prefabs with world coordinates
        /// </summary>
        private void InstantiateWalls()
        {
            for (int x = 0; x < numberOfMaxTiles; x++)
            {
                for (int y = 0; y < numberOfMaxTiles; y++)
                {
                    if ((int)tileMatrix[x, y] == 1)
                    {
                        //reposition to the center of the scene
                        float worldX = x - numberOfMaxTiles / 2;
                        float worldY = y - numberOfMaxTiles / 2;
                    
                        bool upEmpty = (int)tileMatrix[x + 0, y + 1] != 1;
                        bool upRightEmpty = (int)tileMatrix[x + 1, y + 1] != 1;
                        bool rightEmpty = (int)tileMatrix[x + 1, y + 0] != 1;
                        bool downRightEmpty = (int)tileMatrix[x + 1, y - 1] != 1;
                        bool downEmpty = (int)tileMatrix[x + 0, y - 1] != 1;
                        bool downLeftEmpty = (int)tileMatrix[x - 1, y - 1] != 1;
                        bool leftEmpty = (int)tileMatrix[x - 1, y + 0] != 1;
                        bool upLeftEmpty = (int)tileMatrix[x - 1, y + 1] != 1;

                        //choose where to put wall prefabs
                        if (upEmpty)
                        {
                            if (upRightEmpty && upLeftEmpty)
                            {
                                GameObject newWallTile = Instantiate(wallUp, new Vector3(worldX, worldY + 1, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }
                            else if (!upRightEmpty && upLeftEmpty)
                            {
                                GameObject newWallTile = Instantiate(wallUpCornerRight, new Vector3(worldX, worldY + 1, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }
                            else if (upRightEmpty)
                            {
                                GameObject newWallTile = Instantiate(wallUpCornerLeft, new Vector3(worldX, worldY + 1, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }
                            else
                            {
                                GameObject newWallTile = Instantiate(wallUpCornerRightCornerLeft, new Vector3(worldX, worldY + 1, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }

                            if (upRightEmpty && rightEmpty)
                            {
                                GameObject newWallTile = Instantiate(cornerRightUp, new Vector3(worldX + 1, worldY + 1, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }

                            if (upLeftEmpty && leftEmpty)
                            {
                                GameObject newWallTile = Instantiate(cornerLeftUp, new Vector3(worldX - 1, worldY + 1, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }
                        }

                        if (downEmpty)
                        {
                            if (downRightEmpty && downLeftEmpty)
                            {
                                GameObject newWallTile = Instantiate(wallDown, new Vector3(worldX, worldY, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }
                            else if (!downRightEmpty && downLeftEmpty)
                            {
                                GameObject newWallTile = Instantiate(wallDownCornerRight, new Vector3(worldX, worldY, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }
                            else if (downRightEmpty)
                            {
                                GameObject newWallTile = Instantiate(wallDownCornerLeft, new Vector3(worldX, worldY, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }
                            else
                            {
                                GameObject newWallTile = Instantiate(wallDownCornerRightCornerLeft, new Vector3(worldX, worldY, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }

                            if (downRightEmpty && rightEmpty)
                            {
                                GameObject newWallTile = Instantiate(cornerRightDown, new Vector3(worldX + 1, worldY, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }

                            if (downLeftEmpty && leftEmpty)
                            {
                                GameObject newWallTile = Instantiate(cornerLeftDown, new Vector3(worldX - 1, worldY, 0), quaternion.identity);
                                newWallTile.transform.parent = gameObject.transform;
                            }
                        }

                        if (rightEmpty)
                        {
                            GameObject newWallTile = Instantiate(wallRight, new Vector3(worldX + 1, worldY, 0), quaternion.identity);
                            newWallTile.transform.parent = gameObject.transform;
                        }

                        if (leftEmpty)
                        {
                            GameObject newWallTile = Instantiate(wallLeft, new Vector3(worldX - 1, worldY, 0), quaternion.identity);
                            newWallTile.transform.parent = gameObject.transform;
                        }
                    }
                }
            }
        }
    }
}