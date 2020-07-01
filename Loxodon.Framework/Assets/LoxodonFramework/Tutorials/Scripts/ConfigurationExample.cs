/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

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
