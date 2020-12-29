namespace Puzzle
{
    using UnityEngine;
    using Util.Geometry;

    public class PolygonEdge : MonoBehaviour
    {
        public Vector2 Pos1;
        public Vector2 Pos2;
        //public Points points;

        private PuzzleController m_gameController;

        
        public PolygonEdge (Vector2 point1, Vector2 point2)
        {
            this.Pos1 = point1;
            this.Pos2 = point2;
        }

        // Additional methods
        // . . . 
    }
}
