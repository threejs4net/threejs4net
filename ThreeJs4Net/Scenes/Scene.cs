using System;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Scenes
{
    public class Scene : Object3D
    {
        #region Fields
        public bool AutoUpdate;
        public Material OverrideMaterial;
        public Fog Fog;
        public Texture Environment;
        #endregion

        public Scene()
        {
            this.type = "Scene";

            this.Fog = null;
            this.OverrideMaterial = null;
            this.Environment = null;

            this.AutoUpdate = true; // checked by the renderer
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}