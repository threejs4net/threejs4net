using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras
{
    class ShapeUtils
    {
        public static float Area(Vector2[] contour)
        {
            var n = contour.Length;
            float a = 0.0f;
            var p = n - 1;

            for (var q = 0; q < n; p = q++)
            {
                a += contour[p].X * contour[q].Y - contour[q].X * contour[p].Y;
            }

            return a * 0.5f;
        }

        public static bool isClockWise(IEnumerable<Vector2> pts)
        {
            return ShapeUtils.Area(pts.ToArray()) < 0;

        }

        public static List<int[]> TriangulateShape(Vector2[] contour, List<Vector2[]> holes)
        {
            var vertices = new List<float>(); // flat array of vertices like [ x0,y0, x1,y1, x2,y2, ... ]
            var holeIndices = new List<int>(); // array of hole indices
            var faces = new List<int[]>(); // final array of vertex indices like [ [ a,b,d ], [ b,c,d ] ]

            contour = RemoveDupEndPts(contour);
            AddContour(vertices, contour);

            var holeIndex = contour.Length;

            for (int i = 0; i < holes.Count; i++)
            {
                holes[i] = RemoveDupEndPts(holes[i]);
            }

            for (var i = 0; i < holes.Count; i++)
            {
                holeIndices.Add(holeIndex);
                holeIndex += holes[i].Length;
                AddContour(vertices, holes[i]);
            }

            var triangles = Earcut.Triangulate(vertices, holeIndices);

            for (var i = 0; i < triangles.Count; i += 3)
            {
                faces.Add(new [] { triangles[i], triangles[i+1], triangles[i+2] });
            }

            return faces;
        }

        private static Vector2[] RemoveDupEndPts(Vector2[] points)
        {
            var l = points.Length;
            if (l > 2 && points[l - 1].Equals(points[0]))
            {
                return points.Take(points.Length - 1).ToArray();
            }

            return points;
        }

        private static void AddContour(List<float> vertices, Vector2[] contour)
        {
            for (var i = 0; i < contour.Length; i++)
            {
                vertices.Add(contour[i].X);
                vertices.Add(contour[i].Y);
            }
        }
    }
}