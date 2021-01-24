using System;

namespace ThreeJs4Net.Math
{
    public class Line3 : IEquatable<Line3>
    {
        public Vector3 Start = new Vector3();
        public Vector3 End = new Vector3();

        public Line3()
        {
        }

        public Line3(Vector3 start, Vector3 end)
        {
            this.Start.Copy(start);
            this.End.Copy(end);
        }


        #region --- Already in R116 ---
        public Line3 Set(Vector3 start, Vector3 end)
        {
            this.Start.Copy(start);
            this.End.Copy(end);

            return this;
        }

        public Line3 Copy(Line3 other)
        {
            this.Start.Copy(other.Start);
            this.End.Copy(other.End);

            return this;
        }

        public Line3 Clone()
        {
            return new Line3().Copy(this);
        }

        public Vector3 GetCenter(Vector3 target)
        {
            return target.AddVectors(this.Start, this.End).MultiplyScalar((float)0.5);
        }

        public Vector3 Delta(Vector3 target)
        {
            return target.SubVectors(this.End, this.Start);
        }

        public float DistanceSq()
        {
            return this.Start.DistanceToSquared(this.End);
        }

        public float Distance()
        {
            return this.Start.DistanceTo(this.End);
        }

        public Vector3 At(float t, Vector3 target)
        {
            return this.Delta(target).MultiplyScalar(t).Add(this.Start);
        }

        public float ClosestPointToPointParameter(Vector3 point, bool clampToLine)
        {
            var startP = new Vector3();
            var startEnd = new Vector3();

            startP.SubVectors(point, this.Start);
            startEnd.SubVectors(this.End, this.Start);

            var startEnd2 = startEnd.Dot(startEnd);
            var startEndstartP = startEnd.Dot(startP);
            var t = startEndstartP / startEnd2;
            if (clampToLine)
            {
                t = MathUtils.Clamp(t, 0, 1);
            }

            return t;
        }

        public Vector3 ClosestPointToPoint(Vector3 point, bool clampToLine, Vector3 target)
        {
            var t = this.ClosestPointToPointParameter(point, clampToLine);
            return this.Delta(target).MultiplyScalar(t).Add(this.Start);
        }
        
        public Line3 ApplyMatrix4(Matrix4 matrix)
        {
            this.Start.ApplyMatrix4(matrix);
            this.End.ApplyMatrix4(matrix);

            return this;
        }
        #endregion

        public bool Equals(Line3 other)
        {
            return other != null && other.Start.Equals(this.Start) && other.End.Equals(this.End);
        }
    }
}
