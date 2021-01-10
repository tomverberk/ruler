﻿namespace Puzzle
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

        internal bool carryingPiece;
        internal PuzzlePiece pieceCarried;
        internal Vector3 carryOffset = Vector3.zero;
        private List<Vector2> m_points = new List<Vector2>();
        private List<PuzzlePiece> pieces = new List<PuzzlePiece>();

        private List<GameObject> instantObjects;

        protected int m_levelCounter = 0;


        void Start()
        {


            print("Beginning");

            // get unity objects
            instantObjects = new List<GameObject>();
            m_points = new List<Vector2>();
            
            InitLevel();
        }


        public void InitLevel()
        {
            print("creating level" + m_levelCounter);
            if (m_levelCounter >= m_levels.Count)
            {
                SceneManager.LoadScene(m_victoryScene);
                return;
            }

            // clear old level
            Clear();

            // initialize points
            foreach (var point in m_levels[m_levelCounter].Points)
            {
                var obj = Instantiate(m_pointPrefab, point, Quaternion.identity) as GameObject;
                obj.transform.parent = this.transform;
                instantObjects.Add(obj);
                m_points.Add(point);
            }

            //List<PolygonPoint> testTriangle = FindObjectsOfType<PolygonPoint>().ToList();
            Polygon2D testPolygon = new Polygon2D(m_points);
            drawEdgesOfPolygon(testPolygon.Segments);

            //createsmallTriangle(testPolygon);

            List<Polygon2D> monotone = Monotone.MakeMonotone(testPolygon);
            foreach (Polygon2D p in monotone)
            {
                List<Polygon2D> triangles = Triangulate.TriangulatePoly(p);
                foreach (Polygon2D t in triangles)
                {
                    PuzzlePiece piece = CreatePiece(t);
                    // piece.Polygon.transform.localScale = new Vector3(0.1f, 0.1f);
                    // instantObjects.Add(piece);
                    pieces.Add(piece);
                }
            }

            // Set initial position random
            System.Random random = new System.Random();
            foreach (PuzzlePiece piece in pieces) {
                piece.Polygon.transform.position = new Vector3(
                    (float) random.NextDouble(),
                    (float) random.NextDouble(),
                    0f
                );
            }
           
            // disable advance button
            m_advanceButton.Enable();

        }

        public PuzzlePiece CreatePiece(Polygon2D trianglePoints)
        {
            var drawedTriangle = Instantiate(m_triangleMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
            drawedTriangle.transform.parent = this.transform;
            instantObjects.Add(drawedTriangle);

            var mesh = drawedTriangle.GetComponent<Polygon2DMesh>();
            mesh.Polygon = trianglePoints;

            PuzzlePiece piece = drawedTriangle.AddComponent<PuzzlePiece>();;
            piece.Polygon = mesh;
            
            return piece;
        }

        public void AdvanceLevel()
        {
            // increase level index
            m_levelCounter++;
            InitLevel();
        }



        void Update()
        {
            if (!Input.GetMouseButton(0))
            {
                carryingPiece = false;
                pieceCarried = null;

            }

            if ((carryingPiece && Input.GetMouseButton(0)))
            {
                var mousePos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
                var newPos = mousePos - carryOffset;
                pieceCarried.Polygon.transform.position = newPos;

                print("Valid: " + pieceCarried.IsValid);
            }

            if (Input.GetMouseButtonUp(0))
            {
                print("Check the solution");
                CheckSolution();
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
                // Set a new Top? for each triangle
                
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
                m_advanceButton.Enable();
            }
        }

        private bool CheckPlacement()
        {
            bool solution = true;

            if (pieceCarried != null)
            {
                // Position with error margin
                if (!inCircleRadius(pieceCarried.Polygon.transform.position.x, pieceCarried.Polygon.transform.position.y, transform.position.x, transform.position.y))
                {
                    return false;
                }
            }

            // for (int i = 0; i < correctPlaceList.Count; i++)
            // {
            //     if (correctPlaceList[i] == false)
            //     {
            //         solution = false;
            //     }
            // }

            if (solution == true)
            {
                return true;
            }
            return false;
        }

        // Checks if point is in circle radius
        private bool inCircleRadius(float center_x, float center_y, float x, float y)
        {
            return ((x - center_x) * (x - center_x)) + ((y - center_y) * (y - center_y)) <= (0.1 * 0.1);
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

        internal void UpdatePolygon(Polygon polygon, Vector3 current)
        {
            //print("top is" + polygon.top.Pos);
            var differenceX = current.x - polygon.Pos.x;
            var differenceY = current.y - polygon.Pos.y;
            List<PolygonPoint> points = polygon.points;
            List<PolygonPoint> newPoints = new List<PolygonPoint>();
            foreach (var point in points)
            {
                var x = point.Pos.x;
                var y = point.Pos.y;
                PolygonPoint newPoint = new PolygonPoint(new Vector2(x - differenceX, y - differenceY));
                newPoints.Add(newPoint);
            }
            polygon.points = newPoints;
            polygon.CalculateTopBottom(newPoints);
            polygon.edges = new List<PolygonEdge>();
            polygon.initializeEdges(newPoints);
        }

    }
}
