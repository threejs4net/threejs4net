using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeJs4Net.Math
{
    public interface IVector<T>
    {
        T SetComponent(int index, float value);
        public float GetComponent(int index);
        T Copy(T v);
        T Add(T v);

        //      set( ...args: number[] ): this;

        T SetScalar(float scalar);
        T AddVectors(T v1, T v2);
        T AddScaledVector(T v, float s);
        T AddScalar(float scalar);
        T Sub(T v);
        T SubVectors(T a, T b);
        T MultiplyScalar(float scalar);
        T DivideScalar(float scalar);
        T Negate();
        float Dot(T v);
        float LengthSq();
        float Length();
        T Normalize();
        //* NOTE: Vector4 doesn't have the property.
        public float DistanceTo(T vector);
        //* NOTE: Vector4 doesn't have the property.
        public float DistanceToSquared(T vector);
        T SetLength(float length);
        T Lerp(T v, float alpha);
        bool Equals(T v);
        T Clone();
    }
}
