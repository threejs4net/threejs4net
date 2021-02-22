//using System;
//using System.Collections.Generic;
//using ThreeJs4Net.Extras.Curves;
//using ThreeJs4Net.Math;

//namespace ThreeJs4Net.Extras.Core
//{
//    public class CurvePath : Curve<Vector2>
//    {
//        public List<Curve<Vector2>> Curves = new List<Curve<Vector2>>();
//        private bool AutoClose;

//        public override Vector2 GetPoint(float t, Vector2 optionalTarget)
//        {
//            throw new NotImplementedException();
//        }

//        public void Add(Curve<Vector2> curve)
//        {
//            this.Curves.Add(curve);
//        }

//        public void ClosePath()
//        {
//            // Add a line curve if start and end of lines are not connected
//            var startPoint = this.Curves[0].GetPoint(0, new Vector2());
//            var endPoint = this.Curves[this.Curves.Count - 1].GetPoint(1, new Vector2());

//            if (!startPoint.Equals(endPoint))
//            {
//                this.Curves.Add(new LineCurve(endPoint, startPoint));
//            }
//        }

//        public void GetPoint(float t)
//        {
//            var d = t * this.GetLength();
//            var curveLengths = this.GetCurveLengths();
//            var i = 0;

//            // To think about boundaries points.
//            while (i < curveLengths.length)
//            {
//                if (curveLengths[i] >= d)
//                {
//                    var diff = curveLengths[i] - d;
//                    var curve = this.Curves[i];
//                    var segmentLength = curve.GetLength();
//                    var u = segmentLength == 0 ? 0 : 1 - diff / segmentLength;

//                    return curve.GetPointAt(u);
//                }

//                i++;
//            }

//            return null;
//            // loop where sum != 0, sum > d , sum+1 <d
//        }

//        // We cannot use the default THREE.Curve getPoint() with getLength() because in
//        // THREE.Curve, getLength() depends on getPoint() but in THREE.CurvePath
//        // getPoint() depends on getLength

//        public void GetLength()
//        {
//            var lens = this.getCurveLengths();
//            return lens[lens.length - 1];
//        }

//        // cacheLengths must be recalculated.
//        public void UpdateArcLengths()
//        {
//            this.NeedsUpdate = true;
//            this.CacheLengths = null;
//            this.GetCurveLengths();
//        }

//        // Compute lengths and cache them
//        // We cannot overwrite getLengths() because UtoT mapping uses it.

//        public void GetCurveLengths()
//        {
//            // We use cache values if curves and cache array are same length
//            if (this.cacheLengths && this.cacheLengths.length == = this.Curves.length)
//            {
//                return this.cacheLengths;
//            }

//            // Get length of sub-curve
//            // Push sums into cached array
//            var lengths =  [],
//            sums = 0;
//            for (var i = 0, l = this.Curves.length; i < l; i++)
//            {
//                sums += this.Curves[i].getLength();
//                lengths.push(sums);
//            }

//            this.cacheLengths = lengths;
//            return lengths;
//        }

//        public void GetSpacedPoints(divisions)
//        {
//            if (divisions == = undefined) divisions = 40;
//            var points =  []
//            ;
//            for (var i = 0; i <= divisions; i++)
//            {
//                points.push(this.getPoint(i / divisions));
//            }

//            if (this.autoClose)
//            {
//                points.push(points[0]);
//            }

//            return points;
//        }

//        public void GetPoints(int divisions = 12)
//        {
//            var points =  [],

//            for (var i = 0; i < this.Curves.Count; i++)
//            {
//                var curve = Curves[i];
//                var resolution = (curve is EllipseCurve) ? divisions * 2
//                    : ((curve is LineCurve || curve is LineCurve3)) ? 1
//                    : (curve is SplineCurve) ? divisions * curve.points.length
//                    : divisions;
//                var pts = curve.getPoints(resolution);
//                for (var j = 0; j < pts.length; j++)
//                {
//                    var point = pts[j];

//                    if (last && last.equals(point)) continue; // ensures no consecutive points are duplicates

//                    points.push(point);
//                    last = point;

//                }

//            }

//            if (this.autoClose && points.length > 1 && !points[points.length - 1].equals(points[0]))
//            {

//                points.push(points[0]);

//            }

//            return points;

//        }

//        public CurvePath Copy(CurvePath source)
//        {
//            base.Copy(source);
//            this.Curves = new List<Curve<Vector2>>();

//            for (var i = 0; i < source.Curves.Count; i++)
//            {
//                var curve = source.Curves[i];
//                this.Curves.Add(curve.Clone());
//            }

//            this.AutoClose = source.AutoClose;
//            return this;
//        }
//    }
//}