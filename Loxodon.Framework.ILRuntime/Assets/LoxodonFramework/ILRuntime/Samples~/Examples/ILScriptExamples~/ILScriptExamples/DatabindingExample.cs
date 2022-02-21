using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.ILScript;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.ILScriptExamples
{
    /// <summary>
    /// 请确保继承的是 Loxodon.Framework.ILScript中的ObservableObjectBase和ViewModelBase
    /// 或者自己直接实现INotifyPropertyChanged接口，继承Loxodon.Framework.Observables.ObservableObject
    /// 会有跨域问题。
    /// </summary>
    public class Account : ObservableObjectBase
    {
        public static Account Create()
        {
            return new Account()
            {
                Username = "ypc"
            };
        }

        private int id;
        private string username;
        private string password;
        private string email;
        private DateTime birthday;
        private readonly ObservableProperty<string> address = new ObservableProperty<string>();

        public int ID
        {
            get { return this.id; }
            set { this.Set<int>(ref this.id, value); }
        }

        public string Username
        {
            get { return this.username; }
            set { this.Set<string>(ref this.username, value); }
        }

        public string Password
        {
            get { return this.password; }
            set { this.Set<string>(ref this.password, value); }
        }

        public string Email
        {
            get { return this.email; }
            set { this.Set<string>(ref this.email, value); }
        }

        public DateTime Birthday
        {
            get { return this.birthday; }
            set { this.Set<DateTime>(ref this.birthday, value); }
        }

        public ObservableProperty<string> Address
        {
            get { return this.address; }
        }

        public void Print()
        {
            Debug.LogFormat("Username:{0}", username);
        }
    }

    public class AccountViewModel : ViewModelBase
    {
        private Account account;
        private User user;
        private bool remember;
        private string username;
        private string email;
        private ObservableDictionary<string, string> errors = new ObservableDictionary<string, string>();

        public AccountViewModel()
        {
        }

        public User User
        {
            get { return this.user; }
            set { this.Set<User>(ref user, value); }
        }

        public Account Account
        {
            get { return this.account; }
            set { this.Set<Account>(ref account, value); }
        }

        public string Username
        {
            get { return this.username; }
            set { this.Set<string>(ref this.username, value); }
        }

        public string Email
        {
            get { return this.email; }
            set { this.Set<string>(ref this.email, value); }
        }

        public bool Remember
        {
            get { return this.remember; }
            set { this.Set<bool>(ref this.remember, value); }
        }

        public ObservableDictionary<string, string> Errors
        {
            get { return this.errors; }
            set { this.Set<ObservableDictionary<string, string>>(ref this.errors, value); }
        }

        public void OnUsernameValueChanged(string value)
        {
            Debug.LogFormat("Username ValueChanged:{0}", value);
        }

        public void OnEmailValueChanged(string value)
        {
            Debug.LogFormat("Email ValueChanged:{0}", value);
        }

        public void OnSubmit()
        {
            if (string.IsNullOrEmpty(this.Username) || !Regex.IsMatch(this.Username, "^[a-zA-Z0-9_-]{4,12}$"))
            {
                this.errors["errorMessage"] = "Please enter a valid username.";
                return;
            }

            if (string.IsNullOrEmpty(this.Email) || !Regex.IsMatch(this.Email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                this.errors["errorMessage"] = "Please enter a valid email.";
                return;
            }

            this.errors.Clear();
            this.Account.Username = this.Username;
            this.Account.Email = this.Email;
        }


    }

    public class DatabindingExample : UIView
    {

        private Localization localization;

        public static void Run(GameObject go)
        {
            go.AddComponent<DatabindingExample>();
        }

        protected override void Awake()
        {
            ApplicationContext context = Context.GetApplicationContext();
            ILRuntimeBindingServiceBundle bindingService = new ILRuntimeBindingServiceBundle(context.GetContainer());
            bindingService.Start();

            CultureInfo cultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
            localization = Localization.Current;
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("Localizations", new XmlDocumentParser()));
            context.GetContainer().Register<Localization>(localization);
        }


        protected override void Start()
        {
            DatabindingVariables variables = this.GetComponent<DatabindingVariables>();
            Account account = new Account()
            {
                ID = 1,
                Username = "test",
                Password = "test",
                Email = "test@gmail.com",
                Birthday = new DateTime(2000, 3, 3)
            };

            account.Address.Value = "beijing";

            User user = new User()
            {
                FirstName = "Tom"
            };


            AccountViewModel accountViewModel = new AccountViewModel()
            {
                Account = account,
                User = user,
            };

            IBindingContext bindingContext = this.BindingContext();
            bindingContext.DataContext = accountViewModel;

            /* databinding */
            BindingSet bindingSet = this.CreateSimpleBindingSet();
            bindingSet.Bind(variables.Get<Text>("title")).For("text").ToValue(localization.GetValue("databinding.tutorials.title")).OneTime();
            bindingSet.Bind(variables.Get<Text>("username")).For("text").To("Account.Username").OneWay();
            bindingSet.Bind(variables.Get<Text>("password")).For("text").To("Account.Password").OneWay();
            bindingSet.Bind(variables.Get<Text>("email")).For("text").To("Account.Email").OneWay();
            bindingSet.Bind(variables.Get<Text>("remember")).For("text").To("Remember").OneWay();
            //bindingSet.Bind(this.birthday).For("text").ToExpression(vm => string.Format("{0} ({1})",
            // vm.Account.Birthday.ToString("yyyy-MM-dd"), (DateTime.Now.Year - vm.Account.Birthday.Year))).OneWay();

            bindingSet.Bind(variables.Get<Text>("address")).For("text").To("Account.Address").OneWay();
            //bindingSet.Bind(variables.Get<Text>("description")).For(v => v.text).ToExpression(vm => localization.GetFormattedText("databinding.tutorials.description", vm.Account.Username, vm.Username)).OneWay();

            bindingSet.Bind(variables.Get<Text>("errorMessage")).For("text").To("Errors['errorMessage']").OneWay();

            bindingSet.Bind(variables.Get<InputField>("usernameEdit")).For("text", "onEndEdit").To("Username").TwoWay();
            bindingSet.Bind(variables.Get<InputField>("usernameEdit")).For("onValueChanged").To("OnUsernameValueChanged");
            bindingSet.Bind(variables.Get<InputField>("emailEdit")).For("text", "onEndEdit").To("Email").TwoWay();
            bindingSet.Bind(variables.Get<InputField>("emailEdit")).For("onValueChanged").To("OnEmailValueChanged");
            bindingSet.Bind(variables.Get<Toggle>("rememberEdit")).For("isOn", "onValueChanged").To("Remember").TwoWay();
            bindingSet.Bind(variables.Get<Button>("submit")).For("onClick").To("OnSubmit");
            //bindingSet.Bind().For("OnOpenLoginWindow").To("LoginRequest");
            //bindingSet.Bind(variables.Get<Button>("submit")).For("onClick").To("Click");
            bindingSet.Build();
        }
    }
}
