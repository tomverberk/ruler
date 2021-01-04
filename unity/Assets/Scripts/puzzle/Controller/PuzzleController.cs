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
    using Util.Monotone;
    using General.Controller;
    using UnityEngine.SceneManagement;


    public class PuzzleController : MonoBehaviour
    {

        public LineRenderer m_line;

        [SerializeField]
        public GameObject m_edgeMeshPrefab;
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

            // https://www.geogebra.org/calculator/kc4s9xds
            List<PolygonPoint> points = new List<PolygonPoint>();
            points.Add(new PolygonPoint(new Vector2(12.12f, 10.47f))); // A
            points.Add(new PolygonPoint(new Vector2(9f, 9.71f))); // B
            points.Add(new PolygonPoint(new Vector2(8.86f, 7.07f))); // C
            points.Add(new PolygonPoint(new Vector2(11.08f, 5.47f))); // D
            points.Add(new PolygonPoint(new Vector2(13.12f, 6.51f))); // E
            points.Add(new PolygonPoint(new Vector2(14.76f, 5.79f))); // F
            points.Add(new PolygonPoint(new Vector2(17.28f, 7.19f))); // G
            points.Add(new PolygonPoint(new Vector2(16.86f, 8.76f))); // H
            points.Add(new PolygonPoint(new Vector2(15.26f, 11.09f))); // I
            points.Add(new PolygonPoint(new Vector2(13.74f, 9.15f))); // J
            Polygon poly = new Polygon(points);
            List<Polygon> monotone = Monotone.MakeMonotone(poly);

            foreach (Polygon p in monotone)
            {
                print(String.Format("Monotone Polygon ({0}):", p.points.Count));

                List<Polygon> triangles = Triangulate.TriangulatePoly(p);
                foreach (Polygon t in triangles)
                {
                    print(String.Format("Triangle ({0})", t.points.Count));
                    foreach (PolygonEdge e in t.edges)
                    {
                        print(String.Format("TR ({0}, {1}) to ({2}, {3})", e.point1.Pos.x, e.point1.Pos.y, e.point2.Pos.x, e.point2.Pos.y));
                    }
                }
            }

            // get unity objects
            m_points = new List<PolygonPoint>();
            instantObjects = new List<GameObject>();

            InitLevel();
            


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

            // initialize settlements
            foreach (var point in m_levels[m_levelCounter].Points)
            {
                var obj = Instantiate(m_pointPrefab, point, Quaternion.identity) as GameObject;
                obj.transform.parent = this.transform;
                instantObjects.Add(obj);
            }

            m_points = FindObjectsOfType<PolygonPoint>().ToList();

            // create a polygon from the points
            Polygon polygon = createPolygonFromPoints(m_points);

            p_edges = polygon.edges;

            drawEdgesOfPolygon(p_edges);


            // disable advance button
            m_advanceButton.Disable();
            m_advanceButton.Enable();

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

                var drawedEdge = Instantiate(m_edgeMeshPrefab, Vector3.forward , Quaternion.identity) as GameObject;
                drawedEdge.transform.parent = this.transform;
                instantObjects.Add(drawedEdge);

                //drawedEdge.GetComponent<HullSegment>().Segment = segment;
                drawedEdge.GetComponent<PolygonEdge>().Segment = edge.Segment;

                var roadmeshScript = drawedEdge.GetComponent<ReshapingMesh>();
           

                roadmeshScript.CreateNewMesh(edge.point1.transform.position, edge.point2.transform.position);
            }
        }

        public Polygon createPolygonFromPoints(List<PolygonPoint> points)
        {
            foreach (var point in points)
            {
                print("in createPolygonPoints" + point.Pos);
            }
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
