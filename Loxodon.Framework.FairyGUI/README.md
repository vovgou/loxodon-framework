![](docs/images/icon.png)

# Loxodon Framework FairyGUI

[![license](https://img.shields.io/github/license/vovgou/loxodon-framework?color=blue)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE) [![release](https://img.shields.io/github/v/tag/vovgou/loxodon-framework?label=release)](https://github.com/vovgou/loxodon-framework/releases)
[![npm](https://img.shields.io/npm/v/com.vovgou.loxodon-framework-fgui)](https://www.npmjs.com/package/com.vovgou.loxodon-framework-fgui)



*Developed by Clark*

这是一个支持FairyGUI的插件，安装此插件后可以对FairyGUI的UI控件进行数据绑定。

## 要求 ##

[Loxodon Framework](https://github.com/vovgou/loxodon-framework)

[FairyGUI](https://github.com/fairygui/FairyGUI-unity)

本项目作为Loxodon.Framework插件，必须在Loxodon.Framework环境下使用，请在安装使用前先安装Loxodon.Framework和FairyGUI。

## 快速开始 ##

    public class FairyGUIDatabindingExample : MonoBehaviour
    {
        public GButton button;
        public GTextInput textInput;

        protected virtual void Awake()
        {
            ApplicationContext context = Context.GetApplicationContext();
            //初始化数据绑定服务,如果使用XLua，则初始化 LuaBindingServiceBundle模块
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            //初始化支持FairyGUI的数据绑定相关组件，请在BindingServiceBundle启动后执行
            FairyGUIBindingServiceBundle fairyGUIBindingServiceBundle = new FairyGUIBindingServiceBundle(context.GetContainer());
            fairyGUIBindingServiceBundle.Start();
        }

        protected virtual void Start()
        {
            var bindingSet = this.CreateBindingSet<FairyGUIDatabindingExample, AccountViewModel>();

            bindingSet.Bind(this.textInput).For(v => v.text, v => v.onChanged).To(vm => vm.Username).OneWay();
            bindingSet.Bind(this.button).For(v => v.onClick).To(vm => vm.OnSubmit);

            bindingSet.Build();
        }
    }

## Contact Us
Email: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
Website: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ Group: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
