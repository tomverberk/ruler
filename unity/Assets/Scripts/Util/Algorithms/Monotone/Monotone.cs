namespace Util.Monotone
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.DataStructures.BST;
    using Util.DataStructures.Queue;
    using Util.Geometry;
    using Util.Geometry.DCEL;
    using Util.Geometry.Polygon;

    public static class Monotone
    {
        /// <summary>
        /// Given a simple Polygon with vertices in CCW order, and dcel connecting them in CCW order
        /// with point1 before point2 in the CCW order, compute y-monotone polygons
        /// covering the input polygon.
        /// </summary>
        public static List<Polygon2D> MakeMonotone(Polygon2D input)
        {
            if (input.Vertices.Count < 3)
            {
                throw new ArgumentException("Needs at least three points in input polynomail.");
            }
            DCEL dcel = new DCEL();

            // Initialze the event queue with all points.
            IPriorityQueue<VertexStructure> events = new BinaryHeap<VertexStructure>(YComparer.Instance);

            ICollection<LineSegment> segments = input.Segments;
            LineSegment last = segments.Last();
            EdgeStructure lastStruct = new EdgeStructure
            {
                point1 = last.Point1,
                point2 = last.Point2,
            };
            EdgeStructure prev = lastStruct;

            foreach (LineSegment nextEdge in input.Segments)
            {
                dcel.AddSegment(nextEdge);

                EdgeStructure next;
                if (nextEdge == last)
                {
                    next = lastStruct;
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

            // Continue while there are remaining events
            while (events.Count > 0)
            {
                VertexStructure v = events.Pop();
                HandleVertex(status, dcel, v);
            }

            return CreatePolygons(dcel);
        }

        private static List<Polygon2D> CreatePolygons(DCEL dcel)
        {
            List<Polygon2D> result = new List<Polygon2D>();
            foreach (Face f in dcel.InnerFaces)
            {
                Polygon2D p = f.PolygonWithoutHoles;
                if (p.IsClockwise())
                {
                    p.Reverse();
                }
                result.Add(p);
            }

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

            double angle = Vector2.SignedAngle(d1, d2);
            if (angle < 0)
            {
                angle += 360;
            }

            if (d1.y > 0)
            {
                // Start or split vertex.
                if (angle > 180)
                {
                    return VertexType.SPLIT;
                }
                else
                {
                    return VertexType.START;
                }
            }
            else
            {
                // End or merge vertex.
                if (angle > 180)
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

        private static void InsertDiagonal(VertexStructure first, VertexStructure last, DCEL dcel)
        {
            dcel.AddEdge(first.vertex, last.vertex);
        }

        private static void HandleVertex(IBST<EdgeStructure> status, DCEL dcel, VertexStructure v)
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

                        Debug.Log(String.Format("Upper: {0} to {1} ({2})", upper.point1, upper.point2, upper.helper));
                        Debug.Log(String.Format("Lower: {0} to {1} ({2})", lower.point1, lower.point2, lower.helper));
                        if (upper.helper != null && upper.helper.type == VertexType.MERGE)
                        {
                            InsertDiagonal(v, upper.helper, dcel);
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
                            InsertDiagonal(v, e.helper, dcel);
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
                        InsertDiagonal(v, e.helper, dcel);
                    }

                    status.Delete(e);
                    return;

                case VertexType.SPLIT:
                    e = GetLeft(status, v);

                    InsertDiagonal(v, e.helper, dcel);

                    e.helper = v;
                    status.Insert(v.next);
                    v.next.helper = v;
                    return;

                case VertexType.MERGE:
                    e = GetRight(status, v);
                    if (e.helper.type == VertexType.MERGE)
                    {
                        InsertDiagonal(v, e.helper, dcel);
                    }
                    status.Delete(e);

                    e = GetLeft(status, v);
                    if (e.helper.type == VertexType.MERGE)
                    {
                        InsertDiagonal(v, e.helper, dcel);
                    }
                    e.helper = v;

                    return;
            }
        }
    }
}