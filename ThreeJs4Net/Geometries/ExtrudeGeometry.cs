using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ThreeJs4Net.Core;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Math;
using ThreeJs4Net.Renderers.Shaders;

namespace ThreeJs4Net.Geometries
{
    public class ExtrudeGeometry : Geometry
    {
        //this.parameters = {
        //shapes: shapes,
        //	options: options
        //};

        public ExtrudeGeometry(Shape[] shapes, Hashtable options = null)
        {
            this.FromBufferGeometry(new ExtrudeBufferGeometry(shapes, options));
            this.MergeVertices();
        }
    }


    public class ExtrudeBufferGeometry : BufferGeometry
    {
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        private Hashtable options;
        private List<float> verticesArray;
        private List<float> uvArray;

        public ExtrudeBufferGeometry(Shape[] shapes, Hashtable options = null) : base()
        {
            this.options = options;
            //parameters.Add("shapes", shapes);
            //         parameters.Add("shapes", options);


            var verticesArray = new List<float>();
            var uvArray = new List<float>();

            for (var i = 0; i < shapes.Length; i++)
            {
                var shape = shapes[i];
                AddShape(shape);
            }

            // build geometry
            this.SetAttribute("position", new BufferAttribute<float>(verticesArray.ToArray(), 3));
            this.SetAttribute("uv", new BufferAttribute<float>(uvArray.ToArray(), 2));
            this.ComputeVertexNormals();
        }

        private void AddShape(Shape shape)
        {
            var placeholder = new List<float>();

            // options

            var curveSegments = options.ContainsKey("curveSegments") ? (int)options["curveSegments"] : 12;
            var steps = options.ContainsKey("steps") ? (int)options["steps"] : 1;
            var depth = options.ContainsKey("depth") ? (int)options["depth"] : 100;

            var bevelEnabled = options.ContainsKey("bevelEnabled") ? (bool)options["bevelEnabled"] : true;
            var bevelThickness = options.ContainsKey("bevelThickness") ? (int)options["bevelThickness"] : 6;
            var bevelSize = options.ContainsKey("bevelSize") ? (int)options["bevelSize"] : bevelThickness - 2;
            var bevelOffset = options.ContainsKey("bevelOffset") ? (int)options["bevelOffset"] : 0;
            var bevelSegments = options.ContainsKey("bevelSegments") ? (int)options["bevelSegments"] : 3;

            Curve<Vector3> extrudePath = options["extrudePath"] as Curve<Vector3>;

            var uvgen = options.ContainsKey("UVGenerator") ? options["UVGenerator"] : WorldUVGenerator;

            // deprecated options
            if (options.ContainsKey("amount"))
            {
                depth = (int)options["amount"];
            }

            List<Vector3> extrudePts = new List<Vector3>();
            bool extrudeByPath = false;
            Curve<Vector3>.TenetFrames splineTube = new Curve<Vector3>.TenetFrames;
            Vector3 binormal = new Vector3();
            Vector3 normal = new Vector3();
            Vector3 position2 = new Vector3();

            if (extrudePath != null)
            {
                extrudePts = extrudePath.GetSpacedPoints(steps);
                extrudeByPath = true;
                bevelEnabled = false; // bevels not supported for path extrusion

                // SETUP TNB variables
                // TODO1 - have a .isClosed in spline?
                splineTube = extrudePath.ComputeFrenetFrames(steps, false);

                // console.log(splineTube, 'splineTube', splineTube.normals.length, 'steps', steps, 'extrudePts', extrudePts.length);
                binormal = new Vector3();
                normal = new Vector3();
                position2 = new Vector3();
            }

            // Safeguards if bevels are not enabled
            if (!bevelEnabled)
            {
                bevelSegments = 0;
                bevelThickness = 0;
                bevelSize = 0;
                bevelOffset = 0;
            }

            // Variables initialization
            var ahole = new List<Vector2>();
            //var hl; // looping of holes
            var shapePoints = shape.ExtractPoints(curveSegments);
            var vertices = shapePoints.shape;
            var holes = shapePoints.holes;
            var reverse = !ShapeUtils.isClockWise(vertices);

            if (reverse)
            {
                vertices.Reverse();
                // Maybe we should also check if holes are in the opposite direction, just to be safe ...
                for (var h = 0; h < holes.Count; h++)
                {
                    ahole = holes[h].ToList();
                    if (ShapeUtils.isClockWise(ahole))
                    {
                        var temp = ahole;
                        ahole.Reverse();
                        holes[h] = ahole.ToArray();
                    }
                }
            }

            var faces = ShapeUtils.TriangulateShape(vertices.ToArray(), holes);
            /* Vertices */
            var contour = vertices; // vertices has all points but contour has only points of circumference
            for (var h = 0; h < holes.Count; h++)
            {
                ahole = holes[h].ToList();
                vertices.AddRange(ahole);
            }

            Vector2 scalePt2(Vector2 pt, Vector2 vec, float size)
            {
                if (vec == null)
                {
                    throw new Exception("THREE.ExtrudeGeometry: vec does not exist");
                }
                return vec.Clone().MultiplyScalar(size).Add(pt);
            }

            //var b, bs, t, z, face, vert;
            var vlen = vertices.Count;
            var flen = faces.Count;

            // Find directions for point movement
            Vector2 getBevelVec(Vector2 inPt, Vector2 inPrev, Vector2 inNext)
            {
                // computes for inPt the corresponding point inPt' on a new contour
                //   shifted by 1 unit (length of normalized vector) to the left
                // if we walk along contour clockwise, this new contour is outside the old one
                //
                // inPt' is the intersection of the two lines parallel to the two
                //  adjacent edges of inPt at a distance of 1 unit on the left side.

                float v_trans_x, v_trans_y, shrink_by; // resulting translation vector for inPt

                // good reading for geometry algorithms (here: line-line intersection)
                // http://geomalgorithms.com/a05-_intersect-1.html

                float v_prev_x = inPt.X - inPrev.X;
                float v_prev_y = inPt.Y - inPrev.Y;
                float v_next_x = inNext.X - inPt.X;
                float v_next_y = inNext.Y - inPt.Y;

                float v_prev_lensq = (v_prev_x * v_prev_x + v_prev_y * v_prev_y);

                // check for collinear edges
                var collinear0 = (v_prev_x * v_next_y - v_prev_y * v_next_x);

                if (Mathf.Abs(collinear0) > MathUtils.EPS5)
                {
                    // not collinear
                    // length of vectors for normalizing
                    var v_prev_len = Mathf.Sqrt(v_prev_lensq);
                    var v_next_len = Mathf.Sqrt(v_next_x * v_next_x + v_next_y * v_next_y);

                    // shift adjacent points by unit vectors to the left

                    var ptPrevShift_x = (inPrev.X - v_prev_y / v_prev_len);
                    var ptPrevShift_y = (inPrev.Y + v_prev_x / v_prev_len);

                    var ptNextShift_x = (inNext.X - v_next_y / v_next_len);
                    var ptNextShift_y = (inNext.Y + v_next_x / v_next_len);

                    // scaling factor for v_prev to intersection point

                    var sf = ((ptNextShift_x - ptPrevShift_x) * v_next_y -
                            (ptNextShift_y - ptPrevShift_y) * v_next_x) /
                        (v_prev_x * v_next_y - v_prev_y * v_next_x);

                    // vector from inPt to intersection point

                    v_trans_x = (ptPrevShift_x + v_prev_x * sf - inPt.X);
                    v_trans_y = (ptPrevShift_y + v_prev_y * sf - inPt.Y);

                    // Don't normalize!, otherwise sharp corners become ugly
                    //  but prevent crazy spikes
                    var v_trans_lensq = (v_trans_x * v_trans_x + v_trans_y * v_trans_y);
                    if (v_trans_lensq <= 2)
                    {
                        return new Vector2(v_trans_x, v_trans_y);
                    }
                    else
                    {
                        shrink_by = Mathf.Sqrt(v_trans_lensq / 2);
                    }
                }
                else
                {
                    // handle special case of collinear edges
                    var direction_eq = false; // assumes: opposite
                    if (v_prev_x > MathUtils.EPS5)
                    {
                        if (v_next_x > MathUtils.EPS5)
                        {
                            direction_eq = true;
                        }
                    }
                    else
                    {
                        if (v_prev_x < -MathUtils.EPS5)
                        {
                            if (v_next_x < -MathUtils.EPS5)
                            {
                                direction_eq = true;
                            }
                        }
                        else
                        {
                            if (Mathf.Sign(v_prev_y) == Mathf.Sign(v_next_y))
                            {
                                direction_eq = true;
                            }
                        }
                    }

                    if (direction_eq)
                    {
                        // console.log("Warning: lines are a straight sequence");
                        v_trans_x = -v_prev_y;
                        v_trans_y = v_prev_x;
                        shrink_by = Mathf.Sqrt(v_prev_lensq);
                    }
                    else
                    {
                        // console.log("Warning: lines are a straight spike");
                        v_trans_x = v_prev_x;
                        v_trans_y = v_prev_y;
                        shrink_by = Mathf.Sqrt(v_prev_lensq / 2);
                    }
                }
                return new Vector2(v_trans_x / shrink_by, v_trans_y / shrink_by);
            }

            var contourMovements = new List<Vector2>();

            var i = 0;
            var il = contour.Count;
            var j = il - 1;
            var k = i + 1;
            for (i = 0; i < contour.Count; i++, j++, k++)
            {
                if (j == il) j = 0;
                if (k == il) k = 0;

                //  (j)---(i)---(k)
                // console.log('i,j,k', i, j , k)
                contourMovements[i] = getBevelVec(contour[i], contour[j], contour[k]);
            }

            var holesMovements = [];
            List<Vector2> oneHoleMovements;
            List<Vector2> verticesMovements = new List<Vector2>();
            verticesMovements.AddRange(contourMovements);

            for (var h = 0; h < holes.Count; h++)
            {
                ahole = holes[h].ToList();
                oneHoleMovements = new List<Vector2>();
                for (i = 0, il = ahole.Count, j = il - 1, k = i + 1; i < il; i++, j++, k++)
                {
                    if (j == il) j = 0;
                    if (k == il) k = 0;
                    //  (j)---(i)---(k)
                    oneHoleMovements[i] = getBevelVec(ahole[i], ahole[j], ahole[k]);
                }

                holesMovements.push(oneHoleMovements);
                verticesMovements.AddRange(oneHoleMovements);
            }

            // Loop bevelSegments, 1 for the front, 1 for the back
            for (var b = 0; b < bevelSegments; b++)
            {
                //for ( b = bevelSegments; b > 0; b -- ) {
                var t = b / bevelSegments;
                var z = bevelThickness * Mathf.Cos(t * Mathf.PI / 2);
                var bs = bevelSize * Mathf.Sin(t * Mathf.PI / 2) + bevelOffset;

                // contract shape
                for (i = 0, il = contour.Count; i < il; i++)
                {
                    var vert = scalePt2(contour[i], contourMovements[i], bs);
                    v(vert.X, vert.Y, -z);
                }

                // expand holes
                for (var h = 0; h < holes.Count; h++)
                {
                    ahole = holes[h].ToList();
                    oneHoleMovements = holesMovements[h];
                    for (i = 0, il = ahole.Count; i < il; i++)
                    {
                        var vert = scalePt2(ahole[i], oneHoleMovements[i], bs);
                        v(vert.X, vert.Y, -z);
                    }
                }
            }

            float bs = bevelSize + bevelOffset;

            // Back facing vertices

            for (i = 0; i < vlen; i++)
            {
                var vert = bevelEnabled ? scalePt2(vertices[i], verticesMovements[i], bs) : vertices[i];
                if (!extrudeByPath)
                {
                    v(vert.X, vert.Y, 0);
                }
                else
                {
                    // v( vert.x, vert.y + extrudePts[ 0 ].y, extrudePts[ 0 ].x );
                    normal.Copy(splineTube.normals[0]).MultiplyScalar(vert.X);
                    binormal.Copy(splineTube.binormals[0]).MultiplyScalar(vert.Y);
                    position2.Copy(extrudePts[0]).Add(normal).Add(binormal);
                    v(position2.X, position2.Y, position2.Z);
                }
            }

            // Add stepped vertices...
            // Including front facing vertices
            for (var s = 1; s <= steps; s++)
            {
                for (i = 0; i < vlen; i++)
                {
                    var vert = bevelEnabled ? scalePt2(vertices[i], verticesMovements[i], bs) : vertices[i];
                    if (!extrudeByPath)
                    {
                        v(vert.X, vert.Y, depth / steps * s);
                    }
                    else
                    {
                        // v( vert.x, vert.y + extrudePts[ s - 1 ].y, extrudePts[ s - 1 ].x );
                        normal.Copy(splineTube.normals[s]).MultiplyScalar(vert.X);
                        binormal.Copy(splineTube.binormals[s]).MultiplyScalar(vert.Y);
                        position2.Copy(extrudePts[s]).Add(normal).Add(binormal);
                        v(position2.X, position2.Y, position2.Z);
                    }
                }
            }

            // Add bevel segments planes
            //for ( b = 1; b <= bevelSegments; b ++ ) {
            for (var b = bevelSegments - 1; b >= 0; b--)
            {
                var t = b / bevelSegments;
                var z = bevelThickness * Mathf.Cos(t * Mathf.PI / 2);
                bs = bevelSize * Mathf.Sin(t * Mathf.PI / 2) + bevelOffset;
                // contract shape
                for (i = 0, il = contour.Count; i < il; i++)
                {
                    var vert = scalePt2(contour[i], contourMovements[i], bs);
                    v(vert.X, vert.Y, depth + z);
                }

                // expand holes
                for (var h = 0; h < holes.Count; h++)
                {
                    ahole = holes[h].ToList();
                    oneHoleMovements = holesMovements[h];
                    for (i = 0, il = ahole.Count; i < il; i++)
                    {
                        var vert = scalePt2(ahole[i], oneHoleMovements[i], bs);
                        if (!extrudeByPath)
                        {
                            v(vert.X, vert.Y, depth + z);
                        }
                        else
                        {
                            v(vert.X, vert.Y + extrudePts[steps - 1].Y, extrudePts[steps - 1].X + z);
                        }
                    }
                }
            }

            /* Faces */
            // Top and bottom faces
            buildLidFaces();
            // Sides faces
            buildSideFaces();


            /////  Internal functions
            void buildLidFaces()
            {
                var start = verticesArray.Count / 3;
                if (bevelEnabled)
                {
                    var layer = 0; // steps + 1
                    var offset = vlen * layer;
                    // Bottom faces
                    for (i = 0; i < flen; i++)
                    {
                        var face = faces[i];
                        f3(face[2] + offset, face[1] + offset, face[0] + offset);
                    }
                    layer = steps + bevelSegments * 2;
                    offset = vlen * layer;
                    // Top faces
                    for (i = 0; i < flen; i++)
                    {
                        var face = faces[i];
                        f3(face[0] + offset, face[1] + offset, face[2] + offset);
                    }
                }
                else
                {
                    // Bottom faces
                    for (i = 0; i < flen; i++)
                    {
                        var face = faces[i];
                        f3(face[2], face[1], face[0]);
                    }

                    // Top faces
                    for (i = 0; i < flen; i++)
                    {
                        var face = faces[i];
                        f3(face[0] + vlen * steps, face[1] + vlen * steps, face[2] + vlen * steps);
                    }
                }

                scope.addGroup(start, verticesArray.Count / 3 - start, 0);
            }

            // Create faces for the z-sides of the shape

            void buildSideFaces()
            {
                var start = verticesArray.Count / 3;
                var layeroffset = 0;
                sidewalls(contour, layeroffset);
                layeroffset += contour.Count;

                for (var h = 0; h < holes.Count; h++)
                {
                    ahole = holes[h].ToList();
                    sidewalls(ahole, layeroffset);
                    //, true
                    layeroffset += ahole.Count;
                }

                scope.addGroup(start, verticesArray.Count / 3 - start, 1);
            }

            void sidewalls(List<Vector2> contour, int layeroffset)
            {
                var i = contour.Count;
                while (--i >= 0)
                {
                    j = i;
                    k = i - 1;
                    if (k < 0) k = contour.Count - 1;
                    //console.log('b', i,j, i-1, k,vertices.length);
                    var s = 0;
                    var sl = steps + bevelSegments * 2;
                    for (s = 0; s < sl; s++)
                    {
                        var slen1 = vlen * s;
                        var slen2 = vlen * (s + 1);
                        var a = layeroffset + j + slen1;
                        var b = layeroffset + k + slen1;
                        var c = layeroffset + k + slen2;
                        var d = layeroffset + j + slen2;
                        f4(a, b, c, d);
                    }
                }
            }

            void v(float x, float y, float z)
            {
                placeholder.Add(x);
                placeholder.Add(y);
                placeholder.Add(z);
            }


            void f3(int a, int b, int c)
            {
                addVertex(a);
                addVertex(b);
                addVertex(c);

                var nextIndex = verticesArray.Count / 3;
                var uvs = uvgen.generateTopUV(scope, verticesArray, nextIndex - 3, nextIndex - 2, nextIndex - 1);

                addUV(uvs[0]);
                addUV(uvs[1]);
                addUV(uvs[2]);
            }

            void f4(int a, int b, int c, int d)
            {
                addVertex(a);
                addVertex(b);
                addVertex(d);
                addVertex(b);
                addVertex(c);
                addVertex(d);
                var nextIndex = verticesArray.Count / 3;
                var uvs = uvgen.generateSideWallUV(scope, verticesArray, nextIndex - 6, nextIndex - 3, nextIndex - 2, nextIndex - 1);
                addUV(uvs[0]);
                addUV(uvs[1]);
                addUV(uvs[3]);
                addUV(uvs[1]);
                addUV(uvs[2]);
                addUV(uvs[3]);
            }

            void addVertex(int index)
            {
                verticesArray.Add(placeholder[index * 3 + 0]);
                verticesArray.Add(placeholder[index * 3 + 1]);
                verticesArray.Add(placeholder[index * 3 + 2]);
            }

            void addUV(Vector2 vector2)
            {
                uvArray.Add(vector2.X);
                uvArray.Add(vector2.Y);
            }
        }
    }
}

//}


//var WorldUVGenerator = {

//	generateTopUV: function(geometry, vertices, indexA, indexB, indexC) {

//	var a_x = vertices[indexA * 3];
//	var a_y = vertices[indexA * 3 + 1];
//	var b_x = vertices[indexB * 3];
//	var b_y = vertices[indexB * 3 + 1];
//	var c_x = vertices[indexC * 3];
//	var c_y = vertices[indexC * 3 + 1];

//	return [
//		new Vector2(a_x, a_y),
//		new Vector2(b_x, b_y),
//		new Vector2(c_x, c_y)
//	];

//},

//	generateSideWallUV: function(geometry, vertices, indexA, indexB, indexC, indexD) {

//	var a_x = vertices[indexA * 3];
//	var a_y = vertices[indexA * 3 + 1];
//	var a_z = vertices[indexA * 3 + 2];
//	var b_x = vertices[indexB * 3];
//	var b_y = vertices[indexB * 3 + 1];
//	var b_z = vertices[indexB * 3 + 2];
//	var c_x = vertices[indexC * 3];
//	var c_y = vertices[indexC * 3 + 1];
//	var c_z = vertices[indexC * 3 + 2];
//	var d_x = vertices[indexD * 3];
//	var d_y = vertices[indexD * 3 + 1];
//	var d_z = vertices[indexD * 3 + 2];

//	if (Math.abs(a_y - b_y) < 0.01)
//	{
//		return [
//			new Vector2(a_x, 1 - a_z),
//			new Vector2(b_x, 1 - b_z),
//			new Vector2(c_x, 1 - c_z),
//			new Vector2(d_x, 1 - d_z)
//		];
//	}
//	else
//	{
//		return [
//			new Vector2(a_y, 1 - a_z),
//			new Vector2(b_y, 1 - b_z),
//			new Vector2(c_y, 1 - c_z),
//			new Vector2(d_y, 1 - d_z)
//		];
//	}
//}
//};

//function toJSON(shapes, options, data )
//{
//	data.shapes = [];
//	if (Array.isArray(shapes))
//	{
//		for (var i = 0, l = shapes.length; i < l; i++)
//		{
//			var shape = shapes[i];
//			data.shapes.push(shape.uuid);
//		}
//	}
//	else
//	{
//		data.shapes.push(shapes.uuid);
//	}
//	//
//	if (options.extrudePath !== undefined) data.options.extrudePath = options.extrudePath.toJSON();
//	return data;
//}


//export
//{ ExtrudeGeometry, ExtrudeBufferGeometry };

