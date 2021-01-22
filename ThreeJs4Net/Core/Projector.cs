using System;
using System.Collections.Generic;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Math;
using ThreeJs4Net.Renderers.Renderables;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Core
{
    public class RenderDataStorage
    {

        public List<Light> Lights = new List<Light>();

        public List<Object3D> Objects = new List<Object3D>();

        //      public List<Sprite> Sprites = new List<Sprite>();

    }

    public class Projector
    {
        private int _objectCount;

        private int _objectPoolLength;

        private int _vertexCount;

        private int _vertexPoolLength;

        private int _faceCount;

        private int _facePoolLength;

        private int _lineCount;

        private int _linePoolLength;

        private int _spriteCount;

        private int _spritePoolLength;

        private List<RenderableObject> _objectPool = new List<RenderableObject>();

        private List<RenderableVertex> _vertexPool = new List<RenderableVertex>();

        private List<RenderableFace> _facePool = new List<RenderableFace>();

        private List<RenderableLine> _linePool = new List<RenderableLine>();

        private List<RenderableSprite> _spritePool = new List<RenderableSprite>();

        private Vector3 _vA = new Vector3();

        private Vector3 _vB = new Vector3();

        private Vector3 _vC = new Vector3();

        private Vector3 _vector3 = new Vector3();

        private Vector4 _vector4 = new Vector4();

        private Box3 _clipBox = new Box3(new Vector3(- 1, - 1, - 1), new Vector3(1, 1, 1));

        private Box3 _boundingBox = new Box3();

        //private Vector3 _points3 = new Array( 3 );

        //private Vector3 _points4 = new Array( 4 );

        private Matrix4 _viewMatrix = new Matrix4();

        private Matrix4 _viewProjectionMatrix = new Matrix4();

        private Matrix4 _modelMatrix;

        private Matrix4 _modelViewProjectionMatrix = new Matrix4();

        private Matrix3 _normalMatrix = new Matrix3();

        private Frustum _frustum = new Frustum();

        private Vector4 _clippedVertex1PositionScreen = new Vector4();

        private Vector4 _clippedVertex2PositionScreen = new Vector4();

        public RenderDataStorage RenderData = new RenderDataStorage();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="camera"></param>
        public Vector3 ProjectVector(Vector3 vector, Camera camera)
        {
            camera.MatrixWorldInverse = camera.MatrixWorld.GetInverse();
            _viewProjectionMatrix = camera.ProjectionMatrix * camera.MatrixWorldInverse;

            return vector.ApplyProjection(_viewProjectionMatrix);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public Vector3 UnprojectVector(Vector3 vector, Camera camera)
        {
            _viewProjectionMatrix = camera.MatrixWorld * camera.ProjectionMatrix.GetInverse();

            return vector.ApplyProjection(_viewProjectionMatrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="camera"></param>
        public Raycaster PickingRay(Vector3 vector, Camera camera)
        {
            // Set two vectors with opposing z values
            vector.Z = - 1.0f;
            var end = new Vector3(vector.X, vector.Y, 1.0f);

            this.UnprojectVector(vector, camera);
            this.UnprojectVector(end, camera);

            // find direction from vector to end
            var c = new Vector3().SubVectors(end, vector).Normalize();

            return new Raycaster(vector, c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Object"></param>
        public void ProjectObject(Light Object)
        {
            if (Object.Visible == false) return;
            this.RenderData.Lights.Add(Object);

            foreach (Object3D child in Object.Children)
            {
                this.ProjectObject(child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Object"></param>
        public void ProjectObject(Object3D Object)
        {
            if (Object.Visible == false) return;
            //if (Object.FrustumCulled == false || Fru)
            foreach (Object3D child in Object.Children)
            {
                this.ProjectObject(child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RenderList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        /// <param name="sortObjects"></param>
        /// <param name="sortElements"></param>
        public void ProjectScene(Scene scene, Camera camera, bool sortObjects, bool sortElements)
        {
            if (scene.AutoUpdate == true) scene.UpdateMatrixWorld();
            if (camera.Parent == null) camera.UpdateMatrixWorld();

            _viewMatrix.Copy(camera.MatrixWorldInverse = camera.MatrixWorld.GetInverse());
            _viewProjectionMatrix.MultiplyMatrices(camera.ProjectionMatrix, _viewMatrix);

            _frustum.SetFromMatrix(_viewProjectionMatrix);



            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public RenderableObject GetNextObjectInPool()
        {
            if (_objectCount == _objectPoolLength)
            {
                var object3D = new RenderableObject();
                _objectPool.Add(object3D);
                _objectPoolLength ++;
                _objectCount ++;

                return object3D;
            }

            return _objectPool[_objectCount ++];
        }

        /// <summary>
        /// 
        /// </summary>
        public RenderableVertex GetNextVertexInPool()
        {
            if (_vertexCount == _vertexPoolLength)
            {
                var vertex = new RenderableVertex();
                _vertexPool.Add(vertex);
                _vertexPoolLength ++;
                _vertexCount ++;

                return vertex;
            }

            return _vertexPool[_vertexCount ++];
        }

        /// <summary>
        /// 
        /// </summary>
        public RenderableFace GetNextFaceInPool()
        {
            if (_faceCount == _facePoolLength)
            {
                var face = new RenderableFace();
                _facePool.Add(face);
                _facePoolLength ++;
                _faceCount ++;

                return face;
            }

            return _facePool[_faceCount ++];
        }

        /// <summary>
        /// 
        /// </summary>
        public RenderableLine GetNextLineInPool()
        {
            if (_lineCount == _linePoolLength)
            {
                var line = new RenderableLine();
                _linePool.Add(line);
                _linePoolLength ++;
                _lineCount ++;

                return line;
            }

            return _linePool[_lineCount ++];
        }

        /// <summary>
        /// 
        /// </summary>
        public RenderableSprite GetNextSpriteInPool()
        {
            if (_spriteCount == _spritePoolLength)
            {
                var sprite = new RenderableSprite();
                _spritePool.Add(sprite);
                _spritePoolLength ++;
                _spriteCount ++;

                return sprite;
            }

            return _spritePool[_spriteCount ++];
        }

        /// <summary>
        /// 
        /// </summary>
        public void PainterSort()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        public bool ClipLine(Vector4 s1, Vector4 s2)
        {
            float alpha1 = 0;
            float alpha2 = 1;

            // Calculate the boundary coordinate of each vertex for the near and far clip planes,
            // Z = -1 and Z = +1, respectively.
            var bc1Near = s1.Z + s1.W;
            var bc2Near = s2.Z + s2.W;
            var bc1Far = - s1.Z + s1.W;
            var bc2Far = - s2.Z + s2.W;

            if (bc1Near >= 0 && bc2Near >= 0 && bc1Far >= 0 && bc2Far >= 0)
            {
                // Both vertices lie entirely within all clip planes.
                return true;

            }
            else if ((bc1Near < 0 && bc2Near < 0) || (bc1Far < 0 && bc2Far < 0))
            {
                // Both vertices lie entirely outside one of the clip planes.
                return false;

            }
            else
            {
                // The line segment spans at least one clip plane.
                if (bc1Near < 0)
                {

                    // v1 lies outside the near plane, v2 inside
                    alpha1 = System.Math.Max(alpha1, bc1Near / (bc1Near - bc2Near));

                }
                else if (bc2Near < 0)
                {

                    // v2 lies outside the near plane, v1 inside
                    alpha2 = System.Math.Min(alpha2, bc1Near / (bc1Near - bc2Near));

                }

                if (bc1Far < 0)
                {

                    // v1 lies outside the far plane, v2 inside
                    alpha1 = System.Math.Max(alpha1, bc1Far / (bc1Far - bc2Far));

                }
                else if (bc2Far < 0)
                {

                    // v2 lies outside the far plane, v2 inside
                    alpha2 = System.Math.Min(alpha2, bc1Far / (bc1Far - bc2Far));

                }

                if (alpha2 < alpha1)
                {

                    // The line segment spans two boundaries, but is outside both of them.
                    // (This can't happen when we're only clipping against just near/far but good
                    // to leave the check here for future usage if other clip planes are added.)
                    return false;

                }
                else
                {
                    // Update the s1 and s2 vertices to match the clipped line segment.
                    s1.Lerp(s2, alpha1);
                    s2.Lerp(s1, 1 - alpha2);

                    return true;
                }
            }
        }
    }
}