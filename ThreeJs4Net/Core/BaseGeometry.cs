using System;
using System.Collections.Generic;
using System.Drawing;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Core
{
    public abstract class BaseGeometry : IDisposable
    {
        public Guid Uuid = Guid.NewGuid();
        private bool _disposed = false;
        public int Id;
        public string Name;
        public string type;
        public Box3 BoundingBox = null;
        public Sphere BoundingSphere = null;
        public int __webglLineDistanceBuffer = 0;
        public int __webglVertexBuffer = 0;
        public int __webglNormalBuffer = 0;
        public int __webglTangentBuffer = 0;
        public int __webglColorBuffer = 0;
        public int __webglUVBuffer = 0;
        public int __webglUV2Buffer = 0;
        public int __webglSkinIndicesBuffer = 0;
        public int __webglSkinWeightsBuffer = 0;
        public int __webglFaceBuffer = 0;
        public int __webglLineBuffer = 0;
        public List<int> __webglMorphTargetsBuffers;
        public List<int> __webglMorphNormalsBuffers;
        public object __sortArray;
        public float[] __vertexArray;
        public float[] __normalArray;
        public float[] __tangentArray;
        public float[] __colorArray;
        public float[] __uvArray;
        public float[] __uv2Array;
        public float[] __skinIndexArray;
        public float[] __skinWeightArray;
        public Type __typeArray;
        public ushort[] __faceArray;
        public ushort[] __lineArray;
        public List<float[]> __morphTargetsArrays;
        public List<float[]> __morphNormalsArrays;
        public int __webglFaceCount = -1;
        public int __webglLineCount = -1;
        public int __webglParticleCount = -1;
        public List<Renderers.Shaders.Attribute> __webglCustomAttributesList;
        public bool __inittedArrays;
        public float[] __lineDistanceArray;

        public bool __webglInit { get; set; }

        public virtual void ComputeBoundingSphere() { }
        public virtual void ComputeBoundingBox() { }
        public virtual void ComputeVertexNormals(bool areaWeighted = false) { }
        public virtual void ApplyMatrix4(Matrix4 matrix) { }


        public event EventHandler<EventArgs> Disposed;

        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing A second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                try
                {
                    this._disposed = true;

                    this.RaiseDisposed();
                }
                finally
                {
                    //base.Dispose(true);           // call any base classes
                }
            }
        }
        #endregion
    }

    public interface IMorphTarget {
        public string Name { get; set; }
        public List<Vector3> Vertices { get; set; }
    }

    public interface IMorphNormals {
        public string Name { get; set; }
        public List<Vector3> Normals { get; set; }
    }
    
    public interface IMorphColor {
        public string Name { get; set; }
        public List<Color> Colors { get; set; }
    }
}
