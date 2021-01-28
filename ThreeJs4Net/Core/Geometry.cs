using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Core
{
    public class Geometry : BaseGeometry, ICloneable
    {
        protected static int GeometryIdCount;
        public Guid uuid = Guid.NewGuid();

        public List<Vector3> Vertices = new List<Vector3>();
        public List<Color> Colors = new List<Color>(); // one-to-one vertex colors, used in Points and Line
        public List<Face3> Faces = new List<Face3>();
        public List<List<List<Vector2>>> FaceVertexUvs = new List<List<List<Vector2>>>();

        public List<GeometryGroup> MorphTargets = new List<GeometryGroup>();
        public List<Vector3> MorphNormals = new List<Vector3>();

        public List<Vector4> SkinWeights = new List<Vector4>();
        public List<Vector4> SkinIndices = new List<Vector4>();
        public List<float> LineDistances = new List<float>();

        public bool HasTangents = false;
        public bool Dynamic = true; // the intermediate typed arrays will be deleted when Set to false
        public bool MorphTargetsNeedUpdate;

        // update flags
        public bool ElementsNeedUpdate = false;
        public bool VerticesNeedUpdate = false;
        public bool UvsNeedUpdate = false;
        public bool NormalsNeedUpdate = false;
        public bool ColorsNeedUpdate = false;
        public bool LineDistancesNeedUpdate = false;
        public bool GroupsNeedUpdate = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Geometry()
        {
            Id = GeometryIdCount += 2;
            this.type = "Geometry";
            this.FaceVertexUvs.Add(new List<List<Vector2>>());
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected Geometry(Geometry other)
        {

        }





        #region --- Already in R116 ---
        public Geometry RotateX(float angle)
        {
            // rotate geometry around world x-axis
            return this.ApplyMatrix4(new Matrix4().MakeRotationX(angle));
        }

        public Geometry RotateY(float angle)
        {
            // rotate geometry around world y-axis
            return this.ApplyMatrix4(new Matrix4().MakeRotationY(angle));
        }

        public Geometry RotateZ(float angle)
        {
            // rotate geometry around world z-axis
            return this.ApplyMatrix4(new Matrix4().MakeRotationZ(angle));
        }

        public Geometry LookAt(Vector3 vector)
        {
            var obj = new Object3D();
            obj.LookAt(vector);
            obj.UpdateMatrix();
            this.ApplyMatrix4(obj.Matrix);
            return this;
        }

        public Geometry Center()
        {
            this.ComputeBoundingBox();
            var offset = new Vector3();
            this.BoundingBox.GetCenter(offset).Negate();
            this.Translate(offset.X, offset.Y, offset.Z);

            return this;
        }

        public Geometry Normalize()
        {
            this.ComputeBoundingSphere();

            var center = this.BoundingSphere.Center;
            var radius = this.BoundingSphere.Radius;

            var s = radius == 0 ? 1 : (float)1.0 / radius;

            var matrix = new Matrix4();
            matrix.Set(
                s, 0, 0, -s * center.X,
                0, s, 0, -s * center.Y,
                0, 0, s, -s * center.Z,
                0, 0, 0, 1
            );

            this.ApplyMatrix4(matrix);

            return this;
        }

        public void ComputeFlatVertexNormals()
        {
            //var f, fl, face;

            this.ComputeFaceNormals();

            foreach (var face in this.Faces)
            {
                var vertexNormals = face.VertexNormals;

                if (vertexNormals.Count == 3)
                {
                    vertexNormals[0].Copy(face.Normal);
                    vertexNormals[1].Copy(face.Normal);
                    vertexNormals[2].Copy(face.Normal);
                }
                else
                {
                    vertexNormals[0] = face.Normal.Clone();
                    vertexNormals[1] = face.Normal.Clone();
                    vertexNormals[2] = face.Normal.Clone();
                }
            }

            if (this.Faces.Count > 0)
            {
                this.NormalsNeedUpdate = true;
            }
        }

        public override void ComputeBoundingBox()
        {
            if (this.BoundingBox == null)
            {
                this.BoundingBox = new Box3();
            }
            this.BoundingBox.SetFromPoints(this.Vertices);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ComputeBoundingSphere()
        {

            if (this.BoundingSphere == null)
            {
                this.BoundingSphere = new Sphere();
            }

            this.BoundingSphere.SetFromPoints(this.Vertices);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Merge(Geometry geometry, Matrix4 matrix = null, int materialIndexOffset = 0)
        {
            if (geometry == null)
            {
                Trace.TraceError("THREE.Geometry.merge(): geometry not an instance of THREE.Geometry.");
                return;
            }

            Matrix3 normalMatrix = null;
            var vertexOffset = this.Vertices.Count;
            var vertices1 = this.Vertices;
            var vertices2 = geometry.Vertices;
            var faces1 = this.Faces;
            var faces2 = geometry.Faces;
            var colors1 = this.Colors;
            var colors2 = geometry.Colors;

            if (matrix != null)
            {
                normalMatrix = new Matrix3().GetNormalMatrix(matrix);
            }

            foreach (var vertex in vertices2)
            {
                var vertexCopy = vertex.Clone();
                if (matrix != null)
                {
                    vertexCopy.ApplyMatrix4(matrix);
                }
                vertices1.Add(vertexCopy);
            }

            // vertices
            for (var i = 0; i < vertices2.Count; i++)
            {
                var vertex = vertices2[i];

                var vertexCopy = (Vector3)vertex.Clone();

                if (matrix != null) vertexCopy.ApplyMatrix4(matrix);

                vertices1.Add(vertexCopy);
            }

            // Colors
            foreach (var color2 in colors2)
            {
                colors1.Add(color2);
            }

            // faces
            foreach (var face in faces2)
            {

                //var normal, Color,
                //    faceVertexNormals = face.vertexNormals,
                //    faceVertexColors = face.vertexColors;

                var faceVertexNormals = face.VertexNormals;
                var faceVertexColors = face.VertexColors;

                var faceCopy = new Face3(face.A + vertexOffset, face.B + vertexOffset, face.C + vertexOffset);
                faceCopy.Normal.Copy(face.Normal);

                if (normalMatrix != null)
                {
                    faceCopy.Normal.ApplyMatrix3(normalMatrix).Normalize();
                }

                foreach (var faceVertexNormal in faceVertexNormals)
                {
                    var normal = faceVertexNormal.Clone();

                    if (normalMatrix != null)
                    {
                        normal.ApplyMatrix3(normalMatrix).Normalize();
                    }

                    faceCopy.VertexNormals.Add(normal);

                }

                faceCopy.Color = face.Color;

                for (var j = 0; j < faceVertexColors.Length; j++)
                {
                    faceCopy.VertexColors[j] = faceVertexColors[j];
                }

                faceCopy.MaterialIndex = face.MaterialIndex + materialIndexOffset;

                faces1.Add(faceCopy);
            }


            for (int i = 0; i < geometry.FaceVertexUvs.Count; i++)
            {
                var faceVertexUvs2 = geometry.FaceVertexUvs[i];

                if (this.FaceVertexUvs[i] == null)
                {
                    this.FaceVertexUvs[i] = new List<List<Vector2>>();
                }

                foreach (var uvs2 in faceVertexUvs2)
                {
                    var uvsCopy = new List<Vector2>();

                    foreach (var t in uvs2)
                    {
                        uvsCopy.Add(t.Clone());
                    }
                    this.FaceVertexUvs[i].Add(uvsCopy);
                }
            }
        }

        public void MergeMesh(Mesh mesh)
        {
            if (mesh.MatrixAutoUpdate)
            {
                mesh.UpdateMatrix();
            }
            this.Merge((Geometry)mesh.Geometry, mesh.Matrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int MergeVertices()
        {
            var verticesMap = new Dictionary<string, int>(); // Hashmap for looking up vertice by position coordinates (and making sure they are unique)
            var unique = new List<Vector3>();
            var changes = new List<int>();

            var precisionPoints = 4; // number of decimal points, eg. 4 for epsilon of 0.0001
            var precision = System.Math.Pow(10, precisionPoints);

            for (var i = 0; i < this.Vertices.Count; i++)
            {
                var v = this.Vertices[i];

                var key = System.Math.Round(v.X * precision) + "_" + System.Math.Round(v.Y * precision) + "_" + System.Math.Round(v.Z * precision);

                if (!verticesMap.TryGetValue(key, out var value))
                {
                    verticesMap[key] = i;
                    unique.Add(v);
                    changes.Add(unique.Count - 1);
                }
                else
                {
                    //console.log('Duplicate vertex found. ', i, ' could be using ', verticesMap[key]);
                    var idx = verticesMap[key];
                    changes.Add(changes[idx]);
                }
            }

            // if faces are completely degenerate after merging vertices, we
            // have to remove them from the geometry.

            var faceIndicesToRemove = new List<int>();

            for (var i = 0; i < this.Faces.Count; i++)
            {
                var face = this.Faces[i];

                face.A = changes[face.A];
                face.B = changes[face.B];
                face.C = changes[face.C];

                var indices = new[] { face.A, face.B, face.C };

                // if any duplicate vertices are found in A Face3
                // we have to remove the face as nothing can be saved
                for (var n = 0; n < 3; n++)
                {
                    if (indices[n] == indices[(n + 1) % 3])
                    {
                        faceIndicesToRemove.Add(i);
                        break;
                    }
                }
            }

            foreach (var idx in faceIndicesToRemove)
            {
                this.Faces.RemoveAt(idx);

                for (var j = 0; j < this.FaceVertexUvs.Count; j++)
                {
                    this.FaceVertexUvs[j].RemoveAt(idx);
                }
            }

            // Use unique Set of vertices
            var diff = this.Vertices.Count - unique.Count;
            this.Vertices = unique;
            return diff;
        }

        public Geometry SetFromPoints(Vector2[] points)
        {
            this.Vertices = new List<Vector3>();

            foreach (var point in points)
            {
                this.Vertices.Add(new Vector3(point.X, point.Y, 0));
            }

            return this;
        }

        public Geometry SetFromPoints(Vector3[] points)
        {
            this.Vertices = new List<Vector3>();

            foreach (var point in points)
            {
                this.Vertices.Add(point.Clone());
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaWeighted"></param>
        public new void ComputeVertexNormals(bool areaWeighted = true)
        {
            var vertices = new Vector3[this.Vertices.Count];

            for (int v = 0; v < vertices.Length; v++)
            {
                vertices[v] = new Vector3();
            }

            if (areaWeighted)
            {
                // vertex normals weighted by triangle areas
                // http://www.iquilezles.org/www/articles/normals/normals.htm

                var cb = new Vector3();
                var ab = new Vector3();

                foreach (var face in this.Faces)
                {
                    var vA = this.Vertices[face.A];
                    var vB = this.Vertices[face.B];
                    var vC = this.Vertices[face.C];

                    cb.SubVectors(vC, vB);
                    ab.SubVectors(vA, vB);
                    cb.Cross(ab);

                    vertices[face.A].Add(cb);
                    vertices[face.B].Add(cb);
                    vertices[face.C].Add(cb);
                }
            }
            else
            {
                this.ComputeFaceNormals();

                foreach (var face in this.Faces)
                {
                    vertices[face.A].Add(face.Normal);
                    vertices[face.B].Add(face.Normal);
                    vertices[face.C].Add(face.Normal);

                }
            }

            for (int v = 0; v < this.Vertices.Count; v++)
            {
                vertices[v].Normalize();
            }

            foreach (var face in this.Faces)
            {
                var vertexNormals = face.VertexNormals;

                if (vertexNormals.Count == 3)
                {
                    vertexNormals[0].Copy(vertices[face.A]);
                    vertexNormals[1].Copy(vertices[face.B]);
                    vertexNormals[2].Copy(vertices[face.C]);
                }
                else
                {
                    vertexNormals.Add(vertices[face.A].Clone());
                    vertexNormals.Add(vertices[face.B].Clone());
                    vertexNormals.Add(vertices[face.C].Clone());
                }
            }


            if (this.Faces.Count > 0)
            {
                this.NormalsNeedUpdate = true;
            }
        }

        public Geometry Scale(float x, float y, float z)
        {
            return this.ApplyMatrix4(new Matrix4().MakeScale(x, y, z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public new Geometry ApplyMatrix4(Matrix4 matrix)
        {
            var normalMatrix = new Matrix3().GetNormalMatrix(matrix);

            foreach (var vertex in this.Vertices)
            {
                vertex.ApplyMatrix4(matrix);
            }

            foreach (var face in this.Faces)
            {
                face.Normal.ApplyMatrix3(normalMatrix).Normalize();

                foreach (Vector3 vertexNormal in face.VertexNormals)
                {
                    vertexNormal.ApplyMatrix3(normalMatrix).Normalize();
                }
            }

            if (this.BoundingBox != null)
            {
                this.ComputeBoundingBox();
            }

            if (this.BoundingSphere != null)
            {
                this.ComputeBoundingSphere();
            }

            this.VerticesNeedUpdate = true;
            this.NormalsNeedUpdate = true;

            return this;
        }

        public Geometry Translate(float x, float y, float z)
        {
            // translate geometry
            var m1 = new Matrix4().MakeTranslation(x, y, z);
            this.ApplyMatrix4(m1);

            return this;
        }














        #endregion


        public void ComputeFaceNormals()
        {
            var cb = new Vector3(); var ab = new Vector3();

            foreach (var face in this.Faces)
            {
                var vA = this.Vertices[face.A];
                var vB = this.Vertices[face.B];
                var vC = this.Vertices[face.C];

                cb.SubVectors(vC, vB);
                ab.SubVectors(vA, vB);
                cb.Cross(ab);

                cb.Normalize();

                face.Normal.Copy(cb);
            }
        }

        public void ComputeMorphNormals()
        {
            throw new NotImplementedException();
        }

        private struct Hash
        {
            public int hash;
            public int counter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new Geometry().Clone();
        }

        //public Geometry Copy(Geometry source)
        //{
        //    var i, il, j, jl, k, kl;

        //    // reset

        //    this.Vertices.Clear();
        //    this.Colors.Clear();
        //    this.Faces.Clear();
        //    this.FaceVertexUvs.Clear();
        //    this.MorphTargets.Clear();
        //    this.MorphNormals.Clear();
        //    this.SkinWeights.Clear();
        //    this.SkinIndices.Clear();
        //    this.LineDistances.Clear();
        //    this.BoundingBox = null;
        //    this.BoundingSphere = null;

        //    this.Name = source.Name;

        //    var vertices = source.Vertices;

        //    foreach (var vertice in vertices)
        //    {
        //        this.Vertices.Add(vertice.Clone());
        //    }

        //    this.Colors.AddRange(source.Colors);


        //    var faces = source.Faces;
        //    foreach (var face in faces)
        //    {
        //        this.Faces.Add(face.Clone());
        //    }

        //    // face vertex uvs
        //    for (i = 0, il = source.FaceVertexUvs.Count; i < il; i++)
        //    {
        //        var faceVertexUvs = source.FaceVertexUvs[i];

        //        if (this.FaceVertexUvs[i] == null)
        //        {
        //            this.FaceVertexUvs[i] = new List<List<Vector2>>();
        //        }
        //        for (j = 0, jl = faceVertexUvs.Count; j < jl; j++)
        //        {
        //            var uvs = faceVertexUvs[j];
        //            var uvsCopy = new List<Vector2>();

        //            foreach (var uv in uvs)
        //            {
        //                uvsCopy.Add(uv.Clone());
        //            }
        //            this.FaceVertexUvs[i].Add(uvsCopy);
        //        }
        //    }

        //    // morph targets
        //    var morphTargets = source.MorphTargets;

        //    for (i = 0, il = morphTargets.Count; i < il; i++)
        //    {
        //        var morphTarget = new GeometryGroup();
        //        morphTarget.Name = morphTargets[i].Name;

        //        // vertices
        //        if (morphTargets[i].Vertices != null)
        //        {
        //            morphTarget.Vertices = new int();

        //            for (j = 0, jl = morphTargets[i].Vertices.length; j < jl; j++)
        //            {
        //                morphTarget.vertices.push(morphTargets[i].vertices[j].clone());
        //            }
        //        }

        //        // normals

        //        if (morphTargets[i].normals !== undefined)
        //        {

        //            morphTarget.normals = [];

        //            for (j = 0, jl = morphTargets[i].normals.length; j < jl; j++)
        //            {

        //                morphTarget.normals.push(morphTargets[i].normals[j].clone());

        //            }

        //        }

        //        this.morphTargets.push(morphTarget);

        //    }

        //    // morph normals

        //    var morphNormals = source.morphNormals;

        //    for (i = 0, il = morphNormals.length; i < il; i++)
        //    {

        //        var morphNormal = { };

        //        // vertex normals

        //        if (morphNormals[i].vertexNormals !== undefined)
        //        {

        //            morphNormal.vertexNormals = [];

        //            for (j = 0, jl = morphNormals[i].vertexNormals.length; j < jl; j++)
        //            {

        //                var srcVertexNormal = morphNormals[i].vertexNormals[j];
        //                var destVertexNormal = { };

        //                destVertexNormal.A = srcVertexNormal.A.clone();
        //                destVertexNormal.B = srcVertexNormal.B.clone();
        //                destVertexNormal.C = srcVertexNormal.C.clone();

        //                morphNormal.vertexNormals.push(destVertexNormal);

        //            }

        //        }

        //        // face normals

        //        if (morphNormals[i].faceNormals !== undefined)
        //        {

        //            morphNormal.faceNormals = [];

        //            for (j = 0, jl = morphNormals[i].faceNormals.length; j < jl; j++)
        //            {

        //                morphNormal.faceNormals.push(morphNormals[i].faceNormals[j].clone());

        //            }

        //        }

        //        this.morphNormals.push(morphNormal);

        //    }

        //    // skin weights

        //    var skinWeights = source.skinWeights;

        //    for (i = 0, il = skinWeights.length; i < il; i++)
        //    {

        //        this.skinWeights.push(skinWeights[i].clone());

        //    }

        //    // skin indices

        //    var skinIndices = source.skinIndices;

        //    for (i = 0, il = skinIndices.length; i < il; i++)
        //    {

        //        this.skinIndices.push(skinIndices[i].clone());

        //    }

        //    // line distances

        //    var lineDistances = source.lineDistances;

        //    for (i = 0, il = lineDistances.length; i < il; i++)
        //    {

        //        this.lineDistances.push(lineDistances[i]);

        //    }

        //    // bounding box

        //    var boundingBox = source.boundingBox;

        //    if (boundingBox !== null)
        //    {

        //        this.boundingBox = boundingBox.clone();

        //    }

        //    // bounding sphere

        //    var boundingSphere = source.boundingSphere;

        //    if (boundingSphere !== null)
        //    {

        //        this.boundingSphere = boundingSphere.clone();

        //    }

        //    // update flags

        //    this.elementsNeedUpdate = source.elementsNeedUpdate;
        //    this.verticesNeedUpdate = source.verticesNeedUpdate;
        //    this.uvsNeedUpdate = source.uvsNeedUpdate;
        //    this.normalsNeedUpdate = source.normalsNeedUpdate;
        //    this.colorsNeedUpdate = source.colorsNeedUpdate;
        //    this.lineDistancesNeedUpdate = source.lineDistancesNeedUpdate;
        //    this.groupsNeedUpdate = source.groupsNeedUpdate;

        //    return this;

        //},
    }
}
