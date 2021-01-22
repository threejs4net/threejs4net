using System;
using System.IO;

namespace ThreeJs4Net.Loaders
{
    public class XHRLoader
    {
        private readonly Cache cache;

        private readonly LoadingManager manager;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manager"></param>
        public XHRLoader(LoadingManager manager)
        {
            this.cache = new Cache();
            this.manager = manager ?? Three.DefaultLoadingManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="onLoad"></param>
        /// <param name="onProgress"></param>
        /// <param name="onError"></param>
        /// <returns></returns>
        public void Load(string uri, Action<string> onLoad = null, Action onProgress = null, Action onError = null)
        {
            var cached = this.cache.Get(uri);

            if (cached != null)
            {
                if (null != onLoad)
                    onLoad((string)cached);
                return;
            }

            this.manager.ItemStart(uri);

            var response = File.ReadAllText(uri);

            this.cache.Add(uri, response);

            if (null != onLoad) 
                onLoad(response);

            this.manager.ItemEnd(uri);
        }


    }
}
