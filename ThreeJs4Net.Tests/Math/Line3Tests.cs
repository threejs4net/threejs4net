using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class Line3Tests : BaseTests
    {
        [Fact()]
        public void InstancingTest()
        {
            var a = new Line3();
            Assert.True(a.Start.Equals(zero3));
            Assert.True(a.End.Equals(zero3));

            a = new Line3(two3.Clone(), one3.Clone());
            Assert.True(a.Start.Equals(two3));
            Assert.True(a.End.Equals(one3));

        }


        [Fact()]
        public void SetTest()
        {
            var a = new Line3();

            a.Set(one3, one3);
            Assert.True(a.Start.Equals(one3));
            Assert.True(a.End.Equals(one3));
        }

        [Fact()]
        public void Copy_EqualTest()
        {
            var a = new Line3(zero3.Clone(), one3.Clone());
            var b = new Line3().Copy(a);
            Assert.True(b.Start.Equals(zero3));
            Assert.True(b.End.Equals(one3));

            // ensure that it is a true copy
            a.Start = zero3;
            a.End = one3;
            Assert.True(b.Start.Equals(zero3));
            Assert.True(b.End.Equals(one3));
        }

        [Fact()]
        public void Clone_EqualTest()
        {
            var a = new Line3();
            var b = new Line3(zero3, new Vector3(1, 1, 1));
            var c = new Line3(zero3, new Vector3(1, 1, 0));

            Assert.False(a.Equals(b), "Check a and b aren't equal");
            Assert.False(a.Equals(c), "Check a and c aren't equal");
            Assert.False(b.Equals(c), "Check b and c aren't equal");

            a = b.Clone();
            Assert.True(a.Equals(b), "Check a and b are equal after clone()");
            Assert.False(a.Equals(c), "Check a and c aren't equal after clone()");

            a.Set(zero3, zero3);
            Assert.False(a.Equals(b), "Check a and b are not equal after modification");
        }

        [Fact()]
        public void GetCenterTest()
        {
            var center = new Vector3();

            var a = new Line3(zero3.Clone(), two3.Clone());
            Assert.True(a.GetCenter(center).Equals(one3.Clone()), "Passed");
        }

        [Fact()]
        public void DeltaTest()
        {
            var delta = new Vector3();

            var a = new Line3(zero3.Clone(), two3.Clone());
            Assert.True(a.Delta(delta).Equals(two3.Clone()), "Passed");

        }

        [Fact()]
        public void DistanceSqTest()
        {
            var a = new Line3(zero3, zero3);
            var b = new Line3(zero3, one3);
            var c = new Line3(one3.Clone().Negate(), one3);
            var d = new Line3(two3.Clone().MultiplyScalar(-2), two3.Clone().Negate());

            Assert.Equal(0, a.DistanceSq());
            Assert.Equal(3, b.DistanceSq());
            Assert.Equal(12, c.DistanceSq());
            Assert.Equal(12, d.DistanceSq());

        }

        [Fact()]
        public void DistanceTest()
        {
            var a = new Line3(zero3, zero3);
            var b = new Line3(zero3, one3);
            var c = new Line3(one3.Clone().Negate(), one3);
            var d = new Line3(two3.Clone().MultiplyScalar(-2), two3.Clone().Negate());

            Assert.Equal(0, a.Distance());
            Assert.Equal(Mathf.Sqrt(3), b.Distance());
            Assert.Equal(Mathf.Sqrt(12), c.Distance());
            Assert.Equal(Mathf.Sqrt(12), d.Distance());
        }

        [Fact()]
        public void AtTest()
        {
            var a = new Line3(one3.Clone(), new Vector3(1, 1, 2));
            var point = new Vector3();

            a.At(-1, point);
            Assert.True(point.DistanceTo(new Vector3(1, 1, 0)) < 0.0001);
            a.At(0, point);
            Assert.True(point.DistanceTo(one3.Clone()) < 0.0001);
            a.At(1, point);
            Assert.True(point.DistanceTo(new Vector3(1, 1, 2)) < 0.0001);
            a.At(2, point);
            Assert.True(point.DistanceTo(new Vector3(1, 1, 3)) < 0.0001);
        }

        [Fact()]
        public void ClosestPointToPointParameterTest()
        {
            var a = new Line3(one3.Clone(), new Vector3(1, 1, 2));
            var point = new Vector3();

            // nearby the ray
            Assert.True(a.ClosestPointToPointParameter(zero3.Clone(), true) == 0);
            a.ClosestPointToPoint(zero3.Clone(), true, point);
            Assert.True(point.DistanceTo(new Vector3(1, 1, 1)) < 0.0001);

            // nearby the ray
            Assert.True(a.ClosestPointToPointParameter(zero3.Clone(), false) == -1);
            a.ClosestPointToPoint(zero3.Clone(), false, point);
            Assert.True(point.DistanceTo(new Vector3(1, 1, 0)) < 0.0001);

            // nearby the ray
            Assert.True(a.ClosestPointToPointParameter(new Vector3(1, 1, 5), true) == 1);
            a.ClosestPointToPoint(new Vector3(1, 1, 5), true, point);
            Assert.True(point.DistanceTo(new Vector3(1, 1, 2)) < 0.0001);

            // exactly on the ray
            Assert.True(a.ClosestPointToPointParameter(one3.Clone(), true) == 0);
            a.ClosestPointToPoint(one3.Clone(), true, point);
            Assert.True(point.DistanceTo(one3.Clone()) < 0.0001);
        }


        [Fact()]
        public void ApplyMatrix4Test()
        {
            var a = new Line3(zero3.Clone(), two3.Clone());
            var b = new Vector4(two3.X, two3.Y, two3.Z, 1);
            var m = new Matrix4().MakeTranslation(x, y, z);
            var v = new Vector3(x, y, z);

            a.ApplyMatrix4(m);
            Assert.True(a.Start.Equals(v), "Translation: check start");
            Assert.True(a.End.Equals(new Vector3(2 + x, 2 + y, 2 + z)), "Translation: check start");

            // reset starting conditions
            a.Set(zero3.Clone(), two3.Clone());
            m.MakeRotationX(Mathf.PI);

            a.ApplyMatrix4(m);
            b.ApplyMatrix4(m);

            Assert.True(a.Start.Equals(zero3));
            Assert.Equal(a.End.X, b.X / b.W);
            Assert.Equal(a.End.Y, b.Y / b.W);
            Assert.Equal(a.End.Z, b.Z / b.W);

            // reset starting conditions
            a.Set(zero3.Clone(), two3.Clone());
            b.Set(two3.X, two3.Y, two3.Z, 1);
            m.SetPosition(v);

            a.ApplyMatrix4(m);
            b.ApplyMatrix4(m);

            Assert.True(a.Start.Equals(v));
            Assert.Equal(a.End.X, b.X / b.W);
            Assert.Equal(a.End.Y, b.Y / b.W);
            Assert.Equal(a.End.Z, b.Z / b.W);
        }

        [Fact()]
        public void EqualsTest()
        {
            var a = new Line3(zero3.Clone(), zero3.Clone());
            var b = new Line3();
            Assert.True(a.Equals(b), "Passed");

        }
    }
}