using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreeJs4Net.Properties;

namespace ThreeJs4Net.Math
{
    public class Matrix4 : ICloneable, INotifyPropertyChanged
    {
        [NotNull]
        public float[] elements = new float[16];

        /// <summary>
        /// 
        /// </summary>
        public Matrix4()
        {
            Identity();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Matrix4(Vector4 x, Vector4 y, Vector4 z, Vector4 w)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public Matrix4(float[] values)
        {
            this.Set(values);
        }

        public float[] Elements
        {
            get => elements;
            set { elements = value; this.OnPropertyChanged(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private bool CheckParamArray(float[] values)
        {
            if (values.Length == 16)
            {
                return true;
            }

            Trace.TraceWarning("Value Array too small.");
            return false;
        }

        public Matrix4 Set(float n11, float n12, float n13, float n14, float n21, float n22, float n23, float n24, float n31, float n32, float n33, float n34, float n41, float n42, float n43, float n44)
        {
            var te = this.elements;

            te[0] = n11; te[4] = n12; te[8] = n13; te[12] = n14;
            te[1] = n21; te[5] = n22; te[9] = n23; te[13] = n24;
            te[2] = n31; te[6] = n32; te[10] = n33; te[14] = n34;
            te[3] = n41; te[7] = n42; te[11] = n43; te[15] = n44;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public Matrix4 Set(float[] values)
        {
            if (this.CheckParamArray(values))
            {
                for (var i = 0; i < values.Length; i++)
                {
                    this.Elements[i] = values[i];
                }
            }

            return this;
        }

        public Matrix4 MakeScale(float x, float y, float z)
        {
            Set(new float[]{x,0,0,0,
                            0,y,0,0,
                            0,0,z,0,
                            0,0,0,1});
            return this;
        }

        public Matrix4 Multiply(Matrix4 left, Matrix4 right = null)
        {
            if (right != null)
            {
                Trace.TraceInformation("THREE.Matrix4: .multiply() now only accepts one argument. Use .multiplyMatrices( A, B ) instead.");
                return this.MultiplyMatrices(left, right);
            }

            return this.MultiplyMatrices(this, left);
        }

        public Matrix4 Identity()
        {
            Set(new float[]{1,0,0,0,
                            0,1,0,0,
                            0,0,1,0,
                            0,0,0,1});

            return this;
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix44 which holds the result of the multiplication</returns>
        public static Matrix4 operator *(Matrix4 left, Matrix4 right)
        {
            var ae = left.Elements;
            var be = right.Elements;

            var te = new Matrix4();

            var a11 = ae[0]; var a12 = ae[4]; var a13 = ae[8]; var a14 = ae[12];
            var a21 = ae[1]; var a22 = ae[5]; var a23 = ae[9]; var a24 = ae[13];
            var a31 = ae[2]; var a32 = ae[6]; var a33 = ae[10]; var a34 = ae[14];
            var a41 = ae[3]; var a42 = ae[7]; var a43 = ae[11]; var a44 = ae[15];

            var b11 = be[0]; var b12 = be[4]; var b13 = be[8]; var b14 = be[12];
            var b21 = be[1]; var b22 = be[5]; var b23 = be[9]; var b24 = be[13];
            var b31 = be[2]; var b32 = be[6]; var b33 = be[10]; var b34 = be[14];
            var b41 = be[3]; var b42 = be[7]; var b43 = be[11]; var b44 = be[15];

            te.Elements[0] = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
            te.Elements[4] = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
            te.Elements[8] = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
            te.Elements[12] = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;

            te.Elements[1] = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
            te.Elements[5] = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
            te.Elements[9] = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
            te.Elements[13] = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;

            te.Elements[2] = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
            te.Elements[6] = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
            te.Elements[10] = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
            te.Elements[14] = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;

            te.Elements[3] = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
            te.Elements[7] = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
            te.Elements[11] = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
            te.Elements[15] = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

            return te;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector31"></param>
        /// <param name="quaternion"></param>
        /// <param name="vector32"></param>
        public void Decompose(Vector3 vector31, Quaternion quaternion, Vector3 vector32)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Matrix4 ExtractRotation(Matrix4 m)
        {

            var v1 = new Vector3();

            var te = this.elements;
            var me = m.elements;

            var scaleX = 1 / v1.Set(me[0], me[1], me[2]).Length();
            var scaleY = 1 / v1.Set(me[4], me[5], me[6]).Length();
            var scaleZ = 1 / v1.Set(me[8], me[9], me[10]).Length();

            te[0] = me[0] * scaleX;
            te[1] = me[1] * scaleX;
            te[2] = me[2] * scaleX;

            te[4] = me[4] * scaleY;
            te[5] = me[5] * scaleY;
            te[6] = me[6] * scaleY;

            te[8] = me[8] * scaleZ;
            te[9] = me[9] * scaleZ;
            te[10] = me[10] * scaleZ;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="te"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public Matrix4 MultiplyScalar(float s)
        {
            this.Elements[0] *= s; this.Elements[4] *= s; this.Elements[8] *= s; this.Elements[12] *= s;
            this.Elements[1] *= s; this.Elements[5] *= s; this.Elements[9] *= s; this.Elements[13] *= s;
            this.Elements[2] *= s; this.Elements[6] *= s; this.Elements[10] *= s; this.Elements[14] *= s;
            this.Elements[3] *= s; this.Elements[7] *= s; this.Elements[11] *= s; this.Elements[15] *= s;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eye"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        public Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            var x = Vector3.Zero();
            var y = Vector3.Zero();
            var z = Vector3.Zero();

            z.SubVectors(eye, target).Normalize();

            if (z.Length() == 0)
            {
                z.Z = 1;
            }

            x.CrossVectors(up, z).Normalize();

            if (x.Length() == 0)
            {
                z.X += 0.0001f;
                x.CrossVectors(up, z);
                x.Normalize();
            }

            y.CrossVectors(z, x).Normalize();

            this.Elements[0] = x.X; this.Elements[4] = y.X; this.Elements[8] = z.X;
            this.Elements[1] = x.Y; this.Elements[5] = y.Y; this.Elements[9] = z.Y;
            this.Elements[2] = x.Z; this.Elements[6] = y.Z; this.Elements[10] = z.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        public Matrix4 MakeOrthographic(float left, float right, float top, float bottom, float near, float far)
        {
            var te = this.elements;
            var w = right - left;
            var h = top - bottom;
            var p = far - near;

            var x = (right + left) / w;
            var y = (top + bottom) / h;
            var z = (far + near) / p;

            te[0] = 2 / w; te[4] = 0; te[8] = 0; te[12] = -x;
            te[1] = 0; te[5] = 2 / h; te[9] = 0; te[13] = -y;
            te[2] = 0; te[6] = 0; te[10] = -2 / p; te[14] = -z;
            te[3] = 0; te[7] = 0; te[11] = 0; te[15] = 1;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        /// <returns></returns>
        public Matrix4 MakeFrustum(float left, float right, float bottom, float top, float near, float far)
        {
            var x = 2 * near / (right - left);
            var y = 2 * near / (top - bottom);

            var a = (right + left) / (right - left);
            var b = (top + bottom) / (top - bottom);
            var c = -(far + near) / (far - near);
            var d = -2 * far * near / (far - near);

            this.Elements[0] = x; this.Elements[4] = 0; this.Elements[8] = a; this.Elements[12] = 0;
            this.Elements[1] = 0; this.Elements[5] = y; this.Elements[9] = b; this.Elements[13] = 0;
            this.Elements[2] = 0; this.Elements[6] = 0; this.Elements[10] = c; this.Elements[14] = d;
            this.Elements[3] = 0; this.Elements[7] = 0; this.Elements[11] = -1; this.Elements[15] = 0;

            return this;
        }


        public Matrix4 MakePerspective(float left, float right, float top, float bottom, float near, float far)
        {
            var te = this.elements;
            var x = 2 * near / (right - left);
            var y = 2 * near / (top - bottom);

            var a = (right + left) / (right - left);
            var b = (top + bottom) / (top - bottom);
            var c = -(far + near) / (far - near);
            var d = -2 * far * near / (far - near);

            te[0] = x; te[4] = 0; te[8] = a; te[12] = 0;
            te[1] = 0; te[5] = y; te[9] = b; te[13] = 0;
            te[2] = 0; te[6] = 0; te[10] = c; te[14] = d;
            te[3] = 0; te[7] = 0; te[11] = -1; te[15] = 0;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Matrix4 Copy(Matrix4 m)
        {
            this.Set(m.elements);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public float[] ApplyToVector3Array(float[] array, int offset, int length)
        {
            var v1 = new Vector3();

            for (int i = 0, j = offset, il; i < length; i += 3, j += 3)
            {

                v1.X = array[j];
                v1.Y = array[j + 1];
                v1.Z = array[j + 2];

                v1.ApplyMatrix4(this);

                array[j] = v1.X;
                array[j + 1] = v1.Y;
                array[j + 2] = v1.Z;

            }

            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public float[] ApplyToVector3Array(float[] array)
        {
            var offset = 0;
            var length = array.Length;

            return ApplyToVector3Array(array, offset, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="quaternion"></param>
        /// <param name="scale"></param>
        public void Compose(Vector3 position, Quaternion quaternion, Vector3 scale)
        {
            this.MakeRotationFromQuaternion(quaternion);
            this.Scale(scale);
            this.SetPosition(position);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public Matrix4 SetPosition(Vector3 v)
        {
            this.Elements[12] = v.X;
            this.Elements[13] = v.Y;
            this.Elements[14] = v.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public Matrix4 Scale(Vector3 v)
        {
            var x = v.X; var y = v.Y; var z = v.Z;

            this.Elements[0] *= x; this.Elements[4] *= y; this.Elements[8] *= z;
            this.Elements[1] *= x; this.Elements[5] *= y; this.Elements[9] *= z;
            this.Elements[2] *= x; this.Elements[6] *= y; this.Elements[10] *= z;
            this.Elements[3] *= x; this.Elements[7] *= y; this.Elements[11] *= z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        public Matrix4 MakeRotationFromQuaternion(Quaternion q)
        {
            var x = q.X; var y = q.Y; var z = q.Z; var w = q.W;
            var x2 = x + x; var y2 = y + y; var z2 = z + z;
            var xx = x * x2; var xy = x * y2; var xz = x * z2;
            var yy = y * y2; var yz = y * z2; var zz = z * z2;
            var wx = w * x2; var wy = w * y2; var wz = w * z2;

            this.Elements[0] = 1 - (yy + zz);
            this.Elements[4] = xy - wz;
            this.Elements[8] = xz + wy;

            this.Elements[1] = xy + wz;
            this.Elements[5] = 1 - (xx + zz);
            this.Elements[9] = yz - wx;

            this.Elements[2] = xz - wy;
            this.Elements[6] = yz + wx;
            this.Elements[10] = 1 - (xx + yy);

            // last column
            this.Elements[3] = 0;
            this.Elements[7] = 0;
            this.Elements[11] = 0;

            // bottom row
            this.Elements[12] = 0;
            this.Elements[13] = 0;
            this.Elements[14] = 0;
            this.Elements[15] = 1;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PositionArray"></param>
        public void MultiplyVector3Array(List<float> PositionArray)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix4 MakeRotationFromEuler(Euler euler)
        {
            var x = euler.X; var y = euler.Y; var z = euler.Z;

            var a = (float)System.Math.Cos(x); var b = (float)System.Math.Sin(x);
            var c = (float)System.Math.Cos(y); var d = (float)System.Math.Sin(y);
            var e = (float)System.Math.Cos(z); var f = (float)System.Math.Sin(z);

            if (euler.Order == Euler.RotationOrder.XYZ)
            {

                var ae = a * e; var af = a * f; var be = b * e; var bf = b * f;

                this.Elements[0] = c * e;
                this.Elements[4] = -c * f;
                this.Elements[8] = d;

                this.Elements[1] = af + be * d;
                this.Elements[5] = ae - bf * d;
                this.Elements[9] = -b * c;

                this.Elements[2] = bf - ae * d;
                this.Elements[6] = be + af * d;
                this.Elements[10] = a * c;

            }
            else if (euler.Order == Euler.RotationOrder.YXZ)
            {

                var ce = c * e; var cf = c * f; var de = d * e; var df = d * f;

                this.Elements[0] = ce + df * b;
                this.Elements[4] = de * b - cf;
                this.Elements[8] = a * d;

                this.Elements[1] = a * f;
                this.Elements[5] = a * e;
                this.Elements[9] = -b;

                this.Elements[2] = cf * b - de;
                this.Elements[6] = df + ce * b;
                this.Elements[10] = a * c;

            }
            else if (euler.Order == Euler.RotationOrder.ZXY)
            {

                var ce = c * e; var cf = c * f; var de = d * e; var df = d * f;

                this.Elements[0] = ce - df * b;
                this.Elements[4] = -a * f;
                this.Elements[8] = de + cf * b;

                this.Elements[1] = cf + de * b;
                this.Elements[5] = a * e;
                this.Elements[9] = df - ce * b;

                this.Elements[2] = -a * d;
                this.Elements[6] = b;
                this.Elements[10] = a * c;

            }
            else if (euler.Order == Euler.RotationOrder.ZYX)
            {

                var ae = a * e; var af = a * f; var be = b * e; var bf = b * f;

                this.Elements[0] = c * e;
                this.Elements[4] = be * d - af;
                this.Elements[8] = ae * d + bf;

                this.Elements[1] = c * f;
                this.Elements[5] = bf * d + ae;
                this.Elements[9] = af * d - be;

                this.Elements[2] = -d;
                this.Elements[6] = b * c;
                this.Elements[10] = a * c;

            }
            else if (euler.Order == Euler.RotationOrder.YZX)
            {

                var ac = a * c; var ad = a * d; var bc = b * c; var bd = b * d;

                this.Elements[0] = c * e;
                this.Elements[4] = bd - ac * f;
                this.Elements[8] = bc * f + ad;

                this.Elements[1] = f;
                this.Elements[5] = a * e;
                this.Elements[9] = -b * e;

                this.Elements[2] = -d * e;
                this.Elements[6] = ad * f + bc;
                this.Elements[10] = ac - bd * f;

            }
            else if (euler.Order == Euler.RotationOrder.XZY)
            {

                var ac = a * c; var ad = a * d; var bc = b * c; var bd = b * d;

                this.Elements[0] = c * e;
                this.Elements[4] = -f;
                this.Elements[8] = d * e;

                this.Elements[1] = ac * f + bd;
                this.Elements[5] = a * e;
                this.Elements[9] = ad * f - bc;

                this.Elements[2] = bc * f - ad;
                this.Elements[6] = b * e;
                this.Elements[10] = bd * f + ac;

            }

            // last column
            this.Elements[3] = 0;
            this.Elements[7] = 0;
            this.Elements[11] = 0;

            // bottom row
            this.Elements[12] = 0;
            this.Elements[13] = 0;
            this.Elements[14] = 0;
            this.Elements[15] = 1;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix4 GetInverse(Matrix4 m)
        {
            return m.GetInverse();
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix4 GetInverse()
        {
            var n11 = this.Elements[0]; var n12 = this.Elements[4]; var n13 = this.Elements[8]; var n14 = this.Elements[12];
            var n21 = this.Elements[1]; var n22 = this.Elements[5]; var n23 = this.Elements[9]; var n24 = this.Elements[13];
            var n31 = this.Elements[2]; var n32 = this.Elements[6]; var n33 = this.Elements[10]; var n34 = this.Elements[14];
            var n41 = this.Elements[3]; var n42 = this.Elements[7]; var n43 = this.Elements[11]; var n44 = this.Elements[15];

            var result = new Matrix4();

            result.Elements[0] = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
            result.Elements[4] = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
            result.Elements[8] = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
            result.Elements[12] = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;
            result.Elements[1] = n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44;
            result.Elements[5] = n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44;
            result.Elements[9] = n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44;
            result.Elements[13] = n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34;
            result.Elements[2] = n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44;
            result.Elements[6] = n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44;
            result.Elements[10] = n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44;
            result.Elements[14] = n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34;
            result.Elements[3] = n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43;
            result.Elements[7] = n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43;
            result.Elements[11] = n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43;
            result.Elements[15] = n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33;

            var det = n11 * result.Elements[0] + n21 * result.Elements[4] + n31 * result.Elements[8] + n41 * result.Elements[12];

            if (det == 0)
            {
                //var msg = "Matrix4.getInverse(): can't invert matrix; var determinant is 0";

                return new Matrix4().Identity();
            }

            result.MultiplyScalar(1 / det);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetMaxScaleOnAxis()
        {
            var te = this.elements;

            var scaleXSq = te[0] * te[0] + te[1] * te[1] + te[2] * te[2];
            var scaleYSq = te[4] * te[4] + te[5] * te[5] + te[6] * te[6];
            var scaleZSq = te[8] * te[8] + te[9] * te[9] + te[10] * te[10];

            return (float)System.Math.Sqrt(System.Math.Max(scaleXSq, System.Math.Max(scaleYSq, scaleZSq)));
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Matrix4().Copy(this);
        }

        public Matrix4 MakeTranslation(float x, float y, float z)
        {
            this.Set(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1
            );
            return this;
        }

        public Matrix4 MakeRotationX(float theta)
        {
            float c = Mathf.Cos(theta), s = Mathf.Sin(theta);

            this.Set(
                1, 0, 0, 0,
                0, c, -s, 0,
                0, s, c, 0,
                0, 0, 0, 1
            );

            return this;

        }

        public Matrix4 MakeRotationY(float theta)
        {
            float c = Mathf.Cos(theta), s = Mathf.Sin(theta);

            this.Set(
                c, 0, s, 0,
                0, 1, 0, 0,
                -s, 0, c, 0,
                0, 0, 0, 1
            );

            return this;

        }

        public Matrix4 MakeRotationZ(float theta)
        {
            float c = Mathf.Cos(theta), s = Mathf.Sin(theta);
            this.Set(
                c, -s, 0, 0,
                s, c, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
            return this;
        }


        public Matrix4 Translate(Vector3 vec3)
        {
            var x = vec3.X;
            var y = vec3.Y;
            var z = vec3.Z;
            float a00, a01, a02, a03;
            float a10, a11, a12, a13;
            float a20, a21, a22, a23;

            var a = this.elements;

            a00 = a[0]; a01 = a[1]; a02 = a[2]; a03 = a[3];
            a10 = a[4]; a11 = a[5]; a12 = a[6]; a13 = a[7];
            a20 = a[8]; a21 = a[9]; a22 = a[10]; a23 = a[11];

            var b = new Matrix4();
            var o = b.elements;
            o[0] = a00; o[1] = a01; o[2] = a02; o[3] = a03;
            o[4] = a10; o[5] = a11; o[6] = a12; o[7] = a13;
            o[8] = a20; o[9] = a21; o[10] = a22; o[11] = a23;

            o[12] = a00 * x + a10 * y + a20 * z + a[12];
            o[13] = a01 * x + a11 * y + a21 * z + a[13];
            o[14] = a02 * x + a12 * y + a22 * z + a[14];
            o[15] = a03 * x + a13 * y + a23 * z + a[15];

            return b;
        }

        public Matrix4 Rotate(Matrix4 mat, float rad, Vector3 axis)
        {
            float x = axis.X, y = axis.Y, z = axis.Z;
            float len = Mathf.Sqrt(x * x + y * y + z * z);

            if (Mathf.Abs(len) < 0.00001) { return null; }

            len = 1 / len;
            x *= len;
            y *= len;
            z *= len;

            var s = Mathf.Sin(rad);
            var c = Mathf.Cos(rad);
            var t = 1 - c;

            var a = mat.elements;

            var a00 = a[0]; var a01 = a[1]; var a02 = a[2]; var a03 = a[3];
            var a10 = a[4]; var a11 = a[5]; var a12 = a[6]; var a13 = a[7];
            var a20 = a[8]; var a21 = a[9]; var a22 = a[10]; var a23 = a[11];

            // Construct the elements of the rotation matrix
            var b00 = x * x * t + c; var b01 = y * x * t + z * s; var b02 = z * x * t - y * s;
            var b10 = x * y * t - z * s; var b11 = y * y * t + c; var b12 = z * y * t + x * s;
            var b20 = x * z * t + y * s; var b21 = y * z * t - x * s; var b22 = z * z * t + c;

            var o = this.elements;

            // Perform rotation-specific matrix multiplication
            o[0] = a00 * b00 + a10 * b01 + a20 * b02;
            o[1] = a01 * b00 + a11 * b01 + a21 * b02;
            o[2] = a02 * b00 + a12 * b01 + a22 * b02;
            o[3] = a03 * b00 + a13 * b01 + a23 * b02;
            o[4] = a00 * b10 + a10 * b11 + a20 * b12;
            o[5] = a01 * b10 + a11 * b11 + a21 * b12;
            o[6] = a02 * b10 + a12 * b11 + a22 * b12;
            o[7] = a03 * b10 + a13 * b11 + a23 * b12;
            o[8] = a00 * b20 + a10 * b21 + a20 * b22;
            o[9] = a01 * b20 + a11 * b21 + a21 * b22;
            o[10] = a02 * b20 + a12 * b21 + a22 * b22;
            o[11] = a03 * b20 + a13 * b21 + a23 * b22;

            if (a != o)
            { // If the source and destination differ, copy the unchanged last row
                o[12] = a[12];
                o[13] = a[13];
                o[14] = a[14];
                o[15] = a[15];
            }

            return this;
        }

        #region --- Already in R116 ---
        public Matrix4 MultiplyMatrices(Matrix4 left, Matrix4 right)
        {
            this.Set((left * right).Elements);
            return this;
        }

        public Matrix4 Premultiply(Matrix4 m)
        {
            return this.MultiplyMatrices(m, this);
        }
        #endregion

        #region --- Already in R125 ---
        public Matrix4 MakeRotationAxis(Vector3 axis, float angle)
        {
            // Based on http://www.gamedev.net/reference/articles/article1199.asp
            var c = Mathf.Cos(angle);
            var s = Mathf.Sin(angle);
            var t = 1 - c;
            float x = axis.X, y = axis.Y, z = axis.Z;
            float tx = t * x, ty = t * y;

            this.Set(
                tx * x + c, tx * y - s * z, tx * z + s * y, 0,
                tx * y + s * z, ty * y + c, ty * z - s * x, 0,
                tx * z - s * y, ty * z + s * x, t * z * z + c, 0,
                0, 0, 0, 1
            );

            return this;
        }
        #endregion

    }
}
