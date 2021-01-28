using System;

namespace ThreeJs4Net.Math
{
    public class Ray : IEquatable<Ray>
    {
        public Vector3 Origin = new Vector3();

        public Vector3 Direction = new Vector3(0, 0, -1);

        /// <summary>
        /// 
        /// </summary>
        public Ray()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.Origin.Copy(origin);
            this.Direction.Copy(direction);
        }












        #region --- Already in R116 ---
        public Ray ApplyMatrix4(Matrix4 matrix4)
        {
            this.Origin.ApplyMatrix4(matrix4);
            this.Direction.TransformDirection(matrix4);

            return this;
        }

        public Vector3 At(float t, Vector3 target)
        {
            return target.Copy(this.Direction).MultiplyScalar(t).Add(this.Origin);
        }

        public Ray Clone()
        {
            return new Ray().Copy(this);
        }

        public Ray Copy(Ray another)
        {
            this.Origin.Copy(another.Origin);
            this.Direction.Copy(another.Direction);

            return this;
        }

        public Vector3 ClosestPointToPoint(Vector3 point, Vector3 target)
        {
            target.SubVectors(point, this.Origin);
            var directionDistance = target.Dot(this.Direction);
            if (directionDistance < 0)
            {
                return target.Copy(this.Origin);
            }
            return target.Copy(this.Direction).MultiplyScalar(directionDistance).Add(this.Origin);
        }

        public float DistanceToPoint(Vector3 point)
        {
            return Mathf.Sqrt(this.DistanceSqToPoint(point));
        }

        public float DistanceSqToPoint(Vector3 point)
        {
            var vector = new Vector3();
            var directionDistance = vector.SubVectors(point, this.Origin).Dot(this.Direction);

            // point behind the ray
            if (directionDistance < 0)
            {
                return this.Origin.DistanceToSquared(point);
            }

            vector.Copy(this.Direction).MultiplyScalar(directionDistance).Add(this.Origin);

            return vector.DistanceToSquared(point);
        }

        public float DistanceSqToSegment(Vector3 v0, Vector3 v1, Vector3 optionalPointOnRay = null, Vector3 optionalPointOnSegment = null)
        {
            // from http://www.geometrictools.com/LibMathematics/Distance/Wm5DistRay3Segment3.cpp
            // It returns the min distance between the ray and the segment
            // defined by v0 and v1
            // It can also Set two optional targets :
            // - The closest point on the ray
            // - The closest point on the segment

            var segCenter = ((Vector3)v0.Clone()).Add(v1).MultiplyScalar((float)0.5);
            var segDir = ((Vector3)v1.Clone()).Sub(v0).Normalize();
            var diff = ((Vector3)this.Origin.Clone()).Sub(segCenter);

            var segExtent = v0.DistanceTo(v1) * (float)0.5;
            var a01 = -this.Direction.Dot(segDir);
            var b0 = diff.Dot(this.Direction);
            var b1 = -diff.Dot(segDir);
            var c = diff.LengthSq();
            var det = System.Math.Abs(1 - a01 * a01);

            float s0, s1, sqrDist;

            if (det >= 0)
            {
                // The ray and segment are not parallel.

                s0 = a01 * b1 - b0;
                s1 = a01 * b0 - b1;
                var extDet = segExtent * det;

                if (s0 >= 0)
                {
                    if (s1 >= -extDet)
                    {
                        if (s1 <= extDet)
                        {
                            // region 0
                            // Minimum at interior points of ray and segment.
                            var invDet = 1 / det;
                            s0 *= invDet;
                            s1 *= invDet;
                            sqrDist = s0 * (s0 + a01 * s1 + 2 * b0) + s1 * (a01 * s0 + s1 + 2 * b1) + c;
                        }
                        else
                        {
                            // region 1
                            s1 = segExtent;
                            s0 = System.Math.Max(0, -(a01 * s1 + b0));
                            sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;
                        }
                    }
                    else
                    {
                        // region 5
                        s1 = -segExtent;
                        s0 = System.Math.Max(0, -(a01 * s1 + b0));
                        sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;
                    }
                }
                else
                {
                    if (s1 <= -extDet)
                    {
                        // region 4
                        s0 = System.Math.Max(0, -(-a01 * segExtent + b0));
                        s1 = (s0 > 0) ? -segExtent : System.Math.Min(System.Math.Max(-segExtent, -b1), segExtent);
                        sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;
                    }
                    else if (s1 <= extDet)
                    {
                        // region 3
                        s0 = 0;
                        s1 = System.Math.Min(System.Math.Max(-segExtent, -b1), segExtent);
                        sqrDist = s1 * (s1 + 2 * b1) + c;
                    }
                    else
                    {
                        // region 2
                        s0 = System.Math.Max(0, -(a01 * segExtent + b0));
                        s1 = (s0 > 0) ? segExtent : System.Math.Min(System.Math.Max(-segExtent, -b1), segExtent);
                        sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;
                    }
                }
            }
            else
            {
                // Ray and segment are parallel.
                s1 = (a01 > 0) ? -segExtent : segExtent;
                s0 = System.Math.Max(0, -(a01 * s1 + b0));
                sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;
            }

            optionalPointOnRay?.Copy(this.Direction.Clone().MultiplyScalar(s0).Add(this.Origin));
            optionalPointOnSegment?.Copy(segDir.Clone().MultiplyScalar(s1).Add(segCenter));

            return sqrDist;
        }

        public float? DistanceToPlane(Plane plane)
        {
            var denominator = plane.Normal.Dot(this.Direction);

            if (denominator == 0)
            {
                // line is coplanar, return origin
                if (plane.DistanceToPoint(this.Origin) == 0)
                {
                    return 0;
                }

                // Null is preferable to undefined since undefined means.... it is undefined
                return null;
            }

            var t = -(this.Origin.Dot(plane.Normal) + plane.Constant) / denominator;

            // Return if the ray never intersects the plane
            return t >= 0 ? t : (float?)null;
        }

        public bool Equals(Ray ray)
        {
            return ray != null && ray.Origin.Equals(this.Origin) && ray.Direction.Equals(this.Direction);
        }

        public Vector3 IntersectSphere(Sphere sphere, Vector3 target)
        {
            var vector = new Vector3().SubVectors(sphere.Center, this.Origin);
            var tca = vector.Dot(this.Direction);
            var d2 = vector.Dot(vector) - tca * tca;
            var radius2 = sphere.Radius * sphere.Radius;

            if (d2 > radius2)
            {
                return null;
            }

            var thc = Mathf.Sqrt(radius2 - d2);

            // t0 = first intersect point - entrance on front of sphere
            var t0 = tca - thc;

            // t1 = second intersect point - exit point on back of sphere
            var t1 = tca + thc;

            // test to see if both t0 and t1 are behind the ray - if so, return null
            if (t0 < 0 && t1 < 0)
            {
                return null;
            }

            // test to see if t0 is behind the ray:
            // if it is, the ray is inside the sphere, so return the second exit point scaled by t1,
            // in order to always return an intersect point that is in front of the ray.
            if (t0 < 0)
            {
                return this.At(t1, target);
            }

            // else t0 is in front of the ray, so return the first collision point scaled by t0
            return this.At(t0, target);
        }

        public bool IntersectsSphere(Sphere sphere)
        {
            return this.DistanceSqToPoint(sphere.Center) <= (sphere.Radius * sphere.Radius);
        }

        public Vector3 IntersectPlane(Plane plane, Vector3 target)
        {
            var t = this.DistanceToPlane(plane);

            if (t == null)
            {
                return null;
            }

            return this.At((float)t, target);
        }

        public bool IntersectsPlane(Plane plane)
        {
            // check if the ray lies on the plane first
            var distToPoint = plane.DistanceToPoint(this.Origin);

            if (distToPoint == 0)
            {
                return true;
            }

            var denominator = plane.Normal.Dot(this.Direction);

            if (denominator * distToPoint < 0)
            {
                return true;
            }

            // ray origin is behind the plane (and is pointing behind it)
            return false;
        }

        public Vector3 IntersectBox(Box3 box, Vector3 target)
        {
            float tmin, tmax, tymin, tymax, tzmin, tzmax;

            float invdirx = 1 / this.Direction.X,
                invdiry = 1 / this.Direction.Y,
                invdirz = 1 / this.Direction.Z;

            var origin = this.Origin;

            if (invdirx >= 0)
            {
                tmin = (box.Min.X - origin.X) * invdirx;
                tmax = (box.Max.X - origin.X) * invdirx;
            }
            else
            {
                tmin = (box.Max.X - origin.X) * invdirx;
                tmax = (box.Min.X - origin.X) * invdirx;
            }

            if (invdiry >= 0)
            {
                tymin = (box.Min.Y - origin.Y) * invdiry;
                tymax = (box.Max.Y - origin.Y) * invdiry;
            }
            else
            {
                tymin = (box.Max.Y - origin.Y) * invdiry;
                tymax = (box.Min.Y - origin.Y) * invdiry;
            }

            if ((tmin > tymax) || (tymin > tmax))
            {
                return null;
            }

            // These lines also handle the case where tmin or tmax is NaN
            // (result of 0 * Infinity). x !== x returns true if x is NaN

            if (tymin > tmin || tmin != tmin)
            {
                tmin = tymin;
            }

            if (tymax < tmax || tmax != tmax)
            {
                tmax = tymax;
            }

            if (invdirz >= 0)
            {
                tzmin = (box.Min.Z - origin.Z) * invdirz;
                tzmax = (box.Max.Z - origin.Z) * invdirz;
            }
            else
            {
                tzmin = (box.Max.Z - origin.Z) * invdirz;
                tzmax = (box.Min.Z - origin.Z) * invdirz;
            }

            if ((tmin > tzmax) || (tzmin > tmax))
            {
                return null;
            }

            if (tzmin > tmin || tmin != tmin)
            {
                tmin = tzmin;
            }

            if (tzmax < tmax || tmax != tmax)
            {
                tmax = tzmax;
            }

            //return point closest to the ray (positive side)
            if (tmax < 0)
            {
                return null;
            }

            return this.At(tmin >= 0 ? tmin : tmax, target);
        }


        public bool IntersectsBox(Box3 box)
        {
            return this.IntersectBox(box, new Vector3()) != null;
        }

        public Vector3 IntersectTriangle(Vector3 a, Vector3 b, Vector3 c, bool backfaceCulling, Vector3 target)
        {
            // Compute the offset origin, edges, and normal.
            // from http://www.geometrictools.com/GTEngine/Include/Mathematics/GteIntrRay3Triangle3.h

            var edge1 = new Vector3().SubVectors(b, a);
            var edge2 = new Vector3().SubVectors(c, a);
            var normal = new Vector3().CrossVectors(edge1, edge2);

            // Solve Q + t*D = b1*E1 + b2*E2 (Q = kDiff, D = ray direction,
            // E1 = kEdge1, E2 = kEdge2, N = Cross(E1,E2)) by
            //   |Dot(D,N)|*b1 = sign(Dot(D,N))*Dot(D,Cross(Q,E2))
            //   |Dot(D,N)|*b2 = sign(Dot(D,N))*Dot(D,Cross(E1,Q))
            //   |Dot(D,N)|*t = -sign(Dot(D,N))*Dot(Q,N)
            var DdN = this.Direction.Dot(normal);
            float sign;

            if (DdN > 0)
            {
                if (backfaceCulling) { return null; }
                sign = 1;
            }
            else if (DdN < 0)
            {
                sign = -1;
                DdN = -DdN;
            }
            else
            {
                return null;
            }

            var diff = new Vector3().SubVectors(this.Origin, a);
            var DdQxE2 = sign * this.Direction.Dot(edge2.CrossVectors(diff, edge2));

            // b1 < 0, no intersection
            if (DdQxE2 < 0)
            {
                return null;
            }

            var DdE1xQ = sign * this.Direction.Dot(edge1.Cross(diff));

            // b2 < 0, no intersection
            if (DdE1xQ < 0)
            {
                return null;
            }

            // b1+b2 > 1, no intersection
            if (DdQxE2 + DdE1xQ > DdN)
            {
                return null;
            }

            // Line intersects triangle, check if ray does.
            var QdN = -sign * diff.Dot(normal);

            // t < 0, no intersection
            if (QdN < 0)
            {
                return null;
            }

            // Ray intersects triangle.
            return this.At(QdN / DdN, target);
        }

        public Ray LookAt(Vector3 target)
        {
            this.Direction.Copy(target).Sub(this.Origin).Normalize();
            return this;
        }

        public Ray Recast(float t)
        {
            var v1 = new Vector3();
            this.Origin.Copy(this.At(t, v1));
            return this;
        }

        public Ray Set(Vector3 origin, Vector3 direction)
        {
            this.Origin.Copy(origin);
            this.Direction.Copy(direction);

            return this;
        }
        #endregion
    }
}
