using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Properties;

namespace ThreeJs4Net.Math
{
    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}")]
    public class Vector3 : IEquatable<Vector3>, INotifyPropertyChanged
    {
        public static Vector3 Infinity()
        {
            return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        }

        public static Vector3 NegativeInfinity()
        {
            return new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        }

        public static Vector3 UnitX()
        {
            return new Vector3(1, 0, 0);
        }

        public static Vector3 UnitY()
        {
            return new Vector3(0, 1, 0);
        }

        public static Vector3 UnitZ()
        {
            return new Vector3(0, 0, 1);
        }

        public static Vector3 Zero()
        {
            return new Vector3(0, 0, 0);
        }

        public static Vector3 One()
        {
            return new Vector3(1, 1, 1);
        }

        private float x;
        private float y;
        private float z;

        public object UserData;

        /// <summary>
        /// 
        /// </summary>
        public Vector3()
        {
            this.X = this.Y = this.Z = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        public Vector3(float scalar)
        {
            this.X = this.Y = this.Z = scalar;
        }

        public Vector3(Vector3 v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3(Vector2 v, float z)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Vector3 SetLength(float length)
        {
            return Normalize().MultiplyScalar(length);
        }

        public Vector3 SetScalar(float scalar)
        {
            x = scalar;
            y = scalar;
            z = scalar;

            return this;
        }

        public Vector3 SetX(float xValue)
        {
            this.X = xValue;

            return this;
        }

        public Vector3 SetY(float yValue)
        {
            this.y = yValue;
            return this;
        }

        public Vector3 SetZ(float zValue)
        {
            this.z = zValue;
            return this;
        }

        public float Length()
        {
            return Mathf.Sqrt(LengthSq());
        }

        public float X
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }

        public float Y
        {
            get => y;
            set
            {
                y = value;
                OnPropertyChanged();
            }
        }

        public float Z
        {
            get => z;
            set
            {
                z = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 Clone()
        {
            return new Vector3(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public Vector3 Set(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            return this;
        }

        public Vector3 Add(Vector3 v)
        {
            this.X += v.X;
            this.Y += v.Y;
            this.Z += v.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector3 Sub(Vector3 v)
        {
            this.X -= v.X;
            this.Y -= v.Y;
            this.Z -= v.Z;

            return this;
        }

        public Vector3 SubScalar(float s)
        {
            x -= s;
            y -= s;
            z -= s;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
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
                case 2:
                    this.Z = value;
                    break;

                default:
                    throw new IndexOutOfRangeException($"Index {index} is out of bounds");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetComponent(int index)
        {
            switch (index)
            {
                case 0:
                    return this.X;
                case 1:
                    return this.Y;
                case 2:
                    return this.Z;

                default:
                    throw new IndexOutOfRangeException($"Index {index} is out of bounds");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public Vector3 Copy(Vector3 another)
        {
            this.X = another.X;
            this.Y = another.Y;
            this.Z = another.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3 { X = left.X + right.X, Y = left.Y + right.Y, Z = left.Z + right.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3 { X = left.X - right.X, Y = left.Y - right.Y, Z = left.Z - right.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 left, Vector3 right)
        {
            return new Vector3 { X = left.X * right.X, Y = left.Y * right.Y, Z = left.Z * right.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 left, Vector3 right)
        {
            return new Vector3 { X = left.X / right.X, Y = left.Y / right.Y, Z = left.Z / right.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector3 AddScalar(float scalar)
        {
            this.X += scalar;
            this.Y += scalar;
            this.Z += scalar;

            return this;
        }

        public Vector3 AddScaledVector(Vector3 v, float s)
        {
            this.X += v.X * s;
            this.Y += v.Y * s;
            this.Z += v.Z * s;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 AddVectors(Vector3 left, Vector3 right)
        {
            this.X = left.X + right.X;
            this.Y = left.Y + right.Y;
            this.Z = left.Z + right.Z;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 SubVectors(Vector3 left, Vector3 right)
        {
            this.X = left.X - right.X;
            this.Y = left.Y - right.Y;
            this.Z = left.Z - right.Z;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 SubtractVectors(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public Vector3 Multiply(Vector3 v)
        {
            this.X *= v.X;
            this.Y *= v.Y;
            this.Z *= v.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector3 MultiplyScalar(float scalar)
        {
            this.X *= scalar;
            this.Y *= scalar;
            this.Z *= scalar;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 MultiplyVectors(Vector3 left, Vector3 right)
        {
            this.X = left.X * right.X;
            this.Y = left.Y * right.Y;
            this.Z = left.Z * right.Z;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 ApplyMatrix3(Matrix3 matrix)
        {
            var lX = this.X;
            var lY = this.Y;
            var lZ = this.Z;

            var e = matrix.Elements;

            this.X = e[0] * lX + e[3] * lY + e[6] * lZ;
            this.Y = e[1] * lX + e[4] * lY + e[7] * lZ;
            this.Z = e[2] * lX + e[5] * lY + e[8] * lZ;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 ApplyMatrix4(Matrix4 matrix)
        {
            var lX = this.X;
            var lY = this.Y;
            var lZ = this.Z;

            var e = matrix.Elements;

            this.X = e[0] * lX + e[4] * lY + e[8] * lZ + e[12];
            this.Y = e[1] * lX + e[5] * lY + e[9] * lZ + e[13];
            this.Z = e[2] * lX + e[6] * lY + e[10] * lZ + e[14];
            return this;
        }

        public Vector3 ApplyNormalMatrix(Matrix3 m)
        {
            return ApplyMatrix3(m).Normalize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        /// NOTE: This one was removed
        [Obsolete("Will be removed as soon as we update Camera")]
        public Vector3 ApplyProjection(Matrix4 matrix)
        {
            //REMOVE
            float X = this.X, Y = this.Y, Z = this.Z;

            var e = matrix.Elements;
            var d = 1 / (e[3] * X + e[7] * Y + e[11] * Z + e[15]); // perspective divide

            this.X = (e[0] * X + e[4] * Y + e[8] * Z + e[12]) * d;
            this.Y = (e[1] * X + e[5] * Y + e[9] * Z + e[13]) * d;
            this.Z = (e[2] * X + e[6] * Y + e[10] * Z + e[14]) * d;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public Vector3 ApplyQuaternion(Quaternion quaternion)
        {
            var lX = this.X;
            var lY = this.Y;
            var lZ = this.Z;

            var qX = quaternion.X;
            var qY = quaternion.Y;
            var qZ = quaternion.Z;
            var qw = quaternion.W;

            // calculate quat * normal

            var iX = qw * lX + qY * lZ - qZ * lY;
            var iY = qw * lY + qZ * lX - qX * lZ;
            var iZ = qw * lZ + qX * lY - qY * lX;
            var iW = -qX * lX - qY * lY - qZ * lZ;

            // calculate result * inverse quat

            this.X = iX * qw + iW * -qX + iY * -qZ - iZ * -qY;
            this.Y = iY * qw + iW * -qY + iZ * -qX - iX * -qZ;
            this.Z = iZ * qw + iW * -qZ + iX * -qY - iY * -qX;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        public Vector3 Reflect(Vector3 normal)
        {
            var vector = new Vector3().Copy(normal).MultiplyScalar(2 * Dot(normal));
            return Sub(vector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        [Obsolete("Will change when we migrate camera")]
        public Vector3 Unproject(Camera camera)
        {
            //NEEDS TO CHANGE
            var matrix = new Matrix4();

            matrix.MultiplyMatrices(camera.MatrixWorld, matrix.GetInverse(camera.ProjectionMatrix));
            return this.ApplyProjection(matrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 TransformDirection(Matrix4 matrix)
        {
            float lX = this.X;
            float lY = this.Y;
            float lZ = this.Z;

            var e = matrix.Elements;

            this.X = e[0] * lX + e[4] * lY + e[8] * lZ;
            this.Y = e[1] * lX + e[5] * lY + e[9] * lZ;
            this.Z = e[2] * lX + e[6] * lY + e[10] * lZ;

            this.Normalize();

            return this;
        }

        public Vector3 Divide(Vector3 v)
        {
            this.X /= v.X;
            this.Y /= v.Y;
            this.Z /= v.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector3 DivideScalar(float scalar)
        {
            return MultiplyScalar(1 / (scalar == 0 ? 1 : scalar));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Min(Vector3 vector)
        {
            if (this.X > vector.X) this.X = vector.X;
            if (this.Y > vector.Y) this.Y = vector.Y;
            if (this.Z > vector.Z) this.Z = vector.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Max(Vector3 vector)
        {
            if (this.X < vector.X) this.X = vector.X;
            if (this.Y < vector.Y) this.Y = vector.Y;
            if (this.Z < vector.Z) this.Z = vector.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public Vector3 Clamp(Vector3 min, Vector3 max)
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

        public Vector3 ClampLength(float minVal, float maxVal)
        {
            var length = Length();
            return DivideScalar(length).MultiplyScalar(Mathf.Max(minVal, Mathf.Min(maxVal, length)));
        }

        public Vector3 ClampScalar(float minVal, float maxVal)
        {
            var min = new Vector3(minVal, minVal, minVal);
            var max = new Vector3(maxVal, maxVal, maxVal);
            Clamp(min, max);
            return this;
        }

        public Vector3 Floor()
        {
            x = Mathf.Floor(x);
            y = Mathf.Floor(y);
            z = Mathf.Floor(z);
            return this;
        }

        public Vector3 Ceil()
        {
            x = Mathf.Ceiling(x);
            y = Mathf.Ceiling(y);
            z = Mathf.Ceiling(z);

            return this;
        }

        public Vector3 Round()
        {
            x = Mathf.Round(x);
            y = Mathf.Round(y);
            z = Mathf.Round(z);

            return this;
        }

        public Vector3 RoundToZero()
        {
            x = (x < 0) ? Mathf.Ceiling(x) : Mathf.Floor(x);
            y = (y < 0) ? Mathf.Ceiling(y) : Mathf.Floor(y);
            z = (z < 0) ? Mathf.Ceiling(z) : Mathf.Floor(z);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 Negate()
        {
            x = -x;
            y = -y;
            z = -z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float Dot(Vector3 vector)
        {
            return (this.X * vector.X + this.Y * vector.Y + this.Z * vector.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float LengthSq()
        {
            return (this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float ManhattanLength()
        {
            return (Mathf.Abs(this.X) + Mathf.Abs(this.Y) + Mathf.Abs(this.Z));
        }

        public float ManhattanDistanceTo(Vector3 v)
        {
            return Mathf.Abs(this.X - v.X) + Mathf.Abs(this.Y - v.Y) + Mathf.Abs(this.Z - v.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 Normalize()
        {
            return this.DivideScalar(this.Length());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public Vector3 Lerp(Vector3 vector, float alpha)
        {
            this.X += (vector.X - this.X) * alpha;
            this.Y += (vector.Y - this.Y) * alpha;
            this.Z += (vector.Z - this.Z) * alpha;

            return this;
        }

        public Vector3 LerpVectors(Vector3 v1, Vector3 v2, float alpha)
        {
            return new Vector3().SubVectors(v2, v1).MultiplyScalar(alpha).Add(v1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Cross(Vector3 vector)
        {
            return this.CrossVectors(this, vector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 CrossVectors(Vector3 left, Vector3 right)
        {
            float aX = left.X, aY = left.Y, aZ = left.Z;
            float bX = right.X, bY = right.Y, bZ = right.Z;

            this.X = aY * bZ - aZ * bY;
            this.Y = aZ * bX - aX * bZ;
            this.Z = aX * bY - aY * bX;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float AngleTo(Vector3 vector)
        {
            var denominator = Mathf.Sqrt(LengthSq() * vector.LengthSq());
            if (denominator == 0)
            {
                return Mathf.PI / 2;
            }

            var theta = Dot(vector) / (float)denominator;

            // clamp, to handle numerical problems
            return Mathf.Acos(MathUtils.Clamp(theta, -1, 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceTo(Vector3 vector)
        {
            return Mathf.Sqrt(this.DistanceToSquared(vector));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceToSquared(Vector3 vector)
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
        public Vector3 SetFromMatrixPosition(Matrix4 matrix)
        {
            this.X = matrix.Elements[12];
            this.Y = matrix.Elements[13];
            this.Z = matrix.Elements[14];

            return this;
        }

        public float this[int key]
        {
            get
            {
                switch (key)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default: throw new Exception("index is out of range: " + key);
                }
            }
            set
            {
                switch (key)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default: throw new Exception("index is out of range: " + key);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Obsolete("Will be removed as soon as we update BoxGeometry")]
        public Vector3 SetValue(string i, float value)
        {
            switch (i)
            {
                case "x":
                    this.X = value;
                    break;
                case "y":
                    this.Y = value;
                    break;
                case "z":
                    this.Z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("i can only be x,y or z");
                    break;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 SetFromMatrixScale(Matrix4 matrix)
        {
            var sx = SetFromMatrixColumn(matrix, 0).Length();
            var sy = SetFromMatrixColumn(matrix, 1).Length();
            var sz = SetFromMatrixColumn(matrix, 2).Length();

            this.X = sx;
            this.Y = sy;
            this.Z = sz;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 SetFromMatrixColumn(Matrix4 matrix, int index)
        {
            return FromArray(matrix.elements, index * 4);
        }

        public Vector3 SetFromMatrix3Column(Matrix3 m, int index)
        {
            return FromArray(m.Elements, index * 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool Equals(Vector3 vector)
        {
            if (vector == null)
            {
                return false;
            }

            return ((vector.X == this.X) && (vector.Y == this.Y) && (vector.Z == this.Z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Vector3 FromArray(float[] source, int offset = 0)
        {
            this.X = source[offset];
            this.Y = source[offset + 1];
            this.Z = source[offset + 2];

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float[] ToArray()
        {
            return new[] { this.X, this.Y, this.Z };
        }

        public float[] ToArray(ref float[] array, int offset = 0)
        {
            if (array == null)
            {
                array = new float[3];
            }

            if (array.Length < offset + 3)
            {
                Array.Resize(ref array, offset + 3);
            }

            array[offset] = this.X;
            array[offset + 1] = this.Y;
            array[offset + 2] = this.Z;

            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 ProjectOnVector(Vector3 vector)
        {
            var denominator = vector.LengthSq();
            if (denominator == 0)
            {
                return Set(0, 0, 0);
            }

            var scalar = vector.Dot(this) / denominator;
            var t1 = Copy(vector).MultiplyScalar(scalar);
            return Copy(t1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public Vector3 ApplyEuler(Euler euler)
        {
            this.ApplyQuaternion(new Quaternion().SetFromEuler(euler));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector3 ApplyAxisAngle(Vector3 axis, float angle)
        {
            return this.ApplyQuaternion(new Quaternion().SetFromAxisAngle(axis, angle));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planeNormal"></param>
        /// <returns></returns>
        public Vector3 ProjectOnPlane(Vector3 planeNormal)
        {
            var vector = new Vector3().Copy(this).ProjectOnVector(planeNormal);
            return Sub(vector);
        }

        public Vector3 Random()
        {
            x = Mathf.RandomF();
            y = Mathf.RandomF();
            z = Mathf.RandomF();

            return this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() << 8 ^ Z.GetHashCode() << 16;
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