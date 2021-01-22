using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Renderers.WebGL.PlugIns
{
    class SpritePlugin
    {
        private WebGLRenderer _renderer;

        private IList<Object3D> _sprites;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="sprites"></param>
        public SpritePlugin(WebGLRenderer renderer, IList<Object3D> sprites)
        {
            this._renderer = renderer;
            this._sprites = sprites;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        public void Render(Scene scene, Camera camera)
        {
            if (this._sprites.Count == 0) return;

 		    // restore gl

            GL.Enable(EnableCap.CullFace);
		
		    this._renderer.ResetGlState();
       }
    }
}
