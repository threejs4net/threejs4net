using System.Collections;
using System.Drawing;
using ThreeJs4Net.Math;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Materials
{
    public class MeshLambertMaterial : Material, IWireframe, IMap, IMorphTargets
    { 
        public Color Color = Color.White; // diffuse

        public Color Ambient = Color.White;

        public Color Emissive = Color.Black;

        public bool WrapAround = false;

        public Vector3 WrapRgb = new Vector3( 1, 1, 1 );


        // IMap

        public Texture Map { get; set; }

        public Texture AlphaMap { get; set; }

        public Texture SpecularMap { get; set; }

        public Texture NormalMap { get; set; } // TODO: not in ThreeJs, just to be an IMap. Must be NULL

        public Texture BumpMap { get; set; } // TODO: not in ThreeJs, just to be an IMap.  Must be NULL

        public Texture LightMap { get; set; } 

        

        
        public int Combine = Three.MultiplyOperation;

        public float Reflectivity = 1;

        public float RefractionRatio = 0.98f;

        public bool Fog = true;

        public int Shading = Three.SmoothShading;

        // IWireFrameable

        public bool Wireframe { get; set; }

        public float WireframeLinewidth { get; set; }

        //

        public string WireframeLinecap = "round";

        public string WireframeLinejoin = "round";


        public bool Skinning = false;

        public bool MorphTargets { get; set; }

        public bool MorphNormals = false;
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public MeshLambertMaterial(Hashtable parameters = null)
        {
            this.type = "MeshLambertMaterial";

            // IWireFrameable
            this.Wireframe = false;
            this.WireframeLinewidth = 1;
            
            this.SetValues(parameters);
        }
    }
}
