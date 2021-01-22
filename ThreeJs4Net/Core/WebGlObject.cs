using ThreeJs4Net.Materials;

namespace ThreeJs4Net.Core
{
    public class WebGlObject
    {
        public long id;

        public BaseGeometry buffer;

        public Object3D object3D;

        public Material material;

        public float z;

        public bool render;
    }
}