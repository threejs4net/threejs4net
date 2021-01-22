using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_interactive_cubes_ortho", ExampleCategory.OpenTK, "Interactive")]
    class webgl_interactive_cubes_ortho : Example
    {
        private OrthographicCamera camera;

        private Projector projector;

        private Scene scene;

        private Mesh mesh;

        private Line line;

        private Raycaster raycaster;

        private Vector2 mouse = new Vector2();

        private float radius = 100;

        private float theta = 0;

        private Object3D INTERSECTED;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new OrthographicCamera(control.Width / -2, control.Width / 2, control.Height / 2, control.Height / -2, -500, 1000);
            
            scene = new Scene();
 
            var light = new DirectionalLight( Color.White, 1 );
            light.Position = new Vector3( 1, 1, 1 ).Normalize();
            scene.Add(light);

			var geometry = new BoxGeometry( 20, 20, 20 );

			for ( var i = 0; i < 2000; i ++ )
			{
			    var object3D = new Mesh(geometry, new MeshLambertMaterial() { Color = new Color().Random() });

				object3D.Position.X = Mat.Random() * 800 - 400;
				object3D.Position.Y = Mat.Random() * 800 - 400;
				object3D.Position.Z = Mat.Random() * 800 - 400;

				object3D.Rotation.X = Mat.Random() * 2 * (float)System.Math.PI;
				object3D.Rotation.Y = Mat.Random() * 2 * (float)System.Math.PI;
				object3D.Rotation.Z = Mat.Random() * 2 * (float)System.Math.PI;

				object3D.Scale.X = Mat.Random() + 0.5f;
				object3D.Scale.Y = Mat.Random() + 0.5f;
				object3D.Scale.Z = Mat.Random() + 0.5f;

			    scene.Add(object3D);
			}

            raycaster = new Raycaster();

            renderer.SetClearColor((Color)colorConvertor.ConvertFromString("#f0f0f0"));
            renderer.SortObjects = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        public override void Resize(Size clientSize)
        {
            Debug.Assert(null != this.camera);
            Debug.Assert(null != this.renderer);

            camera.Left = clientSize.Width / -2;
            camera.Right = clientSize.Width / 2;
            camera.Top = clientSize.Height / 2;
            camera.Bottom = clientSize.Height / -2;

            camera.UpdateProjectionMatrix();

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

        private Color currentHex;

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
			theta += 0.1f;

			camera.Position.X = radius * (float)System.Math.Sin( Mat.DegToRad( theta ) );
			camera.Position.Y = radius * (float)System.Math.Sin( Mat.DegToRad( theta ) );
			camera.Position.Z = radius * (float)System.Math.Cos( Mat.DegToRad( theta ) );
            camera.LookAt(scene.Position);

			// find intersections
            var vector = new Vector3(mouse.X, mouse.Y, - 1).Unproject(camera);
            var direction = new Vector3(0, 0, -1).TransformDirection(camera.MatrixWorld);

            raycaster = new Raycaster(vector, direction);

            var intersects = raycaster.IntersectObjects(scene.Children);

            if ( intersects.Count > 0 ) {

                if ( INTERSECTED != intersects[ 0 ].Object3D )
                {
                    if (INTERSECTED != null)
                        ((MeshLambertMaterial)INTERSECTED.Material).Emissive = currentHex;

                    INTERSECTED = intersects[0].Object3D;
                    currentHex = ((MeshLambertMaterial)INTERSECTED.Material).Emissive;
                    ((MeshLambertMaterial)INTERSECTED.Material).Emissive = Color.Red;
                }

            } else {
                if (INTERSECTED != null)
                    ((MeshLambertMaterial)INTERSECTED.Material).Emissive = currentHex;
                INTERSECTED = null;
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
