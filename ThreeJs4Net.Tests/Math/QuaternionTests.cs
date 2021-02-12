using ThreeJs4Net.Core;
using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class QuaternionTests : BaseTests
    {
        private Euler eulerAngles = new Euler((float)0.1, (float)-0.3, (float)0.25);
        private Euler.RotationOrder[] orders = new[]
        {
            Euler.RotationOrder.XYZ, Euler.RotationOrder.XZY, Euler.RotationOrder.YXZ,
            Euler.RotationOrder.YZX, Euler.RotationOrder.ZXY, Euler.RotationOrder.ZYX
        };

        private Euler ChangeEulerOrder(Euler euler, Euler.RotationOrder order)
        {
            return new Euler(euler.X, euler.Y, euler.Z, order);
        }

        private Quaternion qSub(Quaternion a, Quaternion b)
        {
            var result = new Quaternion();
            result.Copy(a);

            result.X -= b.X;
            result.Y -= b.Y;
            result.Z -= b.Z;
            result.W -= b.W;

            return result;
        }

        [Fact()]
        public void InstancingTest()
        {
            var a = new Quaternion();
            Assert.Equal(0, a.X);
            Assert.Equal(0, a.Y);
            Assert.Equal(0, a.Z);
            Assert.Equal(1, a.W);

            a = new Quaternion(x, y, z, w);
            Assert.Equal(x, a.X);
            Assert.Equal(y, a.Y);
            Assert.Equal(z, a.Z);
            Assert.Equal(w, a.W);
        }

        [Fact()]
        public void AngleToTest()
        {
            var a = new Quaternion();
            var b = new Quaternion().SetFromEuler(new Euler(0, Mathf.PI, 0));
            var c = new Quaternion().SetFromEuler(new Euler(0, Mathf.PI * 2, 0));

            Assert.Equal(0, a.AngleTo(a));
            Assert.True(Mathf.Abs(Mathf.PI - a.AngleTo(b)) <= MathUtils.EPS);
            Assert.Equal(0, a.AngleTo(c));
        }

        [Fact()]
        public void CopyTest()
        {
            var a = new Quaternion(x, y, z, w);
            var b = new Quaternion().Copy(a);
            Assert.Equal(x, b.X);
            Assert.Equal(y, b.Y);
            Assert.Equal(z, b.Z);
            Assert.Equal(w, b.W);

            // ensure that it is a true copy
            a.X = 0;
            a.Y = -1;
            a.Z = 0;
            a.W = -1;
            Assert.Equal(x, b.X);
            Assert.Equal(y, b.Y);
        }

        [Fact()]
        public void CloneTest()
        {
            var a = new Quaternion().Clone();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);
            Assert.True(a.Z == 0);
            Assert.True(a.W == 1);

            var b = a.Set(x, y, z, w).Clone();
            Assert.True(b.X == x);
            Assert.True(b.Y == y);
            Assert.True(b.Z == z);
            Assert.True(b.W == w);
        }

        [Fact()]
        public void DotTest()
        {
            var a = new Quaternion();
            var b = new Quaternion();

            Assert.True(a.Dot(b) == 1);
            a = new Quaternion(1, 2, 3, 1);
            b = new Quaternion(3, 2, 1, 1);

            Assert.True(a.Dot(b) == 11);

        }

        [Fact()]
        public void EqualsTest()
        {
            var a = new Quaternion(x, y, z, w);
            var b = new Quaternion(-x, -y, -z, -w);

            Assert.True(a.X != b.X);
            Assert.True(a.Y != b.Y);

            Assert.True(!a.Equals(b));
            Assert.True(!b.Equals(a));

            a.Copy(b);
            Assert.True(a.X == b.X);
            Assert.True(a.Y == b.Y);

            Assert.True(a.Equals(b));
            Assert.True(b.Equals(a));

        }

        [Fact()]
        public void FromArrayTest()
        {
            var a = new Quaternion();
            a.FromArray(new float[] { x, y, z, w });
            Assert.Equal(x, a.X);
            Assert.Equal(y, a.Y);
            Assert.Equal(z, a.Z);
            Assert.Equal(w, a.W);

            a.FromArray(new float[] { -1, x, y, z, w, -2 }, 1);
            Assert.Equal(x, a.X);
            Assert.Equal(y, a.Y);
            Assert.Equal(z, a.Z);
            Assert.Equal(w, a.W);
        }

        [Fact()]
        public void FromBufferAttributeTest()
        {
            var a = new Quaternion();

            var attribute = new BufferAttribute<float>(new float[] { 0, 0, 0, 1, (float).7, 0, 0, (float).7, 0, (float).7, 0, (float).7, }, 4);

            a.FromBufferAttribute(attribute, 0);
            Assert.Equal(0, a.X);
            Assert.Equal(0, a.Y);
            Assert.Equal(0, a.Z);
            Assert.Equal(1, a.W);

            a.FromBufferAttribute(attribute, 1);
            Assert.True(Mathf.Abs(a.X - (float)0.7) <= MathUtils.EPS);
            Assert.Equal(0, a.Y);
            Assert.Equal(0, a.Z);
            Assert.True(Mathf.Abs(a.W - (float)0.7) <= MathUtils.EPS);

            a.FromBufferAttribute(attribute, 2);
            Assert.Equal(0, a.X);
            Assert.True(Mathf.Abs(a.Y - (float)0.7) <= MathUtils.EPS);
            Assert.Equal(0, a.Z);
            Assert.True(Mathf.Abs(a.W - (float)0.7) <= MathUtils.EPS);
        }

        [Fact()]
        public void InverseTest()
        {
            var a = new Quaternion(x, y, z, w);

            // TODO: add better validation here.

            var b = a.Clone().Conjugate();

            Assert.True(a.X == -b.X);
            Assert.True(a.Y == -b.Y);
            Assert.True(a.Z == -b.Z);
            Assert.True(a.W == b.W);
        }

        [Fact()]
        public void MultiplyQuaternionsTest()
        {
            var angles = new Euler[] { new Euler(1, 0, 0), new Euler(0, 1, 0), new Euler(0, 0, 1) };

            var q1 = new Quaternion().SetFromEuler(ChangeEulerOrder(angles[0], Euler.RotationOrder.XYZ));
            var q2 = new Quaternion().SetFromEuler(ChangeEulerOrder(angles[1], Euler.RotationOrder.XYZ));
            var q3 = new Quaternion().SetFromEuler(ChangeEulerOrder(angles[2], Euler.RotationOrder.XYZ));

            var q = new Quaternion().MultiplyQuaternions(q1, q2).Multiply(q3);

            var m1 = new Matrix4().MakeRotationFromEuler(ChangeEulerOrder(angles[0], Euler.RotationOrder.XYZ));
            var m2 = new Matrix4().MakeRotationFromEuler(ChangeEulerOrder(angles[1], Euler.RotationOrder.XYZ));
            var m3 = new Matrix4().MakeRotationFromEuler(ChangeEulerOrder(angles[2], Euler.RotationOrder.XYZ));

            var m = new Matrix4().MultiplyMatrices(m1, m2).Multiply(m3);

            var qFromM = new Quaternion().SetFromRotationMatrix(m);

            Assert.True(qSub(q, qFromM).Length() < 0.001);
        }

        [Fact()]
        public void NormalizeTest()
        {
            var a = new Quaternion(x, y, z, w);

            Assert.True(a.Length() != 1);
            Assert.True(a.LengthSq() != 1);
            a.Normalize();
            Assert.True(a.Length() == 1);
            Assert.True(a.LengthSq() == 1);

            a.Set(0, 0, 0, 0);
            Assert.True(a.LengthSq() == 0);
            Assert.True(a.Length() == 0);
            a.Normalize();
            Assert.True(a.LengthSq() == 1);
            Assert.True(a.Length() == 1);
        }

        [Fact()]
        public void PreMultiplyTest()
        {
            var a = new Quaternion(x, y, z, w);
            var b = new Quaternion(2 * x, -y, -2 * z, w);
            var expected = new Quaternion(42, -32, -2, 58);

            a.PreMultiply(b);
            Assert.True(Mathf.Abs(a.X - expected.X) <= MathUtils.EPS, "Check x");
            Assert.True(Mathf.Abs(a.Y - expected.Y) <= MathUtils.EPS, "Check y");
            Assert.True(Mathf.Abs(a.Z - expected.Z) <= MathUtils.EPS, "Check z");
            Assert.True(Mathf.Abs(a.W - expected.W) <= MathUtils.EPS, "Check w");
        }

        [Fact()]
        public void RotateTowardsTest()
        {
            var a = new Quaternion();
            var b = new Quaternion().SetFromEuler(new Euler(0, Mathf.PI, 0));
            var c = new Quaternion();

            float halfPI = Mathf.PI * (float)0.5;

            a.RotateTowards(b, 0);
            Assert.True(a.Equals(a) == true);

            a.RotateTowards(b, Mathf.PI * 2); // test overshoot
            Assert.True(a.Equals(b) == true);

            a.Set(0, 0, 0, 1);
            a.RotateTowards(b, halfPI);
            Assert.True(a.AngleTo(c) - halfPI <= MathUtils.EPS);

        }

        [Fact()]
        public void SetTest()
        {
            var a = new Quaternion();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);
            Assert.True(a.Z == 0);
            Assert.True(a.W == 1);

            a.Set(x, y, z, w);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);
            Assert.True(a.Z == z);
            Assert.True(a.W == w);

        }

        [Fact()]
        public void SetFromEulerTest()
        {
            var angles = new[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) };

            // ensure euler conversion to/from Quaternion matches.
            for (var i = 0; i < orders.Length; i++)
            {

                for (var j = 0; j < angles.Length; j++)
                {

                    var eulers2 = new Euler().SetFromQuaternion(new Quaternion().SetFromEuler(new Euler(angles[j].X, angles[j].Y, angles[j].Z, orders[i])), orders[i]);
                    var newAngle = new Vector3(eulers2.X, eulers2.Y, eulers2.Z);
                    Assert.True(newAngle.DistanceTo(angles[j]) < 0.001);

                }

            }
        }

        [Fact()]
        public void SetFromEulerTest2()
        {
            // ensure euler conversion for Quaternion matches that of Matrix4
            for (var i = 0; i < orders.Length; i++)
            {

                var q = new Quaternion().SetFromEuler(ChangeEulerOrder(eulerAngles, orders[i]));
                var m = new Matrix4().MakeRotationFromEuler(ChangeEulerOrder(eulerAngles, orders[i]));
                var q2 = new Quaternion().SetFromRotationMatrix(m);

                Assert.True(qSub(q, q2).Length() < 0.001);

            }
        }


        [Fact()]
        public void SetFromUnitVectorsTest()
        {
            var a = new Quaternion();
            var b = new Vector3(1, 0, 0);
            var c = new Vector3(0, 1, 0);
            var expected = new Quaternion(0, 0, Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2);

            a.SetFromUnitVectors(b, c);
            Assert.True(Mathf.Abs(a.X - expected.X) <= MathUtils.EPS, "Check x");
            Assert.True(Mathf.Abs(a.Y - expected.Y) <= MathUtils.EPS, "Check y");
            Assert.True(Mathf.Abs(a.Z - expected.Z) <= MathUtils.EPS, "Check z");
            Assert.True(Mathf.Abs(a.W - expected.W) <= MathUtils.EPS, "Check w");
        }

        [Fact()]
        public void SetFromRotationMatrixTest()
        {
            // contrived examples purely to please the god of code coverage...
            // match conditions in various 'else [if]' blocks

            var a = new Quaternion();
            var q = new Quaternion(-9, -2, 3, -4).Normalize();
            var m = new Matrix4().MakeRotationFromQuaternion(q);
            var expected = new Vector4((float)0.8581163303210332, (float)0.19069251784911848, (float)-0.2860387767736777, (float)0.38138503569823695);

            a.SetFromRotationMatrix(m);
            Assert.True(Mathf.Abs(a.X - expected.X) <= MathUtils.EPS, "m11 > m22 && m11 > m33: check x");
            Assert.True(Mathf.Abs(a.Y - expected.Y) <= MathUtils.EPS, "m11 > m22 && m11 > m33: check y");
            Assert.True(Mathf.Abs(a.Z - expected.Z) <= MathUtils.EPS, "m11 > m22 && m11 > m33: check z");
            Assert.True(Mathf.Abs(a.W - expected.W) <= MathUtils.EPS, "m11 > m22 && m11 > m33: check w");

            q = new Quaternion(-1, -2, 1, -1).Normalize();
            m.MakeRotationFromQuaternion(q);
            expected = new Vector4((float)0.37796447300922714, (float)0.7559289460184544, (float)-0.37796447300922714, (float)0.37796447300922714);

            a.SetFromRotationMatrix(m);
            Assert.True(Mathf.Abs(a.X - expected.X) <= MathUtils.EPS, "m22 > m33: check x");
            Assert.True(Mathf.Abs(a.Y - expected.Y) <= MathUtils.EPS, "m22 > m33: check y");
            Assert.True(Mathf.Abs(a.Z - expected.Z) <= MathUtils.EPS, "m22 > m33: check z");
            Assert.True(Mathf.Abs(a.W - expected.W) <= MathUtils.EPS, "m22 > m33: check w");

        }

        [Fact()]
        public void SetFromAxisAngleTest()
        {
            // TODO: find cases to validate.
            // Assert.True( true );

            var zero = new Quaternion();

            var a = new Quaternion().SetFromAxisAngle(new Vector3(1, 0, 0), 0);
            Assert.True(a.Equals(zero));
            a = new Quaternion().SetFromAxisAngle(new Vector3(0, 1, 0), 0);
            Assert.True(a.Equals(zero));
            a = new Quaternion().SetFromAxisAngle(new Vector3(0, 0, 1), 0);
            Assert.True(a.Equals(zero));

            var b1 = new Quaternion().SetFromAxisAngle(new Vector3(1, 0, 0), Mathf.PI);
            Assert.True(!a.Equals(b1));
            var b2 = new Quaternion().SetFromAxisAngle(new Vector3(1, 0, 0), -Mathf.PI);
            Assert.True(!a.Equals(b2));

            b1.Multiply(b2);
            Assert.True(a.Equals(b1));

        }

        [Fact()]
        public void SlerpTest()
        {
            var a = new Quaternion(x, y, z, w);
            var b = new Quaternion(-x, -y, -z, -w);

            var c = a.Clone().Slerp(b, 0);
            var d = a.Clone().Slerp(b, 1);

            Assert.True(a.Equals(c), "Passed");
            Assert.True(b.Equals(d), "Passed");


            var D = Mathf.Sqrt((float)1.0 / (float)2.0);

            var e = new Quaternion(1, 0, 0, 0);
            var f = new Quaternion(0, 0, 1, 0);
            var expected = new Quaternion(D, 0, D, 0);
            var result = e.Clone().Slerp(f, (float)0.5);
            Assert.True(Mathf.Abs(result.X - expected.X) <= MathUtils.EPS, "Check x");
            Assert.True(Mathf.Abs(result.Y - expected.Y) <= MathUtils.EPS, "Check y");
            Assert.True(Mathf.Abs(result.Z - expected.Z) <= MathUtils.EPS, "Check z");
            Assert.True(Mathf.Abs(result.W - expected.W) <= MathUtils.EPS, "Check w");


            var g = new Quaternion(0, D, 0, D);
            var h = new Quaternion(0, -D, 0, D);
            expected = new Quaternion(0, 0, 0, 1);
            result = g.Clone().Slerp(h, (float)0.5);

            Assert.True(Mathf.Abs(result.X - expected.X) <= MathUtils.EPS, "Check x");
            Assert.True(Mathf.Abs(result.Y - expected.Y) <= MathUtils.EPS, "Check y");
            Assert.True(Mathf.Abs(result.Z - expected.Z) <= MathUtils.EPS, "Check z");
            Assert.True(Mathf.Abs(result.W - expected.W) <= MathUtils.EPS, "Check w");
        }

        [Fact()]
        public void ToArrayTest()
        {
            var a = new Quaternion(x, y, z, w);

            var array = a.ToArray();
            Assert.Equal(array[0], x);
            Assert.Equal(array[1], y);
            Assert.Equal(array[2], z);
            Assert.Equal(array[3], w);

            array = new float[] { };
            a.ToArray(ref array);
            Assert.Equal(array[0], x);
            Assert.Equal(array[1], y);
            Assert.Equal(array[2], z);
            Assert.Equal(array[3], w);

            array = new float[] { };
            a.ToArray(ref array, 1);
            //Assert.Equal(array[0], null);
            Assert.Equal(array[1], x);
            Assert.Equal(array[2], y);
            Assert.Equal(array[3], z);
            Assert.Equal(array[4], w);

        }

        [Fact()]
        public void XTest()
        {
            var a = new Quaternion();
            Assert.Equal(0, a.X);

            a = new Quaternion(1, 2, 3);
            Assert.Equal(1, a.X);

            a = new Quaternion(4, 5, 6, 1);
            Assert.Equal(4, a.X);

            a = new Quaternion(7, 8, 9);
            a.X = 10;
            Assert.Equal(10, a.X);

            a = new Quaternion(11, 12, 13);
            var b = false;
            a.PropertyChanged += (sender, args) => b = true;
            Assert.True(!b);
            a.X = 14;
            Assert.True(b);
            Assert.Equal(14, a.X);
        }

        [Fact()]
        public void YTest()
        {
            var a = new Quaternion();
            Assert.Equal(0, a.Y);

            a = new Quaternion(1, 2, 3);
            Assert.Equal(2, a.Y);

            a = new Quaternion(4, 5, 6, 1);
            Assert.Equal(5, a.Y);

            a = new Quaternion(7, 8, 9);
            a.Y = 10;
            Assert.Equal(10, a.Y);

            a = new Quaternion(11, 12, 13);
            var b = false;
            a.PropertyChanged += (sender, args) => b = true;
            Assert.True(!b);
            a.Y = 14;
            Assert.True(b);
            Assert.Equal(14, a.Y);

        }

        [Fact()]
        public void ZTest()
        {
            var a = new Quaternion();
            Assert.Equal(0, a.Z);

            a = new Quaternion(1, 2, 3);
            Assert.Equal(3, a.Z);

            a = new Quaternion(4, 5, 6, 1);
            Assert.Equal(6, a.Z);

            a = new Quaternion(7, 8, 9);
            a.Z = 10;
            Assert.Equal(10, a.Z);

            a = new Quaternion(11, 12, 13);
            var b = false;
            a.PropertyChanged += (sender, args) => b = true;
            Assert.True(!b);
            a.Z = 14;
            Assert.True(b);
            Assert.Equal(14, a.Z);

        }

        [Fact()]
        public void WTest()
        {
            var a = new Quaternion();
            Assert.Equal(1, a.W);

            a = new Quaternion(1, 2, 3);
            Assert.Equal(1, a.W);

            a = new Quaternion(4, 5, 6, 1);
            Assert.Equal(1, a.W);

            a = new Quaternion(7, 8, 9);
            a.W = 10;
            Assert.Equal(10, a.W);

            a = new Quaternion(11, 12, 13);
            var b = false;
            a.PropertyChanged += (sender, args) => b = true;
            Assert.True(!b);
            a.W = 14;
            Assert.True(b);
            Assert.Equal(14, a.W);


        }

    }
}