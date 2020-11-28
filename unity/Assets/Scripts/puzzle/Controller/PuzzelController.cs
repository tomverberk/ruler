namespace Puzzle
{
    using General.Menu;
    using General.Model;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.Geometry.Polygon;
    using Util.Algorithms.Polygon;
    using Util.Geometry;

    public class PuzzleController : MonoBehaviour
    {

        //public LineRenderer m_line;

        //[SerializeField]
        //private GameObject m_roadMeshPrefab;
        [SerializeField]
        private ButtonContainer m_advanceButton;

        //internal HullPoint m_firstPoint;
        //internal HullPoint m_secondPoint;
        internal bool m_locked;

        //private List<HullPoint> m_points;
        //private HashSet<LineSegment> m_segments;
        //private Polygon2D m_solutionHull;


        void Start()
        {
            //  TODO get unity objects
            

            // compute convex hull
        

            // disable advance button
            m_advanceButton.Disable();
        }

        void Update()
        {
            //TODO CREATE MOUSE INTERACTION
            if (m_locked && !Input.GetMouseButton(0))
            {
                // TODO Place puzzelpeace
            }
            else if (Input.GetMouseButton(0))
            {
                
            }

            
            if ((m_locked && !Input.GetMouseButton(0)) || Input.GetMouseButtonUp(0))
            {
                
            }
        }

        /*
         * Method when placing a triangle
         */
        public void PlaceTriangle(Triangle t)
        {
            //TODO EVERYTHING
            
        }

        public void RemoveSegment(Triangle t)
        {
            //TODO REWRITE
            //m_segments.Remove(a_segment.Segment);
            CheckSolution();
        }

        //TODO
        // link advanceButton idk how
        public void CheckSolution()
        {
            if (CheckPlacement())
            {
                //m_advanceButton.Enable();
            }
            else
            {
                //m_advanceButton.Disable();
            }
        }

        private bool CheckPlacement()
        {
            // TODO quick check


            // TODO slow check
            // also check reverse
            return true;
        }
    }
}
