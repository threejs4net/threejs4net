using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThreeJs4Net.Math
{
    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}, Order = {Order}")]
    public class Euler : INotifyPropertyChanged, IEquatable<Euler>
    {
        public static RotationOrder DefaultOrder = RotationOrder.XYZ;
        public enum RotationOrder
        {
            XYZ, YZX, ZXY, XZY, YXZ, ZYX
        }

        private RotationOrder order = DefaultOrder;
        private float x;
        private float y;
        private float z;
        public event PropertyChangedEventHandler PropertyChanged;

        public Euler()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public Euler(float x, float y, float z, RotationOrder order = RotationOrder.XYZ)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Order = order;
        }

        #region --- Already in R116 ---

        public Euler Set(float a, float b, float c)
        {
            return Set(a, b, c, RotationOrder.XYZ);
        }

        public Euler Set(float a, float b, float c, RotationOrder o)
        {
            this.X = a;
            this.Y = b;
            this.Z = c;
            this.Order = o;

            return this;
        }

        public RotationOrder Order
        {
            get => order;
            set { order = value; OnPropertyChanged(); }
        }

        public float X
        {
            get => x;
            set { x = value; OnPropertyChanged(); }
        }

        public float Y
        {
            get => y;
            set { y = value; OnPropertyChanged(); }
        }

        public float Z
        {
            get => z;
            set { z = value; OnPropertyChanged(); }
        }

        public Euler Clone()
        {
            return new Euler(this.x, this.y, this.z, this.order);
        }

        public Euler Copy(Euler euler)
        {
            this.x = euler.x;
            this.y = euler.y;
            this.z = euler.z;

            OnPropertyChanged();

            return this;
        }

        public Euler SetFromRotationMatrix(Matrix4 m, RotationOrder? rotationOrder = null, bool update = false)
        {
            // assumes the upper 3x3 of m is A pure rotation matrix (i.e, unscaled)
            var te = m.elements;
            var m11 = te[0]; var m12 = te[4]; var m13 = te[8];
            var m21 = te[1]; var m22 = te[5]; var m23 = te[9];
            var m31 = te[2]; var m32 = te[6]; var m33 = te[10];

            rotationOrder ??= this.order;

            switch (rotationOrder)
            {
                case RotationOrder.XYZ:
                    this.y = (float)System.Math.Asin(MathUtils.Clamp(m13, -1, 1));

                    if (System.Math.Abs(m13) < 0.99999)
                    {
                        this.x = (float)System.Math.Atan2(-m23, m33);
                        this.z = (float)System.Math.Atan2(-m12, m11);
                    }
                    else
                    {
                        this.x = (float)System.Math.Atan2(m32, m22);
                        this.z = 0;
                    }

                    break;
                case RotationOrder.YXZ:
                    this.x = (float)System.Math.Asin(-MathUtils.Clamp(m23, -1, 1));

                    if (System.Math.Abs(m23) < 0.99999)
                    {
                        this.y = (float)System.Math.Atan2(m13, m33);
                        this.z = (float)System.Math.Atan2(m21, m22);
                    }
                    else
                    {
                        this.y = (float)System.Math.Atan2(-m31, m11);
                        this.z = 0;
                    }

                    break;
                case RotationOrder.ZXY:
                    this.x = (float)System.Math.Asin(MathUtils.Clamp(m32, -1, 1));

                    if (System.Math.Abs(m32) < 0.99999)
                    {
                        this.y = (float)System.Math.Atan2(-m31, m33);
                        this.z = (float)System.Math.Atan2(-m12, m22);
                    }
                    else
                    {
                        this.y = 0;
                        this.z = (float)System.Math.Atan2(m21, m11);
                    }

                    break;
                case RotationOrder.ZYX:
                    this.y = (float)System.Math.Asin(-MathUtils.Clamp(m31, -1, 1));

                    if (System.Math.Abs(m31) < 0.99999)
                    {
                        this.x = (float)System.Math.Atan2(m32, m33);
                        this.z = (float)System.Math.Atan2(m21, m11);
                    }
                    else
                    {
                        this.x = 0;
                        this.z = (float)System.Math.Atan2(-m12, m22);
                    }

                    break;
                case RotationOrder.YZX:
                    this.z = (float)System.Math.Asin(MathUtils.Clamp(m21, -1, 1));

                    if (System.Math.Abs(m21) < 0.99999)
                    {

                        this.x = (float)System.Math.Atan2(-m23, m22);
                        this.y = (float)System.Math.Atan2(-m31, m11);

                    }
                    else
                    {

                        this.x = 0;
                        this.y = (float)System.Math.Atan2(m13, m33);

                    }

                    break;
                case RotationOrder.XZY:
                    this.z = (float)System.Math.Asin(-MathUtils.Clamp(m12, -1, 1));

                    if (System.Math.Abs(m12) < 0.99999)
                    {
                        this.x = (float)System.Math.Atan2(m32, m22);
                        this.y = (float)System.Math.Atan2(m13, m11);
                    }
                    else
                    {

                        this.x = (float)System.Math.Atan2(-m23, m33);
                        this.y = 0;

                    }
                    break;
                default:
                    Trace.TraceInformation("THREE.Euler: .setFromRotationMatrix() given unsupported order: " + order);
                    break;
            }

            this.Order = order;
            if (update)
            {
                this.OnPropertyChanged();
            }

            return this;
        }

        public Euler SetFromQuaternion(Quaternion q, RotationOrder? rotationOrder = null, bool update = false)
        {
            var matrix = new Matrix4().MakeRotationFromQuaternion(q);
            return this.SetFromRotationMatrix(matrix, rotationOrder, update);
        }

        public Euler SetFromVector3(Vector3 v, RotationOrder? rotationOrder)
        {
            return this.Set(v.X, v.Y, v.Z, rotationOrder ?? this.Order);
        }

        public Euler FromArray(float[] array)
        {
            this.x = array[0];
            this.y = array[1];
            this.z = array[2];
            if (array.Length > 3)
            {
                this.order = (RotationOrder)array[3];
            }

            OnPropertyChanged();

            return this;
        }

        public float[] ToArray(float[] array, int offset = 0)
        {
            array ??= new float[4];

            array[offset] = this.x;
            array[offset + 1] = this.y;
            array[offset + 2] = this.z;
            array[offset + 3] = (int)this.Order;

            return array;
        }

        public Vector3 ToVector3(Vector3 optionalResult = null)
        {
            if (optionalResult != null)
            {
                return optionalResult.Set(this.x, this.y, this.z);
            }

            return new Vector3(this.x, this.y, this.z);
        }

        public void Reorder(RotationOrder newOrder)
        {
            // WARNING: this discards revolution information -bhouston
            var q = new Quaternion().SetFromEuler(this);
            this.SetFromQuaternion(q, newOrder);
        }

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Equality members
        public bool Equals(Euler other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return order == other.order && x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Euler)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)order;
                hashCode = (hashCode * 397) ^ x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}
