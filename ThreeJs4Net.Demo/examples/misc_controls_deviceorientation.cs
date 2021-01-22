using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Demo.examples.cs.controls;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("misc_controls_deviceorientation", ExampleCategory.Misc, "controls")]
    class misc_controls_deviceorientation : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private DeviceOrientationControls controls;

        private Geometry geometry;

        private Object3D mesh;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(75, control.Width / (float)control.Height, 1, 1100);

            controls = new DeviceOrientationControls(camera);

            scene = new Scene();

            geometry = new SphereGeometry(500, 16, 8);
            geometry.ApplyMatrix4(new Matrix4().MakeScale(-1, 1, 1));

            var material = new MeshBasicMaterial() { Map = ImageUtils.LoadTexture( @"examples\textures/2294472375_24a3b8ef46_o.jpg" ) };

            mesh = new Mesh(geometry, material);
            scene.Add(mesh);

            var geometry2 = new BoxGeometry( 100, 100, 100, 4, 4, 4 );
            var material2 = new MeshBasicMaterial() { Color = Color.Purple, Side = Three.BackSide, Wireframe = true };
            var mesh2 = new Mesh( geometry2, material2 );
            scene.Add(mesh2);

            controls.Connect();
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
            controls.Update();
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
