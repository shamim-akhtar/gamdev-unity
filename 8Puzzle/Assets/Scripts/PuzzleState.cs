using System;
using System.Linq;
using UnityEngine;

public class PuzzleState
{
    public int[] Arr { get; private set; } = new int[9];
    public int EmptyTileIndex { get; private set; } // Index of the empty tile

    public PuzzleState()
    {
        for (int i = 0; i < Arr.Length; i++)
        {
            Arr[i] = i;
        }
        EmptyTileIndex = Arr.Length - 1;
    }

    public PuzzleState(PuzzleState other)
    {
        Array.Copy(other.Arr, Arr, other.Arr.Length);
        EmptyTileIndex = other.EmptyTileIndex;
    }

    public bool Equals(PuzzleState other)
    {
        return other != null && Arr.SequenceEqual(other.Arr);
    }

    private void FindEmptyTileIndex()
    {
        EmptyTileIndex = Array.IndexOf(Arr, Arr.Length - 1);
    }

    public void SwapWithEmpty(int index)
    {
        (Arr[index], Arr[EmptyTileIndex]) = (Arr[EmptyTileIndex], Arr[index]);
        EmptyTileIndex = index;
    }
}
