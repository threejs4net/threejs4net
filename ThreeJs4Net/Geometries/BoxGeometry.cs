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
            : base(other)
        {
        }

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

            for (var iy = 0; iy < gridY; iy ++)
            {
                for (var ix = 0; ix < gridX; ix ++)
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
}