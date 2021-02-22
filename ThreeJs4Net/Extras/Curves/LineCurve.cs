using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Curves
{
    public class LineCurve : Curve<Vector2>
    {
        private Vector2 v1;
        private Vector2 v2;

        public LineCurve(Vector2 v1, Vector2 v2)
        {
            this.v1 = v1 ?? new Vector2();
            this.v2 = v2 ?? new Vector2();
        }

        public override Vector2 GetPoint(float t, Vector2 optionalTarget)
        {
            var point = optionalTarget ?? new Vector2();

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

        public override Vector2 GetPointAt(float u, Vector2 optionalTarget)
        {
            return this.GetPoint(u, optionalTarget);
        }

        public override Vector2 GetTangent(float t, Vector2 optionalTarget)
        {
            var tangent = optionalTarget ?? new Vector2();
            tangent = tangent.Copy(this.v2).Sub(this.v1).Normalize();
            return tangent;
        }

        public LineCurve Copy(LineCurve source)
        {
            base.Copy(source);

            this.v1.Copy(source.v1);
            this.v2.Copy(source.v2);

            return this;
        }
    }
}
