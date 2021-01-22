using System;
using System.Collections.Generic;
using ThreeJs4Net.Core;

namespace ThreeJs4Net.Math
{
    public class Box3
    {
        public static Box3 Empty()
        {
            return new Box3(Vector3.Infinity(), Vector3.NegativeInfinity());
        }

        public Vector3 Min = new Vector3();
        public Vector3 Max = new Vector3();

        /// <summary>
        /// 
        /// </summary>
        public Box3()
        {
            this.Copy(Box3.Empty());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Box3(Vector3 min, Vector3 max)
        {
            this.Min.Copy(min);
            this.Max.Copy(max);
        }

        public Box3 Set(Vector3 min, Vector3 max)
        {
            this.Min.Copy(min);
            this.Max.Copy(max);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public void MakeEmpty()
        {
            this.Min.X = this.Min.Y = this.Min.Z = float.PositiveInfinity;
            this.Max.X = this.Max.Y = this.Max.Z = float.NegativeInfinity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Box3 ExpandByPoint(Vector3 point)
        {
            this.Min.Min(point);
            this.Max.Max(point);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Box3 SetFromPoints(List<Vector3> points)
        {
            this.MakeEmpty();

            foreach (Vector3 t in points)
            {
                this.ExpandByPoint(t);
            }

            return this;
        }

        public bool IsEmpty()
        {
            // this is a more robust check for empty than ( volume <= 0 ) because volume can get positive with two negative axes
            return (Max.X < Min.X) || (Max.Y < Min.Y) || (Max.Z < Min.Z);
        }

        public Vector3 GetCenter(Vector3 target = null)
        {
            if (target == null)
                target = new Vector3();

            if (this.IsEmpty())
            {
                target.Set(0, 0, 0);
            }
            else
            {
                target.AddVectors(this.Min, this.Max).MultiplyScalar((float)0.5);
            }

            return target;
        }


        public Box3 SetFromArray(float[] array)
        {
            var minX = float.PositiveInfinity;
            var minY = float.PositiveInfinity;
            var minZ = float.PositiveInfinity;

            var maxX = float.NegativeInfinity;
            var maxY = float.NegativeInfinity;
            var maxZ = float.NegativeInfinity;

            for (int i = 0; i < array.Length; i += 3)
            {
                var x = array[i];
                var y = array[i + 1];
                var z = array[i + 2];

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (z < minZ) minZ = z;

                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                if (z > maxZ) maxZ = z;
            }

            this.Min.Set(minX, minY, minZ);
            this.Max.Set(maxX, maxY, maxZ);

            return this;
        }

        public Box3 SetFromCenterAndSize(Vector3 center, Vector3 size)
        {
            var halfSize = new Vector3().Copy(size).MultiplyScalar((float)0.5);

            this.Min.Copy(center).Sub(halfSize);
            this.Max.Copy(center).Add(halfSize);

            return this;
        }

        public Box3 Clone()
        {
            return new Box3().Copy(this);
        }

        public Box3 Copy(Box3 box)
        {
            this.Min.Copy(box.Min);
            this.Max.Copy(box.Max);

            return this;
        }

        public Box3 SetFromPoints(IEnumerable<Vector3> points)
        {
            this.MakeEmpty();

            foreach (var p in points)
            {
                this.ExpandByPoint(p);
            }

            return this;
        }

        public static Box3 FromCenterAndSize(Vector3 center, Vector3 size)
        {
            var b = Box3.Empty();
            var halfSize = size;
            halfSize.MultiplyScalar(0.5f);

            b.Min.SubVectors(center, halfSize);
            b.Max.AddVectors(center, halfSize);
            return b;
        }

        public Box3 SetFromObject(Object3D o)
        {
            this.MakeEmpty();
            return this.ExpandByObject(o);
        }

        public Box3 ExpandByObject(Object3D o)
        {
            // Computes the world-axis-aligned bounding box of an object (including its children),
            // accounting for both the object's, and children's, world transforms
            o.UpdateMatrixWorld(false);   //  object.updateWorldMatrix( false, false );

            var box = Box3.Empty();
            var geometry = o.Geometry;

            if (geometry != null)
            {
                if (geometry.BoundingBox == Box3.Empty())
                {
                    geometry.ComputeBoundingBox();
                }
                box.Copy(geometry.BoundingBox);
                box.ApplyMatrix4(o.MatrixWorld);
                this.Union(box);
            }
            foreach (var child in o.Children)
            {
                this.ExpandByObject(child);
            }
            return this;
        }

        public Vector3 GetSize(Vector3 target = null)
        {
            if (target == null)
                target = new Vector3();

            if (this.IsEmpty())
            {
                target.Set(0, 0, 0);
            }
            else
            {
                target.SubVectors(this.Max, this.Min);
            }

            return target;
        }

        public Box3 ExpandByVector(Vector3 vector)
        {
            Min.Sub(vector);
            Max.Add(vector);

            return this;
        }

        public Box3 ExpandByScalar(float scalar)
        {
            Min.AddScalar(-scalar);
            Max.AddScalar(scalar);

            return this;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return !(point.X < this.Min.X || point.X > this.Max.X ||
                    point.Y < this.Min.Y || point.Y > this.Max.Y ||
                    point.Z < this.Min.Z || point.Z > this.Max.Z);
        }

        public bool ContainsBox(Box3 box)
        {
            return (this.Min.X <= box.Min.X) && (box.Max.X <= this.Max.X) &&
                   (this.Min.Y <= box.Min.Y) && (box.Max.Y <= this.Max.Y) &&
                   (this.Min.Z <= box.Min.Z) && (box.Max.Z <= this.Max.Z);
        }

        //Returns point as a proportion of this box's width and height.
        public Vector3 GetParameter(Vector3 point, Vector3 target = null)
        {
            if (target == null)
                target = new Vector3();

            // This can potentially have a divide by zero if the box
            // has a size dimension of 0.
            var x = (point.X - this.Min.X) / (this.Max.X - this.Min.X);
            var y = (point.Y - this.Min.Y) / (this.Max.Y - this.Min.Y);
            var z = (point.Z - this.Min.Z) / (this.Max.Z - this.Min.Z);

            return target.Set(x, y, z);
        }

        public bool IntersectsBox(Box3 box)
        {
            // using 6 splitting planes to rule out intersections.
            return !(box.Max.X < this.Min.X || box.Min.X > this.Max.X ||
                   box.Max.Y < this.Min.Y || box.Min.Y > this.Max.Y ||
                   box.Max.Z < this.Min.Z || box.Min.Z > this.Max.Z);
        }

        public bool IntersectsSphere(Sphere sphere)
        {
            // Find the point on the AABB closest to the sphere center.
            var vector = this.ClampPoint(sphere.Center);

            // If that point is inside the sphere, the AABB and sphere intersect.
            return vector.DistanceToSquared(sphere.Center) <= (sphere.Radius * sphere.Radius);
        }

        public bool IntersectsPlane(Plane plane)
        {
            // We compute the minimum and maximum dot product values. If those values
            // are on the same side (back or front) of the plane, then there is no intersection.
            float min, max;

            if (plane.Normal.X > 0)
            {
                min = plane.Normal.X * this.Min.X;
                max = plane.Normal.X * this.Max.X;
            }
            else
            {

                min = plane.Normal.X * this.Max.X;
                max = plane.Normal.X * this.Min.X;
            }

            if (plane.Normal.Y > 0)
            {
                min += plane.Normal.Y * this.Min.Y;
                max += plane.Normal.Y * this.Max.Y;
            }
            else
            {
                min += plane.Normal.Y * this.Max.Y;
                max += plane.Normal.Y * this.Min.Y;
            }

            if (plane.Normal.Z > 0)
            {
                min += plane.Normal.Z * this.Min.Z;
                max += plane.Normal.Z * this.Max.Z;
            }
            else
            {
                min += plane.Normal.Z * this.Max.Z;
                max += plane.Normal.Z * this.Min.Z;
            }

            return (min <= -plane.Constant && max >= -plane.Constant);
        }


        public Vector3 ClampPoint(Vector3 point, Vector3 target = null)
        {
            if (target == null)
                target = new Vector3();

            return target.Copy(point).Clamp(this.Min, this.Max);
        }

        public double DistanceToPoint(Vector3 point)
        {
            var clampedPoint = new Vector3().Copy(point).Clamp(this.Min, this.Max);
            return clampedPoint.Sub(point).Length();
        }

        public Sphere GetBoundingSphere(Sphere target)
        {
            this.GetCenter(target.Center);
            target.Radius = this.GetSize().Length() * (float)0.5;
            return target;
        }

        public Box3 Intersect(Box3 box)
        {
            Min.Max(box.Min);
            Max.Min(box.Max);

            if (this.IsEmpty())
            {
                this.MakeEmpty();
            }

            return this;
        }

        public Box3 Union(Box3 box)
        {
            Min.Min(box.Min);
            Max.Max(box.Max);

            return this;
        }

        public Box3 ApplyMatrix4(Matrix4 matrix)
        {
            // transform of empty box is an empty box.
            if (this.IsEmpty())
            {
                return this;
            }

            // NOTE: I am using a binary pattern to specify all 2^3 combinations below
            var points = new List<Vector3>()
            {
                new Vector3(Min.X, Min.Y, Min.Z).ApplyMatrix4(matrix), // 000
                new Vector3(Min.X, Min.Y, Max.Z).ApplyMatrix4(matrix), // 001
                new Vector3(Min.X, Max.Y, Min.Z).ApplyMatrix4(matrix), // 010
                new Vector3(Min.X, Max.Y, Max.Z).ApplyMatrix4(matrix), // 011
                new Vector3(Max.X, Min.Y, Min.Z).ApplyMatrix4(matrix), // 100
                new Vector3(Max.X, Min.Y, Max.Z).ApplyMatrix4(matrix), // 101
                new Vector3(Max.X, Max.Y, Min.Z).ApplyMatrix4(matrix), // 110
                new Vector3(Max.X, Max.Y, Max.Z).ApplyMatrix4(matrix), // 111
            };

            this.SetFromPoints(points);

            return this;
        }

        public Box3 Translate(Vector3 offset)
        {
            Min.Add(offset);
            Max.Add(offset);

            return this;
        }

    }
}
