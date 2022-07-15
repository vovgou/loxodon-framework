![](docs/images/icon.png)

# Loxodon Framework UIToolkit

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![openupm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-uitoolkit?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vovgou.loxodon-framework-uitoolkit/)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-uitoolkit)](https://www.npmjs.com/package/com.vovgou.loxodon-framework-uitoolkit)


*Developed by Clark*

Requires Unity 2021.3 or higher.

此插件扩展了Loxodon.Framework框架针对UIToolkit的支持，增加UIToolkitWindow窗口，对UIToolkit的控件可以进行数据绑定，也支持UIToolkit和UGUI的界面混用。

## 安装

### 从OpenUPM安装

[OpenUPM](https://openupm.com/) 中提供了很多的Unity插件，自动管理依赖，推荐从OpenUPM仓库安装本插件.

命令行方式安装，要求 [nodejs](https://nodejs.org/en/download/)'s npm and openupm-cli, 如果没有安装nodejs命令行环境请先安装nodejs。

    # Install openupm-cli,please ignore if it is already installed.
    npm install -g openupm-cli

    #Go to the root directory of your project
    cd F:/workspace/New Unity Project

    #Install loxodon-framework-uitoolkit
    openupm add com.vovgou.loxodon-framework-uitoolkit

### 通过修改 Packages/manifest.json 文件安装插件(推荐)

在Unity项目的Packages目录中找到manifest.json 文件，增加第三方仓库 "https://package.openupm.com"或者"https://registry.npmjs.org"到配置文件中，然后增加"com.vovgou.loxodon-framework-uitoolkit" 到dependencies节点下，Unity会自动下载插件，使用这种方式安装也相当方便，且省去了安装nodejs和openm-cli客户端的麻烦。

    {
      "dependencies": {
        ...
        "com.unity.modules.xr": "1.0.0",
        "com.vovgou.loxodon-framework-uitoolkit": "2.4.0"
      },
      "scopedRegistries": [
        {
          "name": "package.openupm.com",
          "url": "https://package.openupm.com",
          "scopes": [
            "com.vovgou",
            "com.openupm"
          ]
        }
      ]
    }


### 通过 *.unitypackage 安装

从框架的发布地址下载 Loxodon.Framework.unitypackage 和 Loxodon.Framework.UIToolkit.unitypackage, 导入到项目中即可.

- [Releases](https://github.com/vovgou/loxodon-framework/releases)

## English manual

- [HTML](https://github.com/vovgou/loxodon-framework/blob/master/docs/LoxodonFramework_en.md)
- [PDF](https://github.com/vovgou/loxodon-framework/blob/master/docs/LoxodonFramework_en.pdf)

## 快速开始

创建一个UIToolkit的窗口，将UIElements的控件和ViewModel的属性绑定。

**注意：点击事件请绑定到clickable属性上，控件的值请绑定到value属性。**

    public class Window1 : UIToolkitWindow
    {
        private IUIViewLocator locator;
        protected override void OnCreate(IBundle bundle)
        {            
            this.locator = Context.GetApplicationContext().GetService<IUIViewLocator>();

			WindowViewMode viewModel = new WindowViewMode();
            var bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind(this.Q<Button>("openDialog")).For(v => v.clickable).To(vm => vm.DialogClick);
            bindingSet.Bind(this.Q<Button>("openWindow")).For(v => v.clickable).To(vm => vm.WindowClick);
			bindingSet.Bind(this.Q<Toggle>("toggle")).For(v => v.value).To(vm => vm.Toggle);
            bindingSet.Bind(this.Q<TextField>("username")).For(v => v.value, v => v.RegisterValueChangedCallback).To(vm => vm.Name);
            bindingSet.Bind().For(v => v.OnOpenDialogWindow).To(vm => vm.OpenDialogRequest);
            bindingSet.Bind().For(v => v.OnOpenWindow).To(vm => vm.OpenWindowRequest);

            //如果是分子视图来开发，UIToolkit的子视图没法挂脚本，可以通过绑定集的ScopeKey来管理子视图的绑定
            //bindingSet.Bind().For(v => v.OnOpenWindow).To(vm => vm.OpenWindowRequest).WithScopeKey("xxSubView");

            bindingSet.Build();

            this.Q<Button>("close").clicked += () => this.Dismiss();
        }

        protected void OnOpenDialogWindow(object sender, InteractionEventArgs args)
        {
            var callback = args.Callback;
            AlertDialog.ShowMessage("测试", "标题", "OK", r =>
            {
                callback?.Invoke();
            });
        }

        protected void OnOpenWindow(object sender, InteractionEventArgs args)
        {
            IWindow window = locator.LoadWindow<IWindow>(this.WindowManager, "UI/Window2");
            window.Create();
            window.Show();
        }
    }


    public class WindowViewMode : ViewModelBase
    {
		private string name;
		private bool toggle = true;
        private SimpleCommand dialogCommand;
        private SimpleCommand windowCommand;
        private InteractionRequest openDialogRequest;
        private InteractionRequest openWindowRequest;

        public WindowViewMode()
        {
            this.openDialogRequest = new InteractionRequest(this);
            this.openWindowRequest = new InteractionRequest(this);
            this.dialogCommand = new SimpleCommand(() =>
            {
                this.dialogCommand.Enabled = false;
                this.openDialogRequest.Raise(() =>
                {
                    this.dialogCommand.Enabled = true;
                });
            });
            this.windowCommand = new SimpleCommand(() =>
            {
                this.openWindowRequest.Raise();
            });
        }

		public string Name
        {
            get { return this.name; }
            set { this.Set<string>(ref name, value); }
        }

		public bool Toggle
        {
            get { return this.toggle; }
            set { this.Set<bool>(ref toggle, value); }
        }

        public IInteractionRequest OpenDialogRequest
        {
            get { return this.openDialogRequest; }
        }

        public IInteractionRequest OpenWindowRequest
        {
            get { return this.openWindowRequest; }
        }

        public ICommand DialogClick
        {
            get { return this.dialogCommand; }
        }

        public ICommand WindowClick
        {
            get { return this.windowCommand; }
        }

        public void OnClick()
        {
            Debug.LogFormat("Button OnClick");
        }
    }


## 联系我们
Email: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
Website: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ Group: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
