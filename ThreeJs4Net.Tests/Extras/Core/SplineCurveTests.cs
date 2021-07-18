using Xunit;
using ThreeJs4Net.Extras.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using ThreeJs4Net.Math;
using ThreeJs4Net.Extras.Core;

namespace ThreeJs4Net.Extras.Core.Tests
{
    public class SplineCurveTests
    {
        [Fact()]
        public void SplineCurveTest()
        {
            var _curve = new SplineCurve(new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 5, 5 ),
                new Vector2( 0, 0 ),
                new Vector2( 5, - 5 ),
                new Vector2( 10, 0 )
            });

            var expectedPoints = new Vector2[] {
                        new Vector2( - 10, 0 ),
                        new Vector2( - 6.08f, 4.56f ),
                        new Vector2( - 2, 2.48f ),
                        new Vector2( 2, - 2.48f ),
                        new Vector2( 6.08f, - 4.56f ),
                        new Vector2( 10, 0 )
                };

            var curve = _curve;
            var points = curve.GetPoints(5);
            Assert.Equal(expectedPoints.Length, points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                Assert.True(expectedPoints[i].X - points[i].X <= MathUtils.EPS);
                Assert.True(expectedPoints[i].Y - points[i].Y <= MathUtils.EPS);
            }

            points = curve.GetPoints(4);
            Assert.Equal(curve.points, points);
        }

        [Fact()]
        public void SplineCurve_GetLength()
        {
            var _curve = new SplineCurve(new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 5, 5 ),
                new Vector2( 0, 0 ),
                new Vector2( 5, - 5 ),
                new Vector2( 10, 0 )
            });

            var expectedPoints = new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 6.08f, 4.56f ),
                new Vector2( - 2, 2.48f ),
                new Vector2( 2, - 2.48f ),
                new Vector2( 6.08f, - 4.56f ),
                new Vector2( 10, 0 )
            };

            var curve = _curve;

            var length = curve.GetLength();
            var expectedLength = 28.876950901868135;

            Assert.True(expectedLength - length <= MathUtils.EPS);

            var expectedLengths = new float[] { 0.0f, Mathf.Sqrt(50), Mathf.Sqrt(200), Mathf.Sqrt(450), Mathf.Sqrt(800) };

            var lengths = curve.GetLengths(4);

            Assert.Equal(expectedLengths, lengths);

        }

        [Fact()]
        public void SplineCurve_GetPointAt()
        {
            var _curve = new SplineCurve(new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 5, 5 ),
                new Vector2( 0, 0 ),
                new Vector2( 5, - 5 ),
                new Vector2( 10, 0 )
            });

            var expectedPoints = new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 6.08f, 4.56f ),
                new Vector2( - 2, 2.48f ),
                new Vector2( 2, - 2.48f ),
                new Vector2( 6.08f, - 4.56f ),
                new Vector2( 10, 0 )
            };

            var curve = _curve;
            var point = new Vector2();

            Assert.True(curve.GetPointAt(0, point).Equals(curve.points[0]));
            Assert.True(curve.GetPointAt(1, point).Equals(curve.points[4]));

            curve.GetPointAt(0.5f, point);

            Assert.True(0.0 - point.X <= MathUtils.EPS);
            Assert.True(0.0 - point.Y <= MathUtils.EPS);
        }

        [Fact()]
        public void SplineCurve_GetTangent()
        {
            var _curve = new SplineCurve(new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 5, 5 ),
                new Vector2( 0, 0 ),
                new Vector2( 5, - 5 ),
                new Vector2( 10, 0 )
            });

            var expectedPoints = new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 6.08f, 4.56f ),
                new Vector2( - 2, 2.48f ),
                new Vector2( 2, - 2.48f ),
                new Vector2( 6.08f, - 4.56f ),
                new Vector2( 10, 0 )
            };

            var curve = _curve;
            var expectedTangent = new Vector2[] {
                    new Vector2( 0.7068243340243188f, 0.7073891155729485f ), // 0
                    new Vector2( 0.7069654305325396f, - 0.7072481035902046f ), // 0.5
                    new Vector2( 0.7068243340245123f, 0.7073891155727552f ) // 1
                };

            var tangents = new Vector2[] {
                curve.GetTangent( 0f, new Vector2() ),
                curve.GetTangent( 0.5f, new Vector2() ),
                curve.GetTangent( 1f, new Vector2() )
                };

            for (int i = 0; i < tangents.Length; i++)
            {
                Assert.True(expectedTangent[i].X - tangents[i].X <= MathUtils.EPS3);
                Assert.True(expectedTangent[i].Y - tangents[i].Y <= MathUtils.EPS3);

            }
        }

        [Fact()]
        public void SplineCurve_GetUtoTmapping()
        {
            var _curve = new SplineCurve(new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 5, 5 ),
                new Vector2( 0, 0 ),
                new Vector2( 5, - 5 ),
                new Vector2( 10, 0 )
            });

            var curve = _curve;

            var start = curve.GetUtoTmapping(0, 0);
            var end = curve.GetUtoTmapping(0, curve.GetLength());
            var middle = curve.GetUtoTmapping(0.5f, 0);

            Assert.Equal(0, start);
            Assert.Equal(1, end);
            Assert.True(0.5-middle <= MathUtils.EPS5);


        }


        [Fact()]
        public void SplineCurve_GetSpacedPoints()
        {
            var _curve = new SplineCurve(new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 5, 5 ),
                new Vector2( 0, 0 ),
                new Vector2( 5, - 5 ),
                new Vector2( 10, 0 )
            });

            var expectedPoints = new Vector2[] {
                new Vector2( - 10, 0 ),
                new Vector2( - 4.996509634683014f, 4.999995128640857f ),
                new Vector2( 0, 0 ),
                new Vector2( 4.996509634683006f, - 4.999995128640857f ),
                new Vector2( 10, 0 )
            };

            var curve = _curve;

            var points = curve.GetSpacedPoints(4);

            Assert.Equal(expectedPoints.Length, points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                //Assert.Equal(expectedPoints[i].X, points[i].X);
                //Assert.Equal(expectedPoints[i].Y, points[i].Y);

                Assert.True(MathF.Abs(expectedPoints[i].X - points[i].X) <= MathUtils.EPS3);
                Assert.True(MathF.Abs(expectedPoints[i].Y - points[i].Y) <= MathUtils.EPS3);

            }
        }
    }
}
