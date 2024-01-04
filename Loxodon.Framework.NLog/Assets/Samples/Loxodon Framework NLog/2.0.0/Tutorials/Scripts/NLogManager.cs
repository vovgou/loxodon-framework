using Loxodon.Log.NLogger;
using UnityEngine;

namespace Loxodon.Framework.Tutorials
{
    public class NLogManager : MonoBehaviour
    {
        void Awake()
        {
            ////Load the NLog configuration file from the StreamingAssets directory
            //Loxodon.Log.LogManager.Registry(NLogFactory.Load(Application.streamingAssetsPath + "/config.xml"));

            //Load the NLog configuration file from the Resources directory
            Loxodon.Log.LogManager.Registry(NLogFactory.LoadInResources("config"));

            DontDestroyOnLoad(this.gameObject);
        }

        void OnDestroy()
        {
            NLogFactory.Shutdown();
        }
    }
}
