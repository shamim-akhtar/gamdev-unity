using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
  public enum Directions
  {
    TOP,
    RIGHT,
    BOTTOM,
    LEFT,
    NONE,
  }

  [SerializeField]
  GameObject topWall;
  [SerializeField]
  GameObject rightWall;
  [SerializeField]
  GameObject bottomWall;
  [SerializeField]
  GameObject leftWall;

  Dictionary<Directions, GameObject> walls =
    new Dictionary<Directions, GameObject>();

  public Vector2Int Index
  {
    get;
    set;
  }

  public bool visited { get; set; } = false;

  Dictionary<Directions, bool> dirflags =
    new Dictionary<Directions, bool>();

  private void Start()
  {
    walls[Directions.TOP] = topWall;
    walls[Directions.RIGHT] = rightWall;
    walls[Directions.BOTTOM] = bottomWall;
    walls[Directions.LEFT] = leftWall;
  }

  private void SetActive(Directions dir, bool flag)
  {
    walls[dir].SetActive(flag);
  }

  public void SetDirFlag(Directions dir, bool flag)
  {
    dirflags[dir] = flag;
    SetActive(dir, flag);
  }
}
