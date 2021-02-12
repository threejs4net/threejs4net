using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class Box2Tests : BaseTests
    {
        [Fact()]
        public void Box2Test()
        {
            var a = new Box2();
            Assert.True(a.Min.Equals(posInf2));
            Assert.True(a.Max.Equals(negInf2));

            a = new Box2(zero2.Clone(), zero2.Clone());
            Assert.True(a.Min.Equals(zero2));
            Assert.True(a.Max.Equals(zero2));

            a = new Box2(zero2.Clone(), one2.Clone());
            Assert.True(a.Min.Equals(zero2));
            Assert.True(a.Max.Equals(one2));
        }

        [Fact()]
        public void ClampPointTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var b = new Box2(one2.Clone().Negate(), one2.Clone());

            var point = new Vector2();

            a.ClampPoint(zero2, point);
            Assert.True(point.Equals(new Vector2(0, 0)));
            a.ClampPoint(one2, point);
            Assert.True(point.Equals(new Vector2(0, 0)));
            a.ClampPoint(one2.Clone().Negate(), point);
            Assert.True(point.Equals(new Vector2(0, 0)));

            b.ClampPoint(two2, point);
            Assert.True(point.Equals(new Vector2(1, 1)));
            b.ClampPoint(one2, point);
            Assert.True(point.Equals(new Vector2(1, 1)));
            b.ClampPoint(zero2, point);
            Assert.True(point.Equals(new Vector2(0, 0)));
            b.ClampPoint(one2.Clone().Negate(), point);
            Assert.True(point.Equals(new Vector2(-1, -1)));
            b.ClampPoint(two2.Clone().Negate(), point);
            Assert.True(point.Equals(new Vector2(-1, -1)));

        }

        [Fact()]
        public void CloneTest()
        {
            var a = new Box2(zero2, zero2);

            var b = a.Clone();
            Assert.True(b.Min.Equals(zero2));
            Assert.True(b.Max.Equals(zero2));

            a = new Box2();
            b = a.Clone();
            Assert.True(b.Min.Equals(posInf2));
            Assert.True(b.Max.Equals(negInf2));
        }

        [Fact()]
        public void CopyTest()
        {
            var a = new Box2(zero2.Clone(), one2.Clone());
            var b = new Box2().Copy(a);
            Assert.True(b.Min.Equals(zero2));
            Assert.True(b.Max.Equals(one2));

            // ensure that it is a true copy
            a.Min = zero2;
            a.Max = one2;
            Assert.True(b.Min.Equals(zero2));
            Assert.True(b.Max.Equals(one2));
        }

        [Fact()]
        public void ContainsPointTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());

            Assert.True(a.ContainsPoint(zero2));
            Assert.True(!a.ContainsPoint(one2));

            a.ExpandByScalar(1);
            Assert.True(a.ContainsPoint(zero2));
            Assert.True(a.ContainsPoint(one2));
            Assert.True(a.ContainsPoint(one2.Clone().Negate()));
        }

        [Fact()]
        public void ContainsBoxTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var b = new Box2(zero2.Clone(), one2.Clone());
            var c = new Box2(one2.Clone().Negate(), one2.Clone());

            Assert.True(a.ContainsBox(a));
            Assert.True(!a.ContainsBox(b));
            Assert.True(!a.ContainsBox(c));

            Assert.True(b.ContainsBox(a));
            Assert.True(c.ContainsBox(a));
            Assert.True(!b.ContainsBox(c));
        }

        [Fact()]
        public void DistanceToPointTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var b = new Box2(one2.Clone().Negate(), one2.Clone());

            Assert.True(a.DistanceToPoint(new Vector2(0, 0)) == 0);
            Assert.True(a.DistanceToPoint(new Vector2(1, 1)) == Mathf.Sqrt(2));
            Assert.True(a.DistanceToPoint(new Vector2(-1, -1)) == Mathf.Sqrt(2));

            Assert.True(b.DistanceToPoint(new Vector2(2, 2)) == Mathf.Sqrt(2));
            Assert.True(b.DistanceToPoint(new Vector2(1, 1)) == 0);
            Assert.True(b.DistanceToPoint(new Vector2(0, 0)) == 0);
            Assert.True(b.DistanceToPoint(new Vector2(-1, -1)) == 0);
            Assert.True(b.DistanceToPoint(new Vector2(-2, -2)) == Mathf.Sqrt(2));

        }

        [Fact()]
        public void ExpandByPointTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var size = new Vector2();
            var center = new Vector2();

            a.ExpandByPoint(zero2);
            Assert.True(a.GetSize(size).Equals(zero2));

            a.ExpandByPoint(one2);
            Assert.True(a.GetSize(size).Equals(one2));

            a.ExpandByPoint(one2.Clone().Negate());
            Assert.True(a.GetSize(size).Equals(one2.Clone().MultiplyScalar(2)));
            Assert.True(a.GetCenter(center).Equals(zero2));

        }

        [Fact()]
        public void ExpandByVectorTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var size = new Vector2();
            var center = new Vector2();

            a.ExpandByVector(zero2);
            Assert.True(a.GetSize(size).Equals(zero2));

            a.ExpandByVector(one2);
            Assert.True(a.GetSize(size).Equals(one2.Clone().MultiplyScalar(2)));
            Assert.True(a.GetCenter(center).Equals(zero2));

        }

        [Fact()]
        public void ExpandByScalarTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var size = new Vector2();
            var center = new Vector2();

            a.ExpandByScalar(0);
            Assert.True(a.GetSize(size).Equals(zero2));

            a.ExpandByScalar(1);
            Assert.True(a.GetSize(size).Equals(one2.Clone().MultiplyScalar(2)));
            Assert.True(a.GetCenter(center).Equals(zero2));
        }

        [Fact()]
        public void EqualsTest()
        {
            var a = new Box2();
            var b = new Box2();
            Assert.True(b.Equals(a));
            Assert.True(a.Equals(b));

            a = new Box2(one2, two2);
            b = new Box2(one2, two2);
            Assert.True(b.Equals(a));
            Assert.True(a.Equals(b));

            a = new Box2(one2, two2);
            b = a.Clone();
            Assert.True(b.Equals(a));
            Assert.True(a.Equals(b));

            a = new Box2(one2, two2);
            b = new Box2(one2, one2);
            Assert.True(!b.Equals(a));
            Assert.True(!a.Equals(b));

            a = new Box2();
            b = new Box2(one2, one2);
            Assert.True(!b.Equals(a));
            Assert.True(!a.Equals(b));

            a = new Box2(one2, two2);
            b = new Box2(one2, one2);
            Assert.True(!b.Equals(a));
            Assert.True(!a.Equals(b));
        }

        [Fact()]
        public void IntersectTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var b = new Box2(zero2.Clone(), one2.Clone());
            var c = new Box2(one2.Clone().Negate(), one2.Clone());

            Assert.True(a.Clone().Intersect(a).Equals(a));
            Assert.True(a.Clone().Intersect(b).Equals(a));
            Assert.True(b.Clone().Intersect(b).Equals(b));
            Assert.True(a.Clone().Intersect(c).Equals(a));
            Assert.True(b.Clone().Intersect(c).Equals(b));
            Assert.True(c.Clone().Intersect(c).Equals(c));
        }

        [Fact()]
        public void GetParameterTest()
        {
            var a = new Box2(zero2.Clone(), one2.Clone());
            var b = new Box2(one2.Clone().Negate(), one2.Clone());

            var parameter = new Vector2();

            a.GetParameter(zero2, parameter);
            Assert.True(parameter.Equals(zero2));
            a.GetParameter(one2, parameter);
            Assert.True(parameter.Equals(one2));

            b.GetParameter(one2.Clone().Negate(), parameter);
            Assert.True(parameter.Equals(zero2));
            b.GetParameter(zero2, parameter);
            Assert.True(parameter.Equals(new Vector2((float)0.5, (float)0.5)));
            b.GetParameter(one2, parameter);
            Assert.True(parameter.Equals(one2));

        }

        [Fact()]
        public void GetCenterTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var center = new Vector2();
            Assert.True(a.GetCenter(center).Equals(zero2));

            a = new Box2(zero2, one2);
            var midpoint = one2.Clone().MultiplyScalar((float)0.5);
            Assert.True(a.GetCenter(center).Equals(midpoint));
        }

        [Fact()]
        public void GetSizeTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var size = new Vector2();

            Assert.True(a.GetSize(size).Equals(zero2));

            a = new Box2(zero2.Clone(), one2.Clone());
            Assert.True(a.GetSize(size).Equals(one2));

        }

        [Fact()]
        public void IntersectsBoxTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var b = new Box2(zero2.Clone(), one2.Clone());
            var c = new Box2(one2.Clone().Negate(), one2.Clone());

            Assert.True(a.IntersectsBox(a));
            Assert.True(a.IntersectsBox(b));
            Assert.True(a.IntersectsBox(c));

            Assert.True(b.IntersectsBox(a));
            Assert.True(c.IntersectsBox(a));
            Assert.True(b.IntersectsBox(c));

            b.Translate(two2);
            Assert.True(!a.IntersectsBox(b));
            Assert.True(!b.IntersectsBox(a));
            Assert.True(!b.IntersectsBox(c));

        }

        [Fact()]
        public void IsEmptyTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            Assert.True(!a.IsEmpty());

            a = new Box2(zero2.Clone(), one2.Clone());
            Assert.True(!a.IsEmpty());

            a = new Box2(two2.Clone(), one2.Clone());
            Assert.True(a.IsEmpty());

            a = new Box2(posInf2.Clone(), negInf2.Clone());
            Assert.True(a.IsEmpty());
        }

        [Fact()]
        public void MakeEmptyTest()
        {
            var a = new Box2();

            Assert.True(a.IsEmpty());

            a = new Box2(zero2.Clone(), one2.Clone());
            Assert.True(!a.IsEmpty());

            a.MakeEmpty();
            Assert.True(a.IsEmpty());
        }

        [Fact()]
        public void SetTest()
        {
            var a = new Box2();

            a.Set(zero2, one2);
            Assert.True(a.Min.Equals(zero2));
            Assert.True(a.Max.Equals(one2));
        }

        [Fact()]
        public void SetFromPointsTest()
        {
            var a = new Box2();

            a.SetFromPoints(new Vector2[] { zero2, one2, two2 });
            Assert.True(a.Min.Equals(zero2));
            Assert.True(a.Max.Equals(two2));

            a.SetFromPoints(new Vector2[] { one2 });
            Assert.True(a.Min.Equals(one2));
            Assert.True(a.Max.Equals(one2));

            a.SetFromPoints(new Vector2[] { });
            Assert.True(a.IsEmpty());
        }

        [Fact()]
        public void SetFromCenterAndSizeTest()
        {
            var a = new Box2();

            a.SetFromCenterAndSize(zero2, two2);
            Assert.True(a.Min.Equals(negOne2));
            Assert.True(a.Max.Equals(one2));

            a.SetFromCenterAndSize(one2, two2);
            Assert.True(a.Min.Equals(zero2));
            Assert.True(a.Max.Equals(two2));

            a.SetFromCenterAndSize(zero2, zero2);
            Assert.True(a.Min.Equals(zero2));
            Assert.True(a.Max.Equals(zero2));
        }

        [Fact()]
        public void TranslateTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var b = new Box2(zero2.Clone(), one2.Clone());
            var c = new Box2(one2.Clone().Negate(), one2.Clone());
            var d = new Box2(one2.Clone().Negate(), zero2.Clone());

            Assert.True(a.Clone().Translate(one2).Equals(new Box2(one2, one2)));
            Assert.True(a.Clone().Translate(one2).Translate(one2.Clone().Negate()).Equals(a));
            Assert.True(d.Clone().Translate(one2).Equals(b));
            Assert.True(b.Clone().Translate(one2.Clone().Negate()).Equals(d));
        }

        [Fact()]
        public void UnionTest()
        {
            var a = new Box2(zero2.Clone(), zero2.Clone());
            var b = new Box2(zero2.Clone(), one2.Clone());
            var c = new Box2(one2.Clone().Negate(), one2.Clone());

            Assert.True(a.Clone().Union(a).Equals(a));
            Assert.True(a.Clone().Union(b).Equals(b));
            Assert.True(a.Clone().Union(c).Equals(c));
            Assert.True(b.Clone().Union(c).Equals(c));
        }
    }
}