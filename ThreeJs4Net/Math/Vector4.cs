using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThreeJs4Net.Properties;

namespace ThreeJs4Net.Math
{
    using Math = System.Math;

    public class Vector4 : IEquatable<Vector4>, INotifyPropertyChanged
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4()
        {
            this.X = this.Y = this.Z = this.W = 0;
        }

        public Vector4(float Scalar)
        {
            this.X = this.Y = this.Z = this.W = Scalar;
        }

        public Vector4(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        /// <summary>
        /// Defines A unit-length Vector4 that points towards the X-axis.
        /// </summary>
        public static Vector4 UnitX = new Vector4(1, 0, 0, 0);

        /// <summary>
        /// Defines A unit-length Vector4 that points towards the Y-axis.
        /// </summary>
        public static Vector4 UnitY = new Vector4(0, 1, 0, 0);

        /// <summary>
        /// Defines A unit-length Vector4 that points towards the Z-axis.
        /// </summary>
        public static Vector4 UnitZ = new Vector4(0, 0, 1, 0);

        /// <summary>
        /// Defines A unit-length Vector4 that points towards the W-axis.
        /// </summary>
        public static Vector4 UnitW = new Vector4(0, 0, 0, 1);

        /// <summary>
        /// Defines A zero-length Vector4.
        /// </summary>
    //    public static Vector4 Zero = new Vector4(0, 0, 0, 0);

        /// <summary>
        /// Defines an instance with all components Set to 1.
        /// </summary>
  //      public static readonly Vector4 One = new Vector4(1, 1, 1, 1);

        public float Length
        {
            get { return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z); }
            set
            {
                var OldLength = (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);

                if (OldLength != 0 && value != OldLength)
                {
                    this.MultiplyScalar(value / OldLength);
                }
            }
        }

        public Vector4 Clone()
        {
            return new Vector4(this.X, this.Y, this.Z, this.W);
        }

        public Vector4 Set(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector4 Zero()
        {
            return new Vector4(0, 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector4 One()
        {
            return new Vector4(1, 1, 1, 1);
        }

        public void SetComponent(int Index, float Value)
        {
            switch (Index)
            {
                case 0:
                    this.X = Value;
                    break;
                case 1:
                    this.Y = Value;
                    break;
                case 2:
                    this.Z = Value;
                    break;

                default:
                    throw new IndexOutOfRangeException(String.Format("Index {0} is out of bounds", Index));
            }
        }

        public float GetComponent(int Index)
        {
            switch (Index)
            {
                case 0:
                    return this.X;
                case 1:
                    return this.Y;
                case 2:
                    return this.Z;

                default:
                    throw new IndexOutOfRangeException(String.Format("Index {0} is out of bounds", Index));
            }
        }

        public Vector4 Copy(Vector4 Vector)
        {
            this.X = Vector.X;
            this.Y = Vector.Y;
            this.Z = Vector.Z;
            this.W = Vector.W;

            return this;
        }

        public static Vector4 operator +(Vector4 V1, Vector4 V2)
        {
            V1.X += V2.X;
            V1.Y += V2.Y;
            V1.Z += V2.Z;
            V1.W += V2.W;

            return V1;
        }

        public static Vector4 operator -(Vector4 V1, Vector4 V2)
        {
            V1.X -= V2.X;
            V1.Y -= V2.Y;
            V1.Z -= V2.Z;
            V1.W -= V2.W;

            return V1;
        }

        public static Vector4 operator *(Vector4 V1, Vector4 V2)
        {
            V1.X *= V2.X;
            V1.Y *= V2.Y;
            V1.Z *= V2.Z;
            V1.W *= V2.W;

            return V1;
        }

        public Vector4 AddScalar(float Scalar)
        {
            this.X += Scalar;
            this.Y += Scalar;
            this.Z += Scalar;
            return this;
        }

        public Vector4 AddVectors(Vector4 V1, Vector4 V2)
        {
            this.X = V1.X + V2.X;
            this.Y = V1.Y + V2.Y;
            this.Z = V1.Z + V2.Z;
            return this;
        }

        public Vector4 SubtractVectors(Vector4 V1, Vector4 V2)
        {
            this.X = V1.X - V2.X;
            this.Y = V1.Y - V2.Y;
            this.Z = V1.Z - V2.Z;
            return this;
        }

        public Vector4 MultiplyScalar(float Scalar)
        {
            this.X *= Scalar;
            this.Y *= Scalar;
            this.Z *= Scalar;
            return this;
        }

        public Vector4 MultiplyVectors(Vector4 A, Vector4 B)
        {
            this.X = A.X * B.X;
            this.Y = A.Y * B.Y;
            this.Z = A.Z * B.Z;
            return this;
        }

        public Vector4 ApplyMatrix3(Matrix3 Matrix)
        {
            var X = this.X;
            var Y = this.Y;
            var Z = this.Z;

            var E = Matrix.Elements;

            this.X = E[0] * X + E[3] * Y + E[6] * Z;
            this.Y = E[1] * X + E[4] * Y + E[7] * Z;
            this.Z = E[2] * X + E[5] * Y + E[8] * Z;
            return this;
        }

        public Vector4 ApplyMatrix4(Matrix4 Matrix)
        {
            var X = this.X;
            var Y = this.Y;
            var Z = this.Z;

            var E = Matrix.Elements;

            this.X = E[0] * X + E[4] * Y + E[8] * Z + E[12];
            this.Y = E[1] * X + E[5] * Y + E[9] * Z + E[13];
            this.Z = E[2] * X + E[6] * Y + E[10] * Z + E[14];
            return this;
        }

        public Vector4 ApplyProjection(Matrix4 Matrix)
        {
            float X = this.X, Y = this.Y, Z = this.Z;

            var e = Matrix.Elements;
            var d = 1 / (e[3] * X + e[7] * Y + e[11] * Z + e[15]); // perspective divide

            this.X = (e[0] * X + e[4] * Y + e[8] * Z + e[12]) * d;
            this.Y = (e[1] * X + e[5] * Y + e[9] * Z + e[13]) * d;
            this.Z = (e[2] * X + e[6] * Y + e[10] * Z + e[14]) * d;

            return this;
        }

        public Vector4 ApplyQuaternion(Quaternion Quaternion)
        {
            var X = this.X;
            var Y = this.Y;
            var Z = this.Z;

            var qX = Quaternion.X;
            var qY = Quaternion.Y;
            var qZ = Quaternion.Z;
            var qw = Quaternion.W;

            // calculate quat * vector

            var IX = qw * X + qY * Z - qZ * Y;
            var IY = qw * Y + qZ * X - qX * Z;
            var IZ = qw * Z + qX * Y - qY * X;
            var IW = -qX * X - qY * Y - qZ * Z;

            // calculate result * inverse quat

            this.X = IX * qw + IW * -qX + IY * -qZ - IZ * -qY;
            this.Y = IY * qw + IW * -qY + IZ * -qX - IX * -qZ;
            this.Z = IZ * qw + IW * -qZ + IX * -qY - IY * -qX;

            return this;
        }

        public Vector4 Reflect(Vector4 Vector)
        {
            var V = new Vector4();
            V.Copy(this).ProjectOnVector(Vector).MultiplyScalar(2);
            return this.SubtractVectors(V, this);
        }

        public Vector4 TransformDirection(Matrix4 Matrix)
        {
            float X = this.X, Y = this.Y, Z = this.Z;

            var e = Matrix.Elements;

            this.X = e[0] * X + e[4] * Y + e[8] * Z;
            this.Y = e[1] * X + e[5] * Y + e[9] * Z;
            this.Z = e[2] * X + e[6] * Y + e[10] * Z;

            this.Normalize();

            return this;
        }

        public static Vector4 operator /(Vector4 V1, Vector4 V2)
        {
            V1.X /= V2.X;
            V1.Y /= V2.Y;
            V1.Z /= V2.Z;
            return V1;
        }

        public Vector4 DivideScalar(float Scalar)
        {
            if (Scalar == 0)
            {
                this.Set(0f, 0f, 0f, 0f);
            }
            else
            {
                var InvScalar = 1 / Scalar;
                this.X *= InvScalar;
                this.Y *= InvScalar;
                this.Z *= InvScalar;
            }
            return this;
        }

        public Vector4 Min(Vector4 Vector)
        {
            if (this.X > Vector.X)
            {
                this.X = Vector.X;
            }
            if (this.Y > Vector.Y)
            {
                this.X = Vector.Y;
            }
            if (this.Z > Vector.Z)
            {
                this.X = Vector.Z;
            }
            return this;
        }

        public Vector4 Max(Vector4 Vector)
        {
            if (this.X < Vector.X)
            {
                this.X = Vector.X;
            }
            if (this.Y < Vector.Y)
            {
                this.X = Vector.Y;
            }
            if (this.Z < Vector.Z)
            {
                this.X = Vector.Z;
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public Vector4 Clamp(Vector4 min, Vector4 max)
        {
            // This function assumes Min < Max, if this assumption isn't true it will not operate correctlY
            if (this.X < min.X)
            {
                this.X = min.X;
            }
            else if (this.X > max.X)
            {
                this.X = max.X;
            }

            if (this.Y < min.Y)
            {
                this.Y = min.Y;
            }
            else if (this.Y > max.Y)
            {
                this.Y = max.Y;
            }

            if (this.Z < min.Z)
            {
                this.Z = min.Z;
            }
            else if (this.Z > max.Z)
            {
                this.Z = max.Z;
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector4 Negate()
        {
            return this.MultiplyScalar(-1f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float Dot(Vector4 vector)
        {
            return this.X * vector.X + this.Y * vector.Y + this.Z * vector.Z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float LengthSq()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        public float LengthManhattan()
        {
            return Math.Abs(this.X) + Math.Abs(this.Y) + Math.Abs(this.Z);
        }

        public Vector4 Normalize()
        {
            return this.DivideScalar(this.Length);
        }

        public Vector4 Lerp(Vector4 Vector, float Alpha)
        {
            this.X += (Vector.X - this.X) * Alpha;
            this.Y += (Vector.Y - this.Y) * Alpha;
            this.Z += (Vector.Z - this.Z) * Alpha;

            return this;
        }

        ///
        public Vector4 Cross(Vector4 Vector)
        {
            float X = this.X, Y = this.Y, Z = this.Z;

            this.X = Y * Vector.Z - Z * Vector.Y;
            this.Y = Z * Vector.X - X * Vector.Z;
            this.Z = X * Vector.Y - Y * Vector.X;

            return this;
        }

        public Vector4 CrossVectors(Vector4 V1, Vector4 V2)
        {
            float aX = V1.X, aY = V1.Y, aZ = V1.Z;
            float bX = V2.X, bY = V2.Y, bZ = V2.Z;

            this.X = aY * bZ - aZ * bY;
            this.Y = aZ * bX - aX * bZ;
            this.Z = aX * bY - aY * bX;

            return this;
        }

        //public float AngleTo(Vector4 Vector)
        //{
        //    var Theta = this.Dot(Vector) / (this.Length * Vector.Length);
        //    return (float)System.Math.Acos(Utils.Clamp(Theta, -1, 1));
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceTo(Vector4 vector)
        {
            return (float)Math.Sqrt(this.DistanceToSquared(vector));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceToSquared(Vector4 vector)
        {
            var dX = this.X - vector.X;
            var dY = this.Y - vector.Y;
            var dZ = this.Z - vector.Z;

            return dX * dX + dY * dY + dZ * dZ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector4 SetFromMatrixPosition(Matrix4 matrix)
        {
            this.X = matrix.Elements[12];
            this.Y = matrix.Elements[13];
            this.Z = matrix.Elements[14];
            return this;
        }

        //public Vector4 SetFromMatrixScale(Matrix3 Matrix)
        //{
        //    var sx = this.Set(Matrix.Elements[0], Matrix.Elements[1], Matrix.Elements[2]).Length;
        //    var sy = this.Set(Matrix.Elements[4], Matrix.Elements[5], Matrix.Elements[6]).Length;
        //    var sz = this.Set(Matrix.Elements[8], Matrix.Elements[9], Matrix.Elements[10]).Length;

        //    this.X = sx;
        //    this.Y = sy;
        //    this.Z = sz;

        //    return this;
        //}

        public Vector4 SetFromMatrixColumn(int Index, Matrix4 Matrix)
        {
            var Offset = Index * 4;

            var MElements = Matrix.Elements;

            this.X = MElements[Offset];
            this.Y = MElements[Offset + 1];
            this.Z = MElements[Offset + 2];

            return this;
        }

        public bool Equals(Vector4 Vector)
        {
            return ((Vector.X == this.X) && (Vector.Y == this.Y) && (Vector.Z == this.Z));
        }

        public Vector4 FromArray(float[] Source)
        {
            this.X = Source[0];
            this.Y = Source[1];
            this.Z = Source[2];
            return this;
        }

        public float[] ToArray()
        {
            return new[] { this.X, this.Y, this.Z };
        }

        public Vector4 ProjectOnVector(Vector4 Vector)
        {
            var v1 = new Vector4();
            v1.Copy(Vector).Normalize();
            var dot = this.Dot(v1);
            return this.Copy(v1).MultiplyScalar(dot);
        }

        public Vector4 ApplyEuler(Euler Euler)
        {
            this.ApplyQuaternion(new Quaternion().SetFromEuler(Euler));
            return this;
        }

        //public Vector4 ApplyAxisAngle(Vector4 Axis, float Angle)
        //{
        //    var quaternion = new Quaternion();
        //    this.ApplyQuaternion(quaternion.SetFromAxisAngle(Axis, Angle));
        //    return this;
        //}

        public Vector4 ProjectOnPlane(Vector4 PlaneNormal)
        {
            var vector = new Vector4();
            vector.Copy(this).ProjectOnVector(PlaneNormal);
            return this - vector;
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