using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    /// <summary>
    /// Data container for puzzle level, containing point set and triangles.
    /// </summary>

    [CreateAssetMenu(fileName = "PolygonLevel", menuName = "Levels/Puzzle Level")]
    public class PolygonLevel : ScriptableObject
    {
        [Header("Polygon Points")]

        public List<Vector2> Points = new List<Vector2>();
    }
}
