using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class CircleBufferGeometry : BufferGeometry
    {
        #region Fields
        public float Radius;
        public float ThetaStart;
        public float ThetaLength;
        public int Segments;
        #endregion

        public CircleBufferGeometry(float radius = 1, int segments = 8, float thetaStart = 0, float thetaength = Mathf.PI * 2)
        {
            this.Radius = radius;
            this.Segments = Mathf.Max(3, segments);
            this.ThetaStart = thetaStart;
            this.ThetaLength = thetaength;

            var indices = new List<uint>();
            var vertices = new List<float>();
            var normals = new List<float>();
            var uvs = new List<float>();

            var vertex = new Vector3();
            var uv = new Vector2();

            vertices.Add(0, 0, 0);
            normals.Add(0, 0, 1);
            uvs.Add((float)0.5, (float)0.5);

            var i = 3;
            for (var s = 0; s <= segments; s++, i += 3)
            {
                var segment = thetaStart + s / (float)segments * ThetaLength;

                vertex.X = radius * Mathf.Cos(segment);
                vertex.Y = radius * Mathf.Sin(segment);

                vertices.Add(vertex.X, vertex.Y, vertex.Z);

                // normal
                normals.Add(0, 0, 1);

                // uvs
                uv.X = (vertices[i] / radius + 1) / 2;
                uv.Y = (vertices[i + 1] / radius + 1) / 2;

                uvs.Add(uv.X, uv.Y);
            }

            for (i = 1; i <= segments; i++)
            {
                indices.Add((uint)i);
                indices.Add((uint)i + 1);
                indices.Add(0);
            }

            this.SetIndex(new BufferAttribute<uint>(indices.ToArray(), 1));
            this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
            this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));
            this.SetAttribute("uv", new BufferAttribute<float>(uvs.ToArray(), 2));
        }
    }
}
