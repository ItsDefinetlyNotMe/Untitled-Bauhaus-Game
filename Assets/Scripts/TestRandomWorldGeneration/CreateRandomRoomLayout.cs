using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using static Structs;
using System.Text.RegularExpressions;

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
        [SerializeField] private GameObject leftDoor;
        [SerializeField] private GameObject upDoor;
        [SerializeField] private GameObject rightDoor;
        [SerializeField] private GameObject downDoor;

        [Header("Collectables")]
        [SerializeField] private List<GameObject> UICollectables;
        [SerializeField] private List<GameObject> collectables;
        //[SerializeField] private GameObject maxHealthUp;
        //[SerializeField] private GameObject heal;
        //[SerializeField] private GameObject damageUp;

        [Header("Matrix")]
        [SerializeField] private int minNumberOfTiles;
        [SerializeField] private int maxNumberOfTiles;

        private int numberOfMaxTiles;
        private int numberOfActiveTiles = 1;

        //1 means floor tile,
        //2 means border tile,
        //3 means instantiated object,
        //4 means entry entryDoor,
        //5 means exit exitDoor.
        private float[,] tileMatrix;

        private List<Tuple<int, int>> newTiles = new();

        private CreateRandomRoomInterior createRandomRoomInterior;
        private GameObject player;

        public string loot { private get; set; }
        
        [Header("Door")]
        private List<GameObject> exitDoors = new List<GameObject>();

        public delegate void onRoomGeneratedDelegate();
        public static onRoomGeneratedDelegate onRoomGenerated;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Funtion to make complete room generation
        /// </summary>
        public void StartRoomGeneration(Direction doorDirection)
        {
            ResetEverything();
        
            //Create matrix
            numberOfMaxTiles = UnityEngine.Random.Range(minNumberOfTiles, maxNumberOfTiles + 1);
            tileMatrix = new float[numberOfMaxTiles, numberOfMaxTiles];
            tileMatrix[(int)numberOfMaxTiles / 2, (int)numberOfMaxTiles / 2] = 7;

            GenerateMatrix();
            InstantiateFloor();
            InstantiateWalls();

            SetDoorDirections(doorDirection);

            SetLootForNextRoom();

            //create room interior
            createRandomRoomInterior.SetInteriorVariables(ref tileMatrix, numberOfMaxTiles);

            onRoomGenerated?.Invoke();
        }

        public void OpenDoors()
        {
            foreach (GameObject collectable in collectables)
            {
                if (collectable.name == loot)
                {
                    Instantiate(collectable, Vector2.zero, Quaternion.identity);
                    break;
                }
            }

            //activate doors
            foreach (GameObject door in exitDoors)
            {
                door.GetComponent<Animator>().SetTrigger("open");
                door.GetComponent<Door>().ActivateDoor();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Valhalla")
            {
                loot = Regex.Replace(UICollectables[UnityEngine.Random.Range(0, UICollectables.Count)].name, "UI", "Collectable");

                createRandomRoomInterior = gameObject.GetComponent<CreateRandomRoomInterior>();

                player = GameObject.Find("/Character");

                StartRoomGeneration(Direction.Right);
            }
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
                exitDirection1 = (Direction)UnityEngine.Random.Range(0, 4);
            } while (exitDirection1 == entryDirection);
            
            do
            {
                exitDirection2 = (Direction)UnityEngine.Random.Range(0, 4);
            } while (exitDirection2 == entryDirection || exitDirection2 == exitDirection1);

            SearchDoorSpawnPosition(entryDirection, true);
            SearchDoorSpawnPosition(exitDirection1, false);
            SearchDoorSpawnPosition(exitDirection2, false);
        }

        private void SearchDoorSpawnPosition(Direction doorDirection, bool isEntry)
        {
            bool tileFound = false;
            List<Vector2Int> possibleDoorPositions = new();
            GameObject doorPrefab = rightDoor; 

            Vector3 doorOffset = Vector3.zero;

            switch (doorDirection)
            {
                //search for leftmost floor tiles
                case Direction.Left:
                    doorOffset = new Vector3(-1, 0, 0);
                    doorPrefab = leftDoor;
                    
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
                    doorPrefab = upDoor;
                    doorOffset = new Vector3(0, 1, 0);

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

                case Direction.Right:
                    doorOffset = new Vector3(1, 0, 0);
                    doorPrefab = rightDoor;
                    
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
                    doorOffset = new Vector3(0, 0, 0);
                    doorPrefab = downDoor;

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
            }

            int randomDoorIndex = UnityEngine.Random.Range(0, possibleDoorPositions.Count);
            Vector2Int matDoorPos = possibleDoorPositions[randomDoorIndex];

            float worldX = matDoorPos.x - numberOfMaxTiles / 2;
            float worldY = matDoorPos.y - numberOfMaxTiles / 2;

            Vector3 playerPos = new Vector3(worldX,worldY,0f);
            Vector3 doorPos = playerPos + doorOffset;

            GameObject newDoor = new();
            
            tileMatrix[matDoorPos.x, matDoorPos.y] = 4;
            newDoor = Instantiate(doorPrefab, doorPos, Quaternion.identity);
            newDoor.transform.parent = gameObject.transform;
            
            if (isEntry)
            {
                player.transform.position = playerPos;
            }
            else
            {
                exitDoors.Add(newDoor);
            }
        }

        private void SetLootForNextRoom()
        {
            foreach (GameObject door in exitDoors)
            {
                Door doorScript = door.GetComponent<Door>();
                GameObject collectable = UICollectables[UnityEngine.Random.Range(0, UICollectables.Count)];
                doorScript.loot = Regex.Replace(collectable.name, "UI", "Collectable");

                Vector2 doorPos = door.transform.position;

                Vector2 spawnPos = Vector2.zero;

                switch (doorScript.direction)
                {
                    case Direction.Left:
                        spawnPos = new Vector2(doorPos.x + 0.365f, doorPos.y + 0.7f);
                        break;

                    case Direction.Up:
                        spawnPos = new Vector2(doorPos.x, doorPos.y + 0.7f);
                        break;

                    case Direction.Right:
                        spawnPos = new Vector2(doorPos.x - 0.365f, doorPos.y + 0.7f);
                        break;

                    case Direction.Down:
                        spawnPos = new Vector2(doorPos.x, doorPos.y + 0.7f);
                        break;
                }

                collectable = Instantiate(collectable, spawnPos, Quaternion.identity);
                collectable.transform.parent = gameObject.transform;

                UICollectables.Remove(collectable);
            }
        }


        private void GenerateMatrix()
        {
            int x = numberOfMaxTiles / 2;
            int y = numberOfMaxTiles / 2;
            tileMatrix[x, y] = 1;
            newTiles.Add(new Tuple<int, int>(x, y));

            int whileLoopCounter = 0;
            do
            {
                UpdateProbabilities();
                newTiles.Clear();

                whileLoopCounter++;
                if (whileLoopCounter > 100)
                {
                    Debug.LogError("ERROR: Endless do while loop");
                    break;
                }
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
            if (x >= 0 && x < numberOfMaxTiles && y >= 0 && y < numberOfMaxTiles)
                if ((int)tileMatrix[x, y] != 1)
                {
                    float probability = 0;
                    if (x > 0)
                        probability += tileMatrix[x - 1, y];

                    if (x < numberOfMaxTiles - 1)
                        probability += tileMatrix[x + 1, y];

                    if (y > 0)
                        probability += tileMatrix[x, y - 1];

                    if (y < numberOfMaxTiles - 1)
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
            exitDoors.Clear();
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

                        bool upEmpty = true;
                        bool upRightEmpty = true;
                        bool rightEmpty = true;
                        bool downRightEmpty = true;
                        bool downEmpty = true;
                        bool downLeftEmpty = true;
                        bool leftEmpty = true;
                        bool upLeftEmpty = true;


                        if (y < numberOfMaxTiles - 1)
                            upEmpty = (int)tileMatrix[x + 0, y + 1] != 1;

                        if (x < numberOfMaxTiles - 1 && y < numberOfMaxTiles - 1)
                            upRightEmpty = (int)tileMatrix[x + 1, y + 1] != 1;

                        if (x < numberOfMaxTiles - 1)
                            rightEmpty = (int)tileMatrix[x + 1, y + 0] != 1;

                        if (x < numberOfMaxTiles - 1 && y > 0)
                            downRightEmpty = (int)tileMatrix[x + 1, y - 1] != 1;

                        if (y > 0)
                            downEmpty = (int)tileMatrix[x + 0, y - 1] != 1;

                        if (x > 0 && y > 0)
                            downLeftEmpty = (int)tileMatrix[x - 1, y - 1] != 1;

                        if (x > 0)
                            leftEmpty = (int)tileMatrix[x - 1, y + 0] != 1;

                        if (x > 0 && y < numberOfMaxTiles - 1)
                            upLeftEmpty = (int)tileMatrix[x - 1, y + 1] != 1;

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

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}