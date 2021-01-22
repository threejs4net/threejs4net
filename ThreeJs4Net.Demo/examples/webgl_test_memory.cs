using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_test_memory", ExampleCategory.OpenTK, "test")]
    class webgl_test_memory : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(60, control.Width / (float)control.Height, 1, 10000);
            this.camera.Position.Z = 200;

            scene = new Scene();

            renderer.SetClearColor(Color.White);
            this.renderer.Size = control.Size;
        }

        public override void Render()
        {
            var widthSegments = (int)(Mat.Random() * 64);
            if (widthSegments <= 0) widthSegments = 1;
            var heightSegments = (int)(Mat.Random() * 32);
            if (heightSegments <= 0) heightSegments = 1;

            var geometry = new SphereGeometry(50, widthSegments, heightSegments);

            var texture = ImageUtils.LoadTexture(@"examples/textures/crate.jpg");
            texture.NeedsUpdate = true;

            var material = new MeshBasicMaterial() { Map = texture, Wireframe = true };

            var mesh = new Mesh(geometry, material);

            scene.Add(mesh);

            renderer.Render(scene, camera);

            scene.Remove(mesh);

			// clean up

			geometry.Dispose();
			material.Dispose();
			texture.Dispose();
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
