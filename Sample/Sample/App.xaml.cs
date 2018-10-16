using System.Linq;
using System.Reflection;
using AiForms.Dialogs.Abstractions;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Sample.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Sample
{
	public partial class App : PrismApplication
	{
		public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
		{
			InitializeComponent();

            Configurations.LoadingConfig.IndicatorColor = Color.Accent;
            Configurations.LoadingConfig.OverlayColor = Color.FromRgba(255,255,255,0.7);
            Configurations.LoadingConfig.Opacity = 0.8f;
            Configurations.LoadingConfig.FontColor = Color.Black;
            Configurations.LoadingConfig.OffsetY = 50;
            Configurations.LoadingConfig.OffsetX = 50;
            Configurations.LoadingConfig.IsReusable = false;
            Configurations.LoadingConfig.DefaultMessage = "Loading...";

            //NavigationService.NavigateAsync("MyTabbedPage?createTab=NavigationPage|MainPage");
            NavigationService.NavigateAsync("NavigationPage/IndexPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            
            this.GetType().GetTypeInfo().Assembly
            .DefinedTypes
            .Where(t => t.Namespace?.EndsWith(".Views", System.StringComparison.Ordinal) ?? false)
            .ForEach(t => {
                containerRegistry.RegisterForNavigation(t.AsType(), t.Name);
            });

        }
    }
}

