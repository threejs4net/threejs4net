using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Demo.examples
{
    [Example("webgl_panorama_equirectangular", ExampleCategory.OpenTK, "panorama")]
    class webgl_panorama_equirectangular : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Vector3 target;

        private Object3D mesh;

        private float lat, lon, phi, theta;

        private bool isUserInteracting;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            this.camera = new PerspectiveCamera(75, control.Width / (float)control.Height, 1, 1100);
            this.target = new Vector3(0, 0, 0);

            this.scene = new Scene();

            var geometry = new SphereGeometry(500, 60, 40);
            geometry.ApplyMatrix4(new Matrix4().MakeScale(-1, 1, 1));

            var material = new MeshBasicMaterial {
                Map = ImageUtils.LoadTexture(@"examples/textures/2294472375_24a3b8ef46_o.jpg")
            };

            this.mesh = new Mesh(geometry, material);

            this.scene.Add(mesh);
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

        private float onPointerDownPointerX, onPointerDownPointerY;

        private float onPointerDownLon, onPointerDownLat;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseDown(Size clientSize, Point here)
        {
			isUserInteracting = true;

			onPointerDownPointerX = here.X;
			onPointerDownPointerY = here.Y;

			onPointerDownLon = lon;
			onPointerDownLat = lat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
				if ( isUserInteracting == true ) 
                {
					lon = ( onPointerDownPointerX - here.X ) * 0.1f + onPointerDownLon;
					lat = ( here.Y - onPointerDownPointerY ) * 0.1f + onPointerDownLat;
				}     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        /// <param name="delta"></param>
        public override void MouseWheel(Size clientSize, Point here, int delta)
        {
            camera.Fov -= delta * 0.05f;

			camera.UpdateProjectionMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseUp(Size clientSize, Point here)
        {
            isUserInteracting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            Debug.Assert(null != this.renderer);
            Debug.Assert(null != this.mesh);

            if ( isUserInteracting == false ) {

					lon += 0.1f;

			}

            lat = System.Math.Max(-85, System.Math.Min(85, lat));
            phi = (float)Mat.DegToRad(90 - lat);
            theta = (float)Mat.DegToRad(lon);

            target.X = (float)(500 * System.Math.Sin(phi) * System.Math.Cos(theta));
            target.Y = (float)(500 * System.Math.Cos(phi));
            target.Z = (float)(500 * System.Math.Sin(phi) * System.Math.Sin(theta));

            camera.LookAt(target);

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
