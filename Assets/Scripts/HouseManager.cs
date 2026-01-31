using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class HouseManager : MonoBehaviour
{

    [SerializeField] private GameObject wall, door, wallStart, wallEnd, stairO;
    [SerializeField] private List<Sprite> wallPapers = new List<Sprite>();
    [SerializeField] private List<Room> rooms = new List<Room>();
    [SerializeField] private int difficulty = 0;
    
    private int _floors, _rooms;
    private SpriteRenderer _wallSpriteRender;
    private List<Door> mainDoors = new List<Door>();
    private List<Door> stairs = new List<Door>();
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
        for (i = 0; i <= _floors; i++)
        {
            int numberWalls = Random.Range(4, 9) + _rooms / (_floors + 1);
            _wallSpriteRender.sprite = wallPapers[Random.Range(0, wallPapers.Count)];
            List<int> doorsLoc = new List<int>();
            int j = 0;
            float doorsToAdd = (float)_rooms / (_floors + 1);
            if (i == 0)
            {
                doorsToAdd = Mathf.Ceil(doorsToAdd);
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
            
            if (i != _floors)
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

            startPos += new Vector2(0, -35);
        }

        int doors=0;
        for (i = 0; i <= _rooms; i++)
        {
            int numberWalls = Random.Range(4, 9);
            _wallSpriteRender.sprite = wallPapers[Random.Range(0, wallPapers.Count)];
            int j = 0;
            int doorLoc = Random.Range(0, numberWalls);
            Instantiate(wallStart, startPos, quaternion.identity);
            for (j = 0; j < numberWalls; j++)
            {
                Instantiate(wall, startPos + new Vector2(5.5f * j, 0), Quaternion.identity);
                if (doorLoc == j)
                {
                    var dor2 =  Instantiate(door, startPos + new Vector2(5.5f * j, 0), Quaternion.identity);
                    mainDoors[doors].SetPosToMove(dor2.transform.position);
                    dor2.GetComponent<Door>().SetPosToMove(mainDoors[doors].transform.position);
                    doors++;    
                }
            }
            Instantiate(wallEnd, startPos + new Vector2(5.5f * (j - 1), 0), quaternion.identity);

            startPos += new Vector2(0, -35);
        }
    }
}

[System.Serializable]
class Room
{
    public List<Item> items = new List<Item>();
    public List<Sprite> furniture = new List<Sprite>();
}