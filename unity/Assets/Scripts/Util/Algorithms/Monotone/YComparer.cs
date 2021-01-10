namespace Util.Monotone
{
  using System.Collections.Generic;

  /// <summary>
  /// Comparer instance for comparing polygon points based on their y-coordinate.
  /// </summary>
  class YComparer : IComparer<VertexStructure>
  {
    public static readonly YComparer Instance = new YComparer();
    private YComparer() { }

    public int Compare(VertexStructure p1, VertexStructure p2)
    {
      return -Comparer<float>.Default.Compare(p1.vertex.y, p2.vertex.y);
    }
  }
}
