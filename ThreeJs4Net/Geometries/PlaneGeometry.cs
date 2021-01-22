using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class PlaneGeometry : Geometry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="widthSegments"></param>
        /// <param name="heightSegments"></param>
        public PlaneGeometry(float width, float height, int widthSegments = 1, int heightSegments = 1)
        {
            Debug.Assert(this.FaceVertexUvs.Count == 1, "Should only be 1 element at this stage");

            var widthHalf = width / 2;
            var heightHalf = height / 2;
            var gridX = widthSegments;
            var gridZ = heightSegments;
            var gridX1 = gridX + 1;
            var gridZ1 = gridZ + 1;
            var segmentWidth = width / gridX;
            var segmentHeight = height / gridZ;

            var normal = new Vector3(0, 0, 1);

            for (var iz = 0; iz < gridZ1; iz ++ )
            {
                var y = iz * segmentHeight - heightHalf;
                for (var ix = 0; ix < gridX1; ix ++ )
                {
                    var x = ix * segmentWidth - widthHalf;
                    this.Vertices.Add(new Vector3(x, - y, 0));
                }
            }

            for (var iz = 0; iz < gridZ; iz ++ ) 
            {
                for (var ix = 0; ix < gridX; ix ++ )
                {
                    var a = ix + gridX1 * iz;
                    var b = ix + gridX1 * (iz + 1);
                    var c = (ix + 1) + gridX1 * (iz + 1);
                    var d = (ix + 1) + gridX1 * iz;

                    var uva = new Vector2(ix / (float)gridX1, 1 - iz / gridZ);
                    var uvb = new Vector2(ix / (float)gridX1, 1 - (iz + 1) / gridZ);
                    var uvc = new Vector2((ix + 1) / (float)gridX1, 1 - (iz + 1) / gridZ);
                    var uvd = new Vector2((ix + 1) / (float)gridX1, 1 - iz / gridZ);

                    var face = new Face3( a, b, d, Vector3.One(), Color.White );
                    face.Normal = normal;
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    this.Faces.Add(face);

                    this.FaceVertexUvs[0].Add(new List<Vector2> { uva, uvb, uvd });

                    face = new Face3(b, c, d, Vector3.One(), Color.White);
                    face.Normal = normal;
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    this.Faces.Add(face);

                    this.FaceVertexUvs[0].Add(new List<Vector2> { (Vector2)uvb.Clone(), uvc, (Vector2)uvd.Clone() });
                }
            }
        }
    }
}
