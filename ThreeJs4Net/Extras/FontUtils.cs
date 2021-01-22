using System;
using System.Collections;
using System.Collections.Generic;

namespace ThreeJs4Net.Extras
{
    public class FontUtils
    {
        public static string face= "helvetiker";
               
        public static string weight= "normal";
               
        public static string style= "normal";
               
        public static int size= 150;
               
        public static int divisions= 10;

        public IList<object> faces = new List<object>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public object getFace(string data)
        {
            try
            {
        //        return this.faces[this.face][this.weight][this.style];
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public object loadFace(string data)
        {
            //var family = data.familyName.toLowerCase();

            //var ThreeFont = this;

            //ThreeFont.faces[ family ] = ThreeFont.faces[ family ] || {};

            //ThreeFont.faces[ family ][ data.cssFontWeight ] = ThreeFont.faces[ family ][ data.cssFontWeight ] || {};
            //ThreeFont.faces[ family ][ data.cssFontWeight ][ data.cssFontStyle ] = data;

            //var face = ThreeFont.faces[ family ][ data.cssFontWeight ][ data.cssFontStyle ] = data;

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public object drawText(string text)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="parameters"></param>
        public static object generateShapes(string text, Hashtable parameters = null)
        {
            parameters = parameters ?? null;
/*
            int size = (parameters["size"] != null) ? parameters["size"] : 100;
            var curveSegments = (parameters.curveSegments != null) ? parameters.curveSegments : 4;

            var font = parameters["font"] ?? "helvetiker";
            var weight = parameters["weight"] ?? "normal";
            var style = parameters["style"] ?? "normal";

            FontUtils.size = size;
            FontUtils.divisions = curveSegments;

            FontUtils.face = font;
            FontUtils.weight = weight;
            FontUtils.style = style;

            // Get a Font data json object

            var data = FontUtils.drawText( text );

            var paths = data.paths;
            var shapes = [];

            for ( var p = 0, pl = paths.length; p < pl; p ++ ) {
                Array.prototype.push.apply( shapes, paths[ p ].toShapes() );
            }
            return shapes;
 * */
            return null;
        }
    }
}
