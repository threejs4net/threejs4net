using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Cameras
{
    public class Camera : Object3D
    {
        public Matrix4 MatrixWorldInverse = new Matrix4().Identity();
        public Matrix4 ProjectionMatrix = new Matrix4().Identity();
        public Matrix4 ProjectionMatrixInverse = new Matrix4().Identity();

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Camera()
        {
            this.type = "Camera";
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected Camera(Camera other) : base(other) { }

        #endregion







        #region --- Already in R116 ---
        public Camera Copy(Camera source, bool recursive)
        {
            //!! TEST this... not sure 
            base.Copy(source, recursive);

            this.MatrixWorldInverse.Copy(source.MatrixWorldInverse);
            this.ProjectionMatrix.Copy(source.ProjectionMatrix);
            this.ProjectionMatrixInverse.Copy(source.ProjectionMatrixInverse);

            return this;
        }

        public override Vector3 GetWorldDirection(Vector3 target)
        {
            if (target == null)
            {
                target = new Vector3();
            }

            this.UpdateMatrixWorld(true);
            var e = this.MatrixWorld.elements;
            return target.Set(-e[8], -e[9], -e[10]).Normalize();
        }

        public override void UpdateWorldMatrix(bool updateParents, bool updateChildren)
        {
            base.UpdateWorldMatrix(updateParents, updateChildren);
            this.MatrixWorldInverse.GetInverse(this.MatrixWorld);
        }

        public override void UpdateMatrixWorld(bool force = false)
        {
            base.UpdateMatrixWorld(force);
            this.MatrixWorldInverse.GetInverse(this.MatrixWorld);
        }


        #endregion
    }
}