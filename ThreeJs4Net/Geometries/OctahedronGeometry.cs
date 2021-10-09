using System.Collections.Generic;
using ThreeJs4Net.Core;

namespace ThreeJs4Net.Geometries
{
    public class OctahedronGeometry : Geometry
    {
        public OctahedronGeometry(float radius = 1, float detail = 0)
        {
            FromBufferGeometry(new OctahedronBufferGeometry(radius, detail));
            MergeVertices();
        }
    }

    public class OctahedronBufferGeometry : PolyhedronBufferGeometry
    {
        private new static List<float> Vertices { get; } = new List<float>
        {
                1, 0, 0, -1, 0, 0,  0, 1, 0,
                0, -1, 0, 0, 0, 1,  0, 0, - 1
            };

        private new static List<int> Indices { get; } = new List<int>
        {
                0, 2, 4,    0, 4, 3,    0, 3, 5,
                0, 5, 2,    1, 2, 5,    1, 5, 3,
                1, 3, 4,    1, 4, 2
            };

        public OctahedronBufferGeometry(float radius = 1, float detail = 0) : base(Vertices, Indices, radius, detail)
        {
        }
    }
}
