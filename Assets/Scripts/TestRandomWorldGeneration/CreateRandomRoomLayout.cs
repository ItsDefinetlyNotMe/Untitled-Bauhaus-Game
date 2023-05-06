using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomRoomLayout : MonoBehaviour
{   [SerializeField]
    private GameObject wallLeft;
    [SerializeField]
    private GameObject wallRight;
    [SerializeField]
    private GameObject wallUp;
    [SerializeField]
    private GameObject wallDown;
    [SerializeField]
    private GameObject wallUpWindow;
    [SerializeField]
    private GameObject wallDownWindow;
    [SerializeField]
    private GameObject cornerLeftUp;
    [SerializeField]
    private GameObject cornerLeftDown;
    [SerializeField]
    private GameObject cornerRightUp;
    [SerializeField]
    private GameObject cornerRightDown;
    [SerializeField]
    private GameObject floor;

    private int numberOfMaxTiles;
    private float[,] tileMatrix;
    private int numberOfActiveTiles = 0;

    [SerializeField]
    private int minNumberOfTiles;
    [SerializeField]
    private int maxNumberOfTiles;

    private List<Tuple<int, int>> newTiles = new List<Tuple<int, int>>();

    private void Start()
    {
        numberOfMaxTiles = UnityEngine.Random.Range(minNumberOfTiles, maxNumberOfTiles + 1);
        tileMatrix = new float[numberOfMaxTiles, numberOfMaxTiles];

        GenerateMatrix();

        PrintTileMatrix();

        InstantiateFloor();
    }

    private void GenerateMatrix()
    {
        int x = numberOfMaxTiles / 2;
        int y = numberOfMaxTiles / 2;
        UnityEngine.Debug.Log("Matrix length: " + tileMatrix.Length);
        tileMatrix[x,y] = 1;
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
        System.Diagnostics.Debug.WriteLine("---------------------TileMatrix--------------------");
        String printString = "";
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
        float probability = 0;
        probability += tileMatrix[x - 1, y];
        probability += tileMatrix[x + 1, y];
        probability += tileMatrix[x, y - 1];
        probability += tileMatrix[x, y + 1];
        tileMatrix[x, y] = probability/4;
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
                if (tileMatrix[x, y] == 1)
                {
                    Instantiate(floor, new Vector3(x * 4, y * 4, 0), Quaternion.identity);
                }
            }
        }
    }
}