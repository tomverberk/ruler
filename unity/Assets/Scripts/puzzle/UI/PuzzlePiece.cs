namespace Puzzle.UI
{
    using UnityEngine;
    using General.Model;

    public class PuzzlePiece : MonoBehaviour
    {
        private Polygon2DMesh polygon;

        private PuzzleController puzzleController;

        public Polygon2DMesh Polygon {
            get { return polygon; }
            set { this.polygon = value; }
        }

        public bool IsValid {
            get {
                return polygon.transform.position.magnitude < 0.01f;
            }
        }

        public PuzzlePiece()
        {
        }

        void Awake()
        {
            puzzleController = FindObjectOfType<PuzzleController>();
        }

        void OnMouseDown()
        {
            var mousePos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;

            puzzleController.carryingPiece = true;
            puzzleController.pieceCarried = this;
            puzzleController.carryOffset = mousePos - polygon.transform.position;
        }

        void OnMouseUp()
        {
        }
    }
}