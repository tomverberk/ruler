namespace Puzzle
{
    using General.Menu;
    using General.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.Geometry.Polygon;
    using Util.Algorithms.Polygon;
    using Util.Geometry;

    public class PuzzleController : MonoBehaviour
    {

        public LineRenderer m_line;

        [SerializeField]
        private GameObject m_edgeMesh;
        [SerializeField]
        private ButtonContainer m_advanceButton;

        //internal HullPoint m_firstPoint;
        //internal HullPoint m_secondPoint;
        internal Polygon m_triangle;
        internal bool m_locked;
        internal bool m_carrying_triangle;

        private List<PolygonPoint> m_points;
        private List<Polygon> triangulation;
        private List<PolygonEdge> p_edges;
        
        
        //private HashSet<LineSegment> m_segments;
        //private Polygon2D m_solutionHull;


        void Start()
        {
            print("Beginning");
            // get unity objects
            m_points = FindObjectsOfType<PolygonPoint>().ToList();

            foreach (PolygonPoint point in m_points)
            {
                print("Position of the point = " + point.Pos);
            }
                print("size of m_points");
            print(m_points.Count);

            // create a polygon from the points
            Polygon polygon = createPolygonFromPoints(m_points);

            print("size of edges in polygon");

            // draw the edges of the polygon
            p_edges = polygon.edges;

            print(p_edges.Count);

            drawEdgesOfPolygon(p_edges);

            // TODO MAKE THIS METHOD IN OTHER FILE
            polygonLevel level = new polygonLevel(polygon);
            triangulation = level.triangulation;

            // place the triangles from the triangulations in the file.
            //drawTriangles(triangulation);

            // disable advance button
            m_advanceButton.Disable();


            // enable advace button
            m_advanceButton.Enable();

            m_locked = false;
        }



        void Update()
        {
            //TODO CREATE MOUSE INTERACTION
            if (m_locked && !Input.GetMouseButton(0))
            {
                // TODO Place puzzelpeace and reset values
                m_locked = false;
                m_triangle = null;
                m_carrying_triangle = false;

            }
            else if (Input.GetMouseButton(0))
            {
                // TODO something idk

            }


            if ((m_locked && !Input.GetMouseButton(0)) || Input.GetMouseButtonUp(0))
            {
                //TODO something idk
            }
        }

        public void drawEdgesOfPolygon(List<PolygonEdge> edges){
            foreach (PolygonEdge edge in edges){
                print("position of the edge" + edge.point1.Pos + " , " + edge.point2.Pos);
                
                var drawedEdge = Instantiate(m_edgeMesh, Vector3.forward, Quaternion.identity) as GameObject;
                drawedEdge.transform.parent = this.transform;

                //drawedEdge.GetComponent<HullSegment>().Segment = segment;

                //var roadmeshScript = drawedEdge.GetComponent<ReshapingMesh>();
                //roadmeshScript.CreateNewMesh(edge.point1.transform.position, edge.point2.transform.position);

            }
        }

        public Polygon createPolygonFromPoints(List<PolygonPoint> points)
        {
            Polygon polygon = new Polygon(points);
            return polygon;
        }

        public void drawTriangles(List<Polygon> triangles)
        {
            foreach (Polygon triangle in triangles)
            {
                // Set a new centerpoint for each triangle
                
            }
        }

        /*
         * Method when placing a triangle
         */
        public void PlaceTriangle(Triangle t)
        {
            //TODO EVERYTHING
            
        }

        public void RemoveSegment(Triangle t)
        {
            //TODO REWRITE
            //m_segments.Remove(a_segment.Segment);
            CheckSolution();
        }

        //TODO
        // link advanceButton idk how
        public void CheckSolution()
        {
            if (CheckPlacement())
            {
                //m_advanceButton.Enable();
            }
            else
            {
                //m_advanceButton.Disable();
            }
        }

        private bool CheckPlacement()
        {
            // TODO quick check


            // TODO slow check
            // also check reverse
            return true;
        }
    }
}
