![](docs/images/icon.png)

# Loxodon Framework

[![license](https://img.shields.io/badge/license-MIT-blue.png)](https://github.com/cocowolf/loxodon-framework/blob/master/LICENSE)
[![release](https://img.shields.io/badge/release-v1.7.8-blue.png)](https://github.com/cocowolf/loxodon-framework/releases)

[(English)](README.md)

**MVVM Framework for Unity3D （C# & XLua）**

*开发者 Clark*

要求Unity 5.6.0或者更高版本

LoxodonFramework是一个轻量级的MVVM(Model-View-ViewModel)框架，它是专门为Unity3D游戏开发设计的，参考了WPF和Android的MVVM设计，它提供了视图和视图模型的数据绑定、本地化、一个简单的对象容器、配置文件组件、线程工具组件、应用上下文和玩家上下文，异步线程和协程的任务组件等基本组件，同时还提供了一个UI视图的框架。所有代码都基于面向对象面向接口的思路设计，几乎所有功能都可以自定义。而且在数据绑定部分进行了性能优化，在支持JIT的平台上使用的是委托的方式绑定，在不支持JIT的平台，默认使用的是反射，但是可以通过注入委托函数的方式来优化！

本框架使用C#语言开发，同时也支持使用XLua来开发，XLua插件是一个可选项，如果项目需要热更新，那么只要安装了XLua插件，则可以完全使用Lua来开发游戏。

这个插件兼容 MacOSX,Windows,Linux,UWP,IOS and Android等等，并且完全开源。

**已测试的平台：**  
PC/Mac/Linux  
IOS  
Android  
UWP(window10)

## 下载  
- [AssetStore](https://www.assetstore.unity3d.com/#!/content/77446)
- [Releases](https://github.com/cocowolf/loxodon-framework/releases)

## 关键功能：
- MVVM框架;
- 支持XLua，可以完全使用Lua脚本开发（可选）
- 多平台支持;
- 高扩展性，面向接口开发;
- 支持线程和协程的异步结果和异步任务，采用Future/Promise设计模式;
- 多线程组件和定时执行器;<br>
- 支持消息系统，订阅和发布事件;
- 可加密的配置文件，支持对象存取，可以自定义对象转换器，支持更多的对象;
- 本地化支持，与Android的本地化类似;
- 数据绑定支持:
    - Field绑定，只支持OneTime的模式，因无法支持改变通知;
    - 属性绑定，支持TwoWay双向绑定，值修改自动通知;
    - 普通字典、列表绑定，不支持改变通知;
    - 支持C#事件绑定;
    - 支持Unity3D的EventBase事件绑定;
    - 支持静态类的属性和Field的绑定;
    - 支持方法绑定;
    - 支持命令绑定，通过命令绑定可以方便控制按钮的有效无效状态;
    - 支持可观察属性、字典、列表的绑定，支持改变通知，视图模型修改自动更改UI显示;
    - 支持表达式的绑定;
    - 支持类型转换器，可以将图片名称转换为图集中的Sprite
    
## 注意：  
- LoxodonFramework 支持 .Net2.0 和 .Net2.0 Subset  
- LoxodonFramework 支持 .Net4.x 和 .Net Standard2.0  
- LoxodonFramework 支持 Mono2x 和 IL2CPP  
- IOS平台需要配置 AOT Compilation Options: "nrgctx-trampolines=8192,nimt-trampolines=8192,ntrampolines=8192"   

## 教程和示例

- [教程](https://github.com/cocowolf/loxodon-framework/tree/master/Assets/LoxodonFramework/Tutorials)
- [示例](https://github.com/cocowolf/loxodon-framework/tree/master/Assets/LoxodonFramework/Examples)

## 中文文档

- [HTML](https://github.com/cocowolf/loxodon-framework/blob/master/docs/LoxodonFramework.md)
- [PDF](https://github.com/cocowolf/loxodon-framework/blob/master/Assets/LoxodonFramework/Docs/LoxodonFramework.pdf)

## Loxodon.Framework.XLua插件导入教程 

- 从Xlua的Github仓库下载最新版的XLua，可以使用源码版本Source code.zip或者xlua_v2.x.xx.zip版本（建议使用xlua_v2.x.xx.zip版本，避免命XLua目录下测试类导致的类名冲突）。将下载好的xlua解压缩，拷贝到项目中。**注意：Unity2018请使用.net3.5,否则会出错，如果想使用.net4.6请参考xlua的FQA解决兼容性问题。**[XLua FQA](https://github.com/Tencent/xLua/blob/master/Assets/XLua/Doc/faq.md) [下载XLua](https://github.com/Tencent/xLua/releases) 
- 配置Unity3D项目PlayerSetting/Scripting Defin Symbols，添加XLUA的宏定义，为避免出错，最好将PC、Android、iOS等平台的都配上。
- 导入LoxodonFramework目录下Docs/XLua/Loxodon.Framework.XLua.unitypackage。如果出现编译错误，请检查是否导入了XLua的Examples目录，这个目录下的InvokeLua.cs文件定义了PropertyChangedEventArgs类，因没有使用命名空间，会导致和System.ComponentModel.PropertyChangedEventArgs类冲突，请删除XLua目录下的Examples文件夹或者给InvokeLua.cs文件中的PropertyChangedEventArgs类添加上命名空间。
- 打开LoxodonFramework/Lua/Examples 目录，查看示例。

## 介绍
- 窗口视图示例 
  ![](docs/images/Window.png) 
- 本地化示例 
  ![](docs/images/Localization.png) 
- 数据绑定示例 
  ![](docs/images/Databinding.png) 
- 变量 示例 
  ![](docs/images/Variable.png) 
- 列表视图绑定 
  ![](docs/images/ListView.png) 

## 联系方式
邮箱: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
网站: [https://cocowolf.github.io/loxodon-framework/](https://cocowolf.github.io/loxodon-framework/)  
QQ群: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)

## 其他插件
如果有需要AssetBundle加载和管理，资源冗余分析插件的，可以看看我的[Loxodon.Framework.Bundle](https://assetstore.unity.com/packages/slug/87419)


