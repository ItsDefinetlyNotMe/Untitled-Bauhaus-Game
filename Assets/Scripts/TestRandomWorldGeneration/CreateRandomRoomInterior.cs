using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


[Serializable]
public struct InteriorPrefab
{
    public GameObject prefab;
    public int weight;
}

public class CreateRandomRoomInterior : MonoBehaviour
{
    [SerializeField] private InteriorPrefab[] interiorObjects;

    private List<Vector3> borderTiles = new List<Vector3>();
    private List<Vector3> internalTiles = new List<Vector3>();

    private float[,] tileMatrix;
    private int floorTileCount = 0;

    private int indexOfMainObject = 0;
    private int spaceToFill = 0;
    private int maxNumOfMainObject = 0;
    private Vector2 objectOffset = Vector2.zero;

    public void SetInteriorVariables(float[,] newTileMatrix, int newFloorTileCount)
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
        indexOfMainObject = UnityEngine.Random.Range(0, interiorObjects.Length);

        maxNumOfMainObject = (int)(spaceToFill * 0.7 / interiorObjects[indexOfMainObject].weight);

        SetObjectOffset();
        GeneratePositionLists();

        //foreach (Vector3 vec in borderTiles)
        //    print(vec);

        for (int i = 0; i < maxNumOfMainObject; i++)
        {
            int posIndex = UnityEngine.Random.Range(0, borderTiles.Count);
            GameObject newObject = Instantiate(interiorObjects[indexOfMainObject].prefab, borderTiles[posIndex], Quaternion.identity);
            newObject.transform.parent = gameObject.transform;
            borderTiles.RemoveRange(posIndex, 1);
        }

        //PrintTileMatrix();
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

    private void GeneratePositionLists()
    {
        for (int x = 0; x < floorTileCount; x++)
        {
            for (int y = 0; y < floorTileCount; y++)
            {
                if (tileMatrix[x, y] == 1f)
                {


                    int worldX = x - floorTileCount / 2; //reposition to the center of the scene
                    int worldY = y - floorTileCount / 2;

                    //print(new Vector2(worldX + objectOffset.x, worldY + objectOffset.y));

                    int neighbourCount = 0;

                    bool upFull = false;
                    bool rightFull = false;
                    bool downFull = false;
                    bool leftFull = false;


                    if (y != floorTileCount - 1)                    
                        upFull = tileMatrix[x + 0, y + 1] == 1;

                    if (x != floorTileCount - 1)
                        rightFull = tileMatrix[x + 1, y + 0] == 1;

                    if (y != 0)
                        downFull = tileMatrix[x + 0, y - 1] == 1;

                    if (x != 0)
                        leftFull = tileMatrix[x - 1, y + 0] == 1;

                    if (upFull)
                        neighbourCount += 1;
                    if (rightFull)
                        neighbourCount += 1;
                    if (downFull)
                        neighbourCount += 1;
                    if (leftFull)
                        neighbourCount += 1;

                    if (neighbourCount <= 3)
                        borderTiles.Add(new Vector3(worldX + objectOffset.x, worldY + objectOffset.y, 0));
                    else
                        internalTiles.Add(new Vector3(worldX + objectOffset.x, worldY + objectOffset.y, 0));
                }
            }
        }
    }

    private void ResetEverything()
    {
        borderTiles.Clear();
        internalTiles.Clear();
        objectOffset = Vector2.zero;
    }
}