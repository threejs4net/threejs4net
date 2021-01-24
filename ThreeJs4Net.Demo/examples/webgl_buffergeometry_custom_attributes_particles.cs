using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_buffergeometry_custom_attributes_particles", ExampleCategory.OpenTK, "BufferGeometry", 0.2f)]
    class webgl_buffergeometry_custom_attributes_particles : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Uniforms uniforms;

        private PointCloud particleSystem;

        private BufferGeometry geometry;

        private const int Particles = 100000;

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
            uniform vec3 color;
			uniform sampler2D texture;

			varying vec3 vColor;

			void main() {

				gl_FragColor = vec4( color * vColor, 1.0 );

				gl_FragColor = gl_FragColor * texture2D( texture, gl_PointCoord );

			}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(40, control.Width / (float)control.Height, 1, 10000);
            this.camera.Position.Z = 300;

            scene = new Scene();

            var attributes = new Attributes 
            {
                { "size",        new Attribute() { { "f", null } } }, 
                { "customcolor", new Attribute() { { "c", null } } } 
            };

            uniforms = new Uniforms
            {
                { "color",   new Uniform() { {"type", "c"},  {"value", Color.White}} },
                { "texture", new Uniform() { {"type", "t"},  {"value", ImageUtils.LoadTexture(@"examples\textures/sprites/spark1.png")}} },
            };

			var shaderMaterial = new ShaderMaterial() {
				Uniforms =       uniforms,
				Attributes =     attributes,
				VertexShader =   VertexShader,
				FragmentShader = FragmentShader,

				Blending =       Three.AdditiveBlending,
				DepthTest =      false,
				Transparent =    true
			};

			const int Radius = 200;

			geometry = new BufferGeometry();

            var positions = new float[Particles * 3];
            var values_color = new float[Particles * 3];
            var values_size = new float[Particles];

            var color = new Color();

            for(var v = 0; v < Particles; v++ ) 
            {
				values_size[ v ] = 20;

                positions[v * 3 + 0] = (Mat.Random() * 2 - 1) * Radius;
                positions[v * 3 + 1] = (Mat.Random() * 2 - 1) * Radius;
                positions[v * 3 + 2] = (Mat.Random() * 2 - 1) * Radius;

                color = new HSLColor(512 * v / (float)Particles, 1.0f, 0.5f);
                color = Color.DeepPink;

                values_color[v * 3 + 0] = color.R;
                values_color[v * 3 + 1] = color.G;
                values_color[v * 3 + 2] = color.B;
            }

            geometry.AddAttribute("position",    new BufferAttribute<float>(positions, 3));
            geometry.AddAttribute("customColor", new BufferAttribute<float>(values_color, 3));
            geometry.AddAttribute("size",        new BufferAttribute<float>(values_size, 1));

			particleSystem = new PointCloud( geometry, shaderMaterial );

			scene.Add( particleSystem );
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
        public override void Render()
        {
            Debug.Assert(null != this.renderer);
            Debug.Assert(null != this.particleSystem);

            var time = stopWatch.ElapsedMilliseconds * 0.005f;

            particleSystem.Rotation.Z = 0.01f * time;

            var size = geometry.Attributes["size"] as BufferAttribute<float>;
            Debug.Assert(null != size);
            
            for (var i = 0; i < Particles; i++)
                size.Array[i] = 10 * (1 + (float)System.Math.Sin(0.1 * i + time));

            size.needsUpdate = true;

            renderer.Render(scene, camera);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Unload()
        {
            this.scene.Dispose();
            this.camera.Dispose();

            base.Unload();
        }
    }
}
