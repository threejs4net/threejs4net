using System.Collections.Generic;
using System.Diagnostics;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class SphereGeometry : Geometry
    {
        public float Radius;

        public int WidthSegments;

        public int HeightSegments;

        public float PhiStart;

        public float PhiLength;

        public float ThetaStart;

        public float ThetaLength;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="widthSegments"></param>
        /// <param name="heightSegments"></param>
        /// <param name="phiStart"></param>
        /// <param name="phiLength"></param>
        /// <param name="thetaStart"></param>
        /// <param name="thetaLength"></param>
        public SphereGeometry(float radius = 50, int widthSegments = 8, int heightSegments = 6, float phiStart = 0, float phiLength = (float)Mat.PI2, float thetaStart = 0, float thetaLength = (float)System.Math.PI)
        {
            Debug.Assert(this.FaceVertexUvs.Count == 1, "Should only be 1 element at this stage");

            this.Radius = radius;
            
            this.WidthSegments = widthSegments;
            this.HeightSegments = heightSegments;
            
            this.PhiStart = phiStart;
            this.PhiLength = phiLength;

            this.ThetaStart = thetaStart;
            this.ThetaLength = thetaLength;

            var uvs = new List<List<Vector2>>();
            var vertices = new List<List<int>>();

	        for (var y = 0; y <= heightSegments; y ++ ) {

                var verticesRow = new List<int>();
                var uvsRow = new List<Vector2>();

		        for (var x = 0; x <= widthSegments; x ++ ) {

			        var u = x / (float)widthSegments;
                    var v = y / (float)heightSegments;

			        var vertex = new Vector3();
			        vertex.X = - radius * (float)System.Math.Cos( phiStart + u * phiLength ) * (float)System.Math.Sin( thetaStart + v * thetaLength );
			        vertex.Y = radius * (float)System.Math.Cos( thetaStart + v * thetaLength );
			        vertex.Z = radius * (float)System.Math.Sin( phiStart + u * phiLength ) * (float)System.Math.Sin( thetaStart + v * thetaLength );

		            this.Vertices.Add(vertex);

		            verticesRow.Add(this.Vertices.Count - 1);
		            uvsRow.Add(new Vector2(u, 1 - v));

		        }

	            vertices.Add(verticesRow);
	            uvs.Add(uvsRow);
	        }

	        for (var y = 0; y < heightSegments; y ++ ) {

		        for (var x = 0; x < widthSegments; x ++ )
		        {

		            var v1 = vertices[y][x + 1];
		            var v2 = vertices[y][x];
		            var v3 = vertices[y + 1][x];
		            var v4 = vertices[y + 1][x + 1];

		            var n1 = ((Vector3)this.Vertices[v1].Clone()).Normalize();
                    var n2 = ((Vector3)this.Vertices[v2].Clone()).Normalize();
                    var n3 = ((Vector3)this.Vertices[v3].Clone()).Normalize();
                    var n4 = ((Vector3)this.Vertices[v4].Clone()).Normalize();

		            var uv1 = (Vector2)uvs[y][x + 1].Clone();
                    var uv2 = (Vector2)uvs[y][x].Clone();
                    var uv3 = (Vector2)uvs[y + 1][x].Clone();
                    var uv4 = (Vector2)uvs[y + 1][x + 1].Clone();

			        if ( System.Math.Abs( this.Vertices[ v1 ].Y ) == radius ) {

				        uv1.X = ( uv1.X + uv2.X ) / 2;

                        var face = new Face3(v1, v3, v4);
                        face.VertexNormals.Add(n1);
                        face.VertexNormals.Add(n3);
                        face.VertexNormals.Add(n4);
				        this.Faces.Add(face);

			            this.FaceVertexUvs[0].Add(new List<Vector2> { uv1, uv3, uv4 });

			        } else if ( System.Math.Abs( this.Vertices[ v3 ].Y ) == radius ) {

				        uv3.X = ( uv3.X + uv4.X ) / 2;

			            var face = new Face3(v1, v2, v3);
                        face.VertexNormals.Add(n1);
                        face.VertexNormals.Add(n2);
                        face.VertexNormals.Add(n3);
				        this.Faces.Add(face);

			            this.FaceVertexUvs[0].Add(new List<Vector2> { uv1, uv2, uv3 });

			        } else {

			            var face = new Face3(v1, v2, v4);
				        this.Faces.Add(face);
                        face.VertexNormals.Add(n1);
                        face.VertexNormals.Add(n2);
                        face.VertexNormals.Add(n4);

			            this.FaceVertexUvs[0].Add(new List<Vector2> { uv1, uv2, uv4 });

			            face = new Face3(v2, v3, v4);
			            this.Faces.Add(face);
                        face.VertexNormals.Add((Vector3)n2.Clone());
                        face.VertexNormals.Add(n3);
                        face.VertexNormals.Add((Vector3)n4.Clone());

                        this.FaceVertexUvs[0].Add(new List<Vector2> { (Vector2)uv2.Clone(), uv3, (Vector2)uv4.Clone() });

			        }

		        }

	        }

	        this.ComputeFaceNormals();

            this.BoundingSphere = new Sphere(new Vector3(), radius);
        }
    }
}
