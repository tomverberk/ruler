namespace puzzle
{
    using System.Collections.Generic;
    using UnityEngine;
    using Util.Geometry;
    using puzzle.UI.Polygon;

    public class Triangulation : MonoBehaviour
    {
        public List<Polygon> m_polygons;

        public Triangulation(Polygon, IEnumerable<Polygon> polygons)
        {
            foreach(var pol in polygons)
            {
                m_polygons.AddLast(pol);
            }
        }

        public ICollection<Polygon> polygons {  get { return m_polygons; } }

        public int polygonCount { get { return m_polygons.Count; } }
    }
}