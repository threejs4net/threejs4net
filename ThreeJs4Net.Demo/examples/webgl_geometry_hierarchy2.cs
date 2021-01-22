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

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_geometry_hierarchy2", ExampleCategory.OpenTK, "Geometry")]
    class webgl_geometry_hierarchy2 : Example
    {
        private PerspectiveCamera camera;

        private Object3D group;

        private Object3D root;

        private Scene scene;

        private readonly Vector2 mouse = new Vector2();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(60, control.Width / (float)control.Height, 1, 15000);
            this.camera.Position.Z = 500;

            scene = new Scene();

            var geometry = new BoxGeometry(100, 100, 100);
            var material = new MeshNormalMaterial();

            root = new Mesh(geometry, material);
            root.Position.X = 1000;
            scene.Add(root);

            var amount = 200; Object3D object3D; var parent = root;

            for ( var i = 0; i < amount; i ++ ) 
            {
				object3D = new Mesh( geometry, material );
				object3D.Position.X = 100;

				parent.Add( object3D );
                parent = object3D;
			}

            parent = root;

			for ( var i = 0; i < amount; i ++ ) 
            {
				object3D = new Mesh( geometry, material );
				object3D.Position.X = - 100;

				parent.Add( object3D );
                parent = object3D;
			}

			parent = root;

			for ( var i = 0; i < amount; i ++ ) 
            {
				object3D = new Mesh( geometry, material );
				object3D.Position.Y = - 100;

				parent.Add( object3D );
                parent = object3D;
			}

			parent = root;

            for ( var i = 0; i < amount; i ++ )
            {
				object3D = new Mesh( geometry, material );
				object3D.Position.Y = 100;

				parent.Add( object3D );
                parent = object3D;
			}

			parent = root;

			for ( var i = 0; i < amount; i ++ )
            {
				object3D = new Mesh( geometry, material );
				object3D.Position.Z = - 100;

				parent.Add( object3D );
                parent = object3D;
			}

			parent = root;

            for ( var i = 0; i < amount; i ++ )
            {
                object3D = new Mesh( geometry, material );
                object3D.Position.Z = 100;

                parent.Add(object3D);
                parent = object3D;
            }

            renderer.SetClearColor(Color.White);
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
            this.mouse.X = (here.X - ((float)clientSize.Width / 2)) * 10;
            this.mouse.Y = (here.Y - ((float)clientSize.Height / 2)) * 10;
        }

        private float rx, ry, rz;

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            Debug.Assert(null != this.renderer);

            var time = stopWatch.ElapsedMilliseconds * 0.001f;

            rx = (float)(System.Math.Sin(time * 0.7) * 0.2);
            ry = (float)(System.Math.Sin(time * 0.3) * 0.1);
            rz = (float)(System.Math.Sin(time * 0.2) * 0.1);

            camera.Position.X += (this.mouse.X - camera.Position.X) * .05f;
            camera.Position.Y += (-this.mouse.Y - camera.Position.Y) * .05f;

            camera.LookAt(scene.Position);

            root.Traverse(
                child =>
                    {
                        child.Rotation.X = rx;
                        child.Rotation.Y = ry;
                        child.Rotation.Z = rz;
                    });

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
