using System;
using System.Collections.Generic;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Core
{
    public abstract class Curve<T> where T : IVector<T>
    {
        protected int arcLengthDivisions = 200;
        protected List<float> cacheArcLengths = null;
        protected bool needsUpdate = false;

        public abstract T GetPoint(float t, T optionalTarget);

        public virtual T GetPointAt(float u, T optionalTarget)
        {
            var t = this.GetUtoTmapping(u);
            return this.GetPoint(t, optionalTarget);
        }

        public List<T> GetPoints(int divisions = 5)
        {
            var points = new List<T>();
            for (float d = 0; d <= divisions; d++)
            {
                points.Add(this.GetPoint(d / divisions, default(T)));
            }

            return points;
        }

        public List<T> GetSpacedPoints(int divisions = 5)
        {
            var points = new List<T>();

            for (var d = 0; d <= divisions; d++)
            {
                points.Add(this.GetPointAt((float)d / divisions, default(T)));
            }

            return points;
        }

        public float GetLength()
        {
            var lengths = this.GetLengths();
            return lengths[lengths.Count - 1];
        }

        public List<float> GetLengths(int? divisions = null)
        {
            int lDivisions = divisions ?? this.arcLengthDivisions;

            if (this.cacheArcLengths != null && (this.cacheArcLengths.Count == lDivisions + 1) && !this.needsUpdate)
            {
                return this.cacheArcLengths;
            }

            this.needsUpdate = false;

            var cache = new List<float>();

            T current;
            T last = this.GetPoint(0, default(T));
            float sum = 0;

            cache.Add(0);

            for (float p = 1; p <= lDivisions; p++)
            {
                current = this.GetPoint(p / lDivisions, default(T));
                if (current is Vector2 v2)
                {
                    sum += v2.DistanceTo(last as Vector2);
                }
                else if (current is Vector3 v3)
                {
                    sum += v3.DistanceTo(last as Vector3);
                }

                cache.Add(sum);
                last = current;
            }

            this.cacheArcLengths = cache;

            return cache; // { sums: cache, sum: sum }; Sum is in the last element.
        }

        public void UdateArcLengths()
        {
            this.needsUpdate = true;
            this.GetLengths();
        }

        // Given u ( 0 .. 1 ), get a t to find p. This gives you points which are equidistant

        #region --> GetUtoTmapping
        public float GetUtoTmapping(float u, float? distance = null)
        {
            var arcLengths = this.GetLengths();
            var i = 0;
            var il = arcLengths.Count;
            float targetArcLength; // The targeted u distance value to get

            if (distance != null && distance != 0)
            {
                targetArcLength = (float)distance;
            }
            else
            {
                targetArcLength = u * arcLengths[il - 1];
            }

            // binary search for the index with largest value smaller than target u distance
            int low = 0, high = il - 1;

            while (low <= high)
            {
                i = Mathf.Floor(low +
                               (high - low) /
                               2); // less likely to overflow, though probably not issue here, JS doesn't really have integers, all numbers are floats

                var comparison = arcLengths[i] - targetArcLength;
                if (comparison < 0)
                {
                    low = i + 1;
                }
                else if (comparison > 0)
                {
                    high = i - 1;
                }
                else
                {
                    high = i;
                    break;
                }
            }

            i = high;

            //if (Mathf.Abs(arcLengths[i]-targetArcLength) <= MathUtils.EPS5)
            if (arcLengths[i] == targetArcLength)
            {
                return i / (il - 1);
            }

            // we could get finer grain at lengths, or use simple interpolation between two points
            var lengthBefore = arcLengths[i];
            var lengthAfter = arcLengths[i + 1];
            var segmentLength = lengthAfter - lengthBefore;

            // determine where we are between the 'before' and 'after' points
            var segmentFraction = (targetArcLength - lengthBefore) / segmentLength;

            // add that fractional amount to t
            var t = (i + segmentFraction) / (il - 1);

            return t;
        } 
        #endregion

        // Returns a unit vector tangent at t
        // In case any sub curve does not implement its tangent derivation,
        // 2 points a small delta apart will be used to find its gradient
        // which seems to give a reasonable approximation

        #region --> GetTangent
        public virtual T GetTangent(float t, T optionalTarget)
        {
            float delta = (float)0.0001;
            float t1 = t - delta;
            float t2 = t + delta;

            // Capping in case of danger

            if (t1 < 0) t1 = 0;
            if (t2 > 1) t2 = 1;

            var pt1 = this.GetPoint(t1, default(T));
            var pt2 = this.GetPoint(t2, default(T));

            T tangent = optionalTarget != null ? optionalTarget : default(T);

            if (tangent is Vector2 tangentv2)
            {
                tangentv2.Copy(pt2 as Vector2).Sub(pt1 as Vector2).Normalize();
            }

            if (tangent is Vector3 tangentv3)
            {
                tangentv3.Copy(pt2 as Vector3).Sub(pt1 as Vector3).Normalize();
            }

            return tangent;
        }
        #endregion

        #region --> GetTangentAt
        public T GetTangentAt(float u, T optionalTarget)
        {
            var t = this.GetUtoTmapping(u, null);
            return this.GetTangent(t, optionalTarget);
        }
        #endregion

        #region ##> ComputeFrenetFrames
        public TenetFrames ComputeFrenetFrames(int segments, bool closed)
        {
            if (!(default(T) is Vector3))
            {
                throw new Exception("Method should be used for Vector3 only");
            }

            // see http://www.cs.indiana.edu/pub/techreports/TR425.pdf
            var normal = new Vector3();
            var tangents = new List<Vector3>();
            var normals = new List<Vector3>();
            var binormals = new List<Vector3>();

            var vec = new Vector3();
            var mat = new Matrix4();

            float i, u, theta;

            // compute the tangent vectors for each segment on the curve

            for (i = 0; i <= segments; i++)
            {
                u = i / segments;

                var tangent = this.GetTangentAt(u, default(T));
                (tangent as Vector3).Normalize();
                tangents.Add(tangent as Vector3);
            }

            // select an initial normal vector perpendicular to the first tangent vector,
            // and in the direction of the minimum tangent xyz component

            normals[0] = new Vector3();
            binormals[0] = new Vector3();
            var min = float.PositiveInfinity;

            var lTangent = tangents[0] as Vector3;

            var tx = Mathf.Abs(lTangent.X);
            var ty = Mathf.Abs(lTangent.Y);
            var tz = Mathf.Abs(lTangent.Z);

            if (tx <= min)
            {
                min = tx;
                normal.Set(1, 0, 0);
            }

            if (ty <= min)
            {
                min = ty;
                normal.Set(0, 1, 0);
            }

            if (tz <= min)
            {
                normal.Set(0, 0, 1);
            }

            vec.CrossVectors(lTangent, normal).Normalize();
            var lNormal = normals[0] as Vector3;
            var lBiNormal = binormals[0] as Vector3;

            lNormal.CrossVectors(lTangent, vec);
            lBiNormal.CrossVectors(lTangent, lNormal);

            // compute the slowly-varying normal and binormal vectors for each segment on the curve
            for (var idx = 1; idx <= segments; idx++)
            {
                normals[idx] = normals[idx - 1].Clone();
                binormals[idx] = binormals[idx - 1].Clone();
                vec.CrossVectors((Vector3)tangents[idx - 1], (Vector3)tangents[idx]);
                if (vec.Length() > float.Epsilon)
                {
                    vec.Normalize();
                    theta = Mathf.Acos(MathUtils.Clamp(((Vector3)tangents[idx - 1]).Dot((Vector3)tangents[idx]), -1, 1)); // clamp for floating pt errors
                    normals[idx].ApplyMatrix4(mat.MakeRotationAxis(vec, theta));
                }
                binormals[idx].CrossVectors((Vector3)tangents[idx], normals[idx]);
            }

            // if the curve is closed, postprocess the vectors so the first and last normal vectors are the same
            if (closed)
            {
                theta = Mathf.Acos(MathUtils.Clamp(normals[0].Dot(normals[segments]), -1, 1));
                theta /= segments;

                if (((Vector3)tangents[0]).Dot(vec.CrossVectors(normals[0], normals[segments])) > 0)
                {
                    theta = -theta;
                }

                for (var idx = 1; i <= segments; i++)
                {
                    // twist a little...
                    normals[idx].ApplyMatrix4(mat.MakeRotationAxis((Vector3)tangents[idx], theta * i));
                    binormals[idx].CrossVectors((Vector3)tangents[idx], normals[idx]);
                }
            }

            return new TenetFrames()
            {
                tangents = tangents,
                normals = normals,
                binormals = binormals
            };
        } 
        #endregion

        public virtual Curve<T> Clone()
        {
            //TODO: Test this
            return default(Curve<T>).Copy(this);
        }

        public virtual Curve<T> Copy(Curve<T> source)
        {
            this.arcLengthDivisions = source.arcLengthDivisions;
            return this;
        }

        public class TenetFrames
        {
            public List<Vector3> tangents;
            public List<Vector3> normals;
            public List<Vector3> binormals;

        }
    }
}