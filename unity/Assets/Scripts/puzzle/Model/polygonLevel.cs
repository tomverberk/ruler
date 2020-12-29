namespace Puzzle
{
    using System;
    using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class polygonLevel : MonoBehaviour
	{

		public List<Polygon> triangulation;
		public Polygon mainPolygon;

		public polygonLevel(Polygon mainPolygon)
        {
			this.mainPolygon = mainPolygon;
            this.triangulation = triangulatePolygon(mainPolygon);
        }

        private List<Polygon> triangulatePolygon(Polygon mainPolygon)
        {
            // TODO @Kevin
            List<Polygon> triangulation = null;


            return triangulation;
        }
    }

}
