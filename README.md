![](docs/images/icon.png)

# Loxodon Framework

[![license](https://img.shields.io/badge/license-MIT-blue.png)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE)
[![release](https://img.shields.io/badge/release-v2.0.0-blue.png)](https://github.com/vovgou/loxodon-framework/releases)

[(中文版)](README_CN.md)

**MVVM and Databinding for Unity3d(C# & XLua)**

*Developed by Clark*

Requires Unity 2018.4 or higher.

LoxodonFramework is a lightweight MVVM(Model-View-ViewModel) framework built specifically to target Unity3D.
Databinding and localization are supported.It has a very flexible extensibility.It makes your game development faster and easier.

For tutorials,examples and support,please see the project.You can also discuss the project in the Unity Forums.

The plugin is compatible with MacOSX,Windows,Linux,UWP,WebGL,IOS and Android,and provides all the source code of the project.

If you like this framework or think it is useful, please write a review on [AssetStore](https://assetstore.unity.com/packages/tools/gui/loxodon-framework-2-0-178583#reviews) or give me a STAR or FORK it on Github, thank you!

**Tested in Unity 3D on the following platforms:**  
PC/Mac/Linux  
IOS  
Android  
UWP(window10)  
WebGL  

## Installation

### Install via OpenUPM (recommended)

[OpenUPM](https://openupm.com/) can automatically manage dependencies, it is recommended to use it to install the framework.

Requires [nodejs](https://nodejs.org/en/download/)'s npm and openupm-cli, if not installed please install them first.

    # Install openupm-cli,please ignore if it is already installed.
    npm install -g openupm-cli 
    
    #Go to the root directory of your project
    cd F:/workspace/New Unity Project
    
    #Install loxodon-framework
    openupm add com.vovgou.loxodon-framework
    
### Install via Packages/manifest.json

Modify the Packages/manifest.json file in your project, add the third-party repository "package.openupm.com"'s configuration and add "com.vovgou.loxodon-framework" in the "dependencies" node.

Installing the framework in this way does not require nodejs and openm-cli.

    {
      "dependencies": {
        ...
        "com.unity.modules.xr": "1.0.0",
        "com.vovgou.loxodon-framework": "2.0.0-preview"
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

### Install via git URL

After Unity 2019.3.4f1 that support path query parameter of git package. You can add https://github.com/vovgou/loxodon-framework.git?path=Loxodon.Framework/Assets/LoxodonFramework to Package Manager

![](docs/images/install_via_git.png)

### Install via *.unitypackage file

Download [Loxodon.Framework2.x.x.unitypackage](https://github.com/vovgou/loxodon-framework/releases) and import it into your project.

- [AssetStore](https://assetstore.unity.com/packages/tools/gui/loxodon-framework-77446)
- [Releases](https://github.com/vovgou/loxodon-framework/releases)

### Import the samples

 - Unity 2019 and later versions can import examples through Package Manager.

   ![](docs/images/install_examples.png)
   
 - If the Editor is Unity 2018 version, please find Examples.unitypackage and Tutorials.unitypackage in the "Packages/Loxodon Framework/Package Resources/" folder, double-click to import into the project.

## English manual

- [HTML](https://github.com/vovgou/loxodon-framework/blob/master/docs/LoxodonFramework_en.md)
- [PDF](https://github.com/vovgou/loxodon-framework/blob/master/docs/LoxodonFramework_en.pdf)

## Key Features:
- MVVM Framework;
- Multiple platforms;
- Higher Extensibility;
- async&await (C#&Lua)
- try&catch&finally for lua
- XLua support(You can make your game in lua.);
- Asynchronous result and asynchronous task are supported;
- Scheduled Executor and Multi-threading;<br>
- Messaging system support;
- Preferences can be encrypted;
- Localization support;
- Databinding support:
    - Field binding;
    - Property binding;
    - Dictionary,list and array binding;
    - Event binding;
    - Unity3d's EventBase binding;
    - Static property and field binding;
    - Method binding;
    - Command binding;
    - ObservableProperty,ObservableDictionary and ObservableList binding;
    - Expression binding;

## Notes  
- .Net2.0 and .Net2.0 Subset,please use version 1.9.x.
- LoxodonFramework 2.0 supports .Net4.x and .Net Standard2.0  
- LoxodonFramework 2.0 supports Mono and IL2CPP 

## Quick Start

Create a view and view model of the progress bar.

![](docs/images/progress.png)

    public class ProgressBarViewModel : ViewModelBase
    {
        private string tip;
        private bool enabled;
        private float value;
        public ProgressBarViewModel()
        {
        }

        public string Tip
        {
            get { return this.tip; }
            set { this.Set<string>(ref this.tip, value, nameof(Tip)); }
        }

        public bool Enabled
        {
            get { return this.enabled; }
            set { this.Set<bool>(ref this.enabled, value, nameof(Enabled)); }
        }

        public float Value
        {
            get { return this.value; }
            set { this.Set<float>(ref this.value, value, nameof(Value)); }
        }
    }

    public class ProgressBarView : UIView
    {
        public GameObject progressBar;
        public Text progressTip;
        public Text progressText;
        public Slider progressSlider;

        protected override void Awake()
        {
            var bindingSet = this.CreateBindingSet<ProgressBar, ProgressBarViewModel>();

            bindingSet.Bind(this.progressBar).For(v => v.activeSelf).To(vm => vm.Enabled).OneWay();
            bindingSet.Bind(this.progressTip).For(v => v.text).To(vm => vm.Tip).OneWay();
            bindingSet.Bind(this.progressText).For(v => v.text)
                .ToExpression(vm => string.Format("{0:0.00}%", vm.Value * 100)).OneWay();
            bindingSet.Bind(this.progressSlider).For(v => v.value).To(vm => vm.Value).OneWay();

            bindingSet.Build();
        }
    }


    IEnumerator Unzip(ProgressBarViewModel progressBar)
    {
        progressBar.Tip = "Unziping";
        progressBar.Enabled = true;//Display the progress bar

        for(int i=0;i<30;i++)
        {            
            //TODO:Add unzip code here.

            progressBar.Value = (i/(float)30);            
            yield return null;
        }

        progressBar.Enabled = false;//Hide the progress bar
        progressBar.Tip = "";        
    }

## Plugins
- [Loxodon Framework Localization For CSV](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.LocalizationsForCsv)

    It supports localization files in csv format, requires Unity2018.4 or higher.

- [Loxodon Framework XLua](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.XLua)

    It supports making games with lua scripts.

    - Installation
        - If [Loxodon.Framework 2.0](https://github.com/vovgou/loxodon-framework/blob/master/README.md) is not installed, please install it first.

        - You can download the latest version of xlua from Xlua's Github repository,the file name is usually xlua_v2.x.xx.zip, unzip and copy it to your project.[XLua Download](https://github.com/Tencent/xLua/releases)  
        
        - Download [Loxodon.Framework.XLua.unitypackage](https://github.com/vovgou/loxodon-framework/releases) from github and import it into your Unity project.

        - **In Unity2018 and above, if you use .net 4.x and .net standard 2.0, there will be compatibility issues. Please see the xlua's FAQs.** [XLua FAQ](https://github.com/Tencent/xLua/blob/master/Assets/XLua/Doc/Faq_EN.md) 

        - Find Examples.unitypackage in the "Assets/LoxodonFramework/XLua/Package Resources" folder and import it into the project.

    - Lua precompilation tool

    	![](docs/images/LuaPrecompileWizard.png)

- [Loxodon Framework Bundle](http://u3d.as/NkT)

    Loxodon Framework Bundle is an AssetBundle manager.It provides a functionality that can automatically manage/load an AssetBundle and its dependencies from local or remote location.Asset Dependency Management including BundleManifest that keep track of every AssetBundle and all of their dependencies. An AssetBundle Simulation Mode which allows for iterative testing of AssetBundles in a the Unity editor without ever building an AssetBundle.

    The asset redundancy analyzer can help you find the redundant assets included in the AssetsBundles.Create a fingerprint for the asset by collecting the characteristic data of the asset. Find out the redundant assets in all AssetBundles by fingerprint comparison.it only supports the AssetBundle of Unity 5.6 or higher.

    ![](docs/images/bundle.png)

- [Loxodon Framework Log4Net](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Log4Net)

    This is a log plugin.It helps you to use Log4Net in the Unity3d.

    ![](docs/images/log4net.png)

- [Loxodon Framework Obfuscation](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Obfuscation)

    **NOTE:Please enable "Allow unsafe Code"**

    ![](docs/images/obfuscation_unsafe.png)

    **Example:**

		ObfuscatedInt  length = 200;
		ObfuscatedFloat scale = 20.5f;
		int offset = 30;
		
		float value = (length * scale) + offset;

- [Loxodon Framework Addressable](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Addressable)

- [Loxodon Framework Connection](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.Connection)

    This is a network connection component, implemented using TcpClient, supports IPV6 and IPV4, automatically recognizes the current network when connecting with a domain name, and preferentially connects to the IPV4 network.

- [Json.Net.Aot](https://github.com/Daddoon/Json.NET.Aot)

    This implementation is actually a fork of the excellent work of SaladLab, Json.Net.Unity3D, and of course, the excellent work of the initial author, Newtonsoft!

- [LiteDB](https://github.com/mbdavid/LiteDB)

    LiteDB is a small, fast and lightweight NoSQL embedded database.

    ![](https://camo.githubusercontent.com/d85fc448ef9266962a8e67f17f6d16080afdce6b/68747470733a2f2f7062732e7477696d672e636f6d2f6d656469612f445f313432727a57774145434a44643f666f726d61743d6a7067266e616d653d39303078393030)

- [SQLite4Unity3d](https://github.com/robertohuertasm/SQLite4Unity3d)

    When I started with Unity3d development I needed to use SQLite in my project and it was very hard to me to find a place with simple instructions on how to make it work. All I got were links to paid solutions on the Unity3d's Assets Store and a lot of different and complicated tutorials.

    At the end, I decided that there should be a simpler way and I created SQLite4Unity3d, a plugin that helps you to use SQLite in your Unity3d projects in a clear and easy way and works in iOS, Mac, Android and Windows projects.

    It uses the great [sqlite-net](https://github.com/praeclarum/sqlite-net/) library as a base so you will have Linq besides sql. For a further reference on what possibilities you have available with this library I encourage you to visit its github repository.


## Tutorials and Examples

 ![](docs/images/Launcher.gif)

 ![](docs/images/Databinding.gif)

 ![](docs/images/ListView.gif)

 ![](docs/images/Localization.gif)

![](docs/images/Interaction.gif)

## Introduction
- Window View

  ![](docs/images/Window.png)
- Localization

  ![](docs/images/Localization.png)
- Databinding

  ![](docs/images/Databinding.png)
- Variable Example

  ![](docs/images/Variable.png)
- ListView Binding

  ![](docs/images/ListView.png)

## Contact Us
Email: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
Website: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ Group: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
