using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_shader", ExampleCategory.OpenTK, "Shader")]
    class webgl_shader : Example
    {
        private Camera camera;

        private Scene scene;

        private Object3D mesh;

        private Uniforms uniforms;

        private const string VertexShader = @"
            void main()	{

				gl_Position = vec4( position, 1.0 );

			}";

        private const string FragmentShader = @"
            uniform vec2 resolution;
			uniform float time;

			void main()	{

				vec2 p = -1.0 + 2.0 * gl_FragCoord.xy / resolution.xy;
				float a = time*40.0;
				float d,e,f,g=1.0/40.0,h,i,r,q;
				e=400.0*(p.x*0.5+0.5);
				f=400.0*(p.y*0.5+0.5);
				i=200.0+sin(e*g+a/150.0)*20.0;
				d=200.0+cos(f*g/2.0)*18.0+cos(e*g)*7.0;
				r=sqrt(pow(i-e,2.0)+pow(d-f,2.0));
				q=f/r;
				e=(r*cos(q))-a/2.0;f=(r*sin(q))-a/2.0;
				d=sin(e*g)*176.0+sin(e*g)*164.0+r;
				h=((f+d)+a/2.0)*g;
				i=cos(h+r*p.x/1.3)*(e+e+a)+cos(q*g*6.0)*(r+h/3.0);
				h=sin(f*g)*144.0-sin(e*g)*212.0*p.x;
				h=(h+(f-e)*q+sin(r-(a+h)/7.0)*10.0+i/4.0)*g;
				i+=cos(h*2.3*sin(a/350.0-q))*184.0*sin(q-(r*4.3+a/12.0)*g)+tan(r*g+h)*184.0*cos(r*g+h);
				i=mod(i/5.6,256.0)/64.0;
				if(i<0.0) i+=4.0;
				if(i>=2.0) i=4.0-i;
				d=r/350.0;
				d+=sin(d*d*8.0)*0.52;
				f=(sin(a*g)+1.0)/2.0;
				gl_FragColor=vec4(vec3(f*i/1.6,i/2.0+d/13.0,i)*d*p.x+vec3(i/1.3+d/8.0,i/2.0+d/18.0,i)*d*(1.0-p.x),1.0);

			}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new Camera();
            this.camera.Position.Z = 1;

            scene = new Scene();

            var geometry = new PlaneGeometry(2, 2);

            uniforms = new Uniforms
            {
                { "time",       new Uniform() { {"type", "f"},  {"value", 1.0f}} },
                { "resolution", new Uniform() { {"type", "v2"}, {"value", new Vector2()}} }
            };

            var material = new ShaderMaterial(null)
            {
                Uniforms = uniforms,
                VertexShader = VertexShader,
                FragmentShader = FragmentShader,
            };

            mesh = new Mesh(geometry, material);
            scene.Add(mesh);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        public override void Resize(Size clientSize)
        {
            Debug.Assert(null != this.camera);
            Debug.Assert(null != this.renderer);

            if (this.camera is PerspectiveCamera)
            {
                var perspectiveCamera = this.camera as PerspectiveCamera;

                perspectiveCamera.Aspect = clientSize.Width / (float)clientSize.Height;
                perspectiveCamera.UpdateProjectionMatrix();
            }

            uniforms["resolution"]["value"] = new Vector2(clientSize.Width, clientSize.Height);

            this.renderer.Size = clientSize;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            var ut = (float)uniforms["time"]["value"];
            uniforms["time"]["value"] = ut + 0.05f;

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
