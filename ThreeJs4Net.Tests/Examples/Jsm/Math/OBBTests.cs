using System.Runtime.Intrinsics.X86;
using ThreeJs4Net.Examples.Jsm.Math;
using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Examples.Jsm.Math
{
    public class OBBTests : BaseTests
    {
        [Fact()]
        public void OBBTest()
        {
            var box = new Box3(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
            var obb = new OBB();
            var matrix4 = new Matrix4(new float[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 });

            Assert.Equal(obb.Center.ToArray(), new float[] { 0, 0, 0 });
            obb.FromBox3(box);
            Assert.Equal(obb.Center.ToArray(), new float[] { 5, 5, 5 });
            Assert.Equal(obb.Rotation.ToArray(), new float[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 });

            matrix4 = matrix4.MakeRotationX((float)0.5);
            obb.ApplyMatrix4(matrix4);

            Assert.Equal(obb.Rotation.ToArray(), new float[]
            {
                1, 0, 0,
                0, (float)0.8775825618903728, (float)0.479425538604203,
                0, (float)-0.479425538604203, (float)0.8775825618903728
            });


            matrix4 = matrix4.MakeRotationY((float)0.3);
            obb.ApplyMatrix4(matrix4);

            Assert.Equal(obb.Rotation.ToArray(), new float[]
            {
                (float) 0.9553365, 
                (float) 0.141679943, 
                (float) -0.2593434,
                (float) 0, 
                (float) 0.87758255, 
                (float) 0.47942555,
                (float) 0.295520216, 
                (float) -0.45801273, 
                (float) 0.838386655
            });
        }

        [Fact()]
        public void OBBTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SetTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void CopyTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void CloneTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void GetSizeTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ContainsPointTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectsBox3Test()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ApplyMatrix4Test()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ClampPointTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectsRayTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void FromBox3Test()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectsSphereTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void EqualsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectRayTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectsPlaneTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectsOBBTest()
        {
            Assert.True(false, "This test needs an implementation");
        }
    }
}