namespace Util.Monotone
{
  using Puzzle;

  class VertexStructure
  {
    public PolygonPoint vertex
    {
      get { return next.edge.point1; }
    }

    public IntersectingComponent previous;
    public IntersectingComponent next;

    public VertexType type;
  }
}