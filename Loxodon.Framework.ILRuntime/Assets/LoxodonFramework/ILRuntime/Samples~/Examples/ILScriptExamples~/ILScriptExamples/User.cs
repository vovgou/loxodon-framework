using PropertyChanged;

namespace Loxodon.Framework.ILScriptExamples
{
    /// <summary>
    /// 使用Fody静态注入，简化编程，
    /// Fody会自动实现INotifyPropertyChanged接口注入属性修改通知事件
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName =>$"{FirstName} {LastName}";
    }
}
