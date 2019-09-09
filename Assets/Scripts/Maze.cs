using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public static Maze Instance;

    const int North = 0;
    const int East = 1;
    const int South = 2;
    const int West = 3;

    Position[] Directions = new Position[]
    {
        new Position(0, 1),  // N
        new Position(1, 0),  // E
        new Position(0, -1), // S
        new Position(-1, 0)  // W
    };

    class Tile
    {
        //stores if it has connection to neighbour
        //starts out with no conncections
        public bool[] Neighbours =
        {
            false, // N
            false, // E
            false, // S
            false  // W
        };

        public bool[] FixedWall =
        {
            false, // N
            false, // E
            false, // S
            false  // W
        };
    }

    enum TileType
    {
        None,
        Wall,
        Furniture,
        Floor,
        Door
    }

    TileType[,] finishedTiles;

    public int Width;
    public int Height;
    public int Seed;

    Random Rnd;

    Tile[,] tiles;
    bool[,] visited;

    //prefabs
    public GameObject space;
    public GameObject WallPrefab;
    public GameObject FurniturePrefab;
    public GameObject PickupPrefab;
    public GameObject UI;
    public GameObject PrefabTV;
    public GameObject PrefabMicrowave;
    public GameObject PrefabBathtub;
    public GameObject PrefabBed;
    public GameObject PrefabGrammophone;

    //instantiated objects
    GameObject microwave;
    GameObject tv;
    GameObject grammophone;
    GameObject bathtub;
    GameObject bed;

    public GameObject RoomCoverParent;
    public GameObject BackgroundSolved;

    public GameObject FurnitureSingle;
    public GameObject FurnitureHorizontal;
    public GameObject FurnitureVertical;

    // space between tiles
    public Vector3 Delta;
    public Vector3 Pos0;

    // position of dead ends in world position
    public List<Vector3> DeadEndsWorldPos = new List<Vector3>();
    public List<Position> DeadEndCoordinates = new List<Position>();
    public List<List<int>> DeadEndIndexesByRoom = new List<List<int>>();

    public bool[,] furniturePlaced;

    public int[,] roomNr;

    public GameObject[] roomParents = new GameObject[8];
    public GameObject[] Xs;
    public GameObject[] circles;
    public GameObject[] ticks;
    public bool[] pickedUp;

    public GameObject black;
    public GameObject[] finishedRooms;

    public AudioClip winSong;

    public bool[] RoomSolvable = new bool[8];

    private bool roomSolved = false;

    bool IsInBounds(Position pos)
    {
        return (pos.x >= 0 && pos.x < Width) && (pos.y >= 0 && pos.y < Height);
    }

    bool IsVisited(Position pos)
    {
        return visited[pos.x, pos.y];
    }

    void SetVisited(Position pos)
    {
        visited[pos.x, pos.y] = true;
    }

    Tile GetTile(Position pos)
    {
        return tiles[pos.x, pos.y];
    }

    int GetOppositeDirection(int dir)
    {
        return (dir + 2) % 4;
    }


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Seed = (int)System.DateTime.Now.TimeOfDay.TotalSeconds;
        Random.InitState(Seed);
        File.WriteAllText("Seed.txt", Seed.ToString());
        visited = new bool[Width, Height];
        tiles = new Tile[Width, Height];
        finishedTiles = new TileType[Width * 2 + 1, Height * 2 + 1];
        roomNr = new int[Width * 2 + 1, Height * 2 + 1];

        Delta = new Vector3(1f / (Width * 2f), 1f / (Height * 2f), 0);
        Pos0 = new Vector3(0, 0, Camera.main.nearClipPlane + 100);// + delta / 2f;

        for (int i = 0; i < 8; i++)
        {
            DeadEndIndexesByRoom.Add(new List<int>());
        }

        for (int y = 0; y < Height * 2 + 1; y++)
        {
            for (int x = 0; x < Width * 2 + 1; x++)
            {
                finishedTiles[x, y] = TileType.None;
            }
        }

        for (int y = 0; y < Height * 2 + 1; y++)
        {
            for (int x = 0; x < Width * 2 + 1; x++)
            {
                roomNr[x, y] = 0;
            }
        }

        // fill everything with walls
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                visited[x, y] = false;
                tiles[x, y] = new Tile();
            }
        }


        #region Room1
        Dictionary<int, List<Position>> wallsRoom1 = new Dictionary<int, List<Position>>();

        wallsRoom1.Add(North, new List<Position>());

        for (int x = 0; x <= 5; x++)
        {
            wallsRoom1[North].Add(new Position(x, 1));
        }

        wallsRoom1.Add(East, new List<Position>());

        for (int y = 0; y <= 1; y++)
        {
            wallsRoom1[East].Add(new Position(5, y));
        }

        List<Position> doorsRoom1 = new List<Position>()
        {
            new Position(5, 1),
            new Position(6, 1)
        };

        AddRoom(wallsRoom1, doorsRoom1);
        #endregion

        #region Room2
        Dictionary<int, List<Position>> wallsRoom2 = new Dictionary<int, List<Position>>();

        wallsRoom2.Add(South, new List<Position>());
        for (int x = 0; x <= 5; x++)
        {
            wallsRoom2[South].Add(new Position(x, 8));
        }
        for (int x = 6; x <= 10; x++)
        {
            wallsRoom2[South].Add(new Position(x, 13));
        }

        wallsRoom2.Add(East, new List<Position>());
        for (int y = 8; y <= 12; y++)
        {
            wallsRoom2[East].Add(new Position(5, y));
        }
        for (int y = 13; y <= Height - 1; y++)
        {
            wallsRoom2[East].Add(new Position(10, y));
        }

        List<Position> doorsRoom2 = new List<Position>()
        {
            new Position(2,8),
            new Position(2, 9),
            new Position(10, 16),
            new Position(11, 16)
        };

        AddRoom(wallsRoom2, doorsRoom2);
        #endregion

        #region Room3
        Dictionary<int, List<Position>> wallsRoom3 = new Dictionary<int, List<Position>>();

        wallsRoom3.Add(North, new List<Position>());
        for (int x = 10; x <= 13; x++)
        {
            wallsRoom3[North].Add(new Position(x, 12));
        }

        wallsRoom3.Add(East, new List<Position>());
        for (int y = 0; y <= 12; y++)
        {
            wallsRoom3[East].Add(new Position(13, y));
        }

        List<Position> doorsRoom3 = new List<Position>()
        {
            new Position(13, 4),
            new Position(14, 4),
            new Position(11, 12),
            new Position(11, 13)
        };

        AddRoom(wallsRoom3, doorsRoom3);
        #endregion

        #region Room4
        Dictionary<int, List<Position>> wallsRoom4 = new Dictionary<int, List<Position>>();

        wallsRoom4.Add(South, new List<Position>());
        for (int x = 14; x <= 23; x++)
        {
            wallsRoom4[South].Add(new Position(x, 9));
        }

        wallsRoom4.Add(East, new List<Position>());
        for (int y = 9; y <= Height - 1; y++)
        {
            wallsRoom4[East].Add(new Position(23, y));
        }

        List<Position> doorsRoom4 = new List<Position>()
        {
            new Position(16, 10),
            new Position(16, 9),
            new Position(23, 16),
            new Position(22, 16),
            //new Position(23, 12),
            //new Position(24, 12) 

        };

        AddRoom(wallsRoom4, doorsRoom4);
        #endregion

        #region Room5

        Dictionary<int, List<Position>> wallsRoom5 = new Dictionary<int, List<Position>>();

        wallsRoom5.Add(South, new List<Position>());
        for (int x = 19; x <= Width - 1; x++)
        {
            wallsRoom5[South].Add(new Position(x, 5));
        }

        wallsRoom5.Add(West, new List<Position>());
        for (int y = 5; y <= 8; y++)
        {
            wallsRoom5[West].Add(new Position(19, y));
        }

        List<Position> doorsRoom5 = new List<Position>()
        {
            new Position(18, 7),
            new Position(19, 7)
        };

        AddRoom(wallsRoom5, doorsRoom5);
        #endregion

        #region Room6

        Dictionary<int, List<Position>> wallsRoom6 = new Dictionary<int, List<Position>>();

        wallsRoom6.Add(North, new List<Position>());
        for (int x = 24; x <= Width - 1; x++)
        {
            wallsRoom6[North].Add(new Position(x, 15));
        }

        List<Position> doorsRoom6 = new List<Position>()
        {

        };

        AddRoom(wallsRoom6, doorsRoom6);
        #endregion

        #region Special Objects
        // Microwave
        GetTile(new Position(17, 17)).FixedWall[East] = true;
        GetTile(new Position(18, 17)).FixedWall[West] = true;

        //Bed
        GetTile(new Position(4, 12)).FixedWall[East] = true;
        GetTile(new Position(5, 12)).FixedWall[West] = true;

        //Bathtub
        GetTile(new Position(30, 14)).FixedWall[East] = true;
        GetTile(new Position(31, 14)).FixedWall[West] = true;

        //TV
        GetTile(new Position(2, 5)).FixedWall[East] = true;
        GetTile(new Position(3, 5)).FixedWall[West] = true;

        //Grammophone
        GetTile(new Position(15, 7)).FixedWall[East] = true;
        GetTile(new Position(16, 7)).FixedWall[West] = true;
        #endregion

        CreateMazeDepthFirst();
        WriteMazeToFile();
        ExpandMaze();

        var setFurniture = new GameObject("SetFurniture");
        // dont put stuff there automatically!
        // Micowave
        finishedTiles[36, 35] = TileType.None;
        microwave = Instantiate(PrefabMicrowave, Camera.main.ViewportToWorldPoint(GridToCameraPos(new Position(36, 35))), Quaternion.identity, setFurniture.transform); 

        //Bed
        finishedTiles[10, 25] = TileType.None;
        finishedTiles[10, 26] = TileType.None;
        bed = Instantiate(PrefabBed, Camera.main.ViewportToWorldPoint(GridToCameraPos(new Position(10, 25))), Quaternion.identity, setFurniture.transform);

        //Bathtub
        finishedTiles[62, 29] = TileType.None;
        finishedTiles[62, 30] = TileType.None;
        bathtub = Instantiate(PrefabBathtub, Camera.main.ViewportToWorldPoint(GridToCameraPos(new Position(62, 29))), Quaternion.identity, setFurniture.transform);

        //TV
        finishedTiles[6, 11] = TileType.None;
        tv = Instantiate(PrefabTV, Camera.main.ViewportToWorldPoint(GridToCameraPos(new Position(6, 11))), Quaternion.identity, setFurniture.transform);

        //Grammophone
        finishedTiles[32, 15] = TileType.None;
        grammophone = Instantiate(PrefabGrammophone, Camera.main.ViewportToWorldPoint(GridToCameraPos(new Position(32, 15))), Quaternion.identity, setFurniture.transform);

        //Wall off secret areas

        // Room1
        finishedTiles[11, 4] = TileType.Wall;
        finishedTiles[12, 4] = TileType.Wall;
        finishedTiles[12, 3] = TileType.Wall;

        // Room7
        finishedTiles[48, 33] = TileType.Wall;

        //PutRoomWalls();
        AssignTilesToRooms();
        PlaceFurniture();

        InstantiateStuff();

        string deadEnds = "";
        for (int i = 0; i < DeadEndsWorldPos.Count; i++)
        {
            deadEnds += DeadEndCoordinates[i] + " | " + DeadEndsWorldPos[i] + "\n";
        }
        File.WriteAllText("deadEnds.txt", deadEnds);

        /*
        var pickups = new GameObject("Pickups");
        for (int i = 3; i < 7; i++)
        {           
            var pos = GetRandomDeadEndPos(i);
            Debug.Log("Random pos room " + i + ": " + pos);
            GameObject.Instantiate(PickupPrefab, pos, Quaternion.identity, pickups.transform);
        } */
    }

    // Update is called once per frame
    void Update()
    {

    }

    public class Position
    {
        public int x;
        public int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.x + b.x, a.y + b.y);
        }

        public override string ToString()
        {
            return "(" + x + "|" + y + ")";
        }
    }

    void WriteMazeToFile()
    {
        string maze = "";
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (tiles[x, y].Neighbours[South])
                {
                    maze += " # ";
                }
                else
                {
                    maze += "   ";
                }
            }
            maze += "\n";
            for (int x = 0; x < Width; x++)
            {
                if (tiles[x, y].Neighbours[West])
                {
                    maze += "#";
                }
                else
                {
                    maze += " ";
                }
                maze += "#";
                if (tiles[x, y].Neighbours[East])
                {
                    maze += "#";
                }
                else
                {
                    maze += " ";
                }
            }
            maze += "\n";
            for (int x = 0; x < Width; x++)
            {
                if (tiles[x, y].Neighbours[North])
                {
                    maze += " # ";
                }
                else
                {
                    maze += "   ";
                }
            }
            maze += "\n";
        }
        File.WriteAllText("maze.txt", maze);
    }

    void CreateMazeDepthFirst()
    {

        Position curPos = new Position(0, 0);

        bool waitingForDeadend = true;
        Position deadEndCoord;
        Vector3 deadEndPos;

        // mark inital cell
        visited[curPos.x, curPos.y] = true;
        Stack<Position> stack = new Stack<Position>();

        int unvisited = Width * Height - 1; // minus initial cell
        while (unvisited > 0)
        {
            //Debug.Log("Cur: " + curPos + " unvisited: " + unvisited);

            List<int> neighbours = new List<int>();

            //find all unvisited neighbours
            for (int i = 0; i < 4; i++)
            {
                Position neighbour = curPos + Directions[i];

                if (!IsInBounds(neighbour))
                    continue;

                if (IsVisited(neighbour))
                    continue;

                if (GetTile(curPos).FixedWall[i])
                    continue;

                neighbours.Add(i);
            }

            // choose random neighbour to go to
            if (neighbours.Count > 0)
            {
                waitingForDeadend = true;

                int choice = Random.Range(0, neighbours.Count);
                int chosenDir = neighbours[choice];

                stack.Push(curPos);

                // break wall TO next tile
                GetTile(curPos).Neighbours[chosenDir] = true;

                // switch to new tile
                curPos += Directions[chosenDir];


                // break wall TO prev tile
                GetTile(curPos).Neighbours[GetOppositeDirection(chosenDir)] = true;

                // mark as visited
                SetVisited(curPos);
                unvisited--;
            }
            else // dead end
            {
                if (waitingForDeadend)
                {
                    deadEndCoord = new Position(1 + curPos.x * 2, 1 + curPos.y * 2);
                    DeadEndCoordinates.Add(deadEndCoord);

                    deadEndPos = Camera.main.ViewportToWorldPoint(GridToCameraPos(deadEndCoord));
                    DeadEndsWorldPos.Add(deadEndPos);

                    waitingForDeadend = false;
                }

                // this should never happen while there's unvisited tiles!               
                if (stack.Count == 0)
                {
                    throw new System.Exception("Stack empty before all tiles have been visited!");
                }

                // go back to find a path to continue
                curPos = stack.Pop();
            }
        }
        Debug.Log("Maze created!");

        deadEndCoord = new Position(1 + curPos.x * 2, 1 + curPos.y * 2);
        DeadEndCoordinates.Add(deadEndCoord);

        deadEndPos = Camera.main.ViewportToWorldPoint(GridToCameraPos(deadEndCoord));
        DeadEndsWorldPos.Add(deadEndPos);
    }

    public void ExpandMaze()
    {
        var cam = Camera.main;        
        Vector3 cur = Pos0;

        var spawn = cam.ViewportToWorldPoint(Pos0 + new Vector3(Delta.x, Delta.y * (Height * 2 - 1))); // top left
        //GameObject.Instantiate(PickupPrefab, spawn, Quaternion.identity);
        Debug.Log("Spawn: " + spawn);

        // x and y are coordinates for the original (small) generated labyrinth
        // x2 and y2 are coordinates for the finished (big) labyrinth
        int x2 = 0;
        int y2 = 0;

        // bottom line is always wall
        for (x2 = 0; x2 < Width * 2 + 1; x2++)
        {
            MakeWall(x2, 0);
        }

        y2 = 1;
        x2 = 0;

        for (int y = 0; y < Height; y++)
        {
            //left side is always walls
            MakeWall(x2, y2);
            x2++;

            // horizontal connections and tiles
            for (int x = 0; x < Width; x++)
            {
                // actual tile is always empty
                MakeFloor(x2, y2);
                x2++;

                //Debug.Log("x|y: " + x + "|" + y);

                if (tiles[x, y].Neighbours[East])
                {
                    MakeFloor(x2, y2);
                }
                else
                {
                    MakeFurniture(x2, y2);
                }
                x2++;
            }

            y2++;
            x2 = 0;

            // vertical connections

            // left side is always walls
            MakeWall(x2, y2);
            x2++;

            for (int x = 0; x < Width; x++)
            {
                if (tiles[x, y].Neighbours[North])
                {
                    MakeFloor(x2, y2);
                }
                else
                {
                    MakeFurniture(x2, y2);
                }

                x2++;
                // place wall where vertical connection is below
                MakeFurniture(x2, y2);
                x2++;
            }
            y2++;
            x2 = 0;
        }

        // top line is always wall
        for (x2 = 0; x2 < Width * 2 + 1; x2++)
        {
            MakeWall(x2, Height * 2);
        }

        // right line is always wall
        for (y2 = 0; y2 < Height * 2 + 1; y2++)
        {
            MakeWall(Width * 2, y2);
        }
    }

    void NextBlock(ref Vector3 pos)
    {
        pos.x += Delta.x;
    }

    void NextRow(ref Vector3 pos)
    {
        pos.y += Delta.y;
        pos.x = Pos0.x;
        pos.z -= 0.01f;
    }

    Vector3 GridToCameraPos(Position pos)
    {
        // DONT LOOK AT IT!
        return new Vector3(pos.x * Delta.x, pos.y * Delta.y, Camera.main.nearClipPlane + 100f + 0.01f * pos.y);
    }

    public void MakeWall(int x, int y)
    {
        //Debug.Log("Making wall at: " + new Position(x, y));
        finishedTiles[x, y] = TileType.Wall;
    }

    public void MakeFloor(int x, int y)
    {
        finishedTiles[x, y] = TileType.Floor;
    }

    public void MakeDoor(int x, int y)
    {
        finishedTiles[x, y] = TileType.Door;
    }

    public void MakeFurniture(int x, int y)
    {
        if (finishedTiles[x, y] == TileType.Wall)
            return;
        finishedTiles[x, y] = TileType.Furniture;
    }

    /// <summary>
    /// Places a wall at pos
    /// </summary>
    /// <param name="pos">position in CAMERA space</param>
    public void MakeWall(Vector3 pos)
    {
        GameObject.Instantiate(WallPrefab, Camera.main.ViewportToWorldPoint(pos), Quaternion.identity);
    }

    public void MakeDoor(Vector3 pos)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="walls">maps direction of wall to list of positions</param>
    /// <param name="doors"></param>
    public void AddRoom(Dictionary<int, List<Position>> walls, List<Position> doors)
    {
        foreach (int dir in walls.Keys)
        {
            foreach (var pos in walls[dir])
            {
                if (doors.Exists(doorPos => pos.x == doorPos.x && pos.y == doorPos.y))
                {
                    //int x = 1 + pos.x * 2 + Directions[dir].x;
                    //int y = 1 + pos.y * 2 + Directions[dir].y;
                    //MakeDoor(x, y);
                }
                else
                {
                    /*
                    int x = 1 + pos.x * 2 + Directions[dir].x;
                    int y = 1 + pos.y * 2 + Directions[dir].y;
                    MakeWall(x, y);
                    if(dir % 2 == 0) // vertical wall (i think)
                    {
                        MakeWall(x + 1, y);
                    }
                    else
                    {
                        MakeWall(x, y + 1);
                    }
                    ´*/
                    GetTile(pos).FixedWall[dir] = true;
                    GetTile(pos + Directions[dir]).FixedWall[GetOppositeDirection(dir)] = true;

                    var pos2 = new Position(1 + pos.x * 2, 1 + pos.y * 2) + Directions[dir];
                    MakeWall(pos2.x, pos2.y);
                   switch(dir)
                    {
                        case North:
                            MakeWall(pos2.x + 1, pos2.y);
                            MakeWall(pos2.x - 1, pos2.y);
                            break;

                        case South:
                            MakeWall(pos2.x + 1, pos2.y);
                            MakeWall(pos2.x - 1, pos2.y);
                            break;

                        case East:
                            MakeWall(pos2.x, pos2.y + 1);
                            MakeWall(pos2.x, pos2.y - 1);
                            break;

                        case West:
                            MakeWall(pos2.x, pos2.y + 1);
                            MakeWall(pos2.x, pos2.y - 1);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /*
        // assumes doors are always pairs of cells
        for (int i = 0; i < doors.Count; i+=2)
        {
            var door1 = doors[i];
            var door2 = doors[i + 1];

            int wallPosX = 0;
            int wallPosY = 0;

            if(door1.x > door2.x)
            {
                wallPosX = 1 + door1.x * 2 + Directions[West].x;
                wallPosY = 1 + door1.y * 2 + Directions[West].y + 1;
            }
            else if(door1.x < door2.x)
            {
                wallPosX = 1 + door1.x * 2 + Directions[East].x;
                wallPosY = 1 + door1.y * 2 + Directions[East].y + 1;
            }
            else if(door1.y > door2.y)
            {
                wallPosX = 1 + door1.x * 2 + Directions[South].x + 1;
                wallPosY = 1 + door1.y * 2 + Directions[South].y;
            }
            else if(door1.y < door2.x)
            {
                wallPosX = 1 + door1.x * 2 + Directions[North].x + 1;
                wallPosY = 1 + door1.y * 2 + Directions[North].y;
            }

            MakeWall(wallPosX, wallPosY);
            
        }
        */
    }

    void PutRoomWalls()
    {
        for (int x = 1; x < 13; x++)
        {
            MakeWall(x, 4);
        }

        for (int y = 1; y < 4; y++)
        {
            MakeWall(12, y);
        }

        for (int x = 0; x < 13; x++)
        {
            if (x == 5) // Door
            {
                MakeDoor(x, 16);
                continue;
            };
            MakeWall(x, 16);
        }

        for (int y = 17; y < 27; y++)
        {
            MakeWall(12, y);
        }

        for (int x = 13; x < 23; x++)
        {
            MakeWall(x, 26);
        }

        for (int y = 27; y < 36; y++)
        {
            if (y == 33) // Door
            {
                MakeDoor(22, y);
                continue;
            };
            MakeWall(22, y);
        }

        for (int x = 22; x < 29; x++)
        {
            if (x == 27) // Door
            {
                MakeDoor(x, 26);
                continue;
            };
            MakeWall(x, 26);
        }

        for (int y = 1; y < 26; y++)
        {
            if(y == 9)
            {
                MakeDoor(28, y);
                continue;
            }
            MakeWall(28, y);
        }

        for (int x = 29; x < 49; x++)
        {
            if (x == 33) // Door
            {
                MakeDoor(x, 18);
                continue;
            };
            MakeWall(x, 18);
        }

        for (int y = 19; y < 36; y++)
        {
            //if (y == 33) // Door
            //{ MakeDoor(x, y); continue; };
            MakeWall(48, y);
        }

        for (int y = 10; y < 18; y++)
        {
            if (y == 15) // Door
            {
                MakeDoor(38, y);
                continue;
            };
            MakeWall(38, y);
        }

        for (int x = 39; x < 64; x++)
        {
            MakeWall(x, 10);
        }

        for (int x = 49; x < 64; x++)
        {
            MakeWall(x, 32);
        }
    }

    void InstantiateStuff()
    {
        GameObject wallParent = new GameObject("Walls");

        for (int y = 0; y < Height * 2 + 1; y++)
        {
            for (int x = 0; x < Width * 2 + 1; x++)
            {
                //Debug.Log(finishedTiles[x, y]);
                Vector3 worldPos = Camera.main.ViewportToWorldPoint(GridToCameraPos(new Position(x, y)));
                switch (finishedTiles[x, y])
                {
                    case TileType.None:
                        break;

                    case TileType.Wall:
                        //Debug.Log("Inst. wall");
                        GameObject.Instantiate(WallPrefab, worldPos, Quaternion.identity, wallParent.transform);
                        break;

                    case TileType.Furniture:
                        GameObject parent = roomParents[roomNr[x, y]];
                        switch(furnitureType[x,y])
                        {
                            case FurnitureType.Single:
                                GameObject.Instantiate(FurnitureSingle, worldPos, Quaternion.identity, parent.transform);
                                break;

                            case FurnitureType.Horizontal:
                                GameObject.Instantiate(FurnitureHorizontal, worldPos, Quaternion.identity, parent.transform);
                                break;

                            case FurnitureType.Vertical:
                                GameObject.Instantiate(FurnitureVertical, worldPos, Quaternion.identity, parent.transform);
                                break;
                        }
                        
                        break;

                    case TileType.Floor:
                        break;

                    case TileType.Door:
                        break;
                }
            }
        }
    }

    public void AssignTilesToRooms()
    {
        string roomNrString = "";
        for (int y = 0; y < Height * 2 + 1; y++)
        {
            for (int x = 0; x < Width * 2 + 1; x++)
            {
                if (x <= 12 && y <= 4)
                {
                    roomNr[x, y] = 1;
                }
                else if (x <= 12 && y >= 16 || x <= 22 && y >= 26)
                {
                    roomNr[x, y] = 2;
                }
                else if (x <= 28 && y <= 26)
                {
                    roomNr[x, y] = 3;
                }
                else if (x <= 48 && y >= 18)
                {
                    roomNr[x, y] = 4;
                }
                else if (y >= 32)
                {
                    roomNr[x, y] = 7;
                }
                else if (x >= 38 && y >= 10)
                {
                    roomNr[x, y] = 6;
                }
                else
                {
                    roomNr[x, y] = 5;
                }
                roomNrString += roomNr[x, y];
            }
            roomNrString += "\n";
        }
        File.WriteAllText("roomNrs.txt", roomNrString);

        
        for (int i = 0; i < DeadEndCoordinates.Count; i++)
        {
            var pos = DeadEndCoordinates[i];
            int nr = roomNr[pos.x, pos.y];
            DeadEndIndexesByRoom[nr].Add(i);
        }

        string byRoomString = "";
        foreach (var room in DeadEndIndexesByRoom)
        {
            foreach (var index in room)
            {
                byRoomString += DeadEndCoordinates[index] +" ";
            }
            byRoomString += "\n";
        }
        File.WriteAllText("DeadEndsByRoom.txt", byRoomString);
    }

    public Vector3 GetRandomDeadEndPos(int room)
    {
        var list = DeadEndIndexesByRoom[room];

        Debug.Log("List of dead ends in room " + room);

        string s = "";
        foreach (var item in list)
        {
            s += item + " ";
        }
        Debug.Log(s);

        var i = list[Random.Range(0, list.Count)];

        Debug.Log("Random index: " + i);

        Debug.Log("Random pos room " + room + ": " + DeadEndCoordinates[i]);

        return DeadEndsWorldPos[i];
    }

    public void PickUp(int room)
    {
        roomSolved = true;
        Xs[room].SetActive(false);
        circles[room].SetActive(true);
        pickedUp[room] = true;

        // check if pickup FOR that room and pickup IN that room has been collected
        // then enable particles for deliver locations

        if(pickedUp[3] && pickedUp[4])
        {
            if (!RoomSolvable[4])
            {
                microwave.transform.GetChild(0).gameObject.SetActive(true);
                RoomSolvable[4] = true;
            }
        }

        if(pickedUp[3] && pickedUp[6])
        {
            if (!RoomSolvable[3])
            {
                tv.transform.GetChild(0).gameObject.SetActive(true);
                RoomSolvable[3] = true;
            }
        }

        if(pickedUp[4] && pickedUp[5])
        {
            if (!RoomSolvable[5])
            {
                grammophone.transform.GetChild(0).gameObject.SetActive(true);
                RoomSolvable[5] = true;
            }
        }

        if(pickedUp[5] && pickedUp[6])
        {
            if (!RoomSolvable[6])
            {
                bathtub.transform.GetChild(0).gameObject.SetActive(true);
                RoomSolvable[6] = true;
            }
        }
    }

    public void SolveRoom(int room)
    {
        ticks[room].SetActive(true);
        circles[room].SetActive(false);
        roomParents[room].SetActive(false);
        finishedRooms[room].SetActive(true);
    }

    public void Win()
    {
        var audio = Camera.main.GetComponent<AudioSource>();
        audio.Pause();
        audio.clip = winSong;
        audio.Play();

        UI.SetActive(false);
        //Debug.Log("Win");
        roomParents[1].SetActive(false);
        roomParents[2].SetActive(false);
        roomParents[7].SetActive(false);

        finishedRooms[1].SetActive(true);
        finishedRooms[2].SetActive(true);
        finishedRooms[7].SetActive(true);

        //BackgroundSolved.SetActive(true);
        RoomCoverParent.SetActive(false);
    }

    enum FurnitureType
    {
        None,
        Single,
        Horizontal,
        Vertical
    }

    FurnitureType[,] furnitureType;

    public void PlaceFurniture()
    {
        furnitureType = new FurnitureType[Width * 2 + 1, Height * 2 + 1];

        furniturePlaced = new bool[Width * 2 + 1, Height * 2 + 1];
        for (int y = 0; y < Height * 2 + 1; y++)
        {
            for (int x = 0; x < Width * 2 + 1; x++)
            {
                if (finishedTiles[x, y] == TileType.Furniture)
                    furniturePlaced[x, y] = false;
                else
                    furniturePlaced[x, y] = true;
            }
        }

        List<FurnitureType> possibleTypes = new List<FurnitureType>();
        for (int y = 1; y < Height * 2; y++)
        {
            for (int x = 1; x < Width * 2; x++)
            {
                if(furniturePlaced[x,y])
                {
                    continue;
                }
                if(finishedTiles[x,y] != TileType.Furniture)
                {
                    continue;
                }
                possibleTypes.Clear();
                possibleTypes.Add(FurnitureType.Single);

                if(finishedTiles[x + 1, y] == TileType.Furniture && !furniturePlaced[x + 1, y])
                {
                    possibleTypes.Add(FurnitureType.Horizontal);
                }

                if(finishedTiles[x, y + 1] == TileType.Furniture && !furniturePlaced[x, y + 1])
                {
                    possibleTypes.Add(FurnitureType.Vertical);
                }

                int i = Random.Range(0, possibleTypes.Count);

                FurnitureType type = possibleTypes[i];

                furnitureType[x, y] = type;
                furniturePlaced[x, y] = true; 

                switch (type)
                {
                    case FurnitureType.Single:
                        break;
                    case FurnitureType.Horizontal:
                        finishedTiles[x + 1, y] = TileType.None;
                        furniturePlaced[x + 1, y] = true;
                        break;
                    case FurnitureType.Vertical:
                        furnitureType[x, y + 1] = FurnitureType.None;
                        furniturePlaced[x, y + 1] = true;
                        break;
                }
            }
        }
    }

    public void Space(bool set)
    {
        space.SetActive(set);
    }
}
