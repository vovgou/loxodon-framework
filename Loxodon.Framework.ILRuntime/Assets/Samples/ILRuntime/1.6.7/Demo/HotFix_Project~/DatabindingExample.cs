using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.ILScript;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix_Project
{
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
            UnityEngine.Debug.LogFormat("Username:{0}", username);
        }
    }

    public class LoginViewModel : ViewModelBase
    {
        private string username;
        private string password;
        private SimpleCommand loginCommand;
        private SimpleCommand cancelCommand;

        public LoginViewModel()
        {
            this.loginCommand = new SimpleCommand(this.Login);
            this.cancelCommand = new SimpleCommand(() =>
            {
                //this.interactionFinished.Raise();/* Request to close the login window */
            });
        }

        public ICommand LoginCommand
        {
            get { return this.loginCommand; }
        }

        public ICommand CancelCommand
        {
            get { return this.cancelCommand; }
        }

        public string Username
        {
            get { return this.username; }
            set
            {
                if (this.Set<string>(ref this.username, value, "Username"))
                {
                    //this.ValidateUsername();
                }
            }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.Set<string>(ref this.password, value, "Password"))
                {
                    //this.ValidatePassword();
                }
            }
        }

        public void Login()
        {

        }

    }

    public class AccountViewModel : ViewModelBase
    {
        private Account account;
        private User user;
        private bool remember;
        private string username;
        private string email;
        private InteractionRequest<LoginViewModel> loginRequest;
        private SimpleCommand command;
        private ObservableDictionary<string, string> errors = new ObservableDictionary<string, string>();
        //private Localization localization;

        public AccountViewModel()
        {
            //ApplicationContext context = Context.GetApplicationContext();
            //localization = context.GetService<Localization>();
            this.loginRequest = new InteractionRequest<LoginViewModel>(this);

            var loginViewModel = new LoginViewModel();
            loginViewModel.Username = "Clark";
            this.command = new SimpleCommand(() =>
            {
                this.command.Enabled = false;
                this.loginRequest.Raise(loginViewModel, vm =>
                {
                    this.command.Enabled = true;
                });
            });

        }

        public IInteractionRequest LoginRequest
        {
            get { return this.loginRequest; }
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

        public ICommand Click
        {
            get { return this.command; }
        }

        //public string Description { get { localization.GetFormattedText("databinding.tutorials.description", this.Username, vm.Username); } }

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
            localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
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

            //account.Address.ValueChanged += (sender, e) => { }; //OK
            //account.Address.ValueChanged += Address_ValueChanged;//OK
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
            //BindingSet<DatabindingExample, AccountViewModel> bindingSet = this.CreateBindingSet<DatabindingExample, AccountViewModel>();
            BindingSet bindingSet = this.CreateSimpleBindingSet();

            bindingSet.Bind(variables.Get<Text>("title")).For("text").ToValue(Res.databinding_tutorials_title).OneTime();

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
            //bindingSet.Bind(variables.Get<Button>("submit")).For("onClick").To("OnSubmit");


            bindingSet.Bind().For("OnOpenLoginWindow").To("LoginRequest");
            bindingSet.Bind(variables.Get<Button>("submit")).For("onClick").To("Click");
            bindingSet.Build();


            //BindingSet<DatabindingExample> staticBindingSet = this.CreateBindingSet<DatabindingExample>();
            ////staticBindingSet.Bind(variables.Get<Text>("title")).For("text").ToValue(Res.databinding_tutorials_title).OneTime();//OK
            //staticBindingSet.Bind(variables.Get<Text>("title")).For("text").To("Res.databinding_tutorials_title").OneTime();//OK
            //staticBindingSet.Build();
        }

        private void Address_ValueChanged(object sender, EventArgs e)
        {
        }

        private void OnOpenLoginWindow(object sender, InteractionEventArgs args)
        {
            var loginViewModel = (LoginViewModel) args.Context;

            UnityEngine.Debug.LogFormat("OnOpenWindow:{0}", loginViewModel.Username);
        }
    }



    //public class DatabindingExample2 : MonoBehaviour
    //{
    //    public Text description;
    //    public Text title;
    //    public Text username;
    //    public Text password;
    //    public Text email;
    //    public Text birthday;
    //    public Text address;
    //    public Text remember;

    //    public Text errorMessage;

    //    public InputField usernameEdit;
    //    public InputField emailEdit;
    //    public Toggle rememberEdit;
    //    public Button submit;

    //    private Localization localization;

    //    public static void Run(GameObject go)
    //    {
    //        go.AddComponent<DatabindingExample2>();
    //    }

    //    protected void Awake()
    //    {
    //        ApplicationContext context = Context.GetApplicationContext();
    //        ILRuntimeBindingServiceBundle bindingService = new ILRuntimeBindingServiceBundle(context.GetContainer());
    //        bindingService.Start();

    //        CultureInfo cultureInfo = Locale.GetCultureInfo();
    //        localization = Localization.Current;
    //        localization.CultureInfo = cultureInfo;
    //        localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
    //    }

    //    protected void Start()
    //    {
    //        DatabindingAttritures attrs = this.GetComponent<DatabindingAttritures>();
    //        this.description = attrs.description;
    //        this.title = attrs.title;
    //        this.username = attrs.username;
    //        this.password = attrs.password;
    //        this.email = attrs.email;
    //        this.birthday = attrs.birthday;
    //        this.address = attrs.address;
    //        this.remember = attrs.remember;
    //        this.errorMessage = attrs.errorMessage;
    //        this.usernameEdit = attrs.usernameEdit;
    //        this.emailEdit = attrs.emailEdit;
    //        this.rememberEdit = attrs.rememberEdit;
    //        this.submit = attrs.submit;

    //        Account account = new Account()
    //        {
    //            ID = 1,
    //            Username = "test",
    //            Password = "test",
    //            Email = "test@gmail.com",
    //            Birthday = new DateTime(2000, 3, 3)
    //        };
    //        account.Address.Value = "beijing";

    //        User user = new User()
    //        {
    //            FirstName = "Tom"
    //        };


    //        AccountViewModel accountViewModel = new AccountViewModel()
    //        {
    //            Account = account,
    //            User = user,
    //        };

    //        IBindingContext bindingContext = this.BindingContext();
    //        bindingContext.DataContext = accountViewModel;

    //        /* databinding */
    //        BindingSet<DatabindingExample2, AccountViewModel> bindingSet = this.CreateBindingSet<DatabindingExample2, AccountViewModel>();
    //        bindingSet.Bind(this.username).For("text").To("Account.Username").OneWay();
    //        bindingSet.Bind(this.password).For("text").To("Account.Password").OneWay();
    //        bindingSet.Bind(this.email).For("text").To("Account.Email").OneWay();
    //        bindingSet.Bind(this.remember).For("text").To("Remember").OneWay();
    //        //bindingSet.Bind(this.birthday).For("text").ToExpression(vm => string.Format("{0} ({1})",
    //        // vm.Account.Birthday.ToString("yyyy-MM-dd"), (DateTime.Now.Year - vm.Account.Birthday.Year))).OneWay();

    //        bindingSet.Bind(this.address).For("text").To("Account.Address").OneWay();
    //        //bindingSet.Bind(this.description).For(v => v.text).ToExpression(vm => localization.GetFormattedText("databinding.tutorials.description", vm.Account.Username, vm.Username)).OneWay();

    //        bindingSet.Bind(this.errorMessage).For("text").To("Errors['errorMessage']").OneWay();

    //        bindingSet.Bind(this.usernameEdit).For("text", "onEndEdit").To("Username").TwoWay();
    //        bindingSet.Bind(this.usernameEdit).For("onValueChanged").To("OnUsernameValueChanged");
    //        bindingSet.Bind(this.emailEdit).For("text", "onEndEdit").To("Email").TwoWay();
    //        bindingSet.Bind(this.emailEdit).For("onValueChanged").To("OnEmailValueChanged");
    //        bindingSet.Bind(this.rememberEdit).For("isOn", "onValueChanged").To("Remember").TwoWay();
    //        bindingSet.Bind(this.submit).For("onClick").To("OnSubmit");
    //        bindingSet.Build();









    //        //bindingSet.Bind(this.username).For(v => v.text).To(vm => vm.Account.Username).OneWay();
    //        //bindingSet.Bind(this.password).For(v => v.text).To(vm => vm.Account.Password).OneWay();
    //        //bindingSet.Bind(this.email).For(v => v.text).To(vm => vm.Account.Email).OneWay();
    //        //bindingSet.Bind(this.remember).For(v => v.text).To(vm => vm.Remember).OneWay();
    //        //bindingSet.Bind(this.birthday).For(v => v.text).ToExpression(vm => string.Format("{0} ({1})",
    //        // vm.Account.Birthday.ToString("yyyy-MM-dd"), (DateTime.Now.Year - vm.Account.Birthday.Year))).OneWay();

    //        //bindingSet.Bind(this.address).For(v => v.text).To(vm => vm.Account.Address).OneWay();
    //        //bindingSet.Bind(this.description).For(v => v.text).ToExpression(vm => localization.GetFormattedText("databinding.tutorials.description", vm.Account.Username, vm.Username)).OneWay();

    //        //bindingSet.Bind(this.errorMessage).For(v => v.text).To(vm => vm.Errors["errorMessage"]).OneWay();

    //        //bindingSet.Bind(this.usernameEdit).For(v => v.text, v => v.onEndEdit).To(vm => vm.Username).TwoWay();
    //        //bindingSet.Bind(this.usernameEdit).For(v => v.onValueChanged).To<string>(vm => vm.OnUsernameValueChanged);
    //        //bindingSet.Bind(this.emailEdit).For(v => v.text, v => v.onEndEdit).To(vm => vm.Email).TwoWay();
    //        //bindingSet.Bind(this.emailEdit).For(v => v.onValueChanged).To<string>(vm => vm.OnEmailValueChanged);
    //        //bindingSet.Bind(this.rememberEdit).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.Remember).TwoWay();
    //        //bindingSet.Bind(this.submit).For(v => v.onClick).To(vm => vm.OnSubmit);
    //        //bindingSet.Build();

    //        //BindingSet<DatabindingExample> staticBindingSet = this.CreateBindingSet<DatabindingExample>();
    //        //staticBindingSet.Bind(this.title).For(v => v.text).To(() => Res.databinding_tutorials_title).OneTime();
    //        ////staticBindingSet.Bind(this.title).For(v => v.text).To("Res.databinding_tutorials_title").OneTime();
    //        //staticBindingSet.Build();
    //    }
    //}
}
