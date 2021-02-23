using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Audio;
using ThreeJs4Net.Core;
using ThreeJs4Net.Properties;

namespace ThreeJs4Net.Math
{
    using Math = System.Math;

    [DebuggerDisplay("X = {X}, Y = {Y}")]
    public class Vector2 : IEquatable<Vector2>, IVector<Vector2>
    {
        public float Width
        {
            get => this.X;
            set => this.X = value;
        }

        public float Height
        {
            get => this.Y;
            set => this.Y = value;
        }

        public static Vector2 UnitX()
        {
            return new Vector2(1, 0);
        }

        public static Vector2 UnitY()
        {
            return new Vector2(0, 1);
        }

        public float X;
        public float Y;

        public Vector2()
        {
            this.X = this.Y = 0;
        }

        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2 Zero()
        {
            return new Vector2(0, 0);
        }

        public Vector2 One()
        {
            return new Vector2(1, 1);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            v1.X += v2.X;
            v1.Y += v2.Y;
            return v1;
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            v1.X -= v2.X;
            v1.Y -= v2.Y;
            return v1;
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            v1.X *= v2.X;
            v1.Y *= v2.Y;
            return v1;
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            v1.X /= v2.X;
            v1.Y /= v2.Y;
            return v1;
        }

        #region --- Already in R116 ---
        public Vector2 Add(Vector2 v)
        {
            this.X += v.X;
            this.Y += v.Y;

            return this;
        }

        public Vector2 AddScalar(float scalar)
        {
            this.X += scalar;
            this.Y += scalar;
            return this;
        }

        public Vector2 AddScaledVector(Vector2 v, float s)
        {
            this.X += v.X * s;
            this.Y += v.Y * s;

            return this;
        }

        public Vector2 AddVectors(Vector2 v1, Vector2 v2)
        {
            this.X = v1.X + v2.X;
            this.Y = v1.Y + v2.Y;
            return this;
        }

        public float Angle()
        {
            // computes the angle in radians with respect to the positive x-axis
            var angle = Mathf.Atan2(-this.Y, -this.X) + Mathf.PI;
            return angle;
        }

        public Vector2 ApplyMatrix3(Matrix3 m)
        {
            float x = this.X, y = this.Y;
            var e = m.Elements;

            this.X = e[0] * x + e[3] * y + e[6];
            this.Y = e[1] * x + e[4] * y + e[7];

            return this;
        }

        public Vector2 Ceil()
        {
            this.X = Mathf.Ceiling(this.X);
            this.Y = Mathf.Ceiling(this.Y);

            return this;
        }

        public Vector2 Clamp(Vector2 min, Vector2 max)
        {
            // assumes min < max, componentwise
            this.X = Math.Max(min.X, Math.Min(max.X, this.X));
            this.Y = Math.Max(min.Y, Math.Min(max.Y, this.Y));

            return this;
        }

        public Vector2 ClampLength(float min, float max)
        {
            var length = this.Length();

            return this.DivideScalar(length == 0 ? 1 : length).MultiplyScalar(Math.Max(min, Math.Min(max, length)));
        }

        public Vector2 ClampScalar(float minVal, float maxVal)
        {
            this.X = Math.Max(minVal, Math.Min(maxVal, this.X));
            this.Y = Math.Max(minVal, Math.Min(maxVal, this.Y));

            return this;
        }


        public Vector2 Clone()
        {
            return new Vector2(this.X, this.Y);
        }

        public float Cross(Vector2 v)
        {
            return this.X * v.Y - this.Y * v.X;
        }

        public Vector2 Copy(Vector2 vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceTo(Vector2 vector)
        {
            return Mathf.Sqrt(this.DistanceToSquared(vector));
        }

        public float DistanceToSquared(Vector2 vector)
        {
            var dx = this.X - vector.X;
            var dy = this.Y - vector.Y;
            return dx * dx + dy * dy;
        }

        public Vector2 Divide(Vector2 v)
        {
            this.X /= v.X;
            this.Y /= v.Y;
            return this;
        }


        public Vector2 DivideScalar(float scalar)
        {
            return this.MultiplyScalar(1 / scalar);
        }

        public float Dot(Vector2 v)
        {
            return this.X * v.X + this.Y * v.Y;
        }

        public bool Equals(Vector2 v)
        {
            return ((v.X == this.X) && (v.Y == this.Y));
        }

        public Vector2 FromArray(float[] array, int offset = 0)
        {
            this.X = array[offset];
            this.Y = array[offset + 1];

            return this;
        }

        public Vector2 FromBufferAttribute(BufferAttribute<float> attribute, int index)
        {
            this.X = attribute.GetX(index);
            this.Y = attribute.GetY(index);

            return this;
        }

        public Vector2 Floor()
        {
            this.X = Mathf.Floor(this.X);
            this.Y = Mathf.Floor(this.Y);

            return this;
        }

        public float GetComponent(int index)
        {
            return index switch
            {
                0 => this.X,
                1 => this.Y,
                _ => throw new IndexOutOfRangeException($"Index {index} is out of bounds")
            };
        }

        public float Length()
        {
            return Mathf.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        public float LengthSq()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        public Vector2 Lerp(Vector2 v, float alpha)
        {
            this.X += (v.X - this.X) * alpha;
            this.Y += (v.Y - this.Y) * alpha;

            return this;
        }

        public Vector2 LerpVectors(Vector2 v1, Vector2 v2, float alpha)
        {
            return this.SubVectors(v2, v1).MultiplyScalar(alpha).Add(v1);
        }

        public float ManhattanLength()
        {
            return Mathf.Abs(this.X) + Mathf.Abs(this.Y);
        }

        public Vector2 Normalize()
        {
            var length = this.Length();
            return this.DivideScalar(length == 0 ? 1 : length);
        }


        public float ManhattanDistanceTo(Vector2 v)
        {
            return Mathf.Abs(this.X - v.X) + Mathf.Abs(this.Y - v.Y);
        }

        public Vector2 Max(Vector2 v)
        {
            this.X = Math.Max(this.X, v.X);
            this.Y = Math.Max(this.Y, v.Y);

            return this;
        }

        public Vector2 Min(Vector2 v)
        {
            this.X = Math.Min(this.X, v.X);
            this.Y = Math.Min(this.Y, v.Y);

            return this;
        }

        public Vector2 Multiply(Vector2 v)
        {
            this.X *= v.X;
            this.Y *= v.Y;
            return this;
        }

        public Vector2 MultiplyScalar(float scalar)
        {
            this.X *= scalar;
            this.Y *= scalar;
            return this;
        }

        public Vector2 Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;

            return this;
        }

        public Vector2 Round()
        {
            this.X = Mathf.Round(this.X);
            this.Y = Mathf.Round(this.Y);

            return this;
        }

        public Vector2 RoundToZero()
        {
            this.X = (this.X < 0) ? Mathf.Ceiling(this.X) : Mathf.Floor(this.X);
            this.Y = (this.Y < 0) ? Mathf.Ceiling(this.Y) : Mathf.Floor(this.Y);

            return this;
        }

        public Vector2 RotateAround(Vector2 center, float angle)
        {
            float c = Mathf.Cos(angle), s = Mathf.Sin(angle);

            var x = this.X - center.X;
            var y = this.Y - center.Y;

            this.X = x * c - y * s + center.X;
            this.Y = x * s + y * c + center.Y;

            return this;
        }

        public Vector2 Random()
        {
            this.X = Mathf.RandomF();
            this.Y = Mathf.RandomF();

            return this;
        }

        public Vector2 Set(float X, float Y)
        {
            this.X = X;
            this.Y = Y;

            return this;
        }

        public Vector2 SetComponent(int index, float value)
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                default:
                    throw new IndexOutOfRangeException($"Index {index} is out of bounds");
            }

            return this;
        }

        public Vector2 SetScalar(float scalar)
        {
            this.X = scalar;
            this.Y = scalar;

            return this;
        }

        public Vector2 SetLength(float length)
        {
            return this.Normalize().MultiplyScalar(length);
        }

        public Vector2 SetX(float x)
        {
            this.X = x;
            return this;
        }

        public Vector2 SetY(float y)
        {
            this.Y = y;
            return this;
        }

        public Vector2 Sub(Vector2 v)
        {
            this.X -= v.X;
            this.Y -= v.Y;

            return this;
        }

        public Vector2 SubScalar(float s)
        {
            this.X -= s;
            this.Y -= s;

            return this;
        }

        public Vector2 SubVectors(Vector2 a, Vector2 b)
        {
            this.X = a.X - b.X;
            this.Y = a.Y - b.Y;

            return this;
        }

        public float[] ToArray(ref float[] array, int offset = 0)
        {
            if (array == null)
            {
                array = new float[2];
            }

            if (array.Length < offset + 2)
            {
                Array.Resize(ref array, offset + 2);
            }

            array[offset] = this.X;
            array[offset + 1] = this.Y;

            return array;
        }
       
        #endregion

    }

}