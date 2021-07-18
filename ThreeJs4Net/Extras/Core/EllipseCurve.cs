using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Core
{
    public class EllipseCurve : Curve<Vector2>
    {
        public float aX = 0;
        public float aY = 0;
        public float xRadius = 1;
        public float yRadius = 1;
        public float aStartAngle = 0;
        public float aEndAngle = 2 * Mathf.PI;
        public bool aClockwise = false;
        public float aRotation = 0;

        public EllipseCurve(float aX, float aY, float xRadius, float yRadius, float aStartAngle, float aEndAngle, bool aClockwise = false, float aRotation = Mathf.PI)
        {
            this.aX = aX;
            this.aY = aY;
            this.xRadius = xRadius;
            this.yRadius = yRadius;
            this.aStartAngle = aStartAngle;
            this.aEndAngle = aEndAngle;
            this.aClockwise = aClockwise;
            this.aRotation = aRotation;
        }

        public override Vector2 GetPoint(float t, Vector2 optionalTarget)
        {
            Vector2 point = optionalTarget ?? new Vector2();

            var twoPi = Mathf.PI * 2;
            var deltaAngle = this.aEndAngle - this.aStartAngle;
            var samePoints = Mathf.Abs(deltaAngle) < MathUtils.EPS5;

            // ensures that deltaAngle is 0 .. 2 PI
            while (deltaAngle < 0) deltaAngle += twoPi;
            while (deltaAngle > twoPi) deltaAngle -= twoPi;

            if (deltaAngle < MathUtils.EPS5)
            {
                deltaAngle = samePoints ? 0 : twoPi;
            }

            if (this.aClockwise && !samePoints)
            {
                deltaAngle = deltaAngle == twoPi ? -twoPi : deltaAngle - twoPi;
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

        public Curve<Vector2> Copy(EllipseCurve source)
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
