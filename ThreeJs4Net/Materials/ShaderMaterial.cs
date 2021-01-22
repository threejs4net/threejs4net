using System.Collections;
using ThreeJs4Net.Renderers.Shaders;

namespace ThreeJs4Net.Materials
{
    public class ShaderMaterial : Material, IWireframe, IAttributes, IUniforms, IMorphTargets
    {
        public Hashtable defines = new Hashtable();

        public Uniforms Uniforms { get; set; }

        // IAttributes

        public Attributes Attributes { get; set; }

        //

        public string VertexShader = "void main() {\n\tgl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );\n}";

        public string FragmentShader = "void main() {\n\tgl_FragColor = vec4( 1.0, 0.0, 0.0, 1.0 );\n}";

        public int Shading = Three.SmoothShading;

        public float Linewidth = 1;

        // IWireFrameable

        public bool Wireframe { get; set; }

        public float WireframeLinewidth { get; set; }

        //

        public bool Fog = false; // Set to use scene fog

        public bool Lights = false; // Set to use scene lights


        public bool Skinning = false; // Set to use skinning attribute streams

        public bool MorphTargets { get; set; }  // Set to use morph targets

        public bool MorphNormals = false; // Set to use morph normals

        // When rendered geometry doesn"t include these Attributes but the material does,
        // use these default values in WebGL. This avoids errors when buffer data is missing.
        //public object defaultAttributeValues = {
        //"color": [ 1, 1, 1 ],
        //"uv": [ 0, 0 ],
        //"uv2": [ 0, 0 ]
        //};

        public string Index0AttributeName = null;
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public ShaderMaterial(Hashtable parameters = null)
        {
            this.type = "ShaderMaterial";

            // IAttributes
            this.Attributes = new Attributes();

            // IUniforms
            Uniforms = new Uniforms();

            // IWireFrameable
            this.Wireframe = false;
            this.WireframeLinewidth = 1;
            
            this.SetValues(parameters);
        }
    }
}
