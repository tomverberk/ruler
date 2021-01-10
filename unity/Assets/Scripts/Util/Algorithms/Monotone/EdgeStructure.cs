namespace Util.Monotone
{
  using System;
  using System.Collections.Generic;
  using UnityEngine;

  /// <summary>
  /// Data structure for storing thet intersecting components of the MakeMonotone
  /// status structure. Comparison is implemented based on the x-coordinate of the
  /// x coordinate of one of the points.
  /// </summary>
  class EdgeStructure : IComparable<EdgeStructure>, IEquatable<EdgeStructure>
  {
    public Vector2 point1;

    public Vector2 point2;
    public VertexStructure helper;

    public VertexStructure vertex1;

    public VertexStructure vertex2;

    public int CompareTo(EdgeStructure other)
    {
      return Comparer<float>.Default.Compare(point1.x, other.point1.x);
    }

    public bool Equals(EdgeStructure other)
    {
      return point1.Equals(other.point1) && point2.Equals(other.point2) && helper.Equals(other.helper);
    }
  }
}
