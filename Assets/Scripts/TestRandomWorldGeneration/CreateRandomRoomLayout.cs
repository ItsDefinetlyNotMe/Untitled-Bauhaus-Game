using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CreateRandomRoomLayout : MonoBehaviour
{
    [SerializeField] private GameObject wallLeft;
    [SerializeField] private GameObject wallRight;
    [SerializeField] private GameObject wallUp;
    [SerializeField] private GameObject wallDown;
    [SerializeField] private GameObject wallUpWindow;
    [SerializeField] private GameObject wallDownWindow;
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

    private int numberOfMaxTiles;
    private float[,] tileMatrix;
    private int numberOfActiveTiles = 1;

    [SerializeField]
    private int minNumberOfTiles;
    [SerializeField]
    private int maxNumberOfTiles;

    private List<Tuple<int, int>> newTiles = new List<Tuple<int, int>>();

    private CreateRandomRoomInterior createRandomRoomInterior;

    private void Start()
    {
        createRandomRoomInterior = gameObject.GetComponent<CreateRandomRoomInterior>();

        StartRoomGeneration();
    }

    public void StartRoomGeneration()
    {
        ResetEverything();


        numberOfMaxTiles = UnityEngine.Random.Range(minNumberOfTiles, maxNumberOfTiles + 1);
        tileMatrix = new float[numberOfMaxTiles, numberOfMaxTiles];

        GenerateMatrix();

        InstantiateFloor();
        InstantiateWalls();

        createRandomRoomInterior.SetInteriorVariables(ref tileMatrix, numberOfMaxTiles);
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
                if (tileMatrix[x, y] == 1f)
                    floorTileCount++;
            }
        }

        print("current number of ones: " + floorTileCount);
    }

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

    private void CalculateProbability(int x, int y)
    {
        if (tileMatrix[x, y] != 1)
        {
            float probability = 0;
            probability += tileMatrix[x - 1, y];
            probability += tileMatrix[x + 1, y];
            probability += tileMatrix[x, y - 1];
            probability += tileMatrix[x, y + 1];
            probability /= 4;
            if (probability >= 0.9f)
            {
                probability = 0.9f;
            }
            tileMatrix[x, y] = probability;
        }
    }

    private bool AddTiles()
    {
        for (int x = 0; x < numberOfMaxTiles; x++)
        {
            for (int y = 0; y < numberOfMaxTiles; y++)
            {
                if (numberOfActiveTiles >= numberOfMaxTiles)
                    return true;

                if (tileMatrix[x, y] == 1)
                    continue;

                float rand = UnityEngine.Random.value;

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

    private void InstantiateFloor()
    {
        for (int x = 0; x < numberOfMaxTiles; x++)
        {
            for (int y = 0; y < numberOfMaxTiles; y++)
            {
                if (tileMatrix[x, y] == 1f)
                {
                    int worldX = x - numberOfMaxTiles / 2; //reposition to the center of the scene
                    int worldY = y - numberOfMaxTiles / 2;

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

    private void InstantiateWalls()
    {
        for (int x = 0; x < numberOfMaxTiles; x++)
        {
            for (int y = 0; y < numberOfMaxTiles; y++)
            {
                if (tileMatrix[x, y] == 1)
                {
                    int worldX = x - numberOfMaxTiles / 2; //reposition to the center of the scene
                    int worldY = y - numberOfMaxTiles / 2;

                    bool upEmpty = tileMatrix[x + 0, y + 1] != 1;
                    bool upRightEmpty = tileMatrix[x + 1, y + 1] != 1;
                    bool rightEmpty = tileMatrix[x + 1, y + 0] != 1;
                    bool downRightEmpty = tileMatrix[x + 1, y - 1] != 1;
                    bool downEmpty = tileMatrix[x + 0, y - 1] != 1;
                    bool downLeftEmpty = tileMatrix[x - 1, y - 1] != 1;
                    bool leftEmpty = tileMatrix[x - 1, y + 0] != 1;
                    bool upLeftEmpty = tileMatrix[x - 1, y + 1] != 1;

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
                        else if (upRightEmpty && !upLeftEmpty)
                        {
                            GameObject newWallTile = Instantiate(wallUpCornerLeft, new Vector3(worldX, worldY + 1, 0), quaternion.identity);
                            newWallTile.transform.parent = gameObject.transform;
                        }
                        else if (!upRightEmpty && !upLeftEmpty)
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
                        else if (downRightEmpty && !downLeftEmpty)
                        {
                            GameObject newWallTile = Instantiate(wallDownCornerLeft, new Vector3(worldX, worldY, 0), quaternion.identity);
                            newWallTile.transform.parent = gameObject.transform;
                        }
                        else if (!downRightEmpty && !downLeftEmpty)
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