using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class RayTests : BaseTests
    {
        [Fact()]
        public void InstancingTest()
        {
            var a = new Ray();
            Assert.True(a.Origin.Equals(zero3));
            Assert.True(a.Direction.Equals(new Vector3(0, 0, -1)));

            a = new Ray(two3.Clone(), one3.Clone());
            Assert.True(a.Origin.Equals(two3));
            Assert.True(a.Direction.Equals(one3));
        }

        [Fact()]
        public void RayTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ApplyMatrix4Test()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));
            var m = new Matrix4();

            Assert.True(a.Clone().ApplyMatrix4(m).Equals(a));

            a = new Ray(zero3.Clone(), new Vector3(0, 0, 1));
            m.MakeRotationZ(Mathf.PI);
            Assert.True(a.Clone().ApplyMatrix4(m).Equals(a));

            m.MakeRotationX(Mathf.PI);
            var b = a.Clone();
            b.Direction.Negate();
            var a2 = a.Clone().ApplyMatrix4(m);
            Assert.True(a2.Origin.DistanceTo(b.Origin) < 0.0001);
            Assert.True(a2.Direction.DistanceTo(b.Direction) < 0.0001);

            a.Origin = new Vector3(0, 0, 1);
            b.Origin = new Vector3(0, 0, -1);
            a2 = a.Clone().ApplyMatrix4(m);
            Assert.True(a2.Origin.DistanceTo(b.Origin) < 0.0001);
            Assert.True(a2.Direction.DistanceTo(b.Direction) < 0.0001);
        }

        [Fact()]
        public void AtTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));
            var point = new Vector3();

            a.At(0, point);
            Assert.True(point.Equals(one3));
            a.At(-1, point);
            Assert.True(point.Equals(new Vector3(1, 1, 0)));
            a.At(1, point);
            Assert.True(point.Equals(new Vector3(1, 1, 2)));
        }

        [Fact()]
        public void Copy_EqualsTest()
        {
            var a = new Ray(zero3.Clone(), one3.Clone());
            var b = new Ray().Copy(a);
            Assert.True(b.Origin.Equals(zero3));
            Assert.True(b.Direction.Equals(one3));

            // ensure that it is a true copy
            a.Origin = zero3;
            a.Direction = one3;
            Assert.True(b.Origin.Equals(zero3));
            Assert.True(b.Direction.Equals(one3));
        }

        [Fact()]
        public void ClosestPointToPointTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));
            var point = new Vector3();

            // behind the ray
            a.ClosestPointToPoint(zero3, point);
            Assert.True(point.Equals(one3));

            // front of the ray
            a.ClosestPointToPoint(new Vector3(0, 0, 50), point);
            Assert.True(point.Equals(new Vector3(1, 1, 50)));

            // exactly on the ray
            a.ClosestPointToPoint(one3, point);
            Assert.True(point.Equals(one3));
        }

        [Fact()]
        public void DistanceToPointTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));

            // behind the ray
            var b = a.DistanceToPoint(zero3);
            Assert.Equal(Mathf.Sqrt(3), b);

            // front of the ray
            var c = a.DistanceToPoint(new Vector3(0, 0, 50));
            Assert.Equal(Mathf.Sqrt(2), c);

            // exactly on the ray
            var d = a.DistanceToPoint(one3);
            Assert.Equal(0, d);
        }

        [Fact()]
        public void DistanceSqToPointTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));

            // behind the ray
            var b = a.DistanceSqToPoint(zero3);
            Assert.Equal(3, b);

            // front of the ray
            var c = a.DistanceSqToPoint(new Vector3(0, 0, 50));
            Assert.Equal(2, c);

            // exactly on the ray
            var d = a.DistanceSqToPoint(one3);
            Assert.Equal(0, d);
        }

        [Fact()]
        public void DistanceSqToSegmentTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));
            var ptOnLine = new Vector3();
            var ptOnSegment = new Vector3();

            //segment in front of the ray
            var v0 = new Vector3(3, 5, 50);
            var v1 = new Vector3(50, 50, 50); // just a far away point
            var distSqr = a.DistanceSqToSegment(v0, v1, ptOnLine, ptOnSegment);

            Assert.True(ptOnSegment.DistanceTo(v0) < 0.0001);
            Assert.True(ptOnLine.DistanceTo(new Vector3(1, 1, 50)) < 0.0001);
            // ((3-1) * (3-1) + (5-1) * (5-1) = 4 + 16 = 20
            Assert.True(Mathf.Abs(distSqr - 20) < 0.0001);

            //segment behind the ray
             v0 = new Vector3(-50, -50, -50); // just a far away point
             v1 = new Vector3(-3, -5, -4);
             distSqr = a.DistanceSqToSegment(v0, v1, ptOnLine, ptOnSegment);

            Assert.True(ptOnSegment.DistanceTo(v1) < 0.0001);
            Assert.True(ptOnLine.DistanceTo(one3) < 0.0001);
            // ((-3-1) * (-3-1) + (-5-1) * (-5-1) + (-4-1) + (-4-1) = 16 + 36 + 25 = 77
            Assert.True(Mathf.Abs(distSqr - 77) < 0.0001);

            //exact intersection between the ray and the segment
             v0 = new Vector3(-50, -50, -50);
             v1 = new Vector3(50, 50, 50);
             distSqr = a.DistanceSqToSegment(v0, v1, ptOnLine, ptOnSegment);

            Assert.True(ptOnSegment.DistanceTo(one3) < 0.0001);
            Assert.True(ptOnLine.DistanceTo(one3) < 0.0001);
            Assert.True(distSqr < 0.0001);

        }

        [Fact()]
        public void DistanceToPlaneTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectSphereTest()
        {
            var TOL = 0.0001;
            var point = new Vector3();

            // ray a0 origin located at ( 0, 0, 0 ) and points outward in negative-z direction
            var a0 = new Ray(zero3.Clone(), new Vector3(0, 0, -1));
            // ray a1 origin located at ( 1, 1, 1 ) and points left in negative-x direction
            var a1 = new Ray(one3.Clone(), new Vector3(-1, 0, 0));

            // sphere (radius of 2) located behind ray a0, should result in null
            var b = new Sphere(new Vector3(0, 0, 3), 2);
            a0.IntersectSphere(b, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

            // sphere (radius of 2) located in front of, but too far right of ray a0, should result in null
             b = new Sphere(new Vector3(3, 0, -1), 2);
            a0.IntersectSphere(b, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

            // sphere (radius of 2) located below ray a1, should result in null
             b = new Sphere(new Vector3(1, -2, 1), 2);
            a1.IntersectSphere(b, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

            // sphere (radius of 1) located to the left of ray a1, should result in intersection at 0, 1, 1
             b = new Sphere(new Vector3(-1, 1, 1), 1);
            a1.IntersectSphere(b, point);
            Assert.True(point.DistanceTo(new Vector3(0, 1, 1)) < TOL);

            // sphere (radius of 1) located in front of ray a0, should result in intersection at 0, 0, -1
             b = new Sphere(new Vector3(0, 0, -2), 1);
            a0.IntersectSphere(b, point);
            Assert.True(point.DistanceTo(new Vector3(0, 0, -1)) < TOL);

            // sphere (radius of 2) located in front & right of ray a0, should result in intersection at 0, 0, -1, or left-most edge of sphere
             b = new Sphere(new Vector3(2, 0, -1), 2);
            a0.IntersectSphere(b, point);
            Assert.True(point.DistanceTo(new Vector3(0, 0, -1)) < TOL);

            // same situation as above, but move the sphere a fraction more to the right, and ray a0 should now just miss
             b = new Sphere(new Vector3((float)2.01, 0, -1), 2);
            a0.IntersectSphere(b, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

            // following QUnit.tests are for situations where the ray origin is inside the sphere

            // sphere (radius of 1) center located at ray a0 origin / sphere surrounds the ray origin, so the first intersect point 0, 0, 1,
            // is behind ray a0.  Therefore, second exit point on back of sphere will be returned: 0, 0, -1
            // thus keeping the intersection point always in front of the ray.
             b = new Sphere(zero3.Clone(), 1);
            a0.IntersectSphere(b, point);
            Assert.True(point.DistanceTo(new Vector3(0, 0, -1)) < TOL);

            // sphere (radius of 4) center located behind ray a0 origin / sphere surrounds the ray origin, so the first intersect point 0, 0, 5,
            // is behind ray a0.  Therefore, second exit point on back of sphere will be returned: 0, 0, -3
            // thus keeping the intersection point always in front of the ray.
             b = new Sphere(new Vector3(0, 0, 1), 4);
            a0.IntersectSphere(b, point);
            Assert.True(point.DistanceTo(new Vector3(0, 0, -3)) < TOL);

            // sphere (radius of 4) center located in front of ray a0 origin / sphere surrounds the ray origin, so the first intersect point 0, 0, 3,
            // is behind ray a0.  Therefore, second exit point on back of sphere will be returned: 0, 0, -5
            // thus keeping the intersection point always in front of the ray.
             b = new Sphere(new Vector3(0, 0, -1), 4);
            a0.IntersectSphere(b, point);
            Assert.True(point.DistanceTo(new Vector3(0, 0, -5)) < TOL);

        }

        [Fact()]
        public void IntersectsSphereTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));
            var b = new Sphere(zero3, (float)0.5);
            var c = new Sphere(zero3, (float)1.5);
            var d = new Sphere(one3, (float)0.1);
            var e = new Sphere(two3, (float)0.1);
            var f = new Sphere(two3, 1);

            Assert.True(!a.IntersectsSphere(b));
            Assert.True(!a.IntersectsSphere(c));
            Assert.True(a.IntersectsSphere(d));
            Assert.True(!a.IntersectsSphere(e));
            Assert.True(!a.IntersectsSphere(f));
        }

        [Fact()]
        public void IntersectPlaneTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));
            var point = new Vector3();

            // parallel plane behind
            var b = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(0, 0, 1), new Vector3(1, 1, -1));
            a.IntersectPlane(b, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

            // parallel plane coincident with origin
            var c = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(0, 0, 1), new Vector3(1, 1, 0));
            a.IntersectPlane(c, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

            // parallel plane infront
            var d = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(0, 0, 1), new Vector3(1, 1, 1));
            a.IntersectPlane(d, point.Copy(posInf3));
            Assert.True(point.Equals(a.Origin));

            // perpendical ray that overlaps exactly
            var e = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(1, 0, 0), one3);
            a.IntersectPlane(e, point.Copy(posInf3));
            Assert.True(point.Equals(a.Origin));

            // perpendical ray that doesn't overlap
            var f = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(1, 0, 0), zero3);
            a.IntersectPlane(f, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

        }

        [Fact()]
        public void IntersectsPlaneTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));

            // parallel plane in front of the ray
            var b = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(0, 0, 1), one3.Clone().Sub(new Vector3(0, 0, -1)));
            Assert.True(a.IntersectsPlane(b));

            // parallel plane coincident with origin
            var c = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(0, 0, 1), one3.Clone().Sub(new Vector3(0, 0, 0)));
            Assert.True(a.IntersectsPlane(c));

            // parallel plane behind the ray
            var d = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(0, 0, 1), one3.Clone().Sub(new Vector3(0, 0, 1)));
            Assert.True(!a.IntersectsPlane(d));

            // perpendical ray that overlaps exactly
            var e = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(1, 0, 0), one3);
            Assert.True(a.IntersectsPlane(e));

            // perpendical ray that doesn't overlap
            var f = new Plane().SetFromNormalAndCoplanarPoint(new Vector3(1, 0, 0), zero3);
            Assert.True(!a.IntersectsPlane(f));

        }

        [Fact()]
        public void IntersectBoxTest()
        {
            var TOL = 0.0001;

            var box = new Box3(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            var point = new Vector3();

            var a = new Ray(new Vector3(-2, 0, 0), new Vector3(1, 0, 0));
            //ray should intersect box at -1,0,0
            Assert.True(a.IntersectsBox(box));
            a.IntersectBox(box, point);
            Assert.True(point.DistanceTo(new Vector3(-1, 0, 0)) < TOL);

            var b = new Ray(new Vector3(-2, 0, 0), new Vector3(-1, 0, 0));
            //ray is point away from box, it should not intersect
            Assert.False(b.IntersectsBox(box));
            b.IntersectBox(box, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

            var c = new Ray(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            // ray is inside box, should return exit point
            Assert.True(c.IntersectsBox(box));
            c.IntersectBox(box, point);
            Assert.True(point.DistanceTo(new Vector3(1, 0, 0)) < TOL);

            var d = new Ray(new Vector3(0, 2, 1), new Vector3(0, -1, -1).Normalize());
            //tilted ray should intersect box at 0,1,0
            Assert.True(d.IntersectsBox(box));
            d.IntersectBox(box, point);
            Assert.True(point.DistanceTo(new Vector3(0, 1, 0)) < TOL);

            var e = new Ray(new Vector3(1, -2, 1), new Vector3(0, 1, 0).Normalize());
            //handle case where ray is coplanar with one of the boxes side - box in front of ray
            Assert.True(e.IntersectsBox(box));
            e.IntersectBox(box, point);
            Assert.True(point.DistanceTo(new Vector3(1, -1, 1)) < TOL);

            var f = new Ray(new Vector3(1, -2, 0), new Vector3(0, -1, 0).Normalize());
            //handle case where ray is coplanar with one of the boxes side - box behind ray
            Assert.False(f.IntersectsBox(box));
            f.IntersectBox(box, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3));

        }

        [Fact()]
        public void IntersectsBoxTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectTriangleTest()
        {
            var ray = new Ray();
            var a = new Vector3(1, 1, 0);
            var b = new Vector3(0, 1, 1);
            var c = new Vector3(1, 0, 1);
            var point = new Vector3();

            // DdN == 0
            ray.Set(ray.Origin, zero3.Clone());
            ray.IntersectTriangle(a, b, c, false, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3), "No intersection if direction == zero");

            // DdN > 0, backfaceCulling = true
            ray.Set(ray.Origin, one3.Clone());
            ray.IntersectTriangle(a, b, c, true, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3), "No intersection with backside faces if backfaceCulling is true");

            // DdN > 0
            ray.Set(ray.Origin, one3.Clone());
            ray.IntersectTriangle(a, b, c, false, point);
            Assert.True(Mathf.Abs(point.X - (float)2 / 3) <= MathUtils.EPS5);
            Assert.True(Mathf.Abs(point.Y - (float)2 / 3) <= MathUtils.EPS5);
            Assert.True(Mathf.Abs(point.Z - (float)2 / 3) <= MathUtils.EPS5);

            // DdN > 0, DdQxE2 < 0
            b.MultiplyScalar(-1);
            ray.IntersectTriangle(a, b, c, false, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3), "No intersection");

            // DdN > 0, DdE1xQ < 0
            a.MultiplyScalar(-1);
            ray.IntersectTriangle(a, b, c, false, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3), "No intersection");

            // DdN > 0, DdQxE2 + DdE1xQ > DdN
            b.MultiplyScalar(-1);
            ray.IntersectTriangle(a, b, c, false, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3), "No intersection");

            // DdN < 0, QdN < 0
            a.MultiplyScalar(-1);
            b.MultiplyScalar(-1);
            ray.Direction.MultiplyScalar(-1);
            ray.IntersectTriangle(a, b, c, false, point.Copy(posInf3));
            Assert.True(point.Equals(posInf3), "No intersection when looking in the wrong direction");

        }

        [Fact()]
        public void LookAtTest()
        {
            var a = new Ray(two3.Clone(), one3.Clone());
            var target = one3.Clone();
            var expected = target.Sub(two3).Normalize();

            a.LookAt(target);
            Assert.True(a.Direction.Equals(expected), "Check if we're looking in the right direction");
        }

        [Fact()]
        public void Recast_CloneTest()
        {
            var a = new Ray(one3.Clone(), new Vector3(0, 0, 1));

            Assert.True(a.Recast(0).Equals(a));

            var b = a.Clone();
            Assert.True(b.Recast(-1).Equals(new Ray(new Vector3(1, 1, 0), new Vector3(0, 0, 1))));

            var c = a.Clone();
            Assert.True(c.Recast(1).Equals(new Ray(new Vector3(1, 1, 2), new Vector3(0, 0, 1))));

            var d = a.Clone();
            var e = d.Clone().Recast(1);
            Assert.True(d.Equals(a));
            Assert.True(!e.Equals(d));
            Assert.True(e.Equals(c));

        }

        [Fact()]
        public void SetTest()
        {
            var a = new Ray();

            a.Set(one3, one3);
            Assert.True(a.Origin.Equals(one3));
            Assert.True(a.Direction.Equals(one3));
        }
    }
}