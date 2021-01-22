using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Renderers;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_rtt", ExampleCategory.OpenTK, "rtt", 0.5f)]
    class webgl_rtt : Example
    {
        private PerspectiveCamera camera;

        private OrthographicCamera cameraRtt;

        private Scene scene, sceneRtt, sceneScreen;

        private WebGLRenderTarget rtTexture;

        private ShaderMaterial material;

        private ShaderMaterial materialScreen;

        private Mesh quad, zmesh1, zmesh2;

        private Vector2 mouse = new Vector2();

        private const string FragmentShaderScreen = @"
			varying vec2 vUv;
			uniform sampler2D tDiffuse;

			void main() {
				gl_FragColor = texture2D( tDiffuse, vUv );
			}";

        private const string FragmentShaderPass1 = @"
			varying vec2 vUv;
			uniform float time;

			void main() {
				float r = vUv.x;
				if( vUv.y < 0.5 ) r = 0.0;
				float g = vUv.y;
				if( vUv.x < 0.5 ) g = 0.0;

				gl_FragColor = vec4( r, g, time, 1.0 );
			}";

        private const string VertexShader = @"
			varying vec2 vUv;

			void main() {
				vUv = uv;
				gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );
			}";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(60, control.Width / (float)control.Height, 1, 10000);
            this.camera.Position.Z = 100;

            this.cameraRtt = new OrthographicCamera(control.Width / -2, control.Width / 2, control.Height / 2, control.Height / -2, -10000, 10000);
            this.cameraRtt.Position.Z = 100;

            scene = new Scene();
            this.sceneRtt = new Scene();
            sceneScreen = new Scene();

            var light = new DirectionalLight(Color.White);
            light.Position.Set(0, 0, 1).Normalize();
            this.sceneRtt.Add(light);

            light = new DirectionalLight((Color)colorConvertor.ConvertFromString("#ffaaaa"), 1.5f);
            light.Position.Set(0, 0, -1).Normalize();
            this.sceneRtt.Add(light);

            rtTexture = new WebGLRenderTarget(control.Width, control.Height) { MinFilter = Three.LinearFilter, MagFilter = Three.NearestFilter, Format = Three.RGBFormat };

            material = new ShaderMaterial()
            {
                Uniforms = new Uniforms { { "time", new Uniform() { { "type", "f" }, { "value", 0.0f } } } },
                VertexShader = VertexShader,
                FragmentShader = FragmentShaderPass1,
            };

            materialScreen = new ShaderMaterial()
            {
                Uniforms = new Uniforms { { "tDiffuse", new Uniform() { { "type", "t" }, { "value", rtTexture } } } },
                VertexShader = VertexShader,
                FragmentShader = FragmentShaderScreen,
            };

            var plane = new PlaneBufferGeometry(control.Width, control.Height);

            quad = new Mesh(plane, material);
            quad.Position.Z = -100;
            this.sceneRtt.Add(quad);

            var geometry = new TorusGeometry(100, 25, 15, 30);      

            var mat1 = new MeshPhongMaterial() { Color = (Color)colorConvertor.ConvertFromString("#555555"), Specular = (Color)colorConvertor.ConvertFromString("#ffaa00"), Shininess = 5 };
            var mat2 = new MeshPhongMaterial() { Color = (Color)colorConvertor.ConvertFromString("#550000"), Specular = (Color)colorConvertor.ConvertFromString("#ff2200"), Shininess = 5 };

            zmesh1 = new Mesh(geometry, mat1);
            zmesh1.Position.Set(0, 0, 100);
            zmesh1.Scale.Set(1.5f, 1.5f, 1.5f);
            this.sceneRtt.Add(zmesh1);

            zmesh2 = new Mesh(geometry, mat2);
            zmesh2.Position.Set(0, 150, 100);
            zmesh2.Scale.Set(0.75f, 0.75f, 0.75f);
            this.sceneRtt.Add(zmesh2);

            quad = new Mesh(plane, materialScreen);
            quad.Position.Z = -100;
            sceneScreen.Add(quad);

        	//	
            var geometry3 = new SphereGeometry(10, 64, 32);
			var material2 = new MeshBasicMaterial() { Color = Color.White/*, Map = rtTexture*/ };

            int n = 5;
        	for( var j = 0; j < n; j ++ ) {

				for( var i = 0; i < n; i ++ ) {

                    var mesh = new Mesh(geometry3, material2);

				    mesh.Position.X = (i - (n - 1) / 2) * 20;
				    mesh.Position.Y = (j - (n - 1) / 2) * 20;
					mesh.Position.Z = 0;

					mesh.Rotation.Y = (float)-System.Math.PI / 2.0f;

				    scene.Add(mesh);
				}

			}

            renderer.AutoClear = false;
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
            this.mouse.X = (here.X - clientSize.Width / 2);
            this.mouse.Y = (here.Y - clientSize.Height / 2);
        }

        private float delta = 0.01f;

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            Debug.Assert(null != this.renderer);

            var time = stopWatch.ElapsedMilliseconds * 0.0015f;

            camera.Position.X += (this.mouse.X - camera.Position.X) * .05f;
            camera.Position.Y += (-this.mouse.Y - camera.Position.Y) * .05f;

            camera.LookAt(scene.Position);

            if (zmesh1 != null && zmesh2 != null)
            {
                zmesh1.Rotation.Y = -time;
                zmesh2.Rotation.Y = -time + (float)(System.Math.PI / 2.0f);
            }

            if ((float)material.Uniforms["time"]["value"] > 1 || (float)material.Uniforms["time"]["value"] < 0)
            {
                delta *= -1;
            }

            material.Uniforms["time"]["value"] = (float)material.Uniforms["time"]["value"] + delta;

            if (true)
            {
                renderer.Render(sceneRtt, cameraRtt, null, true);
            }
            else
            {
                renderer.Clear();

                // Render first scene into texture

                renderer.Render(sceneRtt, cameraRtt, rtTexture, true);

                // Render full screen quad with generated texture

                renderer.Render(sceneScreen, cameraRtt);

                // Render second scene to screen
                // (using first scene as regular texture)

                renderer.Render(scene, camera);
            }
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
