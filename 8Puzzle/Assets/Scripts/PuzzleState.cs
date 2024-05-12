using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleState : IEquatable<PuzzleState>
{
    public int[] Arr { get; private set; } = new int[9];
    public int EmptyTileIndex { get; private set; }

    public PuzzleState()
    {
        Arr = Enumerable.Range(0, Arr.Length).ToArray();
        EmptyTileIndex = Arr.Length - 1;
    }

    public PuzzleState(PuzzleState other)
    {
        other.Arr.CopyTo(Arr, 0);
        EmptyTileIndex = other.EmptyTileIndex;
    }

    public bool Equals(PuzzleState other)
    {
        return other != null && Arr.SequenceEqual(other.Arr);
    }

    public override bool Equals(object obj) => Equals(obj as PuzzleState);
    public override int GetHashCode()
    {
        int hc = Arr.Length;
        foreach (int val in Arr)
        {
            hc = unchecked(hc * 314159 + val);
        }
        return hc;
    }

    public void FindEmptyTileIndex()
    {
        EmptyTileIndex = Array.IndexOf(Arr, Arr.Length - 1);
    }

    public void SwapWithEmpty(int index)
    {
        (Arr[index], Arr[EmptyTileIndex]) = (Arr[EmptyTileIndex], Arr[index]);
        EmptyTileIndex = index;
    }

    #region Neighbours.

    static private Dictionary<int, List<int>> edges = new Dictionary<int, List<int>>();

    // Create the neighbour indices. You should call this function once
    // at the start to generate the neighbouring indices.
    // You can also separate this functionality into a new static class.
    public static void CreateNeighbourIndices(int rowsOrCols = 3)
    {
        for(int i = 0; i < rowsOrCols; i++)
        {
            for(int j = 0; j < rowsOrCols; j++)
            {
                int index = i * rowsOrCols + j;
                List<int> neighbourIndices = new List<int>();

                if (i - 1 >= 0) neighbourIndices.Add((i-1)*rowsOrCols + j);
                if (i + 1 < rowsOrCols) neighbourIndices.Add((i + 1) * rowsOrCols + j);
                if (j - 1 >= 0) neighbourIndices.Add(i * rowsOrCols + j - 1);
                if (j + 1 < rowsOrCols) neighbourIndices.Add(i * rowsOrCols + j + 1);

                edges[index] = neighbourIndices;
            }
        }
    }

    public static List<int> GetNeighbourIndices(int id)
    {
        return edges[id];
    }

    public static List<PuzzleState> GetNeighbourOfEmpty(PuzzleState state)
    {
        List<PuzzleState> neighbours = new List<PuzzleState> ();
        int empty = state.EmptyTileIndex;

        List<int> neighbourIndices = GetNeighbourIndices(empty);

        foreach(var neighbourIndex in neighbourIndices)
        {
            PuzzleState new_state = new PuzzleState(state);
            new_state.SwapWithEmpty(neighbourIndex);
            neighbours.Add(new_state);
        }

        return neighbours;
    }
    #endregion

    // The Manhattan cost function.
    public int GetManhattanCost()
    {
        int rowsOrCols = 3;
        int cost = 0;

        for(int i = 0; i < Arr.Length; i++)
        {
            int v = Arr[i];
            if (v == Arr.Length - 1) continue;

            int gx = v % rowsOrCols;
            int gy = v / rowsOrCols;

            int x = i % rowsOrCols;
            int y = i / rowsOrCols;

            int mancost = System.Math.Abs(x - gx) + System.Math.Abs(y - gy);
            cost += mancost;
        }
        return cost;
    }
}
