using System.Collections.Generic;

namespace ThreeJs4Net.Renderers.Shaders
{
    public class Uniforms : Dictionary<string, Uniform>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(Uniforms uniforms, string key, object value)
        {
            Uniform entry = null;
            if (uniforms.TryGetValue(key, out entry))
            {
                entry["value"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public Uniforms Copy(Uniforms original)
        {
            var destination = new Uniforms();

            foreach (var entry in original)
            {
                destination.Add(entry.Key, new Uniform().Copy(entry.Value));
            }

            return destination;
        }

    }
}
