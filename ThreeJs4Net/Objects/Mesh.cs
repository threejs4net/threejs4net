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
        public Vector2 Uv { get; set; }
        public Vector2 Uv2 { get; set; }
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
            if (this.Material == null)
            {
                return;
            }

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

                var a = vertices[face.A];
                var b = vertices[face.B];
                var c = vertices[face.C];
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

                                vA.X += ( targets[ face.A ].x - A.X ) * influence;
                                vA.Y += ( targets[ face.A ].y - A.Y ) * influence;
                                vA.Z += ( targets[ face.A ].z - A.Z ) * influence;

                                vB.X += ( targets[ face.B ].x - B.X ) * influence;
                                vB.Y += ( targets[ face.B ].y - B.Y ) * influence;
                                vB.Z += ( targets[ face.B ].z - B.Z ) * influence;

                                vC.X += ( targets[ face.C ].x - C.X ) * influence;
                                vC.Y += ( targets[ face.C ].y - C.Y ) * influence;
                                vC.Z += ( targets[ face.C ].z - C.Z ) * influence;

                            }

                            vA.Add( A );
                            vB.Add( B );
                            vC.Add( C );

                            A = vA;
                            B = vB;
                            C = vC;

                        }
*/
                Vector3 intersectionPoint = new Vector3();
                if (material.Side == Three.BackSide)
                {
                    intersectionPoint = ray.IntersectTriangle(c, b, a, true, intersectionPoint);
                }
                else
                {
                    intersectionPoint = ray.IntersectTriangle(a, b, c, material.Side != Three.DoubleSide, intersectionPoint);
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

                        Vector3 intersectionPoint = new Vector3();
                        if (material.Side == Three.BackSide)
                        {
                            intersectionPoint = ray.IntersectTriangle(vC, vB, vA, true, intersectionPoint);
                        }
                        else
                        {
                            intersectionPoint = ray.IntersectTriangle(vA, vB, vC, material.Side != Three.DoubleSide, intersectionPoint);
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

                    Vector3 intersectionPoint = new Vector3();
                    if (material.Side == Three.BackSide)
                    {
                        intersectionPoint = ray.IntersectTriangle(vC, vB, vA, true, intersectionPoint);
                    }
                    else
                    {
                        intersectionPoint = ray.IntersectTriangle(vA, vB, vC, material.Side != Three.DoubleSide, intersectionPoint);
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
        public override void Raycast(Raycaster raycaster, List<Intersect> intersects)
        {
            if (this.Material == null)
            {
                return;
            }

            var geometry = this.Geometry;

            // Checking boundingSphere distance to ray
            if (geometry.BoundingSphere == null)
            {
                geometry.ComputeBoundingSphere();
            }

            var sphere = new Sphere().Copy(geometry.BoundingSphere);
            sphere.ApplyMatrix4(MatrixWorld);

            if (!raycaster.Ray.IntersectsSphere(sphere))
            {
                return;
            }

            var inverseMatrix = new Matrix4().GetInverse(MatrixWorld);
            var ray = new Ray().Copy(raycaster.Ray).ApplyMatrix4(inverseMatrix);

            // Check boundingBox before continuing
            if (geometry.BoundingBox != null)
            {
                if (!ray.IntersectsBox(geometry.BoundingBox))
                {
                    return;
                }
            }


            //var intersection;

            if (geometry is BufferGeometry bufGeometry)
            {
                //var a, b, c;
                var index = bufGeometry.Index;
                var position = bufGeometry.GetAttribute<float>("position");
                //!!var morphPosition = bufGeometry.morphAttributes.position;
                //!!var morphTargetsRelative = bufGeometry.morphTargetsRelative;
                var uv = bufGeometry.GetAttribute<float>("uv");
                var uv2 = bufGeometry.GetAttribute<float>("uv2");
                //!!var groups = bufGeometry.groups;
                var drawRange = bufGeometry.DrawRange;
                //var i, j, il, jl;
                //var group, groupMaterial;
                int start, end;

                if (index != null)
                {
                    // indexed buffer geometry
                    if (this.Materials != null && this.Materials.Count > 0)
                    {
                        //!! TODO
                        //for ( i = 0, il = groups.length; i < il; i ++ ) {
                        //	group = groups[ i ];
                        //	groupMaterial = material[ group.materialIndex ];

                        //	start = Math.max( group.start, drawRange.start );
                        //	end = Math.min( ( group.start + group.count ), ( drawRange.start + drawRange.count ) );

                        //	for ( j = start, jl = end; j < jl; j += 3 ) {

                        //		a = index.getX( j );
                        //		b = index.getX( j + 1 );
                        //		c = index.getX( j + 2 );

                        //		intersection = checkBufferGeometryIntersection( this, groupMaterial, raycaster, _ray, position, morphPosition, morphTargetsRelative, uv, uv2, a, b, c );

                        //		if ( intersection ) {

                        //			intersection.faceIndex = Math.floor( j / 3 ); // triangle number in indexed buffer semantics
                        //			intersection.face.materialIndex = group.materialIndex;
                        //			intersects.push( intersection );

                        //		}

                        //	}

                        //}
                    }
                    else
                    {
                        start = Mathf.Max(0, drawRange.Start);
                        end = Mathf.Min(index.Count, (drawRange.Start + drawRange.Count));

                        for (int i = start; i < end; i+=3)
                        {
                            var a = (int)index.GetX(i);
                            var b = (int)index.GetX(i + 1);
                            var c = (int)index.GetX(i + 2);

                            var intersection = CheckBufferGeometryIntersection(this, this.Material, raycaster, ray, position, 
                                null /*morphPosition*/, null /*morphTargetsRelative*/, uv, uv2, a, b, c);

                            if (intersection != null)
                            {
                                intersection.FaceIndex = Mathf.Floor(i / 3); // triangle number in indexed buffer semantics
                                intersects.Add(intersection);
                            }
                        }
                    }
                }
                else if (position != null)
                {
                    // non-indexed buffer geometry
                    if (this.Materials != null && this.Materials.Count > 0)
                    {
                        //for (i = 0, il = groups.length; i < il; i++)
                        //{

                        //    group = groups[i];
                        //    groupMaterial = material[group.materialIndex];

                        //    start = Math.max(group.start, drawRange.start);
                        //    end = Math.min((group.start + group.count), (drawRange.start + drawRange.count));

                        //    for (j = start, jl = end; j < jl; j += 3)
                        //    {

                        //        a = j;
                        //        b = j + 1;
                        //        c = j + 2;

                        //        intersection = checkBufferGeometryIntersection(this, groupMaterial, raycaster, _ray, position, morphPosition, morphTargetsRelative, uv, uv2, a, b, c);

                        //        if (intersection)
                        //        {

                        //            intersection.faceIndex = Math.floor(j / 3); // triangle number in non-indexed buffer semantics
                        //            intersection.face.materialIndex = group.materialIndex;
                        //            intersects.push(intersection);

                        //        }

                        //    }

                        //}
                    }
                    else
                    {
                        start = Mathf.Max(0, drawRange.Start);
                        end = Mathf.Min(position.Count, (drawRange.Start + drawRange.Count));

                        for (var i = start; i < end; i += 3)
                        {
                            var a = i;
                            var b = i + 1;
                            var c = i + 2;
                            var intersection = CheckBufferGeometryIntersection(this, this.Material, raycaster, ray, position, 
                                null /*morphPosition*/, null /*morphTargetsRelative*/, uv, uv2, a, b, c);

                            if (intersection != null)
                            {
                                intersection.FaceIndex = Mathf.Floor(i / 3); // triangle number in non-indexed buffer semantics
                                intersects.Add(intersection);
                            }
                        }
                    }
                }
            }
            else if (geometry is Geometry geoModel)
            {

                //var fvA, fvB, fvC;
                //var isMultiMaterial = this.Materials != null & this.Materials.Count > 0;

                //var vertices = geoModel.Vertices;
                //var faces = geoModel.Faces;
                //var uvs;

                //var faceVertexUvs = geometry.faceVertexUvs[0];
                //if (faceVertexUvs.length > 0) uvs = faceVertexUvs;

                //for (var f = 0, fl = faces.length; f < fl; f++)
                //{

                //    var face = faces[f];
                //    var faceMaterial = isMultiMaterial ? material[face.materialIndex] : material;

                //    if (faceMaterial === undefined) continue;

                //    fvA = vertices[face.a];
                //    fvB = vertices[face.b];
                //    fvC = vertices[face.c];

                //    intersection = checkIntersection(this, faceMaterial, raycaster, _ray, fvA, fvB, fvC, _intersectionPoint);

                //    if (intersection)
                //    {

                //        if (uvs && uvs[f])
                //        {

                //            var uvs_f = uvs[f];
                //            _uvA.copy(uvs_f[0]);
                //            _uvB.copy(uvs_f[1]);
                //            _uvC.copy(uvs_f[2]);

                //            intersection.uv = Triangle.getUV(_intersectionPoint, fvA, fvB, fvC, _uvA, _uvB, _uvC, new Vector2());

                //        }

                //        intersection.face = face;
                //        intersection.faceIndex = f;
                //        intersects.push(intersection);

                //    }

                //}

            }






















            // Checking boundingSphere distance to ray
            if (geometry.BoundingSphere == null)
            {
                geometry.ComputeBoundingSphere();
            }

            sphere = geometry.BoundingSphere;
            sphere.ApplyMatrix4(this.MatrixWorld);

            if (!raycaster.Ray.IntersectsSphere(sphere))
            {
                return;
            }

            // Check boundingBox before continuing
            inverseMatrix = this.MatrixWorld.GetInverse();

            ray = new Ray();
            ray.Copy(raycaster.Ray).ApplyMatrix4(inverseMatrix);

            if (geometry.BoundingBox != null)
            {
                if (!ray.IntersectsBox(geometry.BoundingBox))
                {
                    return;
                }
            }

            // We are within the boundingbox or sphere

            var bufferGeometry = geometry as BufferGeometry;
            if (bufferGeometry != null)
            {
                this.Raycast(raycaster, ray, bufferGeometry, ref intersects);
            }
            else if (geometry is Geometry)
            {
                this.Raycast(raycaster, ray, geometry as Geometry, ref intersects);
            }

        }

        private Intersect CheckIntersection(Object3D object3d, Material material, Raycaster raycaster, Ray ray, Vector3 pA, Vector3 pB, Vector3 pC, Vector3 point)
        {
            Vector3 intersect = null;

            intersect = material.Side == Three.BackSide
                ? ray.IntersectTriangle(pC, pB, pA, true, point)
                : ray.IntersectTriangle(pA, pB, pC, material.Side != Three.DoubleSide, point);

            if (intersect == null)
            {
                return null;
            }

            var intersectionPointWorld = new Vector3().Copy(point);
            intersectionPointWorld.ApplyMatrix4(object3d.MatrixWorld);

            var distance = raycaster.Ray.Origin.DistanceTo(intersectionPointWorld);

            if (distance < raycaster.Near || distance > raycaster.Far)
            {
                return null;
            }

            return new Intersect()
            {
                Distance = distance,
                Point = intersectionPointWorld.Clone(),
                Object3D = object3d
            };
        }


        private Intersect CheckBufferGeometryIntersection(Object3D object3D, Material material, Raycaster raycaster, Ray ray, BufferAttribute<float> position, Vector3 morphPosition,
            object morphTargetsRelative, BufferAttribute<float> uv, BufferAttribute<float> uv2, int a, int b, int c)
        {
            var vA = new Vector3().FromBufferAttribute(position, a);
            var vB = new Vector3().FromBufferAttribute(position, b);
            var vC = new Vector3().FromBufferAttribute(position, c);

            //!!	var morphInfluences = object.morphTargetInfluences;

            //if ( material.morphTargets && morphPosition && morphInfluences ) {

            //	_morphA.set( 0, 0, 0 );
            //	_morphB.set( 0, 0, 0 );
            //	_morphC.set( 0, 0, 0 );

            //	for ( var i = 0, il = morphPosition.length; i < il; i ++ ) {

            //		var influence = morphInfluences[ i ];
            //		var morphAttribute = morphPosition[ i ];

            //		if ( influence === 0 ) continue;

            //		_tempA.fromBufferAttribute( morphAttribute, a );
            //		_tempB.fromBufferAttribute( morphAttribute, b );
            //		_tempC.fromBufferAttribute( morphAttribute, c );

            //		if ( morphTargetsRelative ) {

            //			_morphA.addScaledVector( _tempA, influence );
            //			_morphB.addScaledVector( _tempB, influence );
            //			_morphC.addScaledVector( _tempC, influence );

            //		} else {

            //			_morphA.addScaledVector( _tempA.sub( _vA ), influence );
            //			_morphB.addScaledVector( _tempB.sub( _vB ), influence );
            //			_morphC.addScaledVector( _tempC.sub( _vC ), influence );

            //		}

            //	}

            //	_vA.add( _morphA );
            //	_vB.add( _morphB );
            //	_vC.add( _morphC );

            //}

            //if (object.isSkinnedMesh)
            //{
            //    object.boneTransform(a, _vA);
            //    object.boneTransform(b, _vB);
            //    object.boneTransform(c, _vC);
            //}

            Vector3 intersectionPoint = new Vector3();
            var intersection = CheckIntersection(object3D, material, raycaster, ray, vA, vB, vC, intersectionPoint);

            if (intersection != null)
            {
                if (uv != null)
                {
                    var uvA = new Vector2().FromBufferAttribute(uv, a);
                    var uvB = new Vector2().FromBufferAttribute(uv, b);
                    var uvC = new Vector2().FromBufferAttribute(uv, c);
                    intersection.Uv = Triangle.GetUV(intersectionPoint, vA, vB, vC, uvA, uvB, uvC, new Vector2());
                }

                if (uv2 != null)
                {
                    var uvA = new Vector2().FromBufferAttribute(uv2, a);
                    var uvB = new Vector2().FromBufferAttribute(uv2, b);
                    var uvC = new Vector2().FromBufferAttribute(uv2, c);

                    intersection.Uv2 = Triangle.GetUV(intersectionPoint, vA, vB, vC, uvA, uvB, uvC, new Vector2());
                }

                var face = new Face3(a, b, c);
                Triangle.GetNormal(vA, vB, vC, face.Normal);

                intersection.Face = face;
            }

            return intersection;
        }
    }
}

