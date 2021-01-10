namespace Util.Monotone
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using UnityEngine;
  using Util.DataStructures.BST;
  using Util.DataStructures.Queue;
  using Util.Geometry;
  using Util.Geometry.Polygon;

  public static class Monotone
  {
    /// <summary>
    /// Given a simple Polygon with vertices in CCW order, and edges connecting them in CCW order
    /// with point1 before point2 in the CCW order, compute y-monotone polygons
    /// covering the input polygon.
    /// </summary>
    public static List<Polygon2D> MakeMonotone(Polygon2D input)
    {
      if (input.Vertices.Count < 3)
      {
        throw new ArgumentException("Needs at least three points in input polynomail.");
      }
      List<Polygon2D> result = new List<Polygon2D>();

      // Initialze the event queue with all points.
      IPriorityQueue<VertexStructure> events = new BinaryHeap<VertexStructure>(YComparer.Instance);

      ICollection<LineSegment> segments = input.Segments;
      LineSegment lastEdge = segments.Last();
      // C# List has O(1) index access, no problem for running time.
      EdgeStructure first = new EdgeStructure
      {
        point1 = lastEdge.Point1,
        point2 = lastEdge.Point2,
      };
      EdgeStructure prev = first;

      foreach (LineSegment nextEdge in input.Segments)
      {
        EdgeStructure next;
        if (nextEdge.Point1 == first.point1 && nextEdge.Point2 == first.point2)
        {
          next = first;
        }
        else
        {
          next = new EdgeStructure
          {
            point1 = nextEdge.Point1,
            point2 = nextEdge.Point2,
          };
        }

        VertexType type = DetermineType(prev.point1, next.point1, next.point2);
        VertexStructure v = new VertexStructure
        {
          previous = prev,
          next = next,
          type = type
        };
        events.Push(v);
        prev.vertex2 = v;
        next.vertex1 = v;

        prev = next;
      }

      // Initialize the status structure as an empty BST;
      IBST<EdgeStructure> status = new AATree<EdgeStructure>();

      VertexStructure last = null;

      // Continue while there are remaining events
      while (events.Count > 0)
      {
        VertexStructure v = events.Pop();
        HandleVertex(status, result, v);
        last = v;
      }
      result.Add(InsertDiagonal(last.next.vertex2, last));

      return result;
    }

    private static VertexType DetermineType(Vector2 prev, Vector2 curr, Vector2 next)
    {
      Vector2 d1 = curr - prev;
      Vector2 d2 = next - curr;

      if (d1.y * d2.y >= 0)
      {
        // Y coordinate of both delta vectors have same sign -> regular vertex.
        return VertexType.REGULAR;
      }

      if (d1.y > 0)
      {
        // Start or split vertex.
        if (d1.x < 0)
        {
          return VertexType.START;
        }
        else
        {
          return VertexType.SPLIT;
        }
      }
      else
      {
        // End or merge vertex.
        if (d1.x < 0)
        {
          return VertexType.MERGE;
        }
        else
        {
          return VertexType.END;
        }
      }
    }

    private static EdgeStructure GetLeft(IBST<EdgeStructure> status, VertexStructure v)
    {
      EdgeStructure c;

      // Get Left edge of vertex.
      status.FindNextSmallest(v.next, out c);
      return c;
    }

    private static EdgeStructure GetRight(IBST<EdgeStructure> status, VertexStructure v)
    {
      EdgeStructure c;

      // Get Right edge of vertex.
      status.FindNextBiggest(v.next, out c);
      return c;
    }

    private static Polygon2D InsertDiagonal(VertexStructure first, VertexStructure last)
    {
      List<Vector2> vertices = new List<Vector2>();

      VertexStructure current = first;
      while (current != last)
      {
        vertices.Add(current.vertex);
        current = current.next.vertex2;
      }
      vertices.Add(last.vertex);

      // Update polynomial for next traversals.
      first.next = new EdgeStructure
      {
        point1 = first.vertex,
        point2 = last.vertex,
        vertex1 = first,
        vertex2 = last,
      };
      last.previous = first.next;

      return new Polygon2D(vertices);
    }

    private static void HandleVertex(IBST<EdgeStructure> status, List<Polygon2D> result, VertexStructure v)
    {
      EdgeStructure e;
      switch (v.type)
      {
        case VertexType.REGULAR:
          // Check if Polygon lies locally right based on CCW property:
          if (v.previous.point1.y > v.next.point2.y)
          {
            EdgeStructure upper = v.previous;
            EdgeStructure lower = v.next;

            if (upper.helper.type == VertexType.MERGE)
            {
              result.Add(InsertDiagonal(v, upper.helper));
            }
            status.Delete(upper);

            status.Insert(lower);
            lower.helper = v;
          }
          else
          {
            e = GetLeft(status, v);
            if (e.helper.type == VertexType.MERGE)
            {
              result.Add(InsertDiagonal(v, e.helper));
            }
            e.helper = v;
          }
          return;

        case VertexType.START:
          status.Insert(v.next);
          v.next.helper = v;
          return;

        case VertexType.END:
          e = GetLeft(status, v);

          if (e.helper.type == VertexType.MERGE)
          {
            result.Add(InsertDiagonal(v, e.helper));
          }

          status.Delete(e);
          return;

        case VertexType.SPLIT:
          e = GetLeft(status, v);

          result.Add(InsertDiagonal(v, e.helper));

          e.helper = v;
          status.Insert(v.next);
          v.next.helper = v;
          return;

        case VertexType.MERGE:
          e = GetRight(status, v);
          if (e.helper.type == VertexType.MERGE)
          {
            result.Add(InsertDiagonal(v, e.helper));
          }
          status.Delete(e);

          e = GetLeft(status, v);
          if (e.helper.type == VertexType.MERGE)
          {
            result.Add(InsertDiagonal(v, e.helper));
          }
          e.helper = v;

          return;
      }
    }
  }
}