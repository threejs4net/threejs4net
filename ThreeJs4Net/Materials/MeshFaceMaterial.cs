using System.Collections.Generic;

namespace ThreeJs4Net.Materials
{
    public class MeshFaceMaterial : Material
    {
        public List<Material> Materials;

        /// <summary>
        /// Constructor
        /// </summary>
        public MeshFaceMaterial()
        {
            this.type = "MeshFaceMaterial";
        }
    }
}
