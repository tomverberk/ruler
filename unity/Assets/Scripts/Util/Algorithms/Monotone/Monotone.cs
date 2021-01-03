﻿namespace Util.Monotone
{
  using System;
  using System.Collections.Generic;
  using UnityEngine;
  using Util.DataStructures.BST;
  using Util.DataStructures.Queue;
  using Puzzle;

  public partial class Monotone : MonoBehaviour
  {
    /// <summary>
    /// Given a simple Polygon with vertices in CCW order, and edges connecting them in CCW order
    /// with point1 before point2 in the CCW order, compute y-monotone polygons
    /// covering the input polygon.
    /// </summary>
    public List<Polygon> MakeMonotone(Polygon input)
    {
      if (input.points.Count < 3)
      {
        throw new ArgumentException("Needs at least three points in input polynomail.");
      }

      // Initialze the event queue with all points.
      IPriorityQueue<VertexStructure> events = new BinaryHeap<VertexStructure>(YComparer.Instance);

      // C# List has O(1) index access, no problem for running time.
      PolygonEdge prev = input.edges[input.points.Count - 1];

      foreach (PolygonEdge next in input.edges)
      {
        if (prev.point2 != next.point1)
        {
          throw new ArgumentException("Edges are not in correct CCW order");
        }

        VertexType type = DetermineType(prev.point1, next.point1, next.point2);
        events.Push(new VertexStructure
        {
          previous = prev,
          next = next,
          type = type
        });

        prev = next;
      }

      // Initialize the status structure as an empty BST;
      IBST<IntersectingComponent> status = new AATree<IntersectingComponent>();

      // Continue while there are remaining events
      while (events.Count > 0)
      {
        VertexStructure v = events.Pop();
        HandleVertex(status, v);
      }

      return null;
    }

    private VertexType DetermineType(PolygonPoint prev, PolygonPoint curr, PolygonPoint next)
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

    private IntersectingComponent GetLeft(IBST<IntersectingComponent> status, VertexStructure v)
    {
      IntersectingComponent c;

      // Get Left edge of vertex.
      status.FindNextSmallest(new IntersectingComponent
      {
        edge = v.next
      }, out c);
      return c;
    }

    private IntersectingComponent GetRight(IBST<IntersectingComponent> status, VertexStructure v)
    {
      IntersectingComponent c;

      // Get Right edge of vertex.
      status.FindNextBiggest(new IntersectingComponent
      {
        edge = v.next
      }, out c);
      return c;
    }

    private void HandleVertex(IBST<IntersectingComponent> status, VertexStructure v)
    {
      IntersectingComponent e;
      switch (v.type)
      {
        case VertexType.REGULAR:
          // TODO: implement.
          return;

        case VertexType.START:
          status.Insert(new IntersectingComponent
          {
            edge = v.next,
            helper = v,
          });
          return;

        case VertexType.END:
          e = GetLeft(status, v);

          if (e.helper.type == VertexType.MERGE)
          {
            // TODO: insert diagonal
          }

          status.Delete(e);
          return;

        case VertexType.SPLIT:
          e = GetLeft(status, v);

          // TODO: insert diagonal

          e.helper = v;
          status.Insert(new IntersectingComponent
          {
            edge = v.next,
            helper = v,
          });
          return;

        case VertexType.MERGE:
          e = GetRight(status, v);
          if (e.helper.type == VertexType.MERGE)
          {
            // TODO: insert diagonal
          }
          status.Delete(e);

          e = GetLeft(status, v);
          if (e.helper.type == VertexType.MERGE)
          {
            // TODO: insert diagonal
          }
          e.helper = v;

          return;
      }
    }
  }
}