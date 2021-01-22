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
    [Example("webgl_geometry_hierarchy", ExampleCategory.OpenTK, "Geometry")]
    class webgl_geometry_hierarchy : Example
    {
        private PerspectiveCamera camera;

        private Object3D group;

        private Mesh vb;

        private Scene scene;

        private readonly Vector2 mouse = new Vector2();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(60, control.Width / (float)control.Height, 1, 10000);
            this.camera.Position.Z = 500;

            scene = new Scene();
            scene.Fog = new Fog(Color.White, 1, 10000);

            var geometry = new BoxGeometry(100, 100, 100);
            var material = new MeshNormalMaterial();

            group = new Object3D();

            for (var i = 0; i < 1000; i++)
            {
                var mesh = new Mesh(geometry, material);

                mesh.Position.X = Mat.Random() * 2000 - 1000;
                mesh.Position.Y = Mat.Random() * 2000 - 1000;
                mesh.Position.Z = Mat.Random() * 2000 - 1000;

                mesh.Rotation.X = (float)(Mat.Random() * 2.0 * System.Math.PI);
                mesh.Rotation.Y = (float)(Mat.Random() * 2.0 * System.Math.PI);

                mesh.MatrixAutoUpdate = false;
                mesh.UpdateMatrix();

                group.Add(mesh);
            }

            scene.Add(group);

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

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            Debug.Assert(null != this.renderer);
            Debug.Assert(null != this.group);
            
            var time = stopWatch.ElapsedMilliseconds;
            
            var ftime = time * 0.001f;

            var rx = (float)(System.Math.Sin(ftime * 0.7) * 0.5);
            var ry = (float)(System.Math.Sin(ftime * 0.3) * 0.5);
            var rz = (float)(System.Math.Sin(ftime * 0.2) * 0.5);

            camera.Position.X += ( this.mouse.X - camera.Position.X) * .05f;
            camera.Position.Y += (-this.mouse.Y - camera.Position.Y) * .05f;

            camera.LookAt(scene.Position);

            this.group.Rotation.X = rx;
            this.group.Rotation.Y = ry;
            this.group.Rotation.Z = rz;

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
