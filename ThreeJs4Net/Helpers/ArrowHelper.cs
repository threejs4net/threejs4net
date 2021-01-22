using System;
using System.Drawing;
using ThreeJs4Net.Core;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Helpers
{
    public class ArrowHelper : Object3D
    {
        private Line line;

        private Mesh cone;

        private Color color;

        private float headLength;

        private float headWidth;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="origin"></param>
        /// <param name="length"></param>
        /// <param name="color"></param>
        /// <param name="headLength"></param>
        /// <param name="headWidth"></param>
        public void aa(Vector3 dir, Vector3 origin, float length, Color color, float headLength, float headWidth)
        {
            throw new NotImplementedException();

            var lineGeometry = new Geometry();
            lineGeometry.Vertices.Add(new Vector3(0, 0, 0));
            lineGeometry.Vertices.Add(new Vector3(0, 1, 0));

            var coneGeometry = new CylinderGeometry(0, 0.5f, 1, 5, 1);
            //          coneGeometry.ApplyMatrix4(new Matrix4().MakeTranslation(0, - 0.5, 0));

            //if ( color == null ) color = 0xffff00;
            //if ( length == null ) length = 1;
            //if (headLength == null) headLength = 0.2 * length;
            //if (headWidth == null) headWidth = 0.2 * headLength;

            this.Position = origin;

            //		    this.line = new Line( lineGeometry, new LineBasicMaterial() { color = color } ) ;
            this.line.MatrixAutoUpdate = false;
            this.Add(this.line);

            this.cone = new Mesh(coneGeometry, new MeshBasicMaterial() { Color = color });
            this.cone.MatrixAutoUpdate = false;
            this.Add(this.cone);

            this.SetDirection(dir);
            this.SetLength(length, headLength, headWidth);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDirection(Vector3 dir)
        {
            // dir is assumed to be normalized
            if (dir.Y > 0.99999)
            {
                this.Quaternion = new Quaternion(0, 0, 0, 1);
            }
            else if (dir.Y < -0.99999)
            {
                this.Quaternion = new Quaternion(1, 0, 0, 0);
            }
            else
            {
                Vector3 axis = new Vector3(dir.Z, 0, -dir.X).Normalize();
                var radians = (float)System.Math.Acos(dir.Y);
                this.Quaternion.SetFromAxisAngle(axis, radians);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="headLength"></param>
        /// <param name="headWidth"></param>
        public void SetLength(float length, float headLength, float headWidth)
        {
            //if (headLength) headLength = 0.2 * length;
            //if (headWidth) headWidth = 0.2 * headLength;

            this.line.Scale = new Vector3(1, length, 1);
            this.line.UpdateMatrix();

            this.cone.Scale = new Vector3(headWidth, headLength, headWidth);
            this.cone.Position.Y = length;
            this.cone.UpdateMatrix();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            //this.line.Material.Color = color;
            //this.cone.Material.Color = color;
        }
    }
}
