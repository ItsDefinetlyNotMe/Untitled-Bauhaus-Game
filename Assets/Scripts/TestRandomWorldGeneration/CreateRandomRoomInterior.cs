using System;
using System.Collections;
using System.Collections.Generic;
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


    private float[,] tileMatrix;
    private int floorTileCount = 0;

    private int indexOfMainObject = 0;
    private int spaceToFill = 0;
    private int maxNumOfMainObject = 0;

    public void SetInteriorVariables(float[,] newTileMatrix, int newFloorTileCount)
    {
        tileMatrix = newTileMatrix;
        floorTileCount = newFloorTileCount;
        CreateInterior();
    }

    private void CreateInterior()
    {
        spaceToFill = (int)(floorTileCount * UnityEngine.Random.Range(0.2f, 0.5f)); //Fill up 20% - 50% of the room

        //Choose main furniture and fill up ~70% of the space you want to fill
        //Then choose remaining ~30% random from remaining possible objects
        indexOfMainObject = UnityEngine.Random.Range(0, interiorObjects.Length);

        maxNumOfMainObject = (int)(spaceToFill * 0.7 / interiorObjects[indexOfMainObject].weight);

        switch (interiorObjects[indexOfMainObject].prefab.name)
        {
            case "barrel":
                break;

            case "table":
                break;

            case "vase":
                break;

            case "fireplace":
                break;
        }
    }
}
