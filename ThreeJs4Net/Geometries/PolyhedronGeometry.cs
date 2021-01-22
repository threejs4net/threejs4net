using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class PolyhedronGeometry : Geometry
    {
        public struct UserData
        {
            public int index;
            public Vector2 uv;
        }


        public float Radius;

        public float Detail;

        private Vector3 _centroid;

        /// <summary>
        /// 
        /// </summary>
        protected PolyhedronGeometry()
        {
            this.type = "PolyhedronGeometry";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="radius"></param>
        /// <param name="detail"></param>
        public PolyhedronGeometry(IList<float> vertices, IList<int> indices, float radius, float detail)
        {
            this.Construct(vertices, indices, radius, detail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="radius"></param>
        /// <param name="detail"></param>
        protected void Construct(IList<float> vertices, IList<int> indices, float radius, float detail)
        {
            var i = 0; var j = 0; var l = 0;

            for (i = 0; i < vertices.Count; i += 3)
            {
                this.Prepare(new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]));
            }

            var faces = new List<Face3>();

            for (i = 0, j = 0, l = indices.Count; i < l; i += 3, j++)
            {
                var v1 = this.Vertices[indices[i + 0]];
                var v2 = this.Vertices[indices[i + 1]];
                var v3 = this.Vertices[indices[i + 2]];

                var face = new Face3(((UserData)v1.UserData).index, ((UserData)v2.UserData).index, ((UserData)v3.UserData).index);
                face.VertexNormals.Add((Vector3)v1.Clone());
                face.VertexNormals.Add((Vector3)v2.Clone());
                face.VertexNormals.Add((Vector3)v3.Clone());

                faces.Add(face);
            }

            _centroid = new Vector3();

            for (i = 0; i < faces.Count; i++)
            {
                Subdivide(faces[i], detail);
            }

            // Handle case when face straddles the seam

            for (i = 0; i < this.FaceVertexUvs[0].Count; i++)
            {
                var uvs = this.FaceVertexUvs[0][i];

                var x0 = uvs[0].X;
                var x1 = uvs[1].X;
                var x2 = uvs[2].X;

                var max = System.Math.Max(x0, System.Math.Max(x1, x2));
                var min = System.Math.Min(x0, System.Math.Min(x1, x2));

                if (max > 0.9 && min < 0.1)
                { // 0.9 is somewhat arbitrary
                    if (x0 < 0.2) uvs[0].X += 1;
                    if (x1 < 0.2) uvs[1].X += 1;
                    if (x2 < 0.2) uvs[2].X += 1;
                }
            }

            // Apply radius

            for (i = 0; i < this.Vertices.Count; i++)
            {
                this.Vertices[i].MultiplyScalar(radius);
            }

            // Merge vertices

            this.MergeVertices();

            this.ComputeFaceNormals();

            this.BoundingSphere = new Sphere(new Vector3(), radius);
        }

        /// <summary>
        /// Project vector onto sphere's surface
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private Vector3 Prepare(Vector3 vector) 
        {
            var vertex = (Vector3)(vector.Normalize().Clone());
            this.Vertices.Add(vertex);
            //vertex.index = this.Vertices.Count - 1;

            // Texture coords are equivalent to map coords, calculate angle and convert to fraction of a circle.

            var u = Azimuth( vector ) / 2 / System.Math.PI + 0.5;
            var v = Inclination(vector) / System.Math.PI + 0.5;
            //vertex.uv = new Vector2((float)u, (float)(1.0f - v));

            vertex.UserData = new UserData() { index = this.Vertices.Count - 1, uv = new Vector2((float)u, (float)(1.0f - v)) };

            return vertex;
        }


        /// <summary>
        /// Approximate a curved face with recursively sub-divided triangles.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        private void Make(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var face = new Face3(((UserData)v1.UserData).index, ((UserData)v2.UserData).index, ((UserData)v3.UserData).index);
            face.VertexNormals.Add((Vector3)v1.Clone());
            face.VertexNormals.Add((Vector3)v2.Clone());
            face.VertexNormals.Add((Vector3)v3.Clone());
            this.Faces.Add(face);

            _centroid.Copy(v1).Add(v2).Add(v3).DivideScalar(3);

            var azi = Azimuth(_centroid);

            this.FaceVertexUvs[0].Add(new List<Vector2> 
            {
                CorrectUV( ((UserData)v1.UserData).uv, v1, azi), 
                CorrectUV( ((UserData)v2.UserData).uv, v2, azi),
                CorrectUV( ((UserData)v2.UserData).uv, v3, azi) 
            });
        }
        

        /// <summary>
        /// Analytically subdivide a face to the required detail level.
        /// </summary>
        /// <param name="face"></param>
        /// <param name="detail"></param>
        private void Subdivide(Face3 face, float detail ) 
        {
            var cols = System.Math.Pow(2, detail);
            var cells = System.Math.Pow(4, detail);
            var a = Prepare(this.Vertices[face.a]);
            var b = Prepare(this.Vertices[face.b]);
            var c = Prepare(this.Vertices[face.c]);

            var v = new List<List<Vector3>>();

            // Construct all of the vertices for this subdivision.

            for ( var i = 0 ; i <= cols; i ++ )
            {
                v.Add(new List<Vector3>());

                var aj = Prepare((Vector3)a.Clone()).Lerp(c, i / (float)cols);
                var bj = Prepare((Vector3)b.Clone()).Lerp(c, i / (float)cols);
                var rows = cols - i;

                for ( var j = 0; j <= rows; j ++) 
                {
                    if ( j == 0 && i == cols )
                    {
                        v[i].Add(aj);
                    } else
                    {
                        v[i].Add(Prepare(((Vector3)aj.Clone()).Lerp(bj, j / (float)rows)));
                    }
                }
            }

            // Construct all of the faces.
            
            for ( var i = 0; i < cols ; i ++ ) 
            {
                for ( var j = 0; j < 2 * (cols - i) - 1; j ++ )
                {
                    var k = (int)System.Math.Floor(j / 2.0f);
                    if ( j % 2 == 0 )
                    {
                        Make(v[i][k + 1], v[i + 1][k], v[i][k]);
                    } else
                    {
                        Make(v[i][k + 1], v[i + 1][k + 1], v[i + 1][k]);
                    }
                }
            }
        }

        /// <summary>
        /// Angle around the Y axis, counter-clockwise when looking from above.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private float Azimuth(Vector3 vector ) {
            return (float)System.Math.Atan2( vector.Z, - vector.X );
        }
        
        /// <summary>
        /// Angle above the XZ plane.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private float Inclination(Vector3 vector)
        {
            return (float)System.Math.Atan2(-vector.Y, System.Math.Sqrt((vector.X * vector.X) + (vector.Z * vector.Z)));
        }
        
        /// <summary>
        /// Texture fixing helper. Spheres have some odd behaviours.
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="vector"></param>
        /// <param name="azimuth"></param>
        /// <returns></returns>
        private Vector2 CorrectUV(Vector2 uv, Vector3 vector, float azimuth )
        {
            if ( ( azimuth < 0 ) && ( uv.X == 1 ) ) uv = new Vector2( uv.X - 1, uv.Y );
            if ((vector.X == 0) && (vector.Z == 0)) uv = new Vector2(azimuth / 2.0f / (float)System.Math.PI + 0.5f, uv.Y);
            return (Vector2)uv.Clone();
        }


    }
}
