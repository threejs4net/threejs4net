using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreeJs4Net.Core;
using ThreeJs4Net.Properties;

namespace ThreeJs4Net.Math
{
    using Math = System.Math;

    [DebuggerDisplay("X = {X}, Y = {Y}")]
    public class Vector2 : IEquatable<Vector2>, INotifyPropertyChanged
    {
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

        /// <summary>
        /// 
        /// </summary>
        public Vector2()
        {
            this.X = this.Y = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 Clone()
        {
            return new Vector2(this.X, this.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool Equals(Vector2 vector)
        {
            return (this.X == vector.X) && (this.Y == vector.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceTo(Vector2 vector)
        {
            return (float)Math.Sqrt(this.DistanceToSquared(vector));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public Vector2 Set(float X, float Y)
        {
            this.X = X;
            this.Y = Y;

            return this;
        }

        /// <summary>
        /// Defines A zero-length Vector2.
        /// </summary>
 //       public static readonly Vector2 Zero = new Vector2(0, 0);

        /// <summary>
        /// Defines an instance with all components Set to 1.
        /// </summary>
 //       public static readonly Vector2 One = new Vector2(1, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 Zero()
        {
            return new Vector2(0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 One()
        {
            return new Vector2(1, 1);
        }

        public void SetComponent(int index, float value)
        {
            switch (index)
            {
                case 0:
                    this.X = value;
                    break;
                case 1:
                    this.Y = value;
                    break;

                default:
                    throw new IndexOutOfRangeException(String.Format("Index {0} is out of bounds", index));
            }
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


        public float GetComponent(int index)
        {
            switch (index)
            {
                case 0:
                    return this.X;
                case 1:
                    return this.Y;
                default:
                    throw new IndexOutOfRangeException(String.Format("Index {0} is out of bounds", index));
            }
        }

        public Vector2 Copy(Vector2 vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            return this;
        }

        public float DistanceToSquared(Vector2 vector)
        {
            var dx = this.X - vector.X;
            var dy = this.Y - vector.Y;
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector2 Add(Vector2 v, Vector2 w = null)
        {
            if (w != null)
            {
                Trace.TraceWarning("THREE.Vector2: .add() now only accepts one argument. Use .addVectors( A, B ) instead.");
                return this.AddVectors(v, w);

            }

            this.X += v.X;
            this.Y += v.Y;

            return this;

        }

        public Vector2 AddVectors(Vector2 v1, Vector2 v2)
        {
            this.X = v1.X + v2.X;
            this.Y = v1.Y + v2.Y;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector2 Sub(Vector2 v, Vector2 w = null)
        {
            if (w != null)
            {
                Trace.TraceInformation("THREE.Vector2: .sub() now only accepts one argument. Use .subVectors( A, B ) instead.");
                return this.SubVectors(v, w);
            }

            this.X -= v.X;
            this.Y -= v.Y;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vector2 SubVectors(Vector2 a, Vector2 b)
        {
            return SubstractVectors(a, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2 SubstractVectors(Vector2 v1, Vector2 v2)
        {
            this.X = v1.X - v2.X;
            this.Y = v1.Y - v2.Y;
            return this;
        }

        public Vector2 MultiplyScalar(float scalar)
        {
            this.X *= scalar;
            this.Y *= scalar;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public Vector2 SetLength(float l)
        {
            Length = l;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public float LengthSq
        {
            get
            {
                return this.X * this.X + this.Y * this.Y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Length
        {
            get { return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y); }
            set
            {
                var OldLength = (float)Math.Sqrt(this.X * this.X + this.Y * this.Y);
                if (OldLength == 0 && value != OldLength)
                {
                    this.MultiplyScalar(value / OldLength);
                }
            }
        }

        public Vector2 FromBufferAttribute(BufferAttribute<float> attribute, int index)
        {
            this.X = attribute.GetX(index);
            this.X = attribute.GetY(index);

            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}