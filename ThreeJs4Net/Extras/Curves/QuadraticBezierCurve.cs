using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Curves
{
    public class QuadraticBezierCurve : Curve<Vector2>
    {
        public Vector2 v0;
        public Vector2 v1;
        public Vector2 v2;

        public QuadraticBezierCurve(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }

        public override Vector2 GetPoint(float t, Vector2 optionalTarget)
        {
            var point = optionalTarget ?? new Vector2();
            var lV0 = this.v0;
            var lV1 = this.v1;
            var lV2 = this.v2;

            point.Set(
                Interpolation.QuadraticBezier(t, lV0.X, lV1.X, lV2.X),
                Interpolation.QuadraticBezier(t, lV0.Y, lV1.Y, lV2.Y)
            );

            return point;
        }

        public QuadraticBezierCurve Copy(QuadraticBezierCurve source)
        {
            base.Copy(source);

            this.v0 = source.v0;
            this.v1 = source.v1;
            this.v2 = source.v2;

            return this;
        }
    }
}
