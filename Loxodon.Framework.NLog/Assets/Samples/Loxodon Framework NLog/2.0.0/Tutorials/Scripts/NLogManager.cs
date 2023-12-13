using Loxodon.Log.NLogger;
using System.IO;
using System.Xml;
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
            TextAsset configText = Resources.Load<TextAsset>("config");
            if (configText != null)
            {
                using (XmlReader reader = XmlReader.Create(new StringReader(configText.text)))
                {
                    /* Initialize the Loxodon.Log.LogManager */
                    Loxodon.Log.LogManager.Registry(NLogFactory.Load(reader));
                }
            }

            DontDestroyOnLoad(this.gameObject);
        }

        void OnDestroy()
        {
            NLogFactory.Shutdown();
        }
    }
}
