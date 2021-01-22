using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using ThreeJs4Net.Core;
using ThreeJs4Net.Loaders;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Demo.examples.cs.loaders
{
    public class OBJLoader
    {
        class Geometry
        {
            public readonly List<float> Vertices = new List<float>();
            public readonly List<float> Normals = new List<float>();
            public readonly List<float> Uvs = new List<float>();
        }

        class Material
        {
            public string name;
        }

        class Objekt
        {
            public string name;

            public Geometry Geometry;

            public Material Material;
        }

        private readonly LoadingManager manager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manager"></param>
        public OBJLoader(LoadingManager manager)
        {
            this.manager = manager ?? Three.DefaultLoadingManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onLoad"></param>
        /// <param name="onProgress"></param>
        /// <param name="onError"></param>
        public void Load(string url, Action<Object3D> onLoad = null, Action onProgress = null, Action onError = null)
        {
            var loader = new XHRLoader(this.manager);
            //loader.setCrossOrigin(this.crossOrigin);
            loader.Load(url, text => {
                if (null != onLoad) 
                    onLoad(this.Parse(text));
            }, onProgress, onError);
        }

        private readonly List<float> _vertices = new List<float>();

        private readonly List<float> _normals = new List<float>();

        private readonly List<float> _uvs = new List<float>();

        private Objekt _object;

        private readonly List<Objekt> _objects = new List<Objekt>();

        private Geometry _geometry;

        private Material _material;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int? intParse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float floatParse(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        private Object3D Parse(string text)
        {
            // v float float float

		    var vertex_pattern = @"v( +[\d|\.|\+|\-|e|E]+)( +[\d|\.|\+|\-|e|E]+)( +[\d|\.|\+|\-|e|E]+)";

		    // vn float float float

            var normal_pattern = @"vn( +[\d|\.|\+|\-|e|E]+)( +[\d|\.|\+|\-|e|E]+)( +[\d|\.|\+|\-|e|E]+)";

		    // vt float float

		    var uv_pattern = @"vt( +[\d|\.|\+|\-|e|E]+)( +[\d|\.|\+|\-|e|E]+)";

		    // f vertex vertex vertex ...

		    var face_pattern1 = @"f( +-?\d+)( +-?\d+)( +-?\d+)( +-?\d+)?";

		    // f vertex/uv vertex/uv vertex/uv ...

		    var face_pattern2 = @"f( +(-?\d+)\/(-?\d+))( +(-?\d+)\/(-?\d+))( +(-?\d+)\/(-?\d+))( +(-?\d+)\/(-?\d+))?";

		    // f vertex/uv/normal vertex/uv/normal vertex/uv/normal ...

		    var face_pattern3 = @"f( +(-?\d+)\/(-?\d+)\/(-?\d+))( +(-?\d+)\/(-?\d+)\/(-?\d+))( +(-?\d+)\/(-?\d+)\/(-?\d+))( +(-?\d+)\/(-?\d+)\/(-?\d+))?";

		    // f vertex//normal vertex//normal vertex//normal ... 

		    var face_pattern4 = @"f( +(-?\d+)\/\/(-?\d+))( +(-?\d+)\/\/(-?\d+))( +(-?\d+)\/\/(-?\d+))( +(-?\d+)\/\/(-?\d+))?";


            var lines = text.Split('\n');

            for (var i = 0; i < lines.Length; i ++)
            {
          		var line = lines[ i ];
			    line = line.Trim();

                if ( line.Length == 0 || line[0] == '#' ) 
                {
				    continue;
			    }

                {
                    var rgx = new Regex(vertex_pattern, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(line);

                    foreach (Match match in matches)
                    {
                        this._vertices.AddRange( new [] { floatParse(match.Groups[1].Value), floatParse(match.Groups[2].Value), floatParse(match.Groups[3].Value) });
                    }

                    if (matches.Count > 0) continue;
                }

                {
                    var rgx = new Regex(normal_pattern, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(line);

                    foreach (Match match in matches)
                    {
                        this._normals.AddRange(new[] { floatParse(match.Groups[1].Value), floatParse(match.Groups[2].Value), floatParse(match.Groups[3].Value) });
                    }

                    if (matches.Count > 0) continue;
                }

                {
                    var rgx = new Regex(uv_pattern, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(line);

                    foreach (Match match in matches)
                    {
                        this._uvs.AddRange(new[] { floatParse(match.Groups[1].Value), floatParse(match.Groups[2].Value) });
                    }

                    if (matches.Count > 0) continue;
                }

                {
                    var rgx = new Regex(face_pattern1, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(line);

                    foreach (Match match in matches)
                    {
                        addFace(
                            intParse(match.Groups[1].Value), intParse(match.Groups[2].Value), intParse(match.Groups[3].Value), intParse(match.Groups[4].Value),
                            null, null, null, null,
                            null, null, null, null);
                    }

                    if (matches.Count > 0) continue;
                }

                {
                    var rgx = new Regex(face_pattern2, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(line);

                    foreach (Match match in matches)
                    {
                        addFace(
                            intParse(match.Groups[2].Value), intParse(match.Groups[5].Value), intParse(match.Groups[8].Value), intParse(match.Groups[11].Value),
                            intParse(match.Groups[3].Value), intParse(match.Groups[6].Value), intParse(match.Groups[9].Value), intParse(match.Groups[12].Value),
                            null, null, null, null);
                    }
                    
                    if (matches.Count > 0) continue;
                }

                {
                    var rgx = new Regex(face_pattern3, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(line);

                    foreach (Match match in matches)
                    {
                        addFace(
                            intParse(match.Groups[2].Value), intParse(match.Groups[6].Value), intParse(match.Groups[10].Value), intParse(match.Groups[14].Value),
                            intParse(match.Groups[3].Value), intParse(match.Groups[7].Value), intParse(match.Groups[11].Value), intParse(match.Groups[15].Value),
                            intParse(match.Groups[4].Value), intParse(match.Groups[8].Value), intParse(match.Groups[12].Value), intParse(match.Groups[16].Value));
                    }

                    if (matches.Count > 0) continue;
                }


                {
                    var rgx = new Regex(face_pattern4, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(line);

                    foreach (Match match in matches)
                    {
                        addFace(
                            intParse(match.Groups[2].Value), intParse(match.Groups[5].Value), intParse(match.Groups[8].Value), intParse(match.Groups[11].Value),
                            null, null, null, null,
                            intParse(match.Groups[3].Value), intParse(match.Groups[6].Value), intParse(match.Groups[9].Value), intParse(match.Groups[12].Value));
                    }

                    if (matches.Count > 0) continue;
                }

                if (line.Contains("o "))
                {
                    this._geometry = new Geometry();

                    this._material = new Material();

                    this._object = new Objekt { name = line.Substring(2, line.Length - 2).Trim(), Geometry = this._geometry, Material = this._material };

                    this._objects.Add(this._object);

                    continue;
                }

                if (line.Contains("g "))
                {
                    // group
                    continue;
                }

                if (line.Contains("usemtl "))
                {
                    // material

                    this._material.name = line.Substring(7, line.Length - 7).Trim();

                    continue;
                }

                if (line.Contains("mtllib "))
                {
                    // mtl file
                    continue;
                }

                if (line.Contains("s "))
                {
                    // // smooth shading
                    continue;
                }

                Trace.TraceInformation("OBJLoader: Unhandled line " + line);
            }

            var container = new Object3D();

            for ( var i = 0; i < this._objects.Count; i ++ )
            {
                var obj = this._objects[i];
			    var geom = obj.Geometry;

			    var buffergeometry = new BufferGeometry();

                buffergeometry.AddAttribute("position", new BufferAttribute<float>(geom.Vertices.ToArray(), 3));

			    if ( geom.Normals.Count > 0 )
			    {
			        buffergeometry.AddAttribute("normal", new BufferAttribute<float>(geom.Normals.ToArray(), 3));
			    }

			    if ( geom.Uvs.Count > 0 )
			    {
			        buffergeometry.AddAttribute("uv", new BufferAttribute<float>(geom.Uvs.ToArray(), 2));
			    }

			    var mat = new MeshLambertMaterial();
			    mat.Name = obj.Material.name;

			    var mesh = new Mesh( buffergeometry, mat );
			    mesh.Name = obj.name;

                container.Add(mesh);
		    }

            return container;
        }

        private int parseVertexIndex(int value)
        {
            var index = (int)(value);

            return (index >= 0 ? index - 1 : index + this._vertices.Count / 3) * 3;
        }

        private int parseNormalIndex(int value)
		{
            var index = (int)(value);

		    return (index >= 0 ? index - 1 : index + this._normals.Count / 3) * 3;
		}

        private int parseUVIndex(int value)
        {
            var index = (int)(value);

            return (index >= 0 ? index - 1 : index + this._uvs.Count / 2) * 2;
        }

		private void addVertex(int a, int b, int c )
		{
		    this._geometry.Vertices.Add(_vertices[a]);
		    this._geometry.Vertices.Add(_vertices[a + 1]);
		    this._geometry.Vertices.Add(_vertices[a + 2]);

		    this._geometry.Vertices.Add(_vertices[b]);
		    this._geometry.Vertices.Add(_vertices[b + 1]);
		    this._geometry.Vertices.Add(_vertices[b + 2]);

		    this._geometry.Vertices.Add(_vertices[c]);
		    this._geometry.Vertices.Add(_vertices[c + 1]);
		    this._geometry.Vertices.Add(_vertices[c + 2]);
		}

		private void addNormal( int a, int b, int c )
		{
            this._geometry.Normals.Add(_normals[a]);
            this._geometry.Normals.Add(_normals[a + 1]);
            this._geometry.Normals.Add(_normals[a + 2]);
                           
            this._geometry.Normals.Add(_normals[b]);
            this._geometry.Normals.Add(_normals[b + 1]);
            this._geometry.Normals.Add(_normals[b + 2]);
                           
            this._geometry.Normals.Add(_normals[c]);
            this._geometry.Normals.Add(_normals[c + 1]);
            this._geometry.Normals.Add(_normals[c + 2]);
		}

		private void addUV(int a, int b, int c)
		{
		    this._geometry.Uvs.Add(_uvs[a]);
		    this._geometry.Uvs.Add(_uvs[a + 1]);

		    this._geometry.Uvs.Add(_uvs[b]);
		    this._geometry.Uvs.Add(_uvs[b + 1]);

		    this._geometry.Uvs.Add(_uvs[c]);
		    this._geometry.Uvs.Add(_uvs[c + 1]);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="ua"></param>
        /// <param name="ub"></param>
        /// <param name="uc"></param>
        /// <param name="ud"></param>
        /// <param name="na"></param>
        /// <param name="nb"></param>
        /// <param name="nc"></param>
        /// <param name="nd"></param>
        private void addFace(int? a , int? b , int? c , int? d, int? ua , int? ub , int? uc , int? ud, int? na , int? nb , int? nc , int? nd)
        {
            var ia = parseVertexIndex(a.Value);
            var ib = parseVertexIndex(b.Value);
            var ic = parseVertexIndex(c.Value);

    		if ( d == null )
    		{
    		    addVertex(ia, ib, ic);
    		} 
            else
    		{
    		    var id = parseVertexIndex(d.Value);

                addVertex(ia, ib, id);
                addVertex(ib, ic, id);
            }

			if ( ua != null )
			{
			    ia = parseUVIndex(ua.Value);
			    ib = parseUVIndex(ub.Value);
			    ic = parseUVIndex(uc.Value);

				if ( d == null )
				{
				    addUV(ia, ib, ic);
				} 
                else
				{
				    var id = parseUVIndex(ud.Value);

				    addUV(ia, ib, id);
				    addUV(ib, ic, id);
				}
			}

            if ( na != null )
            {
                ia = parseNormalIndex(na.Value);
                ib = parseNormalIndex(nb.Value);
                ic = parseNormalIndex(nc.Value);

				if ( d == null ) {
					addNormal( ia, ib, ic );
				} 
                else
				{
				    var id = parseNormalIndex(nd.Value);

				    addNormal(ia, ib, id);
				    addNormal(ib, ic, id);
				}
			}
        }
    }
}
