using System;
using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Math
{
    public class Vector3Tests : BaseTests
    {
        [Fact()]
        public void Instancing()
        {
            var a = new Vector3();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);
            Assert.True(a.Z == 0);

            a = new Vector3(x, y, z);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);
            Assert.True(a.Z == z);

        }

        [Fact()]
        public void UnitYTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void UnitZTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ZeroTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void OneTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void Vector3Test()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void Vector3Test1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void Vector3Test2()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void Vector3Test3()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void Vector3Test4()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SetLengthTest()
        {
            var a = new Vector3(x, 0, 0);

            Assert.True(a.Length() == x);
            a.SetLength(y);
            Assert.True(a.Length() == y);

            a = new Vector3(0, 0, 0);
            Assert.True(a.Length() == 0);
            a.SetLength(y);
            Assert.True(a.Length() == 0);
            //A.SetLength();
            //Assert.True(isNaN(A.Length()));
        }

        [Fact()]
        public void SetScalarAddScalarSubScalarTest()
        {
            var a = new Vector3();
            var s = 3;

            a.SetScalar(s);
            Assert.StrictEqual(a.X, s);
            Assert.StrictEqual(a.Y, s);
            Assert.StrictEqual(a.Z, s);

            a.AddScalar(s);
            Assert.StrictEqual(a.X, 2 * s);
            Assert.StrictEqual(a.Y, 2 * s);
            Assert.StrictEqual(a.Z, 2 * s);

            a.SubScalar(2 * s);
            Assert.StrictEqual(a.X, 0);
            Assert.StrictEqual(a.Y, 0);
            Assert.StrictEqual(a.Z, 0);
        }

        [Fact()]
        public void SetXSetYSetZTest()
        {
            var a = new Vector3();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);
            Assert.True(a.Z == 0);

            a.SetX(x);
            a.SetY(y);
            a.SetZ(z);

            Assert.True(a.X == x);
            Assert.True(a.Y == y);
            Assert.True(a.Z == z);
        }


        [Fact()]
        public void LengthLengthSqTest()
        {
            var a = new Vector3(x, 0, 0);
            var b = new Vector3(0, -y, 0);
            var c = new Vector3(0, 0, z);
            var d = new Vector3();

            Assert.True(a.Length() == x);
            Assert.True(a.LengthSq() == x * x);
            Assert.True(b.Length() == y);
            Assert.True(b.LengthSq() == y * y);
            Assert.True(c.Length() == z);
            Assert.True(c.LengthSq() == z * z);
            Assert.True(d.Length() == 0);
            Assert.True(d.LengthSq() == 0);

            a.Set(x, y, z);
            Assert.True(a.Length() == Mathf.Sqrt(x * x + y * y + z * z));
            Assert.True(a.LengthSq() == (x * x + y * y + z * z));
        }


        [Fact()]
        public void SetTest()
        {
            var a = new Vector3();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);
            Assert.True(a.Z == 0);

            a.Set(x, y, z);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);
            Assert.True(a.Z == z);
        }

        [Fact()]
        public void AddTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(-x, -y, -z);

            a.Add(b);
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);
            Assert.True(a.Z == 0);

            var c = new Vector3().AddVectors(b, b);
            Assert.True(c.X == -2 * x);
            Assert.True(c.Y == -2 * y);
            Assert.True(c.Z == -2 * z);
        }

        [Fact()]
        public void SubTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(-x, -y, -z);

            a.Sub(b);
            Assert.True(a.X == 2 * x);
            Assert.True(a.Y == 2 * y);
            Assert.True(a.Z == 2 * z);

            var c = new Vector3().SubVectors(a, a);
            Assert.True(c.X == 0);
            Assert.True(c.Y == 0);
            Assert.True(c.Z == 0);
        }

        [Fact()]
        public void SetComponentGetComponentTest()
        {
            var a = new Vector3();
            Assert.True(a.X == 0);
            Assert.True(a.Y == 0);
            Assert.True(a.Z == 0);

            a.SetComponent(0, 1);
            a.SetComponent(1, 2);
            a.SetComponent(2, 3);
            Assert.True(a.GetComponent(0) == 1);
            Assert.True(a.GetComponent(1) == 2);
            Assert.True(a.GetComponent(2) == 3);
        }

        [Fact()]
        public void GetComponentTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void CopyTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3().Copy(a);
            Assert.True(b.X == x);
            Assert.True(b.Y == y);
            Assert.True(b.Z == z);

            // ensure that it is A true copy
            a.X = 0;
            a.Y = -1;
            a.Z = -2;
            Assert.True(b.X == x);
            Assert.True(b.Y == y);
            Assert.True(b.Z == z);
        }

        [Fact()]
        public void AddScaledVectorTest()
        {

            var a = new Vector3(x, y, z);
            var b = new Vector3(2, 3, 4);
            var s = 3;

            a.AddScaledVector(b, s);
            Assert.StrictEqual(a.X, x + b.X * s);
            Assert.StrictEqual(a.Y, y + b.Y * s);
            Assert.StrictEqual(a.Z, z + b.Z * s);

        }

        [Fact()]
        public void AddVectorsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SubVectorsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SubtractVectorsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void MultiplyDivideTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(2 * x, 2 * y, 2 * z);
            var c = new Vector3(4 * x, 4 * y, 4 * z);

            a.Multiply(b);
            Assert.StrictEqual(a.X, x * b.X);
            Assert.StrictEqual(a.Y, y * b.Y);
            Assert.StrictEqual(a.Z, z * b.Z);

            b.Divide(c);
            Assert.True(MathF.Abs(b.X - (float)0.5) <= MathUtils.EPS);
            Assert.True(MathF.Abs(b.Y - (float)0.5) <= MathUtils.EPS);
            Assert.True(MathF.Abs(b.Z - (float)0.5) <= MathUtils.EPS);
        }

        [Fact()]
        public void MultiplyDivideTest2()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(-x, -y, -z);

            a.MultiplyScalar(-2);
            Assert.True(a.X == x * -2);
            Assert.True(a.Y == y * -2);
            Assert.True(a.Z == z * -2);

            b.MultiplyScalar(-2);
            Assert.True(b.X == 2 * x);
            Assert.True(b.Y == 2 * y);
            Assert.True(b.Z == 2 * z);

            a.DivideScalar(-2);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);
            Assert.True(a.Z == z);

            b.DivideScalar(-2);
            Assert.True(b.X == -x);
            Assert.True(b.Y == -y);
            Assert.True(b.Z == -z);
        }


        [Fact()]
        public void MultiplyVectorsTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(2, 3, -5);

            var c = new Vector3().MultiplyVectors(a, b);
            Assert.StrictEqual(c.X, x * 2);
            Assert.StrictEqual(c.Y, y * 3);
            Assert.StrictEqual(c.Z, z * -5);
        }

        [Fact()]
        public void ApplyMatrix3Test()
        {
            var a = new Vector3(x, y, z);
            var m = new Matrix3().Set(2, 3, 5, 7, 11, 13, 17, 19, 23);

            a.ApplyMatrix3(m);
            Assert.StrictEqual(a.X, 33);
            Assert.StrictEqual(a.Y, 99);
            Assert.StrictEqual(a.Z, 183);
        }

        [Fact()]
        public void ApplyMatrix4Test()
        {
            //var A = new Vector3( x, y, z );
            //var B = new Vector4( x, y, z, 1 );

            //var m = new Matrix4().MakeRotationX( Math.PI );
            //A.ApplyMatrix4( m );
            //B.ApplyMatrix4( m );
            //Assert.True( A.X == B.X / B.w);
            //Assert.True( A.Y == B.Y / B.w);
            //Assert.True( A.Z == B.Z / B.w);

            //var m = new Matrix4().makeTranslation( 3, 2, 1 );
            //A.ApplyMatrix4( m );
            //B.ApplyMatrix4( m );
            //Assert.True( A.X == B.X / B.w);
            //Assert.True( A.Y == B.Y / B.w);
            //Assert.True( A.Z == B.Z / B.w);

            //var m = new Matrix4().Set(
            //    1, 0, 0, 0,
            //    0, 1, 0, 0,
            //    0, 0, 1, 0,
            //    0, 0, 1, 0
            //);
            //A.ApplyMatrix4( m );
            //B.ApplyMatrix4( m );
            //Assert.True( A.X == B.X / B.w);
            //Assert.True( A.Y == B.Y / B.w);
            //Assert.True( A.Z == B.Z / B.w);
        }

        [Fact()]
        public void ApplyNormalMatrixTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ApplyProjectionTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ApplyQuaternionTest()
        {
            var a = new Vector3(x, y, z);

            a.ApplyQuaternion(new Quaternion());
            Assert.StrictEqual(a.X, x);
            Assert.StrictEqual(a.Y, y);
            Assert.StrictEqual(a.Z, z);

            a.ApplyQuaternion(new Quaternion(x, y, z, w));
            Assert.StrictEqual(a.X, 108);
            Assert.StrictEqual(a.Y, 162);
            Assert.StrictEqual(a.Z, 216);
        }

        [Fact()]
        public void ReflectTest()
        {
            var a = new Vector3();
            var normal = new Vector3(0, 1, 0);
            var b = new Vector3();

            a.Set(0, -1, 0);
            Assert.True(b.Copy(a).Reflect(normal).Equals(new Vector3(0, 1, 0)));

            a.Set(1, -1, 0);
            Assert.True(b.Copy(a).Reflect(normal).Equals(new Vector3(1, 1, 0)));

            a.Set(1, -1, 0);
            normal.Set(0, -1, 0);
            Assert.True(b.Copy(a).Reflect(normal).Equals(new Vector3(1, 1, 0)));
        }

        [Fact()]
        public void ProjectUnprojectTest()
        {
            //var A = new Vector3( x, y, z );
            //var camera = new PerspectiveCamera( 75, 16 / 9, (float)0.1, (float)300.0 );
            //var projected = new Vector3( (float)(-0.36653213611158914), (float)(-0.9774190296309043), (float)1.0506835611870624 );

            //A.Project( camera );
            //Assert.True( MathF.Abs( A.X - projected.X ) <= eps, "project: check x" );
            //Assert.True( MathF.Abs( A.Y - projected.Y ) <= eps, "project: check y" );
            //Assert.True( MathF.Abs( A.Z - projected.Z ) <= eps, "project: check z" );

            //A.Unproject( camera );
            //Assert.True( MathF.Abs( A.X - x ) <= eps, "unproject: check x" );
            //Assert.True( MathF.Abs( A.Y - y ) <= eps, "unproject: check y" );
            //Assert.True( MathF.Abs( A.Z - z ) <= eps, "unproject: check z" );
        }

        [Fact()]
        public void TransformDirectionTest()
        {
            var a = new Vector3(x, y, z);
            var m = new Matrix4();
            var transformed = new Vector3((float)0.3713906763541037, (float)0.5570860145311556, (float)0.7427813527082074);

            a.TransformDirection(m);
            Assert.True(MathF.Abs(a.X - transformed.X) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Y - transformed.Y) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Z - transformed.Z) <= MathUtils.EPS);
        }

        [Fact()]
        public void DivideTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void DivideScalarTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void MinMaxClampTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(-x, -y, -z);
            var c = new Vector3();

            c.Copy(a).Min(b);
            Assert.True(c.X == -x);
            Assert.True(c.Y == -y);
            Assert.True(c.Z == -z);

            c.Copy(a).Max(b);
            Assert.True(c.X == x);
            Assert.True(c.Y == y);
            Assert.True(c.Z == z);

            c.Set(-2 * x, 2 * y, -2 * z);
            c.Clamp(b, a);
            Assert.True(c.X == -x);
            Assert.True(c.Y == y);
            Assert.True(c.Z == -z);
        }

        [Fact()]
        public void ClampLengthTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ClampScalarTest()
        {
            var a = new Vector3((float)-0.01, (float)0.5, (float)1.5);
            var clamped = new Vector3((float)0.1, (float)0.5, (float)1.0);

            a.ClampScalar((float)0.1, (float)1.0);
            Assert.True(MathF.Abs(a.X - clamped.X) <= 0.001);
            Assert.True(MathF.Abs(a.Y - clamped.Y) <= 0.001);
            Assert.True(MathF.Abs(a.Z - clamped.Z) <= 0.001);

        }

        [Fact()]
        public void FloorTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void CeilTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void RoundTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void RoundToZeroTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void NegateTest()
        {
            var a = new Vector3(x, y, z);

            a.Negate();
            Assert.True(a.X == -x);
            Assert.True(a.Y == -y);
            Assert.True(a.Z == -z);

        }

        [Fact()]
        public void DotTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(-x, -y, -z);
            var c = new Vector3();

            var result = a.Dot(b);
            Assert.True(result == (-x * x - y * y - z * z));

            result = a.Dot(c);
            Assert.True(result == 0);
        }


        [Fact()]
        public void ManhattanLengthTest()
        {
            var a = new Vector3(x, 0, 0);
            var b = new Vector3(0, -y, 0);
            var c = new Vector3(0, 0, z);
            var d = new Vector3();

            Assert.True(a.ManhattanLength() == x, "Positive x");
            Assert.True(b.ManhattanLength() == y, "Negative y");
            Assert.True(c.ManhattanLength() == z, "Positive z");
            Assert.True(d.ManhattanLength() == 0, "Empty initialization");

            a.Set(x, y, z);
            Assert.True(a.ManhattanLength() == MathF.Abs(x) + MathF.Abs(y) + MathF.Abs(z), "All components");

        }

        [Fact()]
        public void ManhattanDistanceToTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void NormalizeTest()
        {
            var a = new Vector3(x, 0, 0);
            var b = new Vector3(0, -y, 0);
            var c = new Vector3(0, 0, z);

            a.Normalize();
            Assert.True(a.Length() == 1);
            Assert.True(a.X == 1);

            b.Normalize();
            Assert.True(b.Length() == 1);
            Assert.True(b.Y == -1);

            c.Normalize();
            Assert.True(c.Length() == 1);
            Assert.True(c.Z == 1);

        }

        [Fact()]
        public void LerpCloneTest()
        {
            var a = new Vector3(x, 0, z);
            var b = new Vector3(0, -y, 0);

            Assert.True(a.Lerp(a, 0).Equals(a.Lerp(a, (float)0.5)));
            Assert.True(a.Lerp(a, 0).Equals(a.Lerp(a, 1)));

            Assert.True(a.Clone().Lerp(b, 0).Equals(a));

            Assert.True(a.Clone().Lerp(b, (float)0.5).X == x * 0.5);
            Assert.True(a.Clone().Lerp(b, (float)0.5).Y == -y * 0.5);
            Assert.True(a.Clone().Lerp(b, (float)0.5).Z == z * 0.5);

            Assert.True(a.Clone().Lerp(b, 1).Equals(b));
        }

        [Fact()]
        public void LerpVectorsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void CrossTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(2 * x, -y, (float)0.5 * z);
            var crossed = new Vector3(18, 12, -18);

            a.Cross(b);
            Assert.True(MathF.Abs(a.X - crossed.X) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Y - crossed.Y) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Z - crossed.Z) <= MathUtils.EPS);
        }

        [Fact()]
        public void CrossVectorsTest()
        {
            var a = new Vector3(x, y, z);
            var b = new Vector3(x, -y, z);
            var c = new Vector3();
            var crossed = new Vector3(24, 0, -12);

            c.CrossVectors(a, b);
            Assert.True(MathF.Abs(c.X - crossed.X) <= MathUtils.EPS);
            Assert.True(MathF.Abs(c.Y - crossed.Y) <= MathUtils.EPS);
            Assert.True(MathF.Abs(c.Z - crossed.Z) <= MathUtils.EPS);
        }

        [Fact()]
        public void AngleToTest()
        {
            var a = new Vector3(0, (float)-0.18851655680720186, (float)0.9820700116639124);
            var b = new Vector3(0, (float)0.18851655680720186, (float)-0.9820700116639124);

            Assert.Equal(a.AngleTo(a), 0);
            Assert.Equal(a.AngleTo(b), Mathf.PI);

            var x = new Vector3(1, 0, 0);
            var y = new Vector3(0, 1, 0);
            var z = new Vector3(0, 0, 1);

            Assert.Equal(x.AngleTo(y), Mathf.PI / 2);
            Assert.Equal(x.AngleTo(z), Mathf.PI / 2);
            Assert.Equal(z.AngleTo(x), Mathf.PI / 2);

            Assert.True(MathF.Abs(x.AngleTo(new Vector3(1, 1, 0)) - (Mathf.PI / 4)) < 0.0000001);

        }

        [Fact()]
        public void DistanceToDistanceToSqTest()
        {
            var a = new Vector3(x, 0, 0);
            var b = new Vector3(0, -y, 0);
            var c = new Vector3(0, 0, z);
            var d = new Vector3();

            Assert.True(a.DistanceTo(d) == x);
            Assert.True(a.DistanceToSquared(d) == x * x);

            Assert.True(b.DistanceTo(d) == y);
            Assert.True(b.DistanceToSquared(d) == y * y);

            Assert.True(c.DistanceTo(d) == z);
            Assert.True(c.DistanceToSquared(d) == z * z);
        }


        [Fact()]
        public void SetFromMatrixPositionTest()
        {
            var a = new Vector3();
            var m = new Matrix4().Set(2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53);

            a.SetFromMatrixPosition(m);
            Assert.StrictEqual(a.X, 7);
            Assert.StrictEqual(a.Y, 19);
            Assert.StrictEqual(a.Z, 37);
        }

        [Fact()]
        public void SetValueTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SetFromMatrixScaleTest()
        {
            var a = new Vector3();
            var m = new Matrix4().Set(2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53);
            var expected = new Vector3((float)25.573423705088842, (float)31.921779399024736, (float)35.70714214271425);

            a.SetFromMatrixScale(m);
            Assert.True(MathF.Abs(a.X - expected.X) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Y - expected.Y) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Z - expected.Z) <= MathUtils.EPS);
        }

        [Fact()]
        public void SetFromMatrixColumnTest()
        {
            var a = new Vector3();
            var m = new Matrix4().Set(2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53);

            a.SetFromMatrixColumn(m, 0);
            Assert.StrictEqual(a.X, 2);
            Assert.StrictEqual(a.Y, 11);
            Assert.StrictEqual(a.Z, 23);

            a.SetFromMatrixColumn(m, 2);
            Assert.StrictEqual(a.X, 5);
            Assert.StrictEqual(a.Y, 17);
            Assert.StrictEqual(a.Z, 31);
        }

        [Fact()]
        public void SetFromMatrix3ColumnTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void EqualsTest()
        {
            var a = new Vector3(x, 0, z);
            var b = new Vector3(0, -y, 0);

            Assert.True(a.X != b.X);
            Assert.True(a.Y != b.Y);
            Assert.True(a.Z != b.Z);

            Assert.True(!a.Equals(b));
            Assert.True(!b.Equals(a));

            a.Copy(b);
            Assert.True(a.X == b.X);
            Assert.True(a.Y == b.Y);
            Assert.True(a.Z == b.Z);

            Assert.True(a.Equals(b));
            Assert.True(b.Equals(a));
        }

        [Fact()]
        public void FromArrayTest()
        {
            var a = new Vector3();
            var array = new float[] { 1, 2, 3, 4, 5, 6 };

            a.FromArray(array);
            Assert.StrictEqual(a.X, 1);
            Assert.StrictEqual(a.Y, 2);
            Assert.StrictEqual(a.Z, 3);

            a.FromArray(array, 3);
            Assert.StrictEqual(a.X, 4);
            Assert.StrictEqual(a.Y, 5);
            Assert.StrictEqual(a.Z, 6);
        }

        [Fact()]
        public void ToArrayTest()
        {
            var a = new Vector3(x, y, z);

            var array = a.ToArray();
            Assert.StrictEqual(array[0], x);
            Assert.StrictEqual(array[1], y);
            Assert.StrictEqual(array[2], z);

            array = new float[] { };
            a.ToArray(ref array);
            Assert.StrictEqual(array[0], x);
            Assert.StrictEqual(array[1], y);
            Assert.StrictEqual(array[2], z);

            array = new float[] { };
            a.ToArray(ref array, 1);
            //Assert.StrictEqual(array[0], null);
            Assert.StrictEqual(array[1], x);
            Assert.StrictEqual(array[2], y);
            Assert.StrictEqual(array[3], z);

        }

        [Fact()]
        public void ToArrayTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ProjectOnVectorTest()
        {
            var a = new Vector3(1, 0, 0);
            var b = new Vector3();
            var normal = new Vector3(10, 0, 0);

            Assert.True(b.Copy(a).ProjectOnVector(normal).Equals(new Vector3(1, 0, 0)));

            a.Set(0, 1, 0);
            Assert.True(b.Copy(a).ProjectOnVector(normal).Equals(new Vector3(0, 0, 0)));

            a.Set(0, 0, -1);
            Assert.True(b.Copy(a).ProjectOnVector(normal).Equals(new Vector3(0, 0, 0)));

            a.Set(-1, 0, 0);
            Assert.True(b.Copy(a).ProjectOnVector(normal).Equals(new Vector3(-1, 0, 0)));

        }

        [Fact()]
        public void ApplyEulerTest()
        {
            var a = new Vector3(x, y, z);
            var euler = new Euler(90, -45, 0);
            var expected = new Vector3((float)-2.352970120501014, (float)(-4.7441750936226645), (float)0.9779234597246458);

            a.ApplyEuler(euler);
            Assert.True(MathF.Abs(a.X - expected.X) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Y - expected.Y) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Z - expected.Z) <= MathUtils.EPS);

        }

        [Fact()]
        public void ApplyAxisAngleTest()
        {
            var a = new Vector3(x, y, z);
            var axis = new Vector3(0, 1, 0);
            var angle = Mathf.PI / (float)4.0;
            var expected = new Vector3(3 * Mathf.Sqrt(2), 3, Mathf.Sqrt(2));

            a.ApplyAxisAngle(axis, angle);
            Assert.True(MathF.Abs(a.X - expected.X) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Y - expected.Y) <= MathUtils.EPS);
            Assert.True(MathF.Abs(a.Z - expected.Z) <= MathUtils.EPS);
        }

        [Fact()]
        public void ProjectOnPlaneTest()
        {
            var a = new Vector3(1, 0, 0);
            var b = new Vector3();
            var normal = new Vector3(1, 0, 0);

            Assert.True(b.Copy(a).ProjectOnPlane(normal).Equals(new Vector3(0, 0, 0)));

            a.Set(0, 1, 0);
            Assert.True(b.Copy(a).ProjectOnPlane(normal).Equals(new Vector3(0, 1, 0)));

            a.Set(0, 0, -1);
            Assert.True(b.Copy(a).ProjectOnPlane(normal).Equals(new Vector3(0, 0, -1)));

            a.Set(-1, 0, 0);
            Assert.True(b.Copy(a).ProjectOnPlane(normal).Equals(new Vector3(0, 0, 0)));

        }

        [Fact()]
        public void RandomTest()
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