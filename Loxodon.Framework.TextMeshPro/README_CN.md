![](docs/images/icon.png)

# Loxodon Framework TextMeshPro

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![openupm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-textmeshpro?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vovgou.loxodon-framework-textmeshpro/)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-textmeshpro)](https://www.npmjs.com/package/com.vovgou.loxodon-framework-textmeshpro)


*开发者 Clark*

要求Unity 2018.4 或者更高版本

将AlertDialog和Toast视图中的Text替换为TextMeshPro。


## 安装

### 使用 OpenUPM 安装(推荐)

[OpenUPM](https://openupm.com/) 是一个开源的UPM包仓库，它支持发布第三方的UPM包，它能够自动管理包的依赖关系，推荐使用它安装本框架.

通过openupm命令安装包,要求[nodejs](https://nodejs.org/en/download/) and openupm-cli客户端的支持，如果没有安装请先安装nodejs和open-cli。

    # 使用npm命令安装openupm-cli，如果已经安装请忽略.
    npm install -g openupm-cli

    #切换当前目录到项目的根目录
    cd F:/workspace/New Unity Project

    #安装 loxodon-framework-textmeshpro
    openupm add com.vovgou.loxodon-framework-textmeshpro

### 修改Packages/manifest.json文件安装

通过修改manifest.json文件安装，不需要安装nodejs和openupm-cli客户端。在Unity项目根目录下找到Packages/manifest.json文件，在文件的scopedRegistries（没有可以自己添加）节点下添加第三方仓库package.openupm.com的配置，同时在dependencies节点下添加com.vovgou.loxodon-framework-textmeshpro的配置，保存后切换到Unity窗口即可完成安装。

    {
      "dependencies": {
        ...
        "com.unity.modules.xr": "1.0.0",
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


### 通过git URL安装

Unity 2019.3.4f1及以上版本支持使用git URL安装. 如下图添加 https://github.com/vovgou/loxodon-framework.git?path=Loxodon.Framework.TextMeshPro/Assets/LoxodonFramework/TextMeshPro 地址到UPM管理器，耐性等待一段时间，下载完成后即安装成功。

![](docs/images/install_via_git.png)

### 通过 *.unitypackage 文件安装

从以下地址下载 [Loxodon.Framework.TextMeshPro.unitypackage](https://github.com/vovgou/loxodon-framework/releases) 后,导入到你的项目中即完成安装.

- [Releases](https://github.com/vovgou/loxodon-framework/releases)

## 联系方式
邮箱: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
网站: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ群: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
