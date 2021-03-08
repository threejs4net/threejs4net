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
    [Example("webgl_generic_lines_and_dashes", ExampleCategory.OpenTK, "Interactive")]
    class webgl_generic_lines_and_dashes : Example
    {
        private PerspectiveCamera camera;
        private Projector projector;
        private Scene scene;
        private Mesh mesh;
        private Line line;
        private Raycaster raycaster;
        private Vector2 mouse = new Vector2();
        private float radius = 100;
        private float theta = 0;
        private Object3D INTERSECTED;

        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(75, control.Width / (float)control.Height, (float)0.1, 1000);
            //camera = new OrthographicCamera(control.Width / -2, control.Width / 2, control.Height / 2, control.Height / -2, (float)0.01, 200);
            scene = new Scene();

            camera.Position.Set(0, 0, 0);
            camera.LookAt(scene.Position);

            var light = new PointLight(Color.White);
            light.Position.Set(0, 10, 0);
            scene.Add(light);

            //scene.Fog = new FogExp2(Color.Brown, (float)0.00025);


            var lineGeometry = new BufferGeometry();
            var vertices = new float[] { -5, -1, 0, -5, 1, 0 };
            lineGeometry.SetAttribute("position", new BufferAttribute<float>(vertices, 3));
            var lineMaterial = new LineBasicMaterial()
            {
                Color = Color.Red
            };
            var line = new Line(lineGeometry, lineMaterial);
            line.ComputeLineDistances();

            //var lineGeometry = new Geometry();
            //var vertArray = lineGeometry.Vertices;
            //vertArray.Add(new Vector3(-150, -100, 0));
            //vertArray.Add(new Vector3(-150, 100, 0));
            //var lineMaterial = new LineBasicMaterial()
            //{
            //    Color = Color.Red
            //}; 
            //var line = new Line(lineGeometry, lineMaterial);
            //line.ComputeLineDistances();

            scene.Add(line);


            renderer.SetClearColor((Color) colorConvertor.ConvertFromString("#f0f0f0"));
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

        //camera.Left = clientSize.Width / -2;
        //camera.Right = clientSize.Width / 2;
        //camera.Top = clientSize.Height / 2;
        //camera.Bottom = clientSize.Height / -2;

        camera.UpdateProjectionMatrix();

        this.renderer.Size = clientSize;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientSize"></param>
    /// <param name="here"></param>
    public override void MouseMove(Size clientSize, Point here)
    {
        // Normalize mouse position
        mouse.X = (here.X / (float)clientSize.Width) * 2 - 1;
        mouse.Y = -(here.Y / (float)clientSize.Height) * 2 + 1;
    }

    private Color currentHex;

    /// <summary>
    /// 
    /// </summary>
    public override void Render()
    {
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
