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
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Services;
using Loxodon.Framework.Views;
using Loxodon.Log;
using System.Globalization;
using UnityEngine;

namespace Loxodon.Framework.Examples
{
    public class UIToolkitLauncher : MonoBehaviour
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(UIToolkitLauncher));

        private ApplicationContext context;
        void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);

            GlobalWindowManagerBase windowManager = FindObjectOfType<GlobalWindowManagerBase>();
            if (windowManager == null)
                throw new NotFoundException("Not found the GlobalWindowManager.");

            context = Context.GetApplicationContext();

            IServiceContainer container = context.GetContainer();

            /* Initialize the data binding service */
            BindingServiceBundle bundle = new BindingServiceBundle(context.GetContainer());
            bundle.Start();

            /* Initialize the ui view locator and register UIViewLocator */
            container.Register<IUIViewLocator>(new DefaultUIViewLocator());

            /* Initialize the localization service */
            //CultureInfo cultureInfo = Locale.GetCultureInfoByLanguage (SystemLanguage.English);
            CultureInfo cultureInfo = Locale.GetCultureInfo();
            var localization = Localization.Current;
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("LocalizationExamples", new XmlDocumentParser()));

            /* register Localization */
            container.Register<Localization>(localization);
        }

        async void Start()
        {
            /* Create a window container */
            WindowContainer winContainer = WindowContainer.Create("MAIN");

            await new WaitForEndOfFrame();

            IUIViewLocator locator = context.GetService<IUIViewLocator>();
            Window1 window = locator.LoadWindow<Window1>(winContainer, "UI/Window1");
            window.Create();
            ITransition transition = window.Show().OnStateChanged((w, state) =>
            {
                log.DebugFormat("Window:{0} State{1}", w.Name, state);
            });

            //await transition;

            //await new WaitForSeconds(3f);

            //Window2 window2 = locator.LoadWindow<Window2>(winContainer, "UI/Window2");
            //window2.Create();
            //await window2.Show();


            //AlertDialog.ShowMessage("测试", "标题", "OK", r => { });

            //await new WaitForSeconds(3f);

            //TestWindow window2 = locator.LoadWindow<TestWindow>(winContainer, "UI/TestWindow2");
            //window2.Create();
            //ITransition transition2 = window2.Show().OnStateChanged((w, state) =>
            //{
            //    //log.DebugFormat("Window:{0} State{1}",w.Name,state);
            //});

        }
    }
}