using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class PlaneTests : BaseTests
    {

        private bool comparePlane(Plane a, Plane b, float threshold = 0.0001f)
        {
            return (a.Normal.DistanceTo(b.Normal) < threshold &&
                     Mathf.Abs(a.Constant - b.Constant) < threshold);
        }

        [Fact()]
        public void InstancingTest()
        {
            var a = new Plane();
            Assert.True(a.Normal.X == 1, "Passed!");
            Assert.True(a.Normal.Y == 0, "Passed!");
            Assert.True(a.Normal.Z == 0, "Passed!");
            Assert.True(a.Constant == 0, "Passed!");

            a = new Plane(one3.Clone(), 0);
            Assert.True(a.Normal.X == 1, "Passed!");
            Assert.True(a.Normal.Y == 1, "Passed!");
            Assert.True(a.Normal.Z == 1, "Passed!");
            Assert.True(a.Constant == 0, "Passed!");

            a = new Plane(one3.Clone(), 1);
            Assert.True(a.Normal.X == 1, "Passed!");
            Assert.True(a.Normal.Y == 1, "Passed!");
            Assert.True(a.Normal.Z == 1, "Passed!");
            Assert.True(a.Constant == 1, "Passed!");

        }

        [Fact()]
        public void CloneTest()
        {
            var a = new Plane(new Vector3(2.0f, 0.5f, 0.25f));
            var b = a.Clone();

            Assert.True(a.Equals(b), "clones are equal");

        }

        [Fact()]
        public void CopyTest()
        {
            var a = new Plane(new Vector3(x, y, z), w);
            var b = new Plane().Copy(a);
            Assert.Equal(x, b.Normal.X);
            Assert.Equal(y, b.Normal.Y);
            Assert.Equal(z, b.Normal.Z);
            Assert.Equal(w, b.Constant);

            // ensure that it is a true copy
            a.Normal.X = 0;
            a.Normal.Y = -1;
            a.Normal.Z = -2;
            a.Constant = -3;
            Assert.Equal(x, b.Normal.X);
            Assert.Equal(y, b.Normal.Y);
            Assert.Equal(z, b.Normal.Z);
            Assert.Equal(w, b.Constant);
        }

        [Fact()]
        public void SetTest()
        {
            var a = new Plane();
            Assert.True(a.Normal.X == 1, "Passed!");
            Assert.True(a.Normal.Y == 0, "Passed!");
            Assert.True(a.Normal.Z == 0, "Passed!");
            Assert.True(a.Constant == 0, "Passed!");

            var b = a.Clone().Set(new Vector3(x, y, z), w);
            Assert.True(b.Normal.X == x, "Passed!");
            Assert.True(b.Normal.Y == y, "Passed!");
            Assert.True(b.Normal.Z == z, "Passed!");
            Assert.True(b.Constant == w, "Passed!");
        }

        [Fact()]
        public void SetComponentsTest()
        {
            var a = new Plane();
            Assert.True(a.Normal.X == 1, "Passed!");
            Assert.True(a.Normal.Y == 0, "Passed!");
            Assert.True(a.Normal.Z == 0, "Passed!");
            Assert.True(a.Constant == 0, "Passed!");

            var b = a.Clone().SetComponents(x, y, z, w);
            Assert.True(b.Normal.X == x, "Passed!");
            Assert.True(b.Normal.Y == y, "Passed!");
            Assert.True(b.Normal.Z == z, "Passed!");
            Assert.True(b.Constant == w, "Passed!");
        }

        [Fact()]
        public void SetFromNormalAndCoplanarPointTest()
        {
            var normal = one3.Clone().Normalize();
            var a = new Plane().SetFromNormalAndCoplanarPoint(normal, zero3);

            Assert.True(a.Normal.Equals(normal), "Passed!");
            Assert.True(a.Constant == 0, "Passed!");
        }

        [Fact()]
        public void SetFromCoplanarPointsTest()
        {
            var a = new Plane();
            var v1 = new Vector3(2.0f, 0.5f, 0.25f);
            var v2 = new Vector3(2.0f, -0.5f, 1.25f);
            var v3 = new Vector3(2.0f, -3.5f, 2.2f);
            var normal = new Vector3(1, 0, 0);
            var constant = -2;

            a.SetFromCoplanarPoints(v1, v2, v3);

            Assert.True(a.Normal.Equals(normal), "Check normal");
            Assert.Equal(constant, a.Constant);
        }

        [Fact()]
        public void NormalizeTest()
        {
            var a = new Plane(new Vector3(2, 0, 0), 2);

            a.Normalize();
            Assert.True(a.Normal.Length() == 1, "Passed!");
            Assert.True(a.Normal.Equals(new Vector3(1, 0, 0)), "Passed!");
            Assert.True(a.Constant == 1, "Passed!");

        }

        [Fact()]
        public void NegateTest()
        {
            var a = new Plane(new Vector3(2, 0, 0), -2);

            a.Normalize();
            Assert.True(a.DistanceToPoint(new Vector3(4, 0, 0)) == 3, "Passed!");
            Assert.True(a.DistanceToPoint(new Vector3(1, 0, 0)) == 0, "Passed!");

            a.Negate();
            Assert.True(a.DistanceToPoint(new Vector3(4, 0, 0)) == -3, "Passed!");
            Assert.True(a.DistanceToPoint(new Vector3(1, 0, 0)) == 0, "Passed!");

        }

        [Fact()]
        public void DistanceToPointTest()
        {
            var a = new Plane(new Vector3(2, 0, 0), -2);
            var point = new Vector3();

            a.Normalize().ProjectPoint(zero3.Clone(), point);
            Assert.True(a.DistanceToPoint(point) == 0, "Passed!");
            Assert.True(a.DistanceToPoint(new Vector3(4, 0, 0)) == 3, "Passed!");
        }

        [Fact()]
        public void DistanceToSphereTest()
        {
            var a = new Plane(new Vector3(1, 0, 0), 0);

            var b = new Sphere(new Vector3(2, 0, 0), 1);

            Assert.True(a.DistanceToSphere(b) == 1, "Passed!");

            a.Set(new Vector3(1, 0, 0), 2);
            Assert.True(a.DistanceToSphere(b) == 3, "Passed!");
            a.Set(new Vector3(1, 0, 0), -2);
            Assert.True(a.DistanceToSphere(b) == -1, "Passed!");

        }

        [Fact()]
        public void ProjectPointTest()
        {
            var a = new Plane(new Vector3(1, 0, 0), 0);
            var point = new Vector3();

            a.ProjectPoint(new Vector3(10, 0, 0), point);
            Assert.True(point.Equals(zero3), "Passed!");
            a.ProjectPoint(new Vector3(-10, 0, 0), point);
            Assert.True(point.Equals(zero3), "Passed!");

            a = new Plane(new Vector3(0, 1, 0), -1);
            a.ProjectPoint(new Vector3(0, 0, 0), point);
            Assert.True(point.Equals(new Vector3(0, 1, 0)), "Passed!");
            a.ProjectPoint(new Vector3(0, 1, 0), point);
            Assert.True(point.Equals(new Vector3(0, 1, 0)), "Passed!");
        }

        [Fact()]
        public void IntersectLineTest()
        {
            var a = new Plane(new Vector3(1, 0, 0), 0);
            var point = new Vector3();

            var l1 = new Line3(new Vector3(-10, 0, 0), new Vector3(10, 0, 0));
            a.IntersectLine(l1, point);
            Assert.True(point.Equals(new Vector3(0, 0, 0)), "Passed!");

            a = new Plane(new Vector3(1, 0, 0), -3);
            a.IntersectLine(l1, point);
            Assert.True(point.Equals(new Vector3(3, 0, 0)), "Passed!");

        }

        [Fact()]
        public void IntersectsLineTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectsBoxTest()
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

            Assert.True(!b.IntersectsBox(a), "Passed!");
            Assert.True(!c.IntersectsBox(a), "Passed!");
            Assert.True(!d.IntersectsBox(a), "Passed!");
            Assert.True(!e.IntersectsBox(a), "Passed!");
            Assert.True(f.IntersectsBox(a), "Passed!");
            Assert.True(g.IntersectsBox(a), "Passed!");
            Assert.True(h.IntersectsBox(a), "Passed!");
            Assert.True(i.IntersectsBox(a), "Passed!");
            Assert.True(!j.IntersectsBox(a), "Passed!");
        }

        [Fact()]
        public void IntersectsSphereTest()
        {
            var a = new Sphere(zero3.Clone(), 1);
            var b = new Plane(new Vector3(0, 1, 0), 1);
            var c = new Plane(new Vector3(0, 1, 0), 1.25f);
            var d = new Plane(new Vector3(0, -1, 0), 1.25f);

            Assert.True(b.IntersectsSphere(a), "Passed!");
            Assert.True(!c.IntersectsSphere(a), "Passed!");
            Assert.True(!d.IntersectsSphere(a), "Passed!");

        }

        [Fact()]
        public void CoplanarPointTest()
        {
            var point = new Vector3();

            var a = new Plane(new Vector3(1, 0, 0), 0);
            a.CoplanarPoint(point);
            Assert.True(a.DistanceToPoint(point) == 0, "Passed!");

            a = new Plane(new Vector3(0, 1, 0), -1);
            a.CoplanarPoint(point);
            Assert.True(a.DistanceToPoint(point) == 0, "Passed!");

        }

        [Fact()]
        public void ApplyMatrix4Test()
        {
            var a = new Plane(new Vector3(1, 0, 0), 0);

            var m = new Matrix4();
            m.MakeRotationZ(Mathf.PI * 0.5f);

            Assert.True(comparePlane(a.Clone().ApplyMatrix4(m), new Plane(new Vector3(0, 1, 0), 0)), "Passed!");

            a = new Plane(new Vector3(0, 1, 0), -1);
            Assert.True(comparePlane(a.Clone().ApplyMatrix4(m), new Plane(new Vector3(-1, 0, 0), -1)), "Passed!");

            m.MakeTranslation(1, 1, 1);
            Assert.True(comparePlane(a.Clone().ApplyMatrix4(m), a.Clone().Translate(new Vector3(1, 1, 1))), "Passed!");
        }

        [Fact()]
        public void TranslateTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void EqualsTest()
        {
            var a = new Plane(new Vector3(1, 0, 0), 0);
            var b = new Plane(new Vector3(1, 0, 0), 1);
            var c = new Plane(new Vector3(0, 1, 0), 0);

            Assert.True(a.Normal.Equals(b.Normal), "Normals: equal");
            Assert.False(a.Normal.Equals(c.Normal), "Normals: not equal");

            Assert.NotEqual(a.Constant, b.Constant);
            Assert.Equal(a.Constant, c.Constant);

            Assert.False(a.Equals(b), "Planes: not equal");
            Assert.False(a.Equals(c), "Planes: not equal");

            a.Copy(b);
            Assert.True(a.Normal.Equals(b.Normal));
            Assert.Equal(a.Constant, b.Constant);
            Assert.True(a.Equals(b));

        }
    }
}