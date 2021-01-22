using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_buffergeometry_lines", ExampleCategory.OpenTK, "BufferGeometry")]
    class webgl_buffergeometry_lines : Example
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

            //

            camera = new PerspectiveCamera(27, control.Width / (float)control.Height, 1, 4000);
            this.camera.Position.Z = 2750;

            scene = new Scene();


            const int segments = 10000;

            var geometry = new BufferGeometry();
            var material = new LineBasicMaterial { VertexColors = Three.VertexColors };

            var positions = new float[segments * 3];
            var colors = new float[segments * 3];

            const float r = 800.0f;

            for (var i = 0; i < segments; i++)
            {

                var x = Mat.Random() * r - r / 2;
                var y = Mat.Random() * r - r / 2;
                var z = Mat.Random() * r - r / 2;

                // positions

                positions[i * 3 + 0] = x;
                positions[i * 3 + 1] = y;
                positions[i * 3 + 2] = z;

                // colors

                colors[i * 3 + 0] = (x / r) + 0.5f;
                colors[i * 3 + 1] = (y / r) + 0.5f;
                colors[i * 3 + 2] = (z / r) + 0.5f;
            }

            geometry.AddAttribute("position", new BufferAttribute<float>(positions, 3));
            geometry.AddAttribute("Color", new BufferAttribute<float>(colors, 3));

            geometry.ComputeBoundingSphere();

            mesh = new Line(geometry, material);
            scene.Add(mesh);

            renderer.gammaInput = true;
            renderer.gammaOutput = true;
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
            var time = stopWatch.ElapsedMilliseconds;

            var ftime = time * 0.001f;

            this.mesh.Rotation.X = ftime * 0.25f;
            this.mesh.Rotation.Y = ftime * 0.5f;

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
