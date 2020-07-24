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

using Loxodon.Framework.Asynchronous;
using Loxodon.Log;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using static UnityEngine.AddressableAssets.Addressables;

namespace Loxodon.Framework.Localizations
{
    /// <summary>
    /// Addressable data provider.
    /// It supports localized resources in ".Asset" file format.
    /// dir:
    /// root/default/
    /// 
    /// root/zh/
    /// root/zh-CN/
    /// root/zh-TW/
    /// root/zh-HK/
    /// 
    /// root/en/
    /// root/en-US/
    /// root/en-CA/
    /// root/en-AU/
    /// </summary>
    public class AddressableLocalizationSourceDataProvider : IDataProvider, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AddressableLocalizationSourceDataProvider));
        private List<AsyncOperationHandle<IList<LocalizationSourceAsset>>> handles;
        protected string[] filenames;
        protected IList<object> keys;
        protected string[] addresses;

        /// <summary>
        /// Load localized resources from Assetbundles based on asset label and file names.
        /// </summary>
        /// <param name="key">The label of asset in the AssetBundle.</param>
        /// <param name="filenames">The list of file names of localized resources to be loaded</param>
        public AddressableLocalizationSourceDataProvider(object key, string[] filenames)
            : this(new List<object>() { key }, filenames)
        {
        }

        /// <summary>
        /// Load localized resources from Assetbundles based on asset label and file names.
        /// </summary>
        /// <param name="keys">The label of asset in the AssetBundle.</param>
        /// <param name="filenames">The list of file names of localized resources to be loaded</param>
        public AddressableLocalizationSourceDataProvider(IList<object> keys, string[] filenames)
        {
            this.keys = keys ?? throw new ArgumentNullException("keys");
            this.filenames = filenames ?? throw new ArgumentNullException("filenames");
            this.handles = new List<AsyncOperationHandle<IList<LocalizationSourceAsset>>>();
        }

        /// <summary>
        /// Load localized resources from Assetbundles based on asset address.
        /// Please use the asset address of the default language, the loader will automatically replace the current language.
        /// </summary>
        /// <example>
        /// IDataProvider provider = new AddressableLocalizationSourceDataProvider("Assets/LoxodonFramework/Localizations/default/MainMenu.asset");
        /// </example>
        /// <param name="addresses">Addresses used to load asset at runtime.</param>
        public AddressableLocalizationSourceDataProvider(params string[] addresses)
        {
            this.addresses = addresses ?? throw new ArgumentNullException("addresses");
            this.handles = new List<AsyncOperationHandle<IList<LocalizationSourceAsset>>>();
        }

        public Task<Dictionary<string, object>> Load(CultureInfo cultureInfo)
        {
            if (this.keys != null)
                return LoadByKeys(cultureInfo, keys, filenames);
            else
                return LoadByAddress(cultureInfo, addresses);
        }

        protected virtual async Task<Dictionary<string, object>> LoadByKeys(CultureInfo cultureInfo, IList<object> keys, string[] filenames)
        {
            try
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                var locations = await Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Union, typeof(LocalizationSourceAsset));

                List<IResourceLocation> list = locations.ToList();
                List<IResourceLocation> defaultPaths = GetResourceLocations(list, "default", filenames);//eg:default
                List<IResourceLocation> twoLetterISOpaths = GetResourceLocations(list, cultureInfo.TwoLetterISOLanguageName, filenames); //eg:zh  en
                List<IResourceLocation> paths = cultureInfo.Name.Equals(cultureInfo.TwoLetterISOLanguageName) ? null : GetResourceLocations(list, cultureInfo.Name, filenames); //eg:zh-CN  en-US

                await FillData(dict, defaultPaths, cultureInfo);
                await FillData(dict, twoLetterISOpaths, cultureInfo);
                await FillData(dict, paths, cultureInfo);
                return dict;
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("An error occurred when loading localized data.Error:{0}", e);
                throw;
            }
        }

        protected virtual List<IResourceLocation> GetResourceLocations(List<IResourceLocation> list, string languageTag, string[] filenames = null)
        {
            string key = string.Format("/{0}/", languageTag);
            if (filenames != null && filenames.Length > 0)
                return list.FindAll(l => l.InternalId.Contains(key) && filenames.Contains(Path.GetFileName(l.PrimaryKey)));
            else
                return list.FindAll(l => l.InternalId.Contains(key));
        }

        protected virtual async Task<Dictionary<string, object>> LoadByAddress(CultureInfo cultureInfo, string[] addresses)
        {
            try
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                List<object> keys = new List<object>();
                foreach (string address in addresses)
                    AddAddress(keys, address, cultureInfo);

                var locations = await Addressables.LoadResourceLocationsAsync(keys, MergeMode.Union, typeof(LocalizationSourceAsset));
                List<IResourceLocation> list = locations.ToList();
                List<IResourceLocation> defaultPaths = GetResourceLocations(list, "default", filenames);//eg:default
                List<IResourceLocation> twoLetterISOpaths = GetResourceLocations(list, cultureInfo.TwoLetterISOLanguageName); //eg:zh  en
                List<IResourceLocation> paths = cultureInfo.Name.Equals(cultureInfo.TwoLetterISOLanguageName) ? null : GetResourceLocations(list, cultureInfo.Name, filenames); //eg:zh-CN  en-US

                await FillData(dict, defaultPaths, cultureInfo);
                await FillData(dict, twoLetterISOpaths, cultureInfo);
                await FillData(dict, paths, cultureInfo);
                return dict;
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("An error occurred when loading localized data.Error:{0}", e);
                throw;
            }
        }

        protected virtual void AddAddress(List<object> keys, string address, CultureInfo cultureInfo)
        {
            keys.Add(address);

            int end = address.LastIndexOf('/', address.Length - 1);
            int start = address.LastIndexOf('/', end - 1);
            string languageTag = address.Substring(start + 1, end - start - 1);

            keys.Add(address.Replace(string.Format("/{0}/", languageTag), string.Format("/{0}/", cultureInfo.TwoLetterISOLanguageName)));

            if (!cultureInfo.Name.Equals(cultureInfo.TwoLetterISOLanguageName))
                keys.Add(address.Replace(string.Format("/{0}/", languageTag), string.Format("/{0}/", cultureInfo.Name)));
        }

        protected virtual async Task FillData(Dictionary<string, object> dict, IList<IResourceLocation> paths, CultureInfo cultureInfo)
        {
            try
            {
                if (paths == null || paths.Count <= 0)
                    return;

                var result = Addressables.LoadAssetsAsync<LocalizationSourceAsset>(paths, null);
                IList<LocalizationSourceAsset> assets = await result;
                this.handles.Add(result);
                foreach (LocalizationSourceAsset asset in assets)
                {
                    try
                    {
                        MonolingualSource source = asset.Source;
                        if (source == null)
                            continue;

                        foreach (KeyValuePair<string, object> kv in source.GetData())
                        {
                            dict[kv.Key] = kv.Value;
                        }
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("An error occurred when loading localized data from \"{0}\".Error:{1}", asset.name, e);
                    }
                }
                Addressables.Release(result);
            }
            catch (Exception) { }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                foreach (var handle in handles)
                    Addressables.Release(handle);
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
