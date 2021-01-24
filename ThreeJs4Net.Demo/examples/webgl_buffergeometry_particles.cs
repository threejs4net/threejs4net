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
    [Example("webgl_buffergeometry_particles", ExampleCategory.OpenTK, "BufferGeometry", 0.6f)]
    class webgl_buffergeometry_particles : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private PointCloud particleSystem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(27, control.Width / (float)control.Height, 5, 3500);
            this.camera.Position.Z = 2750;

            scene = new Scene();
            scene.Fog = new Fog((Color)colorConvertor.ConvertFromString("#050505"), 2000, 3500);

            //

            const int Particles = 500000;

            var positions = new float[Particles * 3];
            var colors    = new float[Particles * 3];

            var n = 1000; var n2 = n / 2; // particles spread in the cube

            for (var i = 0; i < positions.Length; i += 3)
            {
                // positions

                var x = Mat.Random() * n - n2;
                var y = Mat.Random() * n - n2;
                var z = Mat.Random() * n - n2;

                positions[i + 0] = x;
                positions[i + 1] = y;
                positions[i + 2] = z;

                // colors

                var vx = (x / n) + 0.5f;
                var vy = (y / n) + 0.5f;
                var vz = (z / n) + 0.5f;

                colors[i + 0] = vx;
                colors[i + 1] = vy;
                colors[i + 2] = vz;
            }

            var geometry = new BufferGeometry();

            geometry.AddAttribute("position", new BufferAttribute<float>(positions, 3));
            geometry.AddAttribute("color",    new BufferAttribute<float>(colors, 3));

            geometry.ComputeBoundingSphere();

            //

            var material = new PointCloudMaterial() { Size = 15, VertexColors = Three.VertexColors };

            particleSystem = new PointCloud(geometry, material);
            scene.Add(particleSystem);

            //

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
        public override void Render()
        {
            Debug.Assert(null != this.renderer);
            Debug.Assert(null != this.particleSystem);

            var time = stopWatch.ElapsedMilliseconds;

            var ftime = time * 0.001f;

            this.particleSystem.Rotation.X = ftime * 0.25f;
            this.particleSystem.Rotation.Y = ftime * 0.5f;

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
