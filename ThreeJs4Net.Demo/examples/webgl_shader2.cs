using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Scenes;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_shader2", ExampleCategory.OpenTK, "BufferGeometry")]
    class webgl_shader2 : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Uniforms uniforms1, uniforms2;

        private const string VertexShader = @"			
            varying vec2 vUv;

			void main()
			{
				vUv = uv;
				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );
				gl_Position = projectionMatrix * mvPosition;
			}";

        private const string FragmentShader1 = @"			
            uniform vec2 resolution;
			uniform float time;

			varying vec2 vUv;

			void main(void)
			{

				vec2 p = -1.0 + 2.0 * vUv;
				float A = time*40.0;
				float d,e,f,g=1.0/40.0,h,i,r,q;

				e=400.0*(p.x*0.5+0.5);
				f=400.0*(p.y*0.5+0.5);
				i=200.0+sin(e*g+A/150.0)*20.0;
				d=200.0+cos(f*g/2.0)*18.0+cos(e*g)*7.0;
				r=sqrt(pow(i-e,2.0)+pow(d-f,2.0));
				q=f/r;
				e=(r*cos(q))-A/2.0;f=(r*sin(q))-A/2.0;
				d=sin(e*g)*176.0+sin(e*g)*164.0+r;
				h=((f+d)+A/2.0)*g;
				i=cos(h+r*p.x/1.3)*(e+e+A)+cos(q*g*6.0)*(r+h/3.0);
				h=sin(f*g)*144.0-sin(e*g)*212.0*p.x;
				h=(h+(f-e)*q+sin(r-(A+h)/7.0)*10.0+i/4.0)*g;
				i+=cos(h*2.3*sin(A/350.0-q))*184.0*sin(q-(r*4.3+A/12.0)*g)+tan(r*g+h)*184.0*cos(r*g+h);
				i=mod(i/5.6,256.0)/64.0;
				if(i<0.0) i+=4.0;
				if(i>=2.0) i=4.0-i;
				d=r/350.0;
				d+=sin(d*d*8.0)*0.52;
				f=(sin(A*g)+1.0)/2.0;
				gl_FragColor=vec4(vec3(f*i/1.6,i/2.0+d/13.0,i)*d*p.x+vec3(i/1.3+d/8.0,i/2.0+d/18.0,i)*d*(1.0-p.x),1.0);

			}";

        private const string FragmentShader2 = @"			
            uniform float time;
			uniform vec2 resolution;

			uniform sampler2D texture;

			varying vec2 vUv;

			void main( void ) {

				vec2 position = -1.0 + 2.0 * vUv;

				float A = atan( position.y, position.x );
				float r = sqrt( dot( position, position ) );

				vec2 uv;
				uv.x = cos( A ) / r;
				uv.y = sin( A ) / r;
				uv /= 10.0;
				uv += time * 0.05;

				vec3 Color = texture2D( texture, uv ).rgb;

				gl_FragColor = vec4( Color * r * 1.5, 1.0 );

			}";

        private const string FragmentShader3 = @"
            uniform float time;
			uniform vec2 resolution;

			varying vec2 vUv;

			void main( void ) {

				vec2 position = vUv;

				float Color = 0.0;
				Color += sin( position.x * cos( time / 15.0 ) * 80.0 ) + cos( position.y * cos( time / 15.0 ) * 10.0 );
				Color += sin( position.y * sin( time / 10.0 ) * 40.0 ) + cos( position.x * sin( time / 25.0 ) * 40.0 );
				Color += sin( position.x * sin( time / 5.0 ) * 10.0 ) + sin( position.y * sin( time / 35.0 ) * 80.0 );
				Color *= sin( time / 10.0 ) * 0.5;

				gl_FragColor = vec4( vec3( Color, Color * 0.5, sin( Color + time / 3.0 ) * 0.75 ), 1.0 );

			}";

        private const string FragmentShader4 = @"
            uniform float time;
			uniform vec2 resolution;

			varying vec2 vUv;

			void main( void ) {

				vec2 position = -1.0 + 2.0 * vUv;

				float red = abs( sin( position.x * position.y + time / 5.0 ) );
				float green = abs( sin( position.x * position.y + time / 4.0 ) );
				float blue = abs( sin( position.x * position.y + time / 3.0 ) );
				gl_FragColor = vec4( red, green, blue, 1.0 );

			}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(40, control.Width / (float)control.Height, 1, 3000);
            this.camera.Position.Z = 4;

            scene = new Scene();

            var geometry = new BoxGeometry(0.75f, 0.75f, 0.75f);

            uniforms1 = new Uniforms
            {
                { "time",       new Uniform() { {"type", "f"},  {"value", 1.0f} }},
                { "resolution", new Uniform() { {"type", "v2"}, {"value", new Vector2()} }}
            };

            uniforms2 = new Uniforms
            {
                { "time",       new Uniform() { {"type", "f"},  {"value", 1.0f}} },
                { "resolution", new Uniform() { {"type", "v2"},  {"value", new Vector2()}} },
                { "texture",    new Uniform() { {"type", "t"},  {"value", ImageUtils.LoadTexture(@"examples\textures/disturb.jpg")} }},
            };

            var texture = uniforms2["texture"]["value"] as Texture;
            texture.WrapS = texture.WrapT = Three.RepeatWrapping;

            var p = new Dictionary<string, Uniforms>
            {
                 { "fragment_shader1", this.uniforms1},
                 { "fragment_shader2", this.uniforms2},
                 { "fragment_shader3", this.uniforms1},
                 { "fragment_shader4", this.uniforms1},
            };

            var fragmentShaders = new List<string> { FragmentShader1, FragmentShader2, FragmentShader3, FragmentShader4 };

            int i = 0;
            foreach (var uniforms in p)
            {
                var material = new ShaderMaterial()
                    {
                        Uniforms = uniforms.Value,
                        VertexShader = VertexShader,
                        FragmentShader = fragmentShaders[i],
                    };

                var mesh = new Mesh(geometry, material);
                mesh.Position.X = i - (p.Count - 1) / 2;
                mesh.Position.Y = i % 2 - 0.5f;

                scene.Add(mesh);
                i++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        public override void Resize(Size clientSize)
        {
            Debug.Assert(null != this.camera);
            Debug.Assert(null != this.renderer);

            uniforms1["resolution"]["value"] = new Vector2(clientSize.Width, clientSize.Height);
            uniforms2["resolution"]["value"] = new Vector2(clientSize.Width, clientSize.Height);

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

            var elapsedTime = stopWatch.ElapsedMilliseconds / 1000.0f;

            var delta = 0.018f;

            var ut1 = (float)uniforms1["time"]["value"];
            uniforms1["time"]["value"] = ut1 + (delta * 5);
            uniforms2["time"]["value"] = elapsedTime;

			for ( var i = 0; i < scene.Children.Count; i ++ )
            {
				var object3D = scene.Children[i];

                object3D.Rotation.Y += delta * 0.5f * ((i % 2 == 0) ? 1 : -1);
                object3D.Rotation.X += delta * 0.5f * ((i % 2 == 0) ? -1 : 1);
            }

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
