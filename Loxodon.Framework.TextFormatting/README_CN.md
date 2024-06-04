![](docs/images/icon.png)

# Loxodon Framework TextFormatting

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![openupm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-textformatting?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vovgou.loxodon-framework-textformatting/)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-textformatting)](https://www.npmjs.com/package/com.vovgou.loxodon-framework-textformatting)

[(English)](README.md)

*开发者 Clark*

要求Unity 2021.3 或者更高版本

这是一个基于C#官方库修改的文本格式化插件，它通过扩展StringBuilder的AppendFormat函数，旨在避免在字符串拼接或数字转为字符串时产生垃圾回收（GC）。这样可以提高性能，特别是在对性能要求较高的场景下。

[Loxodon.Framework.TextUGUI](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.TextUGUI)

此插件还对Unity的UGUI进行了扩展，引入了两个新的文本控件：TemplateText和FormattableText。这两个控件支持MVVM的数据绑定特性，可以将ViewModel或值类型的对象与控件进行绑定，同时避免了值类型对象的装箱和拆箱，以最大程度地优化垃圾回收(GC)。

[Loxodon.Framework.TextMeshPro](https://github.com/vovgou/loxodon-framework?path=Loxodon.Framework.TextMeshPro)

Loxodon.Framework.TextMeshPro提供了基于TextMeshPro的TemplateTextMeshPro和FormattableTextMeshProUGUI控件，可以进一步减少垃圾回收（GC），完全0GC的更新游戏视图。

## 安装

### 使用 OpenUPM 安装(推荐)

[OpenUPM](https://openupm.com/) 是一个开源的UPM包仓库，它支持发布第三方的UPM包，它能够自动管理包的依赖关系，推荐使用它安装本框架.

通过openupm命令安装包,要求[nodejs](https://nodejs.org/en/download/) and openupm-cli客户端的支持，如果没有安装请先安装nodejs和open-cli。

    # 使用npm命令安装openupm-cli，如果已经安装请忽略.
    npm install -g openupm-cli

    #切换当前目录到项目的根目录
    cd F:/workspace/New Unity Project

    #安装 loxodon-framework-textformatting
    openupm add com.vovgou.loxodon-framework-textformatting

### 修改Packages/manifest.json文件安装

通过修改manifest.json文件安装，不需要安装nodejs和openupm-cli客户端。在Unity项目根目录下找到Packages/manifest.json文件，在文件的scopedRegistries（没有可以自己添加）节点下添加第三方仓库package.openupm.com的配置，同时在dependencies节点下添加com.vovgou.loxodon-framework-textmeshpro的配置，保存后切换到Unity窗口即可完成安装。

    {
      "dependencies": {
        ...
        "com.unity.modules.xr": "1.0.0",
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

## 快速开始

### 格式化字符串

  本插件扩展了StringBuilder的AppendFormat<>()函数。支持多个不同类型的泛型参数或者泛型数组参数，当这些参数为数字类型、DateTime、TimeSpan类型时，使用它们拼接字符串时不需要将值类型装箱或者拆箱，数字类型转为String类型时不会产生垃圾回收（GC）。使用方式见下面的示例。

	using System;
	using System.Text;
	using UnityEngine;
	using Loxodon.Framework.TextFormatting;//必须先引入这个包名
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

## 联系方式
邮箱: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
网站: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ群: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
