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
    [Example("webgl_interactive_draggablecubes", ExampleCategory.OpenTK, "Interactive", 0.5f)]
    class webgl_interactive_draggablecubes : Example
    {
        private PerspectiveCamera camera;

        private Object3D group;

        private Object3D SELECTED; 

        private Object3D INTERSECTED; 

        private Mesh plane;

        private readonly IList<Object3D> object3Ds = new List<Object3D>();

        private Projector projector;

        private Scene scene;

        private readonly Vector3 offset = new Vector3();

        private readonly Vector2 mouse = new Vector2();

        private TrackballControls controls;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);
            
            camera = new PerspectiveCamera(70, control.Width / (float)control.Height, 1, 10000);
			camera.Position.Z = 1000;

            controls = new TrackballControls(control, camera);
            controls.RotateSpeed = 1.0f;
            controls.ZoomSpeed = 1.2f;
            controls.PanSpeed = 0.8f;
            controls.NoZoom = false;
            controls.NoPan = false;
            controls.StaticMoving = true;
            controls.DynamicDampingFactor = 0.3f;

			scene = new Scene();

            scene.Add(new AmbientLight((Color)colorConvertor.ConvertFromString("#505050")));

			var light = new SpotLight( Color.White, 1.5f );
			light.Position = new Vector3( 0, 500, 2000 );
			light.CastShadow = true;

			light.shadowCameraNear = 200;
			light.shadowCameraFar = camera.Far;
			light.shadowCameraFov = 50;
                  
			light.shadowBias = -0.00022f;
			light.shadowDarkness = 0.5f;
                  
			light.shadowMapWidth = 2048;
			light.shadowMapHeight = 2048;

			scene.Add( light );

			var geometry = new BoxGeometry( 40, 40, 40 );

			for ( var i = 0; i < 200; i ++ ) {

                var object3D = new Mesh(geometry, new MeshLambertMaterial() { Color = new Color().Random() });

			    ((MeshLambertMaterial)object3D.Material).Ambient = ((MeshLambertMaterial)object3D.Material).Color;

			    object3D.Position.X = Mat.Random() * 1000 - 500;
			    object3D.Position.Y = Mat.Random() * 600 - 300;
			    object3D.Position.Z = Mat.Random() * 800 - 400;

			    object3D.Rotation.X = (float)(Mat.Random() * 2 * System.Math.PI);
			    object3D.Rotation.Y = (float)(Mat.Random() * 2 * System.Math.PI);
			    object3D.Rotation.Z = (float)(Mat.Random() * 2 * System.Math.PI);

			    object3D.Scale.X = Mat.Random() * 2 + 1;
			    object3D.Scale.Y = Mat.Random() * 2 + 1;
			    object3D.Scale.Z = Mat.Random() * 2 + 1;

				object3D.CastShadow = true;
				object3D.ReceiveShadow = true;

			    scene.Add(object3D);

			    object3Ds.Add(object3D);
			}

			plane = new Mesh( 
                new PlaneGeometry( 2000, 2000, 8, 8 ),
                new MeshBasicMaterial() { Color = Color.Black, Opacity = 0.25f, Transparent = true } 
            );
			plane.Visible = true;
 //           scene.Add(plane);

			renderer.SetClearColor( (Color)colorConvertor.ConvertFromString("#F0F0F0") );
			renderer.SortObjects = false;

			renderer.shadowMapEnabled = true;
			renderer.shadowMapType = Three.PCFShadowMap;
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
        public override void MouseDown(Size clientSize, Point here)
        {
            var vector = new Vector3(mouse.X, mouse.Y, 0.5f).Unproject(camera);

            var raycaster = new Raycaster(camera.Position, vector.Sub(camera.Position).Normalize());

            var intersects = raycaster.IntersectObjects(object3Ds);

			if ( intersects.Count > 0 ) {

				//controls.enabled = false;

			    SELECTED = intersects[0].Object3D; // take the closest to camera

			    var intersects2 = raycaster.IntersectObject(plane);
			    offset.Copy(intersects2[0].Point).Sub(plane.Position);

			    //container.style.cursor = 'move';
			}
       }


        private Color currentHex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
            // Normalize mouse position
            mouse.X =  (here.X / (float)clientSize.Width) * 2 - 1;
            mouse.Y = -(here.Y / (float)clientSize.Height) * 2 + 1;

            var vector = new Vector3(mouse.X, mouse.Y, 0.5f).Unproject(camera);

            var raycaster = new Raycaster(camera.Position, vector.Sub(camera.Position).Normalize());
/*
            if (null != SELECTED)
            {
                var intersects2 = raycaster.IntersectObject(plane);
                SELECTED.Position.Copy(intersects2[0].Point.Sub(offset));

                return;
            }
*/
            var intersects = raycaster.IntersectObjects(object3Ds);

			if ( intersects.Count > 0 ) {

				if ( INTERSECTED != intersects[ 0 ].Object3D ) {

					if (null != INTERSECTED )
                        ((MeshLambertMaterial)INTERSECTED.Material).Color = currentHex;

                    INTERSECTED = intersects[0].Object3D;
                    currentHex = ((MeshLambertMaterial)INTERSECTED.Material).Color;

					plane.Position.Copy( INTERSECTED.Position );
					plane.LookAt( camera.Position );

				}

			//	container.style.cursor = 'pointer';

			} 
            else
			{
			    if (INTERSECTED != null)
                    ((MeshLambertMaterial)INTERSECTED.Material).Color = currentHex;

				INTERSECTED = null;

			//	container.style.cursor = 'auto';

			}

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseUp(Size clientSize, Point here)
        {
			//controls.enabled = true;

			if (null != INTERSECTED )
            {
				plane.Position.Copy( INTERSECTED.Position );

				SELECTED = null;
			}

			//container.style.cursor = 'auto';
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            //controls.update();

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
