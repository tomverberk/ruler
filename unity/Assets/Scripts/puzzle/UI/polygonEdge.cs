namespace Puzzle
{
    using UnityEngine;
    using Util.Geometry;

    public class PolygonEdge : MonoBehaviour
    {
        public PolygonPoint point1;
        public PolygonPoint point2;
        //public Points points;

        private PuzzleController m_gameController;

        
        public PolygonEdge (PolygonPoint point1, PolygonPoint point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        // Additional methods
        // . . . 
    }
}
