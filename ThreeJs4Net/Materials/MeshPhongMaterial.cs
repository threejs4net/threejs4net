using System.Collections;
using System.Drawing;
using ThreeJs4Net.Math;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Materials
{
    public class MeshPhongMaterial : Material, IWireframe, IMap, IMorphTargets
    {
        public Color Color = Color.White; // diffuse

        public Color Ambient = Color.White;

        public Color Emissive = Color.Black;

        public Color Specular = Color.DarkSlateBlue;

        public float Shininess = 30;

        public bool Metal = false;

        public bool WrapAround = false;

        public Vector3 WrapRgb = new Vector3( 1, 1, 1 );

        // IMap

        public Texture Map  { get; set; }

        public Texture AlphaMap { get; set; }

        public Texture SpecularMap { get; set; }

        public Texture NormalMap { get; set; }

        public Texture BumpMap { get; set; }

        public Texture LightMap { get; set; }


        //


        public float BumpScale = 1;

        public Vector2 NormalScale = new Vector2( 1, 1 );

  //      public Texture EnvMap = null;

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
        public MeshPhongMaterial(Hashtable parameters = null)
        {
            this.type = "MeshPhongMaterial";

            // IWireFrameable
            this.Wireframe = false;
            this.WireframeLinewidth = 1;

            this.SetValues(parameters);
        }
}
}
