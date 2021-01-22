using System;

namespace ThreeJs4Net.Loaders
{
    public class LoadingManager
    {
        private int total;

        private int loaded;

        public Action OnLoad;

        public Action OnProgress;

        public Action OnError;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onLoad"></param>
        /// <param name="onProgress"></param>
        /// <param name="onError"></param>
        public LoadingManager(Action onLoad = null, Action onProgress = null, Action onError = null)
        {
            this.OnLoad     = onLoad;
            this.OnProgress = onProgress;
            this.OnError    = onError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void ItemStart(string url)
        {
            this.total++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void ItemEnd(string url)
        {
            this.loaded++;

            if (this.OnProgress != null)
            {
                // this._onProgress(url, loaded, total);
            }
            if (this.loaded == this.total && this.OnLoad != null)
            {
                this.OnLoad();
            }
        }
    }
}
