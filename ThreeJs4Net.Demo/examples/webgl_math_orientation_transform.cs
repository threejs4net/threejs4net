using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_math_orientation_transform", ExampleCategory.OpenTK, "Math")]
    class webgl_math_orientation_transform : Example
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

            camera = new PerspectiveCamera(60, control.Width / (float)control.Height, 1, 10000);
            this.camera.Position.Z = 100;

            this.scene = new Scene();

            var light = new DirectionalLight(Color.White);
            light.Position.Set(0, 0, 1).Normalize();
            this.scene.Add(light);


            var geometry = new ConeGeometry(30, 40, 64);
            var material = new MeshNormalMaterial();
            this.mesh = new Mesh(geometry, material);

            scene.Add(mesh);




            var geometry2 = new Geometry();
            geometry2.Vertices.Add(new Vector3(0, 0, 0));
            geometry2.Vertices.Add(new Vector3(1, 0, 0));
            geometry2.Vertices.Add(new Vector3(1, 1, 0));
            geometry2.Vertices.Add(new Vector3(0, 1, 0));
            geometry2.Vertices.Add(new Vector3(0, 0, -1));
            geometry2.Vertices.Add(new Vector3(1, 0, -1));
            geometry2.Vertices.Add(new Vector3(1, 1, -1));
            geometry2.Vertices.Add(new Vector3(0, 1, -1));
            geometry2.Faces.Add(new Face3(0, 1, 2));
            geometry2.Faces.Add(new Face3(3, 0, 2));
            geometry2.Faces.Add(new Face3(4, 5, 6));
            geometry2.Faces.Add(new Face3(7, 4, 6));
            geometry2.Faces.Add(new Face3(0, 4, 1));
            geometry2.Faces.Add(new Face3(1, 4, 5));
            geometry2.Faces.Add(new Face3(3, 7, 2));
            geometry2.Faces.Add(new Face3(2, 7, 6));
            geometry2.ComputeVertexNormals();
            geometry2.Normalize();
            geometry2.Scale(10, 10, 10);
            geometry2.RotateX((float)0.2);
            geometry2.RotateY((float)0.2);
            var mesh2 = new Mesh(geometry2, new MeshBasicMaterial() { Side = Three.DoubleSide });
            scene.Add(mesh2);
            mesh2.Position.X += 15;









            //renderer.SetClearColor(scene.Fog.Color);

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
