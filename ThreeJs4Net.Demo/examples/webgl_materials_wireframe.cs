using System.Collections.Generic;
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
    [Example("webgl_materials_wireframe", ExampleCategory.OpenTK, "materials")]
    class webgl_materials_wireframe : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Mesh meshLines;

        private Mesh meshTris;

        private Mesh meshMixed;

        private const string VertexShader = @"

            attribute vec3 center;
			varying vec3 vCenter;

			void main() {

				vCenter = center;
				gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

			}";

        private const string FragmentShader = @"

			#extension GL_OES_standard_derivatives : enable

			varying vec3 vCenter;

			float edgeFactorTri() {

				vec3 d = fwidth( vCenter.xyz );
				vec3 a3 = smoothstep( vec3( 0.0 ), d * 1.5, vCenter.xyz );
				return min( min( a3.x, a3.y ), a3.z );

			}

			void main() {

				gl_FragColor.rgb = mix( vec3( 1.0 ), vec3( 0.2 ), edgeFactorTri() );
				gl_FragColor.a = 1.0;
			}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(40, control.Width / (float)control.Height, 1, 2000);
            this.camera.Position.Z = 800;

            scene = new Scene();
 
        	const int Size = 150;

			var geometryLines = new BoxGeometry( Size, Size, Size );
			var geometryTris  = new BoxGeometry( Size, Size, Size );

			// wireframe using gl.LINES

			var materialLines = new MeshBasicMaterial() { Wireframe = true };

			meshLines = new Mesh( geometryLines, materialLines );
			meshLines.Position.X = -150;
			scene.Add( meshLines );

			// wireframe using gl.TRIANGLES (interpreted as triangles)

            var attributesTris = new Attributes
            {
                { "center", new Attribute { { "type", "v3" }, { "value", new List<List<Vector3>>() }, { "boundTo", "faceVertices" }, } }
            };
            SetupAttributes(geometryTris, (List<List<Vector3>>)(attributesTris["center"])["value"]);

            var materialTris = new ShaderMaterial() { Attributes = attributesTris, VertexShader = VertexShader, FragmentShader= FragmentShader };
            
            meshTris = new Mesh( geometryTris, materialTris );
            meshTris.Position.X = 150;
            scene.Add( meshTris );

            // wireframe using gl.TRIANGLES (mixed triangles and quads)

            var mixedGeometry = new SphereGeometry( Size / 2, 32, 16 );

            var attributesMixed = new Attributes
            {
                { "center", new Attribute { { "type", "v3" }, { "value", new List<List<Vector3>>() }, { "boundTo", "faceVertices" },  } }
            };
            SetupAttributes( mixedGeometry, (List<List<Vector3>>)attributesMixed["center"]["value"] );

            var materialMixed = new ShaderMaterial() 
            { 
                Attributes = attributesMixed,
                VertexShader = VertexShader, 
                FragmentShader = FragmentShader 
            };

            meshMixed = new Mesh( mixedGeometry, materialMixed );
            meshMixed.Position.X = -150;
            scene.Add( meshMixed );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="values"></param>
        private static void SetupAttributes(Geometry geometry, ICollection<List<Vector3>> values)
        {
            for( var f = 0; f < geometry.Faces.Count; f ++ )
            {
                values.Add(new List<Vector3> { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) });
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

            this.camera.Aspect = clientSize.Width / (float)clientSize.Height;
            this.camera.UpdateProjectionMatrix();

            this.renderer.Size = clientSize;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            if (null != meshLines)
            {
                meshLines.Rotation.X += 0.005f;
                meshLines.Rotation.Y += 0.01f;
            }

            if (null != meshTris)
            {
                meshTris.Rotation.X += 0.005f;
                meshTris.Rotation.Y += 0.01f;
            }

            if (null != meshMixed ) {

				meshMixed.Rotation.X += 0.005f;
				meshMixed.Rotation.Y += 0.01f;
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
