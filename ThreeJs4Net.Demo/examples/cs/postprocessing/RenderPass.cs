using System.Drawing;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Renderers;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples.cs.postprocessing
{
    public class RenderPass : IPass
    {
        public Scene Scene;
        public Camera Camera;

        public Material OverrideMaterial;

        public Color? ClearColor;
        public int ClearAlpha;

        public Color OldClearColor = new Color();
        public int OldClearAlpha = 255;

        public bool Enabled { get; set; }
        public bool Clear { get; set; }
        public bool NeedsSwap { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        public RenderPass(Scene scene, Camera camera)
        {
            this.Scene = scene;
            this.Camera = camera;

            this.Enabled = true;
            this.Clear = true;
            this.NeedsSwap = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        /// <param name="overrideMaterial"></param>
        /// <param name="clearColor"></param>
        /// <param name="clearAlpha"></param>
        public RenderPass(Scene scene, Camera camera, Material overrideMaterial, Color clearColor, int clearAlpha) : this(scene, camera)
        {
            this.OverrideMaterial = overrideMaterial;
            this.ClearColor = clearColor;
            this.ClearAlpha = clearAlpha;
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
            this.Scene.OverrideMaterial = this.OverrideMaterial;

            if (null != this.ClearColor)
            {
                this.OldClearColor = renderer.ClearColor;
                this.OldClearAlpha = renderer.ClearAlpha;

                renderer.SetClearColor(this.ClearColor.Value, this.ClearAlpha);
            }

            renderer.Render(this.Scene, this.Camera, readBuffer, this.Clear);

            if (null != this.ClearColor)
            {
                renderer.SetClearColor(this.OldClearColor, this.OldClearAlpha);
            }

            this.Scene.OverrideMaterial = null;
        }
    }
}
