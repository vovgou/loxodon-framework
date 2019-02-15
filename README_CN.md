![](docs/images/icon.png)

# Loxodon Framework

[![release](https://img.shields.io/badge/release-v1.7.0-blue.svg)](https://www.assetstore.unity3d.com/#!/content/77446)

**MVVM Framework for Unity3D**

*开发者 Clark*

要求Unity 5.6.0或者更高版本.

LoxodonFramework 是一个轻量级的MVVM(Model-View-ViewModel)框架，它是专门为Unity3D 游戏开发设计的。参考了WPF和Android的MVVM设计，它提供了视图和视图模型的数据绑定、本地化、一个简单的对象容器、配置文件组件、线程工具组件、应用上下文和玩家上下文，异步线程和协程的任务组件等基本组件，同时还提供了一个UI视图的框架。所有代码都基于面向对象面向接口的思路设计，几乎所有功能都可以自定义。而且在数据绑定部分进行了性能优化，在支持JIT的平台上使用的是委托的方式绑定，在不支持JIT的平台，默认使用的是反射，但是可以通过注入委托函数的方式来优化！

很快将开源这个框架针对XLua的支持插件，目前关于XLua支持的插件核心功能已经完成，在我的QQ群文件共享中可以下载到体验版本

这个插件兼容 MacOSX,Windows,Linux,UWP,IOS and Android,并且完全开源。

**已测试的平台：**  
PC/Mac/Linux  
IOS  
Android  
UWP(window10)

## AssetStore下载  
- [Loxodon Framework](https://www.assetstore.unity3d.com/#!/content/77446)

## 关键功能：
- MVVM框架;
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
- LoxodonFramework supports .Net2.0 and .Net2.0 Subset  
- LoxodonFramework supports Mono2x and IL2CPP  
- IOS平台需要配置 AOT Compilation Options: "nrgctx-trampolines=8192,nimt-trampolines=8192,ntrampolines=8192"   

## 教程和示例

- [教程](https://github.com/cocowolf/loxodon-framework/tree/master/Assets/LoxodonFramework/Tutorials)
- [示例](https://github.com/cocowolf/loxodon-framework/tree/master/Assets/LoxodonFramework/Examples)

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




