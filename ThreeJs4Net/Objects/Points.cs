using System;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Objects
{
    public class Points : Object3D
    {
        private Matrix4 _inverseMatrix = new Matrix4();
        private Ray _ray = new Ray();
        private Sphere _sphere = new Sphere();
        private Vector3 _position = new Vector3();

        public BufferGeometry geometry;
        public PointsMaterial material;

        public Points(BufferGeometry geometry, PointsMaterial material)
        {
            this.geometry = geometry;
            this.material = material;

            this.UpdateMorphTargets();
        }

        public void UpdateMorphTargets()
        {
            throw new Exception("Not implemented");
        }
    }
}
