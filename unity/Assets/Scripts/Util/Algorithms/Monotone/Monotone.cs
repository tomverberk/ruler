namespace Util.Monotone
{
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using Util.DataStructures.BST;
  using Util.DataStructures.Queue;
  using Puzzle;

  public class Monotone : MonoBehaviour
  {
    public List<Polygon> MakeMonotone(Polygon input)
    {
      // Initialze the event queue with all points.
      IPriorityQueue<PolygonPoint> events = new BinaryHeap<PolygonPoint>(YComparer.Instance);
      foreach (PolygonPoint p in input.points)
      {
        events.Push(p);
      }

      // Initialize the status structure as an empty BST;
      IBST<IntersectingComponent> status = new AATree<IntersectingComponent>();

      // Continue while there are remaining events
      while (events.Count > 0)
      {
        PolygonPoint p = events.Pop();
        HandleVertex(status, p);
      }

      return null;
    }

    private void HandleVertex(IBST<IntersectingComponent> status, PolygonPoint p)
    {

    }
  }
}