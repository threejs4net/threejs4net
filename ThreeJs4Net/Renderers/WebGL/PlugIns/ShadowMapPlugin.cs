using System.Collections.Generic;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Renderers.WebGL.PlugIns
{
    class ShadowMapPlugin
    {
        private WebGLRenderer _renderer;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="lights"></param>
        /// <param name="webglObjects"></param>
        /// <param name="webglObjectsImmediate"></param>
        public ShadowMapPlugin(WebGLRenderer renderer, LightCollection lights, Dictionary<int, List<WebGlObject>> webglObjects, List<WebGlObject> webglObjectsImmediate)
        {
            this._renderer = renderer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        public void Render(Scene scene, Camera camera)
        {
		    if ( this._renderer.shadowMapEnabled == false ) 
                return;

            this._renderer.ResetGlState();
        }
    }
}
