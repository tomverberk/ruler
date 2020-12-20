namespace Puzzle
{
    using System.Collections.Generic;
    using UnityEngine;
    using Util.Geometry.Polygon;

    public class Polygon : Polygon2D
    {
        public List<PolygonEdge> edges;
        public Vector2 centerPoint;

        private PuzzleController m_gameController;

        // base Constructor
        public Polygon() { }

        // Constructor of the points
        public Polygon(IEnumerable<Vector2> a_vertices)
        {
            foreach (var v in a_vertices)
                AddVertex(new Vector2(v.x, v.y));
            CalculateCenterPoint(a_vertices);
            initializeEdges(a_vertices);
            return;
        }

        private void CalculateCenterPoint(IEnumerable<Vector2> a_vertices)
        {
            float xlow = 2147483647;
            float xhigh = -2147483648;
            float ylow = 2147483647;
            float yhigh = -2147483648;
            foreach (var vertex in a_vertices)
            {
               if (vertex.x < xlow)
                {
                    xlow = vertex.x;
                }
                if (vertex.x > xhigh)
                {
                    xhigh = vertex.x;
                }
                if (vertex.y < ylow)
                {
                    ylow = vertex.y;
                }
                if (vertex.y > yhigh)
                {
                    yhigh = vertex.y;
                }
            }

            float x = (xlow + xhigh) / 2;
            float y = (ylow + yhigh) / 2;
            this.centerPoint = new Vector2(x, y);

            return;
        }

        private void initializeEdges(IEnumerable<Vector2> a_vertices)
        {
            Vector2 vertex1 = new Vector2(0, 0);
            Vector2 vertex2= new Vector2(0, 0);
            Vector2 firstVertex = new Vector2(0, 0);
            int i = 0;
            foreach (var vertex in a_vertices)
            {
                vertex2 = vertex;
                firstVertex = vertex;
                break;
            }
            foreach (var vertex in a_vertices)
            {
                if (i >= 1)
                {
                    vertex1 = vertex;
                    edges.Add(new PolygonEdge(vertex1, vertex2));
                    vertex2 = vertex1;
                    i += 1;
                } else
                {
                    i += 1;
                }
                
            }
            if (i > 1)
            {
                edges.Add(new PolygonEdge(point1: vertex2, point2: firstVertex));
            }
            return;
        }

        public Vector2? getCenterPoint()
        {
            return this.centerPoint;
        }

        public List<PolygonEdge> getEdges()
        {
            return this.edges;
        }

        // Additional methods
        // . . . 
    }
}