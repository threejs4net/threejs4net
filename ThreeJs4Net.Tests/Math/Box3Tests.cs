using ThreeJs4Net.Core;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class Box3Tests : BaseTests
    {
        private bool compareBox(Box3 a, Box3 b, float threshold = 0.0001f)
        {
            return (a.Min.DistanceTo(b.Min) < threshold &&
                     a.Max.DistanceTo(b.Max) < threshold);

        }


        [Fact()]
        public void InstancingTest()
        {
            var a = new Box3();
            Assert.Equal(posInf3, a.Min);
            Assert.Equal(negInf3, a.Max);

            a = new Box3(zero3.Clone(), zero3.Clone());
            Assert.Equal(zero3, a.Min);
            Assert.Equal(zero3, a.Max);

            a = new Box3(zero3.Clone(), one3.Clone());
            Assert.Equal(zero3, a.Min);
            Assert.Equal(one3, a.Max);
        }

        [Fact()]
        public void ApplyMatrix4Test()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(zero3.Clone(), one3.Clone());
            var c = new Box3(one3.Clone().Negate(), one3.Clone());
            var d = new Box3(one3.Clone().Negate(), zero3.Clone());

            var m = new Matrix4().MakeTranslation(1, -2, 1);
            var t1 = new Vector3(1, -2, 1);

            Assert.True(compareBox(a.Clone().ApplyMatrix4(m), a.Clone().Translate(t1)));
            Assert.True(compareBox(b.Clone().ApplyMatrix4(m), b.Clone().Translate(t1)));
            Assert.True(compareBox(c.Clone().ApplyMatrix4(m), c.Clone().Translate(t1)));
            Assert.True(compareBox(d.Clone().ApplyMatrix4(m), d.Clone().Translate(t1)));

            d.MakeEmpty();
            Assert.True(d.IsEmpty());
        }

        [Fact()]
        public void ClampPointTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(one3.Clone().Negate(), one3.Clone());
            var point = new Vector3();

            a.ClampPoint(zero3, point);
            Assert.True(point.Equals(zero3));
            a.ClampPoint(one3, point);
            Assert.True(point.Equals(zero3));
            a.ClampPoint(one3.Clone().Negate(), point);
            Assert.True(point.Equals(zero3));

            b.ClampPoint(new Vector3(2, 2, 2), point);
            Assert.True(point.Equals(one3));
            b.ClampPoint(one3, point);
            Assert.True(point.Equals(one3));
            b.ClampPoint(zero3, point);
            Assert.True(point.Equals(zero3));
            b.ClampPoint(one3.Clone().Negate(), point);
            Assert.True(point.Equals(one3.Clone().Negate()));
            b.ClampPoint(new Vector3(-2, -2, -2), point);
            Assert.True(point.Equals(one3.Clone().Negate()));
        }

        [Fact()]
        public void CloneTest()
        {
            var a = new Box3(zero3.Clone(), one3.Clone());

            var b = a.Clone();
            Assert.True(b.Min.Equals(zero3));
            Assert.True(b.Max.Equals(one3));

            a = new Box3();
            b = a.Clone();
            Assert.True(b.Min.Equals(posInf3));
            Assert.True(b.Max.Equals(negInf3));
        }

        [Fact()]
        public void ContainsBoxTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(zero3.Clone(), one3.Clone());
            var c = new Box3(one3.Clone().Negate(), one3.Clone());

            Assert.True(a.ContainsBox(a));
            Assert.True(!a.ContainsBox(b));
            Assert.True(!a.ContainsBox(c));

            Assert.True(b.ContainsBox(a));
            Assert.True(c.ContainsBox(a));
            Assert.True(!b.ContainsBox(c));
        }

        [Fact()]
        public void ContainsPointTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());

            Assert.True(a.ContainsPoint(zero3));
            Assert.True(!a.ContainsPoint(one3));

            a.ExpandByScalar(1);
            Assert.True(a.ContainsPoint(zero3));
            Assert.True(a.ContainsPoint(one3));
            Assert.True(a.ContainsPoint(one3.Clone().Negate()));
        }

        [Fact()]
        public void CopyTest()
        {
            var a = new Box3(zero3.Clone(), one3.Clone());
            var b = new Box3().Copy(a);
            Assert.True(b.Min.Equals(zero3));
            Assert.True(b.Max.Equals(one3));

            // ensure that it is a true copy
            a.Min = zero3;
            a.Max = one3;
            Assert.True(b.Min.Equals(zero3));
            Assert.True(b.Max.Equals(one3));
        }

        [Fact()]
        public void DistanceToPointTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(one3.Clone().Negate(), one3.Clone());

            Assert.Equal(0, a.DistanceToPoint(new Vector3(0, 0, 0)));
            Assert.Equal(Mathf.Sqrt(3), a.DistanceToPoint(new Vector3(1, 1, 1)));
            Assert.Equal(Mathf.Sqrt(3), a.DistanceToPoint(new Vector3(-1, -1, -1)));

            Assert.Equal(Mathf.Sqrt(3), b.DistanceToPoint(new Vector3(2, 2, 2)));
            Assert.Equal(0, b.DistanceToPoint(new Vector3(1, 1, 1)));
            Assert.Equal(0, b.DistanceToPoint(new Vector3(0, 0, 0)));
            Assert.Equal(0, b.DistanceToPoint(new Vector3(-1, -1, -1)));
            Assert.Equal(Mathf.Sqrt(3), b.DistanceToPoint(new Vector3(-2, -2, -2)));
        }

        [Fact()]
        public void ExpandByObjectTest()
        {
            var a = new Box3(zero3.Clone(), one3.Clone());
            var b = a.Clone();
            var bigger = new Mesh(new BoxGeometry(2, 2, 2));
            var smaller = new Mesh(new BoxGeometry(0.5f, 0.5f, 0.5f));
            var child = new Mesh(new BoxGeometry(1, 1, 1));

            // just a bigger box to begin with
            a.ExpandByObject(bigger);
            Assert.Equal(new Vector3(-1, -1, -1), a.Min);
            Assert.Equal(new Vector3(1, 1, 1), a.Max);

            // a translated, bigger box
            a.Copy(b);
            bigger.TranslateX(2);
            a.ExpandByObject(bigger);
            Assert.True(a.Min.Equals(new Vector3(0, -1, -1)), "Translated, bigger box: correct new minimum");
            Assert.True(a.Max.Equals(new Vector3(3, 1, 1)), "Translated, bigger box: correct new maximum");

            // a translated, bigger box with child
            a.Copy(b);
            bigger.Add(child);
            a.ExpandByObject(bigger);
            Assert.True(a.Min.Equals(new Vector3(0, -1, -1)), "Translated, bigger box with child: correct new minimum");
            Assert.True(a.Max.Equals(new Vector3(3, 1, 1)), "Translated, bigger box with child: correct new maximum");

            // a translated, bigger box with a translated child
            a.Copy(b);
            child.TranslateX(2);
            a.ExpandByObject(bigger);
            Assert.True(a.Min.Equals(new Vector3(0, -1, -1)), "Translated, bigger box with translated child: correct new minimum");
            Assert.True(a.Max.Equals(new Vector3(4.5f, 1, 1)), "Translated, bigger box with translated child: correct new maximum");

            // a smaller box
            a.Copy(b);
            a.ExpandByObject(smaller);
            Assert.True(a.Min.Equals(new Vector3(-0.25f, -0.25f, -0.25f)), "Smaller box: correct new minimum");
            Assert.True(a.Max.Equals(new Vector3(1, 1, 1)), "Smaller box: correct new maximum");

            //
            Assert.True(new Box3().ExpandByObject(new Mesh()).IsEmpty(), "The AABB of a mesh with inital geometry is empty.");
        }

        [Fact()]
        public void ExpandByPointTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var center = new Vector3();
            var size = new Vector3();

            a.ExpandByPoint(zero3);
            Assert.True(a.GetSize(size).Equals(zero3));

            a.ExpandByPoint(one3);
            Assert.True(a.GetSize(size).Equals(one3));

            a.ExpandByPoint(one3.Clone().Negate());
            Assert.True(a.GetSize(size).Equals(one3.Clone().MultiplyScalar(2)));
            Assert.True(a.GetCenter(center).Equals(zero3));
        }

        [Fact()]
        public void ExpandByScalarTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var center = new Vector3();
            var size = new Vector3();

            a.ExpandByScalar(0);
            Assert.True(a.GetSize(size).Equals(zero3));

            a.ExpandByScalar(1);
            Assert.True(a.GetSize(size).Equals(one3.Clone().MultiplyScalar(2)));
            Assert.True(a.GetCenter(center).Equals(zero3));

        }

        [Fact()]
        public void ExpandByVectorTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var center = new Vector3();
            var size = new Vector3();

            a.ExpandByVector(zero3);
            Assert.True(a.GetSize(size).Equals(zero3));

            a.ExpandByVector(one3);
            Assert.True(a.GetSize(size).Equals(one3.Clone().MultiplyScalar(2)));
            Assert.True(a.GetCenter(center).Equals(zero3));
        }

        [Fact()]
        public void GetBoundingSphereTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(zero3.Clone(), one3.Clone());
            var c = new Box3(one3.Clone().Negate(), one3.Clone());
            var sphere = new Sphere();


            var xxx = a.GetBoundingSphere(sphere);
            var ggg = xxx.Equals(new Sphere(zero3, 0));

            Assert.True(a.GetBoundingSphere(sphere).Equals(new Sphere(zero3, 0)));
            Assert.True(b.GetBoundingSphere(sphere).Equals(new Sphere(one3.Clone().MultiplyScalar(0.5f), Mathf.Sqrt(3) * 0.5f)));
            Assert.True(c.GetBoundingSphere(sphere).Equals(new Sphere(zero3, Mathf.Sqrt(12) * 0.5f)));

        }

        [Fact()]
        public void GetCenterTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var center = new Vector3();

            Assert.True(a.GetCenter(center).Equals(zero3));

            a = new Box3(zero3.Clone(), one3.Clone());
            var midpoint = one3.Clone().MultiplyScalar(0.5f);
            Assert.True(a.GetCenter(center).Equals(midpoint));

        }

        [Fact()]
        public void GetParameterTest()
        {
            var a = new Box3(zero3.Clone(), one3.Clone());
            var b = new Box3(one3.Clone().Negate(), one3.Clone());
            var parameter = new Vector3();

            a.GetParameter(zero3, parameter);
            Assert.True(parameter.Equals(zero3));
            a.GetParameter(one3, parameter);
            Assert.True(parameter.Equals(one3));

            b.GetParameter(one3.Clone().Negate(), parameter);
            Assert.True(parameter.Equals(zero3));
            b.GetParameter(zero3, parameter);
            Assert.True(parameter.Equals(new Vector3(0.5f, 0.5f, 0.5f)));
            b.GetParameter(one3, parameter);
            Assert.True(parameter.Equals(one3));

        }

        [Fact()]
        public void GetSizeTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var size = new Vector3();

            Assert.True(a.GetSize(size).Equals(zero3));

            a = new Box3(zero3.Clone(), one3.Clone());
            Assert.True(a.GetSize(size).Equals(one3));
        }

        [Fact()]
        public void IntersectTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(zero3.Clone(), one3.Clone());
            var c = new Box3(one3.Clone().Negate(), one3.Clone());

            Assert.True(a.Clone().Intersect(a).Equals(a));
            Assert.True(a.Clone().Intersect(b).Equals(a));
            Assert.True(b.Clone().Intersect(b).Equals(b));
            Assert.True(a.Clone().Intersect(c).Equals(a));
            Assert.True(b.Clone().Intersect(c).Equals(b));
            Assert.True(c.Clone().Intersect(c).Equals(c));

        }

        [Fact()]
        public void IntersectsBoxTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(zero3.Clone(), one3.Clone());
            var c = new Box3(one3.Clone().Negate(), one3.Clone());

            Assert.True(a.IntersectsBox(a));
            Assert.True(a.IntersectsBox(b));
            Assert.True(a.IntersectsBox(c));

            Assert.True(b.IntersectsBox(a));
            Assert.True(c.IntersectsBox(a));
            Assert.True(b.IntersectsBox(c));

            b.Translate(new Vector3(2, 2, 2));
            Assert.True(!a.IntersectsBox(b));
            Assert.True(!b.IntersectsBox(a));
            Assert.True(!b.IntersectsBox(c));
        }

        [Fact()]
        public void IntersectsSphereTest()
        {
            var a = new Box3(zero3.Clone(), one3.Clone());
            var b = new Sphere(zero3.Clone(), 1);

            Assert.True(a.IntersectsSphere(b));

            b.Translate(new Vector3(2, 2, 2));
            Assert.True(!a.IntersectsSphere(b));
        }

        [Fact()]
        public void IntersectsPlaneTest()
        {
            var a = new Box3(zero3.Clone(), one3.Clone());
            var b = new Plane(new Vector3(0, 1, 0), 1);
            var c = new Plane(new Vector3(0, 1, 0), 1.25f);
            var d = new Plane(new Vector3(0, -1, 0), 1.25f);
            var e = new Plane(new Vector3(0, 1, 0), 0.25f);
            var f = new Plane(new Vector3(0, 1, 0), -0.25f);
            var g = new Plane(new Vector3(0, 1, 0), -0.75f);
            var h = new Plane(new Vector3(0, 1, 0), -1);
            var i = new Plane(new Vector3(1, 1, 1).Normalize(), -1.732f);
            var j = new Plane(new Vector3(1, 1, 1).Normalize(), -1.733f);

            Assert.True(!a.IntersectsPlane(b));
            Assert.True(!a.IntersectsPlane(c));
            Assert.True(!a.IntersectsPlane(d));
            Assert.True(!a.IntersectsPlane(e));
            Assert.True(a.IntersectsPlane(f));
            Assert.True(a.IntersectsPlane(g));
            Assert.True(a.IntersectsPlane(h));
            Assert.True(a.IntersectsPlane(i));
            Assert.True(!a.IntersectsPlane(j));
        }

        [Fact()]
        public void IsEmptyTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            Assert.True(!a.IsEmpty());

            a = new Box3(zero3.Clone(), one3.Clone());
            Assert.True(!a.IsEmpty());

            a = new Box3(two3.Clone(), one3.Clone());
            Assert.True(a.IsEmpty());

            a = new Box3(posInf3.Clone(), negInf3.Clone());
            Assert.True(a.IsEmpty());
        }

        [Fact()]
        public void MakeEmptyTest()
        {
            var a = new Box3();

            Assert.True(a.IsEmpty());

            a = new Box3(zero3.Clone(), one3.Clone());
            Assert.True(!a.IsEmpty());

            a.MakeEmpty();
            Assert.True(a.IsEmpty());
        }

        [Fact()]
        public void SetTest()
        {
            var a = new Box3();

            a.Set(zero3, one3);
            Assert.Equal(zero3, a.Min);
            Assert.Equal(one3, a.Max);
        }

        [Fact()]
        public void SetFromBufferAttributeTest()
        {
            var a = new Box3(zero3.Clone(), one3.Clone());
            var bigger = new BufferAttribute<float>(new float[] { -2, -2, -2, 2, 2, 2, 1.5f, 1.5f, 1.5f, 0, 0, 0 }, 3);
            var smaller = new BufferAttribute<float>(new float[] { -0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f, 0, 0, 0 }, 3);
            var newMin = new Vector3(-2, -2, -2);
            var newMax = new Vector3(2, 2, 2);

            a.SetFromBufferAttribute(bigger);
            Assert.Equal(newMin, a.Min);
            Assert.Equal(newMax, a.Max);

            newMin.Set(-0.5f, -0.5f, -0.5f);
            newMax.Set(0.5f, 0.5f, 0.5f);

            a.SetFromBufferAttribute(smaller);
            Assert.Equal(newMin, a.Min);
            Assert.Equal(newMax, a.Max);
        }

        [Fact()]
        public void SetFromPointsTest()
        {
            var a = new Box3();

            a.SetFromPoints(new Vector3[] { zero3, one3, two3 });
            Assert.Equal(zero3, a.Min);
            Assert.Equal(two3, a.Max);

            a.SetFromPoints(new Vector3[] { one3 });
            Assert.Equal(one3, a.Min);
            Assert.Equal(one3, a.Max);

            a.SetFromPoints(new Vector3[] { });
            Assert.True(a.IsEmpty());

        }

        [Fact()]
        public void SetFromArrayTest()
        {
            var a = new Box3();

            a.SetFromArray(new float[] { 0, 0, 0, 1, 1, 1, 2, 2, 2 });
            Assert.Equal(zero3, a.Min);
            Assert.Equal(two3, a.Max);
        }



        [Fact()]
        public void SetFromCenterAndSizeTest()
        {
            var a = new Box3(zero3.Clone(), one3.Clone());
            var b = a.Clone();
            var centerA = new Vector3();
            var sizeA = new Vector3();
            var sizeB = new Vector3();
            var newCenter = one3;
            var newSize = two3;

            a.GetCenter(centerA);
            a.GetSize(sizeA);
            a.SetFromCenterAndSize(centerA, sizeA);
            Assert.Equal(b, a);

            a.SetFromCenterAndSize(newCenter, sizeA);
            a.GetCenter(centerA);
            a.GetSize(sizeA);
            b.GetSize(sizeB);

            Assert.Equal(newCenter, centerA);
            Assert.Equal(sizeB, sizeA);
            Assert.NotEqual(b, a);

            a.SetFromCenterAndSize(centerA, newSize);
            a.GetCenter(centerA);
            a.GetSize(sizeA);
            Assert.Equal(newCenter, centerA);
            Assert.Equal(newSize, sizeA);
            Assert.NotEqual(b, a);
        }

        [Fact()]
        public void TranslateTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(zero3.Clone(), one3.Clone());
            var c = new Box3(one3.Clone().Negate(), one3.Clone());
            var d = new Box3(one3.Clone().Negate(), zero3.Clone());

            Assert.True(a.Clone().Translate(one3).Equals(new Box3(one3, one3)));
            Assert.True(a.Clone().Translate(one3).Translate(one3.Clone().Negate()).Equals(a));
            Assert.True(d.Clone().Translate(one3).Equals(b));
            Assert.True(b.Clone().Translate(one3.Clone().Negate()).Equals(d));
        }

        [Fact()]
        public void UnionTest()
        {
            var a = new Box3(zero3.Clone(), zero3.Clone());
            var b = new Box3(zero3.Clone(), one3.Clone());
            var c = new Box3(one3.Clone().Negate(), one3.Clone());

            Assert.True(a.Clone().Union(a).Equals(a));
            Assert.True(a.Clone().Union(b).Equals(b));
            Assert.True(a.Clone().Union(c).Equals(c));
            Assert.True(b.Clone().Union(c).Equals(c));
        }

        [Fact()]
        public void EqualsTest()
        {
            var a = new Box3();
            var b = new Box3();
            Assert.True(b.Equals(a));
            Assert.True(a.Equals(b));

            a = new Box3(one3, two3);
            b = new Box3(one3, two3);
            Assert.True(b.Equals(a));
            Assert.True(a.Equals(b));

            a = new Box3(one3, two3);
            b = a.Clone();
            Assert.True(b.Equals(a));
            Assert.True(a.Equals(b));

            a = new Box3(one3, two3);
            b = new Box3(one3, one3);
            Assert.True(!b.Equals(a));
            Assert.True(!a.Equals(b));

            a = new Box3();
            b = new Box3(one3, one3);
            Assert.True(!b.Equals(a));
            Assert.True(!a.Equals(b));

            Assert.False(a.Equals(null));
        }
    }
}