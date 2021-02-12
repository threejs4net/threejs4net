using System;
using System.Collections.Generic;
using System.Diagnostics;
using ThreeJs4Net.Core;

namespace ThreeJs4Net.Math
{
    [DebuggerDisplay("Min ({Min}), Max ({Max})")]
    public class Box2 : IEquatable<Box2>
    {
        private Vector2 __vector = new Vector2();

        public Vector2 Min = new Vector2();
        public Vector2 Max = new Vector2();

        public Box2()
        {
            this.Min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            this.Max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        }

        public Box2(Vector2 min, Vector2 max)
        {
            this.Min = min.Clone();
            this.Max = max.Clone();
        }

        #region --- Already in R116 ---
        public Vector2 ClampPoint(Vector2 point, Vector2 target)
        {
            return target.Copy(point).Clamp(this.Min, this.Max);
        }

        public Box2 Clone()
        {
            return new Box2().Copy(this);
        }

        public Box2 Copy(Box2 box)
        {
            this.Min.Copy(box.Min);
            this.Max.Copy(box.Max);

            return this;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return !(point.X < this.Min.X) 
                   && !(point.X > this.Max.X) 
                   && !(point.Y < this.Min.Y) 
                   && !(point.Y > this.Max.Y);
        }

        public bool ContainsBox(Box2 box)
        {
            return this.Min.X <= box.Min.X && box.Max.X <= this.Max.X &&
                   this.Min.Y <= box.Min.Y && box.Max.Y <= this.Max.Y;
        }

        public float DistanceToPoint(Vector2 point)
        {
            var clampedPoint = __vector.Copy(point).Clamp(this.Min, this.Max);
            return clampedPoint.Sub(point).Length();
        }

        public Box2 ExpandByPoint(Vector2 point)
        {
            this.Min.Min(point);
            this.Max.Max(point);

            return this;
        }

        public Box2 ExpandByVector(Vector2 vector)
        {
            this.Min.Sub(vector);
            this.Max.Add(vector);

            return this;
        }

        public Box2 ExpandByScalar(float scalar)
        {
            this.Min.AddScalar(-scalar);
            this.Max.AddScalar(scalar);

            return this;
        }

        public bool Equals(Box2 box)
        {
            return box.Min.Equals(this.Min) && box.Max.Equals(this.Max);
        }

        public Box2 Intersect(Box2 box)
        {
            this.Min.Max(box.Min);
            this.Max.Min(box.Max);

            return this;
        }

        public Vector2 GetParameter(Vector2 point, Vector2 target)
        {
            // This can potentially have a divide by zero if the box
            // has a size dimension of 0.
            return target.Set(
                (point.X - this.Min.X) / (this.Max.X - this.Min.X),
                (point.Y - this.Min.Y) / (this.Max.Y - this.Min.Y)
            );
        }

        public Vector2 GetCenter(Vector2 target)
        {
            return this.IsEmpty() ? target.Set(0, 0) : target.AddVectors(this.Min, this.Max).MultiplyScalar((float)0.5);
        }

        public Vector2 GetSize(Vector2 target)
        {
            return this.IsEmpty() ? target.Set(0, 0) : target.SubVectors(this.Max, this.Min);
        }

        public bool IntersectsBox(Box2 box)
        {
            // using 4 splitting planes to rule out intersections
            return !(box.Max.X < this.Min.X)
                   && !(box.Min.X > this.Max.X)
                   && !(box.Max.Y < this.Min.Y)
                   && !(box.Min.Y > this.Max.Y);
        }

        public bool IsEmpty()
        {
            // this is a more robust check for empty than ( volume <= 0 ) because volume can get positive with two negative axes
            return (this.Max.X < this.Min.X) || (this.Max.Y < this.Min.Y);
        }

        public Box2 MakeEmpty()
        {
            this.Min.X = this.Min.Y = float.PositiveInfinity;
            this.Max.X = this.Max.Y = float.NegativeInfinity;

            return this;
        }

        public Box2 Set(Vector2 min, Vector2 max)
        {
            this.Min.Copy(min);
            this.Max.Copy(max);

            return this;
        }

        public Box2 SetFromPoints(IEnumerable<Vector2> points)
        {
            this.MakeEmpty();

            foreach (var point in points)
            {
                this.ExpandByPoint(point);
            }

            return this;
        }

        public Box2 SetFromCenterAndSize(Vector2 center, Vector2 size)
        {
            var halfSize = __vector.Copy(size).MultiplyScalar((float)0.5);
            this.Min.Copy(center).Sub(halfSize);
            this.Max.Copy(center).Add(halfSize);

            return this;
        }

        public Box2 Translate(Vector2 offset)
        {
            this.Min.Add(offset);
            this.Max.Add(offset);

            return this;
        }

        public Box2 Union(Box2 box)
        {
            this.Min.Min(box.Min);
            this.Max.Max(box.Max);

            return this;
        }
        #endregion
    }
}

