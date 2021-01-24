using System.Collections.Generic;
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
using ThreeJs4Net.Textures;
using Vector3 = ThreeJs4Net.Math.Vector3;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_custom_attributes_particles3", ExampleCategory.OpenTK, "custom", 0.3f)]
    class webgl_custom_attributes_particles3 : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Object3D object3D;

        private Mesh sphere;

        private Attributes attributes;

        private Uniforms uniforms;

        private IList<float> noise;

        private int vc1;

        private const string VertexShader = @"	
		
            attribute float size;
			attribute vec4 ca;

			varying vec4 vColor;

			void main() {

				vColor = ca;

				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );

				gl_PointSize = size * ( 150.0 / length( mvPosition.xyz ) );

				gl_Position = projectionMatrix * mvPosition;

			}";

        private const string FragmentShader = @"
	
            uniform vec3 color;
			uniform sampler2D texture;

			varying vec4 vColor;

			void main() {

				vec4 outColor = texture2D( texture, gl_PointCoord );

				if ( outColor.a < 0.5 ) discard;

				gl_FragColor = outColor * vec4( color * vColor.xyz, 1.0 );

				float depth = gl_FragCoord.z / gl_FragCoord.w;
				const vec3 fogColor = vec3( 0.0 );

				float fogFactor = smoothstep( 200.0, 600.0, depth );
				gl_FragColor = mix( gl_FragColor, vec4( fogColor, gl_FragColor.w ), fogFactor );

			}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="geo"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="ry"></param>
        void AddGeo(Geometry geometry, Geometry geo, float x, float y, float z, float ry)
		{
		    var mesh = new Mesh(geo);
			mesh.Position = new Vector3( x, y, z );
			mesh.Rotation.Y = ry;
			mesh.UpdateMatrix();

		    geometry.Merge((Geometry)mesh.Geometry, mesh.Matrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(40, control.Width / (float)control.Height, 1, 1000);
            this.camera.Position.Z = 500;

            scene = new Scene();

            attributes = new Attributes
            {
                { "size", new Attribute() { { "type", "f" }, { "value", new float[0] } } },
                { "ca",   new Attribute() { { "type", "c" }, { "value", new Color[0] } } },
            };

            uniforms = new Uniforms
            {
                { "amplitude", new Uniform() { {"type", "f"},  {"value", 1.0f}} },
                { "color",     new Uniform() { {"type", "c"},  {"value", Color.White}} },
                { "texture",   new Uniform() { {"type", "t"},  {"value", ImageUtils.LoadTexture(@"examples\textures/sprites/ball.png")} }},
            };

            var texture = uniforms["texture"]["value"] as Texture;
            texture.WrapS = texture.WrapT = Three.RepeatWrapping;

            var shaderMaterial = new ShaderMaterial
            {
                Uniforms = uniforms,
                Attributes = attributes,
                VertexShader = VertexShader,
                FragmentShader = FragmentShader,
            };

            float radius = 100.0f; float inner = 0.6f * radius;
            var geometry = new Geometry();

            for (var i = 0; i < 100000; i++)
            {
                var vertex = new Vector3();
                vertex.X = Mat.Random() * 2 - 1;
                vertex.Y = Mat.Random() * 2 - 1;
                vertex.Z = Mat.Random() * 2 - 1;
                vertex.MultiplyScalar(radius);

                if ((vertex.X > inner || vertex.X < -inner)
                || (vertex.Y > inner || vertex.Y < -inner)
                || (vertex.Z > inner || vertex.Z < -inner))
                    geometry.Vertices.Add(vertex);
            }

            vc1 = geometry.Vertices.Count;

            radius = 200;
            var geometry2 = new BoxGeometry(radius, 0.1f * radius, 0.1f * radius, 50, 5, 5);

            // side 1

            this.AddGeo(geometry, geometry2, 0, 110, 110, 0);
            this.AddGeo(geometry, geometry2, 0, 110, -110, 0);
            this.AddGeo(geometry, geometry2, 0, -110, 110, 0);
            this.AddGeo(geometry, geometry2, 0, -110, -110, 0);

            // side 2

            this.AddGeo(geometry, geometry2, 110, 110, 0, (float)Mat.HalfPI);
            this.AddGeo(geometry, geometry2, 110, -110, 0, (float)Mat.HalfPI);
            this.AddGeo(geometry, geometry2, -110, 110, 0, (float)Mat.HalfPI);
            this.AddGeo(geometry, geometry2, -110, -110, 0, (float)Mat.HalfPI);

            // corner edges

			var geometry3 = new BoxGeometry( 0.1f * radius, radius * 1.2f, 0.1f * radius, 5, 60, 5 );

            this.AddGeo(geometry, geometry3, 110, 0, 110, 0);
            this.AddGeo(geometry, geometry3, 110, 0, -110, 0);
            this.AddGeo(geometry, geometry3, -110, 0, 110, 0);
            this.AddGeo(geometry, geometry3, -110, 0, -110, 0);

			// particle system

			object3D = new PointCloud( geometry, shaderMaterial );

			// custom attributes

            var vertices = geometry.Vertices;

            var values_size = new float[vertices.Count];
            var values_color = new Color[vertices.Count];

			for ( var v = 0; v < vertices.Count; v ++ )
			{
			    values_size[v] = 10.0f;
			    values_color[v] = Color.White;

				if ( v < vc1 )
				{
				    //values_color[v].setHSL(0.5 + 0.2 * (v / (float)vc1), 1, 0.5);
                    values_color[v] = Color.Red;
				
                } 
                else
				{
				    values_size[v] = 55.0f;
				    //values_color[v].setHSL(0.1, 1, 0.5);
				    values_color[v] = Color.DeepSkyBlue;
				}
			}

            attributes["size"]["value"] = values_size;
            attributes["ca"]["value"] = values_color;

			//console.log( vertices.length );

            scene.Add(object3D);
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

            var time = stopWatch.ElapsedMilliseconds * 0.01f;

            object3D.Rotation.Y = object3D.Rotation.Z = 0.02f * time;

            for (var i = 0; i < ((float[])attributes["size"]["value"]).Length; i++)
            {
                if (i < vc1)
                    ((float[])attributes["size"]["value"])[i] = (float)System.Math.Max(0, 26 + 32 * System.Math.Sin(0.1 * i + 0.6 * time));
            }

            attributes["size"]["needsUpdate"] = true;

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
