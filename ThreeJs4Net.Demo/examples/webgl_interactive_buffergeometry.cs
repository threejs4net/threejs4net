using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_interactive_buffergeometry", ExampleCategory.OpenTK, "Interactive")]
    class webgl_interactive_buffergeometry : Example
    {
        private PerspectiveCamera camera;

        private Projector projector;

        private Scene scene;

        private Mesh mesh;

        private Line line;

        private Raycaster raycaster;

        private Vector2 mouse;

        private float getRandom()
        {
            return Mat.Random();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(27, control.Width / (float)control.Height, 1, 3500);
            camera.Position.Z = 2750;

            scene = new Scene();
            scene.Fog = new Fog((Color)colorConvertor.ConvertFromString("#050505"), 2000, 3500);
 
            scene.Add( new AmbientLight( (Color)colorConvertor.ConvertFromString("#444444") ) );

            var light1 = new DirectionalLight( Color.White, 0.5f );
            light1.Position = new Vector3( 1, 1, 1 );
            scene.Add( light1 );

            var light2 = new DirectionalLight( Color.White, 1.5f );
            light2.Position = new Vector3( 0, -1, 0 );
            scene.Add( light2 );

			//

            const int Triangles = 5000;

			var geometry = new BufferGeometry();

            var positions = new float[Triangles * 3 * 3];
            var normals = new float[Triangles * 3 * 3];
            var colors = new float[Triangles * 3 * 3];

			var color = new Color();

            const int n = 800; const int n2 = n / 2; // triangles spread in the cube
			const int d = 120; const int d2 = d / 2; // individual triangle size

			for ( var i = 0; i < positions.Length; i += 9 ) {

				// positions

			    var r = 0.5f;

				var x = getRandom() * n - n2;
				var y = getRandom() * n - n2;
				var z = getRandom() * n - n2;

				var ax = x + getRandom() * d - d2;
				var ay = y + getRandom() * d - d2;
				var az = z + getRandom() * d - d2;

				var bx = x + getRandom() * d - d2;
				var by = y + getRandom() * d - d2;
				var bz = z + getRandom() * d - d2;

				var cx = x + getRandom() * d - d2;
				var cy = y + getRandom() * d - d2;
				var cz = z + getRandom() * d - d2;

				positions[ i ]     = ax;
				positions[ i + 1 ] = ay;
				positions[ i + 2 ] = az;

				positions[ i + 3 ] = bx;
				positions[ i + 4 ] = by;
				positions[ i + 5 ] = bz;

				positions[ i + 6 ] = cx;
				positions[ i + 7 ] = cy;
				positions[ i + 8 ] = cz;

				// flat face normals

                var pA = new Vector3(ax, ay, az);
                var pB = new Vector3(bx, by, bz);
                var pC = new Vector3(cx, cy, cz);

                var cb = pC - pB;
                var ab = pA - pB;
                cb.Cross(ab).Normalize();

				var nx = cb.X;
				var ny = cb.Y;
				var nz = cb.Z;

				normals[ i ]     = nx;
				normals[ i + 1 ] = ny;
				normals[ i + 2 ] = nz;

				normals[ i + 3 ] = nx;
				normals[ i + 4 ] = ny;
				normals[ i + 5 ] = nz;

				normals[ i + 6 ] = nx;
				normals[ i + 7 ] = ny;
				normals[ i + 8 ] = nz;

				// colors

				var vx = ( x / n ) + 0.5;
				var vy = ( y / n ) + 0.5;
				var vz = ( z / n ) + 0.5;

                color = Color.FromArgb(255, (int)(vx * 255), (int)(vy * 255), (int)(vz * 255));

                colors[i] = color.R / 255.0f;
                colors[i + 1] = color.G / 255.0f;
                colors[i + 2] = color.B / 255.0f;

                colors[i + 3] = color.R / 255.0f;
                colors[i + 4] = color.G / 255.0f;
                colors[i + 5] = color.B / 255.0f;

                colors[i + 6] = color.R / 255.0f;
                colors[i + 7] = color.G / 255.0f;
                colors[i + 8] = color.B / 255.0f;

			}

			geometry.AddAttribute( "position", new BufferAttribute<float>( positions, 3 ) );
			geometry.AddAttribute( "normal",   new BufferAttribute<float>( normals, 3 ) );
			geometry.AddAttribute( "color",    new BufferAttribute<float>( colors, 3 ) );

			geometry.ComputeBoundingSphere();

			var material = new MeshPhongMaterial()
            {
				Color = (Color)colorConvertor.ConvertFromString("#aaaaaa"), 
                Ambient = (Color)colorConvertor.ConvertFromString("#aaaaaa"), 
                Specular = Color.White, 
                Shininess = 250,
				Side = Three.DoubleSide, VertexColors = Three.VertexColors,
			};

			mesh = new Mesh( geometry, material );
			scene.Add( mesh );

			//

            projector = new Projector();
            raycaster = new Raycaster();

            mouse = new Vector2();

			var geometry2 = new BufferGeometry();
            geometry2.AddAttribute("position", new BufferAttribute<float>(new float[4 * 3], 3));

            var material2 = new LineBasicMaterial() { Color = Color.White, Linewidth = 2, Transparent = true };

            line = new Line(geometry2, material2);
            scene.Add(line);

            renderer.SetClearColor(scene.Fog.Color);
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
 			var time = stopWatch.ElapsedMilliseconds * 0.001f;

			mesh.Rotation.X = time * 0.15f;
			mesh.Rotation.Y = time * 0.25f;

            var vector = new Vector3(mouse.X, mouse.Y, 1);
            projector.UnprojectVector(vector, camera);

            raycaster = new Raycaster(camera.Position, (vector - camera.Position).Normalize());

            var intersects = raycaster.IntersectObject(mesh);
            if (intersects.Count > 0)
            {
                var intersect = intersects[0];

                var object3D = intersect.Object3D;
                var bg = object3D.Geometry as BufferGeometry;
                var positions = ((BufferAttribute<float>)bg.Attributes["position"]).Array;

                var bg2 = line.Geometry as BufferGeometry;
                var array = ((BufferAttribute<float>)bg2.Attributes["position"]).Array;

                for (int i = 0, j = 0; i < 4; i++, j+=3)
                {
                    var index = intersect.Indices[i % 3] * 3;

                    array[j + 0] = positions[index];
                    array[j + 1] = positions[index + 1];
                    array[j + 2] = positions[index + 2];
                }

                mesh.UpdateMatrix();

                line.Geometry.ApplyMatrix4(mesh.Matrix);

                line.Visible = true;
            }
            else
            {                
                line.Visible = false;
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
