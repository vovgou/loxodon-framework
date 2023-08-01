![](docs/images/icon.png)

# Loxodon Framework TextMeshPro

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![openupm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-connection?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vovgou.loxodon-framework-textmeshpro/)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-textmeshpro)](https://www.npmjs.com/package/com.vovgou.loxodon-framework-textmeshpro)


*Developed by Clark*

Requires Unity 2018.4 or higher.

Provides the implementation of TextMeshPro for AlertDialog and Toast.

## Installation

### Install via OpenUPM (recommended)

[OpenUPM](https://openupm.com/) can automatically manage dependencies, it is recommended to use it to install the framework.

Requires [nodejs](https://nodejs.org/en/download/)'s npm and openupm-cli, if not installed please install them first.

    # Install openupm-cli,please ignore if it is already installed.
    npm install -g openupm-cli

    #Go to the root directory of your project
    cd F:/workspace/New Unity Project

    #Install loxodon-framework-textmeshpro
    openupm add com.vovgou.loxodon-framework-textmeshpro

### Install via Packages/manifest.json

Modify the Packages/manifest.json file in your project, add the third-party repository "package.openupm.com"'s configuration and add "com.vovgou.loxodon-framework-textmeshpro" in the "dependencies" node.

Installing the framework in this way does not require nodejs and openm-cli.

    {
      "dependencies": {
        ...
        "com.vovgou.loxodon-framework-textmeshpro": "2.6.0"
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

- Loxodon.Framework.TextMeshPro: https://github.com/vovgou/loxodon-framework.git?path=Loxodon.Framework.TextMeshPro/Assets/LoxodonFramework/TextMeshPro


![](docs/images/install_via_git.png)

### Install via *.unitypackage file

Download Loxodon.Framework.TextMeshPro.unitypackage, import them into your project.

- [Releases](https://github.com/vovgou/loxodon-framework/releases)

## Contact Us
Email: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
Website: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ Group: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
