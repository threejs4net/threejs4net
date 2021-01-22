using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_buffergeometry_rawshader", ExampleCategory.OpenTK, "BufferGeometry")]
    class webgl_buffergeometry_rawshader : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Object3D mesh;

        private const string FragmentShader = @"
            #ifdef GL_ES precision mediump float;
			    precision mediump int;
            #endif

			uniform float time;

			varying vec3 vPosition;
			varying vec4 vColor;

			void main()	{

				vec4 Color = vec4( vColor );
				Color.r += sin( vPosition.x * 10.0 + time ) * 0.5;

				gl_FragColor = Color;

			}";

        private const string VertexShader = @"
            #ifdef GL_ES 
                precision mediump float;
			    precision mediump int;
            #endif

			uniform mat4 modelViewMatrix; // optional
			uniform mat4 projectionMatrix; // optional

			attribute vec3 position;
			attribute vec4 Color;

			varying vec3 vPosition;
			varying vec4 vColor;

			void main()	{

				vPosition = position;
				vColor = Color;

				gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

			}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(50, control.Width / (float)control.Height, 1, 10);
            this.camera.Position.Z = 2;

            scene = new Scene();

            // geometry

            const int triangles = 500;

            var geometry = new BufferGeometry();

            var vertices = new BufferAttribute<float>(new float[triangles * 3 * 3], 3);

            for (var i = 0; i < vertices.length / vertices.ItemSize; i++)
            {
                vertices.SetXYZ(i, (float)(Mat.Random() - 0.5), (float)(Mat.Random() - 0.5), (float)(Mat.Random() - 0.5));
            }

            geometry.AddAttribute("position", vertices);

            var colors = new BufferAttribute<float>(new float[triangles * 3 * 4], 4);

            for (var i = 0; i < colors.length / colors.ItemSize; i++)
            {
                colors.SetXYZW(i, Mat.Random(), Mat.Random(), Mat.Random(), Mat.Random());
            }

            geometry.AddAttribute("Color", colors);

            // material

            var material = new RawShaderMaterial()
            {
                Uniforms = new Uniforms { { "time", new Uniform() { {"type", "f"}, {"value", 1.0f} } } },
                VertexShader = VertexShader,
                FragmentShader = FragmentShader,
                Side = Three.DoubleSide,
                Transparent = true,
            };

            mesh = new Mesh(geometry, material);
            scene.Add(mesh);

            renderer.SetClearColor((Color)colorConvertor.ConvertFromString("#101010"));
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
            var time = stopWatch.ElapsedMilliseconds;

            var object3D = scene.Children[0];

            object3D.Rotation.Y = time * 0.0005f;

            ((IUniforms)object3D.Material).Uniforms["time"]["value"] = time * 0.005;

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
