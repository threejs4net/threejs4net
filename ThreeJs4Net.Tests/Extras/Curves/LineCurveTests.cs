//using Xunit;
//using ThreeJs4Net.Extras.Curves;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using ThreeJs4Net.Math;
//using ThreeJs4Net.Tests;

//namespace ThreeJs4Net.Extras.Curves.Tests
//{
//    public class LineCurveTests : BaseTests
//    {
//        private List<Vector2> _points = new List<Vector2>
//        {
//            new Vector2(0, 0), new Vector2(10, 10), new Vector2(-10, 10), new Vector2(-8, 5)
//        };

//        [Fact()]
//        public void LineCurveTest()
//        {
//            Assert.True(false, "This test needs an implementation");
//        }

//        [Fact()]
//        public void SimpleCurveTest()
//        {
//            var curve = new LineCurve(_points[0], _points[3]);

//            var expectedPoints = new List<Vector2>
//            {
//                new Vector2(0, 0),
//                new Vector2(2, 2),
//                new Vector2(4, 4),
//                new Vector2(6, 6),
//                new Vector2(8, 8),
//                new Vector2(10, 10)
//            };

//            var points = curve.GetPoints();

//            Assert.Equal(points, expectedPoints);

//            curve = new LineCurve(_points[1], _points[2]);

//            expectedPoints = new List<Vector2>
//            {
//                new Vector2(10, 10),
//                new Vector2(6, 10),
//                new Vector2(2, 10),
//                new Vector2(-2, 10),
//                new Vector2(-6, 10),
//                new Vector2(-10, 10)
//            };

//            points = curve.GetPoints();

//            Assert.Equal(points, expectedPoints);

//        }

//        [Fact()]
//        public void GetPointAtTest()
//        {
//            var curve = new LineCurve(_points[0], _points[3]);

//            var expectedPoints = new List<Vector2>
//            {
//                new Vector2( 0, 0 ), new Vector2( (float)-2.4, (float)1.5 ),
//                new Vector2( -4, (float)2.5 ), new Vector2( - 8, 5 )
//            };

//            var points = new List<Vector2>
//            {
//                curve.GetPointAt(0, new Vector2()), curve.GetPointAt((float)0.3, new Vector2()),
//                curve.GetPointAt((float)0.5, new Vector2()), curve.GetPointAt(1, new Vector2())
//            };

//            Assert.Equal(points, expectedPoints);
//        }

//        [Fact()]
//        public void GetTangentTest()
//        {
//            LineCurve curve = new LineCurve(_points[0], _points[1]);

//            var tangent = new Vector2();

//            curve.GetTangent(0, tangent);
//            var expectedTangent = Mathf.Sqrt((float)0.5);

//            Assert.True(Mathf.Abs(tangent.X - expectedTangent) <= MathUtils.EPS);
//            Assert.True(Mathf.Abs(tangent.Y - expectedTangent) <= MathUtils.EPS);
//        }

//        [Fact()]
//        public void CopyTest()
//        {
//            Assert.True(false, "This test needs an implementation");
//        }
//    }
//}