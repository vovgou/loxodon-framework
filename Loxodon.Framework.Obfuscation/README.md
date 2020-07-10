![](docs/icon.png)

# Loxodon Framework Obfuscation

[![license](https://img.shields.io/badge/license-MIT-blue.png)](https://github.com/vovgou/loxodon-framework/blob/master/LICENSE)
[![release](https://img.shields.io/badge/release-v1.0.0-blue.png)](https://github.com/vovgou/loxodon-framework/releases)


*开发者 Clark*

要求Unity 2019.3 或者更高版本

数据类型内存混淆插件，支持ObfuscatedByte，ObfuscatedShort，ObfuscatedInt,ObfuscatedLong,ObfuscatedFloat,ObfuscatedDouble类型，防止内存修改器修改游戏数值，支持数值类型的所有运算符，与byte、short、int、long、float、double类型之间可以自动转换，使用时替换对应的数值类型即可。

Float和Double类型混淆时转为int和long类型进行与或运算，确保不会丢失精度，类型转换时使用unsafe代码，兼顾转换性能。

**注意：要求Unity2018以上版本，请开启"Allow unsafe Code"**

![](docs/images/obfuscation_unsafe.png)

### 使用示例：

    ObfuscatedInt  length = 200;
    ObfuscatedFloat scale = 20.5f;
    int offset = 30;
    
    float value = (length * scale) + offset;


## 联系方式
邮箱: [yangpc.china@gmail.com](mailto:yangpc.china@gmail.com)   
网站: [https://vovgou.github.io/loxodon-framework/](https://vovgou.github.io/loxodon-framework/)  
QQ群: 622321589 [![](https://pub.idqqimg.com/wpa/images/group.png)](https:////shang.qq.com/wpa/qunwpa?idkey=71c1e43c24900ee84aeffc76fb67c0bacddc3f62a516fe80eae6b9521f872c59)
