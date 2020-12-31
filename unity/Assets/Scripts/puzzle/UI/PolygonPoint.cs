namespace Puzzle
{
    using UnityEngine;
    using Util.Geometry;

    public class PolygonPoint : MonoBehaviour
    {
        public Vector2 Pos { get; private set; }

        public PolygonPoint(Vector2 pos)
        {
            this.Pos = pos;
        }
        //TODO Figure out what should be written here
        void awake()
        {
            Pos = new Vector2(transform.position.x, transform.position.y);
        }
    }
}
