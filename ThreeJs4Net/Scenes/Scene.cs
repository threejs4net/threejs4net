using System;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;

namespace ThreeJs4Net.Scenes
{
    public class Scene : Object3D
    {
        #region Fields

        public bool AutoUpdate;
        
        public Material OverrideMaterial;

        public Fog Fog;
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor.  Create a new scene object.
        /// </summary>
        public Scene()
        {
            this.type = "Scene";

            this.Fog = null;
            this.OverrideMaterial = null;

            this.AutoUpdate = true; // checked by the renderer
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
            return null;
        }

        #endregion
    }
}