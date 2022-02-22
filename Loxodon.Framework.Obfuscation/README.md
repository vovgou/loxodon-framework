![](docs/images/icon.png)

# Loxodon Framework Obfuscation

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![openupm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-obfuscation?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vovgou.loxodon-framework-obfuscation/)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-obfuscation)](https://www.npmjs.com/package/com.vovgou.loxodon-framework-obfucation)


*开发者 Clark*

要求Unity 2018.4 或者更高版本

数据类型内存混淆插件，支持ObfuscatedByte，ObfuscatedShort，ObfuscatedInt,ObfuscatedLong,ObfuscatedFloat,ObfuscatedDouble类型，防止内存修改器修改游戏数值，支持数值类型的所有运算符，与byte、short、int、long、float、double类型之间可以自动转换，使用时替换对应的数值类型即可。

Float和Double类型混淆时转为int和long类型进行与或运算，确保不会丢失精度，类型转换时使用unsafe代码，兼顾转换性能。

**注意：要求Unity2018以上版本，请开启"Allow unsafe Code"**

![](docs/images/obfuscation_unsafe.png)

### 使用示例：

    ObfuscatedInt  length = 200;
    ObfuscatedFloat scale = 20.5f;
    int offset = 30;

    float value = (length * scale) + offset;


## 安装

自Loxodon.Framework 2.0版本开始，保留了原有的 *.unitypackage包发布方式，同时添加了UPM发布方式，此版本要求Unity 2018.4.2及以上版本，框架的目录结构进行了一些调整，以符合UPM格式要求。

**安装注意：在中国区下载的Unity版本屏蔽了第三方仓库，会导致UPM包安装失败，咨询了Unity中国相关人员说是马上会放开，如果UPM方式安装失败请使用*.unitypackage文件安装或者使用非中国区的Unity版本**

### 使用 OpenUPM 安装(推荐)

[OpenUPM](https://openupm.com/) 是一个开源的UPM包仓库，它支持发布第三方的UPM包，它能够自动管理包的依赖关系，推荐使用它安装本框架.

通过openupm命令安装包,要求[nodejs](https://nodejs.org/en/download/) and openupm-cli客户端的支持，如果没有安装请先安装nodejs和open-cli。

    # 使用npm命令安装openupm-cli，如果已经安装请忽略.
    npm install -g openupm-cli

    #切换当前目录到项目的根目录
    cd F:/workspace/New Unity Project

    #安装 loxodon-framework-obfuscation
    openupm add com.vovgou.loxodon-framework-obfuscation

### 修改Packages/manifest.json文件安装

通过修改manifest.json文件安装，不需要安装nodejs和openupm-cli客户端。在Unity项目根目录下找到Packages/manifest.json文件，在文件的scopedRegistries（没有可以自己添加）节点下添加第三方仓库package.openupm.com的配置，同时在dependencies节点下添加com.vovgou.loxodon-framework-obfuscation的配置，保存后切换到Unity窗口即可完成安装。

    {
      "dependencies": {
        ...
        "com.unity.modules.xr": "1.0.0",
        "com.vovgou.loxodon-framework-obfuscation": "2.0.1"
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


### 通过git URL安装

Unity 2019.3.4f1及以上版本支持使用git URL安装. 如下图添加 https://github.com/vovgou/loxodon-framework.git?path=Loxodon.Framework.Obfuscation/Assets/LoxodonFramework/Obfuscation 地址到UPM管理器，耐性等待一段时间，下载完成后即安装成功。

![](docs/images/install_via_git.png)

### 通过 *.unitypackage 文件安装

从以下地址下载 [Loxodon.Framework.Obfuscation.unitypackage](https://github.com/vovgou/loxodon-framework/releases) 后,导入到你的项目中即完成安装.

- [Releases](https://github.com/vovgou/loxodon-framework/releases)

## 联系方式
邮箱: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
网站: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ群: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
