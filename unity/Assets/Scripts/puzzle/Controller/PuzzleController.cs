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
        private List<GameObject> m_triangleMeshPrefabList;
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
        protected int colorCounter = 0;


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
                Vector2 p2 = point;
                p2.x += p2.y * 0.000001f;
                p2.y += p2.x * 0.000001f;
                m_points.Add(p2);
            }

            Polygon2D testPolygon = new Polygon2D(m_points);
            drawEdgesOfPolygon(testPolygon.Segments);

            List<Polygon2D> monotone = Monotone.MakeMonotone(testPolygon);
            List<Polygon2D> triangles = new List<Polygon2D>();
            foreach (Polygon2D p in monotone)
            {
                triangles.AddRange(Triangulate.TriangulatePoly(p));
            }

            // TODO: make merge checking more efficient, now O(t^2) worst Kees with 't' number of triangles.
            // Sort on area such that result list contains merge candidates for small triangles
            triangles.Sort((t1, t2) => -Comparer<float>.Default.Compare(t1.Area, t2.Area));

            List<Polygon2D> result = new List<Polygon2D>();
            foreach (Polygon2D t in triangles)
            {
                if (t.Area < 0.5f)
                {
                    print("REMOVE small triangle");
                    bool merged = false;
                    foreach (Polygon2D c in result)
                    {
                        if (CanMergePieces(c, t))
                        {
                            print("REMOVED small triangle" + c.VertexCount);
                            merged = true;
                            break;
                        }
                    }
                    if (!merged)
                    {
                        result.Add(t);
                    }
                }
                else
                {
                    result.Add(t);
                }
            }

            foreach (Polygon2D t in result)
            {
                // drawEdgesOfPolygon(t.Segments);
                Debug.Log(String.Format("{0} {1} {2}", t.Vertices.ElementAt(0), t.Vertices.ElementAt(1), t.Vertices.ElementAt(2)));
                PuzzlePiece piece = CreatePiece(t);
                pieces.Add(piece);
            }

            // Set initial position random
            foreach (PuzzlePiece piece in pieces)
            {
                piece.Polygon.transform.position = new Vector3(
                    UnityEngine.Random.Range(-3f, 3f),
                    UnityEngine.Random.Range(-3f, 3f),
                    0f
                );
            }

            // disable advance button
            m_advanceButton.Disable();

        }

        public PuzzlePiece CreatePiece(Polygon2D trianglePoints)
        {
            colorCounter = (colorCounter + 1) % 10;
            var m_triangleMeshPrefab = m_triangleMeshPrefabList[colorCounter];
            var drawedTriangle = Instantiate(m_triangleMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
            drawedTriangle.transform.parent = this.transform;
            instantObjects.Add(drawedTriangle);

            var mesh = drawedTriangle.GetComponent<Polygon2DMesh>();
            mesh.Polygon = trianglePoints;

            PuzzlePiece piece = drawedTriangle.AddComponent<PuzzlePiece>(); ;
            piece.Polygon = mesh;

            return piece;
        }

        public bool CanMergePieces(Polygon2D p1, Polygon2D p2)
        {
            if (p2.Vertices.Intersect(p1.Vertices).Count() != 2)
                return false;

            Vector2 v1 = p2.Vertices.ElementAt(0);
            Vector2 v2 = p2.Vertices.ElementAt(1);
            Vector2 v3 = p2.Vertices.ElementAt(2);

            if (p1.ContainsVertex(v1))
            {
                if (p1.ContainsVertex(v2))
                {
                    // v1 and v2 are in the intersection.
                    InsertMergeVertex(p1, v1, v2, v3);
                }
                else
                {
                    // v1 and v3 are in the intersection.
                    InsertMergeVertex(p1, v1, v3, v2);
                }
            }
            else
            {
                // v2 and v3 are in the intersection.
                InsertMergeVertex(p1, v2, v3, v1);
            }

            return true;
        }

        private void InsertMergeVertex(Polygon2D p, Vector2 v1, Vector2 v2, Vector2 add)
        {
            Vector2 first = p.Vertices.First();
            Vector2 last = p.Vertices.Last();
            if ((first == v1 && last == v2) || (first == v2 && last == v1))
            {
                p.AddVertexFirst(add);
            }
            else
            {
                foreach (Vector2 v in p.Vertices)
                {
                    if (v == v1 || v == v2)
                    {
                        p.AddVertexAfter(add, v);
                        break;
                    }
                }
            }
        }

        public void AdvanceLevel()
        {
            // increase level index
            m_levelCounter++;
            InitLevel();
        }

        void Update()
        {
            if (!Input.GetMouseButton(0) && carryingPiece)
            {
                carryingPiece = false;
                print(String.Format("Valid: {0}", pieceCarried.IsValid));
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

        public void drawEdgesOfPolygon(ICollection<LineSegment> edges)
        {
            foreach (LineSegment edge in edges)
            {

                var drawedEdge = Instantiate(m_edgeMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
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
            }
            else
            {
                print("Solution invalid.");
                m_advanceButton.Disable();
            }
        }

        private bool CheckPlacement()
        {
            foreach (PuzzlePiece piece in pieces)
            {
                if (!piece.IsValid)
                {
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
