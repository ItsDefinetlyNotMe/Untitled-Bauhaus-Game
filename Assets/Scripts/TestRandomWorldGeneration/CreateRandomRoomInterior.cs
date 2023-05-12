using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


[Serializable]
public struct InteriorPrefab
{
    public GameObject prefab;
    public Vector2Int size;
    public bool possibleMainObject;
}

public class CreateRandomRoomInterior : MonoBehaviour
{
    [SerializeField] private InteriorPrefab[] interiorObjects;

    private List<Vector2> possibleSpawnPositions = new List<Vector2>();

    private float[,] tileMatrix;
    private int floorTileCount = 0;

    private int indexOfMainObject = 0;
    private int spaceToFill = 0;
    private int spaceToFillWithMainObject = 0;
    private int maxNumOfMainObject = 0;
    private Vector2 objectOffset = Vector2.zero;

    public void SetInteriorVariables(ref float[,] newTileMatrix, int newFloorTileCount)
    {
        tileMatrix = newTileMatrix;
        floorTileCount = newFloorTileCount;
        ResetEverything();
        CreateInterior();
    }

    private void PrintTileMatrix()
    {
        print("---------------------TileMatrix--------------------");
        string printString = "";
        for (int x = 0; x < floorTileCount; x++)
        {
            for (int y = 0; y < floorTileCount; y++)
            {
                printString += (tileMatrix[x, y]);
                printString += "   ";
            }
            print(printString);
            printString = "";
        }
    }


    private void CreateInterior()
    {
        spaceToFill = (int)(floorTileCount * UnityEngine.Random.Range(0.2f, 0.5f)); //Fill up 20% - 50% of the room

        //Choose main furniture and fill up ~70% of the space you want to fill
        //Then choose remaining ~30% random from remaining possible objects
        do
        {
            indexOfMainObject = UnityEngine.Random.Range(0, interiorObjects.Length);

        } while (!interiorObjects[indexOfMainObject].possibleMainObject);

        spaceToFillWithMainObject = (int)(spaceToFill * 0.7);

        //SetObjectOffset();
        MarkBorderTiles();

        for (int x = 0; x < 10; x++)
        {  
            InstantiateBigObject(interiorObjects[5]);
        }
    }

    /// <summary>
    /// Instantiate objects of size 1x1 on the room border
    /// </summary>
    private void InstantiateSmallObject(InteriorPrefab interiorObject)
    {
        GenerateSmallObjectPositionList();

        if (possibleSpawnPositions.Count <= 0)
            return;

        int posIndex = UnityEngine.Random.Range(0, possibleSpawnPositions.Count);
        GameObject newObject = Instantiate(interiorObject.prefab, possibleSpawnPositions[posIndex], Quaternion.identity);
        newObject.transform.parent = gameObject.transform;

        BlockWorldPositionInMatrix(possibleSpawnPositions[posIndex]);
    }

    /// <summary>
    /// Instantiate objects of bigger size anywhere in the room
    /// </summary>
    private void InstantiateBigObject(InteriorPrefab interiorObject)
    {
        GenerateBigObjectPositionList(interiorObject.size);

        if (possibleSpawnPositions.Count <= 0)
            return;

        int posIndex = UnityEngine.Random.Range(0, possibleSpawnPositions.Count);
        GameObject newObject = Instantiate(interiorObject.prefab, possibleSpawnPositions[posIndex], Quaternion.identity);
        newObject.transform.parent = gameObject.transform;

        for (int x = 0; x < interiorObject.size.x; x++)
        {
            for (int y = 0; y < interiorObject.size.y; y++)
            {
                BlockWorldPositionInMatrix(new Vector2(possibleSpawnPositions[posIndex].x + x, possibleSpawnPositions[posIndex].y + y));
            }
        }
    }

    private void BlockWorldPositionInMatrix(Vector2 pos)
    {
        int MatX = (int)(pos.x + floorTileCount / 2);
        int MatY = (int)(pos.y + floorTileCount / 2);

        tileMatrix[MatX, MatY] = 3;
    }

    private void SetObjectOffset()
    {
        switch (interiorObjects[indexOfMainObject].prefab.name)
        {
            case "barrel":
                objectOffset = new Vector2(0f, 0.15f);
                break;

            case "table":
                objectOffset = new Vector2(0f, 0.1f);
                break;

            case "vase":
                objectOffset = new Vector2(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.05f, 0.6f));
                break;

            case "fireplace":
                objectOffset = new Vector2(-0.03f, 0.25f);
                break;
        }
    }

    //1 means floor tile
    //2 means border tile
    //3 means instantiated object
    private void MarkBorderTiles()
    {
        for (int x = 0; x < floorTileCount; x++)
        {
            for (int y = 0; y < floorTileCount; y++)
            {
                if (tileMatrix[x, y] == 1f)
                {
                    bool upEmpty = true;
                    bool rightEmpty = true;
                    bool downEmpty = true;
                    bool leftEmpty = true;


                    if (y != floorTileCount - 1)                    
                        upEmpty = tileMatrix[x + 0, y + 1] < 1;

                    if (x != floorTileCount - 1)
                        rightEmpty = tileMatrix[x + 1, y + 0] < 1;

                    if (y != 0)
                        downEmpty = tileMatrix[x + 0, y - 1] < 1;

                    if (x != 0)
                        leftEmpty = tileMatrix[x - 1, y + 0] < 1;

                    if (upEmpty || rightEmpty || downEmpty || leftEmpty)
                        tileMatrix[x, y] = 2;
                }
            }
        }
    }

    private void GenerateSmallObjectPositionList()
    {
        possibleSpawnPositions.Clear();

        for (int x = 0; x < floorTileCount; x++)
        {
            for (int y = 0; y < floorTileCount; y++)
            {
                if (tileMatrix[x, y] == 2f)
                {
                    int worldX = x - floorTileCount / 2; //reposition to the center of the scene
                    int worldY = y - floorTileCount / 2;

                    possibleSpawnPositions.Add(new Vector2(worldX, worldY));
                }
            }
        }
    }
    private void GenerateBigObjectPositionList(Vector2Int size)
    {
        //reset values
        possibleSpawnPositions.Clear();
        bool positionNotValid = false;

        //iterate over every tile in matrix
        for (int x = 0; x < floorTileCount - size.x; x++)
            for (int y = 0; y < floorTileCount - size.y; y++)
            {
                //check if current tile is floor or border tile
                if (tileMatrix[x, y] == 1f || tileMatrix[x, y] == 2f)
                {
                    //iterate over every tile that would be occupied by the new prefab
                    for (int i = 0; i < size.x; i++)
                    {
                        for (int j = 0; j < size.y; j++)
                        {
                            //check for every tile if it is free
                            if (tileMatrix[x + i, y + j] < 1f || tileMatrix[x + i, y + j] == 3f)
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
                        int worldX = x - floorTileCount / 2; //reposition to the center of the scene
                        int worldY = y - floorTileCount / 2;

                        possibleSpawnPositions.Add(new Vector2(worldX, worldY));

                    }

                    //reset bool
                    positionNotValid = false;

                }
            }
    }

    private void ResetEverything()
    {
        objectOffset = Vector2.zero;
    }
}