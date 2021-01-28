using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ThreeJs4Net.Math
{
    public class Spherical : IEquatable<Spherical>
    {
        public float Radius;
        public float Phi;
        public float Theta;

        public Spherical(float radius = (float)1.0, float phi = 0, float theta = 0)
        {

        }

        public Spherical Set(float radius, float phi, float theta)
        {
            this.Radius = radius;
            this.Phi = phi;
            this.Theta = theta;

            return this;
        }

        public Spherical Clone()
        {
            return new Spherical().Copy(this);
        }

        public Spherical Copy(Spherical other)
        {
            this.Radius = other.Radius;
            this.Phi = other.Phi;
            this.Theta = other.Theta;

            return this;
        }

        // restrict phi to be betwee EPS and PI-EPS
        public Spherical MakeSafe()
        {
            float EPS = (float)0.000001;
            this.Phi = Mathf.Max(EPS, Mathf.Min(Mathf.PI - EPS, this.Phi));

            return this;
        }

        public Spherical SetFromVector3(Vector3 v)
        {
            return this.SetFromCartesianCoords(v.X, v.Y, v.Z);
        }

        public Spherical SetFromCartesianCoords(float x, float y, float z)
        {
            this.Radius = Mathf.Sqrt(x * x + y * y + z * z);

            if (this.Radius == 0)
            {
                this.Theta = 0;
                this.Phi = 0;
            }
            else
            {
                this.Theta = Mathf.Atan2(x, z);
                this.Phi = Mathf.Acos(MathUtils.Clamp(y / this.Radius, -1, 1));
            }

            return this;
        }

        #region Equality members
        public bool Equals(Spherical other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Radius == other.Radius && Phi == other.Phi && Theta == other.Theta;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((Spherical) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Radius.GetHashCode();
                hashCode = (hashCode * 397) ^ Phi.GetHashCode();
                hashCode = (hashCode * 397) ^ Theta.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}
