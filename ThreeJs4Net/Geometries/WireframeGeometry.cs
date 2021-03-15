using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class WireframeGeometry : BufferGeometry
    {
        private readonly List<float> _vertices = new List<float>();

        public WireframeGeometry(BaseGeometry geometry)
        {
            var edges = new Dictionary<string, EdgeIndex>();
            var edge = new int[] { 0, 0 };
            var keys = new string[] { "a", "b", "c" };

            // different logic for Geometry and BufferGeometry
            if (geometry is Geometry geometryItem)
            {
                // create a data structure that contains all edges without duplicates
                var faces = geometryItem.Faces;
                for (var i = 0; i < faces.Count; i++)
                {
                    var face = faces[i];
                    for (var j = 0; j < 3; j++)
                    {
                        var edge1 = keys[j] == "a" ? face.A : keys[j] == "b" ? face.B : face.C;
                        var edge2 = keys[(j + 1) % 3] == "a" ? face.A : keys[(j + 1) % 3] == "b" ? face.B : face.C;
                        edge[0] = Mathf.Min(edge1, edge2); // sorting prevents duplicates
                        edge[1] = Mathf.Max(edge1, edge2);

                        string key = $"{edge[0]},{edge[1]}";

                        if (!edges.ContainsKey(key))
                        {
                            edges[key] = new EdgeIndex(edge[0], edge[1]);
                        }
                    }
                }

                // generate vertices
                foreach (var key in edges)
                {
                    var e = key.Value;

                    var vertex = geometryItem.Vertices[e.Index1];
                    _vertices.Add(vertex.X);
                    _vertices.Add(vertex.Y);
                    _vertices.Add(vertex.Z);

                    vertex = geometryItem.Vertices[e.Index2];
                    _vertices.Add(vertex.X);
                    _vertices.Add(vertex.Y);
                    _vertices.Add(vertex.Z);

                }
            }
            else if (geometry is BufferGeometry bufferGeometryItem)
            {
                var vertex = new Vector3();

                if (bufferGeometryItem.Index != null)
                {
                    // indexed BufferGeometry
                    var position = bufferGeometryItem.GetAttribute<float>("position");
                    var indices = bufferGeometryItem.Index;
                    var groups = bufferGeometryItem.groups;

                    if (groups.Count == 0)
                    {
                        groups = new List<BufferGeometryGroups>
                        {
                            new BufferGeometryGroups()
                            {
                                Start = 0, Count = indices.Count, MaterialIndex = 0
                            }
                        };
                    }

                    // create a data structure that contains all eges without duplicates
                    for (var o = 0; o < groups.Count; ++o)
                    {
                        var group = groups[o];

                        var start = group.Start;
                        var count = group.Count;

                        for (var i = start; i < start + count; i += 3)
                        {
                            for (var j = 0; j < 3; j++)
                            {
                                var edge1 = indices.GetX(i + j);
                                var edge2 = indices.GetX(i + (j + 1) % 3);
                                edge[0] = (int)Mathf.Min(edge1, edge2); // sorting prevents duplicates
                                edge[1] = (int)Mathf.Max(edge1, edge2);

                                var key = $"{edge[0]},{edge[1]}";

                                if (!edges.ContainsKey(key))
                                {
                                    edges[key] = new EdgeIndex(edge[0], edge[1]);
                                }
                            }
                        }
                    }

                    // generate vertices
                    foreach (var key in edges)
                    {
                        var e = key.Value;

                        vertex.FromBufferAttribute(position, e.Index1);
                        _vertices.Add(vertex.X);
                        _vertices.Add(vertex.Y);
                        _vertices.Add(vertex.Z);

                        vertex.FromBufferAttribute(position, e.Index2);
                        _vertices.Add(vertex.X);
                        _vertices.Add(vertex.Y);
                        _vertices.Add(vertex.Z);
                    }
                }
                else
                {
                    // non-indexed BufferGeometry
                    var position = bufferGeometryItem.GetAttribute<float>("position");
                    for (var i = 0; i < position.Count / 3; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            // three edges per triangle, an edge is represented as (index1, index2)
                            // e.g. the first triangle has the following edges: (0,1),(1,2),(2,0)

                            var index1 = 3 * i + j;
                            vertex.FromBufferAttribute(position, index1);
                            _vertices.Add(vertex.X);
                            _vertices.Add(vertex.Y);
                            _vertices.Add(vertex.Z);

                            var index2 = 3 * i + ((j + 1) % 3);
                            vertex.FromBufferAttribute(position, index2);
                            _vertices.Add(vertex.X);
                            _vertices.Add(vertex.Y);
                            _vertices.Add(vertex.Z);
                        }
                    }
                }
            }

            // build geometry
            this.SetAttribute("position", new BufferAttribute<float>(_vertices.ToArray(), 3));
        }
    }

    internal class EdgeIndex
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }

        public EdgeIndex(int index1, int index2)
        {
            this.Index1 = index1;
            this.Index2 = index2;
        }
    }
}


