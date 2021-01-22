using System.Diagnostics;
using ThreeJs4Net.Renderers.Shaders;

namespace ThreeJs4Net.Materials
{
    [DebuggerDisplay("Location = {Location}, Uniform = {Uniform}")]
    public struct UniformLocation
    {
        public Uniform Uniform;
        public int Location;
    }
}