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
        if (prev.edge.point2.Pos != nextEdge.point1.Pos)
        {
          Debug.Log("INVALID:");
          Debug.Log(String.Format("({0}, {1}) to ({2}, {3})", prev.edge.point2.Pos.x, prev.edge.point2.Pos.y, nextEdge.point1.Pos.x, nextEdge.point1.Pos.y));
          PolygonEdge e = nextEdge;
          Debug.Log(String.Format("({0}, {1}) to ({2}, {3})", e.point1.Pos.x, e.point1.Pos.y, e.point2.Pos.x, e.point2.Pos.y));
          e = prev.edge;
          Debug.Log(String.Format("({0}, {1}) to ({2}, {3})", e.point1.Pos.x, e.point1.Pos.y, e.point2.Pos.x, e.point2.Pos.y));
          throw new ArgumentException("Edges are not in correct CCW order");
        }
        else if (prev.edge.point1.Pos == nextEdge.point2.Pos)
        {
          throw new ArgumentException("Loop detected.");
        }
        else
        {
          // Debug.Log("Correct");
        }

        EdgeStructure next;
        if (nextEdge.point1.Pos == first.edge.point1.Pos && nextEdge.point2.Pos == first.edge.point2.Pos)
        {
          // Debug.Log(String.Format("Equals: "));
          // PolygonEdge e = nextEdge;
          // Debug.Log(String.Format("({0}, {1}) to ({2}, {3})", e.point1.Pos.x, e.point1.Pos.y, e.point2.Pos.x, e.point2.Pos.y));
          // e = first.edge;
          // Debug.Log(String.Format("({0}, {1}) to ({2}, {3})", e.point1.Pos.x, e.point1.Pos.y, e.point2.Pos.x, e.point2.Pos.y));
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
        // Debug.Log(String.Format("Definition ({0}, {1}) {2}", v.vertex.Pos.x, v.vertex.Pos.y, v.type));
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
        Debug.Log(String.Format("Event ({0}, {1})", v.vertex.Pos.x, v.vertex.Pos.y));
        HandleVertex(status, result, v);
        last = v;
      }
      result.Add(InsertDiagonal(last.next.vertex2, last));

      return result;
    }

    private static VertexType DetermineType(PolygonPoint prev, PolygonPoint curr, PolygonPoint next)
    {
      // Debug.Log(String.Format("prev ({0}, {1})", prev.Pos.x, prev.Pos.y));
      // Debug.Log(String.Format("curr ({0}, {1})", curr.Pos.x, curr.Pos.y));
      // Debug.Log(String.Format("next ({0}, {1})", next.Pos.x, next.Pos.y));
      // Compute delta vectors for the vertices.
      Vector2 d1 = curr.Pos - prev.Pos;
      // Debug.Log(String.Format("D1 ({0}, {1})", d1.x, d1.y));
      Vector2 d2 = next.Pos - curr.Pos;
      // Debug.Log(String.Format("D2 ({0}, {1})", d2.x, d2.y));

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