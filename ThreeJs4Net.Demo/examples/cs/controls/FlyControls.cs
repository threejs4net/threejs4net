using System.Diagnostics;
using System.Windows.Forms;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Demo.examples.cs.controls
{
    public class FlyControls
    {
        struct MoveState
        {
            public float up;

            public float down;

            public float right;

            public float left;

            public float forward;

            public float back;

            public float pitchUp;

            public float pitchDown;

            public float yawLeft;

            public float yawRight;

            public float rollLeft;

            public float rollRight;
        }


        public Object3D Object3D;

        public float MovementSpeed = 1.0f;

        public float RollSpeed = 0.005f;

        public bool DragToLook = false;

        public bool AutoForward = false;

        // disable default target object behavior

        // internals

        private Quaternion tmpQuaternion = new Quaternion();

        private int mouseStatus = 0;

        MoveState moveState = new MoveState() { up= 0, down= 0, left= 0, right= 0, forward= 0, back= 0, pitchUp= 0, pitchDown= 0, yawLeft= 0, yawRight= 0, rollLeft= 0, rollRight= 0 };
    
        private readonly Vector3 moveVector = new Vector3( 0, 0, 0 );
        
        private readonly Vector3 rotationVector = new Vector3( 0, 0, 0 );

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="object3D"></param>
        public FlyControls(Control control, Object3D object3D)
        {
            this.Object3D = object3D;

            control.MouseMove += MouseMove;        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Update(float delta)
        {
		    var moveMult = delta * this.MovementSpeed;
		    var rotMult = delta * this.RollSpeed;

            this.Object3D.TranslateX(this.moveVector.X * moveMult);
            this.Object3D.TranslateY(this.moveVector.Y * moveMult);
            this.Object3D.TranslateZ(this.moveVector.Z * moveMult);

		    this.tmpQuaternion = new Quaternion( this.rotationVector.X * rotMult, this.rotationVector.Y * rotMult, this.rotationVector.Z * rotMult, 1 ).Normalize();
		    this.Object3D.Quaternion.Multiply( this.tmpQuaternion );

		    // expose the rotation vector for convenience
            this.Object3D.Rotation.SetFromQuaternion(this.Object3D.Quaternion, this.Object3D.Rotation.Order);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateMovementVector () 
        {
		    var forward = ( (this.moveState.forward != 0) || ( (this.AutoForward) && (this.moveState.back == 0)) ) ? 1 : 0;

		    this.moveVector.X = ( -this.moveState.left    + this.moveState.right );
		    this.moveVector.Y = ( -this.moveState.down    + this.moveState.up );
		    this.moveVector.Z = ( -forward + this.moveState.back );

		    //console.log( 'move:', [ this.moveVector.x, this.moveVector.y, this.moveVector.z ] );
	    }

        /// <summary>
        /// 
        /// </summary>
	    public void UpdateRotationVector() 
        {
		    this.rotationVector.X = ( -this.moveState.pitchDown + this.moveState.pitchUp );
		    this.rotationVector.Y = ( -this.moveState.yawRight  + this.moveState.yawLeft );
		    this.rotationVector.Z = ( -this.moveState.rollRight + this.moveState.rollLeft );

		    //console.log( 'rotate:', [ this.rotationVector.x, this.rotationVector.y, this.rotationVector.z ] );
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MouseMove(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            Debug.Assert(null != control);

		    if ( !this.DragToLook || this.mouseStatus > 0 ) {

			    var halfWidth  = control.Size.Width / 2;
			    var halfHeight = control.Size.Height / 2;

			    this.moveState.yawLeft   = - ( ( e.X - 0 ) - halfWidth  ) / (float)halfWidth;
			    this.moveState.pitchDown =   ( ( e.Y - 0 ) - halfHeight ) / (float)halfHeight;

			    this.UpdateRotationVector();
		    }
        }

        /*

        public void mousemove ( event )
        {

		    if ( !this.DragToLook || this.mouseStatus > 0 ) {

			    var container = this.getContainerDimensions();
			    var halfWidth  = container.size[ 0 ] / 2;
			    var halfHeight = container.size[ 1 ] / 2;

			    this.moveState.yawLeft   = - ( ( event.pageX - container.offset[ 0 ] ) - halfWidth  ) / halfWidth;
			    this.moveState.pitchDown =   ( ( event.pageY - container.offset[ 1 ] ) - halfHeight ) / halfHeight;

			    this.UpdateRotationVector();

		    }

	    }
*/


    }
}
