using AiForms.Dialogs.Abstractions;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Sample.Views;
using Xamarin.Forms;

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
            //Configurations.LoadingConfig.RegisterView<MyIndicatorView>(new {
            //    Message="Loading...",VAlign=LayoutAlignment.Start,HAlgin= LayoutAlignment.Center,
            //    OffsetX=0,OffsetY=0
            //});

			NavigationService.NavigateAsync("MyTabbedPage?createTab=NavigationPage|MainPage");
		}

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<MyTabbedPage>();
            containerRegistry.Register<MyIndicatorView>();

        }
    }
}

