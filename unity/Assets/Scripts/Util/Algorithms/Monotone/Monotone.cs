namespace Util.Monotone
{
  using System;
  using System.Collections.Generic;
  using UnityEngine;
  using Util.DataStructures.BST;
  using Util.DataStructures.Queue;
  using Puzzle;

  public static class Monotone
  {
    /// <summary>
    /// Given a simple Polygon with vertices in CCW order, and edges connecting them in CCW order
    /// with point1 before point2 in the CCW order, compute y-monotone polygons
    /// covering the input polygon.
    /// </summary>
    public static List<Polygon> MakeMonotone(Polygon input)
    {
      if (input.points.Count < 3)
      {
        throw new ArgumentException("Needs at least three points in input polynomail.");
      }
      List<Polygon> result = new List<Polygon>();

      // Initialze the event queue with all points.
      IPriorityQueue<VertexStructure> events = new BinaryHeap<VertexStructure>(YComparer.Instance);

      // C# List has O(1) index access, no problem for running time.
      EdgeStructure first = new EdgeStructure
      {
        edge = input.edges[input.edges.Count - 1],
      };
      EdgeStructure prev = first;

      foreach (PolygonEdge nextEdge in input.edges)
      {
        if (prev.edge.point2 != nextEdge.point1)
        {
          throw new ArgumentException("Edges are not in correct CCW order");
        }

        EdgeStructure next;
        if (nextEdge == first.edge)
        {
          next = first;
        }
        else
        {
          next = new EdgeStructure
          {
            edge = nextEdge,
          };
        }

        VertexType type = DetermineType(prev.edge.point1, next.edge.point1, next.edge.point2);
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

    private static VertexType DetermineType(PolygonPoint prev, PolygonPoint curr, PolygonPoint next)
    {
      // Compute delta vectors for the vertices.
      Vector2 d1 = curr.Pos - prev.Pos;
      Vector2 d2 = next.Pos - curr.Pos;

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

    private static Polygon InsertDiagonal(VertexStructure first, VertexStructure last)
    {
      Debug.Log(String.Format("DIAGONAL ({0}, {1}) to ({2}, {3})", first.vertex.Pos.x, first.vertex.Pos.y, last.vertex.Pos.x, last.vertex.Pos.y));
      List<PolygonPoint> vertices = new List<PolygonPoint>();

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
        edge = new PolygonEdge(first.vertex, last.vertex),
        vertex1 = first,
        vertex2 = last,
      };
      last.previous = first.next;

      return new Polygon(vertices);
    }

    private static void HandleVertex(IBST<EdgeStructure> status, List<Polygon> result, VertexStructure v)
    {
      EdgeStructure e;
      switch (v.type)
      {
        case VertexType.REGULAR:
          // Check if Polygon lies locally right based on CCW property:
          if (v.previous.edge.point1.Pos.y > v.next.edge.point2.Pos.y)
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