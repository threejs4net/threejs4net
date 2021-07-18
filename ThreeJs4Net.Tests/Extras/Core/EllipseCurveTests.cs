using Xunit;
using ThreeJs4Net.Extras.Core;
using System;
using System.Collections.Generic;
using System.Text;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Core.Tests
{
    public class EllipseCurveTests
    {
        [Fact()]
        public void EllipseCurveTest_SimpleCurve()
        {
            var curve = new EllipseCurve(
                0, 0, // ax, aY
                10, 10, // xRadius, yRadius
                0, 2 * Mathf.PI, // aStartAngle, aEndAngle
                false, // aClockwise
                0 // aRotation
            );

            var expectedPoints = new[]
            {
                new Vector2(10, 0),
                new Vector2(0, 10),
                new Vector2(-10, 0),
                new Vector2(0, -10),
                new Vector2(10, 0)
            };

            var points = curve.GetPoints(expectedPoints.Length - 1);

            Assert.Equal(expectedPoints.Length, points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                Assert.True(expectedPoints[i].X - points[i].X <= MathUtils.EPS5);
                Assert.True(expectedPoints[i].Y - points[i].Y <= MathUtils.EPS5);
            }
        }

        [Fact()]
        public void EllipseCurveTest_GetLength()
        {
            var curve = new EllipseCurve(
                0, 0, // ax, aY
                10, 10, // xRadius, yRadius
                0, 2 * Mathf.PI, // aStartAngle, aEndAngle
                false, // aClockwise
                0 // aRotation
            );

            var length = curve.GetLength();
            var expectedLength = 62.829269247282795;

            Assert.True(expectedLength - length <= MathUtils.EPS5);

            var lengths = curve.GetLengths(5);
            var expectedLengths = new[]
            {
                0,
                11.755705045849462,
                23.51141009169892,
                35.26711513754839,
                47.02282018339785,
                58.77852522924731
            };

            Assert.Equal(expectedLengths.Length, lengths.Count);

            for (int i = 0; i < lengths.Count; i++)
            {
                Assert.True(expectedLengths[i] - lengths[i] <= MathUtils.EPS5);
            }
        }

        [Fact()]
        public void EllipseCurveTest_GetPointAt()
        {
            var curve = new EllipseCurve(
                0, 0, // ax, aY
                10, 10, // xRadius, yRadius
                0, 2 * Mathf.PI, // aStartAngle, aEndAngle
                false, // aClockwise
                0 // aRotation
            );

            var testValues = new[] { 0, 0.3f, 0.5f, 0.7f, 1 };

            var p = new Vector2();
            var a = new Vector2();

            foreach (var val in testValues)
            {
                var expectedX = Mathf.Cos(val * Mathf.PI * 2) * 10;
                var expectedY = Mathf.Sin(val * Mathf.PI * 2) * 10;

                curve.GetPoint(val, p);
                curve.GetPointAt(val, a);

                Assert.True(Mathf.Abs(expectedX - p.X) <= MathUtils.EPS3);
                Assert.True(Mathf.Abs(expectedY - p.Y) <= MathUtils.EPS3);
                Assert.True(Mathf.Abs(expectedX - a.X) <= MathUtils.EPS3);
                Assert.True(Mathf.Abs(expectedY - a.Y) <= MathUtils.EPS3);
            }
        }

        [Fact()]
        public void EllipseCurveTest_GetTangent()
        {
            var curve = new EllipseCurve(
                0, 0, // ax, aY
                10, 10, // xRadius, yRadius
                0, 2 * Mathf.PI, // aStartAngle, aEndAngle
                false, // aClockwise
                0 // aRotation
            );

            var expectedTangents = new[]
            {
                new Vector2(-0.000314159260186071f, 0.9999999506519786f),
                new Vector2(-1, 0),
                new Vector2(0, -1),
                new Vector2(1, 0),
                new Vector2(0.00031415926018600165f, 0.9999999506519784f)
            };

            var tangents = new[]
            {
                curve.GetTangent(0, new Vector2()),
                curve.GetTangent(0.25f, new Vector2()),
                curve.GetTangent(0.5f, new Vector2()),
                curve.GetTangent(0.75f, new Vector2()),
                curve.GetTangent(1, new Vector2())
            };

            for (int i = 0; i < expectedTangents.Length; i++)
            {
                var tangent = tangents[i];

                Assert.True(expectedTangents[i].X - tangent.X <= MathUtils.EPS3);
                Assert.True(expectedTangents[i].Y - tangent.Y <= MathUtils.EPS3);
            }
        }

        [Fact()]
        public void EllipseCurveTest_GetUtoTmapping()
        {
            var curve = new EllipseCurve(
                0, 0, // ax, aY
                10, 10, // xRadius, yRadius
                0, 2 * Mathf.PI, // aStartAngle, aEndAngle
                false, // aClockwise
                0 // aRotation
            );

            var start = curve.GetUtoTmapping(0, 0);
            var end = curve.GetUtoTmapping(0, curve.GetLength());
            var somewhere = curve.GetUtoTmapping(0.7f, 1);

            var expectedSomewhere = 0.01591614882650014;

            Assert.Equal(0, start);
            Assert.Equal(1, end);
            Assert.True(expectedSomewhere - somewhere <= MathUtils.EPS3);

        }

        [Fact()]
        public void EllipseCurveTest_GetSpacedPoints()
        {
            var curve = new EllipseCurve(
                0, 0, // ax, aY
                10, 10, // xRadius, yRadius
                0, 2 * Mathf.PI, // aStartAngle, aEndAngle
                false, // aClockwise
                0 // aRotation
            );

            var expectedPoints = new[] {
                    new Vector2( 10, 0 ),
                    new Vector2( 3.0901699437494603f, 9.51056516295154f ),
                    new Vector2( - 8.090169943749492f, 5.877852522924707f ),
                    new Vector2( - 8.090169943749459f, - 5.877852522924751f ),
                    new Vector2( 3.0901699437494807f, - 9.510565162951533f ),
                    new Vector2( 10, - 2.4492935982947065e-15f )
                };

            var points = curve.GetSpacedPoints();

            Assert.Equal(expectedPoints.Length, points.Count);

            for (int i = 0; i < expectedPoints.Length; i++)
            {
                var point = points[i];

                Assert.True(expectedPoints[i].X - point.X <= MathUtils.EPS3);
                Assert.True(expectedPoints[i].Y - point.Y <= MathUtils.EPS3);
            }
        }
    }
}