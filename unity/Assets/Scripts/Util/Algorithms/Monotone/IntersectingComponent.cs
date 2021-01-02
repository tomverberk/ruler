namespace Util.Monotone
{
  using System;
  using Puzzle;

  /// <summary>
  /// Data structure for storing thet iontersecting components of the MakeMonotone
  /// status structure. Comparison is implemented based on the x-coordinate of the
  /// ??? TODO: sort on?
  /// </summary>
  class IntersectingComponent : IComparable<IntersectingComponent>, IEquatable<IntersectingComponent>
  {
    public PolygonEdge edge;
    public PolygonPoint helper;

    public int CompareTo(IntersectingComponent other)
    {
      return 0;
    }

    public bool Equals(IntersectingComponent other)
    {
      return edge.Equals(other.edge) && helper.Equals(other.helper);
    }
  }
}
