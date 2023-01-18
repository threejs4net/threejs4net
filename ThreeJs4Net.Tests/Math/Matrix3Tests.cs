using System;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class Matrix3Tests : BaseTests
    {
        private readonly float _epsilon = 0.0001f;

        [Fact()]
        public void Instancing()
        {
            var a = new Matrix3();
            Assert.True(a.Elements[0] == 1);
            Assert.True(a.Elements[1] == 0);
            Assert.True(a.Elements[2] == 0);
            Assert.True(a.Elements[3] == 0);
            Assert.True(a.Elements[4] == 1);
            Assert.True(a.Elements[5] == 0);
            Assert.True(a.Elements[6] == 0);
            Assert.True(a.Elements[7] == 0);
            Assert.True(a.Elements[8] == 1);
        }

        [Fact()]
        public void Identity()
        {
            var a = new Matrix3();
            a.Identity();
            Assert.True(a.Elements[0] == 1);
            Assert.True(a.Elements[1] == 0);
            Assert.True(a.Elements[2] == 0);
            Assert.True(a.Elements[3] == 0);
            Assert.True(a.Elements[4] == 1);
            Assert.True(a.Elements[5] == 0);
            Assert.True(a.Elements[6] == 0);
            Assert.True(a.Elements[7] == 0);
            Assert.True(a.Elements[8] == 1);
        }

        [Fact()]
        public void Copy()
        {
            var a = new Matrix3().Set(0, 1, 2, 3, 4, 5, 6, 7, 8);
            var b = new Matrix3().Copy(a);

            Assert.True(a.Elements[0] == b.Elements[0]);
            Assert.True(a.Elements[1] == b.Elements[1]);
            Assert.True(a.Elements[2] == b.Elements[2]);
            Assert.True(a.Elements[3] == b.Elements[3]);
            Assert.True(a.Elements[4] == b.Elements[4]);
            Assert.True(a.Elements[5] == b.Elements[5]);
            Assert.True(a.Elements[6] == b.Elements[6]);
            Assert.True(a.Elements[7] == b.Elements[7]);
            Assert.True(a.Elements[8] == b.Elements[8]);
        }

        [Fact()]
        public void Clone()
        {
            var a = new Matrix3().Set(0, 1, 2, 3, 4, 5, 6, 7, 8);
            var b = a.Clone();

            Assert.True(a.Elements[0] == b.Elements[0]);
            Assert.True(a.Elements[1] == b.Elements[1]);
            Assert.True(a.Elements[2] == b.Elements[2]);
            Assert.True(a.Elements[3] == b.Elements[3]);
            Assert.True(a.Elements[4] == b.Elements[4]);
            Assert.True(a.Elements[5] == b.Elements[5]);
            Assert.True(a.Elements[6] == b.Elements[6]);
            Assert.True(a.Elements[7] == b.Elements[7]);
            Assert.True(a.Elements[8] == b.Elements[8]);
        }

        [Fact()]
        public void Determinant()
        {
            var a = new Matrix3();
            Assert.True(a.Determinant == 1);

            a.Elements[0] = 2;
            Assert.True(a.Determinant == 2);

            a.Elements[0] = 0;
            Assert.True(a.Determinant == 0);

            a.Set(2, 3, 4, 5, 13, 7, 8, 9, 11);
            Assert.True(a.Determinant == -73);
        }

        [Fact()]
        public void Invert()
        {
            var m = new Matrix4().MakeRotationX(0.3f);
            
            var a = new Matrix3().SetFromMatrix4(m);
            var b = a.Clone();

            a.Invert();
            a.Invert();

            Assert.True(Mathf.Abs(a.Elements[0] - b.Elements[0]) <= _epsilon);
            Assert.True(Mathf.Abs(a.Elements[1] - b.Elements[1]) <= _epsilon);
            Assert.True(Mathf.Abs(a.Elements[2] - b.Elements[2]) <= _epsilon);
            Assert.True(Mathf.Abs(a.Elements[3] - b.Elements[3]) <= _epsilon);
            Assert.True(Mathf.Abs(a.Elements[4] - b.Elements[4]) <= _epsilon);
            Assert.True(Mathf.Abs(a.Elements[5] - b.Elements[5]) <= _epsilon);
            Assert.True(Mathf.Abs(a.Elements[6] - b.Elements[6]) <= _epsilon);
            Assert.True(Mathf.Abs(a.Elements[7] - b.Elements[7]) <= _epsilon);
            Assert.True(Mathf.Abs(a.Elements[8] - b.Elements[8]) <= _epsilon);
        }
    }
}