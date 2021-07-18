using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeJs4Net.Math
{
    public class MathUtils
    {
        public static float DEG2RAD = (float)(System.Math.PI / 180);
        public static float RAD2DEG = (float)(180 / System.Math.PI);
        public static float EPS = (float)0.0001;
        public static float EPS3 = (float)0.001;
        public static float EPS5 = (float)0.00001;

        public static float Clamp(float value, float min, float max)
        {
            return System.Math.Max(min, System.Math.Min(max, value));
        }
    }
}
