using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class PolyhedronGeometry : Geometry
    {
        public float Radius { get; }
        public float Detail { get; }

        public List<float> Vertices;
        public List<int> Indices;

        public PolyhedronGeometry(List<float> vertices, List<int> indices, float radius, float detail)
        {
            Vertices = vertices ?? new List<float>();
            Indices = indices ?? new List<int>();
            Radius = radius;
            Detail = detail;

            if (radius == 0) radius = 1;

            FromBufferGeometry(new PolyhedronBufferGeometry(vertices, indices, radius, detail));
            MergeVertices();
        }
    }

    public class PolyhedronBufferGeometry : BufferGeometry
    {
        public float Radius { get; }
        public float Detail { get; }

        public List<float> Vertices;
        public List<int> Indices;
        private List<float> VertexBuffer = new List<float>();
        private List<float> UvBuffer = new List<float>();

        public PolyhedronBufferGeometry(List<float> vertices, List<int> indices, float radius = 1, float detail = 0)
        {
            Vertices = vertices ?? new List<float>();
            Indices = indices ?? new List<int>();
            Radius = radius;
            Detail = detail;

            if (radius == 0) radius = 1;

            Subdivide(detail);
            ApplyRadius(radius);

            GenerateUVs();

            SetAttribute("position", new BufferAttribute<float>(VertexBuffer.ToArray(), 3));
            SetAttribute("normal", new BufferAttribute<float>(VertexBuffer.ToArray(), 3));
            SetAttribute("uv", new BufferAttribute<float>(UvBuffer.ToArray(), 2));

            if (detail == 0)
            {
                ComputeVertexNormals();
            }
            else
            {
                NormalizeNormals();
            }
        }

        private void Subdivide(float vDetail)
        {
            var a = new Vector3();
            var b = new Vector3();
            var c = new Vector3();

            // iterate over all faces and apply a subdivison with the given detail value
            for (var i = 0; i < Indices.Count; i += 3)
            {
                // get the vertices of the face
                GetVertexByIndex(Indices[i + 0], a);
                GetVertexByIndex(Indices[i + 1], b);
                GetVertexByIndex(Indices[i + 2], c);

                // perform subdivision
                SubdivideFace(a, b, c, vDetail);
            }
        }

        private void SubdivideFace(Vector3 a, Vector3 b, Vector3 c, float vDetail)
        {
            float cols = (float)System.Math.Pow(2, vDetail);

            // we use this multidimensional array as a data structure for creating the subdivision
            var v = new List<List<Vector3>>();
            int i, j;

            // construct all of the vertices for this subdivision
            for (i = 0; i <= cols; i++)
            {
                v.Add(new List<Vector3>());

                var aj = a.Clone().Lerp(c, i / cols);
                var bj = b.Clone().Lerp(c, i / cols);
                float rows = cols - i;

                for (j = 0; j <= rows; j++)
                {
                    if (j == 0 && i == cols)
                    {
                        v[i].Add(aj);
                    }
                    else
                    {
                        v[i].Add(aj.Clone().Lerp(bj, j / rows));
                    }
                }
            }

            // construct all of the faces
            for (i = 0; i < cols; i++)
            {
                for (j = 0; j < 2 * (cols - i) - 1; j++)
                {
                    var k = Mathf.Floor((float)j / 2);

                    if (j % 2 == 0)
                    {
                        PushVertex(v[i][k + 1]);
                        PushVertex(v[i + 1][k]);
                        PushVertex(v[i][k]);
                    }
                    else
                    {
                        PushVertex(v[i][k + 1]);
                        PushVertex(v[i + 1][k + 1]);
                        PushVertex(v[i + 1][k]);
                    }
                }
            }
        }

        private void ApplyRadius(float vRadius)
        {
            var vertex = new Vector3();

            // iterate over the entire buffer and apply the radius to each vertex
            for (var i = 0; i < VertexBuffer.Count; i += 3)
            {
                vertex.X = VertexBuffer[i + 0];
                vertex.Y = VertexBuffer[i + 1];
                vertex.Z = VertexBuffer[i + 2];

                vertex.Normalize().MultiplyScalar(vRadius);

                VertexBuffer[i + 0] = vertex.X;
                VertexBuffer[i + 1] = vertex.Y;
                VertexBuffer[i + 2] = vertex.Z;
            }
        }

        private void GenerateUVs()
        {
            var vertex = new Vector3();

            for (var i = 0; i < VertexBuffer.Count; i += 3)
            {
                vertex.X = VertexBuffer[i + 0];
                vertex.Y = VertexBuffer[i + 1];
                vertex.Z = VertexBuffer[i + 2];

                var u = (float)(Azimuth(vertex) / 2 / (Mathf.PI + 0.5));
                var v = (float)(Inclination(vertex) / (Mathf.PI + 0.5));
                UvBuffer.Add(u);
                UvBuffer.Add(1 - v);
            }

            CorrectUVs();
            CorrectSeam();
        }

        private void CorrectSeam()
        {
            // handle case when face straddles the seam, see #3269

            for (var i = 0; i < UvBuffer.Count; i += 6)
            {
                // uv data of a single face
                var x0 = UvBuffer[i + 0];
                var x1 = UvBuffer[i + 2];
                var x2 = UvBuffer[i + 4];
                var max = Mathf.Max(x0, Mathf.Max(x1, x2));
                var min = Mathf.Min(x0, Mathf.Min(x1, x2));

                // 0.9 is somewhat arbitrary
                if (max > 0.9 && min < 0.1)
                {
                    if (x0 < 0.2) UvBuffer[i + 0] += 1;
                    if (x1 < 0.2) UvBuffer[i + 2] += 1;
                    if (x2 < 0.2) UvBuffer[i + 4] += 1;
                }
            }
        }
        private void PushVertex(Vector3 vertex)
        {
            VertexBuffer.Add(vertex.X);
            VertexBuffer.Add(vertex.Y);
            VertexBuffer.Add(vertex.Z);
        }

        private void GetVertexByIndex(int index, Vector3 vertex)
        {
            var stride = index * 3;

            vertex.X = Vertices[stride + 0];
            vertex.Y = Vertices[stride + 1];
            vertex.Z = Vertices[stride + 2];
        }

        private void CorrectUVs()
        {
            var a = new Vector3();
            var b = new Vector3();
            var c = new Vector3();

            var centroid = new Vector3();

            var uvA = new Vector2();
            var uvB = new Vector2();
            var uvC = new Vector2();

            for (int i = 0, j = 0; i < VertexBuffer.Count; i += 9, j += 6)
            {
                a.Set(VertexBuffer[i + 0], VertexBuffer[i + 1], VertexBuffer[i + 2]);
                b.Set(VertexBuffer[i + 3], VertexBuffer[i + 4], VertexBuffer[i + 5]);
                c.Set(VertexBuffer[i + 6], VertexBuffer[i + 7], VertexBuffer[i + 8]);

                uvA.Set(UvBuffer[j + 0], UvBuffer[j + 1]);
                uvB.Set(UvBuffer[j + 2], UvBuffer[j + 3]);
                uvC.Set(UvBuffer[j + 4], UvBuffer[j + 5]);

                centroid.Copy(a).Add(b).Add(c).DivideScalar(3);

                var azi = Azimuth(centroid);

                CorrectUV(uvA, j + 0, a, azi);
                CorrectUV(uvB, j + 2, b, azi);
                CorrectUV(uvC, j + 4, c, azi);
            }
        }

        private void CorrectUV(Vector2 uv, int stride, Vector3 vector, float azimuth)
        {
            if (azimuth < 0 && uv.X == 1)
            {
                UvBuffer[stride] = uv.X - 1;
            }

            if (vector.X == 0 && vector.Z == 0)
            {
                UvBuffer[stride] = (float)(azimuth / 2 / Mathf.PI + 0.5);
            }
        }

        // Angle around the Y axis, counter-clockwise when looking from above.

        private float Azimuth(Vector3 vector)
        {
            return Mathf.Atan2(vector.Z, -vector.X);
        }


        // Angle above the XZ plane.
        private float Inclination(Vector3 vector)
        {
            return Mathf.Atan2(-vector.Y, Mathf.Sqrt((vector.X * vector.X) + (vector.Z * vector.Z)));
        }
    }
}
