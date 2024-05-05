using System;
using UnityEngine;

public class PuzzleState
{
    public int[] Arr { get; private set; }
    public int EmptyTileIndex { get; private set; } // Index of the empty tile

    public PuzzleState()
    {
        Arr = new int[9];
        for (int i = 0; i < Arr.Length; i++)
        {
            Arr[i] = i;
        }
        EmptyTileIndex = Arr.Length - 1;
    }

    public PuzzleState(int[] initialState)
    {
        if (initialState.Length != 9)
        {
            Debug.LogError("Invalid initial state length!");
            return;
        }
        Arr = new int[initialState.Length];
        Array.Copy(initialState, Arr, initialState.Length);
        FindEmptyTileIndex();
    }

    public PuzzleState(PuzzleState other)
    {
        //NumRowsOrCols = other.NumRowsOrCols;
        if(other.Arr.Length != 9)
        {
            Debug.Assert(false);
        }
        Arr = new int[other.Arr.Length];
        Array.Copy(other.Arr, Arr, other.Arr.Length);
        EmptyTileIndex = other.EmptyTileIndex;
    }

    public bool Equals(PuzzleState other)
    {
        if (other == null || Arr.Length != other.Arr.Length)
            return false;

        for (int i = 0; i < Arr.Length; i++)
        {
            if (Arr[i] != other.Arr[i])
                return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (int value in Arr)
        {
            hash = hash * 31 + value.GetHashCode();
        }
        return hash;
    }

    private void FindEmptyTileIndex()
    {
        for (int i = 0; i < Arr.Length; i++)
        {
            if (Arr[i] == Arr.Length - 1)
            {
                EmptyTileIndex = i;
                return;
            }
        }
        EmptyTileIndex = Arr.Length; // Invalid index if empty tile not found (should not happen)
    }

    public void SwapWithEmpty(int index)
    {
        int temp = Arr[index];
        Arr[index] = Arr[EmptyTileIndex];
        Arr[EmptyTileIndex] = temp;
        EmptyTileIndex = index;
    }
}
