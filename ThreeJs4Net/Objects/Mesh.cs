using System.Collections.Generic;
using System.Drawing;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Objects
{
    public class Intersect
    {
        public float Distance;

        public Vector3 Point;

        public int[] Indices;

        public Face3 Face;

        public int FaceIndex;

        public Object3D Object3D;
    }


    public class Mesh : Object3D
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        public Mesh(BaseGeometry geometry = null, Material material = null)
        {
            this.type = "Mesh";
            
            this.Geometry = geometry ?? new Geometry();
            this.Material = material ?? new MeshBasicMaterial() { Color = new Color().Random() };

            UpdateMorphTargets();
        }

        /// <summary>
        /// 
        /// </summary>
        protected Mesh()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Mesh(Mesh other)
            : base(other)
        {

        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Mesh(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateMorphTargets()
        {
            //if (null != this.geometry.MorphTargets && this.geometry.MorphTargets.count > 0)
            //{
            //    this.MorphTargetBase = - 1;
            //    this.MorphTargetForcedOrder.Clear();
            //    this.MorphTargetInfluences.Clear();
            //    this.MorphTargetDictionary.Clear();

            //    foreach (var m in this.geometry.MorphTargets)
            //    {
            //        this.MorphTargetInfluences.Add(0);
            //        this.MorphTargetDictionary[this.geometry.MorphTargets[m].name] = m;
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raycaster"></param>
        /// <param name="ray"></param>
        /// <param name="geometry"></param>
        /// <param name="intersects"></param>
        private void Raycast(Raycaster raycaster, Ray ray, Geometry geometry, ref List<Intersect> intersects)
        {
            var isFaceMaterial = this.Material is MeshFaceMaterial;
            var objectMaterials = isFaceMaterial == true ? ((MeshFaceMaterial)this.Material).Materials : null;

            var precision = raycaster.Precision;

            var vertices = geometry.Vertices;

            var vA = new Vector3();
            var vB = new Vector3();
            var vC = new Vector3();

            for (var f = 0; f < geometry.Faces.Count; f++)
            {

                var face = geometry.Faces[f];

                var material = isFaceMaterial ? objectMaterials[face.MaterialIndex] : this.Material;

                if (material == null) continue;

                var a = vertices[face.a];
                var b = vertices[face.b];
                var c = vertices[face.c];
                /*
                        if ( material.MorphTargets == true ) {

                            var morphTargets = geometry.morphTargets;
                            var morphInfluences = this.morphTargetInfluences;

                            vA = new Vector3( 0, 0, 0 );
                            vB = new Vector3( 0, 0, 0 );
                            vC = new Vector3( 0, 0, 0 );

                            for ( var t = 0; t < morphTargets.length; t ++ ) {

                                var influence = morphInfluences[ t ];

                                if ( influence == 0 ) continue;

                                var targets = morphTargets[ t ].vertices;

                                vA.X += ( targets[ face.a ].x - a.X ) * influence;
                                vA.Y += ( targets[ face.a ].y - a.Y ) * influence;
                                vA.Z += ( targets[ face.a ].z - a.Z ) * influence;

                                vB.X += ( targets[ face.b ].x - b.X ) * influence;
                                vB.Y += ( targets[ face.b ].y - b.Y ) * influence;
                                vB.Z += ( targets[ face.b ].z - b.Z ) * influence;

                                vC.X += ( targets[ face.c ].x - c.X ) * influence;
                                vC.Y += ( targets[ face.c ].y - c.Y ) * influence;
                                vC.Z += ( targets[ face.c ].z - c.Z ) * influence;

                            }

                            vA.Add( a );
                            vB.Add( b );
                            vC.Add( c );

                            a = vA;
                            b = vB;
                            c = vC;

                        }
*/
                Vector3 intersectionPoint;
                if (material.Side == Three.BackSide)
                {

                    intersectionPoint = ray.IntersectTriangle(c, b, a, true);

                }
                else
                {

                    intersectionPoint = ray.IntersectTriangle(a, b, c, material.Side != Three.DoubleSide);

                }

                if (intersectionPoint == null) continue;

                intersectionPoint.ApplyMatrix4(this.MatrixWorld);

                var distance = raycaster.Ray.Origin.DistanceTo(intersectionPoint);

                if (distance < precision || distance < raycaster.Near || distance > raycaster.Far) continue;

                intersects.Add(
                    new Intersect()
                        {
                            Distance = distance,
                            Point = intersectionPoint,
                            Face = face,
                            FaceIndex = f,
                            Object3D = this
                        });

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raycaster"></param>
        /// <param name="ray"></param>
        /// <param name="geometry"></param>
        /// <param name="intersects"></param>
        private void Raycast(Raycaster raycaster, Ray ray, BufferGeometry geometry, ref List<Intersect> intersects)
        {
            var material = this.Material;

            if (material == null) return;

            var attributes = geometry.Attributes;

            var precision = raycaster.Precision;

            var vA = new Vector3();
            var vB = new Vector3();
            var vC = new Vector3();

            if (attributes.ContainsKey("index"))
            {

                var indices = ((BufferAttribute<short>)attributes["index"]).Array;
                var positions = ((BufferAttribute<float>)attributes["position"]).Array;
                var offsets = geometry.Offsets;

                if (offsets.Count == 0)
                {

                    offsets.Add(new Offset() { Start = 0, Count = indices.Length, Index = 0 });

                }

                for (var oi = 0; oi < offsets.Count; ++oi)
                {

                    var start = offsets[oi].Start;
                    var count = offsets[oi].Count;
                    var index = offsets[oi].Index;

                    for (var i = start; i < start + count; i += 3)
                    {

                        var a = index + indices[i];
                        var b = index + indices[i + 1];
                        var c = index + indices[i + 2];

                        vA.Set(
                            positions[a * 3],
                            positions[a * 3 + 1],
                            positions[a * 3 + 2]
                        );
                        vB.Set(
                            positions[b * 3],
                            positions[b * 3 + 1],
                            positions[b * 3 + 2]
                        );
                        vC.Set(
                            positions[c * 3],
                            positions[c * 3 + 1],
                            positions[c * 3 + 2]
                        );

                        Vector3 intersectionPoint;
                        if (material.Side == Three.BackSide)
                        {

                            intersectionPoint = ray.IntersectTriangle(vC, vB, vA, true);

                        }
                        else
                        {

                            intersectionPoint = ray.IntersectTriangle(vA, vB, vC, material.Side != Three.DoubleSide);

                        }

                        if (intersectionPoint == null) continue;

                        intersectionPoint.ApplyMatrix4(this.MatrixWorld);

                        var distance = raycaster.Ray.Origin.DistanceTo(intersectionPoint);

                        if (distance < precision || distance < raycaster.Near || distance > raycaster.Far) continue;

                        intersects.Add(new Intersect()
                        {
                            Distance = distance,
                            Point = intersectionPoint,
                            Indices = new int[] { a, b, c },
                            Face = null,
                            FaceIndex = -1,
                            Object3D = this
                        });

                    }

                }

            }
            else
            {

                var positions = ((BufferAttribute<float>)attributes["position"]).Array;

                int j = 0;
                for (var i = 0; i < positions.Length; i += 3, j += 9)
                {

                    if (j + 8 > positions.Length)
                        break;

                    var a = i;
                    var b = i + 1;
                    var c = i + 2;

                    vA.Set(
                        positions[j],
                        positions[j + 1],
                        positions[j + 2]
                    );
                    vB.Set(
                        positions[j + 3],
                        positions[j + 4],
                        positions[j + 5]
                    );
                    vC.Set(
                        positions[j + 6],
                        positions[j + 7],
                        positions[j + 8]
                    );

                    Vector3 intersectionPoint;
                    if (material.Side == Three.BackSide)
                    {

                        intersectionPoint = ray.IntersectTriangle(vC, vB, vA, true);

                    }
                    else
                    {

                        intersectionPoint = ray.IntersectTriangle(vA, vB, vC, material.Side != Three.DoubleSide);

                    }

                    if (intersectionPoint == null) continue;

                    intersectionPoint.ApplyMatrix4(this.MatrixWorld);

                    var distance = raycaster.Ray.Origin.DistanceTo(intersectionPoint);

                    if (distance < precision || distance < raycaster.Near || distance > raycaster.Far) continue;

                    intersects.Add(new Intersect()
                    {
                        Distance = distance,
                        Point = intersectionPoint,
                        Indices = new int[] { a, b, c },
                        Face = null,
                        FaceIndex = -1,
                        Object3D = this
                    });
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raycaster"></param>
        /// <param name="intersects"></param>
        public override void Raycast(Raycaster raycaster, ref List<Intersect> intersects)
        {
		    var geometry = this.Geometry;

		    // Checking boundingSphere distance to ray

		    if (geometry.BoundingSphere == null)
                geometry.ComputeBoundingSphere();

            var sphere = geometry.BoundingSphere;
		    sphere.ApplyMatrix4( this.MatrixWorld );

		    if ( raycaster.Ray.IsIntersectionSphere(sphere) == false ) {
			    return;
		    }

            // Check boundingBox before continuing

            var inverseMatrix = this.MatrixWorld.GetInverse();

            var ray = new Ray();
            ray.Copy(raycaster.Ray).ApplyMatrix4(inverseMatrix);

            if (geometry.BoundingBox != null)
            {
                if ( ray.IsIntersectionBox( geometry.BoundingBox ) == false )  {
                    return;
                }
            }

            // We are within the boundingbox or sphere

            var bufferGeometry = geometry as BufferGeometry;
            if ( bufferGeometry != null ) 
            {
                this.Raycast(raycaster, ray, bufferGeometry, ref intersects);
            } 
            else if ( geometry is Geometry ) 
            {
                this.Raycast(raycaster, ray, geometry as Geometry, ref intersects);
            }

        }
    }
}

