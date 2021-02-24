﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Renderers.Shaders;
using Attribute = ThreeJs4Net.Renderers.Shaders.Attribute;

namespace ThreeJs4Net.Core
{
    public struct Offset
    {
        public int Start;
        public int Index;
        public int Count;
    }

    public class BufferGeometry : BaseGeometry, IAttributes // Note: in three.js, BufferGeometry is not Geometry
    {
        #region --- Fields ---
        public bool UcVisible = true;
        public BufferAttribute<uint> Index;
        public Attributes Attributes { get; set; }
        public DrawRange DrawRange = new DrawRange();
        #endregion

        protected static int BufferGeometryIdCount;

        public List<BufferGeometryGroups> groups = new List<BufferGeometryGroups>();
        public List<string> AttributesKeys { get; set; }
        public IList<Offset> Drawcalls = new List<Offset>();
        public IList<Offset> Offsets; // backwards compatibility

        public BufferGeometry()
        {
            Id = BufferGeometryIdCount++;

            this.type = "BufferGeometry";

            // IAttributes
            this.Attributes = new Attributes();

            this.Offsets = this.Drawcalls;
        }

        public BufferAttribute<uint> GetIndex()
        {
            return this.Index;
        }

        public void SetIndex(BufferAttribute<uint> index)
        {
            this.Index = index;
        }






        #region --- Already in R116 ---

        [Obsolete("Removed on newest release")]
        public void AddAttribute(string name, Attribute attribute)
        {
            if (attribute is IBufferAttribute == false)
            {
                Trace.TraceWarning("BufferGeometry: .addAttribute() now expects ( name, attribute ).");
            }

            this.Attributes[name] = attribute;

            // Object.keys
            this.AttributesKeys = new List<string>();
            foreach (var entry in this.Attributes)
            {
                this.AttributesKeys.Add(entry.Key);
            }
        }

        public BufferGeometry SetAttribute(string name, Attribute attribute)
        {
            this.Attributes[name] = attribute;

            // Object.keys
            this.AttributesKeys = new List<string>();
            foreach (var entry in this.Attributes)
            {
                this.AttributesKeys.Add(entry.Key);
            }

            return this;
        }

        public object GetAttribute(string name)
        {
            return this.Attributes[name];
        }

        public BufferAttribute<T> GetAttribute<T>(string name)
        {
            if (this.Attributes.ContainsKey(name))
            {
                return this.Attributes[name] as BufferAttribute<T>;
            }

            return null;
        }

        public BufferGeometry DeleteAttribute(string name)
        {
            this.Attributes.Remove(name);
            return this;
        }

        public BufferGeometry Scale(float x, float y, float z)
        {
            this.ApplyMatrix4(new Matrix4().MakeScale(x, y, z));
            return this;
        }

        public BufferGeometry RotateX(float angle)
        {
            this.ApplyMatrix4(new Matrix4().MakeRotationX(angle));
            return this;
        }

        public BufferGeometry RotateY(float angle)
        {
            this.ApplyMatrix4(new Matrix4().MakeRotationY(angle));
            return this;

        }

        public BufferGeometry RotateZ(float angle)
        {
            this.ApplyMatrix4(new Matrix4().MakeRotationZ(angle));
            return this;
        }

        public BufferGeometry Translate(float x, float y, float z)
        {
            this.ApplyMatrix4(new Matrix4().MakeTranslation(x, y, z));
            return this;
        }

        public BufferGeometry Center()
        {
            var offset = new Vector3();
            this.ComputeBoundingBox();
            this.BoundingBox.GetCenter(offset).Negate();
            this.Translate(offset.X, offset.Y, offset.Z);
            return this;
        }

        #endregion




        /// <summary>
        /// 
        /// </summary>
        public override void ComputeBoundingSphere()
        {
            var box = new Box3();
            var vector = new Vector3();

            this.BoundingSphere ??= new Sphere();

            var position = this.Attributes["position"] as BufferAttribute<float>;
            var positions = GetBuffer<float>("position");

            if (positions != null)
            {
                var center = this.BoundingSphere.Center;
                box.SetFromBufferAttribute(position);

                //!! IMPLEMENT MORPH

                box.GetCenter(center);

                float maxRadiusSq = 0;

                for (var i = 0; i < positions.Length / position.ItemSize; i++)
                {
                    vector.FromBufferAttribute(position, i);
                    maxRadiusSq = Mathf.Max(maxRadiusSq, center.DistanceToSquared(vector));
                }

                //!! IMPLEMENT MORPH

                this.BoundingSphere.Radius = Mathf.Sqrt(maxRadiusSq);

                //if ()
                //{
                //    Trace.TraceError( "BufferGeometry.computeBoundingSphere(): Computed radius is NaN. The 'position' attribute is likely to have NaN values." );            
                //}
            }
        }

        public override void ComputeBoundingBox()
        {
            this.BoundingBox ??= new Box3();

            var position = this.Attributes["position"] as BufferAttribute<float>;
            //!! NOT IMPLEMENTED YET:   var morphAttributesPosition = this.Attributes["position"] as BufferAttribute<float>;
            Debug.Assert(position != null);

            var positions = GetBuffer<float>("position");


            if (positions != null)
            {
                this.BoundingBox.SetFromBufferAttribute(position);

                //!! IMPLEMENT MORPH
            }
            else
            {
                this.BoundingBox.MakeEmpty();
            }


            //if ( isNaN( this.BoundingBox.Min.X ) || isNaN( this.BoundingBox.Min.Y ) || isNaN( this.BoundingBox.Min.Z ) ) {
            //    Trace.TraceError( "THREE.BufferGeometry.computeBoundingBox: Computed min/max have NaN values. The 'position' attribute is likely to have NaN values. ););
            //}
        }

        public void ComputeFaceNormals()
        {
            // backwards compatibility
        }


        private T[] GetBuffer<T>(string name)
        {
            var bufferAttr = this.Attributes[name] as BufferAttribute<float>;
            return bufferAttr?.Array as T[];
        }

        public override void ComputeVertexNormals(bool areaWeighted = false)
        {
            var index = this.Index;
            var attributes = this.Attributes;
            var position = this.Attributes["position"] as BufferAttribute<float>;
            var positions = GetBuffer<float>("position");

            if (positions != null)
            {
                if (!attributes.ContainsKey("normal"))
                {
                    this.SetAttribute("normal", new BufferAttribute<float>(new float[positions.Length], 3));
                }
                else
                {
                    // reset existing normals to zero
                    var array = GetBuffer<float>("normal");

                    for (var i = 0; i < array.Length; i++)
                    {
                        array[i] = 0;
                    }
                }

                var pA = new Vector3();
                var pB = new Vector3();
                var pC = new Vector3();

                var cb = new Vector3();
                var ab = new Vector3();

                var normals = GetBuffer<float>("normal");

                // indexed elements

                if (index != null && index.length > 0)
                {
                    var indices = index.Array;

                    for (var i = 0; i < index.Count; i += 3)
                    {
                        var vA = (int)(indices[i + 0]) * 3;
                        var vB = (int)(indices[i + 1]) * 3;
                        var vC = (int)(indices[i + 2]) * 3;

                        pA.FromArray(positions, vA);
                        pB.FromArray(positions, vB);
                        pC.FromArray(positions, vC);

                        cb.SubVectors(pC, pB);
                        ab.SubVectors(pA, pB);
                        cb.Cross(ab);

                        normals[vA + 0] += cb.X;
                        normals[vA + 1] += cb.Y;
                        normals[vA + 2] += cb.Z;

                        normals[vB + 0] += cb.X;
                        normals[vB + 1] += cb.Y;
                        normals[vB + 2] += cb.Z;

                        normals[vC + 0] += cb.X;
                        normals[vC + 1] += cb.Y;
                        normals[vC + 2] += cb.Z;
                    }
                }
                else
                {
                    // non-indexed elements (unconnected triangle soup)

                    for (var i = 0; i < positions.Length; i += 9)
                    {
                        pA.FromArray(positions, i + 0);
                        pB.FromArray(positions, i + 3);
                        pC.FromArray(positions, i + 6);

                        cb.SubVectors(pC, pB);
                        ab.SubVectors(pA, pB);
                        cb.Cross(ab);

                        normals[i + 0] = cb.X;
                        normals[i + 1] = cb.Y;
                        normals[i + 2] = cb.Z;

                        normals[i + 3] = cb.X;
                        normals[i + 4] = cb.Y;
                        normals[i + 5] = cb.Z;

                        normals[i + 6] = cb.X;
                        normals[i + 7] = cb.Y;
                        normals[i + 8] = cb.Z;
                    }
                }

                this.NormalizeNormals();

                ((BufferAttribute<float>)Attributes["normal"]).needsUpdate = true;
            }
        }

        public void Merge()
        {
            throw new NotImplementedException();
        }

        public void NormalizeNormals()
        {
            var normalBufferAttribute = this.Attributes["normal"] as BufferAttribute<float>;
            Debug.Assert(null != normalBufferAttribute);

            var normals = normalBufferAttribute.Array;

            for (var i = 0; i < normals.Length; i += 3)
            {
                var x = normals[i];
                var y = normals[i + 1];
                var z = normals[i + 2];

                var n = 1.0f / (float)System.Math.Sqrt(x * x + y * y + z * z);

                normals[i] *= n;
                normals[i + 1] *= n;
                normals[i + 2] *= n;
            }
        }

        public BufferGeometry Clone()
        {
            return new BufferGeometry().Copy(this);
        }

        public BufferGeometry Copy(BufferGeometry source)
        {
            //var name, i, l;

            // reset

            this.Index = null;
            this.Attributes.Clear();
            //this.morphAttributes = { };
            //this.groups = [];
            this.BoundingBox = null;
            this.BoundingSphere = null;

            // name
            this.Name = source.Name;

            // index

            var index = source.Index;

            if (index != null)
            {
                this.SetIndex(index.Clone());
            }

            // attributes
            var attributes = source.Attributes;

            foreach (var attr in attributes)
            {
                attr.Value.TryGetValue("type", out object bufType);

                if (bufType == typeof(float))
                {
                    this.SetAttribute(attr.Key, (attr.Value as BufferAttribute<float>).Clone());
                }
                if (bufType == typeof(int))
                {
                    this.SetAttribute(attr.Key, (attr.Value as BufferAttribute<int>).Clone());
                }
                if (bufType == typeof(uint))
                {
                    this.SetAttribute(attr.Key, (attr.Value as BufferAttribute<uint>).Clone());
                }
            }

            //TODO: review morph attributes
            //var morphAttributes = source.morphAttributes;
            //for (name in morphAttributes )
            //{
            //    var array = [];
            //    var morphAttribute = morphAttributes[name]; // morphAttribute: array of Float32BufferAttributes
            //    for (i = 0, l = morphAttribute.length; i < l; i++)
            //    {
            //        array.push(morphAttribute[i].clone());
            //    }
            //    this.morphAttributes[name] = array;
            //}
            //this.morphTargetsRelative = source.morphTargetsRelative;

            // groups
            //var groups = source.groups;
            //for (i = 0, l = groups.length; i < l; i++)
            //{
            //    var group = groups[i];
            //    this.addGroup(group.start, group.count, group.materialIndex);
            //}




            // bounding box
            var boundingBox = source.BoundingBox;

            if (boundingBox != null)
            {
                this.BoundingBox = boundingBox.Clone();
            }



            // bounding sphere
            var boundingSphere = source.BoundingSphere;
            if (boundingSphere != null)
            {
                this.BoundingSphere = boundingSphere.Clone();
            }



            // draw range
            this.DrawRange.Start = source.DrawRange.Start;
            this.DrawRange.Count = source.DrawRange.Count;

            // user data
            //this.userData = source.userData;

            return this;

        }

        public void AddDrawCall(int start, int count, int indexOffset)
        {
            //   this.Drawcalls.Add() { start = start, count = count, index = indexOffset };
        }

        public void SetDrawRange(int start, int count)
        {
            this.DrawRange.Start = start;
            this.DrawRange.Count = count;
        }

        public override void ApplyMatrix4(Matrix4 matrix)
        {

            if (this.Attributes.ContainsKey("position"))
            {
                var position = (BufferAttribute<float>)this.Attributes["position"];

                matrix.ApplyToVector3Array(position.Array);
                position.needsUpdate = true;
            }

            if (this.Attributes.ContainsKey("normal"))
            {
                var normal = (BufferAttribute<float>)this.Attributes["normal"];

                var normalMatrix = new Matrix3().GetNormalMatrix(matrix);

                normalMatrix.ApplyToVector3Array(normal.Array);
                normal.needsUpdate = true;
            }
        }

        public void AddGroup(int start, int count, int materialIndex = 0)
        {
            this.groups.Add(new BufferGeometryGroups()
            {
                Start = start,
                Count = count,
                MaterialIndex = materialIndex
            });
        }

        public BufferGeometry SetFromPoints(IEnumerable<Vector3> points)
        {
            var position = new List<float>();

            foreach (var point in points)
            {
                position.Add(point.X);
                position.Add(point.Y);
                position.Add(point.Z);
            }

            this.SetAttribute("position", new BufferAttribute<float>(position.ToArray(), 3));

            return this;
        }
    }

    public class BufferGeometryGroups
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int MaterialIndex { get; set; }
    }
}
