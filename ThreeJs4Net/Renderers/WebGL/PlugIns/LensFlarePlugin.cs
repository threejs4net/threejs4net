using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Renderers.WebGL.PlugIns
{
    class LensFlarePlugin
    {
        private WebGLRenderer _renderer;

        private IList<Object3D> _flares;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="flares"></param>
        public LensFlarePlugin(WebGLRenderer renderer, IList<Object3D> flares)
        {
            this._renderer = renderer;
            this._flares = flares;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Render(Scene scene, Camera camera, int width, int height)
        {
     		if ( this._flares.Count == 0 ) return;

            // ...

            GL.Disable(EnableCap.CullFace);
            GL.DepthMask(false);

            for (var i = 0; i < this._flares.Count; i++)
            {
            }

            // restore gl

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            this._renderer.ResetGlState();
        }
    }
}
