/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Services;
using Loxodon.Framework.Views.TextMeshPro;
using System;
using UnityEngine;

namespace Loxodon.Framework.Tutorials
{
    public class FormattableTextMeshProUGUIExample : MonoBehaviour
    {
        public FormattableTextMeshProUGUI paramBinding1;//参数绑定示例1，支持1-4个不同参数
        public GenericParameters<DateTime, int> paramBinding2;//参数绑定的另外一种方式，支持1-4个不同参数
        public FormattableTextMeshProUGUI arrayBinding;//也可以使用 ArrayParameters<float>
        public TemplateTextMeshProUGUI template;//模版绑定

        private ExampleViewModel viewModel;

        private void Start()
        {
            ApplicationContext context = Context.GetApplicationContext();
            IServiceContainer container = context.GetContainer();
            BindingServiceBundle bundle = new BindingServiceBundle(context.GetContainer());
            bundle.Start();
            
            BindingSet<FormattableTextMeshProUGUIExample, ExampleViewModel> bindingSet = this.CreateBindingSet<FormattableTextMeshProUGUIExample, ExampleViewModel>();

            //使用AsParameters<P1,P2,...>() 函数创建一个参数集合，然后绑定，支持1-4个参数，没有值对象的装箱拆箱，没有字符串拼接，无GC（请在手机上测试，Editor下需要修改TMP_Text.SetText和TMP_Text.StringBuilderToIntArray的源码，关闭调试代码）
            //format:格式与string.Format()的格式化参数相同如：DateTime:Example1,{0:yyyy-MM-dd HH:mm:ss}, FrameCount:{1}
            bindingSet.Bind(paramBinding1.AsParameters<DateTime, int>()).For(v => v.Parameter1).To(vm => vm.Time);
            bindingSet.Bind(paramBinding1.AsParameters<DateTime, int>()).For(v => v.Parameter2).To(vm => vm.FrameCount);

            //本质上与上面的例子是相同的，只是另外一种用法
            //format:Example2,{0:yyyy-MM-dd HH:mm:ss}, FrameCount:{1}
            bindingSet.Bind(paramBinding2).For(v => v.Parameter1).To(vm => vm.Time);
            bindingSet.Bind(paramBinding2).For(v => v.Parameter2).To(vm => vm.FrameCount);

            //使用AsArray<T>() 获得一个数组然后进行绑定，支持多个类型相同的参数，没有值对象的装箱拆箱，没有字符串拼接，无GC（请在手机上测试，Editor下需要修改TMP_Text.SetText和TMP_Text.StringBuilderToIntArray的源码，关闭调试代码）
            //format:MoveSpeed:{0:f4}  AttackSpeed:{1:f2}
            bindingSet.Bind(arrayBinding.AsArray<float>()).For(v => v[0]).To(vm => vm.Hero.MoveSpeed);
            bindingSet.Bind(arrayBinding.AsArray<float>()).For(v => v[1]).To(vm => vm.Hero.AttackSpeed);

            //使用文本模版（TemplateTextMeshProUGUI）绑定，直接将一个对象绑定到模板的Data属性上即可。
            //文本模版格式与string.Format类似，仅需要将{0},{1}中的数字，替换为对象属性名即可
            //template text：当前时间：{Time:yyyy-MM-dd HH:mm:ss} 
            bindingSet.Bind(template).For(v => v.Template).To(vm => vm.Template);//模版可以绑定，也可以在编辑器上配置
            bindingSet.Bind(template).For(v => v.Data).To(vm => vm);
            bindingSet.Build();

            this.viewModel = new ExampleViewModel();
            this.viewModel.Template = "Template,Frame:{FrameCount:D6},Health:{Hero.Health:D4} AttackDamage:{Hero.AttackDamage} Armor:{Hero.Armor}";
            this.viewModel.Time = DateTime.Now;
            this.viewModel.TimeSpan = TimeSpan.FromSeconds(0);
            this.viewModel.Hero = new Hero();
            this.SetDataContext(this.viewModel);
        }

        void Update()
        {
            viewModel.Time = DateTime.Now;
            viewModel.FrameCount = Time.frameCount;
            viewModel.Hero.Health = (Time.frameCount % 1000) / 10;
        }
    }

    public class ExampleViewModel : ObservableObject
    {
        private DateTime time;
        private TimeSpan timeSpan;
        private string template;
        private int frameCount;
        private Hero hero;
        public DateTime Time
        {
            get { return this.time; }
            set { this.Set(ref time, value); }
        }

        public TimeSpan TimeSpan
        {
            get { return this.timeSpan; }
            set { this.Set(ref timeSpan, value); }
        }

        public int FrameCount
        {
            get { return this.frameCount; }
            set { this.Set(ref frameCount, value); }
        }

        public string Template
        {
            get { return this.template; }
            set { this.Set(ref template, value); }
        }

        public Hero Hero
        {
            get { return this.hero; }
            set { this.Set(ref hero, value); }
        }
    }

    public class Hero : ObservableObject
    {
        private float attackSpeed = 95.5f;
        private float moveSpeed = 2.4f;
        private int health = 100;
        private int attackDamage = 20;
        private int armor = 30;

        public float AttackSpeed
        {
            get { return this.attackSpeed; }
            set { this.Set(ref attackSpeed, value); }
        }

        public float MoveSpeed
        {
            get { return this.moveSpeed; }
            set { this.Set(ref moveSpeed, value); }
        }

        public int Health
        {
            get { return this.health; }
            set { this.Set(ref health, value); }
        }

        public int AttackDamage
        {
            get { return this.attackDamage; }
            set { this.Set(ref attackDamage, value); }
        }

        public int Armor
        {
            get { return this.armor; }
            set { this.Set(ref armor, value); }
        }

    }
}