using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Extras.Curves;
using ThreeJs4Net.Math;
using EllipseCurve = ThreeJs4Net.Extras.Curves.EllipseCurve;

namespace ThreeJs4Net.Extras.Core
{
    public class Path : CurvePath
    {
        public Vector2 CurrentPoint;

        public Path() : base()
        {
            CurrentPoint = new Vector2();
        }

        public Path(IEnumerable<Vector2> points) : base()
        {
            CurrentPoint = new Vector2();
            if (points != null && points.Any())
            {
                this.SetFromPoints(points.ToArray());
            }
        }

        public Path SetFromPoints(Vector2[] points) 
        {
            this.MoveTo(points[0].X, points[0].Y);
            for (var i = 1; i < points.Length; i++)
            {
                this.LineTo(points[i].X, points[i].Y);
            }
            return this;
        }

        public Path MoveTo(float x, float y) 
        {
            this.CurrentPoint.Set(x, y); // TODO consider referencing vectors instead of copying?
            return this;
        }

        public Path LineTo(float x, float y) 
        {
            var curve = new LineCurve(this.CurrentPoint.Clone(), new Vector2(x, y));
            this.Curves.Add(curve);
            this.CurrentPoint.Set(x, y);
            return this;
        }

        public Path QuadraticCurveTo(float aCPx, float aCPy, float aX, float aY)
        {
            var curve = new QuadraticBezierCurve(
                this.CurrentPoint.Clone(),
                new Vector2(aCPx, aCPy),
                new Vector2(aX, aY)
            );

            this.Curves.Add(curve);
            this.CurrentPoint.Set(aX, aY);
            return this;
        }

        public Path BezierCurveTo(float aCP1x, float aCP1y, float aCP2x, float aCP2y, float aX, float aY)
        {
            var curve = new CubicBezierCurve(
                this.CurrentPoint.Clone(),
                new Vector2(aCP1x, aCP1y),
                new Vector2(aCP2x, aCP2y),
                new Vector2(aX, aY)
            );

            this.Curves.Add(curve);
            this.CurrentPoint.Set(aX, aY);
            return this;
        }

        public Path SplineThru(Vector2[] pts)
        {
            var npts = new List<Vector2>();
            npts.Add(this.CurrentPoint.Clone());
            npts.AddRange(pts);
            var curve = new SplineCurve(npts);
            this.Curves.Add(curve);
            this.CurrentPoint.Copy(pts[pts.Length - 1]);
            return this;
        }

        public Path Arc(float aX, float aY, float aRadius, float aStartAngle, float aEndAngle, bool aClockwise)
        {
            var x0 = this.CurrentPoint.X;
            var y0 = this.CurrentPoint.Y;

            this.Absarc(aX + x0, aY + y0, aRadius, aStartAngle, aEndAngle, aClockwise);

            return this;
        }

        public Path Absarc(float aX, float aY, float aRadius, float aStartAngle, float aEndAngle, bool aClockwise)
        {
            this.Absellipse(aX, aY, aRadius, aRadius, aStartAngle, aEndAngle, aClockwise, 0);
            return this;
        }

        public Path Ellipse(float aX, float aY, float xRadius, float yRadius, float aStartAngle, float aEndAngle, bool aClockwise, float aRotation) 
        {
            var x0 = this.CurrentPoint.X;
            var y0 = this.CurrentPoint.Y;
            this.Absellipse(aX + x0, aY + y0, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise, aRotation);
            return this;
        }

        public Path Absellipse(float aX, float aY, float xRadius, float yRadius, float aStartAngle, float aEndAngle, bool aClockwise , float aRotation = 0)
        {
            var curve = new EllipseCurve(aX, aY, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise, aRotation);
            if (this.Curves.Count > 0)
            {
                // if a previous curve is present, attempt to join
                var firstPoint = curve.GetPoint(0, new Vector2());
                if (!firstPoint.Equals(this.CurrentPoint))
                {
                    this.LineTo(firstPoint.X, firstPoint.Y);
                }
            }

            this.Curves.Add(curve);
            var lastPoint = curve.GetPoint(1, new Vector2());
            this.CurrentPoint.Copy(lastPoint);
            return this;
        }

        public Path Copy(Path source) {
            base.Copy(source);
            this.CurrentPoint.Copy(source.CurrentPoint);
            return this;
        }
    }
}
