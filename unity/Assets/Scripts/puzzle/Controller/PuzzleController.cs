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
    using General.Controller;
    using UnityEngine.SceneManagement;


    public class PuzzleController : MonoBehaviour
    {

        public LineRenderer m_line;

        [SerializeField]
        private GameObject m_edgeMesh;
        [SerializeField]
        private GameObject m_pointPrefab;
        [SerializeField]
        private ButtonContainer m_advanceButton;

        [SerializeField]
        private List<PolygonLevel> m_levels;
        [SerializeField]
        private string m_victoryScene;


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

        private List<GameObject> instantObjects;

        protected int m_levelCounter = 0;


        void Start()
        {
            print("Beginning");
            // get unity objects
            m_points = new List<PolygonPoint>();
            instantObjects = new List<GameObject>();

            InitLevel();

            foreach (var point in m_points)
            {
                print("position of the points" + point.Pos);
            }

            PolygonEdge test = new PolygonEdge(m_points[0], m_points[1]);
            print("ik zou niet null moeten zijn " + test);
            

            // The points are not null

            // create a polygon from the points
            Polygon polygon = createPolygonFromPoints(m_points);

            // The edges are somehow null

            // draw the edges of the polygon
            p_edges = polygon.edges;
            print("size of edges in polygon" + p_edges.Count);
            foreach(var edge in p_edges)
            {
                print("ik denk dat ik null ben" + edge);
            }

            drawEdgesOfPolygon(p_edges);

            // TODO MAKE THIS METHOD IN OTHER FILE
            //PolygonLevel level = new PolygonLevel(polygon);
            //triangulation = level.triangulation;

            // place the triangles from the triangulations in the file.
            //drawTriangles(triangulation);

            // disable advance button
            m_advanceButton.Disable();


            // enable advace button
            m_advanceButton.Enable();

            m_locked = false;
        }

        public void InitLevel()
        {
            if (m_levelCounter >= m_levels.Count)
            {
                SceneManager.LoadScene(m_victoryScene);
                return;
            }

            // clear old level
            Clear();

            PolygonPoint polygonPoint;
            // initialize settlements
            foreach (var point in m_levels[m_levelCounter].Points)
            {
                var obj = Instantiate(m_pointPrefab, point, Quaternion.identity) as GameObject;
                obj.transform.parent = this.transform;
                instantObjects.Add(obj);
                print("position of the points" + point);
                m_points.Add(new PolygonPoint(point));

            }

            foreach (var point in m_points)
            {
                print("position of the points" + point.Pos);
            }

            //Make vertex list
            //m_points = FindObjectsOfType<PolygonPoint>().ToList();
            print(m_points.Count);
            foreach (var point in m_points)
            {
                print("position of the points" + point.Pos);
            }

            // m_solutionHull = ConvexHull.ComputeConvexHull(m_points.Select(v => v.Pos));

            m_advanceButton.Disable();
        }

        public void AdvanceLevel()
        {
            // increase level index
            m_levelCounter++;
            InitLevel();
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

                print("lal ik ben lekker null" + edge);
                

                var drawedEdge = Instantiate(m_edgeMesh, Vector3.forward , Quaternion.identity) as GameObject;
                drawedEdge.transform.parent = this.transform;
                instantObjects.Add(drawedEdge);

                //drawedEdge.GetComponent<HullSegment>().Segment = segment;
                drawedEdge.GetComponent<PolygonEdge>().Segment = edge.Segment;

                var roadmeshScript = drawedEdge.GetComponent<ReshapingMesh>();
                print("ik ben edge.point1" + edge.point1);
                print("ik ben point 1 " + edge);

                roadmeshScript.CreateNewMesh(edge.point1.transform.position, edge.point2.transform.position);
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

        /// <summary>
        /// Clears hull and relevant game objects
        /// </summary>
        private void Clear()
        {
            //m_points.Clear();
            //triangulation.Clear();
            //p_edges.Clear();
            //m_triangle = null;

            // destroy game objects created in level
            foreach (var obj in instantObjects)
            {
                // destroy immediate
                // since controller will search for existing objects afterwards
                DestroyImmediate(obj);
            }
        }

    }
}
