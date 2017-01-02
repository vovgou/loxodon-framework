# Loxodon Framework

MVVM Framework for Unity3D

Developed by Clark

Requires Unity 5.3.0 or higher.

LoxodonFramework is a lightweight MVVM(Model-View-ViewModel) framework built specifically to target Unity3D.
Databinding and localization are supported.It has a very flexible extensibility.It makes your game development faster and easier.

For tutorials,examples and support,please see the project.You can also discuss the project in the Unity Forums.

The Free version is compatible with MacOSX,Windows,Linux and Android.Check out the Pro version if you want more platforms and full source code.

Tested in Unity 3D on the following platforms:
PC/Mac/Linux
Android

IL2CPP is supported, however there are some gotchas.

## Release Download

- [Loxodon Framework Pro](https://www.assetstore.unity3d.com/#!/content/79304)
- [Loxodon Framework Free](https://www.assetstore.unity3d.com/#!/content/77446)

## Key features:
- Mvvm Framework;
- Multiple platforms;
- Higher Extensibility;
- Asynchronous result and asynchronous task are supported;
- Scheduled Executor and Multi-threading;<br>
- Messaging system support;
- Preferences can be encrypted;
- Localization support;
- Databinding support:
    - Field binding;
    - Property binding;
    - Dictinary,list and array binding;
    - Event binding;
    - Unity3d's EventBase binding;
    - Static property and field binding;
    - Method binding;
    - Command binding;
    - ObservableProperty,ObservableDictinary and ObservableList binding;
    
## Examples
```C#

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Loxodon.Framework.Contexts;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using Loxodon.Framework.ViewModels;

using Loxodon.Framework.Localizations;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Tutorials
{
	public class Account : ObservableObject
	{
		private int id;
		private string username;
		private string password;
		private string email;
		private DateTime birthday;
		private readonly ObservableProperty<string> address = new ObservableProperty<string> ();

		public int ID {
			get { return this.id; }
			set { this.Set<int> (ref this.id, value, "ID"); }
		}

		public string Username {
			get { return this.username; }
			set { this.Set<string> (ref this.username, value, "Username"); }
		}

		public string Password {
			get { return this.password; }
			set { this.Set<string> (ref this.password, value, "Password"); }
		}

		public string Email {
			get{ return this.email; }
			set{ this.Set<string> (ref this.email, value, "Email"); }
		}

		public DateTime Birthday {
			get{ return this.birthday; }
			set{ this.Set<DateTime> (ref this.birthday, value, "Birthday"); }
		}

		public ObservableProperty<string> Address {
			get { return this.address; }
		}
	}

	public class AccountViewModel : ViewModelBase
	{
		private Account account;

		private string username;
		private string email;
		private ObservableDictionary<string, string> errors = new ObservableDictionary<string, string> ();

		public Account Account {
			get { return this.account; }
			set { this.Set<Account> (ref account, value, "Account"); }
		}

		public string Username {
			get { return this.username; }
			set { this.Set<string> (ref this.username, value, "Username"); }
		}

		public string Email {
			get{ return this.email; }
			set{ this.Set<string> (ref this.email, value, "Email"); }
		}

		public ObservableDictionary<string, string> Errors {
			get { return this.errors; }
			set { this.Set<ObservableDictionary<string, string>> (ref this.errors, value, "Errors"); }
		}

		public void OnSubmit ()
		{
			if (string.IsNullOrEmpty (this.Username) || !Regex.IsMatch (this.Username, "^[a-zA-Z0-9_-]{4,12}$")) {
				this.errors ["errorMessage"] = "Please enter a valid username.";
				return;
			}

			if (string.IsNullOrEmpty (this.Email) || !Regex.IsMatch (this.Email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")) {
				this.errors ["errorMessage"] = "Please enter a valid email.";
				return;
			}

			this.errors.Clear ();
			this.Account.Username = this.Username;
			this.Account.Email = this.Email;
		}
	}

	public class DatabindingExample : UIView
	{
		public Text title;
		public Text username;
		public Text password;
		public Text email;
		public Text birthday;
		public Text address;

		public Text errorMessage;

		public InputField usernameEdit;
		public InputField emailEdit;
		public Button submit;

		void Awake ()
		{
			ApplicationContext context = Context.GetApplicationContext ();
			BindingServiceBundle bindingService = new BindingServiceBundle (context.GetContainer());
			bindingService.Start ();


			CultureInfo cultureInfo = Locale.GetCultureInfo ();
			Localization.Current = Localization.Create (new DefaultDataProvider ("LocalizationTutorials", new XmlDocumentParser ()), cultureInfo);

		}

		void Start ()
		{
			Account account = new Account () {
				ID = 1,
				Username = "test",
				Password = "test",
				Email = "clark_ya@163.com",
				Birthday = new DateTime (2000, 3, 3)
			};
			account.Address.Value = "beijing";

			AccountViewModel accountViewModel = new AccountViewModel () {
				Account = account
			};

			IBindingContext bindingContext = this.BindingContext ();
			bindingContext.DataContext = accountViewModel;

			/* databinding */
			BindingSet<DatabindingExample, AccountViewModel> bindingSet = this.CreateBindingSet<DatabindingExample, AccountViewModel> ();
//			bindingSet.Bind (this.username).For ("text").To ("Account.Username").OneWay ();
//			bindingSet.Bind (this.password).For ("text").To ("Account.Password").OneWay ();
			bindingSet.Bind (this.username).For (v => v.text).To (vm => vm.Account.Username).OneWay ();
			bindingSet.Bind (this.password).For (v => v.text).To (vm => vm.Account.Password).OneWay ();
			bindingSet.Bind (this.email).For (v => v.text).To (vm => vm.Account.Email).OneWay ();
			bindingSet.Bind (this.birthday).For (v => v.text).ToExpression (vm => string.Format ("{0} ({1})", 
                vm.Account.Birthday.ToString("yyyy-MM-dd"), (DateTime.Now.Year - vm.Account.Birthday.Year))).OneWay ();

			bindingSet.Bind (this.address).For (v => v.text).To (vm => vm.Account.Address).OneWay ();

			bindingSet.Bind (this.errorMessage).For (v => v.text).To (vm => vm.Errors ["errorMessage"]).OneWay ();

			bindingSet.Bind (this.usernameEdit).For (v => v.text, v => v.onEndEdit).To (vm => vm.Username).TwoWay ();
			bindingSet.Bind (this.emailEdit).For (v => v.text, v => v.onEndEdit).To (vm => vm.Email).TwoWay ();
			bindingSet.Bind (this.submit).For (v => v.onClick).To ("OnSubmit");
			bindingSet.Build ();

			BindingSet<DatabindingExample> staticBindingSet = this.CreateBindingSet<DatabindingExample> ();
			staticBindingSet.Bind (this.title).For (v => v.text).To (() => Res.databinding_tutorials_title).OneTime ();
			staticBindingSet.Build ();
		}
	}
		
}

```

## Contact us
Email: yangpc.china@gmail.com <br>
Website: https://cocowolf.github.io/loxodon-framework/


