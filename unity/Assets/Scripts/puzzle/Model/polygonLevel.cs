namespace Puzzle
{
    using System;
    using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

    /// <summary>
    /// Data container for puzzle level, containing point set and triangles.
    /// </summary>

    [CreateAssetMenu(fileName = "puzzleLevelNew", menuName = "Levels/Puzzle Level")]
    public class PolygonLevel : ScriptableObject
	{
        [Header ("Polygon Points")]

		public List<Polygon> triangulation;
        public List<Vector2> Points = new List<Vector2>();
		public Polygon mainPolygon;

		public PolygonLevel(Polygon mainPolygon)
        {
			this.mainPolygon = mainPolygon;
            this.triangulation = triangulatePolygon(mainPolygon);
        }

        private List<Polygon> triangulatePolygon(Polygon mainPolygon)
        {
            List<Polygon> triangulation = null;
            List<Polygon> subTriangulation;
            List<Polygon> subPolygons = MakeMonotone(mainPolygon);
            foreach (Polygon pol in subPolygons)
            {
                subTriangulation = Triangulate(pol);
                foreach (Polygon triangle in subTriangulation)
                {
                    triangulation.Add(triangle);
                }
            }

            return triangulation;
        }

        private List<Polygon> MakeMonotone(Polygon mainPolygon)
        {
            List<Polygon> monotonePolygons = null;
            if (isMonotone(mainPolygon))
            {
                monotonePolygons.Add(mainPolygon);
            } else
            {
                // Do something
                monotonePolygons.Add(mainPolygon);
            }
            return monotonePolygons;
        }

        private bool isMonotone(Polygon mainPolygon)
        {
            return true;
        }

        private List<Polygon> Triangulate(Polygon pol)
        {
            List<Polygon> triangles = null;
            List<PolygonPoint> leftList = null;
            List<PolygonPoint> rightList = null;

            List<PolygonEdge> edges = pol.getEdges();
            PolygonPoint pointLeft = null;
            PolygonPoint pointRight = null;
            PolygonEdge dummy1 = null;
            PolygonEdge dummy2 = null;

            // Get the first edges left and right from the top of the polygon
            getLeftRightPoints(pol.top, pol.top, edges, ref dummy1, ref dummy2);

            // get the points on the left and right hand side
            getLeftRightSidePoints(pol, ref pointLeft, ref pointRight, dummy1, dummy2);

            leftList.Add(pointLeft);
            rightList.Add(pointRight);

            // Fills the list with the points on the right side, and on the left side

            Boolean leftBottom = false;
            Boolean rightBottom = false;
            while (!leftBottom || !rightBottom)
            {
                // dummy 1 is left edge, dummy2 is right edge
                getLeftRightPoints(pointLeft, pointRight, edges, ref dummy1, ref dummy2);

                if (dummy1.point1 == pointLeft && !leftBottom)
                {
                    pointLeft = dummy1.point2;
                    leftList.Add(pointLeft);
                } 
                else if (dummy1.point2 == pointLeft && !leftBottom)
                {
                    pointLeft = dummy1.point1;
                    leftList.Add(pointLeft);
                }
                if (dummy2.point1 == pointRight && !rightBottom)
                {
                    pointRight = dummy2.point2;
                    rightList.Add(pointRight);
                }
                else if (dummy2.point2 == pointRight && !rightBottom)
                {
                    pointRight = dummy2.point1;
                    rightList.Add(pointRight);
                }
            }

            // Algorithm
            int leftSize = leftList.Count;
            int rightSize = rightList.Count;
            Stack S = new Stack();
            S.Push(pol.top);
            Boolean side = false; // False = left, True is right
            int i = 0; // left index
            int j = 0; // right index
            if (leftList[0].Pos.y >= rightList[0].Pos.y)
            {
                S.Push(leftList[0]);
                i++;
                side = false;
            }
            else if (leftList[0].Pos.y < rightList[0].Pos.y)
            {
                S.Push(rightList[0]);
                j++;
                side = true;
            }

            while(i < leftSize || j < leftSize)
            {
                if (leftList[i].Pos.y >= rightList[j].Pos.y)
                {
                    // Not same side
                    if (side)
                    {
                        PolygonPoint pointIndex1;
                        PolygonPoint pointIndex2;
                        PolygonPoint penultamatePoint = (PolygonPoint)S.Pop();
                        pointIndex2 = penultamatePoint;
                        while (S.Count > 0)
                        {
                            List<PolygonPoint> trianglePoints = null;
                            trianglePoints.Add(leftList[i]);
                            trianglePoints.Add(pointIndex2);
                            pointIndex1 = (PolygonPoint)S.Pop();
                            trianglePoints.Add(pointIndex1);
                            triangles.Add(new Polygon(trianglePoints));
                            pointIndex2 = pointIndex1;
                        }
                        S.Push(penultamatePoint);
                        S.Push(leftList[i]);
                    }
                    // Same boundaries
                    else
                    {
                        // TODO
                    }
                    i++;
                } 
                else if (leftList[i].Pos.y < rightList[j].Pos.y)
                {
                    // Not same side
                    if (!side)
                    {
                        while (S.Count > 0)
                        {
                            PolygonPoint pointIndex1;
                            PolygonPoint pointIndex2;
                            PolygonPoint penultamatePoint = (PolygonPoint)S.Pop();
                            pointIndex2 = penultamatePoint;
                            while (S.Count > 0)
                            {
                                List<PolygonPoint> trianglePoints = null;
                                trianglePoints.Add(rightList[j]);
                                trianglePoints.Add(pointIndex2);
                                pointIndex1 = (PolygonPoint)S.Pop();
                                trianglePoints.Add(pointIndex1);
                                triangles.Add(new Polygon(trianglePoints));
                                pointIndex2 = pointIndex1;
                            }
                            S.Push(penultamatePoint);
                            S.Push(rightList[j]);
                        }
                    }
                    // Same boundaries
                    else
                    {
                        // TODO same boundaries
                    }
                    j++;
                }
            }


            return triangles;
        }

        private void getLeftRightSidePoints(Polygon pol, ref PolygonPoint pointLeft, ref PolygonPoint pointRight, PolygonEdge dummy1, PolygonEdge dummy2)
        {
            if (dummy1.point1 == pol.top && dummy2.point2 == pol.top)
            {
                if (dummy1.point1.Pos.x <= dummy2.point2.Pos.x)
                {
                    pointLeft = dummy1.point1;
                    pointRight = dummy2.point2;
                }
                else
                {
                    pointLeft = dummy2.point2;
                    pointRight = dummy1.point1;
                }
            }
            else if (dummy1.point1 == pol.top && dummy2.point1 == pol.top)
            {
                if (dummy1.point1.Pos.x <= dummy2.point1.Pos.x)
                {
                    pointLeft = dummy1.point1;
                    pointRight = dummy2.point1;
                }
                else
                {
                    pointLeft = dummy2.point1;
                    pointRight = dummy1.point1;
                }
            }
            else if (dummy1.point2 == pol.top && dummy2.point1 == pol.top)
            {
                if (dummy1.point2.Pos.x <= dummy2.point1.Pos.x)
                {
                    pointLeft = dummy1.point2;
                    pointRight = dummy2.point1;
                }
                else
                {
                    pointLeft = dummy2.point1;
                    pointRight = dummy1.point2;
                }
            }
            else if (dummy1.point2 == pol.top && dummy2.point2 == pol.top)
            {
                if (dummy1.point2.Pos.x <= dummy2.point2.Pos.x)
                {
                    pointLeft = dummy2.point2;
                    pointRight = dummy1.point2;
                }
                else
                {
                    pointLeft = dummy2.point2;
                    pointRight = dummy1.point2;
                }
            }
        }

        private void getLeftRightPoints(PolygonPoint leftTop, PolygonPoint rightTop, List<PolygonEdge> edges, ref PolygonEdge dummy1, ref PolygonEdge dummy2)
        {
            int i = 0;
            foreach (PolygonEdge edge in edges)
            {
                if (leftTop == rightTop)
                {
                    if ((edge.point1 == leftTop || edge.point2 == leftTop) && i == 0)
                    {
                        dummy1 = edge;
                        i++;
                    }
                    else if ((edge.point1 == leftTop || edge.point2 == leftTop) && i == 1)
                    {
                        dummy2 = edge;
                        break;
                    }
                }
                else
                {
                    if (edge.point1 == leftTop || edge.point2 == leftTop)
                    {
                        dummy1 = edge;
                    }
                    else if (edge.point1 == rightTop || edge.point2 == rightTop)
                    {
                        dummy2 = edge;
                    }
                }
            }
        }
    }

}
