namespace Util.Monotone
{
  using System;
  using System.Collections.Generic;
  using UnityEngine;
  using Util.DataStructures.BST;
  using Util.DataStructures.Queue;
  using Puzzle;

  public partial class Monotone : MonoBehaviour
  {
    public List<Polygon> MakeMonotone(Polygon input)
    {
      if (input.points.Count < 3)
      {
        throw new ArgumentException("Needs at least three points in input polynomail.");
      }

      // Initialze the event queue with all points.
      IPriorityQueue<VertexStructure> events = new BinaryHeap<VertexStructure>(YComparer.Instance);

      // C# List has O(1) index access, no problem for running time.
      PolygonPoint prev = input.points[input.points.Count - 2];
      PolygonPoint curr = input.points[input.points.Count - 1];

      foreach (PolygonPoint next in input.points)
      {
        VertexType type = DetermineType(prev, curr, next);
        events.Push(new VertexStructure
        {
          vertex = curr,
          type = type
        });

        prev = curr;
        curr = next;
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
        if (d1.x > 0)
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
        if (d1.x > 0)
        {
          return VertexType.MERGE;
        }
        else
        {
          return VertexType.END;
        }
      }
    }

    private void HandleVertex(IBST<IntersectingComponent> status, VertexStructure p)
    {

    }
  }
}