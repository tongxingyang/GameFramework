using GameFramework.Debug;

namespace GameFramework.Res.Base
{
    public delegate void LoadSuccessCallback(string assetName, object asset, float duration, object userData);
    public delegate void LoadFailureCallback(string assetName, string errorMessage, object userData);

    public sealed class LoadCallback
    {

        public LoadCallback(LoadSuccessCallback loadSuccessCallback,
            LoadFailureCallback loadFailureCallback)
        {
            LoadSuccessCallback = loadSuccessCallback;
            LoadFailureCallback = loadFailureCallback;
        }

        public LoadSuccessCallback LoadSuccessCallback { get; set; }

        public LoadFailureCallback LoadFailureCallback { get; set; }
    }
}