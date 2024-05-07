using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleNode : PathFinding.Node<PuzzleState>
{
    public PuzzleNode(PuzzleState value) : base(value)
    {
    }

    public override List<Node<PuzzleState>> GetNeighbours()
    {
        List<Node<PuzzleState>> neighbours = new List<Node<PuzzleState>>();

        List<PuzzleState> neighbour_states = PuzzleState.GetNeighbourOfEmpty(Value);
        for(int i = 0; i < neighbour_states.Count; ++i)
        {
            neighbours.Add(new PuzzleNode(neighbour_states[i]));
        }

        return neighbours;
    }
}
