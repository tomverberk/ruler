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
    using Util.Triangulate;
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
        private GameObject m_triangleMeshPrefab;
        [SerializeField]
        private ButtonContainer m_advanceButton;

        [SerializeField]
        private List<PolygonLevel> m_levels;
        [SerializeField]
        private string m_victoryScene;

        private Vector3 screenPoint;
        private Vector3 offset;


        //internal HullPoint m_firstPoint;
        //internal HullPoint m_secondPoint;
        internal Polygon m_triangle;
        internal bool m_locked;
        internal bool m_carrying_triangle;

        private List<PolygonPoint> m_points = new List<PolygonPoint>();
        private List<Polygon> triangulation;
        private List<PolygonEdge> p_edges;

        public Polygon2DWithHoles testPolygon { get; private set; }


        //private HashSet<LineSegment> m_segments;
        //private Polygon2D m_solutionHull;

        private List<GameObject> instantObjects;

        protected int m_levelCounter = 0;


        void Start()
        {

            
            print("Beginning");

            // https://www.geogebra.org/calculator/kc4s9xds
            List<PolygonPoint> points = new List<PolygonPoint>();
            points.Add(new PolygonPoint(new Vector2(0f, 0f))); // A
            points.Add(new PolygonPoint(new Vector2(1f, 0f))); // B
            points.Add(new PolygonPoint(new Vector2(2f, 2f))); // C
            points.Add(new PolygonPoint(new Vector2(11.08f, 5.47f))); // D
            points.Add(new PolygonPoint(new Vector2(13.12f, 6.51f))); // E
            points.Add(new PolygonPoint(new Vector2(14.76f, 5.79f))); // F
            points.Add(new PolygonPoint(new Vector2(17.28f, 7.19f))); // G
            points.Add(new PolygonPoint(new Vector2(16.86f, 8.76f))); // H
            points.Add(new PolygonPoint(new Vector2(15.26f, 11.09f))); // I
            points.Add(new PolygonPoint(new Vector2(13.74f, 9.15f))); // J
            //Polygon poly = new Polygon(points);
            //List<Polygon> monotone = Monotone.MakeMonotone(poly);

            //foreach (Polygon p in monotone)
            //{
                //print(String.Format("Monotone Polygon ({0}):", p.points.Count));

            //    List<Polygon> triangles = Triangulate.TriangulatePoly(p);
                //print(String.Format("Triangles: {0}", triangles.Count));
            //    foreach (Polygon t in triangles)
            //    {
                    //print(String.Format("Triangle ({0})", t.points.Count));
            //        foreach (PolygonEdge e in t.edges)
            //        {
                        //print(String.Format("TR ({0}, {1}) to ({2}, {3})", e.point1.Pos.x, e.point1.Pos.y, e.point2.Pos.x, e.point2.Pos.y));
            //        }
            //    }
            //}

            // get unity objects
            instantObjects = new List<GameObject>();
            m_points = new List<PolygonPoint>();
            

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
                m_points.Add(new PolygonPoint(point));
            }

            //List<PolygonPoint> testTriangle = FindObjectsOfType<PolygonPoint>().ToList();

            

            // create a polygon from the points
            //var setPoints1 = new ArraySegment<PolygonPoint>(m_points, 0, 2);
            //var setPoints2 = new ArraySegment<PolygonPoint>(m_points, 1, 3);

            //Polygon polygon1 = createPolygonFromPoints(setPoints1);
            //polygon polygon2 = createPolygonFromPoints(setPoints2);

            //createsmallTriangle(polygon1);
            //createsmallTriangle(polygon2);

            Polygon polygon = createPolygonFromPoints(m_points);

            createsmallTriangle(polygon);


            p_edges = polygon.edges;

            drawEdgesOfPolygon(p_edges);

            // add here the triangles for the polygon
            //createsmallTriangles()


            // disable advance button
            m_advanceButton.Disable();
            m_advanceButton.Enable();

        }

        public void createsmallTriangle(Polygon trianglePoints)
        {
            var drawedTriangle = Instantiate(m_triangleMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
            drawedTriangle.transform.parent = this.transform;
            instantObjects.Add(drawedTriangle);

            //drawedTriangle.GetComponent<Polygon>().actualPoints = trianglePoints.actualPoints;

            

            trianglePoints.drawedTriangle.GetComponent<Polygon>().centerPoint = trianglePoints.centerPoint;
            print("I don't give an error here");

            var triangleScript = drawedTriangle.GetComponent<Polygon2DMesh>();
            triangleScript.Polygon = trianglePoints.polygon;
            trianglePoints.drawedTriangle = triangleScript;
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
                //m_triangle = null;
                m_carrying_triangle = false;
                    

            }
            else if (Input.GetMouseButton(0))
            {
                // TODO something idk
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + 10 * Vector3.forward);
                m_triangle.SetCenterPoint(pos);
                print(m_triangle.getCenterPoint());

                m_triangle.drawedTriangle.GetComponent<Polygon>().centerPoint = m_triangle.centerPoint;
                //forEach(var obj in instantObjects){
                //    if(obj is Polygon)
                //    {
                //        if(obj == m_triangle)
                //        {
                //            var centerPoint = m_triangle.getCenterPoint();
                //        }
                //    }
                //}

                //var centerPoint = m_triangle.getCenterPoint();
                //m_triangle.drawedTriangle = UpdateMesh();
                //print(centerPoint);
                print("I arrive here");

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
                drawedEdge.GetComponent<PolygonEdge>().point1 = edge.point1;
                drawedEdge.GetComponent<PolygonEdge>().point2 = edge.point2;

                var roadmeshScript = drawedEdge.GetComponent<ReshapingMesh>();

                Vector2 p1 = edge.point1.Pos;
                Vector2 p2 = edge.point2.Pos;

                roadmeshScript.CreateNewMesh(p1, p2);
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
