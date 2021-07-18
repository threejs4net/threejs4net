using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Curves
{
    public class LineCurve3 : Curve<Vector3>
    {
        private Vector3 v1;
        private Vector3 v2;

        public LineCurve3(Vector3 v1, Vector3 v2)
        {
            this.v1 = v1 ?? new Vector3();
            this.v2 = v2 ?? new Vector3();
        }

        public override Vector3 GetPoint(float t, Vector3 optionalTarget)
        {
            var point = optionalTarget ?? new Vector3();

            if (t == 1)
            {
                point.Copy(this.v2);
            }
            else
            {
                point.Copy(this.v2).Sub(this.v1);
                point.MultiplyScalar(t).Add(this.v1);
            }

            return point;
        }

        public override Vector3 GetPointAt(float u, Vector3 optionalTarget)
        {
            return this.GetPoint(u, optionalTarget);
        }

        public override Vector3 GetTangent(float t, Vector3 optionalTarget)
        {
            var tangent = optionalTarget ?? new Vector3();
            tangent = tangent.Copy(this.v2).Sub(this.v1).Normalize();
            return tangent;
        }

        public LineCurve3 Copy(LineCurve3 source)
        {
            base.Copy(source);

            this.v1.Copy(source.v1);
            this.v2.Copy(source.v2);

            return this;
        }
    }
}
