namespace Puzzle.UI
{
    using UnityEngine;
    using General.Model;

    public class PuzzlePiece : MonoBehaviour
    {
        private Polygon2DMesh polygon;

        public Polygon2DMesh Polygon {
            get { return polygon; }
        }

        public PuzzlePiece(Polygon2DMesh polygon)
        {
            this.polygon = polygon;
        }
    }
}