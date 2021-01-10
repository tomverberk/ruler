namespace Puzzle
{
    using System.Collections.Generic;
    using UnityEngine;
    using Util.Geometry.Polygon;
    using Util.Geometry;

    public class Polygon : MonoBehaviour
    {
        public List<PolygonPoint> points = new List<PolygonPoint>();
        public List<PolygonEdge> edges = new List<PolygonEdge>();
        public Vector3 centerPoint;
        public PolygonPoint top;
        public PolygonPoint bottom;
        public List<Vector2> actualPoints = new List<Vector2>();
        public Polygon2D polygon;
        public GameObject drawedTriangle;

        private PuzzleController m_gameController;

        /// <summary>
        /// Stores lighthouse position. Updates vision after a change in position.
        /// </summary>
        public Vector3 Pos
        {
            get
            {
                return gameObject.transform.position;
            }
            set
            {
                var current = transform.position;
                gameObject.transform.position = value;

                // update vision polygon
                m_gameController.UpdatePolygon(this, current);
            }
        }

        // base Constructor
        public Polygon() { }

        // Constructor of the points
        public Polygon(List<PolygonPoint> a_vertices)
        {
            print(" a polygon is created ");

            foreach (PolygonPoint point in a_vertices)
            {
                points.Add(point);
                actualPoints.Add(point.Pos);
                //print("position of the point = " + point.Pos);

            }
            CalculateTopBottom(a_vertices);
            initializeEdges(a_vertices);
            polygon = new Polygon2D(actualPoints);
            return;
        }

        public void CalculateTopBottom(List<PolygonPoint> a_vertices)
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
                    this.bottom = point;
                }
                if (point.Pos.y > yhigh)
                {
                    yhigh = point.Pos.y;
                    this.top = point;
                }
            }

            float x = (xlow + xhigh) / 2;
            float y = (ylow + yhigh) / 2;
            float z = 10;
            this.centerPoint = new Vector3(x, y, z);

            return;
        }

        public void initializeEdges(List<PolygonPoint> a_vertices)
        {
            PolygonPoint point1 = new PolygonPoint(new Vector2(0, 0));
            PolygonPoint point2 = new PolygonPoint(new Vector2(0, 0));
            PolygonPoint firstPoint = new PolygonPoint(new Vector2(0,0));
            int i = 0;
            foreach (PolygonPoint vertex in a_vertices)
            {
                point2 = vertex;
                firstPoint = vertex;
                break;
            }
            foreach (PolygonPoint vertex in a_vertices)
            {
                if (i >= 1)
                {
                    point1 = vertex;
                    PolygonEdge edge = new PolygonEdge(point2, point1);

                    edges.Add(edge);

                    point2 = point1;
                    i += 1;
                } else
                {
                    i += 1;
                }

            }
            if (i > 1)
            {
                PolygonEdge edge = new PolygonEdge(point1, firstPoint);
                edges.Add(edge);
            }
            return;
        }

        public void SetCenterPoint(Vector3 pos)
        {
            this.centerPoint = pos;
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
            m_gameController = FindObjectOfType<PuzzleController>();
            //m_gameController.m_triangle = this;
            //m_gameController.m_carrying_triangle = true;
            print("I have awoken");
        }

        void OnMouseDown()
        {
            print("My mouse is down in the Polygon");
            m_gameController.m_carrying_triangle = true;
            m_gameController.m_triangle = this;
            // ???? this was in example code
            //m_controller.m_line.SetPosition(0, Pos);
        }

        void OnMouseUp()
        {
            print("I have unclicked the mouse in this polygon");

            //if (m_gameController.m_triangle == null) return;

            //m_gameController.m_secondPoint = this;
            //m_gameController.m_line.SetPosition(1, Pos);
        }

        void OnMouseEnter()
        {
            print("Something is entering me");
        }



        // Additional methods
        // . . .
    }
}
