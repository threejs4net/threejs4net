using System.Collections.Generic;
using System.Diagnostics;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class CylinderGeometry : Geometry
    {
        #region Fields
        public float RadiusTop;
        public float RadiusBottom;
        public float Height;
        public int RadialSegments;
        public int HeightSegments;
        public bool OpenEnded;
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public CylinderGeometry(float radiusTop = 20, float radiusBottom = 20, float height = 100, int radialSegments = 8, int heightSegments = 1, bool openEnded = false)
        {
            Debug.Assert(this.FaceVertexUvs.Count == 1, "Should only be 1 element at this stage");

            this.RadiusTop = radiusTop;
            this.RadiusBottom = radiusBottom;
            this.Height = height;

            this.RadialSegments = radialSegments;
            this.HeightSegments = heightSegments;

            this.OpenEnded = openEnded;

            var heightHalf = height / 2;

            var uvs = new List<List<Vector2>>();
            var vertices = new List<List<int>>();

            for (var y = 0; y <= heightSegments; y++)
            {
                var verticesRow = new List<int>();
                var uvsRow = new List<Vector2>();

                var v = y / (float)heightSegments;
                var radius = v * (radiusBottom - radiusTop) + radiusTop;

                for (var x = 0; x <= radialSegments; x++)
                {

                    var u = x / (float)radialSegments;

                    var vertex = new Vector3();
                    vertex.X = radius * (float)System.Math.Sin(u * System.Math.PI * 2);
                    vertex.Y = -v * height + heightHalf;
                    vertex.Z = radius * (float)System.Math.Cos(u * System.Math.PI * 2);

                    this.Vertices.Add(vertex);

                    verticesRow.Add(this.Vertices.Count - 1);
                    uvsRow.Add(new Vector2(u, 1 - v));
                }

                vertices.Add(verticesRow);
                uvs.Add(uvsRow);
            }


            var tanTheta = (radiusBottom - radiusTop) / height;

            for (var x = 0; x < radialSegments; x++)
            {
                Vector3 na;
                Vector3 nb;

                if (radiusTop != 0)
                {
                    na = (Vector3)this.Vertices[vertices[0][x]].Clone();
                    nb = (Vector3)this.Vertices[vertices[0][x + 1]].Clone();
                }
                else
                {
                    na = (Vector3)this.Vertices[vertices[1][x]].Clone();
                    nb = (Vector3)this.Vertices[vertices[1][x + 1]].Clone();
                }

                na.Y = (float)System.Math.Sqrt(na.X * na.X + na.Z * na.Z) * tanTheta;
                nb.Y = (float)System.Math.Sqrt(nb.X * nb.X + nb.Z * nb.Z) * tanTheta;
                na.Normalize();
                nb.Normalize();

                for (var y = 0; y < heightSegments; y++)
                {
                    var v1 = vertices[y][x];
                    var v2 = vertices[y + 1][x];
                    var v3 = vertices[y + 1][x + 1];
                    var v4 = vertices[y][x + 1];

                    var n1 = (Vector3)na.Clone();
                    var n2 = (Vector3)na.Clone();
                    var n3 = (Vector3)nb.Clone();
                    var n4 = (Vector3)nb.Clone();

                    var uv1 = (Vector2)uvs[y][x].Clone();
                    var uv2 = (Vector2)uvs[y + 1][x].Clone();
                    var uv3 = (Vector2)uvs[y + 1][x + 1].Clone();
                    var uv4 = (Vector2)uvs[y][x + 1].Clone();

                    {
                        var face = new Face3(v1, v2, v4);
                        face.VertexNormals.Add(n1);
                        face.VertexNormals.Add(n2);
                        face.VertexNormals.Add(n4);
                        this.Faces.Add(face);
                    }

                    this.FaceVertexUvs[0].Add(new List<Vector2> { uv1, uv2, uv4 });

                    {
                        var face = new Face3(v2, v3, v4);
                        face.VertexNormals.Add((Vector3)n2.Clone());
                        face.VertexNormals.Add(n3);
                        face.VertexNormals.Add((Vector3)n4.Clone());
                        this.Faces.Add(face);
                    }

                    this.FaceVertexUvs[0].Add(new List<Vector2> { (Vector2)uv2.Clone(), uv3, (Vector2)uv4.Clone() });
                }
            }

            // top cap

            if (openEnded == false && radiusTop > 0)
            {
                this.Vertices.Add(new Vector3(0, heightHalf, 0));

                for (var x = 0; x < radialSegments; x++)
                {
                    var v1 = vertices[0][x];
                    var v2 = vertices[0][x + 1];
                    var v3 = this.Vertices.Count - 1;

                    var n1 = new Vector3(0, 1, 0);
                    var n2 = new Vector3(0, 1, 0);
                    var n3 = new Vector3(0, 1, 0);

                    var uv1 = (Vector2)uvs[0][x].Clone();
                    var uv2 = (Vector2)uvs[0][x + 1].Clone();
                    var uv3 = new Vector2(uv2.X, 0);

                    var face = new Face3(v1, v2, v3);
                    face.VertexNormals.Add(n1);
                    face.VertexNormals.Add(n2);
                    face.VertexNormals.Add(n3);
                    this.Faces.Add(face);

                    this.FaceVertexUvs[0].Add(new List<Vector2> { uv1, uv2, uv3 });

                }
            }

            // bottom cap

            if (openEnded == false && radiusBottom > 0)
            {
                this.Vertices.Add(new Vector3(0, -heightHalf, 0));

                var y = heightSegments;

                for (var x = 0; x < radialSegments; x++)
                {
                    var v1 = vertices[y][x + 1];
                    var v2 = vertices[y][x];
                    var v3 = this.Vertices.Count - 1;

                    var n1 = new Vector3(0, -1, 0);
                    var n2 = new Vector3(0, -1, 0);
                    var n3 = new Vector3(0, -1, 0);

                    var uv1 = (Vector2)uvs[y][x + 1].Clone();
                    var uv2 = (Vector2)uvs[y][x].Clone();
                    var uv3 = new Vector2(uv2.X, 1);

                    var face = new Face3(v1, v2, v3);
                    face.VertexNormals.Add(n1);
                    face.VertexNormals.Add(n2);
                    face.VertexNormals.Add(n3);
                    this.Faces.Add(face);

                    this.FaceVertexUvs[0].Add(new List<Vector2> { uv1, uv2, uv3 });
                }
            }

            this.ComputeFaceNormals();
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected CylinderGeometry(CylinderGeometry other)
            : base(other)
        {
        }

        #endregion
    }
}
