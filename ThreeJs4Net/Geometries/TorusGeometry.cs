using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class TorusGeometry : Geometry
    {
        public float Radius;

        public float Tube;

        public int RadialSegments;

        public int TubularSegments;

        public float Arc;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="tube"></param>
        /// <param name="radialSegments"></param>
        /// <param name="tubularSegments"></param>
        /// <param name="arc"></param>
        public TorusGeometry(float radius = 100, float tube = 40, int radialSegments = 8, int tubularSegments = 6, float arc = (float)Mat.PI2)
        {
            this.Radius = radius;
            this.Tube = tube;
            this.RadialSegments = radialSegments;
            this.TubularSegments = tubularSegments;
            this.Arc = arc;

            var center = new Vector3(); var uvs = new List<Vector2>(); var normals = new List<Vector3>();

            for ( var j = 0; j <= radialSegments; j ++ ) 
            {
                for ( var i = 0; i <= tubularSegments; i ++ )
                {
                    var u = i / (float)tubularSegments * arc;
                    var v = j / (float)radialSegments * System.Math.PI * 2;

                    center.X = radius * (float)System.Math.Cos(u);
                    center.Y = radius * (float)System.Math.Sin(u);

                    var vertex = new Vector3();
                    vertex.X = (radius + tube * (float)System.Math.Cos(v)) * (float)System.Math.Cos(u);
                    vertex.Y = (radius + tube * (float)System.Math.Cos(v)) * (float)System.Math.Sin(u);
                    vertex.Z = tube * (float)System.Math.Sin(v);

                    this.Vertices.Add( vertex );

                    uvs.Add( new Vector2( i / (float)tubularSegments, j / (float)radialSegments ) );
                    normals.Add( ((Vector3)vertex.Clone()).Sub( center ).Normalize() );
                }
            }

            for ( var j = 1; j <= radialSegments; j ++ ) 
            {
                for ( var i = 1; i <= tubularSegments; i ++ )
                {
                    var a = (tubularSegments + 1) * j + i - 1;
                    var b = (tubularSegments + 1) * (j - 1) + i - 1;
                    var c = (tubularSegments + 1) * (j - 1) + i;
                    var d = (tubularSegments + 1) * j + i;

                    {
                        var face = new Face3(a, b, d);
                        face.VertexNormals.Add((Vector3)normals[a].Clone());
                        face.VertexNormals.Add((Vector3)normals[b].Clone());
                        face.VertexNormals.Add((Vector3)normals[d].Clone());
                        this.Faces.Add(face);
                        this.FaceVertexUvs[0].Add(new List<Vector2> { (Vector2)uvs[a].Clone(), (Vector2)uvs[b].Clone(), (Vector2)uvs[d].Clone() });
                        
                    }

                    {
                        var face = new Face3(b, c, d);
                        face.VertexNormals.Add((Vector3)normals[b].Clone());
                        face.VertexNormals.Add((Vector3)normals[c].Clone());
                        face.VertexNormals.Add((Vector3)normals[d].Clone());
                        this.Faces.Add(face);
                        this.FaceVertexUvs[0].Add(new List<Vector2> { (Vector2)uvs[b].Clone(), (Vector2)uvs[c].Clone(), (Vector2)uvs[d].Clone() });
                        
                    }
                }
            }

            this.ComputeFaceNormals();
        }
    }
}
