using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class EulerTests : BaseTests
    {
        [Fact()]
        public void InstancingTest()
        {
            var a = new Euler();
            Assert.Equal(eulerZero, a);
            Assert.NotEqual(eulerAxyz, a);
            Assert.NotEqual(eulerAzyx, a);
        }

        [Fact()]
        public void DefaultOrderTest()
        {
            Assert.Equal(Euler.RotationOrder.XYZ, Euler.DefaultOrder);
        }

        [Fact()]
        public void SetTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void XTest()
        {
            var a = new Euler();
            Assert.Equal(0, a.X);

            a = new Euler(1, 2, 3);
            Assert.Equal(1, a.X);

            a = new Euler(4, 5, 6, Euler.RotationOrder.XYZ);
            Assert.Equal(4, a.X);

            a = new Euler(7, 8, 9, Euler.RotationOrder.XYZ);
            a.X = 10;
            Assert.Equal(10, a.X);

            a = new Euler(11, 12, 13, Euler.RotationOrder.XYZ);
            var b = false;
            a.PropertyChanged += (sender, args) => b = true;

            a.X = 14;
            Assert.True(b);
            Assert.Equal(14, a.X);
        }

        [Fact()]
        public void CloneTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void CopyTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SetFromRotationMatrixTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SetFromQuaternionTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SetFromVector3Test()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void FromArrayTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ToArrayTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ToVector3Test()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ReorderTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void EqualsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void EqualsTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void GetHashCodeTest()
        {
            Assert.True(false, "This test needs an implementation");
        }
    }
}