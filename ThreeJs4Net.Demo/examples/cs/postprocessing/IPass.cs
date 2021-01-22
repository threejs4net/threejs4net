using ThreeJs4Net.Renderers;

namespace ThreeJs4Net.Demo.examples.cs.postprocessing
{
    public interface IPass
    {
        bool Enabled { get; set; }
        bool Clear { get; set; }
        bool NeedsSwap { get; set; }

        void Render(WebGLRenderer renderer, WebGLRenderTarget writeBuffer, WebGLRenderTarget readBuffer, float delta);
    }
}
