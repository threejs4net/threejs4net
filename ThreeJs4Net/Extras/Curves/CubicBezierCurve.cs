using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Curves
{
    public class CubicBezierCurve : Curve<Vector2>
    {
        public Vector2 v0;
        public Vector2 v1;
        public Vector2 v2;
        public Vector2 v3;

        public CubicBezierCurve(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public override Vector2 GetPoint(float t, Vector2 optionalTarget)
        {
            var point = optionalTarget ?? new Vector2();
            var lV0 = this.v0;
            var lV1 = this.v1;
            var lV2 = this.v2;
            var lV3 = this.v3;

            point.Set(
                Interpolation.CubicBezier(t, lV0.X, lV1.X, lV2.X, lV3.X),
                Interpolation.CubicBezier(t, lV0.Y, lV1.Y, lV2.Y, lV3.Y)
            );

            return point;
        }

        public CubicBezierCurve Copy(CubicBezierCurve source)
        {
            base.Copy(source);

            this.v0 = source.v0;
            this.v1 = source.v1;
            this.v2 = source.v2;
            this.v3 = source.v3;

            return this;
        }
    }
}
