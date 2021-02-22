using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class BoxGeometry : Geometry
    {
        #region Fields

        private readonly int depthSegments;

        private readonly int heightSegments;

        private readonly int widthSegments;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public BoxGeometry(
            float width,
            float height,
            float depth,
            int widthSegments = 1,
            int heightSegments = 1,
            int depthSegments = 1)
        {
            this.widthSegments = widthSegments;
            this.heightSegments = heightSegments;
            this.depthSegments = depthSegments;

            var widthHalf = width / 2;
            var heightHalf = height / 2;
            var depthHalf = depth / 2;

            this.BuildPlane("z", "y", -1, -1, depth, height, widthHalf, 0); // px
            this.BuildPlane("z", "y", 1, -1, depth, height, -widthHalf, 1); // nx
            this.BuildPlane("x", "z", 1, 1, width, depth, heightHalf, 2); // py
            this.BuildPlane("x", "z", 1, -1, width, depth, -heightHalf, 3); // ny
            this.BuildPlane("x", "y", 1, -1, width, height, depthHalf, 4); // pz
            this.BuildPlane("x", "y", -1, -1, width, height, -depthHalf, 5); // nz        
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected BoxGeometry(BoxGeometry other)
            : base(other) { }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new BoxGeometry(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="udir"></param>
        /// <param name="vdir"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="materialIndex"></param>
        private void BuildPlane(
            string u,
            string v,
            int udir,
            int vdir,
            float width,
            float height,
            float depth,
            int materialIndex)
        {
            var w = string.Empty;
            var gridX = this.widthSegments;
            var gridY = this.heightSegments;
            var widthHalf = width / 2;
            var heightHalf = height / 2;
            var offset = this.Vertices.Count;

            if ((u == "x" && v == "y") || (u == "y" && v == "x"))
            {
                w = "z";
            }
            else if ((u == "x" && v == "z") || (u == "z" && v == "x"))
            {
                w = "y";
                gridY = this.depthSegments;
            }
            else if ((u == "z" && v == "y") || (u == "y" && v == "z"))
            {
                w = "x";
                gridX = this.depthSegments;
            }

            var gridX1 = gridX + 1;
            var gridY1 = gridY + 1;
            var segmentWidth = width / gridX;
            var segmentHeight = height / gridY;
            var normal = new Vector3().SetValue(w, depth > 0 ? 1 : -1);

            for (var iy = 0; iy < gridY1; iy++)
            {
                for (var ix = 0; ix < gridX1; ix++)
                {
                    var vector = new Vector3();
                    vector.SetValue(u, (ix * segmentWidth - widthHalf) * udir);
                    vector.SetValue(v, (iy * segmentHeight - heightHalf) * vdir);
                    vector.SetValue(w, depth);

                    this.Vertices.Add(vector);
                }
            }

            //if (this.FaceVertexUvs.Count < 1)
            //    this.FaceVertexUvs.Add(new List<List<Vector2>>());
            Debug.Assert(this.FaceVertexUvs.Count == 1, "Should only be 1 element at this stage");

            for (var iy = 0; iy < gridY; iy++)
            {
                for (var ix = 0; ix < gridX; ix++)
                {
                    var a = ix + gridX1 * iy;
                    var b = ix + gridX1 * (iy + 1);
                    var c = (ix + 1) + gridX1 * (iy + 1);
                    var d = (ix + 1) + gridX1 * iy;

                    var uva = new Vector2(ix / gridX, 1 - iy / gridY);
                    var uvb = new Vector2(ix / gridX, 1 - (iy + 1) / gridY);
                    var uvc = new Vector2((ix + 1) / gridX, 1 - (iy + 1) / gridY);
                    var uvd = new Vector2((ix + 1) / gridX, 1 - iy / gridY);

                    var face = new Face3(a + offset, b + offset, d + offset, Vector3.One(), Color.White);
                    face.Normal = normal;
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.MaterialIndex = materialIndex;

                    this.Faces.Add(face);
                    {
                        var uvs = new List<Vector2> { uva, uvb, uvd };
                        this.FaceVertexUvs[0].Add(uvs);
                    }

                    face = new Face3(b + offset, c + offset, d + offset, Vector3.One(), Color.White);
                    face.Normal = normal;
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.VertexNormals.Add((Vector3)normal.Clone());
                    face.MaterialIndex = materialIndex;

                    this.Faces.Add(face);
                    {
                        var uvs = new List<Vector2> { (Vector2)uvb.Clone(), uvc, (Vector2)uvd.Clone() };
                        this.FaceVertexUvs[0].Add(uvs);
                    }
                }
            }

            this.MergeVertices();
        }

        #endregion
    }

    public class BoxBufferGeometry : BufferGeometry
    {
        private BoxBufferGeometryParams parameters;
        private int numberOfVertices = 0;
        private int groupStart = 0;
        private List<uint> indices = new List<uint>();
        private List<float> vertices = new List<float>();
        private List<float> normals = new List<float>();
        private List<float> uvs = new List<float>();

        public BoxBufferGeometry(float width = 1, float height = 1, float depth = 1, int widthSegments = 1,
            int heightSegments = 1, int depthSegments = 1)
        {
            parameters = new BoxBufferGeometryParams()
            {
                Width = width,
                Height = height,
                Depth = depth,
                WidthSegments = widthSegments,
                HeightSegments = heightSegments,
                DepthSegments = depthSegments
            };

            buildPlane('z', 'y', 'x', -1, -1, depth, height, width, depthSegments, heightSegments, 0); // px
            buildPlane('z', 'y', 'x', 1, -1, depth, height, -width, depthSegments, heightSegments, 1); // nx
            buildPlane('x', 'z', 'y', 1, 1, width, depth, height, widthSegments, depthSegments, 2); // py
            buildPlane('x', 'z', 'y', 1, -1, width, depth, -height, widthSegments, depthSegments, 3); // ny
            buildPlane('x', 'y', 'z', 1, -1, width, height, depth, widthSegments, heightSegments, 4); // pz
            buildPlane('x', 'y', 'z', -1, -1, width, height, -depth, widthSegments, heightSegments, 5); // nz

            //geometry.AddAttribute( "index", new BufferAttribute<uint>( indices.ToArray(), 1 ) );
            this.SetIndex(new BufferAttribute<uint>( indices.ToArray(), 1 ) );
            this.SetAttribute( "position", new BufferAttribute<float>( vertices.ToArray(), 3 ) );
            this.SetAttribute( "normal", new BufferAttribute<float>( normals.ToArray(), 3 ) );
            this.SetAttribute( "uv", new BufferAttribute<float>( uvs.ToArray(), 2 ) );
        }

        private void buildPlane(char u1, char v1, char w1, int udir, int vdir, float width, float height, float depth, int gridX, int gridY, int materialIndex)
        {
            int u = u1 == 'x' ? 0 : u1 == 'y' ? 1 : 2;
            int v = v1 == 'x' ? 0 : v1 == 'y' ? 1 : 2;
            int w = w1 == 'x' ? 0 : w1 == 'y' ? 1 : 2;

            var segmentWidth = width / gridX;
            var segmentHeight = height / gridY;

            var widthHalf = width / 2;
            var heightHalf = height / 2;
            var depthHalf = depth / 2;

            var gridX1 = gridX + 1;
            var gridY1 = gridY + 1;

            var vertexCounter = 0;
            var groupCount = 0;

            int ix, iy;

            var vector = new Vector3();

            // generate vertices, normals and uvs

            for (iy = 0; iy < gridY1; iy++)
            {
                var y = iy * segmentHeight - heightHalf;
                for (ix = 0; ix < gridX1; ix++)
                {
                    var x = ix * segmentWidth - widthHalf;

                    // set values to correct vector component
                    vector[u] = x * udir;
                    vector[v] = y * vdir;
                    vector[w] = depthHalf;

                    // now apply vector to vertex buffer

                    vertices.Add(vector.X);
                    vertices.Add(vector.Y);
                    vertices.Add(vector.Z);

                    // set values to correct vector component

                    vector[u] = 0;
                    vector[v] = 0;
                    vector[w] = depth > 0 ? 1 : -1;

                    // now apply vector to normal buffer

                    normals.Add(vector.X);
                    normals.Add(vector.Y);
                    normals.Add(vector.Z);

                    // uvs
                    uvs.Add(ix / gridX);
                    uvs.Add(1 - (iy / gridY));

                    // counters
                    vertexCounter += 1;
                }
            }

            // indices

            // 1. you need three indices to draw a single face
            // 2. a single segment consists of two faces
            // 3. so we need to generate six (2*3) indices per segment
            for (iy = 0; iy < gridY; iy++)
            {
                for (ix = 0; ix < gridX; ix++)
                {
                    var a = numberOfVertices + ix + gridX1 * iy;
                    var b = numberOfVertices + ix + gridX1 * (iy + 1);
                    var c = numberOfVertices + (ix + 1) + gridX1 * (iy + 1);
                    var d = numberOfVertices + (ix + 1) + gridX1 * iy;

                    // faces
                    indices.Add((uint)a); indices.Add((uint)b); indices.Add((uint)d);
                    indices.Add((uint)b); indices.Add((uint)c); indices.Add((uint)d);

                    // increase counter
                    groupCount += 6;
                }
            }

            // add a group to the geometry. this will ensure multi material support
            this.AddGroup(groupStart, groupCount, materialIndex);

            // calculate new start value for groups
            groupStart += groupCount;

            // update total number of vertices
            numberOfVertices += vertexCounter;
        }

        internal class BoxBufferGeometryParams
        {
            public float Width { get; set; }
            public float Height { get; set; }
            public float Depth { get; set; }
            public int WidthSegments { get; set; }
            public float HeightSegments { get; set; }
            public float DepthSegments { get; set; }
        }
    }
}
