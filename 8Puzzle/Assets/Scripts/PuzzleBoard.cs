using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    public List<Texture> puzzleImage = new List<Texture>();
    public GameObject tilePrefab;
    public TextMeshProUGUI statusText;

    private List<GameObject> tiles = new List<GameObject>();

    private List<Vector3> tilesLocations = new List<Vector3>()
    {
        new (-1, 1, 0),
        new ( 0, 1, 0),
        new ( 1, 1, 0),
        new (-1, 0, 0),
        new (0, 0, 0),
        new (1, 0, 0),
        new (-1, -1, 0),
        new (0, -1, 0),
        new (1, -1, 0),
    };

    private PuzzleState currentState = null;
    private bool solved = false; 
    private PuzzleState solvedState = new PuzzleState();
    private Texture currentTexture = null;
    private int currentTextureIndex = 0;
    private bool randomizing = false;

    // Start is called before the first frame update
    void Start()
    {
        PuzzleState.CreateNeighbourIndices(3);

        currentTexture = puzzleImage[currentTextureIndex];
        CreateTiles();
        Init();
    }

    private void Init()
    {
        SetTexture();
        SetPuzzleState(new PuzzleState());
        solved = true;

        statusText.gameObject.SetActive(true);
        statusText.text = "Puzzle in solved state. Randomize to play!";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Randomize();
        }
        if(Input.GetMouseButtonDown(0))
        {
            GameObject obj = Pick3D();
            if(obj != null && !solved)
            {
                //Debug.Log(obj.name);
                // check for tile movement.
                int zero = currentState.EmptyTileIndex;
                List<int> neighbours = PuzzleState.GetNeighbourIndices(zero);

                for (int i = 0; i < neighbours.Count; ++i)
                {
                    if (obj.name == currentState.Arr[neighbours[i]].ToString())
                    {
                        currentState.SwapWithEmpty(neighbours[i]);
                        SetPuzzleState(currentState, 0.1f);
                        solved = currentState.Equals(solvedState);
                        if(solved)
                        {
                            //Debug.Log("Puzzle Solved");
                            statusText.gameObject.SetActive(true);
                            statusText.text = "Yay! You have solved the Puzzle. Click Next to play a new Puzzle!";
                        }
                    }
                }
            }
        }
    }

    void CreateTiles()
    {
        for (int i = 0; i < 9; ++i)
        {
            GameObject tile = Instantiate(tilePrefab);
            tile.name = i.ToString();
            tile.transform.parent = transform;

            // Add tile to the list and position it
            tiles.Add(tile);
            tiles[i].transform.position = tilesLocations[i];
        }
    }

    void SetTexture()
    {
        Texture mainTexture = currentTexture;

        // Assuming the texture is 900x900 pixels and you want to divide it into a 3x3 grid of tiles
        int numRowsCols = 3; // Number of rows and columns in the grid
        int tileSize = currentTexture.width / numRowsCols;

        for (int i = 0; i < 8; ++i)
        {
            GameObject tile = tiles[i];

            // Get the Renderer component of the tile GameObject
            Renderer renderer = tile.GetComponent<Renderer>();
            Material material = renderer.material;

            // Calculate texture coordinates for the current tile
            int row = i / numRowsCols;
            int col = i % numRowsCols;

            // Calculate texture coordinates based on row and column
            float xMin = col * (float)tileSize / mainTexture.width;
            float yMin = 1.0f - (row + 1) * (float)tileSize / mainTexture.height; // Unity uses bottom-left as (0,0)

            // Set texture coordinates for the material
            material.mainTexture = mainTexture;
            material.mainTextureScale = new Vector2((float)tileSize / mainTexture.width, (float)tileSize / mainTexture.height);
            material.mainTextureOffset = new Vector2(xMin, yMin);
        }
        // Last one we set the material to transparent color.
        tiles[8].GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public void SetPuzzleState(PuzzleState state)
    {
        currentState = state;
        for (int i = 0; i < state.Arr.Length; ++i)
        {
            tiles[state.Arr[i]].transform.position = tilesLocations[i];
        }
    }


    IEnumerator Coroutine_Randomize(int depth, float durationPerMove)
    {
        randomizing = true;
        int i = 0;
        while (i < depth)
        {
            List<PuzzleState> neighbours = PuzzleState.GetNeighbourOfEmpty(currentState);

            // get a random neignbour.
            int rn = Random.Range(0, neighbours.Count);
            currentState.SwapWithEmpty(neighbours[rn].EmptyTileIndex);
            i++;
            SetPuzzleState(currentState, durationPerMove);
            yield return new WaitForSeconds(durationPerMove);
        }
        randomizing = false;
    }

    public IEnumerator Coroutine_MoveOverSeconds(
        GameObject objectToMove,
        Vector3 end,
        float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(
                startingPos, end,
                (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }

    public void SetPuzzleState(PuzzleState state, float duration)
    {
        for (int i = 0; i < state.Arr.Length; ++i)
        {
            StartCoroutine(Coroutine_MoveOverSeconds(
                tiles[state.Arr[i]],
                tilesLocations[i],
                duration));
        }
    }
    public static GameObject Pick3D()
    {
        // Get the mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

        // Cast a ray from the camera through the mouse position into the 3D world
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // Create a variable to store information about the object hit by the ray
        RaycastHit hitInfo;

        // Perform the raycast and check if it hits an object
        if (Physics.Raycast(ray, out hitInfo))
        {
            // Retrieve the GameObject that was hit by the raycast
            GameObject hitObject = hitInfo.collider.gameObject;

            // Return the GameObject that was hit
            return hitObject;
        }
        else
        {
            // If no object was hit, return null
            return null;
        }
    }

    public void Randomize()
    {
        if (randomizing) return;
        StartCoroutine(Coroutine_Randomize(100, 0.02f));
        solved = false;

        statusText.gameObject.SetActive(false);
    }

    public void NextImage()
    {
        if (randomizing) return;

        currentTextureIndex++;
        if(currentTextureIndex == puzzleImage.Count)
        {
            currentTextureIndex = 0;
        }
        currentTexture = puzzleImage[currentTextureIndex];
        Init();
    }
}
