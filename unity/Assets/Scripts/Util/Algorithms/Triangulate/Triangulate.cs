namespace Util.Triangulate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Util.DataStructures.Queue;
    using Util.Geometry.Polygon;

    class VertexStructure
    {
    public Vector2 point;
    public bool isLeft;
    }

    public static class Triangulate
    {
    public static List<Polygon2D> TriangulatePoly(Polygon2D pol)
    {
        List<Polygon2D> triangles = new List<Polygon2D>();

        // Initialze the event queue with all points.
        IPriorityQueue<VertexStructure> events = new BinaryHeap<VertexStructure>(YComparer.Instance);

        Vector2 first = Vector2.zero;
        Vector2 prev = Vector2.zero;
        bool hasPrev = false;
        foreach (Vector2 curr in pol.Vertices)
        {
            if (hasPrev) {
                events.Push(new VertexStructure
                {
                    point = curr,
                    isLeft = (prev.y > curr.y),
                });
            } else {
                first = curr;
            }

            prev = curr;
            hasPrev = true;
        }
        events.Push(new VertexStructure
        {
            point = first,
            isLeft = (prev.y > first.y),
        });

        // BEGIN ALGORITHM
        Stack<VertexStructure> s = new Stack<VertexStructure>();
        s.Push(events.Pop()); // Push top
        VertexStructure ujMinusOne = events.Pop();
        s.Push(ujMinusOne); // push uj minus one

        while (events.Count > 1)
        {
            VertexStructure uj = events.Pop();
            if (uj.isLeft != s.Peek().isLeft) // compare uj with ujminusone
            {
                // Different side
                while (s.Count > 0)
                {
                    VertexStructure v = s.Pop();
                    if (s.Count > 0)
                    {
                        List<Vector2> points = new List<Vector2>();
                        points.Add(uj.point);
                        points.Add(s.Peek().point);
                        points.Add(v.point);
                        triangles.Add(new Polygon2D(points));
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

                    List<Vector2> points = new List<Vector2>();
                    points.Add(uj.point);
                    points.Add(vPrime.point);
                    points.Add(v.point);
                    triangles.Add(new Polygon2D(points));

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

            List<Vector2> points = new List<Vector2>();
            points.Add(un.point);
            points.Add(v1.point);
            points.Add(v2.point);
            triangles.Add(new Polygon2D(points));

            v1 = v2;
        }
        // END ALGORITHM


        return triangles;
    }

    // A is highest point, B lowest point, query point should lie on the inside of the line.
    private static bool PointCanSeePoint(Vector2 linePointA, Vector2 queryPoint, Vector2 linePointB, Boolean isleft)
    {
        float position;
        position = Math.Sign((linePointB.x - linePointA.x) * (queryPoint.y - linePointA.y) - (linePointB.y - linePointA.y) * (queryPoint.x - linePointA.x));
        // Right
        if (!isleft)
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
        else if (isleft)
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