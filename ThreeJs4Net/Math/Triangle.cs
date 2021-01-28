using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreeJs4Net.Properties;

namespace ThreeJs4Net.Math
{
    [DebuggerDisplay("A = {a}, B = {b}, C = {c}")]
    public class Triangle : IEquatable<Triangle>, INotifyPropertyChanged
    {
        public Vector3 a, b, c;

        public Triangle(Vector3 a = null, Vector3 b = null, Vector3 c = null)
        {
            this.a = a ?? new Vector3();
            this.b = b ?? new Vector3();
            this.c = c ?? new Vector3();
        }

        public static Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c, Vector3 target)
        {
            target.SubVectors(c, b);
            var v0 = new Vector3().SubVectors(a, b);
            target.Cross(v0);

            var targetLengthSq = target.LengthSq();
            if (targetLengthSq > 0)
            {
                return target.MultiplyScalar(1 / Mathf.Sqrt(targetLengthSq));
            }

            return target.Set(0, 0, 0);
        }

        public static Vector3 GetBarycoord(Vector3 point, Vector3 a, Vector3 b, Vector3 c, Vector3 target)
        {
            var v0 = new Vector3().SubVectors(c, a);
            var v1 = new Vector3().SubVectors(b, a);
            var v2 = new Vector3().SubVectors(point, a);

            var dot00 = v0.Dot(v0);
            var dot01 = v0.Dot(v1);
            var dot02 = v0.Dot(v2);
            var dot11 = v1.Dot(v1);
            var dot12 = v1.Dot(v2);

            var denom = (dot00 * dot11 - dot01 * dot01);

            // collinear or singular triangle
            if (denom == 0)
            {
                // arbitrary location outside of triangle?
                // not sure if this is the best idea, maybe should be returning undefined
                return target.Set(-2, -1, -1);
            }

            var invDenom = 1 / denom;
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // barycentric coordinates must always sum to 1
            return target.Set(1 - u - v, v, u);
        }

        public static bool ContainsPoint(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
        {
            var v3 = new Vector3();
            Triangle.GetBarycoord(point, a, b, c, v3);
            return (v3.X >= 0) && (v3.Y >= 0) && ((v3.X + v3.Y) <= 1);
        }

        public static Vector2 GetUV(Vector3 point, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 target)
        {
            var v3 = new Vector3();
            Triangle.GetBarycoord(point, p1, p2, p3, v3);

            target.Set(0, 0);
            target.AddScaledVector(uv1, v3.X);
            target.AddScaledVector(uv2, v3.Y);
            target.AddScaledVector(uv3, v3.Z);

            return target;
        }

        public static bool IsFrontFacing(Vector3 a, Vector3 b, Vector3 c, Vector3 direction)
        {
            var v0 = new Vector3().SubVectors(c, b);
            var v1 = new Vector3().SubVectors(a, b);

            // strictly front facing
            return (v0.Cross(v1).Dot(direction) < 0);
        }

        public Triangle Set(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a.Copy(a);
            this.b.Copy(b);
            this.c.Copy(c);

            return this;
        }

        public Triangle SetFromPointsAndIndices(Vector3[] points, int i0, int i1, int i2)
        {
            this.a.Copy(points[i0]);
            this.b.Copy(points[i1]);
            this.c.Copy(points[i2]);

            return this;
        }

        public Triangle Clone()
        {
            return new Triangle(this.a, this.b, this.c);
        }

        public Triangle Copy(Triangle triangle)
        {
            this.a.Copy(triangle.a);
            this.b.Copy(triangle.b);
            this.c.Copy(triangle.c);

            return this;
        }

        public float GetArea()
        {
            var v0 = new Vector3().SubVectors(this.c, this.b);
            var v1 = new Vector3().SubVectors(this.a, this.b);

            return v0.Cross(v1).Length() * (float)0.5;
        }

        public Vector3 GetMidpoint(Vector3 target)
        {
            return target.AddVectors(this.a, this.b).Add(this.c).MultiplyScalar((float)1 / (float)3);

        }

        public Vector3 GetNormal(Vector3 target)
        {
            return Triangle.GetNormal(this.a, this.b, this.c, target);
        }

        //!!public Vector3 GetPlane(Plane target)
        //{
        //    return target.setFromCoplanarPoints(this.A, this.B, this.C);
        //}

        public Vector3 GetBarycoord(Vector3 point, Vector3 target)
        {
            return Triangle.GetBarycoord(point, this.a, this.b, this.c, target);
        }

        public Vector2 GetUV(Vector3 point, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 target)
        {
            return Triangle.GetUV(point, this.a, this.b, this.c, uv1, uv2, uv3, target);
        }

        public bool ContainsPoint(Vector3 point)
        {
            return Triangle.ContainsPoint(point, this.a, this.b, this.c);
        }

        public bool IsFrontFacing(Vector3 direction)
        {
            return Triangle.IsFrontFacing(this.a, this.b, this.c, direction);
        }

        //!!public bool IntersectsBox(Box3 box) {
        //    return box.intersectsTriangle( this );
        //}

        public Vector3 ClosestPointToPoint(Vector3 p, Vector3 target)
        {
            Vector3 a = this.a, b = this.b, c = this.c;
            float v, w;

            // algorithm thanks to Real-Time Collision Detection by Christer Ericson,
            // published by Morgan Kaufmann Publishers, (C) 2005 Elsevier Inc.,
            // under the accompanying license; see chapter 5.1.5 for detailed explanation.
            // basically, we're distinguishing which of the voronoi regions of the triangle
            // the point lies in with the minimum amount of redundant computation.

            var vab = new Vector3().SubVectors(b, a);
            var vac = new Vector3().SubVectors(c, a);
            var vap = new Vector3().SubVectors(p, a);
            var d1 = vab.Dot(vap);
            var d2 = vac.Dot(vap);

            if (d1 <= 0 && d2 <= 0)
            {
                // vertex region of A; barycentric coords (1, 0, 0)
                return target.Copy(a);
            }

            var vbp = new Vector3().SubVectors(p, b);
            var d3 = vab.Dot(vbp);
            var d4 = vac.Dot(vbp);
            if (d3 >= 0 && d4 <= d3)
            {
                // vertex region of B; barycentric coords (0, 1, 0)
                return target.Copy(b);
            }

            var vc = d1 * d4 - d3 * d2;
            if (vc <= 0 && d1 >= 0 && d3 <= 0)
            {
                v = d1 / (d1 - d3);
                // edge region of AB; barycentric coords (1-v, v, 0)
                return target.Copy(a).AddScaledVector(vab, v);
            }

            var vcp = new Vector3().SubVectors(p, c);
            var d5 = vab.Dot(vcp);
            var d6 = vac.Dot(vcp);
            if (d6 >= 0 && d5 <= d6)
            {
                // vertex region of C; barycentric coords (0, 0, 1)
                return target.Copy(c);
            }

            var vb = d5 * d2 - d1 * d6;
            if (vb <= 0 && d2 >= 0 && d6 <= 0)
            {
                w = d2 / (d2 - d6);
                // edge region of AC; barycentric coords (1-w, 0, w)
                return target.Copy(a).AddScaledVector(vac, w);
            }

            var va = d3 * d6 - d5 * d4;
            if (va <= 0 && (d4 - d3) >= 0 && (d5 - d6) >= 0)
            {
                var vbc = new Vector3().SubVectors(c, b);
                w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                // edge region of BC; barycentric coords (0, 1-w, w)
                return target.Copy(b).AddScaledVector(vbc, w); // edge region of BC
            }

            // face region
            var denom = 1 / (va + vb + vc);
            // u = va * denom
            v = vb * denom;
            w = vc * denom;

            return target.Copy(a).AddScaledVector(vab, v).AddScaledVector(vac, w);
        }

        #region Equality members
        public bool Equals(Triangle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(a, other.a) && Equals(b, other.b) && Equals(c, other.c);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Triangle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (a != null ? a.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (b != null ? b.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (c != null ? c.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}