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