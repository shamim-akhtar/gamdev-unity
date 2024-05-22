using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

public class RoomNode : Node<Vector2Int>
{
    GenerateMaze maze;

    public RoomNode(Vector2Int value, GenerateMaze maze) : base(value)
    {
        this.maze = maze;
    }

    public override List<Node<Vector2Int>> GetNeighbours()
    {
        return maze.GetNeighbours(Value.x, Value.y);
    }
}
