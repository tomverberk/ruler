namespace Puzzle
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Util.Geometry.Polygon;

    public class Polygon : Polygon2D
    {
        public List<PolygonPoint> points = new List<PolygonPoint>();
        public List<PolygonEdge> edges = new List<PolygonEdge>();
        public Vector2 centerPoint;

        private PuzzleController m_gameController;

        // base Constructor
        public Polygon() { }

        // Constructor of the points
        public Polygon(List<PolygonPoint> a_vertices)
        {
            foreach (PolygonPoint point in a_vertices)
            {
                points.Add(point);
            }
            CalculateCenterPoint(a_vertices);
            initializeEdges(a_vertices);
            return;
        }

        private void CalculateCenterPoint(List<PolygonPoint> a_vertices)
        {
            float xlow = 2147483647;
            float xhigh = -2147483648;
            float ylow = 2147483647;
            float yhigh = -2147483648;
            foreach (var point in a_vertices)
            {
               if (point.Pos.x < xlow)
                {
                    xlow = point.Pos.x;
                }
                if (point.Pos.x > xhigh)
                {
                    xhigh = point.Pos.x;
                }
                if (point.Pos.y < ylow)
                {
                    ylow = point.Pos.y;
                }
                if (point.Pos.y > yhigh)
                {
                    yhigh = point.Pos.y;
                }
            }

            float x = (xlow + xhigh) / 2;
            float y = (ylow + yhigh) / 2;
            this.centerPoint = new Vector2(x, y);

            return;
        }

        private void initializeEdges(List<PolygonPoint> a_vertices)
        {
            Vector2 vertex1 = new Vector2(0, 0);
            Vector2 vertex2= new Vector2(0, 0);
            Vector2 firstVertex = new Vector2(0, 0);
            int i = 0;
            foreach (var vertex in a_vertices)
            {
                vertex2 = vertex.Pos;
                firstVertex = vertex.Pos;
                break;
            }
            foreach (var vertex in a_vertices)
            {
                if (i >= 1)
                {
                    vertex1 = vertex.Pos;
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

        void Awake()
        {
            centerPoint = new Vector2();
        }

        void OnMouseDown()
        {
            m_gameController.m_carrying_triangle = true;
            m_gameController.m_triangle = this;
            // ???? this was in example code
            //m_controller.m_line.SetPosition(0, Pos);
        }

        void OnMouseEnter()
        {
            if (m_gameController.m_triangle == null) return;

            m_gameController.m_locked = true;
            //m_gameController.m_secondPoint = this;
            //m_gameController.m_line.SetPosition(1, Pos);
        }



        // Additional methods
        // . . . 
    }
}