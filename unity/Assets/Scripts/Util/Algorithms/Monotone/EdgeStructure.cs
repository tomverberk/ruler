namespace Util.Monotone
{
  using System;
  using System.Collections.Generic;
  using Puzzle;

  /// <summary>
  /// Data structure for storing thet intersecting components of the MakeMonotone
  /// status structure. Comparison is implemented based on the x-coordinate of the
  /// x coordinate of one of the points.
  /// </summary>
  class EdgeStructure : IComparable<EdgeStructure>, IEquatable<EdgeStructure>
  {
    public PolygonEdge edge;

    public VertexStructure helper;

    public VertexStructure vertex1;

    public VertexStructure vertex2;

    public int CompareTo(EdgeStructure other)
    {
      return Comparer<float>.Default.Compare(edge.point1.Pos.x, other.edge.point1.Pos.x);
    }

    public bool Equals(EdgeStructure other)
    {
      return edge.Equals(other.edge) && helper.Equals(other.helper);
    }
  }
}
