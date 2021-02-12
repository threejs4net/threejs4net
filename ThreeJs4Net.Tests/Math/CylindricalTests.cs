using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class CylindricalTests
    {
        [Fact()]
        public void InstancingTest()
        {
            var a = new Cylindrical();
            float radius = (float)10.0;
            var theta = Mathf.PI;
            float y = 5;

            Assert.Equal((float)1.0, a.Radius);
            Assert.Equal(0, a.Theta);
            Assert.Equal(0, a.Y);

            a = new Cylindrical(radius, theta, y);
            Assert.Equal(a.Radius, radius);
            Assert.Equal(a.Theta, theta);
            Assert.Equal(a.Y, y);

        }

        [Fact()]
        public void SetTest()
        {
            var a = new Cylindrical();
            float radius = (float )10.0;
            float theta = Mathf.PI;
            float y = 5;

            a.Set(radius, theta, y);
            Assert.Equal(radius, a.Radius);
            Assert.Equal(theta, a.Theta);
            Assert.Equal(y, a.Y);
        }

        [Fact()]
        public void CloneTest()
        {
            float radius = (float )10.0;
            float theta = Mathf.PI;
            float y = 5;
            var a = new Cylindrical(radius, theta, y);
            var b = a.Clone();

            Assert.True(a.Equals(b));

            a.Radius = 1;
            Assert.False(a.Equals(b));

        }

        [Fact()]
        public void CopyTest()
        {
            float radius = (float)10.0;
            float theta = Mathf.PI;
            var y = 5;
            var a = new Cylindrical(radius, theta, y);
            var b = new Cylindrical().Copy(a);

            Assert.True(a.Equals(b));

            a.Radius = 1;
            Assert.False(a.Equals(b));

        }

        [Fact()]
        public void SetFromVector3Test()
        {
            var a = new Cylindrical(1, 1, 1);
            var b = new Vector3(0, 0, 0);
            var c = new Vector3(3, -1, -3);
            var expected = new Cylindrical(Mathf.Sqrt(9 + 9), Mathf.Atan2(3, -3), -1);

            a.SetFromVector3(b);
            Assert.Equal(0, a.Radius);
            Assert.Equal(0, a.Theta);
            Assert.Equal(0, a.Y);

            a.SetFromVector3(c);
            Assert.True(Mathf.Abs(a.Radius - expected.Radius) <= MathUtils.EPS);
            Assert.True(Mathf.Abs(a.Theta - expected.Theta) <= MathUtils.EPS);
            Assert.True(Mathf.Abs(a.Y - expected.Y) <= MathUtils.EPS);
        }
    }
}