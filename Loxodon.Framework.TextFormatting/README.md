![](docs/images/icon.png)

# Loxodon Framework TextFormatting

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![openupm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-textformatting?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vovgou.loxodon-framework-textformatting/)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-textformatting)](https://www.npmjs.com/package/com.vovgou.loxodon-framework-textformatting)

[(中文版)](README_CN.md)

*Developed by Clark*

Requires Unity 2021.3 or higher.

This is a text formatting plugin modified based on the official C# library. By extending the AppendFormat function of StringBuilder, it aims to avoid garbage collection (GC) when concatenating strings or converting numbers to strings. This optimization is particularly beneficial in scenarios with high-performance requirements.

[Loxodon.Framework.TextUGUI](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.TextUGUI)

This plugin also extends Unity's UGUI by introducing two new text controls: TemplateText and FormattableText. These controls support MVVM data binding, allowing ViewModel or value-type objects to be bound to the controls. Additionally, they avoid boxing and unboxing of value-type objects, optimizing garbage collection (GC) to the greatest extent possible.

[Loxodon.Framework.TextMeshPro](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.TextMeshPro)

Loxodon.Framework.TextMeshPro provides TemplateTextMeshPro and FormattableTextMeshProUGUI controls based on TextMeshPro, which can further reduce garbage collection (GC) and enable completely 0-GC game view updates.

## Installation

### Install via OpenUPM (recommended)

[OpenUPM](https://openupm.com/) can automatically manage dependencies, it is recommended to use it to install the framework.

Requires [nodejs](https://nodejs.org/en/download/)'s npm and openupm-cli, if not installed please install them first.

    # Install openupm-cli,please ignore if it is already installed.
    npm install -g openupm-cli

    #Go to the root directory of your project
    cd F:/workspace/New Unity Project

    #Install loxodon-framework-textformatting
    openupm add com.vovgou.loxodon-framework-textformatting

### Install via Packages/manifest.json

Modify the Packages/manifest.json file in your project, add the third-party repository "package.openupm.com"'s configuration and add "com.vovgou.loxodon-framework-textformatting" in the "dependencies" node.

Installing the framework in this way does not require nodejs and openm-cli.

    {
      "dependencies": {
        ...
        "com.vovgou.loxodon-framework-textformatting": "2.6.2"
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

## Quick Start

### StringBuilder.AppendFormat

This plugin enhances the AppendFormat<>() function of StringBuilder. It provides support for multiple generic parameters of different types or generic array parameters. When these parameters are of numeric types, DateTime, or TimeSpan, using them to concatenate strings eliminates the need for value type boxing or unboxing. Consequently, converting numeric types to String during string concatenation does not generate garbage collection (GC). See the example below for usage details.

	using System;
	using System.Text;
	using UnityEngine;
	using Loxodon.Framework.TextFormatting;//make sure to first import the required namespace
	public class Example : MonoBehaviour
	{
	    StringBuilder builder = new StringBuilder();
	    void Update()
	    {
	        builder.Clear();
	        builder.AppendFormat<DateTime,int>("Now:{0:yyyy-MM-dd HH:mm:ss} Frame:{0:D6}", DateTime.Now,Time.frameCount);
	        builder.AppendFormat<float>("{0:f2}", Time.realtimeSinceStartup);       
	    }
	    
	}

![](docs/images/append_format.png)

## Contact Us
Email: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
Website: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ Group: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
