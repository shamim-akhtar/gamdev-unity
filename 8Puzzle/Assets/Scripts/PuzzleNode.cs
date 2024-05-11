using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

public class PuzzleNode : Node<PuzzleState>
{
    public PuzzleNode(PuzzleState state): base(state)
    {

    }

    public override List<Node<PuzzleState>> GetNeighbours()
    {
        List<Node<PuzzleState>> neighbours = new List<Node<PuzzleState>>();
        List<PuzzleState> neighbour_states = PuzzleState.GetNeighbourOfEmpty(Value);

        for(int i = 0; i < neighbour_states.Count; i++)
        {
            neighbours.Add(new PuzzleNode(neighbour_states[i]));
        }
        return neighbours;
    }
}
