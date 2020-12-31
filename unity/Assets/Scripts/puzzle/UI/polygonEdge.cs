namespace Puzzle
{
    using UnityEngine;
    using Util.Geometry;

    public class PolygonEdge : MonoBehaviour
    {
        public LineSegment Segment { get; set; }
        public PolygonPoint point1;
        public PolygonPoint point2;
        //public Points points;

        private PuzzleController m_gameController;

        
        public PolygonEdge (PolygonPoint p1, PolygonPoint p2)
        {
            this.point1 = p1;
            this.point2 = p2;
            this.Segment = new LineSegment(point1.Pos, point2.Pos);
        }

        // Additional methods
        // . . . 
    }
}
