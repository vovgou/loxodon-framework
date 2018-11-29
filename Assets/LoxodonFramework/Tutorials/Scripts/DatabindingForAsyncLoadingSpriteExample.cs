using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.ViewModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{

    public class SpriteViewModel : ViewModelBase
    {
        private string spriteName = "EquipImages_1";

        public string SpriteName
        {
            get { return this.spriteName; }
            set { this.Set<string>(ref spriteName, value, "SpriteName"); }
        }

        public void ChangeSpriteName()
        {
            this.SpriteName = string.Format("EquipImages_{0}", Random.Range(1, 30));
        }
    }

    public class DatabindingForAsyncLoadingSpriteExample : MonoBehaviour
    {
        public Button changeSpriteButton;

        public AsyncSpriteLoader spriteLoader;

        void Awake()
        {
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();
        }

        void Start()
        {
            var viewModel = new SpriteViewModel();

            IBindingContext bindingContext = this.BindingContext();
            bindingContext.DataContext = viewModel;

            BindingSet<DatabindingForAsyncLoadingSpriteExample, SpriteViewModel> bindingSet = this.CreateBindingSet<DatabindingForAsyncLoadingSpriteExample, SpriteViewModel>();
            bindingSet.Bind(this.spriteLoader).For(v => v.SpriteName).To(vm => vm.SpriteName).OneWay();

            bindingSet.Bind(this.changeSpriteButton).For(v => v.onClick).To(vm => vm.ChangeSpriteName());

            bindingSet.Build();
        }
    }
}
