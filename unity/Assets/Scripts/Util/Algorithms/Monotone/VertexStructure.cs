namespace Util.Monotone
{
  using Puzzle;
  using UnityEngine;

  class VertexStructure
  {
    public Vector2 vertex
    {
      get { return next.point1; }
    }

    public EdgeStructure previous;
    public EdgeStructure next;

    public VertexType type;
  }
}