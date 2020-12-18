namespace Puzzle
{
    using UnityEngine;
    using Util.Geometry;

    public class PolygonEdge : MonoBehaviour
    {
        public Vertex2 point1;
        public Vertex2 point2;
        //public Points points;

        private PuzzleController m_gameController;

        
        public PolygonEdge (Vertex2 point1, Vertex2 point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        // Additional methods
        // . . . 
    }
}
