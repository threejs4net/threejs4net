using System;
using System.Collections.Generic;
using System.Drawing;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Core
{
    public class Face3
    {
        public int A;
        public int B;
        public int C;

        public Vector3 Normal;
        public IList<Vector3> VertexNormals = new List<Vector3>();
        public Color Color;
        public Color[] VertexColors = new Color[3];
        public int MaterialIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Face3(int a, int b, int c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.Normal = Vector3.One();
            this.Color = Color.White;
            this.MaterialIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="normal"></param>
        /// <param name="color"></param>
        /// <param name="materialIndex"></param>
        public Face3(int a, int b, int c, Vector3 normal, Color color, int materialIndex = 0)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.Normal = normal;
            this.Color = color;
            this.MaterialIndex = materialIndex;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        protected Face3()
        {
            
        }

        #region --- Already in R116 ---

        public Face3 Clone()
        {
            return new Face3().Copy(this);
        }

        public Face3 Copy(Face3 source)
        {
            this.A = source.A;
            this.B = source.B;
            this.C = source.C;

            this.Normal.Copy(source.Normal);
            this.Color = source.Color;
            this.MaterialIndex = source.MaterialIndex;

            this.VertexNormals.Clear();
            
            foreach (var vertextNormal in source.VertexNormals)
            {
                this.VertexNormals.Add(vertextNormal.Clone());
            }

            for (int i = 0; i < source.VertexColors.Length; i++)
            {
                this.VertexColors[i] = source.VertexColors[i];
            }

            return this;
        }

        #endregion
    }
}
