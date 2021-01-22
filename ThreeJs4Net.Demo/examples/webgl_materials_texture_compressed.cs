using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Demo.examples.cs.loaders;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_materials_texture_compressed", ExampleCategory.OpenTK, "materials", 0.4f)]
    class webgl_materials_texture_compressed : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private BoxGeometry geometry;

        private List<Mesh> meshes = new List<Mesh>();

        private readonly Vector2 mouse = new Vector2();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            this.camera = new PerspectiveCamera(50, control.Width / (float)control.Height, 1, 2000);
            this.camera.Position.Z = 1000;

            this.scene = new Scene();

            this.geometry = new BoxGeometry(200, 200, 200);

            /*
            This is how compressed textures are supposed to be used:

            DXT1 - RGB - opaque textures
            DXT3 - RGBA - transparent textures with sharp alpha transitions
            DXT5 - RGBA - transparent textures with full alpha range
            */

            var loader = new DDSLoader();
            loader.Loaded += (o, args) =>
            {
            };

            var map1 = loader.Load("examples/textures/compressed/disturb_dxt1_nomip.dds");
            map1.MinFilter = map1.MagFilter = Three.LinearFilter;
            map1.Anisotropy = 4;

            var map2 = loader.Load("examples/textures/compressed/disturb_dxt1_mip.dds");
            map2.Anisotropy = 4;

            var map3 = loader.Load("examples/textures/compressed/hepatica_dxt3_mip.dds");
			map3.Anisotropy = 4;

            var map4 = loader.Load("examples/textures/compressed/explosion_dxt5_mip.dds");
			map4.Anisotropy = 4;

            var map5 = loader.Load("examples/textures/compressed/disturb_argb_nomip.dds");
			map5.MinFilter = map5.MagFilter = Three.LinearFilter;
			map5.Anisotropy = 4;

            var map6 = loader.Load("examples/textures/compressed/disturb_argb_mip.dds");
			map6.Anisotropy = 4;

            var cubemap1 = loader.Load("examples/textures/compressed/Mountains.dds", texture => { 
                texture.MagFilter = Three.LinearFilter;
                texture.MinFilter = Three.LinearFilter;
                texture.Mapping = new Three.CubeReflectionMapping();
            //    material1.needsUpdate = true;
            });

            var cubemap2 = loader.Load("examples/textures/compressed/Mountains_argb_mip.dds", texture => { 
                texture.MagFilter = Three.LinearFilter;
                texture.MinFilter = Three.LinearFilter;
                texture.Mapping = new Three.CubeReflectionMapping();
            //    material5.needsUpdate = true;
            });

            var cubemap3 = loader.Load("examples/textures/compressed/Mountains_argb_nomip.dds", texture => { 
                texture.MagFilter = Three.LinearFilter;
                texture.MinFilter = Three.LinearFilter;
                texture.Mapping = new Three.CubeReflectionMapping();
            //    material6.needsUpdate = true;
            });

            var material1 = new MeshBasicMaterial() { Map = map1, EnvMap = cubemap1 };
            var material2 = new MeshBasicMaterial() { Map = map2 };
            var material3 = new MeshBasicMaterial() { Map = map3, AlphaTest = 0.5f, Side = Three.DoubleSide };
			var material4 = new MeshBasicMaterial() { Map = map4, Side = Three.DoubleSide, Blending = Three.AdditiveBlending, DepthTest = false, Transparent = true } ;
            var material5 = new MeshBasicMaterial() { EnvMap = cubemap2 };
            var material6 = new MeshBasicMaterial() { EnvMap = cubemap3 };
            var material7 = new MeshBasicMaterial() { Map = map5 };
            var material8 = new MeshBasicMaterial() { Map = map6 };

            // After loading setting

            material1.NeedsUpdate = true;
            material5.NeedsUpdate = true;
            material6.NeedsUpdate = true;




            // Testing purposes

            var texture1 = ImageUtils.LoadTexture(@"examples/textures/crate.jpg");
            texture1.Anisotropy = this.renderer.MaxAnisotropy;

            var material = new MeshBasicMaterial { Map = texture1 };


            // TODO NOTE EnvMap is not working properly


            // . . . .
            // X . . .

            var mesh = new Mesh(new TorusGeometry(100, 50, 32, 16), material1); // material1
			mesh.Position.X = -600;
			mesh.Position.Y = -200;
            this.scene.Add(mesh);
            this.meshes.Add(mesh);

            // . . . .
            // . X . .

            mesh = new Mesh(this.geometry, material2); // material2
			mesh.Position.X = -200;
			mesh.Position.Y = -200;
            this.scene.Add(mesh);
            this.meshes.Add(mesh);

            // . X . .
            // . . . .

            mesh = new Mesh(this.geometry, material3); // material3
			mesh.Position.X = -200;
			mesh.Position.Y = 200;
            this.scene.Add(mesh);
            this.meshes.Add(mesh);

            // X . . .
            // . . . .

            mesh = new Mesh(this.geometry, material4); // material4
			mesh.Position.X = -600;
			mesh.Position.Y = 200;
            this.scene.Add(mesh);
            this.meshes.Add(mesh);

            // . . X .
            // . . . .

            mesh = new Mesh(new BoxGeometry(200, 200, 200), material5); // material5
			mesh.Position.X = 200;
			mesh.Position.Y = 200;
            this.scene.Add(mesh);
            this.meshes.Add(mesh);

            // . . . .
            // . . X .

            mesh = new Mesh(new BoxGeometry(200, 200, 200), material6); // material6
			mesh.Position.X = 200;
			mesh.Position.Y = -200;
            this.scene.Add(mesh);
            this.meshes.Add(mesh);

            // . . . .
            // . . . X

            mesh = new Mesh(this.geometry, material7); // material7
            mesh.Position.X = 600;
            mesh.Position.Y = -200;
            this.scene.Add(mesh);
            this.meshes.Add(mesh);

            // . . . X
            // . . . .

            mesh = new Mesh(this.geometry, material8); // material8
			mesh.Position.X = 600;
			mesh.Position.Y = 200;
            this.scene.Add(mesh);
            this.meshes.Add(mesh);
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
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
            this.mouse.X = (here.X - ((float)clientSize.Width / 2));
            this.mouse.Y = (here.Y - ((float)clientSize.Height / 2));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            var time = this.stopWatch.ElapsedMilliseconds * 0.001f;

            foreach (var mesh in this.meshes)
            {
                mesh.Rotation.X = time;
                mesh.Rotation.Y = time;
            }

            this.renderer.Render(this.scene, this.camera);
        }
    }
}
