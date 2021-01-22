using System;

namespace ThreeJs4Net.Math
{
    public class Ray : ICloneable, IEquatable<Ray>
    {
        public Vector3 Origin = new Vector3();

        public Vector3 Direction = new Vector3();

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public Ray Copy (Ray another )
        {
            this.Origin.Copy(another.Origin);
            this.Direction.Copy(another.Direction);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public Vector3 at(float t, Vector3 optionalTarget) 
        {
            var result = optionalTarget ?? new Vector3();
            return result.Copy(this.Direction).MultiplyScalar(t).Add(this.Origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Ray Recast (float t) 
        {
             var v1 = new Vector3();
             this.Origin.Copy(this.at(t, v1));
             return this;
        }

        public Vector3 ClosestPointToPoint(Vector3 point, Vector3 optionalTarget) 
        {
            var result = optionalTarget ?? new Vector3();

            result.SubVectors(point, this.Origin);
            var directionDistance = result.Dot(this.Direction);
            if (directionDistance < 0)
            {
                return result.Copy(this.Origin);
            }
            return result.Copy(this.Direction).MultiplyScalar(directionDistance).Add(this.Origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public float DistanceToPoint (Vector3 point) 
        {
            var v1 = Vector3.One();

            var directionDistance = v1.SubVectors(point, this.Origin).Dot(this.Direction);

            // point behind the ray

            if (directionDistance < 0)
            {
                return this.Origin.DistanceTo(point);
            }

            v1.Copy(this.Direction).MultiplyScalar(directionDistance).Add(this.Origin);

            return v1.DistanceTo(point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="optionalPointOnRay"></param>
        /// <param name="optionalPointOnSegment"></param>
        /// <returns></returns>
        public float DistanceSqToSegment(Vector3 v0, Vector3 v1, Vector3 optionalPointOnRay, Vector3 optionalPointOnSegment)
        {
            // from http://www.geometrictools.com/LibMathematics/Distance/Wm5DistRay3Segment3.cpp
            // It returns the min distance between the ray and the segment
            // defined by v0 and v1
            // It can also Set two optional targets :
            // - The closest point on the ray
            // - The closest point on the segment

            var segCenter = ((Vector3)v0.Clone()).Add(v1).MultiplyScalar(0.5f);
            var segDir = ((Vector3)v1.Clone()).Sub(v0).Normalize();
            var segExtent = v0.DistanceTo(v1) * 0.5f;
            var diff = ((Vector3)this.Origin.Clone()).Sub(segCenter);
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
            if (null != optionalPointOnRay)
            {
                optionalPointOnRay.Copy(((Vector3)this.Direction.Clone()).MultiplyScalar(s0).Add(this.Origin));
            }

            if (null != optionalPointOnSegment)
            {
                optionalPointOnSegment.Copy(((Vector3)segDir.Clone()).MultiplyScalar(s1).Add(segCenter));
            }

            return sqrDist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public bool IsIntersectionSphere (Sphere sphere ) {
            return (this.DistanceToPoint( sphere.Center ) <= sphere.Radius);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public bool IntersectSphere(Sphere sphere, Vector3 optionalTarget )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public bool IsIntersectionPlane (Plane plane )
        {
               throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public float DistanceToPlane(Plane plane)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public bool IntersectPlane(Plane plane, Vector3 optionalTarget)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool IsIntersectionBox(Box3 box)
        {
            return this.IntersectBox( box, new Vector3() ) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public Vector3 IntersectBox(Box3 box, Vector3 optionalTarget)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="backfaceCulling"></param>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public Vector3 IntersectTriangle(Vector3 a, Vector3 b, Vector3 c, bool backfaceCulling, Vector3 optionalTarget = null)
        {
            // Compute the offset origin, edges, and normal.
         //   var diff = new Vector3();
            //var edge1 = new Vector3();
            //var edge2 = new Vector3();
            var normal = new Vector3();

            // from http://www.geometrictools.com/LibMathematics/Intersection/Wm5IntrRay3Triangle3.cpp
            //edge1.SubVectors(B, A);
            //edge2.SubVectors(C, A);
            //normal.CrossVectors(edge1, edge2);

            var edge1 = b - a;
            var edge2 = c - a;
            normal.CrossVectors(edge1, edge2);

            // Solve Q + t*D = b1*E1 + b2*E2 (Q = kDiff, D = ray direction,
            // E1 = kEdge1, E2 = kEdge2, N = Cross(E1,E2)) by
            // |Dot(D,N)|*b1 = sign(Dot(D,N))*Dot(D,Cross(Q,E2))
            // |Dot(D,N)|*b2 = sign(Dot(D,N))*Dot(D,Cross(E1,Q))
            // |Dot(D,N)|*t = -sign(Dot(D,N))*Dot(Q,N)
            var DdN = this.Direction.Dot(normal);
            int sign;

            if (DdN > 0)
            {
                if (backfaceCulling) return null;
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

      //      diff.SubVectors(this.Origin, A);

            var diff = this.Origin - a;

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
            return this.at(QdN / DdN, optionalTarget);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix4"></param>
        /// <returns></returns>
        public Ray ApplyMatrix4(Matrix4 matrix4)
        {
            this.Direction.Add(this.Origin).ApplyMatrix4(matrix4);
            this.Origin.ApplyMatrix4(matrix4);
            this.Direction.Sub(this.Origin);
            this.Direction.Normalize();

            return this; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Ray().Copy(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Ray other)
        {
            return other.Origin.Equals(this.Origin) && other.Direction.Equals(this.Direction);
        }
    }
}
