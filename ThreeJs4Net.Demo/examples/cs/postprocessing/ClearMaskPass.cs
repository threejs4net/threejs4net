using OpenTK.Graphics.OpenGL;
using ThreeJs4Net.Renderers;

namespace ThreeJs4Net.Demo.examples.cs.postprocessing
{
    public class ClearMaskPass : IPass
    {
        public bool Enabled { get; set; }
        public bool Clear { get; set; }
        public bool NeedsSwap { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ClearMaskPass()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="writeBuffer"></param>
        /// <param name="readBuffer"></param>
        /// <param name="delta"></param>
        public void Render(WebGLRenderer renderer, WebGLRenderTarget writeBuffer, WebGLRenderTarget readBuffer, float delta)
        {
            GL.Disable(EnableCap.StencilTest);
        }
    }
}
