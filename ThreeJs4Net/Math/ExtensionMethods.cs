using System;
using System.Collections.Generic;
using System.Drawing;

namespace ThreeJs4Net.Math
{
    public static class Mat
    {
        private static readonly Random random = new Random();

        public const double PI2 = (2 * 3.14159265358979323846);

        public const double HalfPI = (3.14159265358979323846 / 2.0f);

        public const double SQRT1_2 = (0.7071067811865476); 

        public static double RadToDeg(double rad)
        {
            return (rad * 180.0 / System.Math.PI);
        }

        public static double DegToRad(double deg)
        {
            return (System.Math.PI * deg / 180.0);
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static float Clamp(float val, float min, float max) 
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static float Random()
        {
            return (float)random.NextDouble();
        }

        public static Color Random(this Color value)
        {
            return Color.FromArgb(255, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="values"></param>
        public static void Add(this List<float> list, params float[] values)
        {
            list.AddRange(values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static float hue2rgb ( float p, float q, float t )
        {
		    if ( t < 0.0f ) t += 1;
		    if ( t > 1.0f ) t -= 1;
		    if ( t < 1.0f / 6.0f ) return p + ( q - p ) * 6 * t;
		    if ( t < 1.0f / 2.0f ) return q;
		    if ( t < 2.0f / 3.0f ) return p + ( q - p ) * 6 * ( 2 / 3.0f - t );

		    return p;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Color OffsetHSL (this Color value, float h, float s, float l)
        {
            var hsl = new HSLColor(value);

            hsl.Hue += h; hsl.Saturation += s; hsl.Luminosity += l;

            value = hsl;

            return value;
        }

    }
}
