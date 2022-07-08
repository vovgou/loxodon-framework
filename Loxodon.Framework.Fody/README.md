![](docs/images/icon.png)

# Loxodon Framework Fody

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![openupm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-fody?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vovgou.loxodon-framework-fody/)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-fody)](https://www.npmjs.com/package/com.vovgou.loxodon-framework-fody)


[(中文版)](README_CN.md)


*Developed by Clark*

Requires Unity 2018.4 or higher.

This is a plugin for static weaving code that integrates "[PropertyChanged.Fody](https://github.com/Fody/PropertyChanged)" into a Unity project, it can inject code which raises the PropertyChanged event into property setters of classes which implement "INotifyPropertyChanged".

## Installation

### Install via OpenUPM (recommended)

[OpenUPM](https://openupm.com/) can automatically manage dependencies, it is recommended to use it to install the framework.

Requires [nodejs](https://nodejs.org/en/download/)'s npm and openupm-cli, if not installed please install them first.

    # Install openupm-cli,please ignore if it is already installed.
    npm install -g openupm-cli

    #Go to the root directory of your project
    cd F:/workspace/New Unity Project

    #Install loxodon-framework-fody
    openupm add com.vovgou.loxodon-framework-fody

### Install via Packages/manifest.json

Modify the Packages/manifest.json file in your project, add the third-party repository "package.openupm.com"'s configuration and add "com.vovgou.loxodon-framework-fody" in the "dependencies" node.

Installing the framework in this way does not require nodejs and openm-cli.

    {
      "dependencies": {
        ...
        "com.unity.modules.xr": "1.0.0",
        "com.vovgou.loxodon-framework-fody": "2.4.0"
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

Loxodon.Framework.Fody depends on Loxodon.Framework, please install Loxodon.Framework first.

- Loxodon.Framework:  https://github.com/vovgou/loxodon-framework.git?path=Loxodon.Framework/Assets/LoxodonFramework

- Loxodon.Framework.Fody: https://github.com/vovgou/loxodon-framework.git?path=Loxodon.Framework.Fody/Assets/LoxodonFramework/Fody


![](docs/images/install_via_git.png)

### Install via *.unitypackage file

Download Loxodon.Framework.unitypackage and Loxodon.Framework.Fody.unitypackage, import them into your project.

- [Releases](https://github.com/vovgou/loxodon-framework/releases)

## English manual

- [HTML](https://github.com/vovgou/loxodon-framework/blob/master/docs/LoxodonFramework_en.md)
- [PDF](https://github.com/vovgou/loxodon-framework/blob/master/docs/LoxodonFramework_en.pdf)

## Quick Start

Add the assembly filenames that need to automatically weave "Notifity" code to the configuration file.

![](docs/images/weaver_config.png)

FodyWeavers.xml

    <Weavers xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="FodyWeavers.xsd">
		<AssemblyNames>
	  		<Item>Assembly-CSharp</Item>
		</AssemblyNames>
		<PropertyChanged defaultWeaving ="true" />
    </Weavers>


Create a User class in the project and add annotation "AddINotifyPropertyChangedInterface" to the class, as follows:

    [AddINotifyPropertyChangedInterface]
    public class User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

After the project is compiled, use the ILSpy tool to view the assembly.the code of the User class is as follows, "PropertyChanged.Fody" weaves the INotifyPropertyChanged interface and related code for User.cs

	public class User : INotifyPropertyChanged
	{
		public string FirstName
		{
			[CompilerGenerated]
			get
			{
				return FirstName;
			}
			[CompilerGenerated]
			set
			{
				if (!string.Equals(FirstName, value, StringComparison.Ordinal))
				{
					FirstName = value;
					<>OnPropertyChanged(<>PropertyChangedEventArgs.FullName);
					<>OnPropertyChanged(<>PropertyChangedEventArgs.FirstName);
				}
			}
		}

		public string LastName
		{
			[CompilerGenerated]
			get
			{
				return LastName;
			}
			[CompilerGenerated]
			set
			{
				if (!string.Equals(LastName, value, StringComparison.Ordinal))
				{
					LastName = value;
					<>OnPropertyChanged(<>PropertyChangedEventArgs.FullName);
					<>OnPropertyChanged(<>PropertyChangedEventArgs.LastName);
				}
			}
		}

		public string FullName => FirstName + " " + LastName;

		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		[GeneratedCode("PropertyChanged.Fody", "3.4.1.0")]
		[DebuggerNonUserCode]
		protected void <>OnPropertyChanged(PropertyChangedEventArgs eventArgs)
		{
			this.PropertyChanged?.Invoke(this, eventArgs);
		}
	}



## Contact Us
Email: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
Website: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ Group: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
