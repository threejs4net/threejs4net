using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Math
{
    public class Plane
    {

        public Vector3 Normal;
        public float Constant;

        public Plane(Vector3? normal = null, float constant = 0)
        {
            this.Normal = normal != null ? normal.Clone() : new Vector3(1, 0, 0);
            this.Constant = constant;
        }


        #region --- Already in R116 ---

        public Plane Clone()
        {
            return new Plane().Copy(this);
        }

        public Plane Copy(Plane plane)
        {
            this.Normal.Copy(plane.Normal);
            this.Constant = plane.Constant;

            return this;
        }

        public Plane Set(Vector3 normal, float constant)
        {
            this.Normal.Copy(normal);
            this.Constant = constant;

            return this;
        }

        public Plane SetComponents(float x, float y, float z, float w)
        {
            this.Normal.Set(x, y, z);
            this.Constant = w;
            return this;
        }

        public Plane SetFromNormalAndCoplanarPoint(Vector3 normal, Vector3 point)
        {
            this.Normal.Copy(normal);
            this.Constant = -point.Dot(this.Normal); // must be this.normal, not normal, as this.normal is normalized
            return this;
        }

        public Plane SetFromCoplanarPoints(Vector3 a, Vector3 b, Vector3 c)
        {
            var vector1 = new Vector3();
            var vector2 = new Vector3();
            var normal = vector1.SubVectors(c, b).Cross(vector2.SubVectors(a, b)).Normalize();

            // Q: should an error be thrown if normal is zero (e.g. degenerate plane)?
            this.SetFromNormalAndCoplanarPoint(normal, a);

            return this;
        }

        public Plane Normalize()
        {
            var inverseNormalLength = 1.0f / this.Normal.Length();
            this.Normal.MultiplyScalar(inverseNormalLength);
            this.Constant *= inverseNormalLength;
            return this;
        }

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

        public Vector3 ProjectPoint(Vector3 point, Vector3 target)
        {
            return target.Copy(this.Normal).MultiplyScalar(-this.DistanceToPoint(point)).Add(point);
        }

        public Vector3 IntersectLine(Line3 line, Vector3 target)
        {
            var direction = line.Delta(new Vector3());
            var denominator = this.Normal.Dot(direction);

            if (denominator == 0)
            {
                // line is coplanar, return origin
                if (this.DistanceToPoint(line.Start) == 0)
                {
                    return target.Copy(line.Start);
                }

                // Unsure if this is the correct method to handle this case.
                return null;
            }

            var t = -(line.Start.Dot(this.Normal) + this.Constant) / denominator;
            if (t < 0 || t > 1)
            {
                return null;
            }

            return target.Copy(direction).MultiplyScalar(t).Add(line.Start);
        }

        public bool IntersectsLine(Line3 line)
        {
            // Note: this tests if a line intersects the plane, not whether it (or its end-points) are coplanar with it.
            var startSign = this.DistanceToPoint(line.Start);
            var endSign = this.DistanceToPoint(line.End);

            return (startSign < 0 && endSign > 0) || (endSign < 0 && startSign > 0);
        }

        public bool IntersectsBox(Box3 box)
        {
            return box.IntersectsPlane(this);
        }

        public bool IntersectsSphere(Sphere sphere)
        {
            return sphere.IntersectsPlane(this);
        }

        public Vector3 CoplanarPoint(Vector3 target)
        {
            return target.Copy(this.Normal).MultiplyScalar(-this.Constant);
        }

        public Plane ApplyMatrix4(Matrix4 matrix, Matrix3 normalMatrix = null)
        {
            var v1 = new Vector3();
            normalMatrix ??= new Matrix3().GetNormalMatrix(matrix);
            var referencePoint = this.CoplanarPoint(v1).ApplyMatrix4(matrix);
            var normal = this.Normal.ApplyMatrix3(normalMatrix).Normalize();
            this.Constant = -referencePoint.Dot(normal);

            return this;
        }

        public Plane Translate(Vector3 offset)
        {
            this.Constant -= offset.Dot(this.Normal);
            return this;
        }
        #endregion



        public bool Equals(Plane plane)
        {
            return plane != null && plane.Normal.Equals(this.Normal) && (plane.Constant == this.Constant);
        }
    }

}
