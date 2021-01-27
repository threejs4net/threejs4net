//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ThreeJs4Net.Cameras;
//using ThreeJs4Net.Math;

//namespace ThreeJs4Net.Jsm.Controls
//{
//    public class OrbitControl
//    {
//        private ICameraProjection _object;
//        private Spherical spherical = new Spherical();
//        private Spherical sphericalDelta = new Spherical();
//        private OrbitControl scope;

//        private Vector3 target0;
//        private Vector3 position0;
//        private float zoom0;

//        public OrbitControl(ICameraProjection camera)
//        {
//            this._object = camera;
//            this.scope = this;
//        }

//        public float GetPolarAngle()
//        {
//            return spherical.Phi;
//        }

//        public float GetAzimuthalAngle()
//        {
//            return spherical.Theta;
//        }

//        public void SaveState()
//        {
//            this.target0.Copy(scope.Target);
//            this.position0.Copy(scope._object.Position);
//            this.zoom0 = scope._object.Zoom;
//        }

//        public void Reset()
//        {
//            scope.Target.copy(scope.target0);
//            scope._object.Position.Copy(scope.position0);
//            scope._object.zoom = scope.zoom0;

//            scope._object.UpdateProjectionMatrix();
//            scope.dispatchEvent(changeEvent);

//            scope.update();

//            state = STATE.NONE;
//        }

//        public void RotateUp(float angle)
//        {
//            sphericalDelta.Phi -= angle;
//        }

//        public float GetAutoRotationAngle()
//        {
//            return 2 * Mathf.PI / 60 / 60 * scope.autoRotateSpeed;
//        }

//        public float getZoomScale()
//        {
//            return Mathf.Pow(0.95, scope.zoomSpeed);
//        }

//        public void RotateLeft(float angle)
//        {
//            sphericalDelta.Theta -= angle;
//        }

//        public void PanLeft(float distance, Matrix4 objectMatrix)
//        {
//            var v = new Vector3();

//            v.SetFromMatrixColumn(objectMatrix, 0); // get X column of objectMatrix
//            v.MultiplyScalar(-distance);

//            panOffset.add(v);
//        }

//        public void PanUp(float distance, Matrix4 objectMatrix)
//        {
//            var v = new Vector3();

//            if (scope.screenSpacePanning)
//            {
//                v.SetFromMatrixColumn(objectMatrix, 1);
//            }
//            else
//            {
//                v.SetFromMatrixColumn(objectMatrix, 0);
//                v.CrossVectors(scope._object.up, v);
//            }

//            v.MultiplyScalar(distance);
//            panOffset.add(v);
//        }

//        public void Pan(float deltaX, float deltaY)
//        {
//            var offset = new Vector3();
//            var element = scope.domElement;

//            if (scope._object is PerspectiveCamera perspectiveCamera) {
//                // perspective
//                var position = scope._object.Position;
//                offset.Copy(position).Sub(scope.target);
//                var targetDistance = offset.Length();

//                // half of the fov is center to top of screen
//                targetDistance *= Mathf.Tan((scope._object.fov / 2) * Mathf.PI / 180.0 );

//                // we use only clientHeight here so aspect ratio does not distort speed
//                PanLeft(2 * deltaX * targetDistance / element.clientHeight, scope.object.matrix);
//                PanUp(2 * deltaY * targetDistance / element.clientHeight, scope.object.matrix);
//            } else if (scope.object is OrthographicCamera orthographicCamera) {
//                // orthographic
//                PanLeft(deltaX * (scope.object.right - scope.object.left) / scope.object.zoom / element.clientWidth, scope.object.matrix );
//                PanUp(deltaY * (scope.object.top - scope.object.bottom) / scope.object.zoom / element.clientHeight, scope.object.matrix );
//            } 
//            else
//            {
//                // camera neither orthographic nor perspective
//                console.warn('WARNING: OrbitControls.js encountered an unknown camera type - pan disabled.');
//                scope.EnablePan = false;
//            }
//        }
//    }
//}


