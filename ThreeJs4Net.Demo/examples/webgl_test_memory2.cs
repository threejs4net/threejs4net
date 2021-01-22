using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_test_memory2", ExampleCategory.OpenTK, "test")]
    class webgl_test_memory2 : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Geometry geometry;

        private readonly List<Mesh> meshes = new List<Mesh>();

        private const string FragmentShader = @"
 			void main() {
				if ( mod ( gl_FragCoord.x, 4.0001 ) < 1.0 || mod ( gl_FragCoord.y, 4.0001 ) < 1.0 )
					gl_FragColor = vec4( XXX, 1.0 );
				else
					gl_FragColor = vec4( 1.0 );
			}
       ";

        private const string VertexShader = @"
     			void main() {

				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );
				gl_Position = projectionMatrix * mvPosition;

			}
        ";

        private int N = 100;

        /// <summary>
        /// 
        /// </summary>
        private string GenerateFragmentShader()
        {
            return FragmentShader.Replace("XXX", Mat.Random().ToString(CultureInfo.InvariantCulture) + "," + Mat.Random().ToString(CultureInfo.InvariantCulture) + "," + Mat.Random().ToString(CultureInfo.InvariantCulture) );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(40, control.Width / (float)control.Height, 1, 10000);
            this.camera.Position.Z = 2000;

            scene = new Scene();
 
        	geometry = new SphereGeometry( 15, 64, 32 );

			for ( var i = 0; i < N; i ++ ) {

				var material = new ShaderMaterial() { VertexShader = VertexShader, FragmentShader = this.GenerateFragmentShader() };

				var mesh = new Mesh( geometry, material );

				mesh.Position.X = ( 0.5f - Mat.Random() ) * 1000;
				mesh.Position.Y = ( 0.5f - Mat.Random() ) * 1000;
				mesh.Position.Z = ( 0.5f - Mat.Random() ) * 1000;

				scene.Add( mesh );

				meshes.Add( mesh );
			}

            renderer.SetClearColor(Color.White);
            this.renderer.Size = control.Size;
        }

        public override void Render()
        {
			for ( var i = 0; i < N; i ++ ) {

				var mesh = meshes[ i ];
                mesh.Material = new ShaderMaterial() { VertexShader = VertexShader, FragmentShader = GenerateFragmentShader() };

			}

			renderer.Render( scene, camera );

			Console.WriteLine( "before {0}", renderer.Info.memory.Programs );

			for ( var i = 0; i < N; i ++ ) {

				var mesh = meshes[ i ];
				mesh.Material.Dispose();

			}

			Console.WriteLine( "after {0}", renderer.Info.memory.Programs );
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Unload()
        {
            base.Unload();
        }
    
    }
}
