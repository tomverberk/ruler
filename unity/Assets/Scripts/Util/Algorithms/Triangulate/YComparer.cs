namespace Util.Triangulate
{
  using System.Collections.Generic;
  using Puzzle;

  /// <summary>
  /// Comparer instance for comparing polygon points based on their y-coordinate.
  /// </summary>
  class YComparer : IComparer<VertexStructure>
  {
    public static readonly YComparer Instance = new YComparer();
    private YComparer() { }

    public int Compare(VertexStructure p1, VertexStructure p2)
    {
      return -Comparer<float>.Default.Compare(p1.point.Pos.y, p2.point.Pos.y);
    }
  }
}
