using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreeJs4Net.Core;
using ThreeJs4Net.Properties;

namespace ThreeJs4Net.Math
{
    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}, W = {W}")]
    public class Quaternion : INotifyPropertyChanged
    {
        private float x;
        private float y;
        private float z;
        private float w;
        public static Quaternion Identity()
        {
            return new Quaternion(0, 0, 0, 1);
        }

        #region -- Public fields --
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

        public float W
        {
            get => w;
            set
            {
                w = value;
                OnPropertyChanged();
            }
        }
        #endregion

        internal void _UpdateEuler()
        {
            throw new NotImplementedException();
        }

        public Quaternion Set(float x, float y, float z, float w )
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;

            this.OnPropertyChanged();

            return this;
        }

        public Quaternion SetFromAxisAngle(Vector3 axis, float angle)
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/angleToQuaternion/index.htm
            // assumes axis is normalized
            float halfAngle = angle / 2, s = Mathf.Sin(halfAngle);
            this.X = axis.X * s;
            this.Y = axis.Y * s;
            this.Z = axis.Z * s;
            this.W = Mathf.Cos(halfAngle);

            this.OnPropertyChanged();

            return this;

        }

        #region -- Ctors ---
        /// <summary>
        /// 
        /// </summary>
        public Quaternion()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.W = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Quaternion(float x, float y, float z, float w = 1)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Quaternion(Quaternion quaternion)
        {
            this.X = quaternion.X;
            this.Y = quaternion.Y;
            this.Z = quaternion.Z;
            this.W = quaternion.W;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public Quaternion SetFromEuler(Euler euler)
        {
            var q = this;

            var c1 = (float)System.Math.Cos(euler.X / 2);
            var c2 = (float)System.Math.Cos(euler.Y / 2);
            var c3 = (float)System.Math.Cos(euler.Z / 2);
            var s1 = (float)System.Math.Sin(euler.X / 2);
            var s2 = (float)System.Math.Sin(euler.Y / 2);
            var s3 = (float)System.Math.Sin(euler.Z / 2);

            if (euler.Order == Euler.RotationOrder.XYZ)
            {
                q.x = s1 * c2 * c3 + c1 * s2 * s3;
                q.y = c1 * s2 * c3 - s1 * c2 * s3;
                q.z = c1 * c2 * s3 + s1 * s2 * c3;
                q.w = c1 * c2 * c3 - s1 * s2 * s3;
            }
            else if (euler.Order == Euler.RotationOrder.YXZ)
            {
                q.x = s1 * c2 * c3 + c1 * s2 * s3;
                q.y = c1 * s2 * c3 - s1 * c2 * s3;
                q.z = c1 * c2 * s3 - s1 * s2 * c3;
                q.w = c1 * c2 * c3 + s1 * s2 * s3;
            }
            else if (euler.Order == Euler.RotationOrder.ZXY)
            {
                q.x = s1 * c2 * c3 - c1 * s2 * s3;
                q.y = c1 * s2 * c3 + s1 * c2 * s3;
                q.z = c1 * c2 * s3 + s1 * s2 * c3;
                q.w = c1 * c2 * c3 - s1 * s2 * s3;
            }
            else if (euler.Order == Euler.RotationOrder.ZYX)
            {
                q.x = s1 * c2 * c3 - c1 * s2 * s3;
                q.y = c1 * s2 * c3 + s1 * c2 * s3;
                q.z = c1 * c2 * s3 - s1 * s2 * c3;
                q.w = c1 * c2 * c3 + s1 * s2 * s3;
            }
            else if (euler.Order == Euler.RotationOrder.YZX)
            {
                q.x = s1 * c2 * c3 + c1 * s2 * s3;
                q.y = c1 * s2 * c3 + s1 * c2 * s3;
                q.z = c1 * c2 * s3 - s1 * s2 * c3;
                q.w = c1 * c2 * c3 - s1 * s2 * s3;
            }
            else if (euler.Order == Euler.RotationOrder.XZY)
            {
                q.x = s1 * c2 * c3 - c1 * s2 * s3;
                q.y = c1 * s2 * c3 - s1 * c2 * s3;
                q.z = c1 * c2 * s3 + s1 * s2 * c3;
                q.w = c1 * c2 * c3 + s1 * s2 * s3;
            }

            this.OnPropertyChanged();

            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        public Quaternion SetFromRotationMatrix(Matrix4 m1)
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm

            // assumes the upper 3x3 of m is left pure rotation matrix (i.e, unscaled)

            var te = m1.elements;

            var m11 = te[0]; var m12 = te[4]; var m13 = te[8];
            var m21 = te[1]; var m22 = te[5]; var m23 = te[9];
            var m31 = te[2]; var m32 = te[6]; var m33 = te[10];

            var trace = m11 + m22 + m33;

            if (trace > 0)
            {
                var s = (float)0.5 / Mathf.Sqrt(trace + (float)1.0);

                this.W = (float)0.25 / s;
                this.X = (m32 - m23) * s;
                this.Y = (m13 - m31) * s;
                this.Z = (m21 - m12) * s;
            }
            else if (m11 > m22 && m11 > m33)
            {
                var s = (float)2.0 * Mathf.Sqrt((float)1.0 + m11 - m22 - m33);

                this.W = (m32 - m23) / s;
                this.X = (float)0.25 * s;
                this.Y = (m12 + m21) / s;
                this.Z = (m13 + m31) / s;
            }
            else if (m22 > m33)
            {
                var s = (float)2.0 * Mathf.Sqrt((float)1.0 + m22 - m11 - m33);

                this.W = (m13 - m31) / s;
                this.X = (m12 + m21) / s;
                this.Y = (float)0.25 * s;
                this.Z = (m23 + m32) / s;
            }
            else
            {
                var s = (float)2.0 * Mathf.Sqrt((float)1.0 + m33 - m11 - m22);

                this.W = (m21 - m12) / s;
                this.X = (m13 + m31) / s;
                this.Y = (m23 + m32) / s;
                this.Z = (float)0.25 * s;
            }

            this.OnPropertyChanged();

            return this;
        }

        public Quaternion SetFromUnitVectors(Vector3 vFrom, Vector3 vTo)
        {
            // http://lolengine.net/blog/2014/02/24/quaternion-from-two-vectors-final
            // assumes direction vectors vFrom and vTo are normalized
            var r = vFrom.Dot(vTo) + 1;

            if (r < MathUtils.EPS5)
            {
                r = 0;
                if (Mathf.Abs(vFrom.X) > Mathf.Abs(vFrom.Z))
                {
                    this.X = -vFrom.Y;
                    this.Y = vFrom.X;
                    this.Z = 0;
                    this.W = r;
                }
                else
                {
                    this.X = 0;
                    this.Y = -vFrom.Z;
                    this.Z = vFrom.Y;
                    this.W = r;
                }
            }
            else
            {
                // crossVectors( vFrom, vTo ); // inlined to avoid cyclic dependency on Vector3
                this.X = vFrom.Y * vTo.Z - vFrom.Z * vTo.Y;
                this.Y = vFrom.Z * vTo.X - vFrom.X * vTo.Z;
                this.Z = vFrom.X * vTo.Y - vFrom.Y * vTo.X;
                this.W = r;
            }

            return this.Normalize();
            // assumes direction vectors vFrom and vTo are normalized
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quaternion"></param>
        public Quaternion Copy(Quaternion quaternion)
        {
            this.x = quaternion.x;
            this.y = quaternion.y;
            this.z = quaternion.z;
            this.w = quaternion.w;

            this.OnPropertyChanged();

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float LengthSq()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float Length()
        {
            return Mathf.Sqrt(LengthSq());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Quaternion Normalize()
        {
            var l = this.Length();

            if (l == 0)
            {
                this.X = 0;
                this.Y = 0;
                this.Z = 0;
                this.W = 1;
            }
            else
            {
                l = 1 / l;

                this.X = this.X * l;
                this.Y = this.Y * l;
                this.Z = this.Z * l;
                this.W = this.W * l;
            }

            this.OnPropertyChanged();

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="p"></param>
        public Quaternion Multiply(Quaternion q, Quaternion p = null)
        {
            return this.MultiplyQuaternions(this, q);
        }

        public Quaternion PreMultiply(Quaternion q)
        {
            return MultiplyQuaternions(q, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Quaternion MultiplyQuaternions(Quaternion left, Quaternion right)
        {
            // from http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/code/index.htm

            var qax = left.x; var qay = left.y; var qaz = left.z; var qaw = left.w;
            var qbx = right.x; var qby = right.y; var qbz = right.z; var qbw = right.w;

            this.x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
            this.y = qay * qbw + qaw * qby + qaz * qbx - qax * qbz;
            this.z = qaz * qbw + qaw * qbz + qax * qby - qay * qbx;
            this.w = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;

            this.OnPropertyChanged();

            return this;
        }

        public Quaternion Inverse()
        {
            return this.Conjugate();
        }

        public Quaternion Conjugate()
        {
            this.x *= -1;
            this.y *= -1;
            this.z *= -1;

            this.OnPropertyChanged();

            return this;
        }

        public float Dot(Quaternion v)
        {
            return x * v.x + y * v.y + z * v.z + w * v.w;
        }

        public Quaternion Slerp(Quaternion qb, float t)
        {
            if (t == 0) return this;
            if (t == 1) return this.Copy(qb);

            float x = this.X, y = this.Y, z = this.Z, w = this.W;

            // http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/slerp/
            var cosHalfTheta = w * qb.W + x * qb.X + y * qb.Y + z * qb.Z;

            if (cosHalfTheta < 0)
            {
                this.W = -qb.W;
                this.X = -qb.X;
                this.Y = -qb.Y;
                this.Z = -qb.Z;

                cosHalfTheta = -cosHalfTheta;
            }
            else
            {
                this.Copy(qb);
            }

            if (cosHalfTheta >= 1.0)
            {
                this.W = w;
                this.X = x;
                this.Y = y;
                this.Z = z;

                return this;

            }

            var sqrSinHalfTheta = (float)1.0 - cosHalfTheta * cosHalfTheta;

            if (sqrSinHalfTheta <= MathUtils.EPS5)
            {
                var s = 1 - t;
                this.W = s * w + t * this.W;
                this.X = s * x + t * this.X;
                this.Y = s * y + t * this.Y;
                this.Z = s * z + t * this.Z;

                this.Normalize();
                this.OnPropertyChanged();

                return this;
            }

            var sinHalfTheta = Mathf.Sqrt(sqrSinHalfTheta);
            var halfTheta = Mathf.Atan2(sinHalfTheta, cosHalfTheta);
            var ratioA = Mathf.Sin((1 - t) * halfTheta) / sinHalfTheta;
            var ratioB = Mathf.Sin(t * halfTheta) / sinHalfTheta;

            this.W = (w * ratioA + this.W * ratioB);
            this.X = (x * ratioA + this.X * ratioB);
            this.Y = (y * ratioA + this.Y * ratioB);
            this.Z = (z * ratioA + this.Z * ratioB);

            this.OnPropertyChanged();

            return this;
        }

        public bool Equals(Quaternion quaternion)
        {
            if (quaternion == null)
            {
                return false;
            }

            return ((quaternion.X == this.X) && (quaternion.Y == this.Y) && (quaternion.Z == this.Z) && (quaternion.W == this.W));
        }

        public Quaternion FromArray(float[] array, int offset = 0)
        {
            this.X = array[offset];
            this.Y = array[offset + 1];
            this.Z = array[offset + 2];
            this.W = array[offset + 3];

            this.OnPropertyChanged();

            return this;
        }

        public float[] ToArray()
        {
            return new[] { this.X, this.Y, this.Z, this.W };
        }

        public float[] ToArray(ref float[] array, int offset = 0)
        {
            if (array.Length < offset + 4)
            {
                Array.Resize(ref array, offset + 4);
            }

            array[offset] = this.X;
            array[offset + 1] = this.Y;
            array[offset + 2] = this.Z;
            array[offset + 3] = this.W;

            return array;
        }

        public float AngleTo(Quaternion q)
        {
            return 2 * Mathf.Acos(Mathf.Abs(MathUtils.Clamp(this.Dot(q), -1, 1)));
        }

        public Quaternion RotateTowards(Quaternion q, float step)
        {
            var angle = this.AngleTo(q);
            if (angle == 0)
            {
                return this;
            }
            var t = Mathf.Min(1, step / angle);
            this.Slerp(q, t);

            return this;
        }

        public Quaternion Clone()
        {
            return new Quaternion(this);
        }

        public Quaternion FromBufferAttribute(object attribute, int index)
        {
            throw new Exception("Implementation required");
            //this.X = attribute.GetX( index );
            //this.Y = attribute.GetY( index );
            //this.Z = attribute.GetZ( index );
            //this.W = attribute.GetW( index );

            //return this;
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
