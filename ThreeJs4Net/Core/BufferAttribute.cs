using System;
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
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array"></param>
        /// <param name="itemSize"></param>
        public BufferAttribute(T[] array, int itemSize) : this()
        {
            this.Array = array;
            this.ItemSize = itemSize;
            this.Type = typeof(T);
        }

        /// <summary>
        /// 
        /// </summary>
        public int length => this.Array.Length;

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
    }
}
