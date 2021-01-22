using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Policy;
using System.Text.RegularExpressions;
using ThreeJs4Net.Core;

namespace ThreeJs4Net.Demo.examples.cs.loaders
{
    public class BufferGeometryLoaderEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes A new instance of the <see cref="BufferGeometryLoaderEventArgs"/> class.
        /// </summary>
        /// <param name="bufferGeometry">
        /// The channel carrier.
        /// </param>
        public BufferGeometryLoaderEventArgs(BufferGeometry bufferGeometry)
        {
            this.BufferGeometry = bufferGeometry;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the channel carrier.
        /// </summary>
        public BufferGeometry BufferGeometry { get; private set; }

        #endregion
    }

    public class VTKLoader
    {
        public event EventHandler<BufferGeometryLoaderEventArgs> Loaded;

        protected virtual void RaiseLoaded(BufferGeometry geometry)
        {
            var handler = this.Loaded;
            if (handler != null)
            {
                handler(this, new BufferGeometryLoaderEventArgs(geometry));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void Load(Url url)
        {
            var geometry = this.Parse("");

            RaiseLoaded(geometry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var data = File.ReadAllText(filename);

            var geometry = this.Parse(data);

            stopWatch.Stop();

            Trace.TraceInformation("VTK model {0} loaded in {1} seconds", Path.GetFileName(filename), stopWatch.ElapsedMilliseconds / 1000.0f);

            RaiseLoaded(geometry);
        }

        /// <summary>
        /// 
        /// </summary>
        private BufferGeometry Parse(string data) 
        {
            var indices = new List<uint>();
            var positions = new List<float>();


                // float float float

            {
                string pattern = @"([\+|\-]?[\d]+[\.][\d|\-|e]+)[ ]+([\+|\-]?[\d]+[\.][\d|\-|e]+)[ ]+([\+|\-]?[\d]+[\.][\d|\-|e]+)";

                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                var matches = rgx.Matches(data);

                foreach (Match match in matches)
                {
                    // ["1.0 2.0 3.0", "1.0", "2.0", "3.0"]

                    var x = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    var y = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var z = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    positions.Add(x); positions.Add(y); positions.Add(z);
                }
            }

            // 3 int int int

            {
                string pattern = @"3[ ]+([\d]+)[ ]+([\d]+)[ ]+([\d]+)";

                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                var matches = rgx.Matches(data);

                foreach (Match match in matches)
                {
                    // ["3 1 2 3", "1", "2", "3"]
                    try
                    {
                        var a = uint.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                        var b = uint.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                        var c = uint.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                        indices.Add(a); indices.Add(b); indices.Add(c);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.Message);
                    }
                }
            }

            // 4 int int int int

            {
                string pattern = @"4[ ]+([\d]+)[ ]+([\d]+)[ ]+([\d]+)[ ]+([\d]+)";

                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                var matches = rgx.Matches(data);

                foreach (Match match in matches)
                {
                    // ["4 1 2 3 4", "1", "2", "3", "4"]

                    var a = uint.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    var b = uint.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var c = uint.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    var d = uint.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    indices.Add(a); indices.Add(b); indices.Add(d);
                    indices.Add(b); indices.Add(c); indices.Add(d);
                }
            }

            var geometry = new BufferGeometry();
            geometry.AddAttribute( "index", new BufferAttribute<uint>( indices.ToArray(), 1 ) );
            geometry.AddAttribute( "position", new BufferAttribute<float>( positions.ToArray(), 3 ) );

            return geometry;
        }

    }
}
