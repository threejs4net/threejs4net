﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeJs4Net.Extras.Core
{
    public static class Interpolation
    {
        public static float CatmullRom(float t, float p0, float p1, float p2, float p3)
        {
            float v0 = (p2 - p0) * (float)0.5;
            float v1 = (p3 - p1) * (float)0.5;
            float t2 = t * t;
            float t3 = t * t2;
            return (2 * p1 - 2 * p2 + v0 + v1) * t3 + (-3 * p1 + 3 * p2 - 2 * v0 - v1) * t2 + v0 * t + p1;
        }

        public static float QuadraticBezierP0(float t, float p)
        {
            var k = 1 - t;
            return k * k * p;
        }

        public static float QuadraticBezierP1(float t, float p)
        {
            return 2 * (1 - t) * t * p;
        }

        public static float QuadraticBezierP2(float t, float p)
        {

            return t * t * p;

        }

        public static float QuadraticBezier(float t, float p0, float p1, float p2)
        {
            return QuadraticBezierP0(t, p0) + QuadraticBezierP1(t, p1) +
                   QuadraticBezierP2(t, p2);
        }

        public static float CubicBezierP0(float t, float p)
        {
            var k = 1 - t;
            return k * k * k * p;
        }

        public static float CubicBezierP1(float t, float p)
        {
            var k = 1 - t;
            return 3 * k * k * t * p;
        }

        public static float CubicBezierP2(float t, float p)
        {
            return 3 * (1 - t) * t * t * p;
        }

        public static float CubicBezierP3(float t, float p)
        {
            return t * t * t * p;
        }

        public static float CubicBezier(float t, float p0, float p1, float p2, float p3)
        {
            return CubicBezierP0(t, p0) + CubicBezierP1(t, p1) + CubicBezierP2(t, p2) +
                   CubicBezierP3(t, p3);
        }
    }
}
