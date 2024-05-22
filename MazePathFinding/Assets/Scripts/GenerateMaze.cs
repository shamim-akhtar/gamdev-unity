using System;
using System.Collections;
using System.Collections.Generic;
using PathFinding;
using Unity.Collections;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField]
    GameObject roomPrefab;

    // The grid.
    Room[,] rooms = null;

    [SerializeField]
    int numX = 10;
    [SerializeField]
    int numY = 10;

    // The room width and height.
    float roomWidth;
    float roomHeight;

    // The stack for backtracking.
    Stack<Room> stack = new Stack<Room>();

    bool generating = false;

    private void GetRoomSize()
    {
        SpriteRenderer[] spriteRenderers =
          roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach (SpriteRenderer ren in spriteRenderers)
        {
            minBounds = Vector3.Min(
              minBounds,
              ren.bounds.min);

            maxBounds = Vector3.Max(
              maxBounds,
              ren.bounds.max);
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;
    }


    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3(
          numX * (roomWidth - 1) / 2,
          numY * (roomHeight - 1) / 2,
          -100.0f);

        float min_value = Mathf.Min(numX * (roomWidth - 1), numY * (roomHeight - 1));
        Camera.main.orthographicSize = min_value * 0.75f;
    }

    private void Start()
    {
        GetRoomSize();

        rooms = new Room[numX, numY];

        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                GameObject room = Instantiate(roomPrefab,
                  new Vector3(i * roomWidth, j * roomHeight, 0.0f),
                  Quaternion.identity);

                room.name = "Room_" + i.ToString() + "_" + j.ToString();
                rooms[i, j] = room.GetComponent<Room>();
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        SetCamera();
    }

    private void RemoveRoomWall(
      int x,
      int y,
      Room.Directions dir)
    {
        if (dir != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(dir, false);
        }

        Room.Directions opp = Room.Directions.NONE;
        switch (dir)
        {
            case Room.Directions.TOP:
                if (y < numY - 1)
                {
                    opp = Room.Directions.BOTTOM;
                    ++y;
                }
                break;
            case Room.Directions.RIGHT:
                if (x < numX - 1)
                {
                    opp = Room.Directions.LEFT;
                    ++x;
                }
                break;
            case Room.Directions.BOTTOM:
                if (y > 0)
                {
                    opp = Room.Directions.TOP;
                    --y;
                }
                break;
            case Room.Directions.LEFT:
                if (x > 0)
                {
                    opp = Room.Directions.RIGHT;
                    --x;
                }
                break;
        }
        if (opp != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(opp, false);
        }
    }

    public List<Tuple<Room.Directions, Room>> GetNeighboursNotVisited(
      int cx, int cy)
    {
        List<Tuple<Room.Directions, Room>> neighbours =
          new List<Tuple<Room.Directions, Room>>();

        foreach (Room.Directions dir in Enum.GetValues(
          typeof(Room.Directions)))
        {
            int x = cx;
            int y = cy;

            switch (dir)
            {
                case Room.Directions.TOP:
                    if (y < numY - 1)
                    {
                        ++y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                              Room.Directions.TOP,
                              rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.RIGHT:
                    if (x < numX - 1)
                    {
                        ++x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                              Room.Directions.RIGHT,
                              rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.BOTTOM:
                    if (y > 0)
                    {
                        --y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                              Room.Directions.BOTTOM,
                              rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.LEFT:
                    if (x > 0)
                    {
                        --x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                              Room.Directions.LEFT,
                              rooms[x, y]));
                        }
                    }
                    break;
            }
        }
        return neighbours;
    }

    private bool GenerateStep()
    {
        if (stack.Count == 0) return true;

        Room r = stack.Peek();
        var neighbours = GetNeighboursNotVisited(r.Index.x, r.Index.y);

        if (neighbours.Count != 0)
        {
            var index = 0;
            if (neighbours.Count > 1)
            {
                index = UnityEngine.Random.Range(0, neighbours.Count);
            }

            var item = neighbours[index];
            Room neighbour = item.Item2;
            neighbour.visited = true;
            RemoveRoomWall(r.Index.x, r.Index.y, item.Item1);

            stack.Push(neighbour);
        }
        else
        {
            stack.Pop();
        }

        return false;
    }

    public void CreateMaze()
    {
        if (generating) return;

        Reset();

        RemoveRoomWall(0, 0, Room.Directions.BOTTOM);

        RemoveRoomWall(numX - 1, numY - 1, Room.Directions.RIGHT);

        stack.Push(rooms[0, 0]);

        StartCoroutine(Coroutine_Generate());
    }

    IEnumerator Coroutine_Generate()
    {
        generating = true;
        bool flag = false;
        while (!flag)
        {
            flag = GenerateStep();
            //yield return new WaitForSeconds(0.05f);
            yield return null;
        }

        generating = false;
    }

    private void Reset()
    {
        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                rooms[i, j].SetDirFlag(Room.Directions.TOP, true);
                rooms[i, j].SetDirFlag(Room.Directions.RIGHT, true);
                rooms[i, j].SetDirFlag(Room.Directions.BOTTOM, true);
                rooms[i, j].SetDirFlag(Room.Directions.LEFT, true);
                rooms[i, j].visited = false;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!generating)
            {
                CreateMaze();
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            RaycastAndSetStart();
        }
    }

    #region PathFinding
    private Vector2Int startPos = Vector2Int.zero;
    private Vector2Int goalPos = Vector2Int.zero;

    private void RaycastAndSetStart()
    {
        Vector2 rayPos = new Vector2(
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0.0f);

        if(hit)
        {
            GameObject obj = hit.transform.gameObject;
            Room room = obj.GetComponent<Room>();
            rooms[startPos.x, startPos.y].ResetMarker();
            rooms[goalPos.x, goalPos.y].ResetMarker();

            startPos.x = goalPos.x;
            startPos.y = goalPos.y;

            goalPos.x = room.Index.x;
            goalPos.y = room.Index.y;

            rooms[startPos.x, startPos.y].SetMarkerAsStart();
            rooms[goalPos.x, goalPos.y].SetMarkerAsGoal();

            FindPath();
        }
    }

    public List<Node<Vector2Int>> GetNeighbours(int xx, int yy)
    {
        List<Node<Vector2Int>> neighbours = new List<Node<Vector2Int>>();
        foreach(Room.Directions dir in Enum.GetValues(typeof(Room.Directions)))
        {
            int x = xx;
            int y = yy;

            switch(dir)
            {
                case Room.Directions.TOP:
                    if(y < numY - 1)
                    {
                        if (!rooms[x,y].GetDirFlag(dir))
                        {
                            ++y;
                            neighbours.Add(new RoomNode(new Vector2Int(x,y), this));
                        }
                    }
                    break;
                case Room.Directions.RIGHT:
                    if (x < numX - 1)
                    {
                        if (!rooms[x, y].GetDirFlag(dir))
                        {
                            ++x;
                            neighbours.Add(new RoomNode(new Vector2Int(x, y), this));
                        }
                    }
                    break;
                case Room.Directions.BOTTOM:
                    if (y > 0)
                    {
                        if (!rooms[x, y].GetDirFlag(dir))
                        {
                            --y;
                            neighbours.Add(new RoomNode(new Vector2Int(x, y), this));
                        }
                    }
                    break;
                case Room.Directions.LEFT:
                    if (x > 0)
                    {
                        if (!rooms[x, y].GetDirFlag(dir))
                        {
                            --x;
                            neighbours.Add(new RoomNode(new Vector2Int(x, y), this));
                        }
                    }
                    break;
            }
        }

        return neighbours;
    }

    // Cost functions.
    public static float ManhattanCost(
        Vector2Int a,
        Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public static float CostBetweenTwoCells(
        Vector2Int a,
        Vector2Int b)
    {
        return Mathf.Sqrt(
            (a.x-b.x) * (a.x-b.x) + 
            (a.y-b.y) * (a.y-b.y)
        );
    }

    public void OnChangeCurrentNode(PathFinder<Vector2Int>.PathFinderNode node)
    {
        int x = node.Location.Value.x;
        int y = node.Location.Value.y;
        rooms[x, y].SetFloorColor(Color.magenta);
    }

    public void OnAddToOpenList(PathFinder<Vector2Int>.PathFinderNode node)
    {
        int x = node.Location.Value.x;
        int y = node.Location.Value.y;
        rooms[x,y].SetFloorColor(Color.cyan);
    }

    public void OnAddToClosedList(PathFinder<Vector2Int>.PathFinderNode node)
    {
        int x = node.Location.Value.x;
        int y = node.Location.Value.y;
        rooms[x, y].SetFloorColor(Color.grey);
    }

    void ResetColor()
    {
        for(int i = 0; i < numX; i++)
        {
            for(int j = 0; j < numY; j++)
            {
                rooms[i, j].ResetFloor();
            }    
        }
    }

    AStarPathFinder<Vector2Int> pathFinder = new AStarPathFinder<Vector2Int>();
    void FindPath()
    {
        if(pathFinder.Status == PathFinderStatus.RUNNING)
        {
            Debug.Log("PathFinder is running. Cannot start a new pathfinding now");
            return;
        }

        pathFinder.HeuristicCost = ManhattanCost;
        pathFinder.NodeTraversalCost = CostBetweenTwoCells;

        ResetColor();

        pathFinder.onAddToCloasedList = OnAddToClosedList;
        pathFinder.onAddToOpenList = OnAddToOpenList;
        pathFinder.onChangeCurrentNode = OnChangeCurrentNode;

        pathFinder.Initialise(new RoomNode(startPos, this),
            new RoomNode(goalPos, this));
        StartCoroutine(Coroutine_FindPathStep());
    }

    IEnumerator Coroutine_FindPathStep()
    {
        while(pathFinder.Status == PathFinderStatus.RUNNING)
        {
            pathFinder.Step();
            // We purposely make it slower so that
            // we can visualize the search.
            yield return new WaitForSeconds(0.05f);
        }

        if(pathFinder.Status == PathFinderStatus.SUCCESS)
        {
            StartCoroutine(Coroutine_OnSuccessPathFinding());
        }

        if(pathFinder.Status == PathFinderStatus.FAILURE)
        {
            OnFailurePathFinding();
        }

        IEnumerator Coroutine_OnSuccessPathFinding()
        {
            PathFinder<Vector2Int>.PathFinderNode node = pathFinder.CurrentNode;
            List<Vector2Int> reverse_indices = new List<Vector2Int>();

            while(node != null)
            {
                reverse_indices.Add(node.Location.Value);
                node = node.Parent;
            }

            for(int i = reverse_indices.Count - 1; i >= 0; i--)
            {
                rooms[reverse_indices[i].x, reverse_indices[i].y].SetFloorColor(Color.black);
                yield return new WaitForSeconds(0.05f);
            }
        }

        void OnFailurePathFinding()
        {
            Debug.Log("Cannot find path as no valid path exists");
        }
    }

    #endregion
}
