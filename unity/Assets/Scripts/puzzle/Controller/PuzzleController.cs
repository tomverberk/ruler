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
    using Puzzle.UI;


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

        internal bool m_locked;
        internal bool m_carrying_triangle;

        private List<Vector2> m_points = new List<Vector2>();
        private List<PuzzlePiece> pieces = new List<PuzzlePiece>();

        //private HashSet<LineSegment> m_segments;
        //private Polygon2D m_solutionHull;

        private List<GameObject> instantObjects;

        protected int m_levelCounter = 0;


        void Start()
        {

            
            print("Beginning");

            // https://www.geogebra.org/calculator/kc4s9xds
            // List<Vector2> points = new List<Vector2>();
            // points.Add(new Vector2(0f, 0f)); // A
            // points.Add(new Vector2(1f, 0f)); // B
            // points.Add(new Vector2(2f, 2f)); // C
            // points.Add(new Vector2(11.08f, 5.47f)); // D
            // points.Add(new Vector2(13.12f, 6.51f)); // E
            // points.Add(new Vector2(14.76f, 5.79f)); // F
            // points.Add(new Vector2(17.28f, 7.19f)); // G
            // points.Add(new Vector2(16.86f, 8.76f)); // H
            // points.Add(new Vector2(15.26f, 11.09f)); // I
            // points.Add(new Vector2(13.74f, 9.15f)); // J
            // Polygon2D poly = new Polygon2D(points);
            // List<Polygon2D> monotone = Monotone.MakeMonotone(poly);

            // foreach (Polygon2D p in monotone)
            // {
            //     print(String.Format("Monotone Polygon ({0}):", p.Vertices.Count));

            //     List<Polygon2D> triangles = Triangulate.TriangulatePoly(p);
            //     print(String.Format("Triangles: {0}", triangles.Count));
            //     foreach (Polygon2D t in triangles)
            //     {
            //         print(String.Format("Triangle ({0})", t.Vertices.Count));
            //         foreach (LineSegment e in t.Segments)
            //         {
            //             print(String.Format("TR ({0}, {1}) to ({2}, {3})", e.Point1.x, e.Point1.y, e.Point2.x, e.Point2.y));
            //         }
            //     }
            // }

            // get unity objects
            instantObjects = new List<GameObject>();
            m_points = new List<Vector2>();
            

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
                m_points.Add(point);
            }

            //List<PolygonPoint> testTriangle = FindObjectsOfType<PolygonPoint>().ToList();
            Polygon2D testPolygon = new Polygon2D(m_points);

            //createsmallTriangle(testPolygon);

            List<Polygon2D> monotone = Monotone.MakeMonotone(testPolygon);
            foreach (Polygon2D p in monotone)
            {
                List<Polygon2D> triangles = Triangulate.TriangulatePoly(p);
                foreach (Polygon2D t in triangles)
                {
                    // drawEdgesOfPolygon(t.Segments);
                    Polygon2DMesh mesh = CreatePolygonMesh(t);
                    PuzzlePiece piece = new PuzzlePiece(mesh);
                    pieces.Add(piece);
                }
            }
           
            // disable advance button
            m_advanceButton.Disable();
            m_advanceButton.Enable();

        }

        public Polygon2DMesh CreatePolygonMesh(Polygon2D trianglePoints)
        {
            var drawedTriangle = Instantiate(m_triangleMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
            drawedTriangle.transform.parent = this.transform;
            instantObjects.Add(drawedTriangle);

            var triangleScript = drawedTriangle.GetComponent<Polygon2DMesh>();
            triangleScript.Polygon = trianglePoints;
            return triangleScript;
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
                // m_triangle = null;
                m_carrying_triangle = false;

            }
            else if (Input.GetMouseButton(0))
            {
                // TODO something idk

            }

            // Polygon2DMesh mesh = createsmallTriangle(m_triangle);
            // m_triangle.transform.position = m_triangle.transform.position + new Vector3(0.1f, 0.1f, 0f);


            if ((m_locked && !Input.GetMouseButton(0)) || Input.GetMouseButtonUp(0))
            {
                //TODO something idk
            }
        }

        public void drawEdgesOfPolygon(ICollection<LineSegment> edges){
            foreach (LineSegment edge in edges){

                var drawedEdge = Instantiate(m_edgeMeshPrefab, Vector3.forward , Quaternion.identity) as GameObject;
                drawedEdge.transform.parent = this.transform;
                instantObjects.Add(drawedEdge);

                var roadmeshScript = drawedEdge.GetComponent<ReshapingMesh>();

                roadmeshScript.CreateNewMesh(edge.Point1, edge.Point2);
            }
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
            instantObjects.Clear();
            m_points.Clear();
            pieces.Clear();

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
