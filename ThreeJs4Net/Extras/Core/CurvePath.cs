using System;
using System.Collections.Generic;
using ThreeJs4Net.Extras.Curves;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Core
{
    public class CurvePath : Curve<Vector2>
    {
        public List<Curve<Vector2>> Curves = new List<Curve<Vector2>>();
        private bool AutoClose;
        private float[] cacheLengths = null;

        public CurvePath(): base()
        {
            this.Curves = new List<Curve<Vector2>>();
            this.AutoClose = false;
        }

        public void Add(Curve<Vector2> curve)
        {
            this.Curves.Add(curve);
        }
       
        public void ClosePath()
        {
            // Add a line curve if start and end of lines are not connected
            var startPoint = this.Curves[0].GetPoint(0, new Vector2());
            var endPoint = this.Curves[this.Curves.Count - 1].GetPoint(1, new Vector2());

            if (!startPoint.Equals(endPoint))
            {
                this.Curves.Add(new LineCurve(endPoint, startPoint));
            }
        }

        public override Vector2 GetPoint(float t, Vector2 optionalTarget)
        {
            var d = t * this.GetLength();
            var curveLengths = this.GetCurveLengths();
            var i = 0;

            // To think about boundaries points.
            while (i < curveLengths.Length)
            {
                if (curveLengths[i] >= d)
                {
                    var diff = curveLengths[i] - d;
                    var curve = this.Curves[i];
                    var segmentLength = curve.GetLength();
                    var u = segmentLength == 0 ? 0 : 1 - diff / segmentLength;

                    return curve.GetPointAt(u, null);
                }

                i++;
            }

            return null;
            // loop where sum != 0, sum > d , sum+1 <d
        }

        // We cannot use the default THREE.Curve getPoint() with getLength() because in
        // THREE.Curve, getLength() depends on getPoint() but in THREE.CurvePath
        // getPoint() depends on getLength

        public new float GetLength()
        {
            var lens = this.GetCurveLengths();
            return lens[lens.Length - 1];
        }

        // cacheLengths must be recalculated.
        public void UpdateArcLengths()
        {
            needsUpdate = true;
            cacheLengths = null;
            this.GetCurveLengths();
        }

        // Compute lengths and cache them
        // We cannot overwrite getLengths() because UtoT mapping uses it.

        public float[] GetCurveLengths()
        {
            // We use cache values if curves and cache array are same length
            if (this.cacheLengths?.Length == Curves.Count)
            {
                return this.cacheLengths;
            }

            // Get length of sub-curve
            // Push sums into cached array
            var lengths = new List<float>();
            float sums = 0;
            for (var i = 0; i < Curves.Count; i++)
            {
                sums += Curves[i].GetLength();
                lengths.Add(sums);
            }

            this.cacheLengths = lengths.ToArray();
            return lengths.ToArray();
        }

        public new List<Vector2> GetSpacedPoints(int divisions = 40)
        {
            var points = new List<Vector2>();
            for (var i = 0; i <= divisions; i++)
            {
                points.Add(this.GetPoint((float)i / divisions, null));
            }

            if (this.AutoClose)
            {
                points.Add(points[0]);
            }

            return points;
        }

        public new List<Vector2> GetPoints(int divisions = 12)
        {
            var points = new List<Vector2>();
            Vector2 last = null;

            for (var i = 0; i < this.Curves.Count; i++)
            {
                var curve = Curves[i];
                var resolution = (curve is EllipseCurve) ? divisions * 2
                    : ((curve is LineCurve || curve is LineCurve3)) ? 1
                    : (curve is SplineCurve) ? divisions * curve.GetPoints().Count
                    : divisions;
                var pts = curve.GetPoints(resolution);
                for (var j = 0; j < pts.Count; j++)
                {
                    var point = pts[j];

                    if (last != null && last.Equals(point))
                    {
                        continue; // ensures no consecutive points are duplicates
                    }

                    points.Add(point);
                    last = point;
                }
            }

            if (this.AutoClose && points.Count > 1 && !points[points.Count - 1].Equals(points[0]))
            {
                points.Add(points[0]);
            }

            return points;
        }

        public CurvePath Copy(CurvePath source)
        {
            base.Copy(source);
            this.Curves = new List<Curve<Vector2>>();

            for (var i = 0; i < source.Curves.Count; i++)
            {
                var curve = source.Curves[i];
                this.Curves.Add(curve.Clone());
            }

            this.AutoClose = source.AutoClose;
            return this;
        }
   }
}