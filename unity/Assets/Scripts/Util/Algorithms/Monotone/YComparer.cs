namespace Util.Monotone
{
  using System.Collections.Generic;
  using Puzzle;

  /// <summary>
  /// Comparer instance for comparing polygon points based on their y-coordinate.
  /// </summary>
  class YComparer : IComparer<PolygonPoint>
  {
    public static readonly YComparer Instance = new YComparer();
    private YComparer() { }

    public int Compare(PolygonPoint p1, PolygonPoint p2)
    {
      return Comparer<float>.Default.Compare(p1.Pos.y, p2.Pos.y);
    }
  }
}
