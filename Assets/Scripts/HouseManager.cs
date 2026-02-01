using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class HouseManager : MonoBehaviour
{

    [SerializeField] private GameObject wall, door, wallStart, wallEnd, stairO, camera;
    [SerializeField] private List<Sprite> wallPapers = new List<Sprite>();
    [SerializeField] private List<Room> rooms = new List<Room>();
    [SerializeField] private int difficulty = 0;
    
    private int _floors, _rooms;
    private SpriteRenderer _wallSpriteRender;
    private List<Door> mainDoors = new List<Door>();
    private List<Door> stairs = new List<Door>();
    private List<Item> totalItems = new List<Item>();
    // Start is called before the first frame update
    void Start()
    {
        _wallSpriteRender = wall.GetComponent<SpriteRenderer>();
        GenerateHouse();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateHouse()
    {
        switch (difficulty)
        {
            case 0:
                //easy
                _floors = 0;
                _rooms = Random.Range(2, 3);
                break;
            case 1:
                //medium:
                _floors = Random.Range(1, 3);
                _rooms = Random.Range(4, 7);
                break;
            case 2:
                //hard
                _floors = Random.Range(2, 4);
                _rooms = Random.Range(5, 9);
                break;
        }
        
        // generate floors
        Vector2 startPos = new Vector2(0, 0);
        int i = 0;
        int stairsTotal = 0;
        for (i = 0; i < _floors; i++)
        {
            int numberWalls = Random.Range(4, 9) + _rooms / _floors;
            _wallSpriteRender.sprite = wallPapers[Random.Range(0, wallPapers.Count)];
            List<int> doorsLoc = new List<int>();
            int j = 0;
            float doorsToAdd = (float)_rooms / _floors;
            if (i == 0)
            {
                doorsToAdd = Mathf.Ceil(doorsToAdd);
            }

            if (_rooms < mainDoors.Count + doorsToAdd)
            {
                doorsToAdd = _rooms - mainDoors.Count;
            }
            for (j = 0; j < doorsToAdd; j++)
            {
                int chosen = Random.Range(0, numberWalls);
                while (doorsLoc.Contains(chosen))
                {
                    chosen = Random.Range(0, numberWalls);
                }
                doorsLoc.Add(chosen);
            }
            int stair = -1;
            int stair2 = -1;
            if(_floors > 0){
            
            if (i != _floors - 1)
            {
                stair = Random.Range(0, numberWalls);
                while (doorsLoc.Contains(stair))
                {
                    stair = Random.Range(0, numberWalls);
                }
            }
            

            
            if (i != 0)
            {
                stair2 = Random.Range(0, numberWalls);
                while (doorsLoc.Contains(stair2) || stair == stair2)
                {
                    stair2 = Random.Range(0, numberWalls);
                }
            }
            }
            Instantiate(wallStart, startPos, quaternion.identity);
            
            for (j = 0; j < numberWalls; j++)
            {
                Instantiate(wall, startPos + new Vector2(5.5f * j, 0), Quaternion.identity);
                if (doorsLoc.Contains(j))
                {
                    var dor = Instantiate(door, startPos + new Vector2(5.5f * j, 0), Quaternion.identity);
                    mainDoors.Add(dor.GetComponent<Door>());
                }

                if (j == stair || j == stair2)
                {
                    var staris = Instantiate(stairO, startPos + new Vector2(5.5f * j, 0), quaternion.identity);
                    
                    stairs.Add(staris.GetComponent<Door>());
                    if (stairsTotal != 0 && stairsTotal % 2 != 0)
                    {
                        staris.GetComponent<Door>().SetPosToMove(stairs[stairs.Count - 2].transform.position);
                        stairs[stairs.Count -2 ].SetPosToMove(staris.transform.position);
                        staris.GetComponent<SpriteRenderer>().flipX = true;
                    }
                    stairsTotal++;
                }

                
            }
            Instantiate(wallEnd, startPos + new Vector2(5.5f * (j - 1), 0), quaternion.identity);
            int nrCam = Random.Range(1, 6);
            for (int w = 0; w < nrCam; w++)
            {
                Instantiate(camera, startPos + new Vector2(Random.Range(0, 5.5f * (j - 1)), 2.45f), Quaternion.identity);
            }
            startPos += new Vector2(0, -35);
        }

        int doors=0;
        for (i = 0; i < _rooms; i++)
        {
            print("okok " + i + " " + _rooms);
            int numberWalls = Random.Range(4, 9);
            _wallSpriteRender.sprite = wallPapers[Random.Range(0, wallPapers.Count)];
            int j = 0;
            int doorLoc = Random.Range(0, numberWalls);
            int roomType = Random.Range(0, rooms.Count);
            Instantiate(wallStart, startPos, quaternion.identity);
            List<Room> roomsTemp = new List<Room>(rooms);
            Room roomCur = roomsTemp[roomType];
            List<GameObject> furnitureTemp = new List<GameObject>(roomCur.furniture);
            for (j = 0; j < numberWalls; j++)
            {
                Instantiate(wall, startPos + new Vector2(5.5f * j, 0), Quaternion.identity);
                if (doorLoc == j)
                {
                    var dor2 =  Instantiate(door, startPos + new Vector2(5.5f * j, 0), Quaternion.identity);
                    mainDoors[doors].SetPosToMove(dor2.transform.position);
                    dor2.GetComponent<Door>().SetPosToMove(mainDoors[doors].transform.position);
                    doors++;    
                }else if (Random.Range(0, 101) < 50 && furnitureTemp.Count > 0)
                {
                    int chosenFurniture = Random.Range(0, furnitureTemp.Count);
                    var cur = Instantiate(furnitureTemp[chosenFurniture], startPos + new Vector2(5.5f * j, 0), Quaternion.identity);
                    furnitureTemp.RemoveAt(chosenFurniture);
                    cur.GetComponent<SpriteRenderer>().flipX = Random.Range(0, 2) == 0 ? true : false;
                }
            }
            Instantiate(wallEnd, startPos + new Vector2(5.5f * (j - 1), 0), quaternion.identity);
            
            //spawn items
            int itemsToSpawn = 0;
            if (Random.Range(1, 101) > 15)
            {
                itemsToSpawn = Random.Range(1, 4);
            }
            for (int y = 0; y < itemsToSpawn; y++)
            {
                if (roomCur.items.Count > 0)
                {
                    Instantiate(roomCur.items[Random.Range(0, roomCur.items.Count)], startPos + new Vector2(Random.Range(0, 5.5f * (j-1)), 0), Quaternion.identity);
                }
                
            }
            int nrCam2 = Random.Range(1, 6);
            for (int w2 = 0; w2 < nrCam2; w2++)
            {
                Instantiate(camera, startPos + new Vector2(Random.Range(0, 5.5f * (j - 1)), 2.45f), Quaternion.identity);
            }
            startPos += new Vector2(0, -35);
            
        }
    }
}

[System.Serializable]
class Room
{
    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> furniture = new List<GameObject>();
}