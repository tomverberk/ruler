﻿namespace Util.Geometry.Triangulation
{
    using System;
    using UnityEngine;
    using Util.Geometry.Graph;
    using Util.Math;

    public class Triangle {

        public Vertex P0 { get; private set; }
        public Vertex P1 { get; private set; }
        public Vertex P2 { get; private set; }

        public TriangleEdge E0 { get; private set; }
        public TriangleEdge E1 { get; private set; }
        public TriangleEdge E2 { get; private set; }

        public Vector2 Circumcenter { get; private set; }

        public Triangle() : this(new Vertex(), new Vertex(), new Vertex())
        { }

        public Triangle(Vertex v0, Vertex v1, Vertex v2) : this(
                new TriangleEdge(v0, v1, null, null),
                new TriangleEdge(v1, v2, null, null),
                new TriangleEdge(v2, v0, null, null)
            )
        {
            E0.T = this;
            E1.T = this;
            E2.T = this;
        }

        public Triangle(TriangleEdge e0, TriangleEdge e1, TriangleEdge e2)
        {
            if(e0.End != e1.Start || e1.End != e2.Start || e2.End != e0.Start)
            {
                throw new ArgumentException("Invalid triangle edges given.");
            }
            E0 = e0;
            E1 = e1;
            E2 = e2;
            P0 = e0.Start;
            P1 = e1.Start;
            P2 = e2.Start;
            Circumcenter = MathUtil.CalculateCircumcenter(P0.Pos, P1.Pos, P2.Pos);
        }

        public bool ContainsEndpoint(Vertex x)
        {
            return x.Equals(P0) || x.Equals(P1) || x.Equals(P2);
        }

        public bool Inside(Vector2 X)
        {
            int firstSide = Math.Sign(MathUtil.Orient2D(P0.Pos, P1.Pos, X));
            int secondSide = Math.Sign(MathUtil.Orient2D(P1.Pos, P2.Pos, X));
            int thirdSide = Math.Sign(MathUtil.Orient2D(P2.Pos, P0.Pos, X));
            return (firstSide != 0 && firstSide == secondSide && secondSide == thirdSide);
        }

        public bool InsideCircumcircle(Vector2 X)
        {
            return MathUtil.InsideCircle(P0.Pos, P1.Pos, P2.Pos, X);
        }

        public Vertex OtherVertex(TriangleEdge a_Edge)
        {
            return OtherVertex(a_Edge.Start, a_Edge.End);
        }
            
        public Vertex OtherVertex(Vertex a, Vertex b)
        {
            if (P0 != a && P0 != b) return P0;
            if (P1 != a && P1 != b) return P1;
            if (P2 != a && P2 != b) return P2;
            return null;
        }

        public TriangleEdge OtherEdge(TriangleEdge a, Vertex b)
        {
            if (E0 != a && E0.ContainsVertex(b)) return E0;
            if (E1 != a && E1.ContainsVertex(b)) return E1;
            if (E2 != a && E2.ContainsVertex(b)) return E2;
            return null;
        }
    }
}

