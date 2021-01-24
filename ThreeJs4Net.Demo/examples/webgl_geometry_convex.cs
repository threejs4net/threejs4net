using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_geometry_convex", ExampleCategory.OpenTK, "Geometry")]
    class webgl_geometry_convex : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Object3D mesh;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            this.camera = new PerspectiveCamera(70, control.Width / (float)control.Height, 1, 1000);
            this.camera.Position.Z = 400;

            this.scene = new Scene();

            var geometry = new BoxGeometry(200, 200, 200);

            //var texture = ImageUtils.LoadTexture(@"examples/textures/crate.gif");
            var texture = ImageUtils.LoadTexture(@"examples/textures/crate.jpg");
            texture.Anisotropy = this.renderer.MaxAnisotropy;

            var material = new MeshBasicMaterial { Map = texture };

            this.mesh = new Mesh(geometry, material);
            this.scene.Add(mesh);
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
            Debug.Assert(null != this.mesh);

            // Cube sample
            this.mesh.Rotation.X += 0.005f;
            this.mesh.Rotation.Y += 0.01f;

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
