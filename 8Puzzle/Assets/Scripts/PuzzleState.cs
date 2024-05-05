using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents the state of a puzzle board with a 3x3 grid of tiles.
/// Each state contains an array representing the tile arrangement and the index of the empty tile.
/// </summary>
public class PuzzleState
{
    // Static dictionary to store precalculated neighbour indices for each tile
    static private Dictionary<int, List<int>> edges = new Dictionary<int, List<int>>();

    /// <summary>
    /// Gets the array representing the tile arrangement in the puzzle state.
    /// </summary>
    public int[] Arr { get; private set; } = new int[9];

    /// <summary>
    /// Gets the index of the empty tile in the puzzle state.
    /// </summary>
    public int EmptyTileIndex { get; private set; }

    /// <summary>
    /// Initializes a new instance of the PuzzleState class with tiles arranged in order.
    /// </summary>
    public PuzzleState()
    {
        Arr = Enumerable.Range(0, Arr.Length).ToArray();
        EmptyTileIndex = Arr.Length - 1;
    }

    /// <summary>
    /// Initializes a new instance of the PuzzleState class by copying another puzzle state.
    /// </summary>
    /// <param name="other">The puzzle state to copy.</param>
    public PuzzleState(PuzzleState other)
    {
        Arr = other.Arr.ToArray();
        EmptyTileIndex = other.EmptyTileIndex;
    }

    /// <summary>
    /// Checks if this puzzle state is equal to another puzzle state by comparing their tile arrangements.
    /// </summary>
    /// <param name="other">The other puzzle state to compare.</param>
    /// <returns>True if the puzzle states are equal; otherwise, false.</returns>
    public bool Equals(PuzzleState other)
    {
        return other != null && Arr.SequenceEqual(other.Arr);
    }

    /// <summary>
    /// Finds and updates the index of the empty tile in the puzzle state.
    /// </summary>
    private void FindEmptyTileIndex()
    {
        EmptyTileIndex = Array.IndexOf(Arr, Arr.Length - 1);
    }

    /// <summary>
    /// Swaps the specified tile with the empty tile in the puzzle state.
    /// </summary>
    /// <param name="index">The index of the tile to swap with the empty tile.</param>
    public void SwapWithEmpty(int index)
    {
        (Arr[index], Arr[EmptyTileIndex]) = (Arr[EmptyTileIndex], Arr[index]);
        EmptyTileIndex = index;
    }

    /// <summary>
    /// Generates a list of neighboring puzzle states by swapping the empty tile with its neighbors.
    /// </summary>
    /// <param name="state">The current puzzle state.</param>
    /// <returns>A list of neighboring puzzle states.</returns>
    public static List<PuzzleState> GetNeighbourOfEmpty(PuzzleState state)
    {
        List<PuzzleState> neighbours = new List<PuzzleState>();
        int zero = state.EmptyTileIndex;
        List<int> neighbourIndices = GetNeighbourIndices(zero);

        foreach (var neighbourIndex in neighbourIndices)
        {
            PuzzleState new_state = new PuzzleState(state);
            new_state.SwapWithEmpty(neighbourIndex);
            neighbours.Add(new_state);
        }

        return neighbours;
    }

    /// <summary>
    /// Retrieves a list of neighbor indices for the specified tile index based on precalculated edges.
    /// </summary>
    /// <param name="id">The index of the tile to get neighbor indices for.</param>
    /// <returns>A list of neighbor indices.</returns>
    public static List<int> GetNeighbourIndices(int id)
    {
        return edges[id];
    }

    /// <summary>
    /// Precalculates and stores the neighboring indices for each tile based on the grid dimensions.
    /// </summary>
    /// <param name="rowsOrCols">The number of rows or columns in the puzzle grid.</param>
    public static void CreateNeighbourIndices(int rowsOrCols)
    {
        for (int i = 0; i < rowsOrCols; i++)
        {
            for (int j = 0; j < rowsOrCols; j++)
            {
                int index = i * rowsOrCols + j;
                List<int> neighbourIndices = new List<int>();

                if (i - 1 >= 0) neighbourIndices.Add((i - 1) * rowsOrCols + j);
                if (i + 1 < rowsOrCols) neighbourIndices.Add((i + 1) * rowsOrCols + j);
                if (j - 1 >= 0) neighbourIndices.Add(i * rowsOrCols + j - 1);
                if (j + 1 < rowsOrCols) neighbourIndices.Add(i * rowsOrCols + j + 1);

                edges[index] = neighbourIndices;
            }
        }
    }
}
