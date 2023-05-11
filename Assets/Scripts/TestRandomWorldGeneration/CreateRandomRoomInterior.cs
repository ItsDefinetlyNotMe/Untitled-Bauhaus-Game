using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomRoomInterior : MonoBehaviour
{
    [SerializeField] private GameObject vase;


    private float[,] tileMatrix;
    private int floorTileCount = 0;

    public void SetInteriorVariables(float[,] newTileMatrix, int newFloorTileCount)
    {
        tileMatrix = newTileMatrix;
        floorTileCount = newFloorTileCount;
        CreateInterior();
    }

    private void CreateInterior()
    {
        //Choose random furniture and fill up ~70% of the space you want to fill
        //Then chose remaining ~30% truly random from remaining possible objects


    }
}
