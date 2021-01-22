using ThreeJs4Net.Core;

namespace ThreeJs4Net.Loaders
{
    public class JSONLoader
    {
        /// <summary>
        /// 
        /// </summary>
        public void Load(string url, int callback, string texturePath)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void Parse(string json, string texturePath)
        {
            var geometry = new Geometry();
            var scale = 1.0f;

            parseModel(scale);

            parseSkin();
            parseMorphing(scale);

            geometry.ComputeFaceNormals();
            geometry.ComputeBoundingSphere();
        
        }

        /// <summary>
        /// 
        /// </summary>
        public void parseModel(float scale)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void parseSkin()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void parseMorphing(float scale)
        {
        }
    }
}
