using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Demo.examples.cs.controls;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Helpers;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("misc_controls_transform", ExampleCategory.Misc, "controls", 0.4f)]
    class misc_controls_transform : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Object3D mesh;

        private Object3D arrow;

        private TransformControls control;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openTKcontrol"></param>
        public override void Load(Control openTKcontrol)
        {
            base.Load(openTKcontrol);

            camera = new PerspectiveCamera(70, openTKcontrol.Width / (float)openTKcontrol.Height, 1, 3000);
            camera.Position = new Vector3(1000, 500, 1000);
            camera.LookAt(new Vector3(0, 200, 0));

            scene = new Scene();
            scene.Add(new GridHelper(500, 100));

            var light = new DirectionalLight(Color.White, 2);
            light.Position = new Vector3(1, 1, 1);
            scene.Add(light);

            var texture = ImageUtils.LoadTexture(@"examples/textures/crate.jpg");
            texture.Anisotropy = this.renderer.MaxAnisotropy;

            var geometry = new BoxGeometry(200, 200, 200);
            var material = new MeshLambertMaterial(null) { Map = texture };


//var arrowGeometry = new Geometry();
            
//var cm = new Mesh(new CylinderGeometry(0, 200, 1000, 12, 1, false));
//cm.Position.Y = 100;
//cm.UpdateMatrix();

//arrowGeometry.Merge((Geometry)cm.Geometry, cm.Matrix);

//var ww = new Mesh(arrowGeometry);


//scene.Add(ww);






            control = new TransformControls(camera, openTKcontrol);
            control.PropertyChanged += control_PropertyChanged;

            mesh = new Mesh(geometry, material);
            scene.Add(mesh);

            control.Attach(mesh);
            scene.Add(control);

            openTKcontrol.KeyDown += (o, args) =>
                {
                    switch (args.KeyValue)
                    {
                        case 81: // Q
                            control.setSpace(control.space == "local" ? "world" : "local");
                            break;
                        case 87: // W
                            control.SetMode("translate");
                            break;
                        case 69: // E
                            control.SetMode("rotate");
                            break;
                        case 82: // R
                            control.SetMode("scale");
                            break;
                        case 187:
                        case 107: // +,=,num+
                            control.setSize(control.size + 0.1f);
                            break;
                        case 189:
                        case 10: // -,_,num-
                            control.setSize(System.Math.Max(control.size - 0.1f, 0.1f));
                            break;
                    }
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void control_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Render();
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
            control.Update();

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
