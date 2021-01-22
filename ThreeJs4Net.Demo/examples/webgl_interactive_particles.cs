using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_interactive_particles", ExampleCategory.OpenTK, "Interactive", 0.3f)]
    class webgl_interactive_particles : Example
    {
        private PerspectiveCamera camera;

        private Projector projector;

        private Scene scene;

        private Mesh mesh;

        private Line line;

        private Raycaster raycaster;

        private Uniforms uniforms;

        private PointCloud particles;

        private Vector2 mouse = new Vector2();

        private Attributes attributes;

        private int intersect;

        private const int PARTICLE_SIZE = 20;

        private const string VertexShader = @"
			attribute float size;
			attribute vec3 customColor;

			varying vec3 vColor;

			void main() {

				vColor = customColor;

				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );

				gl_PointSize = size * ( 300.0 / length( mvPosition.xyz ) );

				gl_Position = projectionMatrix * mvPosition;

			}";

        private const string FragmentShader = @"
			uniform vec3 Color;
			uniform sampler2D texture;

			varying vec3 vColor;

			void main() {

				gl_FragColor = vec4( Color * vColor, 1.0 );

				gl_FragColor = gl_FragColor * texture2D( texture, gl_PointCoord );

				if ( gl_FragColor.A < ALPHATEST ) discard;

			}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(45, control.Width / (float)control.Height, 1, 10000);
            camera.Position.Z = 250;

            scene = new Scene();
            scene.Fog = new Fog((Color)colorConvertor.ConvertFromString("#050505"), 2000, 3500);

            attributes = new Attributes
            { 
                { "size",        new Attribute() { {"type", "f"},  {"value", null }} },
                { "customColor", new Attribute() { {"type", "C"},  {"value", null }} },
            };

            uniforms = new Uniforms 
            { 
                { "Color",   new Uniform() { {"type", "C"},  {"value", Color.White}} },
                { "texture", new Uniform() { {"type", "t"},  {"value", ImageUtils.LoadTexture(@"examples\textures/sprites/disc.png")}} } 
            };
   
        	var shaderMaterial = new ShaderMaterial() {
                    Uniforms = uniforms,
					Attributes = attributes,
					VertexShader = VertexShader,
					FragmentShader = FragmentShader,
					AlphaTest = 0.9f };

            var geometry = new BoxGeometry(200, 200, 200, 16, 16, 16);

            particles = new PointCloud(geometry, shaderMaterial);

            var vertices = ((BoxGeometry)particles.Geometry).Vertices;

            attributes["size"]["value"] = new float[vertices.Count];
            attributes["customColor"]["value"] = new Color[vertices.Count];

            var values_size = attributes["size"]["value"] as float[];
            var values_color = attributes["customColor"]["value"] as Color[];

            for (int v = 0; v < vertices.Count; v++)
            {
                values_size[v] = PARTICLE_SIZE * 0.5f;
                values_color[v] =  Color.Salmon;//   new Color().setHSL(0.01f + 0.1f * (v / vl), 1.0f, 0.5f);
            }

            scene.Add(particles);

            //

            projector = new Projector();
            raycaster = new Raycaster();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        public override void Resize(Size clientSize)
        {
            Debug.Assert(null != this.camera);
            Debug.Assert(null != this.renderer);

            this.camera.Aspect = clientSize.Width / (float)clientSize.Height;
            this.camera.UpdateProjectionMatrix();

            this.renderer.Size = clientSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
            // Normalize mouse position
            mouse.X = (here.X / (float)clientSize.Width) * 2 - 1;
            mouse.Y = -(here.Y / (float)clientSize.Height) * 2 + 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
 			particles.Rotation.X += 0.0005f;
			particles.Rotation.Y += 0.001f;
/*
			var vector = new Vector3( mouse.X, mouse.Y, 0.5f );

			projector.UnprojectVector( vector, camera );

            raycaster.Ray = new Ray(camera.Position, vector.Sub(camera.Position).Normalize());

			var intersects = raycaster.IntersectObject( particles );

			if ( intersects.Count > 0 ) {

              //  if (intersect != intersects[0].index)
              //  {

              ////      var positions = ((BufferAttribute<float>)bg.Attributes["position"]).array;

              //      ((BufferAttribute<float>)Attributes["size"]).array[intersect] = PARTICLE_SIZE;

              //      intersect = intersects[0].index;

              //      ((BufferAttribute<float>)Attributes["size"]).array[intersect] = PARTICLE_SIZE * 1.25f;
              //      ((BufferAttribute<float>)Attributes["size"]).needsUpdate = true;

              //  }

            }
            else if (intersect != null)
            {
                //((BufferAttribute<float>)Attributes["size"]).array[intersect] = PARTICLE_SIZE;
                //((BufferAttribute<float>)Attributes["size"]).needsUpdate = true;
                //intersect = null;
			}
*/
			renderer.Render( scene, camera );
       }

    }
}
