using System;
using System.Collections.Generic;
using System.Drawing;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Core
{
    public class Face3
    {
        public int a;
        public int b;
        public int c;

        public Vector3 Normal;

        public IList<Vector3> VertexNormals = new List<Vector3>();

        public Color color;

        public Color[] VertexColors = new Color[3];

        public List<Vector4> VertexTangents = new List<Vector4>();

        public int MaterialIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Face3(int a, int b, int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.Normal = Vector3.One();
            this.color = Color.White;
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
            this.a = a;
            this.b = b;
            this.c = c;
            this.Normal = normal;
            this.color = color;
            this.MaterialIndex = materialIndex;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        protected Face3(Face3 other)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Face3 Clone()
        {
            return new Face3(this);
        }

    }
}
