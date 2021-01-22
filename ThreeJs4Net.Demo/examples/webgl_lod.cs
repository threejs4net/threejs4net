using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Demo.examples.cs.controls;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_lod", ExampleCategory.OpenTK, "lod", 0.5f)]
    class webgl_lod : Example
    {
        class GeometryEx
        {
            public Geometry geometry;

            public float distance;
        }

        private PerspectiveCamera camera;

        private Scene scene;

        private FlyControls controls;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(45, control.Width / (float)control.Height, 0.1f, 15000);
            this.camera.Position.Z = 1000;

            controls = new FlyControls(control, camera);
            controls.MovementSpeed = 1000.0f;
            controls.RollSpeed = (float)System.Math.PI / 10;

            scene = new Scene();
            scene.Fog = new Fog(Color.Black, 1, 15000);
            scene.AutoUpdate = false;

            var light1 = new PointLight((Color)colorConvertor.ConvertFromString("#ff2200"));
            light1.Position = new Vector3(0, 0, 0);
            scene.Add(light1);

            var light2 = new DirectionalLight(Color.White);
            light2.Position = new Vector3(0, 0, 1).Normalize();
            scene.Add(light2);

            var geometry = new List<GeometryEx>();
            geometry.Add(new GeometryEx() { distance = 50, geometry = new IcosahedronGeometry(100, 4) });
            geometry.Add(new GeometryEx() { distance = 300, geometry = new IcosahedronGeometry(100, 3) });
            geometry.Add(new GeometryEx() { distance = 1000, geometry = new IcosahedronGeometry(100, 2) });
            geometry.Add(new GeometryEx() { distance = 2000, geometry = new IcosahedronGeometry(100, 1) });
            geometry.Add(new GeometryEx() { distance = 8000, geometry = new IcosahedronGeometry(100, 4) });
            
			var material = new MeshLambertMaterial() { Color = Color.White, Wireframe = true } ;

			for (var j = 0; j < 1000; j ++ ) {

				var lod = new LOD();

				for (var i = 0; i < geometry.Count; i ++ )
				{
				    var mesh = new Mesh(geometry[i].geometry, material);
					mesh.Scale = new Vector3( 1.5f, 1.5f, 1.5f );
					mesh.UpdateMatrix();
					mesh.MatrixAutoUpdate = false;
				    lod.AddLevel(mesh, geometry[i].distance);
				}

				lod.Position.X = 10000 * ( 0.5f - Mat.Random() );
				lod.Position.Y =  7500 * ( 0.5f - Mat.Random() );
				lod.Position.Z = 10000 * ( 0.5f - Mat.Random() );
				lod.UpdateMatrix();
				lod.MatrixAutoUpdate = false;
			    scene.Add(lod);
			}

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

            this.camera.Aspect = clientSize.Width / (float)clientSize.Height;
            this.camera.UpdateProjectionMatrix();

            this.renderer.Size = clientSize;

            //controls.handleResize();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            Debug.Assert(null != this.renderer);

            var delta = stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
            stopWatch.Start();

            controls.Update(delta);

			scene.UpdateMatrixWorld();
            scene.Traverse(this.Update);

            renderer.Render(scene, camera);
        }

        private void Update(Object3D object3D)
        {
			if ( object3D is LOD ) {
                ((LOD)object3D).Update(camera);
			}
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
