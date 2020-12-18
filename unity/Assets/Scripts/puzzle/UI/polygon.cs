namespace puzzle
{
    using System.Collections.Generic;
    using UnityEngine;
    using Util.Geometry;
    using Util.Geometry.Polygon;
    using puzzle.UI.polygonEdge;

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
            CalculateCenterPoint();
            initializeEdges();
        }

        private CalculateCenterPoint(a_vertices)
        {
            float xlow = 2147483647;
            float xhigh = -2147483648;
            float ylow = 2147483647;
            float yhigh = -2147483648;
            for (Vector2 vertex in m_vertices)
            {
               if (vertex.X < xlow)
                {
                    xlow = vertex.X;
                }
                if (vertex.X > xhigh)
                {
                    xhigh = vertex.X;
                }
                if (vertex.Y < ylow)
                {
                    ylow = vertex.Y;
                }
                if (vertex.Y > yhigh)
                {
                    yhigh = vertex.Y;
                }
            }
            self.centerPoint = new Vector2((xlow + xhigh) / 2, (ylow + yhigh) / 2);
        }

        private initializeEdges()
        {
            return;
            //TODO
        }

        public getCenterPoint()
        {
            return this.centerPoint;
        }

        public getEdges()
        {
            return this.edges;
        }

        // Additional methods
        // . . . 
    }
}