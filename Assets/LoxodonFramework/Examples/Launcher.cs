using UnityEngine;
using System.Globalization;
using System.Collections;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Log;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Services;

namespace Loxodon.Framework.Examples
{
	public class Launcher : MonoBehaviour
    {

        //private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private ApplicationContext context;
		void Awake()
		{           
			GlobalWindowManager windowManager = FindObjectOfType<GlobalWindowManager>();
			if (windowManager == null)
				throw new NotFoundException("Can't find the GlobalWindowManager.");
			
			context = Context.GetApplicationContext();

			IServiceContainer container = context.GetContainer();

			/* Initialize the data binding service */
			BindingServiceBundle bundle = new BindingServiceBundle(context.GetContainer());
			bundle.Start();

			/* Initialize the ui view locator and register UIViewLocator */
			container.Register<IUIViewLocator>(new ResourcesViewLocator ());

			/* Initialize the localization service */
			//		CultureInfo cultureInfo = Locale.GetCultureInfoByLanguage (SystemLanguage.English);
			CultureInfo cultureInfo = Locale.GetCultureInfo();
			Localization.Current = Localization.Create(new ResourcesDataProvider("LocalizationExamples", new XmlDocumentParser()), cultureInfo);

			/* register Localization */
			container.Register<Localization>(Localization.Current);

			/* register AccountRepository */
			IAccountRepository accountRepository = new AccountRepository();
			container.Register<IAccountService>(new AccountService(accountRepository));
		}

        IEnumerator Start()
        {
            /* Create a window container */
            WindowContainer winContainer = WindowContainer.Create("MAIN");

            yield return null;

            IUIViewLocator locator = context.GetService<IUIViewLocator>();
            StartupWindow window = locator.LoadWindow<StartupWindow>(winContainer, "UI/Startup/Startup");
            window.Create();
            ITransition transition = window.Show().OnStateChanged((w, state) =>
            {
                //log.DebugFormat("Window:{0} State{1}",w.Name,state);
            });

            yield return transition.WaitForDone();
        }
    }
}