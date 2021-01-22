using System.Collections;

namespace ThreeJs4Net.Loaders
{
    public class Cache
    {
        private readonly Hashtable files = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="file"></param>
        public void Add(object key, object file)
        {
            // Trace.TraceInformation( 'THREE.Cache', 'Adding key:', key );
            
            files[key] = file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(object key)
        {
            return this.files[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(object key)
        {
            files.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            files.Clear();
        }
    }
}
