using System;

namespace ThreeJs4Net.Math
{
    public class Line3 : ICloneable, IEquatable<Line3>
    {
        public Vector3 Start;

        public Vector3 End;

        /// <summary>
        /// 
        /// </summary>
        public Line3()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Line3(Vector3 start, Vector3 end)
        {
            this.Start.Copy(start);
            this.End.Copy(end);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Line3 Copy(Line3 other)
        {
            this.Start.Copy(other.Start);
            this.End.Copy(other.End);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public Vector3 Center(Vector3 optionalTarget = null)
        {
            var result = optionalTarget ?? new Vector3();
            return result.AddVectors(this.Start, this.End).MultiplyScalar(0.5f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public Vector3 delta(Vector3 optionalTarget = null)
        {
            var result = optionalTarget ?? new Vector3();
            return result.SubVectors(this.End, this.Start);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float distanceSq()
        {
            return this.Start.DistanceToSquared(this.End);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float distance()
        {
            return this.Start.DistanceTo(this.End);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 at(float t, Vector3 optionalTarget = null)
        {
            var result = optionalTarget ?? new Vector3();
            return this.delta(result).MultiplyScalar(t).Add(this.Start);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float closestPointToPointParameter(Vector3 point, bool clampToLine )
        {
            var startP = new Vector3();
            var startEnd = new Vector3();

            startP.SubVectors(point, this.Start);
            startEnd.SubVectors(this.End, this.Start);
            var startEnd2 = startEnd.Dot(startEnd);
            var startEnd_startP = startEnd.Dot(startP);
            var t = startEnd_startP / startEnd2;
            if (clampToLine)
            {
                t.Clamp(0, 1);
            }

            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 closestPointToPoint(Vector3 point, bool clampToLine, Vector3 optionalTarget = null)
        {
            var t = this.closestPointToPointParameter(point, clampToLine); 
            var result = optionalTarget ?? new Vector3();
            return this.delta(result).MultiplyScalar(t).Add(this.Start);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Line3 applyMatrix4(Matrix4 matrix)
        {
            this.Start.ApplyMatrix4(matrix);
            this.End.ApplyMatrix4(matrix);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Line3().Copy(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Line3 other)
        {
            return other.Start.Equals(this.Start) && other.End.Equals(this.End);
        }
    }
}
