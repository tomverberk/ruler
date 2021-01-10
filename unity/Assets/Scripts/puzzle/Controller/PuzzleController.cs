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
            m_advanceButton.Disable();

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

        // link advanceButton idk how
        public void CheckSolution()
        {
            if (CheckPlacement())
            {
                print("Solution valid.");
                m_advanceButton.Enable();
            } else {
                print("Solution invalid.");
                m_advanceButton.Disable();
            }
        }

        private bool CheckPlacement()
        {
            foreach (PuzzlePiece piece in pieces) {
                if (!piece.IsValid) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Clears level objects
        /// </summary>
        private void Clear()
        {
            // destroy game objects created in level
            foreach (var obj in instantObjects)
            {
                // destroy immediate
                // since controller will search for existing objects afterwards
                DestroyImmediate(obj);
            }

            instantObjects.Clear();
            m_points.Clear();
            pieces.Clear();
        }
    }
}
