using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ThreeJs4Net.Math;
using Attribute = ThreeJs4Net.Renderers.Shaders.Attribute;

namespace ThreeJs4Net.Core
{
    public class BufferAttribute<T> : Renderers.Shaders.Attribute, IBufferAttribute
    {
        public T[] Array
        {
            get => (T[])this["array"];
            set => this["array"] = value;
        }

        public Type Type
        {
            get => (Type)this["type"];
            set => this["type"] = value;
        }

        public int ItemSize
        {
            get => (int)this["itemSize"];
            set => this["itemSize"] = value;
        }

        public int buffer
        {
            get => (int)this["buffer"];
            set => this["buffer"] = value;
        }

        public bool needsUpdate
        {
            get => (bool)this["needsUpdate"];
            set => this["needsUpdate"] = value;
        }

        public bool Normalized
        {
            get => (bool)this["normalized"];
            set => this["normalized"] = value;
        }

        public int Usage
        {
            get => (int)this["usage"];
            set => this["usage"] = value;
        }

        public int length => this.Array.Length;

        // We are hiding the Count implementation for Dictionary in here.
        // If needed we can use base.Count to use it again
        public new int Count => this.Array.Length / this.ItemSize;

        /// <summary>
        /// Constructor
        /// </summary>
        public BufferAttribute()
        {
            this.Add("array", null);
            this.Add("itemSize", -1);
            this.Add("buffer", -1);
            this.Add("needsUpdate", false);
            this.Add("type", null);
            this.Add("normalized", false);
            this.Add("usage", Three.StaticDrawUsage);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        /// <param name="itemSize"></param>
        /// <param name="normalized"></param>
        public BufferAttribute(T[] array, int itemSize, bool normalized = false) : this()
        {
            this.Array = array;
            this.ItemSize = itemSize;
            this.Type = typeof(T);
            this.Normalized = normalized;
        }



        #region --- Already in R116 ---

        public T GetX(int index)
        {
            return this.Array[index * this.ItemSize];
        }

        public T GetY(int index)

        {
            return this.Array[index * this.ItemSize + 1];
        }

        public T GetZ(int index)
        {
            return this.Array[index * this.ItemSize + 2];
        }

        public T GetW(int index)
        {
            return this.Array[index * this.ItemSize + 3];
        }


        public BufferAttribute<T> Set(T[] source, int offset = 0)
        {
            int newSize;
            if (offset > this.Array.Length)
            {
                newSize = offset + source.Length;
            }
            else
            {
                newSize = this.Array.Length - offset + source.Length;
            }

            if (this.Array.Length < newSize)
            {
                var tempArray = new T[newSize];
                System.Array.Copy(this.Array, tempArray, this.Array.Length);
                this.Array = tempArray;
            }

            System.Array.Copy(source, 0, this.Array, offset, source.Length);

            return this;
        }

        public BufferAttribute<T> SetX(int index, T x)
        {
            this.Array[index * this.ItemSize] = x;
            return this;
        }

        public BufferAttribute<T> SetY(int index, T y)
        {
            this.Array[index * this.ItemSize + 1] = y;
            return this;
        }

        public BufferAttribute<T> SetZ(int index, T z)
        {
            this.Array[index * this.ItemSize + 2] = z;
            return this;
        }

        public BufferAttribute<T> SetW(int index, T w)
        {
            this.Array[index * this.ItemSize + 3] = w;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public BufferAttribute<T> SetXY(int index, T x, T y)
        {
            index *= this.ItemSize;

            this.Array[index] = x;
            this.Array[index + 1] = y;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public BufferAttribute<T> SetXYZ(int index, T x, T y, T z)
        {
            index *= this.ItemSize;

            this.Array[index] = x;
            this.Array[index + 1] = y;
            this.Array[index + 2] = z;

            return this;
        }

        public BufferAttribute<T> SetXYZW(int index, T x, T y, T z, T w)
        {
            index *= this.ItemSize;

            this.Array[index] = x;
            this.Array[index + 1] = y;
            this.Array[index + 2] = z;
            this.Array[index + 3] = w;

            return this;
        }


        public BufferAttribute<T> SetUsage(int value)
        {
            this.Usage = value;
            return this;
        }

        public BufferAttribute<T> Copy(BufferAttribute<T> source)
        {
            //!!this.Name = source.name;
            this.Array = (new T[] { }).Concat(source.Array).ToArray();
            this.ItemSize = source.ItemSize;
            //!!this.Count = source.Count;
            this.Normalized = source.Normalized;
            this.Usage = source.Usage;

            return this;
        }

        public BufferAttribute<T> Clone()
        {
            return new BufferAttribute<T>(this.Array, this.ItemSize).Copy(this);
        }

        public BufferAttribute<T> CopyAt(int index1, BufferAttribute<T> attribute, int index2)
        {
            index1 *= this.ItemSize;
            index2 *= attribute.ItemSize;

            for (int i = 0; i < this.ItemSize; i++)
            {
                this.Array[index1 + i] = attribute.Array[index2 + i];
            }

            return this;
        }

        public BufferAttribute<T> CopyArray(T[] array)
        {
            this.Array = array;
            return this;
        }

        public BufferAttribute<T> CopyVector4sArray(IEnumerable<Vector4> vectors)
        {
            if (typeof(T) != typeof(float))
            {
                throw new Exception("BufferAttribute is not a float buffer. Can't copy Vector4 into it");
            }

            int targetLength = vectors.Count() * 4;
            var tempArray = new T[targetLength];
            int offset = 0;

            foreach (var vector in vectors)
            {
                tempArray[offset++] = (T)(object)vector.X;
                tempArray[offset++] = (T)(object)vector.Y;
                tempArray[offset++] = (T)(object)vector.Z;
                tempArray[offset++] = (T)(object)vector.W;
            }

            this.Array = tempArray;

            return this;
        }

        public BufferAttribute<T> CopyVector3sArray(IEnumerable<Vector3> vectors)
        {
            if (typeof(T) != typeof(float))
            {
                throw new Exception("BufferAttribute is not a float buffer. Can't copy Vector3 into it");
            }

            int targetLength = vectors.Count() * 3;
            var tempArray = new T[targetLength];
            int offset = 0;

            foreach (var vector in vectors)
            {
                tempArray[offset++] = (T)(object)vector.X;
                tempArray[offset++] = (T)(object)vector.Y;
                tempArray[offset++] = (T)(object)vector.Z;
            }

            this.Array = tempArray;

            return this;
        }

        public BufferAttribute<T> CopyVector2sArray(IEnumerable<Vector2> vectors)
        {
            if (typeof(T) != typeof(float))
            {
                throw new Exception("BufferAttribute is not a float buffer. Can't copy Vector2 into it");
            }

            int targetLength = vectors.Count() * 2;
            var tempArray = new T[targetLength];
            int offset = 0;

            foreach (var vector in vectors)
            {
                tempArray[offset++] = (T)(object)vector.X;
                tempArray[offset++] = (T)(object)vector.Y;
            }

            this.Array = tempArray;

            return this;
        }

        public BufferAttribute<T> ApplyMatrix3(Matrix3 m)
        {
            if (typeof(T) != typeof(float))
            {
                throw new Exception("BufferAttribute is not a float buffer. Can't apply Matrix3 to it");
            }

            for (var i = 0; i < this.Count; i++)
            {
                var vector = new Vector3
                {
                    X = (float)(object)this.GetX(i),
                    Y = (float)(object)this.GetY(i),
                    Z = (float)(object)this.GetZ(i)
                };

                vector.ApplyMatrix3(m);

                this.SetXYZ(i, (T)(object)vector.X, (T)(object)vector.Y, (T)(object)vector.Z);
            }

            return this;
        }

        public BufferAttribute<T> ApplyMatrix4(Matrix4 m)
        {
            if (typeof(T) != typeof(float))
            {
                throw new Exception("BufferAttribute is not a float buffer. Can't apply Matrix4 to it");
            }

            for (var i = 0; i < this.Count; i++)
            {
                var vector = new Vector3
                {
                    X = (float)(object)this.GetX(i),
                    Y = (float)(object)this.GetY(i),
                    Z = (float)(object)this.GetZ(i)
                };

                vector.ApplyMatrix4(m);

                this.SetXYZ(i, (T)(object)vector.X, (T)(object)vector.Y, (T)(object)vector.Z);
            }

            return this;
        }

        public BufferAttribute<T> ApplyNormalMatrix(Matrix3 m)
        {
            if (typeof(T) != typeof(float))
            {
                throw new Exception("BufferAttribute is not a float buffer. Can't apply Normal Matrix to it");
            }

            for (var i = 0; i < this.Count; i++)
            {
                var vector = new Vector3
                {
                    X = (float)(object)this.GetX(i),
                    Y = (float)(object)this.GetY(i),
                    Z = (float)(object)this.GetZ(i)
                };

                vector.ApplyNormalMatrix(m);

                this.SetXYZ(i, (T)(object)vector.X, (T)(object)vector.Y, (T)(object)vector.Z);
            }

            return this;
        }

        public BufferAttribute<T> TransformDirection(Matrix4 m)
        {
            if (typeof(T) != typeof(float))
            {
                throw new Exception("BufferAttribute is not a float buffer. Can't apply Normal Matrix to it");
            }

            for (var i = 0; i < this.Count; i++)
            {
                var vector = new Vector3
                {
                    X = (float)(object)this.GetX(i),
                    Y = (float)(object)this.GetY(i),
                    Z = (float)(object)this.GetZ(i)
                };

                vector.TransformDirection(m);

                this.SetXYZ(i, (T)(object)vector.X, (T)(object)vector.Y, (T)(object)vector.Z);
            }

            return this;
        }
        #endregion

    }
}

