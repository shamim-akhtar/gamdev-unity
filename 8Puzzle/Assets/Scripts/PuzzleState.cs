using System;
using UnityEngine;

public class PuzzleState : IEquatable<PuzzleState>
{
    private int[] arr; // The integer array representing the internal state of the puzzle
    public int NumRowsOrCols { get; } // The number of rows or columns of the puzzle
    public int EmptyTileIndex { get; private set; } // Index of the empty tile

    public int[] Arr { get { return arr; } }

    public PuzzleState(int rowsOrCols)
    {
        NumRowsOrCols = rowsOrCols;
        arr = new int[NumRowsOrCols * NumRowsOrCols];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = i;
        }
        EmptyTileIndex = arr.Length - 1;
    }

    public PuzzleState(int[] initialState, int numRowsOrCols)
    {
        NumRowsOrCols = numRowsOrCols;
        if (initialState.Length != NumRowsOrCols * NumRowsOrCols)
        {
            Debug.LogError("Invalid initial state length!");
            return;
        }
        arr = new int[initialState.Length];
        Array.Copy(initialState, arr, initialState.Length);
        FindEmptyTileIndex();
    }

    public PuzzleState(PuzzleState other)
    {
        NumRowsOrCols = other.NumRowsOrCols;
        arr = new int[other.arr.Length];
        Array.Copy(other.arr, arr, other.arr.Length);
        EmptyTileIndex = other.EmptyTileIndex;
    }

    public bool Equals(PuzzleState other)
    {
        if (other == null || arr.Length != other.arr.Length)
            return false;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != other.arr[i])
                return false;
        }
        return true;
    }

    public override bool Equals(object obj)
    {
        if (obj is PuzzleState state)
        {
            return Equals(state);
        }
        return false;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (int value in arr)
        {
            hash = hash * 31 + value.GetHashCode();
        }
        return hash;
    }

    private void FindEmptyTileIndex()
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == arr.Length - 1)
            {
                EmptyTileIndex = i;
                return;
            }
        }
        EmptyTileIndex = arr.Length; // Invalid index if empty tile not found (should not happen)
    }

    public void SwapWithEmpty(int index)
    {
        int temp = arr[index];
        arr[index] = arr[EmptyTileIndex];
        arr[EmptyTileIndex] = temp;
        EmptyTileIndex = index;
    }

    public int GetManhattanDistance()
    {
        int distance = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == arr.Length - 1) continue; // Skip empty tile

            int currentRow = i / NumRowsOrCols;
            int currentCol = i % NumRowsOrCols;
            int targetRow = arr[i] / NumRowsOrCols;
            int targetCol = arr[i] % NumRowsOrCols;

            distance += Mathf.Abs(currentRow - targetRow) + Mathf.Abs(currentCol - targetCol);
        }
        return distance;
    }
}
