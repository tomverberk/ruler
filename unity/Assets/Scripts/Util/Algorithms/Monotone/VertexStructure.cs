namespace Util.Monotone
{
  using Puzzle;

  class VertexStructure
  {
    public PolygonPoint vertex
    {
      get { return next.edge.point1; }
    }

    public EdgeStructure previous;
    public EdgeStructure next;

    public VertexType type;
  }
}