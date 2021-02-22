using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Curves
{
    public class EllipseCurve : Curve<Vector2>
    {
        public float aX;
        public float aY;
        public float xRadius;
        public float yRadius;
        public float aStartAngle;
        public float aEndAngle;
        public bool aClockwise;
        public float aRotation;

        public EllipseCurve(float aX = 0, float aY = 0, float xRadius = 1, float yRadius = 1, float aStartAngle = 0, float? aEndAngle = null, bool aClockwise = false, float aRotation = 0)
        {
            this.aEndAngle = aEndAngle ?? 2 * Mathf.PI;
        }

        public override Vector2 GetPoint(float t, Vector2 optionalTarget)
        {
            var point = optionalTarget ?? new Vector2();

            var twoPi = Mathf.PI * 2;
            var deltaAngle = this.aEndAngle - this.aStartAngle;
            var samePoints = Mathf.Abs(deltaAngle) < float.Epsilon;

            // ensures that deltaAngle is 0 .. 2 PI
            while (deltaAngle < 0) deltaAngle += twoPi;
            while (deltaAngle > twoPi) deltaAngle -= twoPi;

            if (deltaAngle < float.Epsilon)
            {
                if (samePoints)
                {
                    deltaAngle = 0;
                }
                else
                {
                    deltaAngle = twoPi;
                }
            }

            if (this.aClockwise && !samePoints)
            {
                if (deltaAngle == twoPi)
                {
                    deltaAngle = -twoPi;
                }
                else
                {
                    deltaAngle = deltaAngle - twoPi;
                }
            }

            var angle = this.aStartAngle + t * deltaAngle;
            var x = this.aX + this.xRadius * Mathf.Cos(angle);
            var y = this.aY + this.yRadius * Mathf.Sin(angle);

            if (this.aRotation != 0)
            {
                var cos = Mathf.Cos(this.aRotation);
                var sin = Mathf.Sin(this.aRotation);
                var tx = x - this.aX;
                var ty = y - this.aY;

                // Rotate the point about the center of the ellipse.
                x = tx * cos - ty * sin + this.aX;
                y = tx * sin + ty * cos + this.aY;
            }

            return point.Set(x, y);
        }

        public EllipseCurve Copy(EllipseCurve source)
        {
            base.Copy(source);

            this.aX = source.aX;
            this.aY = source.aY;
            this.xRadius = source.xRadius;
            this.yRadius = source.yRadius;
            this.aStartAngle = source.aStartAngle;
            this.aEndAngle = source.aEndAngle;
            this.aClockwise = source.aClockwise;
            this.aRotation = source.aRotation;

            return this;
        }
    }
}
