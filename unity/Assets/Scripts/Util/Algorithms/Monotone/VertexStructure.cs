namespace Util.Monotone
{
  using Puzzle;

  class VertexStructure
  {
    public PolygonPoint vertex
    {
      get { return next.point1; }
    }

    public PolygonEdge previous;
    public PolygonEdge next;

    public VertexType type;

    public IntersectingComponent left;
  }
}