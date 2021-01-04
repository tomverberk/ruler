namespace Util.Triangulate
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using Util.DataStructures.Queue;
  using Puzzle;

  public static class Triangulate
  {
    public static List<Polygon> TriangulatePoly(Polygon pol)
    {
      List<Polygon> triangles = new List<Polygon>();
      List<PolygonPoint> leftList = new List<PolygonPoint>();
      List<PolygonPoint> rightList = new List<PolygonPoint>();

      PolygonPoint bottom = pol.points[0];
      PolygonPoint prev = pol.points[pol.points.Count - 1];
      foreach (PolygonPoint curr in pol.points)
      {
        if (prev.Pos.y > curr.Pos.y)
        {
          // We are on the left side
          leftList.Add(curr);
        }
        else
        {
          // We are on the right side
          rightList.Add(curr);
        }

        prev = curr;
        if (curr.Pos.y < bottom.Pos.y)
        {
          bottom = curr;
        }
      }
      rightList.Add(bottom);

      leftList.Sort(YComparer.Instance);
      rightList.Sort(YComparer.Instance);

      // BEGIN ALGORITHM
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

      int iter = 0;
      while (i < leftSize && j < rightSize)
      {
        iter++;
        if (iter > pol.points.Count * 2)
        {
          Debug.Log(String.Format("i={0}, j={1}", i, j));
          throw new Exception("Should not be reached");
        }
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
              List<PolygonPoint> trianglePoints = new List<PolygonPoint>();
              trianglePoints.Add(leftList[i]);
              trianglePoints.Add(pointIndex2);
              pointIndex1 = (PolygonPoint)S.Pop();
              trianglePoints.Add(pointIndex1);
              triangles.Add(new Polygon(trianglePoints));
              pointIndex2 = pointIndex1;
            }
            S.Push(penultamatePoint);
            S.Push(leftList[i]);
            side = false;
          }
          // Same boundaries
          else
          {
            PolygonPoint penultamatePoint = (PolygonPoint)S.Pop();
            PolygonPoint pointIndex1;
            PolygonPoint pointIndex2;
            pointIndex2 = penultamatePoint;
            while (S.Count > 0 && PointCanSeePoint(leftList[i], pointIndex2, (PolygonPoint)S.Peek(), false))
            {
              pointIndex1 = (PolygonPoint)S.Pop();
              List<PolygonPoint> trianglePoints = new List<PolygonPoint>();
              trianglePoints.Add(leftList[i]);
              trianglePoints.Add(pointIndex2);
              trianglePoints.Add(pointIndex1);
              triangles.Add(new Polygon(trianglePoints));
              pointIndex2 = pointIndex1;
            }
            S.Push(pointIndex2);
            S.Push(leftList[i]);
          }
          i++;
        }
        else if (leftList[i].Pos.y < rightList[j].Pos.y) // Point lies on right side
        {
          // Not same side
          if (!side)
          {
            PolygonPoint pointIndex1;
            PolygonPoint pointIndex2;
            PolygonPoint penultamatePoint = (PolygonPoint)S.Pop();
            pointIndex2 = penultamatePoint;
            while (S.Count > 0)
            {
              List<PolygonPoint> trianglePoints = new List<PolygonPoint>();
              trianglePoints.Add(rightList[j]);
              trianglePoints.Add(pointIndex2);
              pointIndex1 = (PolygonPoint)S.Pop();
              trianglePoints.Add(pointIndex1);
              triangles.Add(new Polygon(trianglePoints));
              pointIndex2 = pointIndex1;
            }
            S.Push(penultamatePoint);
            S.Push(rightList[j]);
            side = true;
          }
          // Same boundaries
          else
          {
            PolygonPoint penultamatePoint = (PolygonPoint)S.Pop();
            PolygonPoint pointIndex1;
            PolygonPoint pointIndex2;
            pointIndex2 = penultamatePoint;
            while (S.Count > 0 && PointCanSeePoint(rightList[j], pointIndex2, (PolygonPoint)S.Peek(), true))
            {
              pointIndex1 = (PolygonPoint)S.Pop();
              List<PolygonPoint> trianglePoints = new List<PolygonPoint>();
              trianglePoints.Add(rightList[j]);
              trianglePoints.Add(pointIndex2);
              trianglePoints.Add(pointIndex1);
              triangles.Add(new Polygon(trianglePoints));
              pointIndex2 = pointIndex1;
            }
            S.Push(pointIndex2);
            S.Push(rightList[j]);
          }
          j++;
        }
      }
      // END ALGORITHM


      return triangles;
    }

    // A is highest point, B lowest point, query point should lie on the inside of the line.
    private static bool PointCanSeePoint(PolygonPoint linePointA, PolygonPoint queryPoint, PolygonPoint linePointB, Boolean side)
    {
      float position;
      position = Math.Sign((linePointB.Pos.x - linePointA.Pos.x) * (queryPoint.Pos.y - linePointA.Pos.y) - (linePointB.Pos.y - linePointA.Pos.y) * (queryPoint.Pos.x - linePointA.Pos.x));
      // Right
      if (side)
      {
        if (position == -1)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      // Left
      else if (!side)
      {
        if (position == 1)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      return false;
    }

  }
}