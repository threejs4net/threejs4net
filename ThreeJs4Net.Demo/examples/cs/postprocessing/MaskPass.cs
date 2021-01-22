using OpenTK.Graphics.OpenGL;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Renderers;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples.cs.postprocessing
{
    public class MaskPass : IPass
    {
        private readonly Scene scene;
        private readonly Camera camera;

        public bool Enabled { get; set; }
        public bool Clear { get; set; }
        public bool NeedsSwap { get; set; }

        public bool Inverse = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        public MaskPass(Scene scene, Camera camera)
        {
            this.scene = scene;
            this.camera = camera;

            this.Enabled = true;
            this.Clear = true;
            this.NeedsSwap = false;

            this.Inverse = false;
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
            // don't update Color or depth

            GL.ColorMask(false, false, false, false);
            GL.DepthMask(false);

            // Set up stencil

            int writeValue; int clearValue;

            if (this.Inverse)
            {
                writeValue = 0;
                clearValue = 1;
            }
            else
            {
                writeValue = 1;
                clearValue = 0;
            }

            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
            GL.StencilFunc(StencilFunction.Always, writeValue, 0xffffffff);
            GL.ClearStencil(clearValue);

            // draw into the stencil buffer

            renderer.Render(this.scene, this.camera, readBuffer, this.Clear);
            renderer.Render(this.scene, this.camera, writeBuffer, this.Clear);

            // re-enable update of Color and depth

            GL.ColorMask(true, true, true, true);
            GL.DepthMask(true);

            // only render where stencil is Set to 1

            GL.StencilFunc(StencilFunction.Equal, 1, 0xffffffff);  // draw if == 1
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
        }
    }
}
