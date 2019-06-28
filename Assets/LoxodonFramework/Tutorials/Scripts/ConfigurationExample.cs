using Loxodon.Framework.Configurations;
using System;
using UnityEngine;
namespace Loxodon.Framework.Tutorials
{
    public class ConfigurationExample : MonoBehaviour
    {
        private void Start()
        {
            TextAsset text = Resources.Load<TextAsset>("application.properties");
            IConfiguration conf = new PropertiesConfiguration(text.text);

            Version appVersion = conf.GetVersion("application.app.version");
            Version dataVersion = conf.GetVersion("application.data.version");

            Debug.LogFormat("application.app.version:{0}", appVersion);
            Debug.LogFormat("application.data.version:{0}", dataVersion);

            string groupName = conf.GetString("application.config-group");
            IConfiguration currentGroupConf = conf.Subset("application." + groupName);

            string upgradeUrl = currentGroupConf.GetString("upgrade.url");
            string username = currentGroupConf.GetString("username");
            string password = currentGroupConf.GetString("password");
            string[] gatewayArray = currentGroupConf.GetArray<string>("gateway");

            Debug.LogFormat("upgrade.url:{0}", upgradeUrl);
            Debug.LogFormat("username:{0}", username);
            Debug.LogFormat("password:{0}", password);

            int i = 1;
            foreach (string gateway in gatewayArray)
            {
                Debug.LogFormat("gateway {0}:{1}", i++, gateway);
            }
        }

    }
}
