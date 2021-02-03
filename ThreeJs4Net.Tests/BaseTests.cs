using System;
using System.Collections.Generic;
using System.Text;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Tests
{
    public class BaseTests
    {
        public float x = 2;
        public float y = 3;
        public float z = 4;
        public float w = 5;
        public Euler eulerZero = new Euler(0, 0, 0, Euler.RotationOrder.XYZ);
        public Euler eulerAxyz = new Euler(1, 0, 0, Euler.RotationOrder.XYZ);
        public Euler eulerAzyx = new Euler(0, 1, 0, Euler.RotationOrder.ZYX);

        public Vector2 negInf2 = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        public Vector2 posInf2 = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        public Vector2 negOne2 = new Vector2(-1, -1);
        public Vector2 zero2 = new Vector2();
        public Vector2 one2 = new Vector2(1, 1);
        public Vector2 two2 = new Vector2(2, 2);
        public Vector3 negInf3 = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        public Vector3 posInf3 = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        public Vector3 zero3 = new Vector3();
        public Vector3 one3 = new Vector3(1, 1, 1);
        public Vector3 two3 = new Vector3(2, 2, 2);
        public Vector3 unit3 = new Vector3( 1, 0, 0 );

        public bool RoundEquals(Vector3 left, Vector3 right, int precision)
        {
            return System.Math.Round(left.X, precision) == System.Math.Round(right.X, precision)
                   && System.Math.Round(left.Y, precision) == System.Math.Round(right.Y, precision)
                   && System.Math.Round(left.Z, precision) == System.Math.Round(right.Z, precision);
        }

        public Vector3 RoundVector(Vector3 v, int precision)
        {
            return new Vector3((float)System.Math.Round(v.X, precision), (float)System.Math.Round(v.Y, precision),
                (float)System.Math.Round(v.Z, precision));
        }

    }
}
