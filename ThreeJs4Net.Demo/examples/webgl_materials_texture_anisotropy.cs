using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_materials_texture_anisotropy", ExampleCategory.OpenTK, "materials")]
    class webgl_materials_texture_anisotropy : Example
    {
        private PerspectiveCamera camera;

        private Scene scene1, scene2;

        private readonly Vector2 mouse = new Vector2();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(35, control.Width / (float)control.Height, 1, 25000);
            camera.Position.Z = 1500;

            scene1 = new Scene();
            scene2 = new Scene();

            scene1.Fog = new Fog((Color)colorConvertor.ConvertFromString("#f2f7ff"), 1, 25000);
            scene2.Fog = scene1.Fog;

            scene1.Add(new AmbientLight((Color)colorConvertor.ConvertFromString("#eef0ff")));
            scene2.Add(new AmbientLight((Color)colorConvertor.ConvertFromString("#eef0ff")));

            var light1 = new DirectionalLight(Color.White, 2);
            light1.Position = new Vector3(1, 1, 1);
            scene1.Add(light1);

            var light2 = new DirectionalLight(Color.White, 2);
            light2.Position = new Vector3(1, 1, 1);
            scene2.Add(light2);

            // GROUND

            var maxAnisotropy = renderer.MaxAnisotropy;

            var texture1 = ImageUtils.LoadTexture("examples/textures/crate.jpg");
            var material1 = new MeshPhongMaterial() { Color = Color.White, Map = texture1 };

            texture1.Anisotropy = maxAnisotropy;
            texture1.WrapS = texture1.WrapT = Three.RepeatWrapping;
            texture1.Repeat = new Vector2(512, 512);

            var texture2 = ImageUtils.LoadTexture("examples/textures/crate.jpg");
            var material2 = new MeshPhongMaterial() { Color = Color.White, Map = texture2 };

            texture2.Anisotropy = 1;
            texture2.WrapS = texture2.WrapT = Three.RepeatWrapping;
            texture2.Repeat = new Vector2(512, 512);

            //if (maxAnisotropy > 0)
            //{
            //    document.getElementById("val_left").innerHTML = texture1.anisotropy;
            //    document.getElementById("val_right").innerHTML = texture2.anisotropy;
            //}
            //else
            //{
            //    document.getElementById("val_left").innerHTML = "not supported";
            //    document.getElementById("val_right").innerHTML = "not supported";
            //}

            //

            var geometry = new PlaneGeometry(100, 100);

            var mesh1 = new Mesh(geometry, material1);
            mesh1.Rotation.X = (float) -System.Math.PI / 2;
            mesh1.Scale = new Vector3(1000, 1000, 1000);

            var mesh2 = new Mesh(geometry, material2);
            mesh2.Rotation.X = (float)-System.Math.PI / 2;
            mesh2.Scale = new Vector3(1000, 1000, 1000);

            scene1.Add(mesh1);
            scene2.Add(mesh2);
    
			// RENDERER

            renderer.SetClearColor(scene1.Fog.Color, 1);
            renderer.AutoClear = false;
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

            SCREEN_WIDTH = clientSize.Width;
            SCREEN_HEIGHT = clientSize.Height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
            this.mouse.X = (here.X - ((float)clientSize.Width / 2));
            this.mouse.Y = (here.Y - ((float)clientSize.Height / 2));
        }

        private int SCREEN_WIDTH;

        private int SCREEN_HEIGHT;

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            camera.Position.X += (mouse.X - camera.Position.X) * .05f;
            camera.Position.Y = Mat.Clamp(camera.Position.Y + (-(mouse.Y - 200) - camera.Position.Y) * .05f, 50, 1000);

            camera.LookAt(scene1.Position);

            renderer.EnableScissorTest(false);
            renderer.Clear();
            renderer.EnableScissorTest(true);

            renderer.SetScissor(0, 0, SCREEN_WIDTH / 2 - 2, SCREEN_HEIGHT);
            renderer.Render(scene1, camera);

            renderer.SetScissor(SCREEN_WIDTH / 2, 0, SCREEN_WIDTH / 2 - 2, SCREEN_HEIGHT);
            renderer.Render(scene2, camera);
        }
    }
}
