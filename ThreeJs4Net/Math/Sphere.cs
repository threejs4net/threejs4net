using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ThreeJs4Net.Math
{
    [DebuggerDisplay("Center = ({Center.X}, {Center.Y}, {Center.Z}), Radius = {Radius}")]
    public class Sphere: IEquatable<Sphere>
    {
        public Vector3 Center = new Vector3();

        public float Radius;

        /// <summary>
        /// 
        /// </summary>
        public Sphere()
        {
            this.Center = new Vector3();
            this.Radius = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public Sphere(Vector3 center, float radius)
        {
            this.Center.Copy(center);
            this.Radius = radius;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="optionalCenter"></param>
        public Sphere SetFromPoints(List<Vector3> points, Vector3 optionalCenter = null)
        {
            var center = this.Center;

            if (optionalCenter != null)
            {
                center.Copy(optionalCenter);
            }
            else
            {
                new Box3().SetFromPoints(points).GetCenter(center);
            }

            float maxRadiusSq = 0;

            foreach (var pt in points)
            {
                maxRadiusSq = Mathf.Max(maxRadiusSq, center.DistanceToSquared(pt));
            }

            this.Radius = Mathf.Sqrt(maxRadiusSq);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public Sphere Copy(Sphere sphere)
        {
            this.Center.Copy(sphere.Center);
            this.Radius = sphere.Radius;

            return this;
        }

        public Sphere Clone()
        {
            return new Sphere().Copy(this);
        }

        /// <summary>
        /// 
        /// </summary>
	    public bool IsEmpty()
        {
            return (this.Radius <= 0);
        }
        /*
                public void containsPoint ( point ) {

                    return ( point.distanceToSquared( this.center ) <= ( this.radius * this.radius ) );

                }

                public void distanceToPoint ( point ) {

                    return ( point.distanceTo( this.center ) - this.radius );

                }

                public void intersectsSphere ( sphere ) {

                    var radiusSum = this.radius + sphere.radius;

                    return sphere.center.distanceToSquared( this.center ) <= ( radiusSum * radiusSum );

                }

                public void clampPoint ( point, optionalTarget ) {

                    var deltaLengthSq = this.center.distanceToSquared( point );

                    var result = optionalTarget || new THREE.Vector3();
                    result.copy( point );

                    if ( deltaLengthSq > ( this.radius * this.radius ) ) {

                        result.sub( this.center ).normalize();
                        result.multiplyScalar( this.radius ).add( this.center );

                    }

                    return result;

                }

                public void getBoundingBox ( optionalTarget ) {

                    var box = optionalTarget || new THREE.Box3();

                    box.Set( this.center, this.center );
                    box.ExpandByScalar( this.radius );

                    return box;

                }
                */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Sphere ApplyMatrix4(Matrix4 matrix)
        {
            this.Center.ApplyMatrix4(matrix);
            this.Radius = this.Radius * matrix.GetMaxScaleOnAxis();
            return this;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
	    public Sphere Translate(Vector3 offset)
        {
            this.Center.Add(offset);

            return this;
        }

        public bool IntersectsPlane(Plane plane)
        {
            return Mathf.Abs(plane.DistanceToPoint(this.Center)) <= this.Radius;
        }


        #region Equality members
        public bool Equals(Sphere other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Center.Equals(other.Center) && Radius == other.Radius;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Center.Equals(((Sphere)obj).Center) && Radius == ((Sphere)obj).Radius;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Center != null ? Center.GetHashCode() : 0) * 397) ^ Radius.GetHashCode();
            }
        }
        #endregion
    }
}
