namespace Util.Triangulate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Util.DataStructures.Queue;
    using Puzzle;

    class VertexStructure
    {
    public PolygonPoint point;
    public bool isLeft;
    }

    public static class Triangulate
    {
    public static List<Polygon> TriangulatePoly(Polygon pol)
    {
        List<Polygon> triangles = new List<Polygon>();

        // Initialze the event queue with all points.
        IPriorityQueue<VertexStructure> events = new BinaryHeap<VertexStructure>(YComparer.Instance);

        PolygonPoint bottom = pol.points[0];
        PolygonPoint prev = pol.points[pol.points.Count - 1];
        foreach (PolygonPoint curr in pol.points)
        {
        events.Push(new VertexStructure
        {
            point = curr,
            isLeft = (prev.Pos.y > curr.Pos.y),
        });

        prev = curr;
        if (curr.Pos.y < bottom.Pos.y)
        {
            bottom = curr;
        }
        }

        // BEGIN ALGORITHM
        Stack<VertexStructure> s = new Stack<VertexStructure>();
        s.Push(events.Pop());
        VertexStructure ujMinusOne = events.Pop();
        s.Push(ujMinusOne);

        while (events.Count > 1)
        {
        VertexStructure uj = events.Pop();
        if (uj.isLeft != s.Peek().isLeft)
        {
            // Different side
            while (s.Count > 0)
            {
            VertexStructure v = s.Pop();
            if (s.Count > 0)
            {
                List<PolygonPoint> points = new List<PolygonPoint>();
                points.Add(uj.point);
                points.Add(ujMinusOne.point);
                points.Add(v.point);
                triangles.Add(new Polygon(points));
            }
            }
            s.Push(ujMinusOne);
            s.Push(uj);
        }
        else
        {
            // Same side
            VertexStructure v = s.Pop();
            while (s.Count > 0 && PointCanSeePoint(uj.point, v.point, s.Peek().point, v.isLeft))
            {
            VertexStructure vPrime = s.Pop();

            List<PolygonPoint> points = new List<PolygonPoint>();
            points.Add(uj.point);
            points.Add(vPrime.point);
            points.Add(v.point);
            triangles.Add(new Polygon(points));

            v = vPrime;
            }
            s.Push(v);
            s.Push(uj);
        }

        ujMinusOne = uj;
        }

        VertexStructure un = events.Pop();
        VertexStructure v1 = s.Pop();
        while (s.Count > 0)
        {
        VertexStructure v2 = s.Pop();

        List<PolygonPoint> points = new List<PolygonPoint>();
        points.Add(un.point);
        points.Add(v1.point);
        points.Add(v2.point);
        triangles.Add(new Polygon(points));

        v1 = v2;
        }
        // END ALGORITHM


        return triangles;
    }

    // A is highest point, B lowest point, query point should lie on the inside of the line.
    private static bool PointCanSeePoint(PolygonPoint linePointA, PolygonPoint queryPoint, PolygonPoint linePointB, Boolean side)
    {
        float position;
        position = Math.Sign((linePointB.Pos.x - linePointA.Pos.x) * (queryPoint.Pos.y - linePointA.Pos.y) - (linePointB.Pos.y - linePointA.Pos.y) * (queryPoint.Pos.x - linePointA.Pos.x));
        // Right
        if (side)
        {
        if (position == -1)
        {
            return true;
        }
        else
        {
            return false;
        }
        }
        // Left
        else if (!side)
        {
        if (position == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
        }
        return false;
    }

    }
}