using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBoard : MonoBehaviour
{
    public GameObject tilePrefab;
    public List<Texture> puzzleImages = new List<Texture>();

    public Text statusText;
    public Text numberOfMovesText;

    private bool solved = false;
    private PuzzleState goalState = new PuzzleState();
    private PuzzleState randomizedState;

    private int numberOfMoves = 0;


    private int currentTextureIndex = 0;

    private List<GameObject> tiles = new List<GameObject>();

    private List<Vector3> tilesLocation = new List<Vector3>()
    {
        new Vector3(-1.0f, 1.0f, 0.0f),
        new Vector3( 0.0f, 1.0f, 0.0f),
        new Vector3( 1.0f, 1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3( 0.0f, 0.0f, 0.0f),
        new Vector3( 1.0f, 0.0f, 0.0f),
        new Vector3(-1.0f, -1.0f, 0.0f),
        new Vector3( 0.0f, -1.0f, 0.0f),
        new Vector3( 1.0f, -1.0f, 0.0f),
    };

    private PuzzleState currentState;
    private bool randomizing = false;

    private AStarPathFinder<PuzzleState> pathFinder = new AStarPathFinder<PuzzleState>();
    private bool solvingUsingAStarInProgress = false;
    private int numberOfMovesAStar = 0;

    // Start is called before the first frame update
    void Start()
    {
        PuzzleState.CreateNeighbourIndices();
        CreateTiles();
        Init();
    }

    private void Init()
    {
        SetTexture();
        SetPuzzleState(new PuzzleState());

        statusText.text = "Puzzle in solved state. Randomize to play!";
        numberOfMoves = 0;
        solved = true;

        numberOfMovesText.gameObject.SetActive(false);
    }

    void CreateTiles()
    {
        for(int i = 0; i < tilesLocation.Count; i++)
        {
            GameObject tile = Instantiate(tilePrefab);
            tile.name = i.ToString();
            tile.transform.parent = transform;
            tiles.Add(tile);
            tiles[i].transform.position = tilesLocation[i];
        }
    }

    void SetTexture()
    {
        Texture mainTexture = puzzleImages[currentTextureIndex];
        int numRows = 3;
        int tileSize = mainTexture.width / numRows;

        for(int i = 0; i < 8; i++)
        {
            GameObject tile = tiles[i];
            Renderer renderer = tile.GetComponent<Renderer>();
            Material material = renderer.material;

            // Calculate the texture coordinates.
            int row = i / numRows;
            int col = i % numRows;

            float xMin = col * (float)tileSize/mainTexture.width;
            float yMin = 1.0f - (row + 1) * (float)tileSize / mainTexture.height;

            material.mainTexture = mainTexture;
            material.mainTextureScale = new Vector2((float)tileSize/mainTexture.width,
                (float)tileSize/mainTexture.height);
            material.mainTextureOffset = new Vector2(xMin, yMin);
        }

        // We want the last tile to be empty and hence transparent in color.
        tiles[8].GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public void NextImage()
    {
        if (solvingUsingAStarInProgress) return;
        Debug.Log("NextImage.");
        currentTextureIndex++;
        if(currentTextureIndex == puzzleImages.Count)
        {
            currentTextureIndex = 0;
        }
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            NextImage();
        }

        if(Input.GetMouseButtonDown(0))
        {
            GameObject obj = PickTile();
            if(obj != null && !solved)
            {
                int empty = currentState.EmptyTileIndex;
                List<int> neighbours = PuzzleState.GetNeighbourIndices(empty);

                for(int i = 0; i < neighbours.Count; i++) 
                {
                    if(obj.name == currentState.Arr[neighbours[i]].ToString())
                    {
                        numberOfMoves++;
                        numberOfMovesText.gameObject.SetActive(true);
                        numberOfMovesText.text = numberOfMoves.ToString();
                        currentState.SwapWithEmpty(neighbours[i]);
                        SetPuzzleState(currentState, 0.2f);

                        solved = currentState.Equals(goalState);

                        if(solved)
                        {
                            statusText.gameObject.SetActive(true);
                            statusText.text = "Yay! You have solved the puzzle. Click Next to play a new puzzle";
                        }
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Coroutine_Randomize(100, 0.02f));
        }
    }

    public void SetPuzzleState(PuzzleState state)
    {
        currentState = state;
        for(int i = 0; i < state.Arr.Length; i++)
        {
            tiles[state.Arr[i]].transform.position = tilesLocation[i];
        }
    }

    public void SetPuzzleState(PuzzleState state, float duration)
    {
        currentState = state;
        for (int i = 0; i < state.Arr.Length; i++)
        {
            StartCoroutine(Coroutine_MoveOverSeconds(
                tiles[state.Arr[i]],
                tilesLocation[i],
                duration));
        }
    }

    private GameObject PickTile()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            return hitObject;
        }
        return null;
    }

    public IEnumerator Coroutine_MoveOverSeconds(
        GameObject objectToMove,
        Vector3 end,
        float seconds)
    {
        float elaspedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;

        while(elaspedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(
                startingPos, end,
                elaspedTime / seconds);
            elaspedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }

    IEnumerator Coroutine_Randomize(int depth, float durationPerMove)
    {
        randomizing = true;
        int i = 0;
        while (i < depth)
        {
            List<PuzzleState> neighbours = PuzzleState.GetNeighbourOfEmpty(currentState);

            // get a random index.
            int rn = Random.Range(0, neighbours.Count);
            currentState.SwapWithEmpty(neighbours[rn].EmptyTileIndex);
            i++;
            SetPuzzleState(currentState, durationPerMove);
            yield return new WaitForSeconds(durationPerMove);
        }
        randomizing = false;
        solved = false;
        statusText.gameObject.SetActive(false);

        randomizedState = new PuzzleState(currentState);
    }

    public void Randomize()
    {
        if (solvingUsingAStarInProgress) return;
        Debug.Log("Randomize.");
        if (randomizing) return;
        StartCoroutine(Coroutine_Randomize(100, 0.02f));
    }

    public void Reset()
    {
        if(solvingUsingAStarInProgress) return;
        SetPuzzleState(new PuzzleState(randomizedState));
        numberOfMoves = 0;
        numberOfMovesText.gameObject.SetActive(false);
        statusText.gameObject.SetActive(false);
    }

    static public float ManhattanCost(PuzzleState a, PuzzleState goal)
    {
        return (float)a.GetManhattanCost();
    }

    static public float TraversalCost(PuzzleState a, PuzzleState b)
    {
        return 1.0f;
    }

    IEnumerator Coroutine_Solve()
    {
        statusText.gameObject.SetActive(true);
        statusText.text = "Finding solution using A* search.";
        pathFinder.Initialise(new PuzzleNode(new PuzzleState(randomizedState)),
            new PuzzleNode(new PuzzleState()));

        while(pathFinder.Status == PathFinderStatus.RUNNING)
        {
            pathFinder.Step();
            yield return null;
        }

        if(pathFinder.Status == PathFinderStatus.SUCCESS)
        {
            // We will show the solution.
            StartCoroutine(Coroutine_ShowSolution());
        }
        if(pathFinder.Status != PathFinderStatus.FAILURE)
        {
            Debug.Log("No solution found!");
        }
    }

    IEnumerator Coroutine_ShowSolution()
    {
        List<PuzzleState> reverseSolution = new List<PuzzleState>();
        PathFinder<PuzzleState>.PathFinderNode node = pathFinder.CurrentNode;

        while(node != null)
        {
            reverseSolution.Add(node.Location.Value);
            node = node.Parent;
        }

        statusText.text = "Solution found! The puzzle can be solved in " + 
            (reverseSolution.Count - 1).ToString() + " moves.";

        if(reverseSolution.Count > 0)
        {
            SetPuzzleState(reverseSolution[reverseSolution.Count - 1]);
        }
        if(reverseSolution.Count > 2)
        {
            for(int i = reverseSolution.Count - 2; i >= 0; i -= 1)
            {
                numberOfMovesAStar++;
                numberOfMovesText.text = numberOfMovesAStar.ToString();
                numberOfMovesText.gameObject.SetActive(true);

                SetPuzzleState(reverseSolution[i], 0.5f);
                yield return new WaitForSeconds(1.0f);
            }
        }
        statusText.text = "Puzzle in solved state. Randomize to play!";
        solvingUsingAStarInProgress = false;
    }

    public void Solve()
    {
        if (solvingUsingAStarInProgress) return;
        numberOfMovesAStar = 0;
        solvingUsingAStarInProgress = true;

        pathFinder.HeuristicCost = ManhattanCost;
        pathFinder.NodeTraversalCost = TraversalCost;

        StartCoroutine(Coroutine_Solve());
    }
}
