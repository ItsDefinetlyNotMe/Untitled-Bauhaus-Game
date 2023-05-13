using System.Collections.Generic;
using UnityEngine;

namespace TestRandomWorldGeneration
{
    public class CreateRandomRoomInterior : MonoBehaviour
    {
        [SerializeField] private Structs.InteriorPrefab[] interiorObjects;
        [SerializeField] private float percentageOfMainObject;

        private List<Vector2> possibleSpawnPositions = new();

        private SpawnRandomEnemies spawnRandomEnemies;

        private float[,] tileMatrix;
        private int floorTileCount;

        private int indexOfMainObject;
        private int spaceToFill;
        private int spaceToFillWithMainObject;

        private int alreadyFilledSpace;
    
        /// <summary>
        /// Start to create random room interior
        /// </summary>
        /// <param name="newTileMatrix"> position matrix </param>
        /// <param name="newFloorTileCount"> number of tiles </param>
        public void SetInteriorVariables(ref float[,] newTileMatrix, int newFloorTileCount)
        {
            tileMatrix = newTileMatrix;
            floorTileCount = newFloorTileCount;
            ResetEverything();
            MarkBorderTiles();
            CreateInterior();

            spawnRandomEnemies.StartEnemySpawning(ref tileMatrix, floorTileCount, floorTileCount - spaceToFill);
        }

        private void Awake()
        {
            spawnRandomEnemies = gameObject.GetComponent<SpawnRandomEnemies>();
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

    
        /// <summary>
        /// Add objects to room
        /// </summary>
        private void CreateInterior()
        {
            //Fill up 20% - 50% of the room
            spaceToFill = (int)(floorTileCount * UnityEngine.Random.Range(0.2f, 0.4f));

            //Choose main furniture and fill up ~70% of the space you want to fill
            //Then choose remaining random from remaining possible objects
            do
            {
                indexOfMainObject = UnityEngine.Random.Range(0, interiorObjects.Length);

            } while (!interiorObjects[indexOfMainObject].possibleMainObject);

            spaceToFillWithMainObject = (int)(spaceToFill * percentageOfMainObject);

            int sizeOfMainObject = interiorObjects[indexOfMainObject].size.x * interiorObjects[indexOfMainObject].size.y;

            //instantiate main object
            do
            {
                if (sizeOfMainObject > 1)
                    InstantiateBigObject(interiorObjects[indexOfMainObject]);
                else
                    InstantiateSmallObject(interiorObjects[indexOfMainObject]);
                alreadyFilledSpace += sizeOfMainObject;

            } while (alreadyFilledSpace < spaceToFillWithMainObject);

            int indexOfSideObject;

            //instantiate different side objects
            do
            {
                indexOfSideObject = UnityEngine.Random.Range(0, interiorObjects.Length);
                if(indexOfSideObject == indexOfMainObject)
                    continue;
                int sizeOfSideObject = interiorObjects[indexOfSideObject].size.x * interiorObjects[indexOfSideObject].size.y;
               
                if (alreadyFilledSpace + sizeOfSideObject <= spaceToFill)
                {
                    if (sizeOfSideObject > 1)
                        InstantiateBigObject(interiorObjects[indexOfSideObject]);
                    else
                        InstantiateSmallObject(interiorObjects[indexOfSideObject]);
                    
                    alreadyFilledSpace += sizeOfSideObject;
                }
            } while (alreadyFilledSpace != spaceToFill);
        }

    
        /// <summary>
        /// Instantiate objects of size 1x1 on the room border
        /// </summary>
        /// <param name="interiorObject"> object to instantiate </param>
        private void InstantiateSmallObject(Structs.InteriorPrefab interiorObject)
        {
            //Generate possible position list
            GenerateSmallObjectPositionList();

            if (possibleSpawnPositions.Count <= 0)
                return;
        
            //Instantiate object
            int posIndex = UnityEngine.Random.Range(0, possibleSpawnPositions.Count);
            GameObject newObject = Instantiate(interiorObject.prefab, possibleSpawnPositions[posIndex] + interiorObject.offset, Quaternion.identity);
            newObject.transform.parent = gameObject.transform;

            //Mark position in matrix
            BlockWorldPositionInMatrix(possibleSpawnPositions[posIndex]);
        }

    
        /// <summary>
        /// Instantiate objects of bigger size anywhere in the room
        /// </summary>
        /// <param name="interiorObject"> object to instantiate </param>
        private void InstantiateBigObject(Structs.InteriorPrefab interiorObject)
        {
            //Generate possible position list
            GenerateBigObjectPositionList(interiorObject.size);

            if (possibleSpawnPositions.Count <= 0)
                return;

            //Instantiate object
            int posIndex = UnityEngine.Random.Range(0, possibleSpawnPositions.Count);
            GameObject newObject = Instantiate(interiorObject.prefab, possibleSpawnPositions[posIndex] + interiorObject.offset, Quaternion.identity);
            newObject.transform.parent = gameObject.transform;

            //Mark position in matrix
            for (int x = 0; x < interiorObject.size.x; x++)
            {
                for (int y = 0; y < interiorObject.size.y; y++)
                {
                    BlockWorldPositionInMatrix(new Vector2(possibleSpawnPositions[posIndex].x + x, possibleSpawnPositions[posIndex].y + y));
                }
            }
        }
    
    
        /// <summary>
        /// Changes number of position in matrix to instantiated object
        /// </summary>
        /// <param name="pos"> World position x,y of object </param>
        private void BlockWorldPositionInMatrix(Vector2 pos)
        {
            int matX = (int)(pos.x + floorTileCount / 2);
            int matY = (int)(pos.y + floorTileCount / 2);

            tileMatrix[matX, matY] = 3;
        }


        /// <summary>
        /// Mark border tiles in Matrix.
        ///1 means floor tile,
        ///2 means border tile,
        ///3 means instantiated object,
        ///4 means entry door,
        ///5 means exit door.
        /// </summary>
        private void MarkBorderTiles()
        {
            for (int x = 0; x < floorTileCount; x++)
            {
                for (int y = 0; y < floorTileCount; y++)
                {
                    if ((int)tileMatrix[x, y] == 1)
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

    
        /// <summary>
        /// Generate all possible spawn positions for a given small object 
        /// </summary>
        private void GenerateSmallObjectPositionList()
        {
            possibleSpawnPositions.Clear();

            for (int x = 0; x < floorTileCount; x++)
            {
                for (int y = 0; y < floorTileCount; y++)
                {
                    if ((int)tileMatrix[x, y] == 2)
                    {
                        float worldX = x - floorTileCount / 2; //reposition to the center of the scene
                        float worldY = y - floorTileCount / 2;

                        possibleSpawnPositions.Add(new Vector2(worldX, worldY));
                    }
                }
            }
        }
    
    
        /// <summary>
        /// Generate all possible spawn positions for a given big object 
        /// </summary>
        /// <param name="size"> x,y of object size </param>
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
                if ((int)tileMatrix[x, y] == 1 || (int)tileMatrix[x, y] == 2)
                {
                    //iterate over every tile that would be occupied by the new prefab
                    for (int i = 0; i < size.x; i++)
                    {
                        for (int j = 0; j < size.y; j++)
                        {
                            //check for every tile if it is free
                            if (tileMatrix[x + i, y + j] < 1f || (int)tileMatrix[x + i, y + j] == 3)
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
                        float worldX = x - floorTileCount / 2; //reposition to the center of the scene
                        float worldY = y - floorTileCount / 2;

                        possibleSpawnPositions.Add(new Vector2(worldX, worldY));
                    }

                    //reset bool
                    positionNotValid = false;
                }
            }
        }

    
        private void ResetEverything()
        {
            alreadyFilledSpace = 0;
        }
    }
}