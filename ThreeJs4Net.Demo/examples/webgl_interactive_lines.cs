using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;
using Vector2 = ThreeJs4Net.Math.Vector2;
using Vector3 = ThreeJs4Net.Math.Vector3;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_interactive_lines", ExampleCategory.OpenTK, "Interactive")]
    class webgl_interactive_lines : Example
    {
        private PerspectiveCamera camera;

        private Object3D currentIntersected;

        private Scene scene;

        private Mesh sphereInter;

        private Object3D parentTransform;

        private Raycaster raycaster;

        private Vector2 mouse = new Vector2(0,0);

        private float theta;

        private const float radius = 100;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(70, control.Width / (float)control.Height, 1, 10000);

            scene = new Scene();

            var sphereGeometry = new SphereGeometry(5);
            var material = new MeshBasicMaterial() { Color = Color.Red };

            sphereInter = new Mesh(sphereGeometry, material);
            sphereInter.Visible = false;
            scene.Add(sphereInter);
            
            var geometry = new Geometry();

            var point = new Vector3();
            var direction = new Vector3();

            for (var i = 0; i < 100; i++)
            {
                direction.X += Mat.Random() - 0.5f;
                direction.Y += Mat.Random() - 0.5f;
                direction.Z += Mat.Random() - 0.5f;
                direction.Normalize().MultiplyScalar(5);

                point.Add(direction);

                geometry.Vertices.Add((Vector3)point.Clone());
            }

            parentTransform = new Object3D();
            parentTransform.Position.X = Mat.Random() * 40 - 20;
            parentTransform.Position.Y = Mat.Random() * 40 - 20;
            parentTransform.Position.Z = Mat.Random() * 40 - 20;

            parentTransform.Rotation.X = Mat.Random() * 2 * (float)System.Math.PI;
            parentTransform.Rotation.Y = Mat.Random() * 2 * (float)System.Math.PI;
            parentTransform.Rotation.Z = Mat.Random() * 2 * (float)System.Math.PI;

            parentTransform.Scale.X = Mat.Random() + 0.5f;
            parentTransform.Scale.Y = Mat.Random() + 0.5f;
            parentTransform.Scale.Z = Mat.Random() + 0.5f;

			for ( var i = 0; i < 50; i ++ ) 
            {
                var type = Mat.Random() > 0.5f ? Three.LineStrip : Three.LinePieces;
                var object3D = new Line(geometry, new LineBasicMaterial() { Color = new Color().Random() }, type);

                object3D.Position.X = Mat.Random() * 400 - 200;
                object3D.Position.Y = Mat.Random() * 400 - 200;
                object3D.Position.Z = Mat.Random() * 400 - 200;

                object3D.Rotation.X = Mat.Random() * 2 * (float)System.Math.PI;
                object3D.Rotation.Y = Mat.Random() * 2 * (float)System.Math.PI;
                object3D.Rotation.Z = Mat.Random() * 2 * (float)System.Math.PI;

                object3D.Scale.X = Mat.Random() + 0.5f;
                object3D.Scale.Y = Mat.Random() + 0.5f;
                object3D.Scale.Z = Mat.Random() + 0.5f;

                parentTransform.Add( object3D );
			}

            scene.Add(parentTransform);

            raycaster = new Raycaster();
			raycaster.LinePrecision = 3;

            renderer.SetClearColor((Color)colorConvertor.ConvertFromString("#f0f0f0"));
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
            theta += 0.1f;

            camera.Position.X = radius * (float)System.Math.Sin(Mat.DegToRad(theta));
            camera.Position.Y = radius * (float)System.Math.Sin(Mat.DegToRad(theta));
            camera.Position.Z = radius * (float)System.Math.Cos(Mat.DegToRad(theta));
            camera.LookAt(scene.Position);

            // find intersections

            var vector = new Vector3(mouse.X, mouse.Y, 1).Unproject(camera);

            raycaster = new Raycaster(camera.Position, vector.Sub(camera.Position).Normalize());

            var intersects = raycaster.IntersectObjects(parentTransform.Children, true);

			if ( intersects.Count > 0 ) {

				if ( currentIntersected != null ) 
                {
                    ((LineBasicMaterial)currentIntersected.Material).Linewidth = 1;
				}

				currentIntersected = intersects[ 0 ].Object3D;
                ((LineBasicMaterial)currentIntersected.Material).Linewidth = 5;

				sphereInter.Visible = true;
			    sphereInter.Position.Copy(intersects[0].Point);

			} else {

				if ( currentIntersected != null )
                {
                    ((LineBasicMaterial)currentIntersected.Material).Linewidth = 1;
				}

				currentIntersected = null;

				sphereInter.Visible = false;
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
