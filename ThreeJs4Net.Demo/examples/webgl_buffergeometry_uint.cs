using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_buffergeometry_uint", ExampleCategory.OpenTK, "BufferGeometry")]
    class webgl_buffergeometry_uint : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Mesh mesh;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(27, control.Width / (float)control.Height, 1, 3500);
            this.camera.Position.Z = 2750;

            scene = new Scene();
            scene.Fog = new Fog((Color)colorConvertor.ConvertFromString("#050505"), 2000, 3500);

            scene.Add(new AmbientLight((Color)colorConvertor.ConvertFromString("#444444")));

            var light1 = new DirectionalLight(Color.White, 0.5f);
            light1.Position = new Vector3(1, 1, 1);
            scene.Add(light1);

            var light2 = new DirectionalLight(Color.White, 1.5f);
            light2.Position = new Vector3(0, -1, 0);
            scene.Add(light2);

            const int triangles = 160000;

            var geometry = new BufferGeometry();

            var indices = new uint[triangles * 3];

            for (uint i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            var positions = new float[triangles * 3 * 3];
            var normals = new float[triangles * 3 * 3];
            var colors = new float[triangles * 3 * 3];

            var color = Color.White;

            const int n = 800; var n2 = n / 2;	// triangles spread in the cube
            const int d = 12; var d2 = d / 2;	// individual triangle size

            for (var i = 0; i < positions.Length; i += 9)
            {

                // positions

                var x = Mat.Random() * n - n2;
                var y = Mat.Random() * n - n2;
                var z = Mat.Random() * n - n2;

                var ax = x + Mat.Random() * d - d2;
                var ay = y + Mat.Random() * d - d2;
                var az = z + Mat.Random() * d - d2;

                var bx = x + Mat.Random() * d - d2;
                var by = y + Mat.Random() * d - d2;
                var bz = z + Mat.Random() * d - d2;

                var cx = x + Mat.Random() * d - d2;
                var cy = y + Mat.Random() * d - d2;
                var cz = z + Mat.Random() * d - d2;

                positions[i + 0] = ax;
                positions[i + 1] = ay;
                positions[i + 2] = az;

                positions[i + 3] = bx;
                positions[i + 4] = by;
                positions[i + 5] = bz;

                positions[i + 6] = cx;
                positions[i + 7] = cy;
                positions[i + 8] = cz;

                // flat face normals

                var pA = new Vector3(ax, ay, az);
                var pB = new Vector3(bx, by, bz);
                var pC = new Vector3(cx, cy, cz);

                var cb = new Vector3().SubVectors(pC, pB);
                var ab = new Vector3().SubVectors(pA, pB);
                cb.Cross(ab).Normalize();

                var nx = cb.X;
                var ny = cb.Y;
                var nz = cb.Z;

                normals[i + 0] = nx;
                normals[i + 1] = ny;
                normals[i + 2] = nz;

                normals[i + 3] = nx;
                normals[i + 4] = ny;
                normals[i + 5] = nz;

                normals[i + 6] = nx;
                normals[i + 7] = ny;
                normals[i + 8] = nz;

                // colors

                var vx = (x / n) + 0.5;
                var vy = (y / n) + 0.5;
                var vz = (z / n) + 0.5;

                color = Color.FromArgb(255, (int)(vx * 255), (int)(vy * 255), (int)(vz * 255));

                colors[i + 0] = color.R / 255.0f;
                colors[i + 1] = color.G / 255.0f;
                colors[i + 2] = color.B / 255.0f;

                colors[i + 3] = color.R / 255.0f;
                colors[i + 4] = color.G / 255.0f;
                colors[i + 5] = color.B / 255.0f;

                colors[i + 6] = color.R / 255.0f;
                colors[i + 7] = color.G / 255.0f;
                colors[i + 8] = color.B / 255.0f;

            }


            geometry.AddAttribute("index", new BufferAttribute<uint>(indices, 1));
            geometry.AddAttribute("position", new BufferAttribute<float>(positions, 3));
            geometry.AddAttribute("normal", new BufferAttribute<float>(normals, 3));
            geometry.AddAttribute("color", new BufferAttribute<float>(colors, 3));

            geometry.ComputeBoundingSphere();

            var material = new MeshPhongMaterial
            {
                Color = (Color)colorConvertor.ConvertFromString("#aaaaaa"),
                Ambient = (Color)colorConvertor.ConvertFromString("#aaaaaa"),
                Specular = (Color)colorConvertor.ConvertFromString("#ffffff"),
                Shininess = 250,
                Side = Three.DoubleSide,
                VertexColors = Three.VertexColors,
            };

            this.mesh = new Mesh(geometry, material);
            scene.Add(mesh);

            renderer.SetClearColor(scene.Fog.Color);

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
            Debug.Assert(null != this.mesh);
            Debug.Assert(null != this.renderer);

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
