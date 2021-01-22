using System.Collections;
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
    [Example("misc_lookat", ExampleCategory.Misc, "controls")]
    class misc_lookat : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Object3D mesh;

        private Object3D sphere;

        private readonly Vector2 mouse = new Vector2();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(40, control.Width / (float)control.Height, 1, 15000);
            this.camera.Position.Z = 3200;

            scene = new Scene();

            sphere = new Mesh(new SphereGeometry(100, 20, 20), new MeshNormalMaterial(new Hashtable { { "shading", Three.SmoothShading } }));
            scene.Add( sphere );

            var geometry = new CylinderGeometry(0, 10, 100, 3);
            geometry.ApplyMatrix4(new Matrix4().MakeRotationFromEuler(new Euler((float)System.Math.PI / 2, (float)System.Math.PI, 0)));

            var material = new MeshNormalMaterial();
                        
            for ( var i = 0; i < 1000; i ++ )
            {
                var mesh = new Mesh(geometry, material);
                mesh.Position.X = Mat.Random() * 4000 - 2000;
                mesh.Position.Y = Mat.Random() * 4000 - 2000;
                mesh.Position.Z = Mat.Random() * 4000 - 2000;

                mesh.Scale.X = mesh.Scale.Y = mesh.Scale.Z = Mat.Random() * 4 + 2;

                scene.Add( mesh );
            }
                        
            scene.MatrixAutoUpdate = false;

            renderer.SetClearColor(Color.White);
            renderer.SortObjects = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
            this.mouse.X = (here.X - ((float)clientSize.Width / 2) ) * 10;
            this.mouse.Y = (here.Y - ((float)clientSize.Height / 2)) * 10;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        public override void Resize(Size clientSize)
        {
            Debug.Assert(null != this.camera);
            Debug.Assert(null != this.renderer);

            if (this.camera != null)
            {
                var perspectiveCamera = this.camera;

                perspectiveCamera.Aspect = clientSize.Width / (float)clientSize.Height;
                perspectiveCamera.UpdateProjectionMatrix();
            }

            this.renderer.Size = clientSize;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            var time = stopWatch.ElapsedMilliseconds * 0.0005;

            sphere.Position.X = (float)System.Math.Sin(time * 0.7f) * 2000.0f;
            sphere.Position.Y = (float)System.Math.Cos(time * 0.5f) * 2000.0f;
            sphere.Position.Z = (float)System.Math.Cos(time * 0.3f) * 2000.0f;

            for (var i = 1; i < scene.Children.Count; i++)
            {
                scene.Children[i].LookAt(sphere.Position);
            }

            camera.Position.X += (this.mouse.X - camera.Position.X) * .05f;
            camera.Position.Y += (-this.mouse.Y - camera.Position.Y) * .05f;
            camera.LookAt(scene.Position);

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
