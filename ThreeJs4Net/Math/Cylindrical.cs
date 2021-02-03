using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThreeJs4Net.Math
{
    public class Cylindrical : IEquatable<Cylindrical>
    {
        #region --- Already in R116 ---

        public float Radius { get; set; }
        public float Theta { get; set; }
        public float Y { get; set; }

        public Cylindrical()
        {
            this.Radius = (float)1.0;
            this.Theta = 0;
            this.Y = 0;
        }

        public Cylindrical(float radius, float theta, float y)
        {
            this.Radius = radius;
            this.Theta = theta;
            this.Y = y;
        }

        public Cylindrical Set(float radius, float theta, float y)
        {
            this.Radius = radius;
            this.Theta = theta;
            this.Y = y;

            return this;
        }

        public Cylindrical Clone()
        {
            return new Cylindrical().Copy(this);
        }

        public Cylindrical Copy(Cylindrical other)
        {
            this.Radius = other.Radius;
            this.Theta = other.Theta;
            this.Y = other.Y;

            return this;
        }

        public Cylindrical SetFromVector3(Vector3 v)
        {
            return this.SetFromCartesianCoords(v.X, v.Y, v.Z);
        }

        public Cylindrical SetFromCartesianCoords(float x, float y, float z)
        {
            this.Radius = Mathf.Sqrt(x * x + z * z);
            this.Theta = Mathf.Atan2(x, z);
            this.Y = y;

            return this;
        }

        public bool Equals(Cylindrical other)
        {
            return other != null && (other.Radius == this.Radius) && (other.Theta == this.Theta) && (other.Y == this.Y);
        }

        #endregion
    }
}
