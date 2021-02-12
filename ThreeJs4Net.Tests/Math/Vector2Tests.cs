using ThreeJs4Net.Core;
using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class Vector2Tests : BaseTests
    {
        [Fact()]
        public void InstancingTest()
        {
            var a = new Vector2();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);

            a = new Vector2(x, y);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);
        }

        [Fact()]
        public void PropertiesTest()
        {
            var a = new Vector2(0, 0);
            var width = 100;
            var height = 200;

            a.Width = width;
            a.Height= height;

            Assert.Equal(width, a.Width);
            Assert.Equal(height, a.Height);

            a.Set(width+1, height+1);
            Assert.Equal(width+1, a.Width);
            Assert.Equal(height+1, a.Height);
        }

        [Fact()]
        public void AddTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2(-x, -y);

            a.Add(b);
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);

            var c = new Vector2().AddVectors(b, b);
            Assert.Equal(-2 * x, c.X);
            Assert.Equal(-2 * y, c.Y);
        }

        [Fact()]
        public void AddScaledVectorTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2(2, 3);
            var s = 3;

            a.AddScaledVector(b, s);
            Assert.Equal(x + b.X * s, a.X);
            Assert.Equal(y + b.Y * s, a.Y);
        }

        [Fact()]
        public void ApplyMatrix3Test()
        {
            var a = new Vector2(x, y);
            var m = new Matrix3().Set(2, 3, 5, 7, 11, 13, 17, 19, 23);

            a.ApplyMatrix3(m);
            Assert.Equal(18, a.X);
            Assert.Equal(60, a.Y);

        }

        [Fact()]
        public void CrossTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2(2 * x, -y);
            var answer = -18;
            var crossed = a.Cross(b);

            Assert.True(Mathf.Abs(answer - crossed) <= MathUtils.EPS);
        }

        [Fact()]
        public void CopyTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2().Copy(a);
            Assert.Equal(x, b.X);
            Assert.Equal(y, b.Y);

            // ensure that it is a true copy
            a.X = 0;
            a.Y = -1;
            Assert.Equal(x, b.X);
            Assert.Equal(y, b.Y);
        }

        [Fact()]
        public void DistanceToTest()
        {

            var a = new Vector2(x, 0);
            var b = new Vector2(0, -y);
            var c = new Vector2();

            Assert.Equal(x, a.DistanceTo(c));
            Assert.Equal(x * x, a.DistanceToSquared(c));

            Assert.Equal(y, b.DistanceTo(c));
            Assert.Equal(y * y, b.DistanceToSquared(c));
        }

        [Fact()]
        public void DivideTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2(2 * x, 2 * y);
            var c = new Vector2(4 * x, 4 * y);

            a.Multiply(b);
            Assert.Equal(x * b.X, a.X);
            Assert.Equal(y * b.Y, a.Y);

            b.Divide(c);
            Assert.Equal(0.5, b.X);
            Assert.Equal(0.5, b.Y);
        }

        [Fact()]
        public void DotTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2(-x, -y);
            var c = new Vector2();

            var result = a.Dot(b);
            Assert.Equal((-x * x - y * y), result);

            result = a.Dot(c);
            Assert.Equal(0, result);
        }

        [Fact()]
        public void EqualsTest()
        {
            var a = new Vector2(x, 0);
            var b = new Vector2(0, -y);

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
            var a = new Vector2();
            var array = new float[] { 1, 2, 3, 4 };

            a.FromArray(array);
            Assert.Equal(1, a.X);
            Assert.Equal(2, a.Y);

            a.FromArray(array, 2);
            Assert.Equal(3, a.X);
            Assert.Equal(4, a.Y);
        }

        [Fact()]
        public void FromBufferAttributeTest()
        {
            var a = new Vector2();
            var attr = new BufferAttribute<float>(new float[] { 1, 2, 3, 4 }, 2);

            a.FromBufferAttribute(attr, 0);
            Assert.Equal(1, a.X);
            Assert.Equal(2, a.Y);

            a.FromBufferAttribute(attr, 1);
            Assert.Equal(3, a.X);
            Assert.Equal(4, a.Y);

        }

        [Fact()]
        public void LengthTest()
        {
            var a = new Vector2(x, 0);
            var b = new Vector2(0, -y);
            var c = new Vector2();

            Assert.Equal(x, a.Length());
            Assert.Equal(x * x, a.LengthSq());
            Assert.Equal(y, b.Length());
            Assert.Equal(y * y, b.LengthSq());
            Assert.Equal(0, c.Length());
            Assert.Equal(0, c.LengthSq());

            a.Set(x, y);
            Assert.Equal(Mathf.Sqrt(x * x + y * y), a.Length());
            Assert.Equal((x * x + y * y), a.LengthSq());
        }

        [Fact()]
        public void LerpTest()
        {
            var a = new Vector2(x, 0);
            var b = new Vector2(0, -y);

            Assert.True(a.Lerp(a, 0).Equals(a.Lerp(a, (float)0.5)));
            Assert.True(a.Lerp(a, 0).Equals(a.Lerp(a, 1)));

            Assert.True(a.Clone().Lerp(b, 0).Equals(a));

            Assert.True(a.Clone().Lerp(b, (float)0.5).X == x * 0.5);
            Assert.True(a.Clone().Lerp(b, (float)0.5).Y == -y * 0.5);

            Assert.True(a.Clone().Lerp(b, 1).Equals(b));
        }

        [Fact()]
        public void ManhattanLengthTest()
        {
            var a = new Vector2(x, 0);
            var b = new Vector2(0, -y);
            var c = new Vector2();

            Assert.Equal(a.ManhattanLength(), x);
            Assert.Equal(b.ManhattanLength(), y);
            Assert.Equal(c.ManhattanLength(), 0);

            a.Set(x, y);
            Assert.Equal(a.ManhattanLength(), Mathf.Abs(x) + Mathf.Abs(y));

        }

        [Fact()]
        public void NormalizeTest()
        {
            var a = new Vector2(x, 0);
            var b = new Vector2(0, -y);

            a.Normalize();
            Assert.True(a.Length() == 1);
            Assert.True(a.X == 1);

            b.Normalize();
            Assert.True(b.Length() == 1);
            Assert.True(b.Y == -1);
        }

        [Fact()]
        public void MinTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2(-x, -y);
            var c = new Vector2();

            c.Copy(a).Min(b);
            Assert.True(c.X == -x);
            Assert.True(c.Y == -y);

            c.Copy(a).Max(b);
            Assert.True(c.X == x);
            Assert.True(c.Y == y);

            c.Set(-2 * x, 2 * y);
            c.Clamp(b, a);
            Assert.True(c.X == -x);
            Assert.True(c.Y == y);

            c.Set(-2 * x, 2 * x);
            c.ClampScalar(-x, x);
            Assert.Equal(c.X, -x);
            Assert.Equal(c.Y, x);
        }

        [Fact()]
        public void MultiplyTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2(-x, -y);

            a.MultiplyScalar(-2);
            Assert.True(a.X == x * -2);
            Assert.True(a.Y == y * -2);

            b.MultiplyScalar(-2);
            Assert.True(b.X == 2 * x);
            Assert.True(b.Y == 2 * y);

            a.DivideScalar(-2);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);

            b.DivideScalar(-2);
            Assert.True(b.X == -x);
            Assert.True(b.Y == -y);
        }

        [Fact()]
        public void NegateTest()
        {
            var a = new Vector2(x, y);

            a.Negate();
            Assert.True(a.X == -x);
            Assert.True(a.Y == -y);
        }

        [Fact()]
        public void RoundTest()
        {
            Assert.Equal(new Vector2((float)-0.1, (float)0.1).Floor(), new Vector2(-1, 0));
            Assert.Equal(new Vector2((float)-0.5, (float)0.5).Floor(), new Vector2(-1, 0));
            Assert.Equal(new Vector2((float)-0.9, (float)0.9).Floor(), new Vector2(-1, 0));

            Assert.Equal(new Vector2((float)-0.1, (float)0.1).Ceil(), new Vector2(0, 1));
            Assert.Equal(new Vector2((float)-0.5, (float)0.5).Ceil(), new Vector2(0, 1));
            Assert.Equal(new Vector2((float)-0.9, (float)0.9).Ceil(), new Vector2(0, 1));

            Assert.Equal(new Vector2((float)-0.1, (float)0.1).Round(), new Vector2(0, 0));
            Assert.Equal(new Vector2((float)-0.5, (float)0.5).Round(), new Vector2(0, 1));
            Assert.Equal(new Vector2((float)-0.9, (float)0.9).Round(), new Vector2(-1, 1));

            Assert.Equal(new Vector2((float)-0.1, (float)0.1).RoundToZero(), new Vector2(0, 0));
            Assert.Equal(new Vector2((float)-0.5, (float)0.5).RoundToZero(), new Vector2(0, 0));
            Assert.Equal(new Vector2((float)-0.9, (float)0.9).RoundToZero(), new Vector2(0, 0));
            Assert.Equal(new Vector2((float)-1.1, (float)1.1).RoundToZero(), new Vector2(-1, 1));
            Assert.Equal(new Vector2((float)-1.5, (float)1.5).RoundToZero(), new Vector2(-1, 1));
            Assert.Equal(new Vector2((float)-1.9, (float)1.9).RoundToZero(), new Vector2(-1, 1));
        }


        [Fact()]
        public void SetTest()
        {
            var a = new Vector2();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);

            a.Set(x, y);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);

        }

        [Fact()]
        public void SetComponentTest()
        {
            var a = new Vector2();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);

            a.SetComponent(0, 1);
            a.SetComponent(1, 2);
            Assert.True(a.GetComponent(0) == 1);
            Assert.True(a.GetComponent(1) == 2);

        }

        //[Fact()]
        //public void SetComponentTest2()
        //{
        //    var a = new Vector2(0, 0);

        //    Assert.throws(
        //        function() {

        //        a.SetComponent(2, 0);

        //    },
        /// index is out of range/,
        //"setComponent with an out of range index throws Error"
        //    );
        //    Assert.throws(
        //        function() {

        //        a.getComponent(2);

        //    },
        /// index is out of range/,
        //"getComponent with an out of range index throws Error"
        //    );
        //}

        [Fact()]
        public void SetScalarTest()
        {
            var a = new Vector2(1, 1);
            var s = 3;

            a.SetScalar(s);
            Assert.Equal(a.X, s);
            Assert.Equal(a.Y, s);

            a.AddScalar(s);
            Assert.Equal(a.X, 2 * s);
            Assert.Equal(a.Y, 2 * s);

            a.SubScalar(2 * s);
            Assert.Equal(a.X, 0);
            Assert.Equal(a.Y, 0);
        }

        [Fact()]
        public void SetLengthTest()
        {

            var a = new Vector2(x, 0);

            Assert.True(a.Length() == x);
            a.SetLength(y);
            Assert.True(a.Length() == y);

            a = new Vector2(0, 0);
            Assert.True(a.Length() == 0);
            a.SetLength(y);
            Assert.True(a.Length() == 0);
            //a.SetLength();
            //Assert.True(isNaN(a.Length()));
        }

        [Fact()]
        public void SetXTest()
        {
            var a = new Vector2();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);

            a.SetX(x);
            a.SetY(y);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);
        }

        [Fact()]
        public void SubTest()
        {
            var a = new Vector2(x, y);
            var b = new Vector2(-x, -y);

            a.Sub(b);
            Assert.True(a.X == 2 * x);
            Assert.True(a.Y == 2 * y);

            var c = new Vector2().SubVectors(a, a);
            Assert.True(c.X == 0);
            Assert.True(c.Y == 0);
        }

        [Fact()]
        public void ToArrayTest()
        {
            var a = new Vector2(x, y);

            var array = new float[] { };

            array = a.ToArray(ref array);
            Assert.Equal(array[0], x);
            Assert.Equal(array[1], y);

            array = new float[] { };
            a.ToArray(ref array);
            Assert.Equal(array[0], x);
            Assert.Equal(array[1], y);

            array = new float[] { };
            a.ToArray(ref array, 1);
            //Assert.Equal(array[0], null);
            Assert.Equal(array[1], x);
            Assert.Equal(array[2], y);
        }
    }
}