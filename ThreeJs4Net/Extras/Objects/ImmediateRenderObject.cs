using ThreeJs4Net.Core;

namespace ThreeJs4Net.Extras.Objects
{
    public class ImmediateRenderObject : Object3D
    {
        public object immediateRenderCallback;

        public delegate void render();
    }
}
