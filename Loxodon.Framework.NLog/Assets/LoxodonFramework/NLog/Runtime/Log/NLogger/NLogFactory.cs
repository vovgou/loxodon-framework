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

using Loxodon.Log.NLogger.Directories;
using Loxodon.Log.NLogger.Targets;
using NLog;
using NLog.Config;
using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace Loxodon.Log.NLogger
{
    public class NLogFactory : ILogFactory
    {
        static NLogFactory()
        {
            ConfigurationItemFactory configurationFactory = ConfigurationItemFactory.Default;
            configurationFactory.LayoutRendererFactory.RegisterType<PersistentDataPathLayoutRenderer>("persistent-data-path");
            configurationFactory.LayoutRendererFactory.RegisterType<TemporaryCachePathLayoutRenderer>("temporary-cache-path");
            configurationFactory.TargetFactory.RegisterType<UnityConsoleTarget>("UnityConsole");
        }

        public static NLogFactory Load(XmlReader reader)
        {
            try
            {
                XmlLoggingConfiguration configuration = new XmlLoggingConfiguration(reader);
                NLog.LogManager.Configuration = configuration;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogErrorFormat("Failed to load NLog configuration file, default configuration will be used.exception:{0}", e);
                InitializeDefaultConfiguration();
            }
            return new NLogFactory(NLog.LogManager.LogFactory);
        }

        public static NLogFactory Load(string filename)
        {
            try
            {
                using (UnityWebRequest www = UnityWebRequest.Get(filename))
                {
                    www.SendWebRequest();
                    while (!www.isDone) { }
                    if (www.isNetworkError || www.isHttpError)
                        throw new Exception(www.error);

                    string text = www.downloadHandler.text;
                    using (XmlReader reader = XmlReader.Create(new StringReader(text)))
                    {
                        XmlLoggingConfiguration configuration = new XmlLoggingConfiguration(reader);
                        NLog.LogManager.Configuration = configuration;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogErrorFormat("Failed to load NLog configuration file from \"{0}\", default configuration will be used.exception:{1}", filename, e);
                InitializeDefaultConfiguration();
            }

            return new NLogFactory(NLog.LogManager.LogFactory);
        }

        public static NLogFactory LoadInResources(string filename)
        {
            try
            {
                string path = filename;
                TextAsset configText = Resources.Load<TextAsset>(path);
                if (configText == null)
                {
                    string extension = Path.GetExtension(path);
                    if (!string.IsNullOrEmpty(extension))
                        path = path.Replace(extension, "");
                    configText = Resources.Load<TextAsset>(path);
                }

                using (XmlReader reader = XmlReader.Create(new StringReader(configText.text)))
                {
                    return Load(reader);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogErrorFormat("Failed to load NLog configuration file from \"{0}\", default configuration will be used.exception:{1}", filename, e);
                InitializeDefaultConfiguration();
                return new NLogFactory(NLog.LogManager.LogFactory);
            }
        }

        private static void InitializeDefaultConfiguration()
        {
            LoggingConfiguration configuration = new LoggingConfiguration();
            var consoleTarget = new UnityConsoleTarget();
            consoleTarget.Layout = "${longdate} [${uppercase:${level}}] ${callsite}(${callsite-filename:includeSourcePath=False}:${callsite-linenumber}) - ${message} ${exception:format=ToString}";
            configuration.AddTarget("logconsole", consoleTarget);

            var rule = new LoggingRule("*", LogLevel.Info, consoleTarget);
            configuration.LoggingRules.Add(rule);
            NLog.LogManager.Configuration = configuration;
        }

        public static void Shutdown()
        {
            NLog.LogManager.Shutdown();
        }

        private readonly LogFactory logFactory;
        public NLogFactory(LogFactory logFactory)
        {
            this.logFactory = logFactory;
        }

        public ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public ILog GetLogger(Type type)
        {
            return logFactory.GetLogger<NLogLogImpl>(type.FullName);
        }

        public ILog GetLogger(string name)
        {
            return logFactory.GetLogger<NLogLogImpl>(name);
        }
    }
}
