namespace Puzzle
{
    using System.Collections.Generic;
    using UnityEngine;
    using Util.Geometry;

    public class Triangulation : MonoBehaviour
    {
        public Polygon triangulatedPolygon;
        public List<Polygon> m_polygons;

        public Triangulation(Polygon polygon, IEnumerable<Polygon> polygons)
        {
            triangulatedPolygon = polygon;
            foreach(var pol in polygons)
            {
                m_polygons.Add(pol);
            }
        }

        public ICollection<Polygon> polygons {  get { return m_polygons; } }

        public int polygonCount { get { return m_polygons.Count; } }
    }
}