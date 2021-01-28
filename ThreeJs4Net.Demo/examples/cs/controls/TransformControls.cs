using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Demo.examples.cs.controls
{
    class GizmoMaterial : MeshBasicMaterial
    {
        private float oldOpacity;

        private Color oldColor;

        /// <summary>
        /// Constructor
        /// </summary>
        public GizmoMaterial()
        {
            this.DepthTest = false;
            this.DepthWrite = false;
            this.Side = Three.FrontSide;
            this.Transparent = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="highlighted"></param>
        public void Highlight(bool highlighted)
        {
            if (highlighted)
            {
                this.Color = Color.FromArgb(255, 255, 255, 0);
                this.Opacity = 1;
            }
            else
            {
                this.Color = this.oldColor;
                this.Opacity = this.oldOpacity;
            }
        }
    }

    class GizmoLineMaterial : LineBasicMaterial
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GizmoLineMaterial()
        {
            this.DepthTest = false;
            this.DepthWrite = false;
            this.Transparent = true;
            this.Linewidth = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="highlighted"></param>
        /// <returns></returns>
        public void Highlight(bool highlighted)
        {
        }
    }

    abstract class TransformGizmo : Object3D
    {
        protected bool showPickers = false; //debug
        protected bool showActivePlane = false; //debug

        protected Object3D handles;
        protected Object3D pickers;
        protected Object3D planes;

        protected Object3D activePlane;

        protected Hashtable handleGizmos;
        protected Hashtable pickerGizmos;

        /// <summary>
        /// Constructor
        /// </summary>
        protected TransformGizmo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="eye"></param>
        public void Update(Euler rotation, Vector3 eye)
        {
            var vec1 = new Vector3(0, 0, 0);
            var vec2 = new Vector3(0, 1, 0);
            var lookAtMatrix = new Matrix4();

            this.Traverse(child =>
            {
                if(string.IsNullOrEmpty(child.Name))
                {
                }
                else if (child.Name.Contains("E"))
                {
                    child.Quaternion.SetFromRotationMatrix(lookAtMatrix.LookAt(eye, vec1, vec2));
                }
                else if (child.Name.Contains("X") || child.Name.Contains("Y") || child.Name.Contains("Z"))
                {
                    child.Quaternion.SetFromEuler(rotation);
                }
            });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public void init()
        {
            this.handles = new Object3D();
            this.pickers = new Object3D();
            this.planes = new Object3D();

            this.Add(this.handles);
            this.Add(this.pickers);
            this.Add(this.planes);

			//// PLANES

			var planeGeometry = new PlaneGeometry( 50, 50, 2, 2 );
			var planeMaterial = new MeshBasicMaterial { Wireframe = true };
			planeMaterial.Side = Three.DoubleSide;

            var planes = new Hashtable();
            planes["XY"] = new Mesh(planeGeometry, planeMaterial);
            planes["YZ"] = new Mesh(planeGeometry, planeMaterial);
            planes["XZ"] = new Mesh(planeGeometry, planeMaterial);
            planes["XYZE"] = new Mesh(planeGeometry, planeMaterial);

            this.activePlane = (Mesh)planes["XYZE"];

            ((Object3D)planes["YZ"]).Rotation.Set(0, (float)System.Math.PI / 2, 0);
            ((Object3D)planes["XZ"]).Rotation.Set((float)-System.Math.PI / 2, 0, 0);

			foreach (DictionaryEntry i in planes)
			{
                ((Mesh)planes[i.Key]).Name = (string)i.Key;
                this.planes.Add((Object3D)planes[i.Key]);
                this.planes[i.Key] = planes[i.Key];
                ((Object3D)planes[i.Key]).Visible = false;
			}

            //// HANDLES AND PICKERS

            setupGizmos(this.handleGizmos, this.handles);
            setupGizmos(this.pickerGizmos, this.pickers);

            // reset Transformations

            this.Traverse(child => {
				if (child is Mesh) {
					child.UpdateMatrix();

				    var tempGeometry = new Geometry();
				    tempGeometry.Merge((Geometry)child.Geometry, child.Matrix);

					child.Geometry = tempGeometry;
				    child.Position.Set(0, 0, 0);
				    child.Rotation.Set(0, 0, 0);
				    child.Scale.Set(1, 1, 1);
				}
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gizmoMap"></param>
        /// <param name="parent"></param>
        private void setupGizmos(Hashtable gizmoMap, Object3D parent)
        {
			foreach (DictionaryEntry name in gizmoMap)
			{
                for (int i = ((ArrayList)gizmoMap[name.Key]).Count; i-- > 0; )
                {
                    var map = (ArrayList)((ArrayList)gizmoMap[name.Key])[i];

                    var object3D = (Object3D)map[0];
                    ArrayList position = null;
                    if (map.Count > 1)
                        position = (ArrayList)map[1];
                    ArrayList rotation = null;
                    if (map.Count > 2)
                        rotation = (ArrayList)map[2];

                    object3D.Name = (string)name.Key;

                    if (null != position) object3D.Position.Set((float)position[0], (float)position[1], (float)position[2]);
                    if (null != rotation) object3D.Rotation.Set((float)rotation[0], (float)rotation[1], (float)rotation[2]);

                    parent.Add(object3D);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
    	public void Hide()
        {
            this.Traverse( child => {
                child.Visible = false;
            });
        }

        /// <summary>
        /// 
        /// </summary>
		public void Show()
        {
            this.Traverse( child => {
				child.Visible = true;
				if (child.Parent == this.pickers ) child.Visible = showPickers;
				if (child.Parent == this.planes ) child.Visible = false;
            });

			this.activePlane.Visible = showActivePlane;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        public void Highlight(string axis )
        {
            this.Traverse(child =>
            {
                if ((null != child.Material) && (child.Material is GizmoMaterial))
                {
                    if (child.Name == axis)
                    {
                        ((GizmoMaterial)child.Material).Highlight(true);
                    }
                    else
                    {
                        ((GizmoMaterial)child.Material).Highlight(false);
                    }
                }
            });
        }
    }

    class TransformGizmoRotate : TransformGizmo
    {

    }

    struct handleGizmo
    {
        public Mesh mesh;
        public Object3D object3D;
        public Vector3 s1;
        public Vector3 s2;
    }

    class TransformGizmoTranslate : TransformGizmo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TransformGizmoTranslate()
        {
            var arrowGeometry = new Geometry();

            var mesh = new Mesh(new CylinderGeometry(0, 0.05f, 0.2f, 12, 1, false));
            mesh.Position.Y = 0.5f;
            mesh.UpdateMatrix();

            arrowGeometry.Merge((Geometry)mesh.Geometry, mesh.Matrix);

            var lineXGeometry = new Geometry();
            lineXGeometry.Vertices.AddRange( new [] { new Vector3(0, 0, 0), new Vector3(1, 0, 0) });

            var lineYGeometry = new Geometry();
            lineYGeometry.Vertices.AddRange(new[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0) });

            var lineZGeometry = new Geometry();
            lineZGeometry.Vertices.AddRange(new[] { new Vector3(0, 0, 0), new Vector3(0, 0, 1) });

            this.handleGizmos = new Hashtable();
            this.handleGizmos.Add("X",
                new ArrayList { new ArrayList { new Mesh(arrowGeometry, new GizmoMaterial { Color = Color.Red }) , new ArrayList { 0.5f, 0.0f, 0.0f }, new ArrayList { 0.0f, 0.0f, (float)-Mat.HalfPI }}, 
                                new ArrayList { new Line(lineXGeometry, new GizmoLineMaterial { Color = Color.Red })}});
            this.handleGizmos.Add("Y",
                new ArrayList { new ArrayList { new Mesh(arrowGeometry, new GizmoMaterial { Color = Color.Lime }), new ArrayList {0.0f,  0.5f, 0.0f }}, 
                                new ArrayList { new Line(lineYGeometry, new GizmoLineMaterial { Color = Color.Lime })} });
            this.handleGizmos.Add("Z",
                new ArrayList { new ArrayList { new Mesh(arrowGeometry, new GizmoMaterial { Color = Color.Blue }) , new ArrayList {0.0f, 0.0f, 0.5f }, new ArrayList { (float)Mat.HalfPI, 0.0f, 0.0f }}, 
                                new ArrayList { new Line(lineZGeometry, new GizmoLineMaterial { Color = Color.Blue }) }} );
            this.handleGizmos.Add("XYZ",
                new ArrayList { new ArrayList { new Mesh(new OctahedronGeometry(0.1f, 0), new GizmoMaterial { Color = Color.White, Opacity = 0.25f }), new ArrayList { 0.0f, 0.0f, 0.0f }, new ArrayList { 0.0f, 0.0f, 0.0f } } } );
            this.handleGizmos.Add("XY",
                new ArrayList { new ArrayList { new Mesh(new PlaneGeometry(0.29f, 0.29f), new GizmoMaterial { Color = Color.Yellow, Opacity = 0.25f }), new ArrayList { 0.15f, 0.15f, 0.0f } } } );
            this.handleGizmos.Add("YZ",
                new ArrayList { new ArrayList { new Mesh(new PlaneGeometry(0.29f, 0.29f), new GizmoMaterial { Color = Color.Cyan, Opacity = 0.25f }), new ArrayList { 0.0f, 0.15f, 0.15f }, new ArrayList { 0.0f, (float)Mat.HalfPI, 0.0f } } });
            this.handleGizmos.Add("XZ",
                new ArrayList { new ArrayList { new Mesh(new PlaneGeometry(0.29f, 0.29f), new GizmoMaterial { Color = Color.Magenta, Opacity = 0.25f }), new ArrayList { 0.15f, 0.0f, 0.15f }, new ArrayList { (float)-Mat.HalfPI, 0.0f, 0.0f } } });


            this.pickerGizmos = new Hashtable();
            this.pickerGizmos.Add("X",
                new ArrayList { new ArrayList { new Mesh(new CylinderGeometry(0.2f, 0, 1, 4, 1, false), new GizmoMaterial { Color = Color.Red, Opacity = 0.25f }), new ArrayList { 0.6f, 0.0f, 0.0f }, new ArrayList { 0.0f, 0.0f, (float)-Mat.HalfPI } } });
            this.pickerGizmos.Add("Y",
                new ArrayList { new ArrayList { new Mesh(new CylinderGeometry(0.2f, 0, 1, 4, 1, false), new GizmoMaterial { Color = Color.Lime, Opacity = 0.25f }), new ArrayList { 0.0f, 0.6f, 0.0f }, }});
            this.pickerGizmos.Add("Z",
                new ArrayList { new ArrayList { new Mesh(new CylinderGeometry(0.2f, 0, 1, 4, 1, false), new GizmoMaterial { Color = Color.Lime, Opacity = 0.25f }), new ArrayList { 0.0f, 0.0f, 0.6f }, new ArrayList { (float)Mat.HalfPI, 0.0f, 0.0f }}});
            this.pickerGizmos.Add("XYZ",
                new ArrayList { new ArrayList { new Mesh(new OctahedronGeometry(0.2f, 0), new GizmoMaterial { Color = Color.White, Opacity = 0.25f }) } } );
            this.pickerGizmos.Add("XY",
                new ArrayList { new ArrayList { new Mesh(new PlaneGeometry(0.4f, 0.4f), new GizmoMaterial { Color = Color.Yellow, Opacity = 0.25f }), new ArrayList { 0.2f, 0.2f, 0.0f } } });
            this.pickerGizmos.Add("YZ",
                new ArrayList { new ArrayList { new Mesh(new PlaneGeometry(0.4f, 0.4f), new GizmoMaterial { Color = Color.Cyan, Opacity = 0.25f }), new ArrayList { 0.0f, 0.2f, 0.2f }, new ArrayList { 0.0f, (float)Mat.HalfPI, 0.0f } } });
            this.pickerGizmos.Add("XZ",
                new ArrayList { new ArrayList { new Mesh(new PlaneGeometry(0.4f, 0.4f), new GizmoMaterial { Color = Color.Magenta, Opacity = 0.25f }), new ArrayList { 0.2f, 0.0f, 0.2f }, new ArrayList { (float)-Mat.HalfPI, 0.0f, 0.0f } } });
      
            this.init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="eye"></param>
    	public void setActivePlane (string axis, Vector3 eye ) 
        {
			var tempMatrix = new Matrix4();
            eye.ApplyMatrix4(tempMatrix.GetInverse(tempMatrix.ExtractRotation(((Object3D)this.planes["XY"]).MatrixWorld)));

			if ( axis == "X" ) {
                this.activePlane = (Object3D)this.planes["XY"];
                if (System.Math.Abs(eye.Y) > System.Math.Abs(eye.Z)) this.activePlane = (Object3D)this.planes["XZ"];
			}

			if ( axis == "Y" ){
                this.activePlane = (Object3D)this.planes["XY"];
                if (System.Math.Abs(eye.X) > System.Math.Abs(eye.Z)) this.activePlane = (Object3D)this.planes["YZ"];
			}

			if ( axis == "Z" ){
                this.activePlane = (Object3D)this.planes["XZ"];
                if (System.Math.Abs(eye.X) > System.Math.Abs(eye.Y)) this.activePlane = (Object3D)this.planes["YZ"];
			}

            if (axis == "XYZ") this.activePlane = (Object3D)this.planes["XYZE"];

            if (axis == "XY") this.activePlane = (Object3D)this.planes["XY"];

            if (axis == "YZ") this.activePlane = (Object3D)this.planes["YZ"];

            if (axis == "XZ") this.activePlane = (Object3D)this.planes["XZ"];

			this.Hide();
			this.Show();

		}
    }

    class TransformGizmoScale : TransformGizmo
    {

    }

    class TransformControls : Object3D, INotifyPropertyChanged
    {
        private readonly Hashtable gizmo;

        private Camera camera = null;

        private Object3D object3D = null;
        private Object3D snap = null;
		public string space = "world";
        public float size = 1;
		private string axis;

        private bool _dragging = false;
        private string _mode = "translate";
        private string _plane = "XY";

        private Raycaster ray = new Raycaster();
        private Vector3 pointerVector = new Vector3();

        private Vector3 point = new Vector3();
        private Vector3 offset = new Vector3();

        private Vector3 rotation = new Vector3();
        private Vector3 offsetRotation = new Vector3();
   //     private float scale = 1;

        private Matrix4 lookAtMatrix = new Matrix4();
   //     private Vector3 eye = new Vector3();

        private readonly Matrix4 tempMatrix = new Matrix4();
        private Vector3 tempVector = new Vector3();
        private Quaternion tempQuaternion = new Quaternion();
        private Vector3 unitX = new Vector3(1, 0, 0);
        private Vector3 unitY = new Vector3(0, 1, 0);
        private Vector3 unitZ = new Vector3(0, 0, 1);

        private Quaternion quaternionXYZ = new Quaternion();
        private Quaternion quaternionX = new Quaternion();
        private Quaternion quaternionY = new Quaternion();
        private Quaternion quaternionZ = new Quaternion();
        private Quaternion quaternionE = new Quaternion();

        private Vector3 oldPosition = new Vector3();
        private Vector3 oldScale = new Vector3();
        private Matrix4 oldRotationMatrix = new Matrix4();

        private Matrix4 parentRotationMatrix = new Matrix4();
        private Vector3 parentScale = new Vector3();

        private Vector3 worldPosition = new Vector3();
        private Euler worldRotation = new Euler();
        private Matrix4 worldRotationMatrix = new Matrix4();
        private Vector3 camPosition = new Vector3();
        private Euler camRotation = new Euler();

        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="control"></param>
        public TransformControls(Camera camera, Control control)
        {
            this.camera = camera;

            this.gizmo = new Hashtable();
            this.gizmo["translate"] = new TransformGizmoTranslate();
            this.gizmo["rotate"] = new TransformGizmoRotate();
            this.gizmo["scale"] = new TransformGizmoScale();

            control.MouseDown += onPointerDown;
            //domElement.addEventListener("touchstart", onPointerDown, false);

            control.MouseMove += onPointerHover;
            //domElement.addEventListener("touchmove", onPointerHover, false);

            control.MouseMove += onPointerMove;
            //domElement.addEventListener("touchmove", onPointerMove, false);

            control.MouseUp += onPointerUp;
            control.MouseLeave += onPointerOut;
            //domElement.addEventListener("touchend", onPointerUp, false);
            //domElement.addEventListener("touchcancel", onPointerUp, false);
            //domElement.addEventListener("touchleave", onPointerUp, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        public void Attach(Object3D object3D)
        {
            this.object3D = object3D;

            ((TransformGizmo)this.gizmo["translate"]).Hide();
            ((TransformGizmo)this.gizmo["rotate"]).Hide();
            ((TransformGizmo)this.gizmo["scale"]).Hide();
            ((TransformGizmo)this.gizmo[_mode]).Show();

			this.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        public void Detach(Object3D object3D)
        {
			this.object3D = null;
			this.axis = null;

            ((TransformGizmo)this.gizmo["translate"]).Hide();
            ((TransformGizmo)this.gizmo["rotate"]).Hide();
            ((TransformGizmo)this.gizmo["scale"]).Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetMode(string mode)
        {
            if (_mode == "scale") this.space = "local";

            ((TransformGizmo)this.gizmo["translate"]).Hide();
            ((TransformGizmo)this.gizmo["rotate"]).Hide();
            ((TransformGizmo)this.gizmo["scale"]).Hide();
            ((TransformGizmo)this.gizmo[_mode]).Show();

            this.Update();
            this.OnPropertyChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snap"></param>
        public void setSnap(Object3D snap)
        {
            this.snap = snap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public void setSize(float size)
        {
            this.size = size;
            this.Update();
            this.OnPropertyChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="space"></param>
        public void setSpace(string space)
        {
            this.space = space;
            this.Update();
            this.OnPropertyChanged();
        }


        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
			if ( this.object3D == null ) return;

            this.object3D.UpdateMatrixWorld();
            worldPosition.SetFromMatrixPosition(this.object3D.MatrixWorld);
            worldRotation.SetFromRotationMatrix(tempMatrix.ExtractRotation(this.object3D.MatrixWorld));

			camera.UpdateMatrixWorld();
            camPosition.SetFromMatrixPosition(camera.MatrixWorld);
            camRotation.SetFromRotationMatrix(tempMatrix.ExtractRotation(camera.MatrixWorld));

            var scale = worldPosition.DistanceTo(camPosition) / 6.0f * this.size;
            this.Position.Copy(worldPosition);
            this.Scale = new Vector3(scale, scale, scale);

            var eye = new Vector3();
            eye.Copy(camPosition).Sub(worldPosition).Normalize();

            if (this.space == "local")
                ((TransformGizmo)this.gizmo[_mode]).Update(worldRotation, eye);

            else if (this.space == "world")
                ((TransformGizmo)this.gizmo[_mode]).Update(new Euler(), eye);

            ((TransformGizmo)this.gizmo[_mode]).Highlight(this.axis);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onPointerDown(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onPointerOut(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onPointerUp(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onPointerHover(object sender, MouseEventArgs e)
        {
			if ( this.object3D == null || _dragging == true ) return;

		//	event.preventDefault();

            var pointer = e.Location;

            //var intersect = this.IntersectObjects( pointer, this.gizmo[_mode].pickers.Children );

            //var axis = null;

            //if (null != intersect ) {

            //    axis = intersect.object3D.name;

            //}

            //if ( this.axis != axis ) {

            //    this.axis = axis;
            //    this.Update();
            //    OnPropertyChanged();

            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onPointerMove(object sender, MouseEventArgs e)
        {
        }

            
        Intersect IntersectObjects(Point pointer, IEnumerable<Object3D> object3Ds ) 
        {
            //var rect = domElement.getBoundingClientRect();
            //var x = ( pointer.X - rect.left ) / rect.width;
            //var y = ( pointer.Y - rect.top ) / rect.height;

            //pointerVector.Set( ( x * 2 ) - 1, - ( y * 2 ) + 1, 0.5f );
            //pointerVector.Unproject(camera);

            //ray = new Raycaster(camPosition, pointerVector.Sub(camPosition).Normalize());

            //var intersections = ray.IntersectObjects(object3Ds, true);
            //return (intersections.Count > 0) ? intersections[0] : null;
            return null;
        }





        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
