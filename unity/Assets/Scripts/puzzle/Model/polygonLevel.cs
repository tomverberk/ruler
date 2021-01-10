using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util.Monotone;

namespace Puzzle
{
    /// <summary>
    /// Data container for puzzle level, containing point set and triangles.
    /// </summary>

    [CreateAssetMenu(fileName = "puzzleLevelNew", menuName = "Levels/Puzzle Level")]
    public class PolygonLevel : ScriptableObject
    {
        [Header("Polygon Points")]

        public List<Vector2> Points = new List<Vector2>();
    }
}
