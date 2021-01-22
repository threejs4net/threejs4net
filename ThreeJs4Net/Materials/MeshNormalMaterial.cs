using System.Collections;

namespace ThreeJs4Net.Materials
{
    public class MeshNormalMaterial : Material, IWireframe, IMorphTargets
    {
        public int Shading = Three.FlatShading;

        public bool Wireframe { get; set; }

        public float WireframeLinewidth { get; set; }

        public bool MorphTargets { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public MeshNormalMaterial(Hashtable parameters = null)
        {
            Shading = Three.FlatShading;

            this.type = "MeshNormalMaterial";

            this.Wireframe = false;
            this.WireframeLinewidth = 1;

            this.SetValues(parameters);
        }
    }
}
