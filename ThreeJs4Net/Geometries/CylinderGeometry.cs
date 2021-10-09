using System.Collections.Generic;
using System.Diagnostics;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class CylinderGeometry : Geometry
    {
        public float RadiusTop { get; }
        public float RadiusBottom { get; }
        public float Height { get; }
        public int RadialSegments { get; }
        public int HeightSegments { get; }
        public bool OpenEnded { get; }
        public float ThetaStart { get; }
        public float ThetaLength { get; }

        public CylinderGeometry(float radiusTop = 20, float radiusBottom = 20, float height = 10, int radialSegments = 8, int heightSegments = 1, bool openEnded = false, float thetaStart = 0, float thetaLength = Mathf.PI * 2)
            : base()
        {
            RadiusTop = radiusTop;
            RadiusBottom = radiusBottom;
            Height = height;

            RadialSegments = radialSegments;
            HeightSegments = heightSegments;

            ThetaLength = thetaLength;
            ThetaStart = thetaStart;

            OpenEnded = openEnded;

            FromBufferGeometry(new CylinderBufferGeometry(radiusTop, radiusBottom, height, radialSegments, heightSegments, openEnded, thetaStart, thetaLength));
            MergeVertices();            
        }
    }

    public class CylinderBufferGeometry : BufferGeometry
    {
        public float RadiusTop { get; }
        public float RadiusBottom { get; }
        public float Height { get; }
        public int RadialSegments { get; }
        public int HeightSegments { get; }
        public bool OpenEnded { get; }
        public float ThetaStart { get; }
        public float ThetaLength { get; }
        private List<uint> indices = new List<uint>();
        private List<float> vertices = new List<float>();
        private List<float> normals = new List<float>();
        private List<float> uvs = new List<float>();
        private List<List<int>> indexArray = new List<List<int>>();
        private float halfHeight;
        private int groupStart;
        private int index;

        public CylinderBufferGeometry(float radiusTop = 20, float radiusBottom = 20, float height = 10, int radialSegments = 8, int heightSegments = 1, bool openEnded = false, float thetaStart = 0, float thetaLength = Mathf.PI * 2)
        {
            RadiusTop = radiusTop;
            RadiusBottom = radiusBottom;
            Height = height != 0 ? height : 1;

            RadialSegments = radialSegments;
            HeightSegments = heightSegments;

            OpenEnded = openEnded;
            ThetaStart = thetaStart;
            ThetaLength = thetaLength;

            halfHeight = height / 2;

            GenerateTorso();

            if (!OpenEnded)
            {
                if (RadiusTop > 0) GenerateCap(true);
                if (RadiusBottom > 0) GenerateCap(false);
            }

            this.SetIndex(new BufferAttribute<uint>(indices.ToArray(), 1));
            SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
            SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));
            SetAttribute("uv", new BufferAttribute<float>(uvs.ToArray(), 2));
        }

        private void GenerateTorso()
        {
            int x, y;
            int groupCount = 0;

            Vector3 normal = Vector3.Zero();
            Vector3 vertex = Vector3.Zero();

            float slope = (RadiusBottom - RadiusTop) / Height;

            for (y = 0; y <= HeightSegments; y++)
            {
                List<int> indexRow = new List<int>();

                float v = y / (float)HeightSegments;

                float radius = v * (RadiusBottom - RadiusTop) + RadiusTop;

                for (x = 0; x <= RadialSegments; x++)
                {
                    float u = x / (float)RadialSegments;
                    float theta = u * ThetaLength + ThetaStart;

                    float sinTheta = (float)System.Math.Sin(theta);
                    float cosTheta = (float)System.Math.Cos(theta);

                    //vertex

                    vertex.X = radius * sinTheta;
                    vertex.Y = -v * Height + halfHeight;
                    vertex.Z = radius * cosTheta;
                    vertices.Add(vertex.X); vertices.Add(vertex.Y); vertices.Add(vertex.Z);

                    //normal

                    normal.Set(sinTheta, slope, cosTheta).Normalize();
                    normals.Add(normal.X); normals.Add(normal.Y); normals.Add(normal.Z);

                    //uv

                    uvs.Add(u); uvs.Add(1 - v);
                    indexRow.Add(index++);
                }
                indexArray.Add(indexRow);
            }

            // generate indices

            for (x = 0; x < RadialSegments; x++)
            {
                for (y = 0; y < HeightSegments; y++)
                {
                    uint a = (uint)indexArray[y][x];
                    uint b = (uint)indexArray[y + 1][x];
                    uint c = (uint)indexArray[y + 1][x + 1];
                    uint d = (uint)indexArray[y][x + 1];

                    indices.Add(a); indices.Add(b); indices.Add(d);
                    indices.Add(b); indices.Add(c); indices.Add(d);

                    groupCount += 6;
                }
            }

            AddGroup(groupStart, groupCount);

            groupStart += groupCount;
        }

        private void GenerateCap(bool top)
        {
            int x, centerIndexStart, centerIndexEnd;

            Vector2 uv = new Vector2();
            Vector3 vertex = new Vector3();

            int groupCount = 0;

            float radius = top ? RadiusTop : RadiusBottom;
            float sign = top ? 1 : -1;

            centerIndexStart = index;

            for (x = 1; x <= RadialSegments; x++)
            {
                vertices.Add(0); vertices.Add(halfHeight * sign); vertices.Add(0);
                normals.Add(0); normals.Add(sign); normals.Add(0);
                uvs.Add(0.5f); uvs.Add(0.5f);
                index++;
            }

            centerIndexEnd = index;

            for (x = 0; x <= RadialSegments; x++)
            {
                float u = x / (float)RadialSegments;
                float theta = u * ThetaLength + ThetaStart;

                float cosTheta = (float)System.Math.Cos(theta);
                float sinTheta = (float)System.Math.Sin(theta);

                //vertex

                vertex.X = radius * sinTheta;
                vertex.Y = halfHeight * sign;
                vertex.Z = radius * cosTheta;

                vertices.Add(vertex.X); vertices.Add(vertex.Y); vertices.Add(vertex.Z);

                //normal
                normals.Add(0); normals.Add(sign); normals.Add(0);

                //uv

                uv.X = (cosTheta * 0.5f) + 0.5f;
                uv.Y = (sinTheta * 0.5f * sign) + 0.5f;
                uvs.Add(uv.X); uvs.Add(uv.Y);

                index++;
            }

            //generate indices

            for (x = 0; x < RadialSegments; x++)
            {
                uint c = (uint)(centerIndexStart + x);
                uint i = (uint)(centerIndexEnd + x);

                if (top)
                {
                    indices.Add(i); indices.Add(i + 1); indices.Add(c);
                }
                else
                {
                    indices.Add(i + 1); indices.Add(i); indices.Add(c);
                }
                groupCount += 3;
            }

            AddGroup(groupStart, groupCount, top ? 1 : 2);

            groupStart += groupCount;
        }

    }
}
