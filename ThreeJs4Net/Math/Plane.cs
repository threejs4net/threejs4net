using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Math
{
    public class Plane 
    {

        public Vector3 Normal = new Vector3(1, 0, 0);
        public float Constant = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        public Plane Set(Vector3 normal, float constant)
        {
            this.Normal.Copy(normal);
            this.Constant = constant;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Plane SetComponents(float x, float y, float z, float w)
        {
            this.Normal.Set(x, y, z);
            this.Constant = w;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public Plane SetFromNormalAndCoplanarPoint(Vector3 normal, Vector3 point)
        {
            this.Normal.Copy(Normal);
            this.Constant -= point.Dot(this.Normal); // must be this.normal, not normal, as this.normal is normalized
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="V1"></param>
        /// <param name="V2"></param>
        /// <param name="V3"></param>
        /// <returns></returns>
        public Plane SetFromCoplanarPoints(Vector3 V1, Vector3 V2, Vector3 V3)
        {
            var V1B = new Vector3();
            var V2B = new Vector3();

            var Normal = V1B.SubVectors(V3, V2).Cross(V2B.SubVectors(V1, V2)).Normalize();
            this.SetFromNormalAndCoplanarPoint(Normal, V1);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public Plane Copy(Plane plane)
        {
            this.Normal.Copy(plane.Normal);
            this.Constant = plane.Constant;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Plane Normalize()
        {
            var inverseNormalLength = 1.0f / this.Normal.Length();
            this.Normal.MultiplyScalar(inverseNormalLength);
            this.Constant *= inverseNormalLength;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Plane Negate()
        {
            this.Constant *= -1;
            this.Normal.Negate();
            return this;
        }

        public float DistanceToPoint(Vector3 point)
        {
            return this.Normal.Dot(point) + this.Constant;
        }

        public float DistanceToSphere(Sphere sphere)
        {
            return this.DistanceToPoint(sphere.Center) - sphere.Radius;
        }

        public Vector3 ProjectPoint(Vector3 point, Vector3 target = null)
        {
            return (this.OrthoPoint(point, target) - point).Negate();
        }

        public Vector3 OrthoPoint(Vector3 point, Vector3 target = null)
        {
            var perpendicularMagnitude = this.DistanceToPoint(point);

            var result = target ?? new Vector3();
            return result.Copy(this.Normal).MultiplyScalar(perpendicularMagnitude);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsIntersectionLine(Line line)
        {
            var startSign = this.DistanceToPoint(line.Start);
            var endSign = this.DistanceToPoint(line.End);

            return (startSign < 0 && endSign > 0) || (endSign < 0 && startSign > 0);
        }

        public Vector3 IntersectLine(Line line, Vector3 target = null)
        {
            // Vector3 V1 = new Vector3();
            // Vector3 Result = Target ?? new Vector3();
            //TODO: Where the fuck is Line.Delta
            // Vector3 Direction = Line.Delta(V1);
            // float Denominator = this.Normal.Dot(Direction);

            // if (Denominator == 0) {
            //     if (this.DistanceToPoint(Line.Start) == 0) {
            //         return Result.Copy(Line.Start);
            //     }
            //     Trace.TraceWarning("Uhm wat");
            //     return null;
            // }

            // float T = (Line.Start.Dot(this.Normal) + this.Constant) / Denominator;
            // if (T < 0 || T > 1) {
            //     return null;
            // }
            // return Result.Copy(Direction).MultiplyScalar(T) + Line.Start;
            return new Vector3();
        }

        public Vector3 CoplanarPoint(Vector3 target = null)
        {
            var result = target ?? new Vector3();
            return result.Copy(this.Normal).MultiplyScalar(-this.Constant);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="normalMatrix"></param>
        /// <returns></returns>
        public Plane ApplyMatrix4(Matrix4 matrix, Matrix3 normalMatrix = null)
        {
            var v1 = new Vector3();
            var v2 = new Vector3();
            var m1 = new Matrix3();

            var normalM = normalMatrix ?? m1.GetNormalMatrix(matrix);
            var newNormal = v1.Copy(this.Normal).ApplyMatrix3(normalM);
            var newCoplanarPoint = this.CoplanarPoint(v2);
            newCoplanarPoint.ApplyMatrix4(matrix);

            this.SetFromNormalAndCoplanarPoint(newNormal, newCoplanarPoint);
            return this;
        }

        public Plane Translate(Vector3 offset)
        {
            this.Constant = this.Constant - offset.Dot(this.Normal);
            return this;
        }

        public bool Equals(Plane plane)
        {
            return plane.Normal.Equals(this.Normal) && (plane.Constant == this.Constant);
        }

        public Plane Clone()
        {
            return new Plane().Copy(this);
        }

    }

}
